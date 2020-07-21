using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
	internal class ConditionBuilder
	{
		private readonly LogicalFlowBuilderContext logicalBuilderContext;

		private readonly TypeReference booleanTypeReference;

		private readonly TypeSystem typeSystem;

		public ConditionBuilder(LogicalFlowBuilderContext logicalBuilderContext, TypeSystem typeSystem)
		{
			base();
			this.typeSystem = typeSystem;
			this.logicalBuilderContext = logicalBuilderContext;
			this.booleanTypeReference = logicalBuilderContext.get_CFG().get_MethodBody().get_Method().get_Module().get_TypeSystem().get_Boolean();
			return;
		}

		private bool ArePredecessorsLegal(ILogicalConstruct node, HashSet<ILogicalConstruct> allowedPredecessors)
		{
			V_0 = node.get_SameParentPredecessors().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (ILogicalConstruct)V_0.get_Current();
					if (allowedPredecessors.Contains(V_1))
					{
						continue;
					}
					V_2 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return true;
		}

		public void BuildConstructs(ILogicalConstruct theConstruct)
		{
			this.CreateSimpleConditions();
			this.CreateComplexConditions(theConstruct);
			return;
		}

		private bool CanBePartOfComplexCondition(ILogicalConstruct node, HashSet<ILogicalConstruct> nodesInCondition, CFGBlockLogicalConstruct commonSuccessor)
		{
			if (node == null || node as ConditionLogicalConstruct == null || !this.ArePredecessorsLegal(node, nodesInCondition) || !node.get_CFGSuccessors().Contains(commonSuccessor))
			{
				return false;
			}
			return !nodesInCondition.Contains(node);
		}

		private ConditionLogicalConstruct CreateComplexCondition(ConditionLogicalConstruct conditionNode)
		{
			V_0 = conditionNode.get_ConditionExpression();
			V_1 = new HashSet<ILogicalConstruct>();
			V_2 = conditionNode;
			dummyVar0 = V_1.Add(V_2);
			V_3 = V_2.get_TrueSuccessor();
			V_4 = V_2.get_FalseSuccessor();
			while (true)
			{
				if (!this.CanBePartOfComplexCondition(V_3, V_1, V_2.get_FalseCFGSuccessor()))
				{
					if (!this.CanBePartOfComplexCondition(V_4, V_1, V_2.get_TrueCFGSuccessor()))
					{
						break;
					}
					V_6 = V_4 as ConditionLogicalConstruct;
					if (V_6.get_TrueSuccessor() != V_3)
					{
						V_6.Negate(this.typeSystem);
					}
					V_8 = 11;
				}
				else
				{
					V_6 = V_3 as ConditionLogicalConstruct;
					if (V_6.get_FalseSuccessor() != V_4)
					{
						V_6.Negate(this.typeSystem);
					}
					V_8 = 12;
				}
				V_0 = new BinaryExpression(V_8, V_0, V_6.get_ConditionExpression(), this.typeSystem, null, false);
				V_0.set_ExpressionType(this.booleanTypeReference);
				V_2 = V_6;
				V_3 = V_2.get_TrueSuccessor();
				V_4 = V_2.get_FalseSuccessor();
				dummyVar1 = V_1.Add(V_2);
			}
			if (V_1.get_Count() == 1)
			{
				return conditionNode;
			}
			V_5 = new HashSet<ConditionLogicalConstruct>();
			V_9 = V_1.GetEnumerator();
			try
			{
				while (V_9.MoveNext())
				{
					V_10 = (ConditionLogicalConstruct)V_9.get_Current();
					dummyVar2 = V_5.Add(V_10);
				}
			}
			finally
			{
				((IDisposable)V_9).Dispose();
			}
			return new ConditionLogicalConstruct(conditionNode, V_2, V_5, V_0);
		}

		private void CreateComplexConditions(ILogicalConstruct theConstruct)
		{
			if (theConstruct as ConditionLogicalConstruct != null || theConstruct as CFGBlockLogicalConstruct != null)
			{
				return;
			}
			V_1 = theConstruct.get_Children().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (ILogicalConstruct)V_1.get_Current();
					this.CreateComplexConditions(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			while (this.TryTraverseAndMerge(theConstruct))
			{
			}
			return;
		}

		private void CreateSimpleConditions()
		{
			V_0 = this.logicalBuilderContext.get_CFGBlockToLogicalConstructMap().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					stackVariable8 = V_0.get_Current();
					V_1 = stackVariable8[(int)stackVariable8.Length - 1];
					V_2 = V_1.get_TheBlock();
					if ((int)V_2.get_Successors().Length != 2 || !InstructionBlock.op_Inequality(V_2.get_Successors()[0], V_2.get_Successors()[1]) || V_2.get_Last().get_OpCode().get_Code() == 68)
					{
						continue;
					}
					dummyVar0 = ConditionLogicalConstruct.GroupInSimpleConditionConstruct(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private bool TryTraverseAndMerge(ILogicalConstruct theConstruct)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = new Queue<ILogicalConstruct>();
			V_1.Enqueue(theConstruct.get_Entry() as ILogicalConstruct);
			V_2 = false;
			while (V_1.get_Count() > 0)
			{
				V_3 = V_1.Dequeue();
				V_4 = V_3 as ConditionLogicalConstruct;
				if (V_4 != null)
				{
					V_5 = this.CreateComplexCondition(V_4);
					V_2 = V_2 | V_5 != V_3;
					V_3 = V_5;
				}
				dummyVar0 = V_0.Add(V_3);
				V_6 = V_3.get_SameParentSuccessors().GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = (ILogicalConstruct)V_6.get_Current();
						if (V_0.Contains(V_7))
						{
							continue;
						}
						V_1.Enqueue(V_7);
					}
				}
				finally
				{
					((IDisposable)V_6).Dispose();
				}
				while (V_1.get_Count() > 0 && V_0.Contains(V_1.Peek()))
				{
					dummyVar1 = V_1.Dequeue();
				}
			}
			return V_2;
		}
	}
}