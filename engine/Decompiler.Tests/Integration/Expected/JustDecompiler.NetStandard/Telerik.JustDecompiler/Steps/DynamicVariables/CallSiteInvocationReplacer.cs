using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
	internal class CallSiteInvocationReplacer : BaseCodeTransformer
	{
		private const string InvalidIsEventString = "Invalid IsEvent construction";

		private readonly Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap;

		private readonly Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap;

		private readonly Dictionary<IfStatement, MethodInvocationExpression> isEventIfStatements;

		private readonly TypeReference objectTypeRef;

		private readonly TypeSystem typeSystem;

		private IfStatement closestIf;

		private CallSiteInvocationReplacer(Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap, Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap, TypeSystem typeSystem)
		{
			this.isEventIfStatements = new Dictionary<IfStatement, MethodInvocationExpression>();
			base();
			this.fieldToCallSiteInfoMap = fieldToCallSiteInfoMap;
			this.variableToCallSiteInfoMap = variableToCallSiteInfoMap;
			this.typeSystem = typeSystem;
			this.objectTypeRef = typeSystem.get_Object();
			return;
		}

		private bool CanReplaceIf(BlockStatement statementBlock)
		{
			if (statementBlock.get_Statements().get_Count() == 2 || statementBlock.get_Statements().get_Count() == 3 && statementBlock.get_Statements().get_Item(1).get_CodeNodeType() == 5 && (statementBlock.get_Statements().get_Item(1) as ExpressionStatement).get_Expression().get_CodeNodeType() == 59)
			{
				if (statementBlock.get_Statements().get_Count() == 3 && statementBlock.get_Statements().get_Item(2).get_CodeNodeType() != 5 || (statementBlock.get_Statements().get_Item(2) as ExpressionStatement).get_Expression().get_CodeNodeType() != 57)
				{
					return false;
				}
				return true;
			}
			if ((statementBlock.get_Statements().get_Item(1) as ExpressionStatement).get_Expression().get_CodeNodeType() != 24)
			{
				return false;
			}
			V_0 = (statementBlock.get_Statements().get_Item(1) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0.get_IsAssignmentExpression() && V_0.get_Right().get_CodeNodeType() == 59)
			{
				return true;
			}
			return false;
		}

		private Expression GenerateBinaryExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() != 2)
			{
				throw new Exception("Invalid number of arguments for binary expression.");
			}
			return new BinaryExpression(DynamicHelper.GetBinaryOperator(callSiteInfo.get_Operator()), arguments.get_Item(0), arguments.get_Item(1), this.objectTypeRef, this.typeSystem, instructions, false);
		}

		private Expression GenerateConvertExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() != 1)
			{
				throw new Exception("Invalid number of arguments for convert expression.");
			}
			return new ExplicitCastExpression(arguments.get_Item(0), callSiteInfo.get_ConvertType(), instructions);
		}

		private Expression GenerateExpression(CallSiteInfo callSiteInfo, IEnumerable<Expression> originalArguments, IEnumerable<Instruction> instructions)
		{
			V_0 = this.GetAllButFirst(originalArguments);
			this.MarkDynamicArguments(callSiteInfo, V_0);
			switch (callSiteInfo.get_BinderType())
			{
				case 0:
				{
					return this.GenerateBinaryExpression(callSiteInfo, V_0, instructions);
				}
				case 1:
				{
					return this.GenerateConvertExpression(callSiteInfo, V_0, instructions);
				}
				case 2:
				{
					return this.GenerateGetIndexExpression(V_0, instructions);
				}
				case 3:
				{
					return this.GenerateGetMemberExpression(callSiteInfo, V_0, instructions);
				}
				case 4:
				{
					return this.GenerateInvokeExpression(callSiteInfo, V_0, instructions);
				}
				case 5:
				{
					return this.GenerateInvokeConstructorExpression(V_0, instructions);
				}
				case 6:
				{
					return this.GenerateInvokeMemeberExpression(callSiteInfo, V_0, instructions);
				}
				case 7:
				{
				Label0:
					throw new Exception("Invalid binder type.");
				}
				case 8:
				{
					return this.GenerateSetIndexExpression(V_0, instructions);
				}
				case 9:
				{
					return this.GenerateSetMemberExpression(callSiteInfo, V_0, instructions);
				}
				case 10:
				{
					return this.GenerateUnaryExpression(callSiteInfo, V_0, instructions);
				}
				default:
				{
					goto Label0;
				}
			}
		}

		private Expression GenerateGetIndexExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() < 2)
			{
				throw new Exception("Invalid number of arguments for get index expression.");
			}
			V_0 = new DynamicIndexerExpression(arguments.get_Item(0), this.objectTypeRef, instructions);
			V_1 = 1;
			while (V_1 < arguments.get_Count())
			{
				V_0.get_Indices().Add(arguments.get_Item(V_1));
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		private Expression GenerateGetMemberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() != 1)
			{
				throw new Exception("Invalid number of arguments for get member expression.");
			}
			return new DynamicMemberReferenceExpression(arguments.get_Item(0), callSiteInfo.get_MemberName(), this.objectTypeRef, instructions);
		}

		private Expression GenerateInvokeConstructorExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() < 1)
			{
				throw new Exception("Invalid number of arguments for invoke constructor expression.");
			}
			if (arguments.get_Item(0).get_CodeNodeType() != 19 || !(arguments.get_Item(0) as MethodInvocationExpression).IsTypeOfExpression(out V_0))
			{
				throw new Exception("Invalid type argument for invoke constructor expression.");
			}
			return new DynamicConstructorInvocationExpression(V_0, this.GetAllButFirst(arguments), instructions);
		}

		private Expression GenerateInvokeExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			return this.GenerateInvokeMemeberExpression(callSiteInfo, arguments, instructions);
		}

		private Expression GenerateInvokeMemeberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() < 1)
			{
				throw new Exception("Invalid number of arguments for invoke expression.");
			}
			if (arguments.get_Item(0).get_CodeNodeType() != 19 || !(arguments.get_Item(0) as MethodInvocationExpression).IsTypeOfExpression(out V_1))
			{
				V_0 = arguments.get_Item(0);
			}
			else
			{
				V_0 = new TypeReferenceExpression(V_1, arguments.get_Item(0).get_UnderlyingSameMethodInstructions());
			}
			return new DynamicMemberReferenceExpression(V_0, callSiteInfo.get_MemberName(), this.objectTypeRef, instructions, this.GetAllButFirst(arguments), callSiteInfo.get_GenericTypeArguments());
		}

		private Expression GenerateSetIndexExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() < 3)
			{
				throw new Exception("Invalid number of arguments for set index expression.");
			}
			V_0 = new DynamicIndexerExpression(arguments.get_Item(0), this.objectTypeRef, instructions);
			V_1 = 1;
			while (V_1 < arguments.get_Count() - 1)
			{
				V_0.get_Indices().Add(arguments.get_Item(V_1));
				V_1 = V_1 + 1;
			}
			return new BinaryExpression(26, V_0, arguments.get_Item(arguments.get_Count() - 1), this.typeSystem, null, false);
		}

		private Expression GenerateSetMemberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() != 2)
			{
				throw new Exception("Invalid number of arguments for set index expression.");
			}
			return new BinaryExpression(26, new DynamicMemberReferenceExpression(arguments.get_Item(0), callSiteInfo.get_MemberName(), this.objectTypeRef, instructions), arguments.get_Item(1), this.typeSystem, null, false);
		}

		private Expression GenerateUnaryExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.get_Count() != 1)
			{
				throw new Exception("Invalid number of arguments for unary expression.");
			}
			if (callSiteInfo.get_Operator() == 83)
			{
				return arguments.get_Item(0);
			}
			return new UnaryExpression(DynamicHelper.GetUnaryOperator(callSiteInfo.get_Operator()), arguments.get_Item(0), instructions);
		}

		private IList<Expression> GetAllButFirst(IEnumerable<Expression> expressionEnumeration)
		{
			V_0 = new List<Expression>();
			V_1 = expressionEnumeration.GetEnumerator();
			try
			{
				dummyVar0 = V_1.MoveNext();
				while (V_1.MoveNext())
				{
					V_0.Add(this.RemoveUnneededCast(V_1.get_Current()));
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private void ManageIsEventOperations(HashSet<Statement> statementsToRemove)
		{
			V_0 = this.isEventIfStatements.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Key();
					V_3 = V_1.get_Value();
					if (V_2.get_Condition().get_CodeNodeType() != 23 || (V_2.get_Condition() as UnaryExpression).get_Operator() != 11 || (V_2.get_Condition() as UnaryExpression).get_Operand() != V_3)
					{
						if (V_2.get_Condition().get_CodeNodeType() != 23 || (V_2.get_Condition() as UnaryExpression).get_Operator() != UnaryOperator.Negate || (V_2.get_Condition() as UnaryExpression).get_Operand() != V_3)
						{
							throw new Exception("Invalid invocation of IsEvent operation.");
						}
						this.ReplaceIfWith(V_2, V_2.get_Else());
					}
					else
					{
						V_4 = V_2.get_Then().get_Statements().get_Item(V_2.get_Then().get_Statements().get_Count() - 1);
						if (V_4.get_CodeNodeType() == 5 && ((ExpressionStatement)V_4).get_Expression().get_CodeNodeType() == 57)
						{
							V_5 = (BlockStatement)V_2.get_Parent();
							V_6 = V_5.get_Statements().IndexOf(V_2) + 1;
							while (V_6 < V_5.get_Statements().get_Count())
							{
								dummyVar0 = statementsToRemove.Add(V_5.get_Statements().get_Item(V_6));
								V_6 = V_6 + 1;
							}
						}
						this.ReplaceIfWith(V_2, V_2.get_Then());
					}
					V_7 = V_2.get_Then().get_Statements().GetEnumerator();
					try
					{
						while (V_7.MoveNext())
						{
							V_8 = V_7.get_Current();
							dummyVar1 = statementsToRemove.Remove(V_8);
						}
					}
					finally
					{
						if (V_7 != null)
						{
							V_7.Dispose();
						}
					}
					if (V_2.get_Else() == null)
					{
						continue;
					}
					V_7 = V_2.get_Else().get_Statements().GetEnumerator();
					try
					{
						while (V_7.MoveNext())
						{
							V_9 = V_7.get_Current();
							dummyVar2 = statementsToRemove.Remove(V_9);
						}
					}
					finally
					{
						if (V_7 != null)
						{
							V_7.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void MarkDynamicArguments(CallSiteInfo callSiteInfo, IList<Expression> arguments)
		{
			V_0 = callSiteInfo.get_DynamicArgumentIndices().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (arguments.get_Item(V_1).get_CodeNodeType() == 24 || arguments.get_Item(V_1).get_CodeNodeType() == 23 && (arguments.get_Item(V_1) as UnaryExpression).get_Operator() == 11 || arguments.get_Item(V_1).get_CodeNodeType() == 61 || arguments.get_Item(V_1).get_CodeNodeType() == 59 || DynamicElementAnalyzer.Analyze(arguments.get_Item(V_1)))
					{
						continue;
					}
					V_2 = new ExplicitCastExpression(arguments.get_Item(V_1), this.objectTypeRef, null);
					stackVariable40 = new Boolean[1];
					stackVariable40[0] = true;
					V_2.set_DynamicPositioningFlags(stackVariable40);
					arguments.set_Item(V_1, V_2);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private Expression RemoveUnneededCast(Expression expression)
		{
			while (expression.get_CodeNodeType() == 31 && (expression as ExplicitCastExpression).get_TargetType().get_Name().get_Chars(0) == '!')
			{
				expression = (expression as ExplicitCastExpression).get_Expression();
			}
			return expression;
		}

		private void ReplaceIfWith(IfStatement theIf, BlockStatement statementBlock)
		{
			if (!this.CanReplaceIf(statementBlock))
			{
				throw new Exception("Invalid IsEvent construction");
			}
			V_0 = (statementBlock.get_Statements().get_Item(1) as ExpressionStatement).get_Expression() as DynamicMemberReferenceExpression;
			if (V_0 == null)
			{
				V_0 = ((statementBlock.get_Statements().get_Item(1) as ExpressionStatement).get_Expression() as BinaryExpression).get_Right() as DynamicMemberReferenceExpression;
			}
			if (V_0.get_MemberName() == null || !V_0.get_IsMethodInvocation() || V_0.get_IsGenericMethod() || V_0.get_InvocationArguments().get_Count() != 1)
			{
				throw new Exception("Invalid IsEvent construction");
			}
			V_1 = V_0.get_MemberName().IndexOf('\u005F');
			if (V_1 != 3 && V_1 != 6)
			{
				throw new Exception("Invalid IsEvent construction");
			}
			V_2 = new DynamicMemberReferenceExpression(V_0.get_Target(), V_0.get_MemberName().Substring(V_1 + 1), V_0.get_ExpressionType(), V_0.get_MappedInstructions());
			if (V_1 == 3)
			{
				stackVariable44 = 2;
			}
			else
			{
				stackVariable44 = 4;
			}
			stackVariable56 = new BinaryExpression(stackVariable44, V_2, V_0.get_InvocationArguments().get_Item(0), V_2.get_ExpressionType(), this.typeSystem, null, false);
			V_3 = (BlockStatement)theIf.get_Parent();
			V_4 = V_3.get_Statements().IndexOf(theIf);
			V_5 = new ExpressionStatement(stackVariable56);
			V_5.set_Parent(V_3);
			V_3.get_Statements().set_Item(V_4, V_5);
			if (statementBlock.get_Statements().get_Count() == 3)
			{
				V_3.AddStatementAt(V_4 + 1, statementBlock.get_Statements().get_Item(2).Clone());
			}
			return;
		}

		public static BlockStatement ReplaceInvocations(BlockStatement block, Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap, Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap, HashSet<Statement> statementsToRemove, TypeSystem typeSystem)
		{
			stackVariable3 = new CallSiteInvocationReplacer(fieldToCallSiteInfoMap, variableToCallSiteInfoMap, typeSystem);
			V_0 = (BlockStatement)stackVariable3.Visit(block);
			stackVariable3.ManageIsEventOperations(statementsToRemove);
			return V_0;
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			if (node.IsAssignmentStatement())
			{
				V_0 = node.get_Expression() as BinaryExpression;
				if (V_0.get_Left().get_CodeNodeType() == 26 && V_0.get_Right().get_CodeNodeType() == 30)
				{
					V_1 = (V_0.get_Right() as FieldReferenceExpression).get_Field().Resolve();
					if (V_1 != null && this.fieldToCallSiteInfoMap.ContainsKey(V_1))
					{
						return null;
					}
				}
			}
			return this.VisitExpressionStatement(node);
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			this.closestIf = node;
			return this.VisitIfStatement(node);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = (Expression)this.VisitMethodInvocationExpression(node);
			V_1 = V_0 as MethodInvocationExpression;
			if (V_1 != null && V_1.get_MethodExpression().get_Target() != null && String.op_Equality(V_1.get_MethodExpression().get_Method().get_Name(), "Invoke"))
			{
				if (V_1.get_MethodExpression().get_Target().get_CodeNodeType() != 30 || !String.op_Equality((V_1.get_MethodExpression().get_Target() as FieldReferenceExpression).get_Field().get_Name(), "Target") || (V_1.get_MethodExpression().get_Target() as FieldReferenceExpression).get_Target() == null || (V_1.get_MethodExpression().get_Target() as FieldReferenceExpression).get_Target().get_CodeNodeType() != 30)
				{
					if (V_1.get_MethodExpression().get_Target().get_CodeNodeType() == 26)
					{
						V_4 = (V_1.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable();
						if (this.variableToCallSiteInfoMap.TryGetValue(V_4, out V_5))
						{
							if (V_5.get_BinderType() != 7)
							{
								return this.GenerateExpression(V_5, V_1.get_Arguments(), V_1.get_InvocationInstructions());
							}
							this.isEventIfStatements.Add(this.closestIf, V_1);
						}
					}
				}
				else
				{
					V_2 = ((V_1.get_MethodExpression().get_Target() as FieldReferenceExpression).get_Target() as FieldReferenceExpression).get_Field().Resolve();
					if (V_2 != null && this.fieldToCallSiteInfoMap.TryGetValue(V_2, out V_3))
					{
						if (V_3.get_BinderType() != 7)
						{
							return this.GenerateExpression(V_3, V_1.get_Arguments(), V_1.get_InvocationInstructions());
						}
						this.isEventIfStatements.Add(this.closestIf, V_1);
					}
				}
			}
			return V_0;
		}
	}
}