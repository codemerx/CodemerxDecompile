using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
	internal class GotoCancelation : IDecompilationStep
	{
		private const int MaximumStatementsToCopy = 15;

		private MethodSpecificContext methodContext;

		private BlockStatement body;

		public GotoCancelation()
		{
			base();
			return;
		}

		private bool ContainsLabel(Statement statement)
		{
			return (new GotoCancelation.LableAndGotoFinder()).CheckForLableOrGoto(statement);
		}

		private bool EliminateGotoPair(GotoStatement gotoStatement, Statement labeledStatement)
		{
			if (this.TryRemoveChainedGoto(labeledStatement, gotoStatement))
			{
				return true;
			}
			if (this.TryRemoveGoto(labeledStatement, gotoStatement))
			{
				return true;
			}
			if (this.TryCopyTargetedBlock(labeledStatement, gotoStatement))
			{
				return true;
			}
			return false;
		}

		private int GetStatementsCount(Statement statement)
		{
			return (new GotoCancelation.StatementsCounter()).GetStatementsCount(statement);
		}

		private bool IsReturnStatement(Statement statement)
		{
			if (statement as ExpressionStatement == null)
			{
				return false;
			}
			return (statement as ExpressionStatement).get_Expression().get_CodeNodeType() == 57;
		}

		private bool IsThrowStatement(Statement labeledClone)
		{
			if (labeledClone as ExpressionStatement == null)
			{
				return false;
			}
			return (labeledClone as ExpressionStatement).get_Expression().get_CodeNodeType() == 6;
		}

		private void MoveStatements(GotoStatement gotoStatement, IEnumerable<Statement> toMove)
		{
			V_0 = gotoStatement.get_Parent() as BlockStatement;
			if (V_0 == null)
			{
				throw new DecompilationException("Goto statement not inside a block.");
			}
			V_1 = V_0.get_Statements().IndexOf(gotoStatement);
			V_0.get_Statements().RemoveAt(V_1);
			dummyVar0 = this.methodContext.get_GotoStatements().Remove(gotoStatement);
			V_2 = V_1;
			V_3 = toMove.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					V_0.AddStatementAt(V_2, V_4);
					V_2 = V_2 + 1;
				}
			}
			finally
			{
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			if (!String.IsNullOrEmpty(gotoStatement.get_Label()))
			{
				V_5 = gotoStatement.get_Label();
				V_6 = V_0.get_Statements().get_Item(V_1);
				this.methodContext.get_GotoLabels().set_Item(V_5, V_6);
				V_6.set_Label(gotoStatement.get_Label());
			}
			return;
		}

		private IEnumerable<KeyValuePair<GotoStatement, Statement>> Preprocess()
		{
			V_0 = new List<KeyValuePair<GotoStatement, Statement>>();
			V_1 = this.methodContext.get_GotoStatements().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = this.methodContext.get_GotoLabels().get_Item(V_2.get_TargetLabel());
					V_0.Add(new KeyValuePair<GotoStatement, Statement>(V_2, V_3));
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.body = body;
			this.RemoveGotoStatements();
			return body;
		}

		private void RemoveGotoStatements()
		{
			V_0 = false;
			do
			{
				V_0 = false;
				V_1 = this.Preprocess().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_0)
						{
							stackVariable10 = true;
						}
						else
						{
							stackVariable10 = this.EliminateGotoPair(V_2.get_Key(), V_2.get_Value());
						}
						V_0 = stackVariable10;
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			while (V_0);
			return;
		}

		private void RemoveInnerGotoStatementsFromContext(Statement s)
		{
			if (s == null)
			{
				return;
			}
			V_0 = s.get_CodeNodeType();
			switch (V_0)
			{
				case 0:
				case 1:
				{
					V_1 = (s as BlockStatement).get_Statements().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							this.RemoveInnerGotoStatementsFromContext(V_2);
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
					break;
				}
				case 2:
				{
					dummyVar0 = this.methodContext.get_GotoStatements().Remove(s as GotoStatement);
					return;
				}
				case 3:
				{
					V_3 = s as IfStatement;
					this.RemoveInnerGotoStatementsFromContext(V_3.get_Then());
					this.RemoveInnerGotoStatementsFromContext(V_3.get_Else());
					return;
				}
				case 4:
				{
					V_4 = (s as IfElseIfStatement).get_ConditionBlocks().GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							this.RemoveInnerGotoStatementsFromContext(V_5.get_Value());
						}
					}
					finally
					{
						((IDisposable)V_4).Dispose();
					}
					this.RemoveInnerGotoStatementsFromContext((s as IfElseIfStatement).get_Else());
					return;
				}
				case 5:
				case 6:
				case 9:
				case 10:
				case 16:
				{
				Label0:
					return;
				}
				case 7:
				{
					this.RemoveInnerGotoStatementsFromContext((s as WhileStatement).get_Body());
					return;
				}
				case 8:
				{
					this.RemoveInnerGotoStatementsFromContext((s as DoWhileStatement).get_Body());
					return;
				}
				case 11:
				{
					this.RemoveInnerGotoStatementsFromContext((s as ForStatement).get_Body());
					return;
				}
				case 12:
				{
					this.RemoveInnerGotoStatementsFromContext((s as ForEachStatement).get_Body());
					return;
				}
				case 13:
				case 14:
				{
					this.RemoveInnerGotoStatementsFromContext((s as SwitchCase).get_Body());
					return;
				}
				case 15:
				{
					V_6 = (s as SwitchStatement).get_Cases().GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_7 = V_6.get_Current();
							this.RemoveInnerGotoStatementsFromContext(V_7);
						}
						goto Label0;
					}
					finally
					{
						if (V_6 != null)
						{
							V_6.Dispose();
						}
					}
					break;
				}
				case 17:
				{
					this.RemoveInnerGotoStatementsFromContext((s as TryStatement).get_Try());
					return;
				}
				default:
				{
					if (V_0 == 37)
					{
						this.RemoveInnerGotoStatementsFromContext((s as FixedStatement).get_Body());
						return;
					}
					if (V_0 != 44)
					{
						return;
					}
					this.RemoveInnerGotoStatementsFromContext((s as UsingStatement).get_Body());
					goto Label0;
				}
			}
		}

		private bool Targeted(string label)
		{
			V_0 = this.methodContext.get_GotoStatements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (!String.op_Equality(V_0.get_Current().get_TargetLabel(), label))
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}

		private bool TryCopyTargetedBlock(Statement labeledStatement, GotoStatement gotoStatement)
		{
			V_0 = new StatementCollection();
			V_1 = new StatementCollection();
			V_2 = labeledStatement.get_Parent() as BlockStatement;
			if (V_2 == null)
			{
				return false;
			}
			V_3 = V_2.get_Statements().IndexOf(labeledStatement);
			V_1.Add(labeledStatement);
			V_4 = labeledStatement.CloneStatementOnly();
			V_4.set_Label(String.Empty);
			V_0.Add(V_4);
			V_5 = 15;
			if (this.ContainsLabel(V_4))
			{
				return false;
			}
			V_5 = V_5 - this.GetStatementsCount(V_4);
			if (V_5 < 0)
			{
				return false;
			}
			if (!this.IsReturnStatement(V_4) && !this.IsThrowStatement(V_4))
			{
				V_6 = V_3 + 1;
				while (V_6 < V_2.get_Statements().get_Count())
				{
					V_7 = V_2.get_Statements().get_Item(V_6);
					if (this.ContainsLabel(V_7))
					{
						return false;
					}
					V_5 = V_5 - this.GetStatementsCount(V_7);
					if (V_5 < 0)
					{
						return false;
					}
					V_1.Add(V_7);
					V_0.Add(V_7.CloneStatementOnly());
					if (this.IsReturnStatement(V_7) || this.IsThrowStatement(V_7))
					{
						break;
					}
					V_6 = V_6 + 1;
				}
				if (V_6 == V_2.get_Statements().get_Count())
				{
					return false;
				}
			}
			this.MoveStatements(gotoStatement, V_0);
			if (!this.Targeted(labeledStatement.get_Label()))
			{
				this.UpdateUntargetedStatement(labeledStatement, V_1);
			}
			return true;
		}

		private bool TryRemoveChainedGoto(Statement labeledStatement, GotoStatement gotoStatement)
		{
			if (labeledStatement as GotoStatement == null)
			{
				return false;
			}
			stackVariable4 = gotoStatement.get_Parent() as BlockStatement;
			V_0 = stackVariable4.get_Statements().IndexOf(gotoStatement);
			V_1 = labeledStatement.CloneStatementOnly();
			V_1.set_Label(String.Empty);
			stackVariable4.get_Statements().RemoveAt(V_0);
			stackVariable4.AddStatementAt(V_0, V_1);
			dummyVar0 = this.methodContext.get_GotoStatements().Remove(gotoStatement);
			this.methodContext.get_GotoStatements().Add(V_1 as GotoStatement);
			if (!this.Targeted(labeledStatement.get_Label()))
			{
				stackVariable34 = new Statement[1];
				stackVariable34[0] = labeledStatement;
				this.UpdateUntargetedStatement(labeledStatement, (IEnumerable<Statement>)stackVariable34);
			}
			return true;
		}

		private bool TryRemoveGoto(Statement labeledStatement, GotoStatement gotoStatement)
		{
			V_0 = gotoStatement.get_Parent() as BlockStatement;
			if (V_0 == null)
			{
				throw new DecompilationException("Goto statement not inside a block.");
			}
			V_1 = V_0.get_Statements().IndexOf(gotoStatement);
			if (labeledStatement.get_Parent() == V_0 && V_0.get_Statements().IndexOf(labeledStatement) == V_1 + 1)
			{
				V_0.get_Statements().RemoveAt(V_1);
				dummyVar0 = this.methodContext.get_GotoStatements().Remove(gotoStatement);
				if (!this.Targeted(labeledStatement.get_Label()))
				{
					this.UpdateUntargetedStatement(labeledStatement, new List<Statement>());
				}
				return true;
			}
			if (V_1 == V_0.get_Statements().get_Count() - 1)
			{
				V_2 = V_0.get_Parent();
				if (V_2 == null)
				{
					return false;
				}
				if (V_2.get_CodeNodeType() == 13 || V_2.get_CodeNodeType() == 14)
				{
					V_2 = V_2.get_Parent();
				}
				if (V_2.get_CodeNodeType() == 15 || V_2.get_CodeNodeType() == 12 || V_2.get_CodeNodeType() == 7 || V_2.get_CodeNodeType() == 11 || V_2.get_CodeNodeType() == 8 || V_2.get_CodeNodeType() == 3 || V_2.get_CodeNodeType() == 4)
				{
					V_3 = V_2.get_Parent() as BlockStatement;
					if (labeledStatement.get_Parent() != V_3)
					{
						return false;
					}
					if (V_3.get_Statements().IndexOf(V_2) == V_3.get_Statements().IndexOf(labeledStatement) - 1)
					{
						V_0.get_Statements().RemoveAt(V_1);
						dummyVar1 = this.methodContext.get_GotoStatements().Remove(gotoStatement);
						if (V_2.get_CodeNodeType() != 3 && V_2.get_CodeNodeType() != 4)
						{
							V_0.AddStatementAt(V_1, new BreakStatement(gotoStatement.get_UnderlyingSameMethodInstructions()));
						}
						if (!this.Targeted(labeledStatement.get_Label()))
						{
							this.UpdateUntargetedStatement(labeledStatement, new List<Statement>());
						}
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateUntargetedStatement(Statement labeledStatement, IEnumerable<Statement> originalStatements)
		{
			dummyVar0 = this.methodContext.get_GotoLabels().Remove(labeledStatement.get_Label());
			labeledStatement.set_Label(String.Empty);
			if (!this.methodContext.get_StatementToLogicalConstruct().ContainsKey(labeledStatement))
			{
				return;
			}
			V_0 = this.methodContext.get_StatementToLogicalConstruct().get_Item(labeledStatement);
			if (V_0.get_Parent() as CaseLogicalConstruct != null)
			{
				return;
			}
			V_1 = V_0.get_AllPredecessors().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					if (((ILogicalConstruct)V_1.get_Current()).get_FollowNode() != V_0)
					{
						continue;
					}
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			V_2 = originalStatements.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar1 = (V_3.get_Parent() as BlockStatement).get_Statements().Remove(V_3);
					this.RemoveInnerGotoStatementsFromContext(V_3);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
		Label0:
			return;
		}

		private class LableAndGotoFinder : BaseCodeVisitor
		{
			private bool hasLableOrGoto;

			public LableAndGotoFinder()
			{
				base();
				return;
			}

			public bool CheckForLableOrGoto(Statement statement)
			{
				this.hasLableOrGoto = false;
				this.Visit(statement);
				return this.hasLableOrGoto;
			}

			public override void Visit(ICodeNode node)
			{
				if (this.hasLableOrGoto || node as GotoStatement != null)
				{
					stackVariable3 = true;
				}
				else
				{
					if (node as Statement == null)
					{
						stackVariable3 = false;
					}
					else
					{
						stackVariable3 = !String.IsNullOrEmpty((node as Statement).get_Label());
					}
				}
				this.hasLableOrGoto = stackVariable3;
				if (!this.hasLableOrGoto)
				{
					this.Visit(node);
				}
				return;
			}
		}

		private class StatementsCounter : BaseCodeVisitor
		{
			private int statementsVisited;

			public StatementsCounter()
			{
				base();
				return;
			}

			public int GetStatementsCount(Statement statement)
			{
				this.statementsVisited = 1;
				this.Visit(statement);
				return this.statementsVisited;
			}

			public override void VisitBlockStatement(BlockStatement node)
			{
				this.statementsVisited = this.statementsVisited + node.get_Statements().get_Count();
				this.VisitBlockStatement(node);
				return;
			}
		}
	}
}