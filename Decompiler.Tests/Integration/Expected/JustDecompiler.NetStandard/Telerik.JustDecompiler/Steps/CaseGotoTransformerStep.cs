using System;
using System.Collections.Generic;
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
			base();
			return;
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
				if (targetedNode as T != null)
				{
					return (T)(targetedNode as T);
				}
				targetedNode = targetedNode.get_Parent();
			}
			V_0 = default(T);
			return V_0;
		}

		private bool IsFirstStatement(Statement targetedNode, BlockStatement blockStatement)
		{
			if (blockStatement == null)
			{
				return false;
			}
			if (!blockStatement.get_Statements().Contains(targetedNode))
			{
				return false;
			}
			return blockStatement.get_Statements().IndexOf(targetedNode) == 0;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			dummyVar0 = this.Visit(body);
			return body;
		}

		private void RemoveLabelFromCacheIfNotTargetedAnymore(string gotoLabel, GotoStatement oldGotoStatement)
		{
			V_0 = this.context.get_MethodContext().get_GotoStatements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 == oldGotoStatement || !String.op_Equality(V_1.get_TargetLabel(), gotoLabel))
					{
						continue;
					}
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.context.get_MethodContext().get_GotoLabels().get_Item(gotoLabel).set_Label(String.Empty);
			dummyVar0 = this.context.get_MethodContext().get_GotoLabels().Remove(gotoLabel);
		Label0:
			return;
		}

		private bool TargetIsSwitchCaseEntryStatement(Statement targetedNode)
		{
			if (targetedNode.get_Parent() as BlockStatement != null && targetedNode.get_Parent().get_Parent() as SwitchCase != null && this.IsFirstStatement(targetedNode, targetedNode.get_Parent() as BlockStatement))
			{
				return true;
			}
			if (targetedNode.get_Parent() as SwitchCase != null && this.IsFirstStatement(targetedNode, targetedNode.get_Parent() as BlockStatement))
			{
				return true;
			}
			return false;
		}

		private void UpdateContext(GotoStatement oldGoto, GotoStatement newGoto)
		{
			this.RemoveLabelFromCacheIfNotTargetedAnymore(oldGoto.get_TargetLabel(), oldGoto);
			V_1 = this.context.get_MethodContext().get_GotoStatements().IndexOf(oldGoto);
			this.context.get_MethodContext().get_GotoStatements().set_Item(V_1, newGoto);
			if (this.context.get_MethodContext().get_StatementToLogicalConstruct().ContainsKey(oldGoto))
			{
				V_2 = this.context.get_MethodContext().get_StatementToLogicalConstruct().get_Item(oldGoto);
				dummyVar0 = this.context.get_MethodContext().get_StatementToLogicalConstruct().Remove(oldGoto);
				this.context.get_MethodContext().get_StatementToLogicalConstruct().Add(newGoto, V_2);
			}
			return;
		}

		public override ICodeNode VisitGotoStatement(GotoStatement node)
		{
			node = (GotoStatement)this.VisitGotoStatement(node);
			V_0 = node.get_TargetLabel();
			V_1 = this.context.get_MethodContext().get_GotoLabels().get_Item(V_0);
			if (this.TargetIsSwitchCaseEntryStatement(V_1))
			{
				V_2 = this.GetInnerMostParentOfType<SwitchCase>(V_1);
				V_3 = this.GetInnerMostParentOfType<SwitchStatement>(V_2);
				V_4 = this.GetInnerMostParentOfType<SwitchStatement>(node);
				if (V_4 != null && V_4 == V_3)
				{
					V_5 = this.CreateCaseGoto(node, V_2);
					this.UpdateContext(node, V_5);
					return V_5;
				}
			}
			return node;
		}
	}
}