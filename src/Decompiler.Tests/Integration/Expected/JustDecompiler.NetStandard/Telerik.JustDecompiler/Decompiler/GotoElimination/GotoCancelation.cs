using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
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
			if (!(statement is ExpressionStatement))
			{
				return false;
			}
			return (statement as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression;
		}

		private bool IsThrowStatement(Statement labeledClone)
		{
			if (!(labeledClone is ExpressionStatement))
			{
				return false;
			}
			return (labeledClone as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ThrowExpression;
		}

		private void MoveStatements(GotoStatement gotoStatement, IEnumerable<Statement> toMove)
		{
			BlockStatement parent = gotoStatement.Parent as BlockStatement;
			if (parent == null)
			{
				throw new DecompilationException("Goto statement not inside a block.");
			}
			int num = parent.Statements.IndexOf(gotoStatement);
			parent.Statements.RemoveAt(num);
			this.methodContext.GotoStatements.Remove(gotoStatement);
			int num1 = num;
			foreach (Statement statement in toMove)
			{
				parent.AddStatementAt(num1, statement);
				num1++;
			}
			if (!String.IsNullOrEmpty(gotoStatement.Label))
			{
				string label = gotoStatement.Label;
				Statement item = parent.Statements[num];
				this.methodContext.GotoLabels[label] = item;
				item.Label = gotoStatement.Label;
			}
		}

		private IEnumerable<KeyValuePair<GotoStatement, Statement>> Preprocess()
		{
			List<KeyValuePair<GotoStatement, Statement>> keyValuePairs = new List<KeyValuePair<GotoStatement, Statement>>();
			foreach (GotoStatement gotoStatement in this.methodContext.GotoStatements)
			{
				Statement item = this.methodContext.GotoLabels[gotoStatement.TargetLabel];
				keyValuePairs.Add(new KeyValuePair<GotoStatement, Statement>(gotoStatement, item));
			}
			return keyValuePairs;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.body = body;
			this.RemoveGotoStatements();
			return body;
		}

		private void RemoveGotoStatements()
		{
			bool flag = false;
			do
			{
				flag = false;
				foreach (KeyValuePair<GotoStatement, Statement> keyValuePair in this.Preprocess())
				{
					flag = (flag ? true : this.EliminateGotoPair(keyValuePair.Key, keyValuePair.Value));
				}
			}
			while (flag);
		}

		private void RemoveInnerGotoStatementsFromContext(Statement s)
		{
			if (s == null)
			{
				return;
			}
			CodeNodeType codeNodeType = s.CodeNodeType;
			switch (codeNodeType)
			{
				case CodeNodeType.BlockStatement:
				case CodeNodeType.UnsafeBlock:
				{
					using (IEnumerator<Statement> enumerator = (s as BlockStatement).Statements.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							this.RemoveInnerGotoStatementsFromContext(enumerator.Current);
						}
						return;
					}
					break;
				}
				case CodeNodeType.GotoStatement:
				{
					this.methodContext.GotoStatements.Remove(s as GotoStatement);
					return;
				}
				case CodeNodeType.IfStatement:
				{
					IfStatement ifStatement = s as IfStatement;
					this.RemoveInnerGotoStatementsFromContext(ifStatement.Then);
					this.RemoveInnerGotoStatementsFromContext(ifStatement.Else);
					return;
				}
				case CodeNodeType.IfElseIfStatement:
				{
					foreach (KeyValuePair<Expression, BlockStatement> conditionBlock in (s as IfElseIfStatement).ConditionBlocks)
					{
						this.RemoveInnerGotoStatementsFromContext(conditionBlock.Value);
					}
					this.RemoveInnerGotoStatementsFromContext((s as IfElseIfStatement).Else);
					return;
				}
				case CodeNodeType.ExpressionStatement:
				case CodeNodeType.ThrowExpression:
				case CodeNodeType.BreakStatement:
				case CodeNodeType.ContinueStatement:
				case CodeNodeType.CatchClause:
				{
					return;
				}
				case CodeNodeType.WhileStatement:
				{
					this.RemoveInnerGotoStatementsFromContext((s as WhileStatement).Body);
					return;
				}
				case CodeNodeType.DoWhileStatement:
				{
					this.RemoveInnerGotoStatementsFromContext((s as DoWhileStatement).Body);
					return;
				}
				case CodeNodeType.ForStatement:
				{
					this.RemoveInnerGotoStatementsFromContext((s as ForStatement).Body);
					return;
				}
				case CodeNodeType.ForEachStatement:
				{
					this.RemoveInnerGotoStatementsFromContext((s as ForEachStatement).Body);
					return;
				}
				case CodeNodeType.ConditionCase:
				case CodeNodeType.DefaultCase:
				{
					this.RemoveInnerGotoStatementsFromContext((s as SwitchCase).Body);
					return;
				}
				case CodeNodeType.SwitchStatement:
				{
					using (IEnumerator<SwitchCase> enumerator1 = (s as SwitchStatement).Cases.GetEnumerator())
					{
						while (enumerator1.MoveNext())
						{
							this.RemoveInnerGotoStatementsFromContext(enumerator1.Current);
						}
						return;
					}
					break;
				}
				case CodeNodeType.TryStatement:
				{
					this.RemoveInnerGotoStatementsFromContext((s as TryStatement).Try);
					return;
				}
				default:
				{
					if (codeNodeType == CodeNodeType.FixedStatement)
					{
						this.RemoveInnerGotoStatementsFromContext((s as FixedStatement).Body);
						return;
					}
					if (codeNodeType != CodeNodeType.UsingStatement)
					{
						return;
					}
					this.RemoveInnerGotoStatementsFromContext((s as UsingStatement).Body);
					return;
				}
			}
		}

		private bool Targeted(string label)
		{
			bool flag;
			List<GotoStatement>.Enumerator enumerator = this.methodContext.GotoStatements.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.TargetLabel != label)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool TryCopyTargetedBlock(Statement labeledStatement, GotoStatement gotoStatement)
		{
			int i;
			StatementCollection statementCollection = new StatementCollection();
			StatementCollection statementCollection1 = new StatementCollection();
			BlockStatement parent = labeledStatement.Parent as BlockStatement;
			if (parent == null)
			{
				return false;
			}
			int num = parent.Statements.IndexOf(labeledStatement);
			statementCollection1.Add(labeledStatement);
			Statement empty = labeledStatement.CloneStatementOnly();
			empty.Label = String.Empty;
			statementCollection.Add(empty);
			int statementsCount = 15;
			if (this.ContainsLabel(empty))
			{
				return false;
			}
			statementsCount -= this.GetStatementsCount(empty);
			if (statementsCount < 0)
			{
				return false;
			}
			if (!this.IsReturnStatement(empty) && !this.IsThrowStatement(empty))
			{
				for (i = num + 1; i < parent.Statements.Count; i++)
				{
					Statement item = parent.Statements[i];
					if (this.ContainsLabel(item))
					{
						return false;
					}
					statementsCount -= this.GetStatementsCount(item);
					if (statementsCount < 0)
					{
						return false;
					}
					statementCollection1.Add(item);
					statementCollection.Add(item.CloneStatementOnly());
					if (this.IsReturnStatement(item) || this.IsThrowStatement(item))
					{
						break;
					}
				}
				if (i == parent.Statements.Count)
				{
					return false;
				}
			}
			this.MoveStatements(gotoStatement, statementCollection);
			if (!this.Targeted(labeledStatement.Label))
			{
				this.UpdateUntargetedStatement(labeledStatement, statementCollection1);
			}
			return true;
		}

		private bool TryRemoveChainedGoto(Statement labeledStatement, GotoStatement gotoStatement)
		{
			if (!(labeledStatement is GotoStatement))
			{
				return false;
			}
			BlockStatement parent = gotoStatement.Parent as BlockStatement;
			int num = parent.Statements.IndexOf(gotoStatement);
			Statement empty = labeledStatement.CloneStatementOnly();
			empty.Label = String.Empty;
			parent.Statements.RemoveAt(num);
			parent.AddStatementAt(num, empty);
			this.methodContext.GotoStatements.Remove(gotoStatement);
			this.methodContext.GotoStatements.Add(empty as GotoStatement);
			if (!this.Targeted(labeledStatement.Label))
			{
				this.UpdateUntargetedStatement(labeledStatement, (IEnumerable<Statement>)(new Statement[] { labeledStatement }));
			}
			return true;
		}

		private bool TryRemoveGoto(Statement labeledStatement, GotoStatement gotoStatement)
		{
			BlockStatement parent = gotoStatement.Parent as BlockStatement;
			if (parent == null)
			{
				throw new DecompilationException("Goto statement not inside a block.");
			}
			int num = parent.Statements.IndexOf(gotoStatement);
			if (labeledStatement.Parent == parent && parent.Statements.IndexOf(labeledStatement) == num + 1)
			{
				parent.Statements.RemoveAt(num);
				this.methodContext.GotoStatements.Remove(gotoStatement);
				if (!this.Targeted(labeledStatement.Label))
				{
					this.UpdateUntargetedStatement(labeledStatement, new List<Statement>());
				}
				return true;
			}
			if (num == parent.Statements.Count - 1)
			{
				Statement statement = parent.Parent;
				if (statement == null)
				{
					return false;
				}
				if (statement.CodeNodeType == CodeNodeType.ConditionCase || statement.CodeNodeType == CodeNodeType.DefaultCase)
				{
					statement = statement.Parent;
				}
				if (statement.CodeNodeType == CodeNodeType.SwitchStatement || statement.CodeNodeType == CodeNodeType.ForEachStatement || statement.CodeNodeType == CodeNodeType.WhileStatement || statement.CodeNodeType == CodeNodeType.ForStatement || statement.CodeNodeType == CodeNodeType.DoWhileStatement || statement.CodeNodeType == CodeNodeType.IfStatement || statement.CodeNodeType == CodeNodeType.IfElseIfStatement)
				{
					BlockStatement blockStatement = statement.Parent as BlockStatement;
					if (labeledStatement.Parent != blockStatement)
					{
						return false;
					}
					if (blockStatement.Statements.IndexOf(statement) == blockStatement.Statements.IndexOf(labeledStatement) - 1)
					{
						parent.Statements.RemoveAt(num);
						this.methodContext.GotoStatements.Remove(gotoStatement);
						if (statement.CodeNodeType != CodeNodeType.IfStatement && statement.CodeNodeType != CodeNodeType.IfElseIfStatement)
						{
							parent.AddStatementAt(num, new BreakStatement(gotoStatement.UnderlyingSameMethodInstructions));
						}
						if (!this.Targeted(labeledStatement.Label))
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
			this.methodContext.GotoLabels.Remove(labeledStatement.Label);
			labeledStatement.Label = String.Empty;
			if (!this.methodContext.StatementToLogicalConstruct.ContainsKey(labeledStatement))
			{
				return;
			}
			ILogicalConstruct item = this.methodContext.StatementToLogicalConstruct[labeledStatement];
			if (item.Parent is CaseLogicalConstruct)
			{
				return;
			}
			foreach (ISingleEntrySubGraph allPredecessor in item.AllPredecessors)
			{
				if (((ILogicalConstruct)allPredecessor).FollowNode != item)
				{
					continue;
				}
				return;
			}
			foreach (Statement originalStatement in originalStatements)
			{
				(originalStatement.Parent as BlockStatement).Statements.Remove(originalStatement);
				this.RemoveInnerGotoStatementsFromContext(originalStatement);
			}
		}

		private class LableAndGotoFinder : BaseCodeVisitor
		{
			private bool hasLableOrGoto;

			public LableAndGotoFinder()
			{
			}

			public bool CheckForLableOrGoto(Statement statement)
			{
				this.hasLableOrGoto = false;
				this.Visit(statement);
				return this.hasLableOrGoto;
			}

			public override void Visit(ICodeNode node)
			{
				bool flag;
				if (this.hasLableOrGoto || node is GotoStatement)
				{
					flag = true;
				}
				else
				{
					flag = (!(node is Statement) ? false : !String.IsNullOrEmpty((node as Statement).Label));
				}
				this.hasLableOrGoto = flag;
				if (!this.hasLableOrGoto)
				{
					base.Visit(node);
				}
			}
		}

		private class StatementsCounter : BaseCodeVisitor
		{
			private int statementsVisited;

			public StatementsCounter()
			{
			}

			public int GetStatementsCount(Statement statement)
			{
				this.statementsVisited = 1;
				this.Visit(statement);
				return this.statementsVisited;
			}

			public override void VisitBlockStatement(BlockStatement node)
			{
				this.statementsVisited += node.Statements.Count;
				base.VisitBlockStatement(node);
			}
		}
	}
}