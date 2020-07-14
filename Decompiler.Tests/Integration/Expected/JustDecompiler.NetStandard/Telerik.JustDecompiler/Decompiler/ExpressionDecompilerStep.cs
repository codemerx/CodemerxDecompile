using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Decompiler.TypeInference;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class ExpressionDecompilerStep : BaseInstructionVisitor, IDecompilationStep
	{
		private Stack<Expression> expressionStack = new Stack<Expression>();

		private readonly Dictionary<int, Expression> offsetToExpression;

		private int currentOffset;

		private readonly HashSet<Expression> used;

		private DecompilationContext context;

		private MethodSpecificContext methodContext;

		private TypeSystem currentTypeSystem;

		private TypeSpecificContext typeContext;

		private readonly ExpressionDecompilerData results;

		private readonly Dictionary<VariableReference, KeyValuePair<int, bool>> exceptionVariables;

		private int dummyVarCounter;

		private readonly HashSet<VariableDefinition> stackVariableAssignmentsToRemove = new HashSet<VariableDefinition>();

		private InstructionBlock currentBlock;

		private Instruction CurrentInstruction
		{
			get
			{
				return this.methodContext.ControlFlowGraph.OffsetToInstruction[this.currentOffset];
			}
		}

		public ExpressionDecompilerStep()
		{
			this.offsetToExpression = new Dictionary<int, Expression>();
			this.results = new ExpressionDecompilerData();
			this.used = new HashSet<Expression>();
			this.exceptionVariables = new Dictionary<VariableReference, KeyValuePair<int, bool>>();
		}

		private void AddDupInstructionsToMapping(InstructionBlock instructionBlock, Instruction instruction, Expression expression)
		{
			if (instruction.get_OpCode().get_Code() != 37)
			{
				List<Instruction> instructions = new List<Instruction>();
				Instruction next = instruction;
				while ((object)next != (object)instructionBlock.Last && next.get_Next().get_OpCode().get_Code() == 36)
				{
					next = next.get_Next();
					instructions.Add(next);
				}
				if (instructions.Count > 0)
				{
					expression.MapDupInstructions(instructions);
				}
			}
		}

		private static void AddRange<T>(IList<T> list, IEnumerable<T> range)
		{
			foreach (T t in range)
			{
				list.Add(t);
			}
		}

		private void AddUninlinedStackVariablesToContext()
		{
			foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> variableToDefineUseInfo in this.methodContext.StackData.VariableToDefineUseInfo)
			{
				if (variableToDefineUseInfo.Value.DefinedAt.Count <= 0)
				{
					continue;
				}
				this.methodContext.Variables.Add(variableToDefineUseInfo.Key);
				this.methodContext.VariablesToRename.Add(variableToDefineUseInfo.Key);
			}
		}

		public bool CheckForCastBecauseForeach(Expression expression)
		{
			ExplicitCastExpression explicitCastExpression = expression as ExplicitCastExpression;
			if (explicitCastExpression != null && explicitCastExpression.Expression is MethodInvocationExpression)
			{
				return true;
			}
			return false;
		}

		private ArrayCreationExpression ConvertObjectToArray(ObjectCreationExpression objectCreation)
		{
			TypeReference elementTypeFromCtor = this.GetElementTypeFromCtor(objectCreation.Constructor);
			if (elementTypeFromCtor == null)
			{
				return null;
			}
			return new ArrayCreationExpression(elementTypeFromCtor, null, objectCreation.MappedInstructions)
			{
				Dimensions = this.CopyExpressionCollection(objectCreation.Arguments)
			};
		}

		private ExpressionCollection CopyExpressionCollection(ExpressionCollection arguments)
		{
			ExpressionCollection expressionCollection = new ExpressionCollection();
			foreach (Expression argument in arguments)
			{
				expressionCollection.Add(argument);
			}
			return expressionCollection;
		}

		private void CreateExpressions()
		{
			VariableDefinition variableDefinition;
			TypeReference expressionType;
			InstructionBlock[] blocks = this.methodContext.ControlFlowGraph.Blocks;
			for (int i = 0; i < (int)blocks.Length; i++)
			{
				InstructionBlock instructionBlocks = blocks[i];
				this.currentBlock = instructionBlocks;
				List<Expression> expressions = new List<Expression>();
				foreach (Instruction instruction in instructionBlocks)
				{
					this.currentOffset = instruction.get_Offset();
					this.SetInitialStack(instruction);
					base.Visit(instruction);
					if (this.expressionStack.Count == 0)
					{
						continue;
					}
					this.offsetToExpression.Add(instruction.get_Offset(), this.expressionStack.Peek());
					if (instruction.get_OpCode().get_Code() != 37 && this.methodContext.StackData.InstructionOffsetToAssignedVariableMap.TryGetValue(instruction.get_Offset(), out variableDefinition))
					{
						Expression expression = this.expressionStack.Pop();
						if (this.methodContext.StackData.VariableToDefineUseInfo[variableDefinition].DefinedAt.Count == 1)
						{
							expressionType = expression.ExpressionType;
						}
						else
						{
							expressionType = null;
						}
						variableDefinition.set_VariableType(expressionType);
						this.expressionStack.Push(new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(variableDefinition, null), expression, this.currentTypeSystem, null, false));
						this.AddDupInstructionsToMapping(instructionBlocks, instruction, expression);
					}
					expressions.Add(this.expressionStack.Pop());
				}
				if (this.methodContext.YieldData != null)
				{
					this.ProcessYieldBlock(instructionBlocks, expressions);
				}
				this.results.BlockExpressions.Add(instructionBlocks.First.get_Offset(), expressions);
			}
			foreach (IList<Expression> value in this.results.BlockExpressions.Values)
			{
				for (int j = 0; j < value.Count; j++)
				{
					BinaryExpression item = value[j] as BinaryExpression;
					if (item != null && item.IsAssignmentExpression && item.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression && this.stackVariableAssignmentsToRemove.Contains((item.Left as VariableReferenceExpression).Variable.Resolve()))
					{
						value.RemoveAt(j);
					}
				}
			}
			foreach (KeyValuePair<int, VariableDefinition> exceptionHandlerStartToExceptionVariableMap in this.methodContext.StackData.ExceptionHandlerStartToExceptionVariableMap)
			{
				this.results.ExceptionHandlerStartToVariable.Add(exceptionHandlerStartToExceptionVariableMap.Key, new VariableReferenceExpression(exceptionHandlerStartToExceptionVariableMap.Value, null));
			}
		}

		private Expression FixCallTarget(Instruction instruction, Expression target)
		{
			if (target != null)
			{
				if (this.IsPointerType(target))
				{
					target = new UnaryExpression(UnaryOperator.AddressDereference, target, null);
				}
				if (target.HasType && target.ExpressionType != null)
				{
					MethodReference operand = (MethodReference)instruction.get_Operand();
					MethodDefinition methodDefinition = operand.Resolve();
					if (operand.get_DeclaringType() != null && methodDefinition != null && methodDefinition.get_DeclaringType() != null && methodDefinition.get_DeclaringType().get_IsInterface())
					{
						TypeDefinition typeDefinition = target.ExpressionType.Resolve();
						if (typeDefinition != null && (object)typeDefinition != (object)methodDefinition.get_DeclaringType() && !typeDefinition.get_IsInterface())
						{
							return new ExplicitCastExpression(target, operand.get_DeclaringType(), null)
							{
								IsExplicitInterfaceCast = true
							};
						}
					}
				}
			}
			return target;
		}

		private ArrayCreationExpression GetClosestArrayCreationExpression()
		{
			for (int i = this.currentOffset; i >= 0; i--)
			{
				if (this.offsetToExpression.ContainsKey(i) && this.offsetToExpression[i] is ArrayCreationExpression)
				{
					return this.offsetToExpression[i] as ArrayCreationExpression;
				}
			}
			return null;
		}

		private TypeReference GetElementTypeFromCtor(MethodReference constructor)
		{
			ArrayType declaringType = constructor.get_DeclaringType() as ArrayType;
			if (declaringType == null)
			{
				return null;
			}
			return declaringType.get_ElementType();
		}

		private int GetYieldReturnAssignmentIndex(List<Expression> blockExpressions)
		{
			FieldReference currentItemField = this.methodContext.YieldData.FieldsInfo.CurrentItemField;
			int num = 0;
			foreach (Expression blockExpression in blockExpressions)
			{
				if (blockExpression.CodeNodeType != CodeNodeType.BinaryExpression || !(blockExpression as BinaryExpression).IsAssignmentExpression)
				{
					num++;
				}
				else
				{
					FieldReferenceExpression left = (blockExpression as BinaryExpression).Left as FieldReferenceExpression;
					if (left == null || left.Field.Resolve() != currentItemField)
					{
						num++;
					}
					else
					{
						if (num == blockExpressions.Count)
						{
							throw new Exception("No assignment of the current field");
						}
						return num;
					}
				}
			}
			if (num == blockExpressions.Count)
			{
				throw new Exception("No assignment of the current field");
			}
			return num;
		}

		private Expression GetYieldReturnItem(List<Expression> blockExpressions)
		{
			Expression right;
			FieldReference currentItemField = this.methodContext.YieldData.FieldsInfo.CurrentItemField;
			List<Expression>.Enumerator enumerator = blockExpressions.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Expression current = enumerator.Current;
					if (current.CodeNodeType != CodeNodeType.BinaryExpression || !(current as BinaryExpression).IsAssignmentExpression)
					{
						continue;
					}
					FieldReferenceExpression left = (current as BinaryExpression).Left as FieldReferenceExpression;
					if (left == null || left.Field.Resolve() != currentItemField)
					{
						continue;
					}
					right = (current as BinaryExpression).Right;
					return right;
				}
				throw new Exception("No assignment of the current field");
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return right;
		}

		private TypeReference Import(Type type)
		{
			return Utilities.GetCorlibTypeReference(type, this.methodContext.Method.get_Module());
		}

		private IEnumerable<Instruction> IncludePrefixIfPresent(Instruction instruction, Code prefixCode)
		{
			if (instruction.get_Previous() == null || instruction.get_Previous().get_OpCode().get_Code() != prefixCode)
			{
				return new Instruction[] { instruction };
			}
			return new Instruction[] { instruction.get_Previous(), instruction };
		}

		private bool IsIntegerType(TypeReference expressionType)
		{
			string fullName = expressionType.get_FullName();
			if (!(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_Byte().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_SByte().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_Int16().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_UInt16().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_Int32().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_UInt16().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_Int64().get_FullName()) && !(fullName == this.methodContext.Method.get_Module().get_TypeSystem().get_UInt64().get_FullName()))
			{
				return false;
			}
			return true;
		}

		private bool IsPointerType(Expression expression)
		{
			if (!expression.HasType)
			{
				return false;
			}
			TypeReference expressionType = expression.ExpressionType;
			if (expressionType == null)
			{
				return false;
			}
			if (expressionType.get_IsPointer())
			{
				return true;
			}
			if (!expressionType.get_IsPinned())
			{
				if (expressionType.get_IsByReference())
				{
					return true;
				}
				return false;
			}
			PinnedType pinnedType = expressionType as PinnedType;
			if (pinnedType.get_ElementType().get_IsByReference())
			{
				return true;
			}
			return pinnedType.get_ElementType().get_IsPointer();
		}

		private bool IsUnmanagedPointerType(Expression ex)
		{
			if (ex == null)
			{
				return false;
			}
			if (!ex.HasType)
			{
				return false;
			}
			if (ex.ExpressionType.get_IsPointer())
			{
				return true;
			}
			return false;
		}

		public override void OnAdd(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Add, instruction);
		}

		public override void OnAdd_Ovf(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Add, instruction);
		}

		public override void OnAdd_Ovf_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Add, instruction);
		}

		public override void OnAnd(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.BitwiseAnd, instruction);
		}

		public override void OnArglist(Instruction instruction)
		{
			TypeReference typeReference = this.Import(typeof(RuntimeArgumentHandle));
			ParameterDefinition parameterDefinition = new ParameterDefinition("__arglist", 0xcfe0, typeReference);
			this.methodContext.Method.get_Parameters().Add(parameterDefinition);
			this.PushArgumentReference(parameterDefinition);
		}

		public override void OnBeq(Instruction instruction)
		{
			this.OnCeq(instruction);
		}

		public override void OnBge(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.GreaterThanOrEqual, instruction);
		}

		public override void OnBge_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.GreaterThanOrEqual, instruction);
		}

		public override void OnBgt(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
		}

		public override void OnBgt_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
		}

		public override void OnBle(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LessThanOrEqual, instruction);
		}

		public override void OnBle_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LessThanOrEqual, instruction);
		}

		public override void OnBlt(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LessThan, instruction);
		}

		public override void OnBlt_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LessThan, instruction);
		}

		public override void OnBne_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.ValueInequality, instruction);
		}

		public override void OnBox(Instruction instruction)
		{
			Expression explicitCastExpression = this.Pop();
			TypeReference operand = (TypeReference)instruction.get_Operand();
			if (explicitCastExpression.ExpressionType != null && explicitCastExpression.ExpressionType.GetFriendlyFullName(null) != operand.GetFriendlyFullName(null))
			{
				explicitCastExpression = new ExplicitCastExpression(explicitCastExpression, operand, null);
			}
			else if (explicitCastExpression.ExpressionType == null && explicitCastExpression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				(explicitCastExpression as VariableReferenceExpression).Variable.set_VariableType(operand);
			}
			explicitCastExpression = new BoxExpression(explicitCastExpression, this.methodContext.Method.get_Module().get_TypeSystem(), (TypeReference)instruction.get_Operand(), new Instruction[] { instruction });
			this.Push(explicitCastExpression);
		}

		public override void OnBr(Instruction instruction)
		{
		}

		public override void OnBreak(Instruction instruction)
		{
		}

		public override void OnBrfalse(Instruction instruction)
		{
		}

		public override void OnBrtrue(Instruction instruction)
		{
		}

		public override void OnCall(Instruction instruction)
		{
			int num;
			MethodInvocationExpression baseCtorExpression;
			Expression expression;
			TypeReference elementType;
			Expression expression1;
			MethodReference operand = (MethodReference)instruction.get_Operand();
			ExpressionCollection expressionCollection = this.ProcessArguments(operand);
			if (operand.get_Name() == null || !(operand.get_Name() == ".ctor"))
			{
				if (operand.get_HasThis())
				{
					expression = this.Pop();
				}
				else
				{
					expression = null;
				}
				Expression expression2 = this.FixCallTarget(instruction, expression);
				MethodInvocationExpression methodInvocationExpression = new MethodInvocationExpression(new MethodReferenceExpression(expression2, (MethodReference)instruction.get_Operand(), null), this.IncludePrefixIfPresent(instruction, 211));
				ExpressionDecompilerStep.AddRange<Expression>(methodInvocationExpression.Arguments, expressionCollection);
				if (!this.TryProcessRuntimeHelpersInitArray(methodInvocationExpression) && !this.TryProcessMultidimensionalIndexing(methodInvocationExpression))
				{
					methodInvocationExpression.VirtualCall = false;
					this.Push(methodInvocationExpression);
					if (Utilities.IsComputeStringHashMethod(instruction.get_Operand() as MethodReference))
					{
						this.context.MethodContext.SwitchByStringData.SwitchBlocksStartInstructions.Add(this.currentBlock.First.get_Offset());
					}
				}
				return;
			}
			TypeReference typeReference = operand.get_DeclaringType().GetElementType();
			if (this.methodContext.Method.get_DeclaringType().get_BaseType() != null)
			{
				elementType = this.methodContext.Method.get_DeclaringType().get_BaseType().GetElementType();
			}
			else
			{
				elementType = null;
			}
			TypeReference typeReference1 = elementType;
			TypeReference elementType1 = this.methodContext.Method.get_DeclaringType().GetElementType();
			if (!this.methodContext.Method.get_IsConstructor() || (typeReference1 == null || !(typeReference1.GetFriendlyFullName(null) == typeReference.GetFriendlyFullName(null))) && !(elementType1.GetFriendlyFullName(null) == typeReference.GetFriendlyFullName(null)) || instruction.get_Next().IsStoreRegister(out num))
			{
				Expression expression3 = this.ProcessConstructor(instruction, expressionCollection);
				this.PushAssignment(new UnaryExpression(UnaryOperator.AddressDereference, this.Pop(), null), expression3, null);
				return;
			}
			if (operand.get_HasThis())
			{
				expression1 = this.Pop();
			}
			else
			{
				expression1 = null;
			}
			Expression expression4 = expression1;
			expression4 = this.FixCallTarget(instruction, expression4);
			MethodReferenceExpression methodReferenceExpression = new MethodReferenceExpression(expression4, (MethodReference)instruction.get_Operand(), null);
			if (elementType1.GetFriendlyFullName(null) != typeReference.GetFriendlyFullName(null))
			{
				baseCtorExpression = new BaseCtorExpression(methodReferenceExpression, this.IncludePrefixIfPresent(instruction, 211))
				{
					InstanceReference = expression4
				};
			}
			else
			{
				baseCtorExpression = new ThisCtorExpression(methodReferenceExpression, this.IncludePrefixIfPresent(instruction, 211))
				{
					InstanceReference = expression4
				};
			}
			ExpressionDecompilerStep.AddRange<Expression>(baseCtorExpression.Arguments, expressionCollection);
			this.Push(baseCtorExpression);
		}

		public override void OnCalli(Instruction instruction)
		{
			Expression expression;
			if (!this.TryGetVariableValueAndMarkForRemoval(this.Pop() as VariableReferenceExpression, out expression) || expression.CodeNodeType != CodeNodeType.MethodReferenceExpression)
			{
				throw new DecompilationException("Method pointer cannot be resolved to a method definition.");
			}
			MethodReferenceExpression methodReferenceExpression = expression as MethodReferenceExpression;
			ExpressionCollection expressionCollection = this.ProcessArguments(methodReferenceExpression.Method);
			if (methodReferenceExpression.Method.get_HasThis())
			{
				methodReferenceExpression.Target = this.Pop();
			}
			MethodInvocationExpression methodInvocationExpression = new MethodInvocationExpression(methodReferenceExpression, new Instruction[] { instruction })
			{
				Arguments = expressionCollection
			};
			this.Push(methodInvocationExpression);
		}

		public override void OnCallvirt(Instruction instruction)
		{
			this.OnCall(instruction);
			Expression expression = this.Peek();
			if (expression != null && expression.CodeNodeType == CodeNodeType.MethodInvocationExpression)
			{
				MethodInvocationExpression operand = expression as MethodInvocationExpression;
				operand.VirtualCall = true;
				if (instruction.get_Previous() != null && instruction.get_Previous().get_OpCode().get_Code() == 211 && instruction.get_Previous().get_Operand() != null)
				{
					operand.ConstraintType = instruction.get_Previous().get_Operand() as TypeReference;
				}
			}
		}

		public override void OnCastclass(Instruction instruction)
		{
			this.PushCastExpression((TypeReference)instruction.get_Operand(), instruction);
		}

		public override void OnCeq(Instruction instruction)
		{
			Expression expression = this.Pop();
			Expression expression1 = this.Pop();
			BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.ValueEquality, expression1, expression, this.currentTypeSystem, new Instruction[] { instruction }, false)
			{
				ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean()
			};
			this.Push(binaryExpression);
		}

		public override void OnCgt(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
		}

		public override void OnCgt_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
		}

		public override void OnClt(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LessThan, instruction);
		}

		public override void OnClt_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LessThan, instruction);
		}

		public override void OnConstrained(Instruction instruction)
		{
		}

		public override void OnConv_I(Instruction instruction)
		{
			Expression expression = this.expressionStack.Peek();
			if (this.IsPointerType(expression) || expression.CodeNodeType == CodeNodeType.UnaryExpression && (expression as UnaryExpression).Operator == UnaryOperator.AddressOf)
			{
				this.Push(this.Pop());
				return;
			}
			Expression expression1 = this.Pop();
			if (expression1.HasType && (expression1.ExpressionType.get_FullName() == this.methodContext.Method.get_Module().get_TypeSystem().get_Int32().get_FullName() || expression1.ExpressionType.get_FullName() == this.methodContext.Method.get_Module().get_TypeSystem().get_UInt32().get_FullName()))
			{
				this.Push(expression1);
				return;
			}
			if (!expression1.HasType || !this.IsIntegerType(expression1.ExpressionType))
			{
				this.Push(new UnaryExpression(UnaryOperator.AddressOf, expression1, new Instruction[] { instruction }));
				return;
			}
			this.Push(new ExplicitCastExpression(expression1, new PointerType(this.methodContext.Method.get_Module().get_TypeSystem().get_Void()), new Instruction[] { instruction }));
		}

		public override void OnConv_I1(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_SByte(), instruction);
		}

		public override void OnConv_I2(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int16(), instruction);
		}

		public override void OnConv_I4(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int32(), instruction);
		}

		public override void OnConv_I8(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int64(), instruction);
		}

		public override void OnConv_Ovf_I(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_IntPtr(), instruction);
		}

		public override void OnConv_Ovf_I_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_IntPtr(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_I1(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_SByte(), instruction);
		}

		public override void OnConv_Ovf_I1_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_SByte(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_I2(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int16(), instruction);
		}

		public override void OnConv_Ovf_I2_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int16(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_I4(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int32(), instruction);
		}

		public override void OnConv_Ovf_I4_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int32(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_I8(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int64(), instruction);
		}

		public override void OnConv_Ovf_I8_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Int64(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_U(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UIntPtr(), instruction);
		}

		public override void OnConv_Ovf_U_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UIntPtr(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_U1(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Byte(), instruction);
		}

		public override void OnConv_Ovf_U1_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Byte(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_U2(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt16(), instruction);
		}

		public override void OnConv_Ovf_U2_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt16(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_U4(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt32(), instruction);
		}

		public override void OnConv_Ovf_U4_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt32(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_Ovf_U8(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt64(), instruction);
		}

		public override void OnConv_Ovf_U8_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt64(), instruction);
			this.PushCastExpression(new PointerType(this.currentTypeSystem.get_Void()), null);
		}

		public override void OnConv_R_Un(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Single(), instruction);
		}

		public override void OnConv_R4(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Single(), instruction);
		}

		public override void OnConv_R8(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Double(), instruction);
		}

		public override void OnConv_U(Instruction instruction)
		{
			this.OnConv_I(instruction);
		}

		public override void OnConv_U1(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_Byte(), instruction);
		}

		public override void OnConv_U2(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt16(), instruction);
		}

		public override void OnConv_U4(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt32(), instruction);
		}

		public override void OnConv_U8(Instruction instruction)
		{
			this.PushCastExpression(this.methodContext.Method.get_Module().get_TypeSystem().get_UInt64(), instruction);
		}

		public override void OnCpobj(Instruction instruction)
		{
			this.OnLdobj(null);
			this.OnStobj(instruction);
		}

		public override void OnDiv(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Divide, instruction);
		}

		public override void OnDiv_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Divide, instruction);
		}

		public override void OnDup(Instruction instruction)
		{
		}

		public override void OnEndfilter(Instruction instruction)
		{
		}

		public override void OnEndfinally(Instruction instruction)
		{
		}

		public override void OnInitblk(Instruction instruction)
		{
		}

		public override void OnInitobj(Instruction instruction)
		{
			Expression unaryExpression = new UnaryExpression(UnaryOperator.AddressDereference, this.Pop(), null);
			TypeReference operand = (TypeReference)instruction.get_Operand();
			Expression objectCreationExpression = new ObjectCreationExpression(null, operand, null, null);
			if (operand.get_IsGenericInstance())
			{
				if (operand.GetFriendlyFullName(null).IndexOf("System.Nullable<") >= 0)
				{
					objectCreationExpression = new LiteralExpression(null, this.currentTypeSystem, null);
				}
			}
			else if (operand.get_IsGenericParameter())
			{
				objectCreationExpression = new DefaultObjectExpression((TypeReference)instruction.get_Operand(), null);
			}
			this.PushAssignment(unaryExpression, objectCreationExpression, new Instruction[] { instruction });
		}

		public override void OnIsinst(Instruction instruction)
		{
			this.Push(new SafeCastExpression(this.Pop(), (TypeReference)instruction.get_Operand(), new Instruction[] { instruction }));
		}

		public override void OnLdarg(Instruction instruction)
		{
			ParameterDefinition operand = (ParameterDefinition)instruction.get_Operand();
			if ((object)operand != (object)this.methodContext.Method.get_Body().get_ThisParameter())
			{
				this.PushArgumentReference(operand);
				return;
			}
			this.Push(new ThisReferenceExpression(this.methodContext.Method.get_DeclaringType(), new Instruction[] { instruction }));
		}

		public override void OnLdarg_0(Instruction instruction)
		{
			this.PushArgumentReference(0, instruction);
		}

		public override void OnLdarg_1(Instruction instruction)
		{
			this.PushArgumentReference(1, instruction);
		}

		public override void OnLdarg_2(Instruction instruction)
		{
			this.PushArgumentReference(2, instruction);
		}

		public override void OnLdarg_3(Instruction instruction)
		{
			this.PushArgumentReference(3, instruction);
		}

		public override void OnLdarga(Instruction instruction)
		{
			ArgumentReferenceExpression argumentReferenceExpression = new ArgumentReferenceExpression((ParameterDefinition)instruction.get_Operand(), new Instruction[] { instruction });
			this.Push(new UnaryExpression(UnaryOperator.AddressReference, argumentReferenceExpression, null));
		}

		public override void OnLdc_I4(Instruction instruction)
		{
			this.PushLiteral(Convert.ToInt32(instruction.get_Operand()), instruction);
		}

		public override void OnLdc_I4_0(Instruction instruction)
		{
			this.PushLiteral(0, instruction);
		}

		public override void OnLdc_I4_1(Instruction instruction)
		{
			this.PushLiteral(1, instruction);
		}

		public override void OnLdc_I4_2(Instruction instruction)
		{
			this.PushLiteral(2, instruction);
		}

		public override void OnLdc_I4_3(Instruction instruction)
		{
			this.PushLiteral(3, instruction);
		}

		public override void OnLdc_I4_4(Instruction instruction)
		{
			this.PushLiteral(4, instruction);
		}

		public override void OnLdc_I4_5(Instruction instruction)
		{
			this.PushLiteral(5, instruction);
		}

		public override void OnLdc_I4_6(Instruction instruction)
		{
			this.PushLiteral(6, instruction);
		}

		public override void OnLdc_I4_7(Instruction instruction)
		{
			this.PushLiteral(7, instruction);
		}

		public override void OnLdc_I4_8(Instruction instruction)
		{
			this.PushLiteral(8, instruction);
		}

		public override void OnLdc_I4_M1(Instruction instruction)
		{
			this.PushLiteral(-1, instruction);
		}

		public override void OnLdc_I8(Instruction instruction)
		{
			this.PushLiteral(Convert.ToInt64(instruction.get_Operand()), instruction);
		}

		public override void OnLdc_R4(Instruction instruction)
		{
			this.PushLiteral(Convert.ToSingle(instruction.get_Operand()), instruction);
		}

		public override void OnLdc_R8(Instruction instruction)
		{
			this.PushLiteral(Convert.ToDouble(instruction.get_Operand()), instruction);
		}

		public override void OnLdelem_Any(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_I(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_I1(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_I2(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_I4(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_I8(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_R4(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_R8(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_Ref(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_U1(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_U2(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelem_U4(Instruction instruction)
		{
			this.PushArrayIndexer(false, instruction);
		}

		public override void OnLdelema(Instruction instruction)
		{
			this.PushArrayIndexer(true, instruction);
		}

		public override void OnLdfld(Instruction instruction)
		{
			this.PushFieldReference(instruction, this.Pop());
		}

		public override void OnLdflda(Instruction instruction)
		{
			Expression expression = this.Pop();
			UnaryOperator unaryOperator = UnaryOperator.AddressReference;
			unaryOperator = (!this.IsUnmanagedPointerType(expression) ? UnaryOperator.AddressReference : UnaryOperator.AddressOf);
			this.PushFieldReference(instruction, expression);
			this.Push(new UnaryExpression(unaryOperator, this.Pop(), null));
		}

		public override void OnLdftn(Instruction instruction)
		{
			this.Push(new MethodReferenceExpression(null, (MethodReference)instruction.get_Operand(), new Instruction[] { instruction }));
		}

		private void OnLdind(TypeReference type, Instruction instruction)
		{
			Expression expression = this.Pop();
			UnaryExpression unaryExpression = new UnaryExpression(UnaryOperator.AddressDereference, expression, this.IncludePrefixIfPresent(instruction, 208));
			if (unaryExpression.HasType && (!(unaryExpression.ExpressionType.get_FullName() != "System.Boolean") || !(unaryExpression.ExpressionType.get_FullName() != type.get_FullName()) || !(type.get_FullName() != "System.Object")))
			{
				this.Push(unaryExpression);
				return;
			}
			this.Push(new ExplicitCastExpression(unaryExpression, type, null));
		}

		public override void OnLdind_I(Instruction instruction)
		{
			this.OnLdind(new PointerType(this.currentTypeSystem.get_Void()), instruction);
		}

		public override void OnLdind_I1(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_SByte(), instruction);
		}

		public override void OnLdind_I2(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_Int16(), instruction);
		}

		public override void OnLdind_I4(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_Int32(), instruction);
		}

		public override void OnLdind_I8(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_Int64(), instruction);
		}

		public override void OnLdind_R4(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_Single(), instruction);
		}

		public override void OnLdind_R8(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_Double(), instruction);
		}

		public override void OnLdind_Ref(Instruction instruction)
		{
			this.OnLdind(this.methodContext.Method.get_Module().get_TypeSystem().get_Object(), instruction);
		}

		public override void OnLdind_U1(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_Byte(), instruction);
		}

		public override void OnLdind_U2(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_UInt16(), instruction);
		}

		public override void OnLdind_U4(Instruction instruction)
		{
			this.OnLdind(this.currentTypeSystem.get_UInt32(), instruction);
		}

		public override void OnLdlen(Instruction instruction)
		{
			this.Push(new ArrayLengthExpression(this.Pop(), this.methodContext.Method.get_Module().get_TypeSystem(), new Instruction[] { instruction }));
		}

		public override void OnLdloc(Instruction instruction)
		{
			this.PushVariableReference((VariableReference)instruction.get_Operand(), instruction);
		}

		public override void OnLdloc_0(Instruction instruction)
		{
			this.PushVariableReference(0, instruction);
		}

		public override void OnLdloc_1(Instruction instruction)
		{
			this.PushVariableReference(1, instruction);
		}

		public override void OnLdloc_2(Instruction instruction)
		{
			this.PushVariableReference(2, instruction);
		}

		public override void OnLdloc_3(Instruction instruction)
		{
			this.PushVariableReference(3, instruction);
		}

		public override void OnLdloca(Instruction instruction)
		{
			Expression variableReferenceExpression = new VariableReferenceExpression(instruction.get_Operand() as VariableReference, new Instruction[] { instruction });
			this.Push(new UnaryExpression(UnaryOperator.AddressReference, variableReferenceExpression, null));
		}

		public override void OnLdnull(Instruction instruction)
		{
			this.PushLiteral(null, instruction);
		}

		public override void OnLdobj(Instruction instruction)
		{
			IEnumerable<Instruction> instructions;
			Expression expression = this.Pop();
			if (instruction != null)
			{
				instructions = this.IncludePrefixIfPresent(instruction, 208);
			}
			else
			{
				instructions = null;
			}
			this.Push(new UnaryExpression(UnaryOperator.AddressDereference, expression, instructions));
		}

		public override void OnLdsfld(Instruction instruction)
		{
			this.PushFieldReference(instruction);
		}

		public override void OnLdsflda(Instruction instruction)
		{
			this.PushFieldReference(instruction);
			this.Push(new UnaryExpression(UnaryOperator.AddressReference, this.Pop(), null));
		}

		public override void OnLdstr(Instruction instruction)
		{
			this.PushLiteral(instruction.get_Operand(), instruction);
		}

		public override void OnLdtoken(Instruction instruction)
		{
			MemberReference operand = instruction.get_Operand() as MemberReference;
			if (operand == null)
			{
				throw new NotSupportedException();
			}
			this.Push(new MemberHandleExpression(operand, new Instruction[] { instruction }));
		}

		public override void OnLdvirtftn(Instruction instruction)
		{
			Expression unaryExpression = this.Pop();
			if (this.IsPointerType(unaryExpression))
			{
				unaryExpression = new UnaryExpression(UnaryOperator.AddressDereference, unaryExpression, new Instruction[] { instruction });
			}
			this.Push(new MethodReferenceExpression(unaryExpression, (MethodReference)instruction.get_Operand(), new Instruction[] { instruction }));
		}

		public override void OnLeave(Instruction instruction)
		{
		}

		public override void OnLocalloc(Instruction instruction)
		{
			this.Push(new StackAllocExpression(this.Pop(), new PointerType(this.methodContext.Method.get_Module().get_TypeSystem().get_IntPtr()), new Instruction[] { instruction }));
		}

		public override void OnMkrefany(Instruction instruction)
		{
			this.Push(new MakeRefExpression(this.Pop(), (TypeReference)instruction.get_Operand(), new Instruction[] { instruction }));
		}

		public override void OnMul(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Multiply, instruction);
		}

		public override void OnMul_Ovf(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Multiply, instruction);
		}

		public override void OnMul_Ovf_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Multiply, instruction);
		}

		public override void OnNeg(Instruction instruction)
		{
			this.Push(new UnaryExpression(UnaryOperator.Negate, this.Pop(), new Instruction[] { instruction }));
		}

		public override void OnNewarr(Instruction instruction)
		{
			ArrayCreationExpression arrayCreationExpression = new ArrayCreationExpression((TypeReference)instruction.get_Operand(), new InitializerExpression(new BlockExpression(null), InitializerType.ArrayInitializer), new Instruction[] { instruction });
			arrayCreationExpression.Dimensions.Add(this.Pop());
			this.Push(arrayCreationExpression);
		}

		public override void OnNewobj(Instruction instruction)
		{
			MethodReference operand = (MethodReference)instruction.get_Operand();
			ObjectCreationExpression objectCreationExpression = this.ProcessConstructor(instruction, this.ProcessArguments(operand));
			Expression array = this.ConvertObjectToArray(objectCreationExpression);
			if (array == null)
			{
				array = objectCreationExpression;
			}
			this.Push(array);
		}

		public override void OnNop(Instruction instruction)
		{
		}

		public override void OnNot(Instruction instruction)
		{
			this.Push(new UnaryExpression(UnaryOperator.BitwiseNot, this.Pop(), new Instruction[] { instruction }));
		}

		public override void OnOr(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.BitwiseOr, instruction);
		}

		public override void OnPop(Instruction instruction)
		{
			Expression expression = this.Pop();
			string str = String.Concat("dummyVar", this.dummyVarCounter.ToString());
			this.dummyVarCounter++;
			TypeReference expressionType = null;
			if (expression.HasType)
			{
				expressionType = expression.ExpressionType;
			}
			VariableDefinition variableDefinition = new VariableDefinition(str, expressionType, this.methodContext.Method);
			StackVariableDefineUseInfo stackVariableDefineUseInfo = new StackVariableDefineUseInfo();
			stackVariableDefineUseInfo.DefinedAt.Add(instruction.get_Offset());
			this.methodContext.StackData.VariableToDefineUseInfo.Add(variableDefinition, stackVariableDefineUseInfo);
			this.methodContext.StackData.InstructionOffsetToAssignedVariableMap.Add(instruction.get_Offset(), variableDefinition);
			this.methodContext.VariablesToRename.Add(variableDefinition);
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(variableDefinition, new Instruction[] { instruction });
			BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, variableReferenceExpression, expression, this.currentTypeSystem, null, false);
			this.Push(binaryExpression);
		}

		public override void OnRefanytype(Instruction instruction)
		{
			this.PushCastExpression(this.Import(typeof(RuntimeTypeHandle)), instruction);
		}

		public override void OnRem(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Modulo, instruction);
		}

		public override void OnRem_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Modulo, instruction);
		}

		public override void OnRet(Instruction instruction)
		{
			ReturnExpression returnExpression;
			if (this.expressionStack.Count == 0)
			{
				returnExpression = new ReturnExpression(null, new Instruction[] { instruction });
			}
			else if (!this.methodContext.Method.get_ReturnType().get_IsByReference())
			{
				returnExpression = new ReturnExpression(this.Pop(), new Instruction[] { instruction });
			}
			else
			{
				returnExpression = new RefReturnExpression(this.Pop(), new Instruction[] { instruction });
			}
			this.Push(returnExpression);
		}

		public override void OnRethrow(Instruction instruction)
		{
			this.Push(new ThrowExpression(null, new Instruction[] { instruction }));
		}

		public override void OnShl(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.LeftShift, instruction);
		}

		public override void OnShr(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.RightShift, instruction);
		}

		public override void OnShr_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.RightShift, instruction);
		}

		public override void OnSizeof(Instruction instruction)
		{
			this.Push(new SizeOfExpression((TypeReference)instruction.get_Operand(), new Instruction[] { instruction }));
		}

		public override void OnStarg(Instruction instruction)
		{
			this.PushArgumentReference((ParameterReference)instruction.get_Operand());
			this.PushAssignment(this.Pop(), this.Pop(), null);
		}

		public override void OnStelem_Any(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_I(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_I1(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_I2(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_I4(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_I8(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_R4(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_R8(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStelem_Ref(Instruction instruction)
		{
			this.PushArrayStore(instruction);
		}

		public override void OnStfld(Instruction instruction)
		{
			Expression expression = this.Pop();
			Expression unaryExpression = this.Pop();
			if (this.IsPointerType(unaryExpression))
			{
				unaryExpression = new UnaryExpression(UnaryOperator.AddressDereference, unaryExpression, null);
			}
			this.PushAssignment(new FieldReferenceExpression(unaryExpression, (FieldReference)instruction.get_Operand(), this.IncludePrefixIfPresent(instruction, 208))
			{
				IsSimpleStore = true
			}, expression, null);
		}

		public override void OnStind_I(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_I1(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_I2(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_I4(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_I8(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_R4(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_R8(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStind_Ref(Instruction instruction)
		{
			this.OnStobj(instruction);
		}

		public override void OnStloc(Instruction instruction)
		{
			this.PushVariableAssignement((VariableReference)instruction.get_Operand(), instruction);
		}

		private void OnStloc(int index, Instruction instruction)
		{
			this.PushVariableAssignement(index, instruction);
		}

		public override void OnStloc_0(Instruction instruction)
		{
			this.OnStloc(0, instruction);
		}

		public override void OnStloc_1(Instruction instruction)
		{
			this.OnStloc(1, instruction);
		}

		public override void OnStloc_2(Instruction instruction)
		{
			this.OnStloc(2, instruction);
		}

		public override void OnStloc_3(Instruction instruction)
		{
			this.OnStloc(3, instruction);
		}

		public override void OnStobj(Instruction instruction)
		{
			Expression expression = this.Pop();
			Expression unaryExpression = new UnaryExpression(UnaryOperator.AddressDereference, this.Pop(), null);
			this.PushAssignment(unaryExpression, expression, this.IncludePrefixIfPresent(instruction, 208));
		}

		public override void OnStsfld(Instruction instruction)
		{
			this.PushAssignment(new FieldReferenceExpression(null, (FieldReference)instruction.get_Operand(), this.IncludePrefixIfPresent(instruction, 208))
			{
				IsSimpleStore = true
			}, this.Pop(), null);
		}

		public override void OnSub(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Subtract, instruction);
		}

		public override void OnSub_Ovf(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Subtract, instruction);
		}

		public override void OnSub_Ovf_Un(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.Subtract, instruction);
		}

		public override void OnSwitch(Instruction instruction)
		{
		}

		public override void OnThrow(Instruction instruction)
		{
			this.Push(new ThrowExpression(this.Pop(), new Instruction[] { instruction }));
		}

		public override void OnUnbox(Instruction instruction)
		{
			this.PushCastExpression(new ByReferenceType((TypeReference)instruction.get_Operand()), instruction);
		}

		public override void OnUnbox_Any(Instruction instruction)
		{
			this.PushCastExpression((TypeReference)instruction.get_Operand(), instruction);
		}

		public override void OnVolatile(Instruction instruction)
		{
		}

		public override void OnXor(Instruction instruction)
		{
			this.PushBinaryExpression(BinaryOperator.BitwiseXor, instruction);
		}

		private Expression Peek()
		{
			return this.expressionStack.Peek();
		}

		private Expression Pop()
		{
			Expression expression = this.expressionStack.Pop();
			this.used.Add(expression);
			if (expression is VariableReferenceExpression)
			{
				VariableReference variable = (expression as VariableReferenceExpression).Variable;
				if (this.exceptionVariables.ContainsKey(variable))
				{
					KeyValuePair<int, bool> item = this.exceptionVariables[variable];
					this.exceptionVariables[variable] = new KeyValuePair<int, bool>(item.Key, true);
				}
			}
			if (expression is VariableDeclarationExpression)
			{
				VariableReference keyValuePair = (expression as VariableDeclarationExpression).Variable;
				if (this.exceptionVariables.ContainsKey(keyValuePair))
				{
					KeyValuePair<int, bool> item1 = this.exceptionVariables[keyValuePair];
					this.exceptionVariables[keyValuePair] = new KeyValuePair<int, bool>(item1.Key, true);
				}
			}
			return expression;
		}

		public BlockStatement Process(DecompilationContext theContext, BlockStatement body)
		{
			this.context = theContext;
			this.methodContext = theContext.MethodContext;
			this.currentTypeSystem = this.methodContext.Method.get_Module().get_TypeSystem();
			this.typeContext = theContext.TypeContext;
			this.CreateExpressions();
			this.methodContext.Expressions = this.results;
			(new StackVariablesInliner(this.methodContext, this.offsetToExpression, this.context.Language.VariablesToNotInlineFinder)).InlineVariables();
			this.AddUninlinedStackVariablesToContext();
			(new TypeInferer(theContext, this.offsetToExpression)).InferTypes();
			(new FixBinaryExpressionsStep(this.methodContext.Method.get_Module().get_TypeSystem())).Process(theContext, body);
			(new MethodVariablesInliner(this.methodContext, this.context.Language.VariablesToNotInlineFinder)).InlineVariables();
			(new UsageBasedExpressionFixer(this.methodContext)).FixLiterals();
			(new FindAutoBoxesStep()).Process(theContext, body);
			return body;
		}

		private ExpressionCollection ProcessArguments(MethodReference method)
		{
			ExpressionCollection expressionCollection = new ExpressionCollection();
			for (int i = method.get_Parameters().get_Count() - 1; i >= 0; i--)
			{
				expressionCollection.Insert(0, this.Pop());
			}
			return expressionCollection;
		}

		private ObjectCreationExpression ProcessConstructor(Instruction instruction, ExpressionCollection arguments)
		{
			IEnumerable<Instruction> instructions;
			TypeReference declaringType;
			if (instruction.get_OpCode().get_Code() == 110)
			{
				instructions = this.IncludePrefixIfPresent(instruction, 211);
			}
			else
			{
				instructions = new Instruction[] { instruction };
			}
			IEnumerable<Instruction> instructions1 = instructions;
			MethodReference operand = (MethodReference)instruction.get_Operand();
			MethodReference methodReference = operand;
			if (operand != null)
			{
				declaringType = operand.get_DeclaringType();
			}
			else
			{
				declaringType = null;
			}
			ObjectCreationExpression objectCreationExpression = new ObjectCreationExpression(methodReference, declaringType, null, instructions1);
			ExpressionDecompilerStep.AddRange<Expression>(objectCreationExpression.Arguments, arguments);
			return objectCreationExpression;
		}

		private void ProcessYieldBlock(InstructionBlock theBlock, List<Expression> blockExpressions)
		{
			int count = blockExpressions.Count;
			if (this.methodContext.YieldData.YieldBreaks.Contains(theBlock))
			{
				if (blockExpressions[count - 1].CodeNodeType != CodeNodeType.ReturnExpression)
				{
					throw new Exception("No return at the end of yield break block");
				}
				Instruction instruction = (blockExpressions[count - 1] as ReturnExpression).UnderlyingSameMethodInstructions.Last<Instruction>();
				blockExpressions[count - 1] = new YieldBreakExpression(new Instruction[] { instruction });
				return;
			}
			if (this.methodContext.YieldData.YieldReturns.Contains(theBlock))
			{
				Expression yieldReturnItem = this.GetYieldReturnItem(blockExpressions);
				int yieldReturnAssignmentIndex = this.GetYieldReturnAssignmentIndex(blockExpressions);
				blockExpressions[yieldReturnAssignmentIndex] = new YieldReturnExpression(yieldReturnItem, blockExpressions[yieldReturnAssignmentIndex].UnderlyingSameMethodInstructions);
				count = blockExpressions.Count;
				if (blockExpressions[count - 1].CodeNodeType == CodeNodeType.ReturnExpression)
				{
					blockExpressions.RemoveAt(count - 1);
				}
			}
		}

		public void Push(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			this.expressionStack.Push(expression);
		}

		private void PushArgumentReference(ParameterReference parameter)
		{
			this.Push(new ArgumentReferenceExpression(parameter, new Instruction[] { this.CurrentInstruction }));
		}

		private void PushArgumentReference(int index, Instruction instruction)
		{
			if (this.methodContext.Method.get_HasThis())
			{
				if (index == 0)
				{
					this.Push(new ThisReferenceExpression(this.methodContext.Method.get_DeclaringType(), new Instruction[] { instruction }));
					return;
				}
				index--;
			}
			this.Push(new ArgumentReferenceExpression(this.methodContext.Method.get_Parameters().get_Item(index), new Instruction[] { this.CurrentInstruction }));
		}

		private void PushArrayIndexer(bool isAddress, Instruction instruction)
		{
			Expression expression = this.Pop();
			ArrayIndexerExpression arrayIndexerExpression = new ArrayIndexerExpression(this.Pop(), new Instruction[] { instruction });
			arrayIndexerExpression.Indices.Add(expression);
			if (!isAddress)
			{
				this.Push(arrayIndexerExpression);
				return;
			}
			this.Push(new UnaryExpression(UnaryOperator.AddressReference, arrayIndexerExpression, null));
		}

		private void PushArrayStore(Instruction instruction)
		{
			Expression expression = this.Pop();
			this.PushArrayIndexer(false, instruction);
			ArrayIndexerExpression arrayIndexerExpression = this.Pop() as ArrayIndexerExpression;
			arrayIndexerExpression.IsSimpleStore = true;
			BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, arrayIndexerExpression, expression, this.currentTypeSystem, null, false);
			this.Push(binaryExpression);
		}

		private void PushAssignment(Expression left, Expression right, IEnumerable<Instruction> instructions = null)
		{
			BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, left, right, this.currentTypeSystem, instructions, false);
			this.Push(binaryExpression);
		}

		public void PushBinaryExpression(BinaryOperator op, Instruction instruction)
		{
			Expression expression = this.Pop();
			Expression expression1 = this.Pop();
			BinaryExpression binaryExpression = new BinaryExpression(op, expression1, expression, this.currentTypeSystem, new Instruction[] { instruction }, false);
			if (binaryExpression.IsComparisonExpression || binaryExpression.IsLogicalExpression)
			{
				binaryExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean();
			}
			this.Push(binaryExpression);
		}

		private void PushCastExpression(TypeReference targetType, Instruction instruction)
		{
			// 
			// Current member / type: System.Void Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep::PushCastExpression(Mono.Cecil.TypeReference,Mono.Cecil.Cil.Instruction)
			// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\Integration\Actual\JustDecompiler.NetStandard.dll
			// 
			// Product version: 0.0.0.0
			// Exception in: System.Void PushCastExpression(Mono.Cecil.TypeReference,Mono.Cecil.Cil.Instruction)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at Telerik.JustDecompiler.Ast.Expressions.ArrayIndexerExpression.get_ExpressionType() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.UpdateType() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Ast\Expressions\BinaryExpression.cs:line 228
			//    at Telerik.JustDecompiler.Steps.FixBinaryExpressionsStep.VisitBinaryExpression(BinaryExpression expression) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\FixBinaryExpressionsStep.cs:line 74
			//    at Telerik.JustDecompiler.Ast.BaseCodeTransformer.Visit(ICodeNode node) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Ast\BaseCodeTransformer.cs:line 276
			//    at Telerik.JustDecompiler.Steps.FixBinaryExpressionsStep.Process(DecompilationContext context, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\FixBinaryExpressionsStep.cs:line 44
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.Process(DecompilationContext theContext, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\ExpressionDecompilerStep.cs:line 93
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.Decompile(MethodBody body, ILanguage language, DecompilationContext& context, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 61
			//    at Telerik.JustDecompiler.Decompiler.WriterContextServices.BaseWriterContextService.DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void PushFieldReference(Instruction instruction)
		{
			this.PushFieldReference(instruction, null);
		}

		private void PushFieldReference(Instruction instruction, Expression target)
		{
			EventDefinition eventDefinition;
			FieldDefinition fieldDefinition = (instruction.get_Operand() as FieldReference).Resolve();
			if (fieldDefinition != null && (object)fieldDefinition.get_DeclaringType() == (object)this.methodContext.Method.get_DeclaringType() && this.methodContext.EnableEventAnalysis && this.typeContext.GetFieldToEventMap(this.context.Language).TryGetValue(fieldDefinition, out eventDefinition))
			{
				this.Push(new EventReferenceExpression(target, eventDefinition, this.IncludePrefixIfPresent(instruction, 208)));
				return;
			}
			if (target != null && this.IsPointerType(target))
			{
				target = new UnaryExpression(UnaryOperator.AddressDereference, target, null);
			}
			this.Push(new FieldReferenceExpression(target, (FieldReference)instruction.get_Operand(), this.IncludePrefixIfPresent(instruction, 208)));
		}

		private void PushLiteral(object value, Instruction instruction)
		{
			LiteralExpression literalExpression = new LiteralExpression(value, this.currentTypeSystem, new Instruction[] { instruction });
			if (literalExpression.Value == null)
			{
				literalExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Object();
			}
			else if (literalExpression.Value is Int32)
			{
				literalExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Int32();
			}
			else if (literalExpression.Value is String)
			{
				literalExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_String();
			}
			else if (literalExpression.Value is Single)
			{
				literalExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Single();
			}
			else if (literalExpression.Value is Double)
			{
				literalExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Double();
			}
			else if (literalExpression.Value is Int64)
			{
				literalExpression.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Int64();
			}
			this.Push(literalExpression);
		}

		private void PushVariableAssignement(VariableReference variable, Instruction instruction)
		{
			this.PushVariableAssignement(variable.get_Index(), instruction);
		}

		private void PushVariableAssignement(int index, Instruction instruction)
		{
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(this.methodContext.Method.get_Body().get_Variables().get_Item(index).Resolve(), new Instruction[] { instruction });
			this.PushAssignment(variableReferenceExpression, this.Pop(), null);
		}

		private void PushVariableReference(int index, Instruction instruction)
		{
			this.PushVariableReference(this.methodContext.Method.get_Body().get_Variables().get_Item(index).Resolve(), instruction);
		}

		private void PushVariableReference(VariableReference variable, Instruction instruction)
		{
			this.Push(new VariableReferenceExpression(variable, new Instruction[] { instruction }));
		}

		private void SetInitialStack(Instruction instruction)
		{
			List<VariableDefinition> variableDefinitions;
			if (!this.methodContext.StackData.InstructionOffsetToUsedStackVariablesMap.TryGetValue(instruction.get_Offset(), out variableDefinitions))
			{
				this.expressionStack = new Stack<Expression>();
				return;
			}
			this.expressionStack = new Stack<Expression>(
				from varDef in variableDefinitions
				select new VariableReferenceExpression(varDef, null));
		}

		private bool TryGetArrayIndexingMethodName(MethodReferenceExpression methodRef, out string methodName)
		{
			methodName = methodRef.Member.get_Name();
			if (methodName == "Get" || methodName == "Set")
			{
				return true;
			}
			return methodName == "Address";
		}

		private bool TryGetTargetArray(MethodReferenceExpression methodRef, out Expression target)
		{
			if (methodRef == null || !(methodRef.Member.get_DeclaringType() is ArrayType))
			{
				target = null;
				return false;
			}
			target = methodRef.Target;
			return true;
		}

		private bool TryGetVariableValueAndMarkForRemoval(VariableReferenceExpression variableRefExpression, out Expression value)
		{
			StackVariableDefineUseInfo stackVariableDefineUseInfo;
			if (variableRefExpression == null || !this.methodContext.StackData.VariableToDefineUseInfo.TryGetValue(variableRefExpression.Variable.Resolve(), out stackVariableDefineUseInfo) || stackVariableDefineUseInfo.DefinedAt.Count != 1 || stackVariableDefineUseInfo.UsedAt.Count != 1 || !this.offsetToExpression.TryGetValue(stackVariableDefineUseInfo.DefinedAt.First<int>(), out value))
			{
				value = null;
				return false;
			}
			VariableDefinition variableDefinition = variableRefExpression.Variable.Resolve();
			this.methodContext.StackData.VariableToDefineUseInfo.Remove(variableDefinition);
			this.stackVariableAssignmentsToRemove.Add(variableDefinition);
			return true;
		}

		private bool TryProcessMultidimensionalIndexing(MethodInvocationExpression invocation)
		{
			if (invocation.MethodExpression == null || invocation.Arguments == null || invocation.Arguments.Count < 1)
			{
				return false;
			}
			MethodReferenceExpression methodExpression = invocation.MethodExpression;
			Expression expression = null;
			string str = null;
			if (!this.TryGetTargetArray(methodExpression, out expression) || !this.TryGetArrayIndexingMethodName(methodExpression, out str))
			{
				return false;
			}
			ArrayIndexerExpression arrayIndexerExpression = new ArrayIndexerExpression(expression, invocation.InvocationInstructions)
			{
				Indices = invocation.Arguments
			};
			if (str == "Set")
			{
				arrayIndexerExpression.IsSimpleStore = true;
				Expression item = arrayIndexerExpression.Indices[arrayIndexerExpression.Indices.Count - 1];
				arrayIndexerExpression.Indices.RemoveAt(arrayIndexerExpression.Indices.Count - 1);
				this.PushAssignment(arrayIndexerExpression, item, null);
			}
			else if (str != "Address")
			{
				this.Push(arrayIndexerExpression);
			}
			else
			{
				this.Push(new UnaryExpression(UnaryOperator.AddressReference, arrayIndexerExpression, null));
			}
			return true;
		}

		private bool TryProcessRuntimeHelpersInitArray(MethodInvocationExpression invocation)
		{
			Expression expression;
			string str = "System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle)";
			MethodReferenceExpression methodExpression = invocation.MethodExpression;
			if (methodExpression == null || !(methodExpression.Method.get_FullName() == str))
			{
				return false;
			}
			ArrayCreationExpression closestArrayCreationExpression = this.GetClosestArrayCreationExpression();
			if (closestArrayCreationExpression == null)
			{
				throw new Exception("The expression at the top of the expression stack is not ArrayCreationExpression");
			}
			BlockExpression blockExpression = new BlockExpression(invocation.InvocationInstructions);
			closestArrayCreationExpression.Initializer = new InitializerExpression(blockExpression, InitializerType.ArrayInitializer);
			if (!this.TryGetVariableValueAndMarkForRemoval(invocation.Arguments[1] as VariableReferenceExpression, out expression))
			{
				throw new Exception("Invalid array initialization info");
			}
			closestArrayCreationExpression.Initializer.Expressions.Add(expression);
			return true;
		}
	}
}