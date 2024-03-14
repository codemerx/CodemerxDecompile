using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Steps;

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
				ILogicalConstruct logicalConstruct;
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this.FalseCFGSuccessor, this.parent as ILogicalConstruct, out logicalConstruct))
				{
					return logicalConstruct;
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
				ILogicalConstruct logicalConstruct;
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this.TrueCFGSuccessor, this.parent as ILogicalConstruct, out logicalConstruct))
				{
					return logicalConstruct;
				}
				return null;
			}
		}

		private ConditionLogicalConstruct(CFGBlockLogicalConstruct cfgConditionBlock)
		{
			this.Entry = cfgConditionBlock;
			this.SetTrueAndFalseSuccessors(cfgConditionBlock);
			this.ConditionExpression = cfgConditionBlock.LogicalConstructExpressions[0];
			base.RedirectChildrenToNewParent((IEnumerable<ILogicalConstruct>)(new ILogicalConstruct[] { cfgConditionBlock }));
			this.AddTrueFalseSuccessors();
			this.LogicalContainer = null;
		}

		public ConditionLogicalConstruct(ConditionLogicalConstruct entry, ConditionLogicalConstruct lastNode, HashSet<ConditionLogicalConstruct> body, Expression conditionExpression)
		{
			this.Entry = entry.FirstBlock;
			this.TrueCFGSuccessor = lastNode.TrueCFGSuccessor;
			this.FalseCFGSuccessor = lastNode.FalseCFGSuccessor;
			this.ConditionExpression = conditionExpression;
			base.RedirectChildrenToNewParent(this.RestoreOriginalCFGNodes(body));
			this.AddTrueFalseSuccessors();
			this.LogicalContainer = null;
		}

		private void AddTrueFalseSuccessors()
		{
			this.ManageSuccessorInCaseOfLoop(this.TrueCFGSuccessor);
			this.ManageSuccessorInCaseOfLoop(this.FalseCFGSuccessor);
		}

		public static ConditionLogicalConstruct GroupInSimpleConditionConstruct(CFGBlockLogicalConstruct cfgConditionBlock)
		{
			return new ConditionLogicalConstruct(cfgConditionBlock);
		}

		private void ManageSuccessorInCaseOfLoop(CFGBlockLogicalConstruct cfgSuccessor)
		{
			if (this.FirstBlock == cfgSuccessor)
			{
				base.AddToSuccessors(cfgSuccessor);
				foreach (CFGBlockLogicalConstruct child in this.Children)
				{
					if (!child.CFGSuccessors.Contains(cfgSuccessor))
					{
						continue;
					}
					base.AddToPredecessors(child);
				}
			}
		}

		public void Negate(TypeSystem typeSystem)
		{
			CFGBlockLogicalConstruct trueCFGSuccessor = this.TrueCFGSuccessor;
			this.TrueCFGSuccessor = this.FalseCFGSuccessor;
			this.FalseCFGSuccessor = trueCFGSuccessor;
			this.ConditionExpression = Negator.Negate(this.ConditionExpression, typeSystem);
		}

		private HashSet<ILogicalConstruct> RestoreOriginalCFGNodes(HashSet<ConditionLogicalConstruct> body)
		{
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			foreach (ConditionLogicalConstruct firstBlock in body)
			{
				if (firstBlock.Parent.Entry == firstBlock)
				{
					firstBlock.Parent.Entry = firstBlock.FirstBlock;
				}
				foreach (CFGBlockLogicalConstruct child in firstBlock.Children)
				{
					child.Parent = firstBlock.Parent;
					child.Parent.Children.Remove(firstBlock);
					logicalConstructs.Add(child);
				}
			}
			return logicalConstructs;
		}

		private void SetTrueAndFalseSuccessors(CFGBlockLogicalConstruct cfgConditionBlock)
		{
			InstructionBlock theBlock = cfgConditionBlock.TheBlock;
			InstructionBlock successors = theBlock.Successors[0];
			InstructionBlock instructionBlocks = theBlock.Successors[1];
			foreach (CFGBlockLogicalConstruct cFGSuccessor in cfgConditionBlock.CFGSuccessors)
			{
				if (cFGSuccessor.TheBlock == successors)
				{
					this.TrueCFGSuccessor = cFGSuccessor;
				}
				if (cFGSuccessor.TheBlock != instructionBlocks)
				{
					continue;
				}
				this.FalseCFGSuccessor = cFGSuccessor;
			}
		}

		protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("ConditionLogicalConstruct");
			stringBuilder.AppendLine("{");
			stringBuilder.Append(String.Format("\t{0}: ", base.NodeILOffset(context, this.FirstBlock)));
			stringBuilder.AppendLine(this.ConditionExpression.ToCodeString());
			stringBuilder.AppendLine(String.Format("\tTrueSuccessorBlock: {0}", base.NodeILOffset(context, this.TrueCFGSuccessor)));
			stringBuilder.AppendLine(String.Format("\tFalseSuccessorBlock: {0}", base.NodeILOffset(context, this.FalseCFGSuccessor)));
			string str = String.Format("\tFollowNode: {0}", base.NodeILOffset(context, base.CFGFollowNode));
			stringBuilder.AppendLine(str);
			stringBuilder.AppendLine("}");
			printedBlocks.UnionWith(this.CFGBlocks);
			return stringBuilder.ToString();
		}
	}
}