using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Steps
{
	class CaseGotoTransformerStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			Visit(body);
			return body;
		}

		public override ICodeNode VisitGotoStatement(GotoStatement node)
		{
			node = (GotoStatement)base.VisitGotoStatement(node);
			string label = node.TargetLabel;
			Statement targetedNode = context.MethodContext.GotoLabels[label];
			if (TargetIsSwitchCaseEntryStatement(targetedNode))
			{
				SwitchCase targetedCase = GetInnerMostParentOfType<SwitchCase>(targetedNode);
				SwitchStatement targetedCaseSwitchParent = GetInnerMostParentOfType<SwitchStatement>(targetedCase);
				SwitchStatement gotoNodeSwitchParent = GetInnerMostParentOfType<SwitchStatement>(node);
				if (gotoNodeSwitchParent != null && gotoNodeSwitchParent == targetedCaseSwitchParent)
				{ 
					GotoStatement newGoto = CreateCaseGoto(node, targetedCase);
					UpdateContext(node, newGoto);
					return newGoto;
				}
			}
			return node;
		}
  
		private void UpdateContext(GotoStatement oldGoto, GotoStatement newGoto)
		{
			//return;
			string gotoLabel = oldGoto.TargetLabel;
			RemoveLabelFromCacheIfNotTargetedAnymore(gotoLabel, oldGoto);
			int oldGotoIndex = this.context.MethodContext.GotoStatements.IndexOf(oldGoto);
			this.context.MethodContext.GotoStatements[oldGotoIndex] = newGoto;

			if (this.context.MethodContext.StatementToLogicalConstruct.ContainsKey(oldGoto)) 
			{
				ILogicalConstruct value = this.context.MethodContext.StatementToLogicalConstruct[oldGoto];
				this.context.MethodContext.StatementToLogicalConstruct.Remove(oldGoto);
				this.context.MethodContext.StatementToLogicalConstruct.Add(newGoto, value);
			}
		}
  
		private void RemoveLabelFromCacheIfNotTargetedAnymore(string gotoLabel, GotoStatement oldGotoStatement)
		{
			foreach (GotoStatement gotoStatement in this.context.MethodContext.GotoStatements)
			{
				if (gotoStatement == oldGotoStatement)
				{
					continue;
				}
				if (gotoStatement.TargetLabel == gotoLabel)
				{
					return;
				}
			}
			/// if this point is reached, there is no GotoStatement jumping to the target label.

			this.context.MethodContext.GotoLabels[gotoLabel].Label = string.Empty; // clear the label
			this.context.MethodContext.GotoLabels.Remove(gotoLabel);
		}
  
		private GotoStatement CreateCaseGoto(GotoStatement node, SwitchCase targetedCase)
		{
			CaseGotoStatement result = new CaseGotoStatement(node, (SwitchCase)targetedCase.CloneStatementOnly());
			return result;
		}
  
		private T GetInnerMostParentOfType<T>(Statement targetedNode) where T : Statement
		{
			while (targetedNode != null)
			{
				if (targetedNode is T)
				{
					return targetedNode as T;
				}
				targetedNode = targetedNode.Parent;
			}
			return null;
		}
  
		private bool TargetIsSwitchCaseEntryStatement(Statement targetedNode)
		{
			if (targetedNode.Parent is BlockStatement && targetedNode.Parent.Parent is SwitchCase)
			{
				if (IsFirstStatement(targetedNode,targetedNode.Parent as BlockStatement))
				{
					return true;
				}
			}
			if (targetedNode.Parent is SwitchCase)
			{
				if (IsFirstStatement(targetedNode, targetedNode.Parent as BlockStatement))
				{
					return true;
				}
			}
			return false;
		}
  
		private bool IsFirstStatement(Statement targetedNode, BlockStatement blockStatement)
		{
			if (blockStatement == null)
			{
				return false;
			}

			if (!blockStatement.Statements.Contains(targetedNode))
			{
				return false;
			}

			return blockStatement.Statements.IndexOf(targetedNode) == 0;
		}
	}
}
