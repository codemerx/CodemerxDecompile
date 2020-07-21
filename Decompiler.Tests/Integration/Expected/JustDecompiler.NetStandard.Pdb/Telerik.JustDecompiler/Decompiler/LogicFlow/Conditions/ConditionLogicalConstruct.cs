using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
	internal class ConditionLogicalConstruct : LogicalConstructBase
	{
		public Expression ConditionExpression
		{
			get;
			private set;
		}

		public CFGBlockLogicalConstruct FalseCFGSuccessor
		{
			get;
			private set;
		}

		public ILogicalConstruct FalseSuccessor
		{
			get
			{
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this.get_FalseCFGSuccessor(), this.parent as ILogicalConstruct, out V_0))
				{
					return V_0;
				}
				return null;
			}
		}

		public ILogicalConstruct LogicalContainer
		{
			get;
			set;
		}

		public CFGBlockLogicalConstruct TrueCFGSuccessor
		{
			get;
			private set;
		}

		public ILogicalConstruct TrueSuccessor
		{
			get
			{
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this.get_TrueCFGSuccessor(), this.parent as ILogicalConstruct, out V_0))
				{
					return V_0;
				}
				return null;
			}
		}

		private ConditionLogicalConstruct(CFGBlockLogicalConstruct cfgConditionBlock)
		{
			base();
			this.set_Entry(cfgConditionBlock);
			this.SetTrueAndFalseSuccessors(cfgConditionBlock);
			this.set_ConditionExpression(cfgConditionBlock.get_LogicalConstructExpressions().get_Item(0));
			stackVariable12 = new ILogicalConstruct[1];
			stackVariable12[0] = cfgConditionBlock;
			this.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)stackVariable12);
			this.AddTrueFalseSuccessors();
			this.set_LogicalContainer(null);
			return;
		}

		public ConditionLogicalConstruct(ConditionLogicalConstruct entry, ConditionLogicalConstruct lastNode, HashSet<ConditionLogicalConstruct> body, Expression conditionExpression)
		{
			base();
			this.set_Entry(entry.get_FirstBlock());
			this.set_TrueCFGSuccessor(lastNode.get_TrueCFGSuccessor());
			this.set_FalseCFGSuccessor(lastNode.get_FalseCFGSuccessor());
			this.set_ConditionExpression(conditionExpression);
			this.RedirectChildrenToNewParent(this.RestoreOriginalCFGNodes(body));
			this.AddTrueFalseSuccessors();
			this.set_LogicalContainer(null);
			return;
		}

		private void AddTrueFalseSuccessors()
		{
			this.ManageSuccessorInCaseOfLoop(this.get_TrueCFGSuccessor());
			this.ManageSuccessorInCaseOfLoop(this.get_FalseCFGSuccessor());
			return;
		}

		public static ConditionLogicalConstruct GroupInSimpleConditionConstruct(CFGBlockLogicalConstruct cfgConditionBlock)
		{
			return new ConditionLogicalConstruct(cfgConditionBlock);
		}

		private void ManageSuccessorInCaseOfLoop(CFGBlockLogicalConstruct cfgSuccessor)
		{
			if (this.get_FirstBlock() == cfgSuccessor)
			{
				this.AddToSuccessors(cfgSuccessor);
				V_0 = this.get_Children().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = (CFGBlockLogicalConstruct)V_0.get_Current();
						if (!V_1.get_CFGSuccessors().Contains(cfgSuccessor))
						{
							continue;
						}
						this.AddToPredecessors(V_1);
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
			}
			return;
		}

		public void Negate(TypeSystem typeSystem)
		{
			V_0 = this.get_TrueCFGSuccessor();
			this.set_TrueCFGSuccessor(this.get_FalseCFGSuccessor());
			this.set_FalseCFGSuccessor(V_0);
			this.set_ConditionExpression(Negator.Negate(this.get_ConditionExpression(), typeSystem));
			return;
		}

		private HashSet<ILogicalConstruct> RestoreOriginalCFGNodes(HashSet<ConditionLogicalConstruct> body)
		{
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = body.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Parent().get_Entry() == V_2)
					{
						V_2.get_Parent().set_Entry(V_2.get_FirstBlock());
					}
					V_3 = V_2.get_Children().GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = (CFGBlockLogicalConstruct)V_3.get_Current();
							V_4.set_Parent(V_2.get_Parent());
							dummyVar0 = V_4.get_Parent().get_Children().Remove(V_2);
							dummyVar1 = V_0.Add(V_4);
						}
					}
					finally
					{
						((IDisposable)V_3).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private void SetTrueAndFalseSuccessors(CFGBlockLogicalConstruct cfgConditionBlock)
		{
			stackVariable1 = cfgConditionBlock.get_TheBlock();
			V_0 = stackVariable1.get_Successors()[0];
			V_1 = stackVariable1.get_Successors()[1];
			V_2 = cfgConditionBlock.get_CFGSuccessors().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (InstructionBlock.op_Equality(V_3.get_TheBlock(), V_0))
					{
						this.set_TrueCFGSuccessor(V_3);
					}
					if (!InstructionBlock.op_Equality(V_3.get_TheBlock(), V_1))
					{
						continue;
					}
					this.set_FalseCFGSuccessor(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			return;
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			stackVariable0 = new StringBuilder();
			dummyVar0 = stackVariable0.AppendLine("ConditionLogicalConstruct");
			dummyVar1 = stackVariable0.AppendLine("{");
			dummyVar2 = stackVariable0.Append(String.Format("\t{0}: ", this.NodeILOffset(context, this.get_FirstBlock())));
			dummyVar3 = stackVariable0.AppendLine(this.get_ConditionExpression().ToCodeString());
			dummyVar4 = stackVariable0.AppendLine(String.Format("\tTrueSuccessorBlock: {0}", this.NodeILOffset(context, this.get_TrueCFGSuccessor())));
			dummyVar5 = stackVariable0.AppendLine(String.Format("\tFalseSuccessorBlock: {0}", this.NodeILOffset(context, this.get_FalseCFGSuccessor())));
			V_0 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar6 = stackVariable0.AppendLine(V_0);
			dummyVar7 = stackVariable0.AppendLine("}");
			printedBlocks.UnionWith(this.get_CFGBlocks());
			return stackVariable0.ToString();
		}
	}
}