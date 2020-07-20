using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class ClassHierarchyBuilder
	{
		protected readonly Dictionary<string, ClassHierarchyNode> typeNameToNode;

		protected readonly Dictionary<int, Expression> offsetToExpression;

		protected readonly Dictionary<string, ClassHierarchyNode> variableNameToNode;

		protected readonly Dictionary<int, Instruction> offsetToInstruction;

		protected readonly HashSet<ClassHierarchyNode> resultingGraph;

		protected readonly MethodSpecificContext methodContext;

		protected readonly TypeSystem typeSystem;

		public ClassHierarchyBuilder(Dictionary<int, Expression> offsetToExpression, Dictionary<int, Instruction> offsetToInstruction, DecompilationContext context)
		{
			base();
			this.typeNameToNode = new Dictionary<string, ClassHierarchyNode>();
			this.variableNameToNode = new Dictionary<string, ClassHierarchyNode>();
			this.resultingGraph = new HashSet<ClassHierarchyNode>();
			this.offsetToExpression = offsetToExpression;
			this.offsetToInstruction = offsetToInstruction;
			this.methodContext = context.get_MethodContext();
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			return;
		}

		private void AddObjectClassNodeIfMIssing()
		{
			V_1 = this.GetTypeNode(this.typeSystem.get_Object());
			dummyVar0 = this.resultingGraph.Add(V_1);
			return;
		}

		internal ICollection<ClassHierarchyNode> BuildHierarchy(HashSet<VariableReference> resolvedVariables)
		{
			V_1 = this.methodContext.get_StackData().get_VariableToDefineUseInfo().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_Key();
					if (resolvedVariables.Contains(V_3) || !this.ShouldConsiderVariable(V_3))
					{
						continue;
					}
					V_4 = this.GetVariableNode(V_3);
					dummyVar0 = this.resultingGraph.Add(V_4);
					V_5 = V_2.get_Value().get_DefinedAt().GetEnumerator();
					try
					{
						while (V_5.MoveNext())
						{
							V_6 = V_5.get_Current();
							this.OnPhiVariableAssigned(V_6, V_4);
						}
					}
					finally
					{
						((IDisposable)V_5).Dispose();
					}
					V_7 = V_2.get_Value().get_UsedAt().GetEnumerator();
					try
					{
						while (V_7.MoveNext())
						{
							V_8 = V_7.get_Current();
							this.OnPhiVariableUsed(V_8, V_4);
						}
					}
					finally
					{
						((IDisposable)V_7).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			this.RemoveImpossibleEdges();
			V_0 = new HashSet<ClassHierarchyNode>();
			V_9 = this.resultingGraph.GetEnumerator();
			try
			{
				while (V_9.MoveNext())
				{
					V_10 = V_9.get_Current();
					if (!V_10.get_IsHardNode())
					{
						continue;
					}
					dummyVar1 = V_0.Add(V_10);
				}
			}
			finally
			{
				((IDisposable)V_9).Dispose();
			}
			this.BuildUpHardNodesHierarchy(V_0);
			return this.resultingGraph;
		}

		protected virtual void BuildUpHardNodesHierarchy(IEnumerable<ClassHierarchyNode> hardNodes)
		{
			V_0 = new Queue<ClassHierarchyNode>(hardNodes);
			V_1 = new HashSet<ClassHierarchyNode>();
			while (V_0.get_Count() > 0)
			{
				V_2 = V_0.Dequeue();
				dummyVar0 = V_1.Add(V_2);
				dummyVar1 = this.resultingGraph.Add(V_2);
				V_3 = V_2.get_NodeType().Resolve();
				V_4 = null;
				if (V_3 == null)
				{
					continue;
				}
				V_4 = V_3.get_BaseType();
				if (V_4 != null)
				{
					V_5 = this.GetTypeNode(V_4);
					V_2.AddSupertype(V_5);
					if (!V_1.Contains(V_5))
					{
						V_0.Enqueue(V_5);
					}
				}
				if (V_3.get_IsInterface())
				{
					V_2.AddSupertype(this.GetTypeNode(this.typeSystem.get_Object()));
				}
				V_7 = V_2.get_NodeType().Resolve().get_Interfaces().GetEnumerator();
				try
				{
					while (V_7.MoveNext())
					{
						V_8 = V_7.get_Current();
						V_9 = this.GetTypeNode(V_8);
						V_2.AddSupertype(V_9);
						if (V_1.Contains(V_9))
						{
							continue;
						}
						V_0.Enqueue(V_9);
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
			this.AddObjectClassNodeIfMIssing();
			return;
		}

		protected virtual ClassHierarchyNode GetTypeNode(TypeReference type)
		{
			V_0 = type.get_FullName();
			if (String.op_Equality(V_0, "System.Byte") || String.op_Equality(V_0, "System.SByte") || String.op_Equality(V_0, "System.Char") || String.op_Equality(V_0, "System.Int16") || String.op_Equality(V_0, "System.UInt16") || String.op_Equality(V_0, "System.Boolean"))
			{
				V_0 = "System.Int32";
			}
			if (!this.typeNameToNode.ContainsKey(V_0))
			{
				V_1 = new ClassHierarchyNode(type);
				this.typeNameToNode.Add(V_0, V_1);
			}
			return this.typeNameToNode.get_Item(V_0);
		}

		protected ClassHierarchyNode GetUseExpressionTypeNode(Instruction instruction, VariableReference variable)
		{
			V_0 = instruction.get_OpCode().get_Code();
			if (V_0 == 112)
			{
				return this.GetTypeNode(instruction.get_Operand() as TypeReference);
			}
			if (ClassHierarchyBuilder.IsConditionalBranch(V_0))
			{
				return this.GetTypeNode(this.typeSystem.get_Boolean());
			}
			if (V_0 == 68)
			{
				return this.GetTypeNode(this.typeSystem.get_UInt32());
			}
			V_1 = this.offsetToExpression.get_Item(instruction.get_Offset());
			return this.GetUseExpressionTypeNode(V_1, variable);
		}

		private ClassHierarchyNode GetUseExpressionTypeNode(Expression expression, VariableReference variable)
		{
			V_2 = expression.get_CodeNodeType();
			if (V_2 > 33)
			{
				if (V_2 > 53)
				{
					if (V_2 == 57)
					{
						return this.GetTypeNode(this.methodContext.get_Method().get_FixedReturnType());
					}
					if (V_2 == 62)
					{
						return this.GetTypeNode((expression as BoxExpression).get_BoxedAs());
					}
				}
				else
				{
					switch (V_2 - 38)
					{
						case 0:
						{
							return this.GetUseInArrayCreation(expression as ArrayCreationExpression, variable);
						}
						case 1:
						{
							return this.GetUseInArrayIndexer(expression as ArrayIndexerExpression, variable);
						}
						case 2:
						{
							return this.GetUseInObjectCreation(expression as ObjectCreationExpression, variable);
						}
						case 3:
						case 5:
						case 6:
						{
							break;
						}
						case 4:
						{
							goto Label0;
						}
						case 7:
						{
							return this.GetTypeNode((expression as StackAllocExpression).get_ExpressionType());
						}
						default:
						{
							if (V_2 - 52 <= 1)
							{
								goto Label0;
							}
							break;
						}
					}
				}
			}
			else
			{
				switch (V_2 - 19)
				{
					case 0:
					{
						goto Label0;
					}
					case 1:
					case 2:
					case 3:
					case 6:
					{
						break;
					}
					case 4:
					{
						return this.GetUseExpressionTypeNode((expression as UnaryExpression).get_Operand(), variable);
					}
					case 5:
					{
						return this.GetUseInBinaryExpression(expression as BinaryExpression, variable);
					}
					case 7:
					{
						return this.GetTypeNode((expression as VariableReferenceExpression).get_Variable().get_VariableType());
					}
					case 8:
					{
						return this.GetTypeNode((expression as VariableDeclarationExpression).get_Variable().get_VariableType());
					}
					default:
					{
						if (V_2 == 31 || V_2 == 33)
						{
							return this.GetTypeNode(this.typeSystem.get_Object());
						}
						break;
					}
				}
			}
			throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
		Label0:
			return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
		}

		private ClassHierarchyNode GetUseInArrayCreation(ArrayCreationExpression arrayCreationExpression, VariableReference variable)
		{
			V_0 = arrayCreationExpression.get_Dimensions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 as VariableReferenceExpression == null || (object)(V_1 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_2 = this.GetTypeNode(this.typeSystem.get_Int32());
					goto Label0;
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			V_0 = arrayCreationExpression.get_Initializer().get_Expressions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_3 = V_0.get_Current();
					if (V_3 as VariableReferenceExpression == null || (object)(V_3 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_2 = this.GetTypeNode(arrayCreationExpression.get_ElementType());
					goto Label0;
				}
				goto Label1;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label0:
			return V_2;
		Label1:
			throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
		}

		private ClassHierarchyNode GetUseInArrayIndexer(ArrayIndexerExpression arrayIndexerExpression, VariableReference variable)
		{
			V_1 = arrayIndexerExpression.get_Indices().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as VariableReferenceExpression == null || (object)(V_2 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_3 = this.GetTypeNode(this.typeSystem.get_Int32());
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			V_0 = new TypeReference("System", "Array", this.typeSystem.get_Object().get_Module(), this.typeSystem.get_Object().get_Scope());
			return this.GetTypeNode(V_0);
		}

		private ClassHierarchyNode GetUseInBinaryExpression(BinaryExpression binaryExpression, VariableReference variable)
		{
			if (binaryExpression.get_Right().get_CodeNodeType() == 26 && (object)(binaryExpression.get_Right() as VariableReferenceExpression).get_Variable() == (object)variable)
			{
				if (binaryExpression.get_Left() as VariableReferenceExpression == null || binaryExpression.get_Left().get_HasType())
				{
					return this.GetTypeNode(binaryExpression.get_Left().get_ExpressionType());
				}
				return this.GetVariableNode((binaryExpression.get_Left() as VariableReferenceExpression).get_Variable());
			}
			if (binaryExpression.get_Left() as VariableReferenceExpression == null || (object)(binaryExpression.get_Left() as VariableReferenceExpression).get_Variable() != (object)variable)
			{
				return this.GetUseExpressionTypeNode(binaryExpression.get_Left(), variable);
			}
			if (binaryExpression.get_Right() as VariableReferenceExpression == null || binaryExpression.get_Right().get_HasType())
			{
				return this.GetTypeNode(binaryExpression.get_Right().get_ExpressionType());
			}
			return this.GetVariableNode((binaryExpression.get_Right() as VariableReferenceExpression).get_Variable());
		}

		private ClassHierarchyNode GetUseInMethodInvocation(MethodInvocationExpression methodInvocationExpression, VariableReference variable)
		{
			V_0 = null;
			V_1 = methodInvocationExpression.get_Arguments().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as VariableReferenceExpression == null || (object)(V_2 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_0 = V_2;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			if (V_0 != null)
			{
				V_3 = methodInvocationExpression.get_Arguments().IndexOf(V_0);
				return this.GetTypeNode(methodInvocationExpression.get_MethodExpression().get_Method().get_Parameters().get_Item(V_3).get_ParameterType());
			}
			if ((object)(methodInvocationExpression.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable() != (object)variable)
			{
				return null;
			}
			return this.GetTypeNode(methodInvocationExpression.get_MethodExpression().get_Member().get_DeclaringType());
		}

		private ClassHierarchyNode GetUseInObjectCreation(ObjectCreationExpression objectCreationExpression, VariableReference variable)
		{
			V_0 = null;
			V_1 = objectCreationExpression.get_Arguments().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as VariableReferenceExpression == null || (object)(V_2 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_0 = V_2;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return this.GetTypeNode(objectCreationExpression.get_Constructor().get_Parameters().get_Item(objectCreationExpression.get_Arguments().IndexOf(V_0)).get_ParameterType());
		}

		protected ClassHierarchyNode GetVariableNode(VariableReference variable)
		{
			V_0 = variable.get_Name();
			if (!this.variableNameToNode.ContainsKey(V_0))
			{
				V_1 = this.MergeWithVariableTypeIfNeeded(variable, new ClassHierarchyNode(variable));
				this.variableNameToNode.Add(V_0, V_1);
			}
			return this.variableNameToNode.get_Item(V_0);
		}

		private IEnumerable<VariableReference> GetVariables(int offset)
		{
			if (!this.methodContext.get_StackData().get_InstructionOffsetToUsedStackVariablesMap().TryGetValue(offset, out V_0))
			{
				return new VariableReference[0];
			}
			V_1 = new List<VariableReference>();
			V_2 = V_0.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!this.methodContext.get_StackData().get_VariableToDefineUseInfo().ContainsKey(V_3))
					{
						continue;
					}
					V_1.Add(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return V_1;
		}

		private static bool IsConditionalBranch(Code instructionOpCode)
		{
			if (instructionOpCode == 57 || instructionOpCode == 44 || instructionOpCode == 56)
			{
				return true;
			}
			return instructionOpCode == 43;
		}

		protected virtual ClassHierarchyNode MergeWithVariableTypeIfNeeded(VariableReference variable, ClassHierarchyNode variableNode)
		{
			if (variable.get_VariableType() != null)
			{
				V_0 = this.GetTypeNode(variable.get_VariableType());
				stackVariable8 = new ClassHierarchyNode[2];
				stackVariable8[0] = variableNode;
				stackVariable8[1] = V_0;
				variableNode = new ClassHierarchyNode(stackVariable8);
			}
			return variableNode;
		}

		protected virtual void OnPhiVariableAssigned(int instructionOffset, ClassHierarchyNode variableNode)
		{
			V_0 = this.offsetToExpression.get_Item(instructionOffset).get_ExpressionType();
			if (V_0 != null)
			{
				V_1 = this.GetTypeNode(V_0);
				V_1.AddSupertype(variableNode);
				dummyVar1 = this.resultingGraph.Add(V_1);
				return;
			}
			V_2 = this.GetVariables(instructionOffset).GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_1 = this.GetVariableNode(V_3);
					V_1.AddSupertype(variableNode);
					dummyVar0 = this.resultingGraph.Add(V_1);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return;
		}

		protected virtual void OnPhiVariableUsed(int instructionOffset, ClassHierarchyNode variableNode)
		{
			return;
		}

		protected virtual void RemoveImpossibleEdges()
		{
			return;
		}

		protected virtual bool ShouldConsiderVariable(VariableReference variableReference)
		{
			return variableReference.get_VariableType() == null;
		}
	}
}