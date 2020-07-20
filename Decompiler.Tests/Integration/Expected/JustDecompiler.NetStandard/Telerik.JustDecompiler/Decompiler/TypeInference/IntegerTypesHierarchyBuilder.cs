using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class IntegerTypesHierarchyBuilder : ClassHierarchyBuilder
	{
		private HashSet<ClassHierarchyNode> notPossibleBooleanNodes;

		public IntegerTypesHierarchyBuilder(Dictionary<int, Expression> offsetToExpression, DecompilationContext context)
		{
			base(offsetToExpression, context.get_MethodContext().get_ControlFlowGraph().get_OffsetToInstruction(), context);
			this.notPossibleBooleanNodes = new HashSet<ClassHierarchyNode>();
			return;
		}

		private void AddEdge(ClassHierarchyNode biggerTypeNode, ClassHierarchyNode smallerTypeNode)
		{
			smallerTypeNode.AddSupertype(biggerTypeNode);
			dummyVar0 = this.resultingGraph.Add(smallerTypeNode);
			dummyVar1 = this.resultingGraph.Add(biggerTypeNode);
			return;
		}

		protected override void BuildUpHardNodesHierarchy(IEnumerable<ClassHierarchyNode> hardNodes)
		{
			V_0 = this.GetTypeNode(this.typeSystem.get_Boolean());
			V_1 = this.GetTypeNode(this.typeSystem.get_Byte());
			this.AddEdge(V_1, V_0);
			V_0 = this.GetTypeNode(this.typeSystem.get_Byte());
			V_1 = this.GetTypeNode(this.typeSystem.get_Char());
			this.AddEdge(V_1, V_0);
			V_0 = this.GetTypeNode(this.typeSystem.get_Byte());
			V_1 = this.GetTypeNode(this.typeSystem.get_Int16());
			this.AddEdge(V_1, V_0);
			V_0 = this.GetTypeNode(this.typeSystem.get_Char());
			V_1 = this.GetTypeNode(this.typeSystem.get_Int32());
			this.AddEdge(V_1, V_0);
			V_0 = this.GetTypeNode(this.typeSystem.get_Int16());
			V_1 = this.GetTypeNode(this.typeSystem.get_Int32());
			this.AddEdge(V_1, V_0);
			return;
		}

		protected override ClassHierarchyNode GetTypeNode(TypeReference assignedType)
		{
			V_0 = assignedType.get_FullName();
			if (!this.typeNameToNode.ContainsKey(V_0))
			{
				V_1 = new ClassHierarchyNode(assignedType);
				this.typeNameToNode.Add(V_0, V_1);
			}
			return this.typeNameToNode.get_Item(V_0);
		}

		private List<ClassHierarchyNode> GetUsedPhiVariableNodes(int offset)
		{
			V_0 = new List<ClassHierarchyNode>();
			if (this.methodContext.get_StackData().get_InstructionOffsetToAssignedVariableMap().TryGetValue(offset, out V_1) && this.methodContext.get_StackData().get_VariableToDefineUseInfo().ContainsKey(V_1))
			{
				V_0.Add(this.GetVariableNode(V_1));
			}
			if (this.methodContext.get_StackData().get_InstructionOffsetToUsedStackVariablesMap().TryGetValue(offset, out V_2))
			{
				V_0.AddRange(V_2.Where<VariableDefinition>(new Func<VariableDefinition, bool>(this.u003cGetUsedPhiVariableNodesu003eb__8_0)).Select<VariableDefinition, ClassHierarchyNode>(new Func<VariableDefinition, ClassHierarchyNode>(this.u003cGetUsedPhiVariableNodesu003eb__8_1)));
			}
			return V_0;
		}

		private bool IsArithmeticOperation(Code code)
		{
			if (code == 87 || code == 88 || code == 89 || code == 90 || code == 92 || code == 180 || code == 181 || code == 184 || code == 185 || code == 93 || code == 182 || code == 183)
			{
				return true;
			}
			return code == 91;
		}

		private bool IsStackVariable(VariableReference varRef)
		{
			return this.methodContext.get_StackData().get_VariableToDefineUseInfo().ContainsKey(varRef.Resolve());
		}

		protected override ClassHierarchyNode MergeWithVariableTypeIfNeeded(VariableReference variable, ClassHierarchyNode variableNode)
		{
			return variableNode;
		}

		private bool OnlyPhiVariablesUsed(Expression expression)
		{
			switch (expression.get_CodeNodeType() - 22)
			{
				case 0:
				{
					return true;
				}
				case 1:
				case 3:
				{
				Label0:
					return false;
				}
				case 2:
				{
					V_1 = expression as BinaryExpression;
					if (V_1.get_Left() as VariableReferenceExpression == null)
					{
						if (V_1.get_Left() as VariableReferenceExpression != null && this.IsStackVariable((V_1.get_Right() as VariableReferenceExpression).get_Variable()))
						{
							return this.OnlyPhiVariablesUsed(V_1.get_Left());
						}
					}
					else
					{
						if (this.IsStackVariable((V_1.get_Left() as VariableReferenceExpression).get_Variable()))
						{
							return this.OnlyPhiVariablesUsed(V_1.get_Right());
						}
					}
					return false;
				}
				case 4:
				{
					return this.IsStackVariable((expression as VariableReferenceExpression).get_Variable());
				}
				case 5:
				{
					return this.IsStackVariable((expression as VariableDeclarationExpression).get_Variable());
				}
				default:
				{
					goto Label0;
				}
			}
		}

		protected override void OnPhiVariableAssigned(int instructionOffset, ClassHierarchyNode variableNode)
		{
			V_0 = this.offsetToExpression.get_Item(instructionOffset);
			V_1 = V_0.get_ExpressionType();
			if (V_0 as LiteralExpression != null)
			{
				V_3 = (Int32)(V_0 as LiteralExpression).get_Value();
				if (V_3 == 0 || V_3 == 1)
				{
					V_1 = this.typeSystem.get_Boolean();
				}
				else
				{
					if (V_3 > 0xff || V_3 < 0)
					{
						if (V_3 < 0xffff && V_3 >= 0)
						{
							V_1 = this.typeSystem.get_Char();
						}
					}
					else
					{
						V_1 = this.typeSystem.get_Byte();
					}
				}
			}
			V_2 = this.GetTypeNode(V_1);
			V_2.AddSupertype(variableNode);
			dummyVar0 = this.resultingGraph.Add(V_2);
			return;
		}

		protected override void OnPhiVariableUsed(int instructionOffset, ClassHierarchyNode variableNode)
		{
			V_0 = this.offsetToInstruction.get_Item(instructionOffset);
			if (V_0.get_OpCode().get_Code() == 36 || V_0.get_OpCode().get_Code() == 37)
			{
				return;
			}
			V_1 = this.GetUseExpressionTypeNode(V_0, variableNode.get_Variable());
			if (V_0.get_OpCode().get_Code() == 68)
			{
				variableNode.AddSupertype(V_1);
				dummyVar0 = this.resultingGraph.Add(V_1);
				return;
			}
			if (!String.op_Equality(V_1.get_NodeType().get_FullName(), "System.Int32") || !this.OnlyPhiVariablesUsed(this.offsetToExpression.get_Item(V_0.get_Offset())))
			{
				variableNode.AddSupertype(V_1);
				dummyVar2 = this.resultingGraph.Add(V_1);
				return;
			}
			V_3 = this.GetUsedPhiVariableNodes(V_0.get_Offset());
			V_4 = 0;
			while (V_4 < V_3.get_Count())
			{
				V_5 = V_4 + 1;
				while (V_5 < V_3.get_Count())
				{
					V_3.get_Item(V_4).AddSupertype(V_3.get_Item(V_5));
					V_3.get_Item(V_5).AddSupertype(V_3.get_Item(V_4));
					V_5 = V_5 + 1;
				}
				V_4 = V_4 + 1;
			}
			if (this.IsArithmeticOperation(V_0.get_OpCode().get_Code()))
			{
				V_6 = 0;
				while (V_6 < V_3.get_Count())
				{
					dummyVar1 = this.notPossibleBooleanNodes.Add(V_3.get_Item(V_6));
					V_7 = this.GetTypeNode(this.typeSystem.get_Int32());
					if (!V_7.get_CanAssignTo().Contains(V_3.get_Item(V_6)))
					{
						V_7.get_CanAssignTo().Add(V_3.get_Item(V_6));
						V_3.get_Item(V_6).get_SubTypes().Add(V_7);
					}
					V_6 = V_6 + 1;
				}
			}
			return;
		}

		private void RemoveBooleanAsASubtype(ClassHierarchyNode variableNode)
		{
			V_0 = this.GetTypeNode(this.typeSystem.get_Boolean());
			if (variableNode.get_SubTypes().Contains(V_0))
			{
				dummyVar0 = variableNode.get_SubTypes().Remove(V_0);
				dummyVar1 = V_0.get_CanAssignTo().Remove(variableNode);
			}
			return;
		}

		protected override void RemoveImpossibleEdges()
		{
			V_0 = this.notPossibleBooleanNodes.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.RemoveBooleanAsASubtype(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		protected override bool ShouldConsiderVariable(VariableReference variableReference)
		{
			return String.op_Equality(variableReference.get_VariableType().get_FullName(), "System.Int32");
		}
	}
}