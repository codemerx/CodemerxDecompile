using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
	[Obsolete]
	internal class TotalGotoEliminationStep : IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private readonly Dictionary<string, VariableDefinition> labelToVariable;

		private readonly List<VariableDefinition> switchVariables;

		private VariableReference breakVariable;

		private bool usedBreakVariable;

		private VariableReference continueVariable;

		private bool usedContinueVariable;

		private readonly Dictionary<VariableDefinition, Statement> variableToAssignment;

		private readonly Dictionary<VariableDefinition, bool> assignedOnly;

		private TypeSystem typeSystem;

		private BlockStatement body;

		public TotalGotoEliminationStep()
		{
			base();
			this.labelToVariable = new Dictionary<string, VariableDefinition>();
			this.switchVariables = new List<VariableDefinition>();
			this.variableToAssignment = new Dictionary<VariableDefinition, Statement>();
			this.assignedOnly = new Dictionary<VariableDefinition, bool>();
			return;
		}

		private void AddBreakContinueConditional(int index, BlockStatement containingBlock, Statement statement, VariableReference conditionVariable)
		{
			V_0 = new BlockStatement();
			V_0.AddStatement(statement);
			V_1 = new IfStatement(new VariableReferenceExpression(conditionVariable, null), V_0, null);
			containingBlock.AddStatementAt(index, V_1);
			return;
		}

		private void AddDefaultAssignmentsToNewConditionalVariables(BlockStatement body)
		{
			V_0 = this.labelToVariable.get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = new BinaryExpression(26, new VariableReferenceExpression(V_1, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
					body.AddStatementAt(0, new ExpressionStatement(V_2));
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			if (this.usedBreakVariable)
			{
				V_3 = new BinaryExpression(26, new VariableReferenceExpression(this.breakVariable, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
				body.AddStatementAt(0, new ExpressionStatement(V_3));
			}
			if (this.usedContinueVariable)
			{
				V_4 = new BinaryExpression(26, new VariableReferenceExpression(this.continueVariable, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
				body.AddStatementAt(0, new ExpressionStatement(V_4));
			}
			return;
		}

		private void AddLabelVariables()
		{
			V_0 = (new List<Statement>(this.methodContext.get_GotoLabels().get_Values())).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Label();
					V_1.set_Label(String.Empty);
					V_3 = this.GetLabelVariable(V_2);
					stackVariable19 = V_1.get_Parent() as BlockStatement;
					if (stackVariable19 == null)
					{
						throw new ArgumentOutOfRangeException("Label target is not within a block.");
					}
					V_4 = stackVariable19.get_Statements().IndexOf(V_1);
					stackVariable35 = new BinaryExpression(26, new VariableReferenceExpression(V_3, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
					stackVariable35.get_Right().set_ExpressionType(this.methodContext.get_Method().get_Module().get_TypeSystem().get_Boolean());
					V_5 = new ExpressionStatement(stackVariable35);
					V_5.set_Label(V_2);
					this.methodContext.get_GotoLabels().set_Item(V_2, V_5);
					this.variableToAssignment.Add(V_3, V_5);
					stackVariable19.AddStatementAt(V_4, V_5);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private bool AreEqual(Expression first, Expression second)
		{
			return first.Equals(second);
		}

		private int CalculateLevel(Statement statement)
		{
			V_0 = 0;
			while (statement != null)
			{
				statement = statement.get_Parent();
				V_0 = V_0 + 1;
			}
			return V_0;
		}

		private void CleanupUnneededVariables()
		{
			V_0 = this.assignedOnly.get_Keys().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.assignedOnly.get_Item(V_1))
					{
						continue;
					}
					V_2 = this.variableToAssignment.get_Item(V_1);
					dummyVar0 = (V_2.get_Parent() as BlockStatement).get_Statements().Remove(V_2);
					dummyVar1 = this.labelToVariable.Remove(V_1.get_Name().Remove(V_1.get_Name().get_Length() - 5));
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void ClearLabelStatements()
		{
			V_0 = this.methodContext.get_GotoLabels().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_0.get_Current().set_Label(String.Empty);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private BlockStatement CollectStatements(int startingIndex, int endingIndex, BlockStatement containingBlock)
		{
			V_0 = new BlockStatement();
			V_1 = startingIndex;
			while (V_1 < endingIndex)
			{
				V_0.AddStatement(containingBlock.get_Statements().get_Item(V_1));
				containingBlock.get_Statements().RemoveAt(V_1);
				endingIndex = endingIndex - 1;
			}
			return V_0;
		}

		private void EliminateGotoPair(IfStatement gotoStatement, Statement labeledStatement)
		{
			V_0 = this.CalculateLevel(gotoStatement);
			V_1 = this.CalculateLevel(labeledStatement);
			V_2 = this.Precedes(gotoStatement, labeledStatement);
			V_3 = this.ResolveRelation(gotoStatement, labeledStatement);
			while (V_3 != TotalGotoEliminationStep.GotoToLabelRelation.Siblings)
			{
				if (V_3 == 2)
				{
					this.MoveOut(gotoStatement, labeledStatement.get_Label());
				}
				if (V_3 == 1)
				{
					if (V_0 <= V_1)
					{
						if (!V_2)
						{
							V_5 = this.GetSameLevelParent(labeledStatement, gotoStatement);
							this.LiftGoto(gotoStatement, V_5, labeledStatement.get_Label());
						}
						V_4 = this.GetSameLevelParent(labeledStatement, gotoStatement);
						this.MoveIn(gotoStatement, V_4, labeledStatement.get_Label());
					}
					else
					{
						this.MoveOut(gotoStatement, labeledStatement.get_Label());
					}
				}
				V_0 = this.CalculateLevel(gotoStatement);
				V_1 = this.CalculateLevel(labeledStatement);
				V_3 = this.ResolveRelation(gotoStatement, labeledStatement);
				V_2 = this.Precedes(gotoStatement, labeledStatement);
			}
			if (V_2)
			{
				this.EliminateViaIf(gotoStatement, labeledStatement);
				return;
			}
			this.EliminateViaDoWhile(gotoStatement, labeledStatement);
			return;
		}

		private void EliminateViaDoWhile(IfStatement gotoStatement, Statement labeledStatement)
		{
			V_0 = new List<BreakStatement>();
			V_1 = new List<ContinueStatement>();
			V_2 = this.GetOuterBlock(labeledStatement);
			V_3 = V_2.get_Statements().IndexOf(labeledStatement);
			V_4 = V_2.get_Statements().IndexOf(gotoStatement);
			V_5 = this.CollectStatements(V_3, V_4, V_2);
			if (this.ShouldCheck(gotoStatement))
			{
				stackVariable127 = new ContinueAndBreakFinder();
				stackVariable127.Visit(V_5);
				V_0 = stackVariable127.get_Breaks();
				V_1 = stackVariable127.get_Continues();
			}
			V_7 = V_0.GetEnumerator();
			try
			{
				while (V_7.MoveNext())
				{
					V_8 = V_7.get_Current();
					stackVariable29 = this.GetOuterBlock(V_8);
					V_9 = stackVariable29.get_Statements().IndexOf(V_8);
					this.usedBreakVariable = true;
					V_10 = new ExpressionStatement(new BinaryExpression(26, new VariableReferenceExpression(this.breakVariable, null), this.GetLiteralExpression(true), this.typeSystem, null, false));
					stackVariable29.AddStatementAt(V_9, V_10);
				}
			}
			finally
			{
				if (V_7 != null)
				{
					V_7.Dispose();
				}
			}
			V_11 = V_1.GetEnumerator();
			try
			{
				while (V_11.MoveNext())
				{
					V_12 = V_11.get_Current();
					stackVariable60 = this.GetOuterBlock(V_12);
					V_13 = stackVariable60.get_Statements().IndexOf(V_12);
					this.usedContinueVariable = true;
					V_14 = new ExpressionStatement(new BinaryExpression(26, new VariableReferenceExpression(this.continueVariable, null), this.GetLiteralExpression(true), this.typeSystem, null, false));
					stackVariable60.get_Statements().RemoveAt(V_13);
					stackVariable60.AddStatementAt(V_13, new BreakStatement(null));
					stackVariable60.AddStatementAt(V_13, V_14);
				}
			}
			finally
			{
				if (V_11 != null)
				{
					V_11.Dispose();
				}
			}
			V_6 = new DoWhileStatement(gotoStatement.get_Condition(), V_5);
			V_4 = V_2.get_Statements().IndexOf(gotoStatement);
			V_2.AddStatementAt(V_4, V_6);
			dummyVar0 = V_2.get_Statements().Remove(gotoStatement);
			if (V_0.get_Count() > 0)
			{
				this.AddBreakContinueConditional(V_4 + 1, V_2, new BreakStatement(null), this.breakVariable);
			}
			if (V_1.get_Count() > 0)
			{
				this.AddBreakContinueConditional(V_4 + 1, V_2, new ContinueStatement(null), this.continueVariable);
			}
			return;
		}

		private void EliminateViaIf(IfStatement gotoStatement, Statement labeledStatement)
		{
			V_0 = labeledStatement.get_Parent() as BlockStatement;
			V_1 = V_0.get_Statements().IndexOf(gotoStatement);
			V_2 = V_0.get_Statements().IndexOf(labeledStatement);
			if (V_1 == V_2 - 1)
			{
				V_0.get_Statements().RemoveAt(V_1);
				return;
			}
			V_3 = this.CollectStatements(V_1 + 1, V_2, V_0);
			V_4 = Negator.Negate(gotoStatement.get_Condition(), this.typeSystem);
			while (V_3.get_Statements().get_Item(0) as IfStatement != null)
			{
				V_5 = V_3.get_Statements().get_Item(0) as IfStatement;
				if (!this.AreEqual(V_5.get_Condition(), V_4) || V_5.get_Else() != null)
				{
					break;
				}
				V_3.get_Statements().RemoveAt(0);
				V_6 = 0;
				while (V_6 < V_5.get_Then().get_Statements().get_Count())
				{
					V_3.AddStatement(V_5.get_Then().get_Statements().get_Item(V_6));
					V_6 = V_6 + 1;
				}
			}
			gotoStatement.set_Then(V_3);
			gotoStatement.set_Condition(V_4);
			return;
		}

		private void EmbedIntoDefaultIf(GotoStatement jump)
		{
			V_0 = jump.get_Parent() as BlockStatement;
			V_1 = new BlockStatement();
			V_1.AddStatement(jump);
			V_2 = new IfStatement(this.GetLiteralExpression(true), V_1, null);
			V_1.set_Parent(V_2);
			V_3 = V_0.get_Statements().IndexOf(jump);
			V_0.get_Statements().RemoveAt(V_3);
			V_0.AddStatementAt(V_3, V_2);
			if (V_0.get_Parent() as ConditionCase != null && V_0.get_Statements().IndexOf(V_2) == V_0.get_Statements().get_Count())
			{
				V_0.AddStatement(new BreakStatement(null));
			}
			return;
		}

		private void ExtractConditionIntoVariable(VariableReferenceExpression conditionVar, ConditionStatement statement, BlockStatement containingBlock)
		{
			V_0 = new ExpressionStatement(new BinaryExpression(26, conditionVar, statement.get_Condition(), this.typeSystem, null, false));
			containingBlock.AddStatementAt(containingBlock.get_Statements().IndexOf(statement), V_0);
			statement.set_Condition(conditionVar.CloneExpressionOnly());
			return;
		}

		private Expression GetCaseConditionExpression(SwitchCase switchCase)
		{
			if (switchCase as ConditionCase != null)
			{
				return (switchCase as ConditionCase).get_Condition();
			}
			V_0 = 1;
			V_1 = (switchCase.get_Parent() as SwitchStatement).get_Cases().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as DefaultCase != null)
					{
						continue;
					}
					V_0 = V_0 + (Int32)((V_2 as ConditionCase).get_Condition() as LiteralExpression).get_Value();
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return this.GetLiteralExpression(V_0);
		}

		private List<KeyValuePair<IfStatement, Statement>> GetGotoPairs()
		{
			V_0 = new List<KeyValuePair<IfStatement, Statement>>();
			V_1 = this.methodContext.get_GotoStatements().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = this.methodContext.get_GotoLabels().get_Item(V_2.get_TargetLabel());
					V_4 = V_2.get_Parent().get_Parent() as IfStatement;
					if (V_4 == null)
					{
						throw new ArgumentOutOfRangeException("Goto not embeded in condition.");
					}
					V_5 = new KeyValuePair<IfStatement, Statement>(V_4, V_3);
					V_0.Add(V_5);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private VariableDefinition GetLabelVariable(string label)
		{
			if (this.labelToVariable.ContainsKey(label))
			{
				V_0 = this.labelToVariable.get_Item(label);
				this.assignedOnly.set_Item(V_0, false);
				return V_0;
			}
			V_1 = this.methodContext.get_Method().get_Module().get_TypeSystem().get_Boolean();
			V_2 = new VariableDefinition(String.Concat(label, "_cond"), V_1, this.methodContext.get_Method());
			this.labelToVariable.Add(label, V_2);
			this.assignedOnly.Add(V_2, true);
			return V_2;
		}

		private LiteralExpression GetLiteralExpression(object value)
		{
			return new LiteralExpression(value, this.typeSystem, null);
		}

		private Statement GetLowestCommonParent(Statement first, Statement second)
		{
			V_0 = this.GetParentsChain(first);
			V_1 = this.GetParentsChain(second);
			V_2 = null;
			while (V_0.Peek() == V_1.Peek())
			{
				V_2 = V_0.Pop();
				dummyVar0 = V_1.Pop();
			}
			return V_2;
		}

		private BlockStatement GetOuterBlock(Statement statement)
		{
			V_0 = statement.get_Parent();
			while (V_0 as BlockStatement == null)
			{
				V_0 = V_0.get_Parent();
			}
			return V_0 as BlockStatement;
		}

		private Stack<Statement> GetParentsChain(Statement statement)
		{
			V_0 = new Stack<Statement>();
			while (statement != null)
			{
				V_0.Push(statement);
				statement = statement.get_Parent();
			}
			return V_0;
		}

		private Statement GetSameLevelParent(Statement labeledStatement, IfStatement gotoStatement)
		{
			V_0 = this.GetParentsChain(labeledStatement);
			V_1 = gotoStatement.get_Parent() as BlockStatement;
			while (!V_1.get_Statements().Contains(V_0.Peek()))
			{
				dummyVar0 = V_0.Pop();
			}
			return V_0.Peek();
		}

		private TypeReference GetSwitchType(SwitchStatement switchStatement)
		{
			V_0 = null;
			V_1 = switchStatement.get_Cases().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as ConditionCase == null)
					{
						continue;
					}
					V_0 = V_2 as ConditionCase;
					goto Label0;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
		Label0:
			stackVariable14 = V_0.get_Condition() as LiteralExpression;
			if (stackVariable14 == null)
			{
				throw new NotSupportedException("Case should have literal condition.");
			}
			return stackVariable14.get_ExpressionType();
		}

		private bool IsUnconditionalJump(GotoStatement jump)
		{
			V_0 = jump.get_Parent() as BlockStatement;
			if (V_0 == null)
			{
				throw new ArgumentOutOfRangeException("Goto statement outside of block.");
			}
			if (V_0.get_Parent() == null)
			{
				return true;
			}
			if (V_0.get_Parent() as IfStatement == null)
			{
				return true;
			}
			V_1 = V_0.get_Parent() as IfStatement;
			if (V_1.get_Then() == V_0 && V_0.get_Statements().get_Count() == 1 && V_1.get_Else() == null)
			{
				return false;
			}
			return true;
		}

		private void LiftGoto(IfStatement gotoStatement, Statement labelContainingStatement, string label)
		{
			V_0 = this.GetOuterBlock(gotoStatement);
			V_1 = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			this.ExtractConditionIntoVariable(V_1, gotoStatement, V_0);
			V_2 = V_0.get_Statements().IndexOf(gotoStatement);
			V_3 = V_0.get_Statements().IndexOf(labelContainingStatement);
			V_4 = this.CollectStatements(V_3, V_2, V_0);
			V_4.AddStatementAt(0, gotoStatement);
			V_2 = V_0.get_Statements().IndexOf(gotoStatement);
			dummyVar0 = V_0.get_Statements().Remove(gotoStatement);
			V_5 = new DoWhileStatement(gotoStatement.get_Condition().CloneExpressionOnly(), V_4);
			V_0.AddStatementAt(V_2, V_5);
			return;
		}

		private void MoveIn(IfStatement gotoStatement, Statement targetStatement, string label)
		{
			V_0 = gotoStatement.get_Parent() as BlockStatement;
			V_1 = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			this.ExtractConditionIntoVariable(V_1.CloneExpressionOnly() as VariableReferenceExpression, gotoStatement, V_0);
			V_2 = V_0.get_Statements().IndexOf(gotoStatement);
			V_3 = V_0.get_Statements().IndexOf(targetStatement);
			V_4 = this.CollectStatements(V_2 + 1, V_3, V_0);
			V_5 = new IfStatement(new UnaryExpression(1, V_1.CloneExpressionOnly(), null), V_4, null);
			if (V_5.get_Then().get_Statements().get_Count() > 0)
			{
				V_0.AddStatementAt(V_2, V_5);
			}
			dummyVar0 = V_0.get_Statements().Remove(gotoStatement);
			if (targetStatement as DoWhileStatement != null)
			{
				(targetStatement as DoWhileStatement).get_Body().AddStatementAt(0, gotoStatement);
				return;
			}
			if (targetStatement as IfStatement != null)
			{
				V_6 = targetStatement as IfStatement;
				V_6.set_Condition(this.UpdateCondition(V_6.get_Condition(), V_1.CloneExpressionOnly() as VariableReferenceExpression));
				V_6.get_Then().AddStatementAt(0, gotoStatement);
				return;
			}
			if (targetStatement as SwitchCase != null)
			{
				this.MoveInCase(gotoStatement, targetStatement as SwitchCase, label);
				return;
			}
			if (targetStatement as WhileStatement == null)
			{
				throw new NotSupportedException("Unsupported target statement for goto jump.");
			}
			V_7 = targetStatement as WhileStatement;
			V_7.get_Body().AddStatementAt(0, gotoStatement);
			V_7.set_Condition(this.UpdateCondition(V_7.get_Condition(), V_1.CloneExpressionOnly() as VariableReferenceExpression));
			return;
		}

		private void MoveInCase(IfStatement gotoStatement, SwitchCase switchCase, string label)
		{
			V_0 = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			V_1 = this.GetOuterBlock(gotoStatement);
			V_2 = switchCase.get_Parent() as SwitchStatement;
			V_3 = V_1.get_Statements().IndexOf(gotoStatement);
			V_4 = V_1.get_Statements().IndexOf(V_2);
			V_12 = V_2.get_ConditionBlock().get_First().get_Offset();
			stackVariable26 = String.Concat("switch", V_12.ToString());
			V_5 = this.GetSwitchType(V_2);
			V_6 = new VariableDefinition(stackVariable26, V_5, this.methodContext.get_Method());
			this.switchVariables.Add(V_6);
			V_7 = new VariableReferenceExpression(V_6, null);
			this.ExtractConditionIntoVariable(V_7, V_2, V_1);
			V_8 = this.CollectStatements(V_3 + 1, V_4 + 1, V_1);
			V_9 = new BlockStatement();
			V_10 = new BinaryExpression(26, V_7.CloneExpressionOnly(), this.GetCaseConditionExpression(switchCase), this.typeSystem, null, false);
			V_9.AddStatement(new ExpressionStatement(V_10));
			V_11 = new IfStatement(new UnaryExpression(1, V_0, null), V_8, V_9);
			if (V_11.get_Then().get_Statements().get_Count() != 0)
			{
				V_1.AddStatementAt(V_3, V_11);
			}
			dummyVar0 = V_1.get_Statements().Remove(gotoStatement);
			switchCase.get_Body().AddStatementAt(0, gotoStatement);
			return;
		}

		private void MoveOut(IfStatement gotoStatement, string label)
		{
			V_0 = gotoStatement.get_Parent() as BlockStatement;
			V_1 = this.GetOuterBlock(V_0);
			V_2 = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			this.ExtractConditionIntoVariable(V_2.CloneExpressionOnly() as VariableReferenceExpression, gotoStatement, V_0);
			V_3 = V_0.get_Parent();
			if (V_3 as SwitchCase != null)
			{
				V_3 = V_3.get_Parent();
			}
			if (V_0.get_Parent() as SwitchCase != null || V_0.get_Parent() as WhileStatement != null || V_0.get_Parent() as DoWhileStatement != null || V_0.get_Parent() as ForStatement != null || V_0.get_Parent() as ForEachStatement != null)
			{
				V_4 = new BlockStatement();
				V_4.AddStatement(new BreakStatement(null));
				V_5 = new IfStatement(V_2.CloneExpressionOnly(), V_4, null);
				V_6 = V_0.get_Statements().IndexOf(gotoStatement);
				dummyVar0 = V_0.get_Statements().Remove(gotoStatement);
				V_0.AddStatementAt(V_6, V_5);
			}
			else
			{
				if (V_0.get_Parent() as IfStatement == null && V_0.get_Parent() as TryStatement == null && V_0.get_Parent() as IfElseIfStatement == null)
				{
					throw new ArgumentOutOfRangeException("Goto statement can not leave this parent construct.");
				}
				V_7 = V_0.get_Statements().IndexOf(gotoStatement) + 1;
				V_8 = new BlockStatement();
				while (V_7 < V_0.get_Statements().get_Count())
				{
					V_8.AddStatement(V_0.get_Statements().get_Item(V_7));
					V_0.get_Statements().RemoveAt(V_7);
				}
				V_9 = new IfStatement(new UnaryExpression(1, V_2.CloneExpressionOnly(), null), V_8, null);
				dummyVar1 = V_0.get_Statements().Remove(gotoStatement);
				if (V_9.get_Then().get_Statements().get_Count() != 0)
				{
					V_0.AddStatement(V_9);
				}
			}
			V_1.AddStatementAt(V_1.get_Statements().IndexOf(V_3) + 1, gotoStatement);
			return;
		}

		private bool Precedes(Statement first, Statement second)
		{
			V_0 = this.GetParentsChain(first);
			V_1 = this.GetParentsChain(second);
			V_2 = null;
			while (V_0.Peek() == V_1.Peek())
			{
				V_2 = V_0.Pop();
				dummyVar0 = V_1.Pop();
			}
			if (V_2 as SwitchStatement == null)
			{
				if (V_2 as BlockStatement == null)
				{
					throw new ArgumentException("No common block found.");
				}
				V_3 = (V_2 as BlockStatement).get_Statements().IndexOf(V_0.Peek());
				V_4 = (V_2 as BlockStatement).get_Statements().IndexOf(V_1.Peek());
				return V_3 < V_4;
			}
			V_5 = (V_2 as SwitchStatement).get_Cases().GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					if (V_5.get_Current() != V_0.Peek())
					{
						continue;
					}
					V_6 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_5 != null)
				{
					V_5.Dispose();
				}
			}
		Label1:
			return V_6;
		Label0:
			return false;
		}

		internal IEnumerable<KeyValuePair<IfStatement, Statement>> Preprocess()
		{
			this.ReplaceUnconditionalGoto();
			this.AddLabelVariables();
			stackVariable3 = this.GetGotoPairs();
			stackVariable4 = TotalGotoEliminationStep.u003cu003ec.u003cu003e9__17_0;
			if (stackVariable4 == null)
			{
				dummyVar0 = stackVariable4;
				stackVariable4 = new Func<KeyValuePair<IfStatement, Statement>, string>(TotalGotoEliminationStep.u003cu003ec.u003cu003e9.u003cPreprocessu003eb__17_0);
				TotalGotoEliminationStep.u003cu003ec.u003cu003e9__17_0 = stackVariable4;
			}
			stackVariable5 = stackVariable3.OrderBy<KeyValuePair<IfStatement, Statement>, string>(stackVariable4);
			this.breakVariable = new VariableDefinition("breakCondition", this.methodContext.get_Method().get_Module().get_TypeSystem().get_Boolean(), this.methodContext.get_Method());
			dummyVar1 = this.methodContext.get_VariablesToRename().Add(this.breakVariable.Resolve());
			this.continueVariable = new VariableDefinition("continueCondition", this.methodContext.get_Method().get_Module().get_TypeSystem().get_Boolean(), this.methodContext.get_Method());
			dummyVar2 = this.methodContext.get_VariablesToRename().Add(this.continueVariable.Resolve());
			return stackVariable5;
		}

		public virtual BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.typeSystem = this.methodContext.get_Method().get_Module().get_TypeSystem();
			this.body = body;
			this.RemoveGotoStatements();
			this.methodContext.get_Variables().AddRange(this.switchVariables);
			this.methodContext.get_VariablesToRename().UnionWith(this.switchVariables);
			this.methodContext.get_Variables().AddRange(this.labelToVariable.get_Values());
			this.methodContext.get_VariablesToRename().UnionWith(this.labelToVariable.get_Values());
			this.CleanupUnneededVariables();
			this.AddDefaultAssignmentsToNewConditionalVariables(body);
			return body;
		}

		private void RemoveGotoStatements()
		{
			V_0 = this.Preprocess().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.EliminateGotoPair(V_1.get_Key(), V_1.get_Value());
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			this.ClearLabelStatements();
			return;
		}

		private void ReplaceUnconditionalGoto()
		{
			V_0 = this.methodContext.get_GotoStatements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.IsUnconditionalJump(V_1))
					{
						continue;
					}
					this.EmbedIntoDefaultIf(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private TotalGotoEliminationStep.GotoToLabelRelation ResolveRelation(IfStatement gotoStatement, Statement labeledStatement)
		{
			V_0 = this.GetLowestCommonParent(gotoStatement, labeledStatement) as BlockStatement;
			if (V_0 == null)
			{
				return 2;
			}
			if (V_0.get_Statements().Contains(gotoStatement) && V_0.get_Statements().Contains(labeledStatement))
			{
				return 0;
			}
			if (!V_0.get_Statements().Contains(gotoStatement) && !V_0.get_Statements().Contains(labeledStatement))
			{
				return 2;
			}
			return 1;
		}

		private bool ShouldCheck(IfStatement gotoStatement)
		{
			V_0 = gotoStatement.get_Parent();
			while (V_0 != null)
			{
				if (V_0 as SwitchStatement != null || V_0 as ForStatement != null || V_0 as ForEachStatement != null || V_0 as WhileStatement != null || V_0 as DoWhileStatement != null)
				{
					return true;
				}
				V_0 = V_0.get_Parent();
			}
			return false;
		}

		private BinaryExpression UpdateCondition(Expression oldCondition, VariableReferenceExpression conditionVar)
		{
			if (oldCondition as BinaryExpression != null)
			{
				V_0 = oldCondition as BinaryExpression;
				if (V_0.get_Left() as VariableReferenceExpression != null && (object)(V_0.get_Left() as VariableReferenceExpression).get_Variable() == (object)conditionVar.get_Variable())
				{
					return V_0;
				}
			}
			stackVariable9 = new BinaryExpression(11, conditionVar, oldCondition, this.typeSystem, null, false);
			stackVariable9.set_ExpressionType(this.methodContext.get_Method().get_Module().get_TypeSystem().get_Boolean());
			return stackVariable9;
		}

		private enum GotoToLabelRelation
		{
			Siblings,
			DirectlyRelated,
			IndirectlyRelated
		}
	}
}