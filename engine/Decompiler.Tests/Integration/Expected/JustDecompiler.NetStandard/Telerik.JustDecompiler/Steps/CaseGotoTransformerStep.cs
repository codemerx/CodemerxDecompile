using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Steps
{
	internal class CaseGotoTransformerStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		public CaseGotoTransformerStep()
		{
		}

		private GotoStatement CreateCaseGoto(GotoStatement node, SwitchCase targetedCase)
		{
			return new CaseGotoStatement(node, (SwitchCase)targetedCase.CloneStatementOnly());
		}

		private T GetInnerMostParentOfType<T>(Statement targetedNode)
		where T : Statement
		{
			while (targetedNode != null)
			{
				if (targetedNode is T)
				{
					return (T)(targetedNode as T);
				}
				targetedNode = targetedNode.Parent;
			}
			return default(T);
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

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			return body;
		}

		private void RemoveLabelFromCacheIfNotTargetedAnymore(string gotoLabel, GotoStatement oldGotoStatement)
		{
			foreach (GotoStatement gotoStatement in this.context.MethodContext.GotoStatements)
			{
				if (gotoStatement == oldGotoStatement || !(gotoStatement.TargetLabel == gotoLabel))
				{
					continue;
				}
				return;
			}
			this.context.MethodContext.GotoLabels[gotoLabel].Label = String.Empty;
			this.context.MethodContext.GotoLabels.Remove(gotoLabel);
		}

		private bool TargetIsSwitchCaseEntryStatement(Statement targetedNode)
		{
			if (targetedNode.Parent is BlockStatement && targetedNode.Parent.Parent is SwitchCase && this.IsFirstStatement(targetedNode, targetedNode.Parent as BlockStatement))
			{
				return true;
			}
			if (targetedNode.Parent is SwitchCase && this.IsFirstStatement(targetedNode, targetedNode.Parent as BlockStatement))
			{
				return true;
			}
			return false;
		}

		private void UpdateContext(GotoStatement oldGoto, GotoStatement newGoto)
		{
			this.RemoveLabelFromCacheIfNotTargetedAnymore(oldGoto.TargetLabel, oldGoto);
			int num = this.context.MethodContext.GotoStatements.IndexOf(oldGoto);
			this.context.MethodContext.GotoStatements[num] = newGoto;
			if (this.context.MethodContext.StatementToLogicalConstruct.ContainsKey(oldGoto))
			{
				ILogicalConstruct item = this.context.MethodContext.StatementToLogicalConstruct[oldGoto];
				this.context.MethodContext.StatementToLogicalConstruct.Remove(oldGoto);
				this.context.MethodContext.StatementToLogicalConstruct.Add(newGoto, item);
			}
		}

		public override ICodeNode VisitGotoStatement(GotoStatement node)
		{
			node = (GotoStatement)base.VisitGotoStatement(node);
			string targetLabel = node.TargetLabel;
			Statement item = this.context.MethodContext.GotoLabels[targetLabel];
			if (this.TargetIsSwitchCaseEntryStatement(item))
			{
				SwitchCase innerMostParentOfType = this.GetInnerMostParentOfType<SwitchCase>(item);
				SwitchStatement switchStatement = this.GetInnerMostParentOfType<SwitchStatement>(innerMostParentOfType);
				SwitchStatement innerMostParentOfType1 = this.GetInnerMostParentOfType<SwitchStatement>(node);
				if (innerMostParentOfType1 != null && innerMostParentOfType1 == switchStatement)
				{
					GotoStatement gotoStatement = this.CreateCaseGoto(node, innerMostParentOfType);
					this.UpdateContext(node, gotoStatement);
					return gotoStatement;
				}
			}
			return node;
		}
	}
}