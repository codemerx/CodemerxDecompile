using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
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
			this.labelToVariable = new Dictionary<string, VariableDefinition>();
			this.switchVariables = new List<VariableDefinition>();
			this.variableToAssignment = new Dictionary<VariableDefinition, Statement>();
			this.assignedOnly = new Dictionary<VariableDefinition, bool>();
		}

		private void AddBreakContinueConditional(int index, BlockStatement containingBlock, Statement statement, VariableReference conditionVariable)
		{
			BlockStatement blockStatement = new BlockStatement();
			blockStatement.AddStatement(statement);
			IfStatement ifStatement = new IfStatement(new VariableReferenceExpression(conditionVariable, null), blockStatement, null);
			containingBlock.AddStatementAt(index, ifStatement);
		}

		private void AddDefaultAssignmentsToNewConditionalVariables(BlockStatement body)
		{
			foreach (VariableDefinition value in this.labelToVariable.Values)
			{
				BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(value, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
				body.AddStatementAt(0, new ExpressionStatement(binaryExpression));
			}
			if (this.usedBreakVariable)
			{
				BinaryExpression binaryExpression1 = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(this.breakVariable, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
				body.AddStatementAt(0, new ExpressionStatement(binaryExpression1));
			}
			if (this.usedContinueVariable)
			{
				BinaryExpression binaryExpression2 = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(this.continueVariable, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
				body.AddStatementAt(0, new ExpressionStatement(binaryExpression2));
			}
		}

		private void AddLabelVariables()
		{
			foreach (Statement empty in new List<Statement>(this.methodContext.GotoLabels.Values))
			{
				string label = empty.Label;
				empty.Label = String.Empty;
				VariableDefinition labelVariable = this.GetLabelVariable(label);
				BlockStatement parent = empty.Parent as BlockStatement;
				if (parent == null)
				{
					throw new ArgumentOutOfRangeException("Label target is not within a block.");
				}
				int num = parent.Statements.IndexOf(empty);
				BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(labelVariable, null), this.GetLiteralExpression(false), this.typeSystem, null, false);
				binaryExpression.Right.ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean();
				ExpressionStatement expressionStatement = new ExpressionStatement(binaryExpression)
				{
					Label = label
				};
				this.methodContext.GotoLabels[label] = expressionStatement;
				this.variableToAssignment.Add(labelVariable, expressionStatement);
				parent.AddStatementAt(num, expressionStatement);
			}
		}

		private bool AreEqual(Expression first, Expression second)
		{
			return first.Equals(second);
		}

		private int CalculateLevel(Statement statement)
		{
			int num = 0;
			while (statement != null)
			{
				statement = statement.Parent;
				num++;
			}
			return num;
		}

		private void CleanupUnneededVariables()
		{
			foreach (VariableDefinition key in this.assignedOnly.Keys)
			{
				if (!this.assignedOnly[key])
				{
					continue;
				}
				Statement item = this.variableToAssignment[key];
				(item.Parent as BlockStatement).Statements.Remove(item);
				this.labelToVariable.Remove(key.get_Name().Remove(key.get_Name().Length - 5));
			}
		}

		private void ClearLabelStatements()
		{
			foreach (Statement value in this.methodContext.GotoLabels.Values)
			{
				value.Label = String.Empty;
			}
		}

		private BlockStatement CollectStatements(int startingIndex, int endingIndex, BlockStatement containingBlock)
		{
			BlockStatement blockStatement = new BlockStatement();
			int num = startingIndex;
			while (num < endingIndex)
			{
				blockStatement.AddStatement(containingBlock.Statements[num]);
				containingBlock.Statements.RemoveAt(num);
				endingIndex--;
			}
			return blockStatement;
		}

		private void EliminateGotoPair(IfStatement gotoStatement, Statement labeledStatement)
		{
			int num = this.CalculateLevel(gotoStatement);
			int num1 = this.CalculateLevel(labeledStatement);
			bool flag = this.Precedes(gotoStatement, labeledStatement);
			TotalGotoEliminationStep.GotoToLabelRelation gotoToLabelRelation = this.ResolveRelation(gotoStatement, labeledStatement);
			while (gotoToLabelRelation != TotalGotoEliminationStep.GotoToLabelRelation.Siblings)
			{
				if (gotoToLabelRelation == TotalGotoEliminationStep.GotoToLabelRelation.IndirectlyRelated)
				{
					this.MoveOut(gotoStatement, labeledStatement.Label);
				}
				if (gotoToLabelRelation == TotalGotoEliminationStep.GotoToLabelRelation.DirectlyRelated)
				{
					if (num <= num1)
					{
						if (!flag)
						{
							Statement sameLevelParent = this.GetSameLevelParent(labeledStatement, gotoStatement);
							this.LiftGoto(gotoStatement, sameLevelParent, labeledStatement.Label);
						}
						Statement statement = this.GetSameLevelParent(labeledStatement, gotoStatement);
						this.MoveIn(gotoStatement, statement, labeledStatement.Label);
					}
					else
					{
						this.MoveOut(gotoStatement, labeledStatement.Label);
					}
				}
				num = this.CalculateLevel(gotoStatement);
				num1 = this.CalculateLevel(labeledStatement);
				gotoToLabelRelation = this.ResolveRelation(gotoStatement, labeledStatement);
				flag = this.Precedes(gotoStatement, labeledStatement);
			}
			if (flag)
			{
				this.EliminateViaIf(gotoStatement, labeledStatement);
				return;
			}
			this.EliminateViaDoWhile(gotoStatement, labeledStatement);
		}

		private void EliminateViaDoWhile(IfStatement gotoStatement, Statement labeledStatement)
		{
			ICollection<BreakStatement> breakStatements = new List<BreakStatement>();
			ICollection<ContinueStatement> continueStatements = new List<ContinueStatement>();
			BlockStatement outerBlock = this.GetOuterBlock(labeledStatement);
			int num = outerBlock.Statements.IndexOf(labeledStatement);
			int num1 = outerBlock.Statements.IndexOf(gotoStatement);
			BlockStatement blockStatement = this.CollectStatements(num, num1, outerBlock);
			if (this.ShouldCheck(gotoStatement))
			{
				ContinueAndBreakFinder continueAndBreakFinder = new ContinueAndBreakFinder();
				continueAndBreakFinder.Visit(blockStatement);
				breakStatements = continueAndBreakFinder.Breaks;
				continueStatements = continueAndBreakFinder.Continues;
			}
			foreach (BreakStatement breakStatement in breakStatements)
			{
				BlockStatement outerBlock1 = this.GetOuterBlock(breakStatement);
				int num2 = outerBlock1.Statements.IndexOf(breakStatement);
				this.usedBreakVariable = true;
				ExpressionStatement expressionStatement = new ExpressionStatement(new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(this.breakVariable, null), this.GetLiteralExpression(true), this.typeSystem, null, false));
				outerBlock1.AddStatementAt(num2, expressionStatement);
			}
			foreach (ContinueStatement continueStatement in continueStatements)
			{
				BlockStatement blockStatement1 = this.GetOuterBlock(continueStatement);
				int num3 = blockStatement1.Statements.IndexOf(continueStatement);
				this.usedContinueVariable = true;
				ExpressionStatement expressionStatement1 = new ExpressionStatement(new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(this.continueVariable, null), this.GetLiteralExpression(true), this.typeSystem, null, false));
				blockStatement1.Statements.RemoveAt(num3);
				blockStatement1.AddStatementAt(num3, new BreakStatement(null));
				blockStatement1.AddStatementAt(num3, expressionStatement1);
			}
			DoWhileStatement doWhileStatement = new DoWhileStatement(gotoStatement.Condition, blockStatement);
			num1 = outerBlock.Statements.IndexOf(gotoStatement);
			outerBlock.AddStatementAt(num1, doWhileStatement);
			outerBlock.Statements.Remove(gotoStatement);
			if (breakStatements.Count > 0)
			{
				this.AddBreakContinueConditional(num1 + 1, outerBlock, new BreakStatement(null), this.breakVariable);
			}
			if (continueStatements.Count > 0)
			{
				this.AddBreakContinueConditional(num1 + 1, outerBlock, new ContinueStatement(null), this.continueVariable);
			}
		}

		private void EliminateViaIf(IfStatement gotoStatement, Statement labeledStatement)
		{
			BlockStatement parent = labeledStatement.Parent as BlockStatement;
			int num = parent.Statements.IndexOf(gotoStatement);
			int num1 = parent.Statements.IndexOf(labeledStatement);
			if (num == num1 - 1)
			{
				parent.Statements.RemoveAt(num);
				return;
			}
			BlockStatement blockStatement = this.CollectStatements(num + 1, num1, parent);
			Expression expression = Negator.Negate(gotoStatement.Condition, this.typeSystem);
			while (blockStatement.Statements[0] is IfStatement)
			{
				IfStatement item = blockStatement.Statements[0] as IfStatement;
				if (!this.AreEqual(item.Condition, expression) || item.Else != null)
				{
					break;
				}
				blockStatement.Statements.RemoveAt(0);
				for (int i = 0; i < item.Then.Statements.Count; i++)
				{
					blockStatement.AddStatement(item.Then.Statements[i]);
				}
			}
			gotoStatement.Then = blockStatement;
			gotoStatement.Condition = expression;
		}

		private void EmbedIntoDefaultIf(GotoStatement jump)
		{
			BlockStatement parent = jump.Parent as BlockStatement;
			BlockStatement blockStatement = new BlockStatement();
			blockStatement.AddStatement(jump);
			IfStatement ifStatement = new IfStatement(this.GetLiteralExpression(true), blockStatement, null);
			blockStatement.Parent = ifStatement;
			int num = parent.Statements.IndexOf(jump);
			parent.Statements.RemoveAt(num);
			parent.AddStatementAt(num, ifStatement);
			if (parent.Parent is ConditionCase && parent.Statements.IndexOf(ifStatement) == parent.Statements.Count)
			{
				parent.AddStatement(new BreakStatement(null));
			}
		}

		private void ExtractConditionIntoVariable(VariableReferenceExpression conditionVar, ConditionStatement statement, BlockStatement containingBlock)
		{
			ExpressionStatement expressionStatement = new ExpressionStatement(new BinaryExpression(BinaryOperator.Assign, conditionVar, statement.Condition, this.typeSystem, null, false));
			containingBlock.AddStatementAt(containingBlock.Statements.IndexOf(statement), expressionStatement);
			statement.Condition = conditionVar.CloneExpressionOnly();
		}

		private Expression GetCaseConditionExpression(SwitchCase switchCase)
		{
			if (switchCase is ConditionCase)
			{
				return (switchCase as ConditionCase).Condition;
			}
			int value = 1;
			foreach (SwitchCase @case in (switchCase.Parent as SwitchStatement).Cases)
			{
				if (@case is DefaultCase)
				{
					continue;
				}
				value += (Int32)((@case as ConditionCase).Condition as LiteralExpression).Value;
			}
			return this.GetLiteralExpression(value);
		}

		private List<KeyValuePair<IfStatement, Statement>> GetGotoPairs()
		{
			List<KeyValuePair<IfStatement, Statement>> keyValuePairs = new List<KeyValuePair<IfStatement, Statement>>();
			foreach (GotoStatement gotoStatement in this.methodContext.GotoStatements)
			{
				Statement item = this.methodContext.GotoLabels[gotoStatement.TargetLabel];
				IfStatement parent = gotoStatement.Parent.Parent as IfStatement;
				if (parent == null)
				{
					throw new ArgumentOutOfRangeException("Goto not embeded in condition.");
				}
				keyValuePairs.Add(new KeyValuePair<IfStatement, Statement>(parent, item));
			}
			return keyValuePairs;
		}

		private VariableDefinition GetLabelVariable(string label)
		{
			if (this.labelToVariable.ContainsKey(label))
			{
				VariableDefinition item = this.labelToVariable[label];
				this.assignedOnly[item] = false;
				return item;
			}
			TypeReference flag = this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean();
			VariableDefinition variableDefinition = new VariableDefinition(String.Concat(label, "_cond"), flag, this.methodContext.Method);
			this.labelToVariable.Add(label, variableDefinition);
			this.assignedOnly.Add(variableDefinition, true);
			return variableDefinition;
		}

		private LiteralExpression GetLiteralExpression(object value)
		{
			return new LiteralExpression(value, this.typeSystem, null);
		}

		private Statement GetLowestCommonParent(Statement first, Statement second)
		{
			Stack<Statement> parentsChain = this.GetParentsChain(first);
			Stack<Statement> statements = this.GetParentsChain(second);
			Statement statement = null;
			while (parentsChain.Peek() == statements.Peek())
			{
				statement = parentsChain.Pop();
				statements.Pop();
			}
			return statement;
		}

		private BlockStatement GetOuterBlock(Statement statement)
		{
			Statement parent = statement.Parent;
			while (!(parent is BlockStatement))
			{
				parent = parent.Parent;
			}
			return parent as BlockStatement;
		}

		private Stack<Statement> GetParentsChain(Statement statement)
		{
			Stack<Statement> statements = new Stack<Statement>();
			while (statement != null)
			{
				statements.Push(statement);
				statement = statement.Parent;
			}
			return statements;
		}

		private Statement GetSameLevelParent(Statement labeledStatement, IfStatement gotoStatement)
		{
			Stack<Statement> parentsChain = this.GetParentsChain(labeledStatement);
			BlockStatement parent = gotoStatement.Parent as BlockStatement;
			while (!parent.Statements.Contains(parentsChain.Peek()))
			{
				parentsChain.Pop();
			}
			return parentsChain.Peek();
		}

		private TypeReference GetSwitchType(SwitchStatement switchStatement)
		{
			LiteralExpression condition;
			ConditionCase conditionCase = null;
			foreach (SwitchCase @case in switchStatement.Cases)
			{
				if (!(@case is ConditionCase))
				{
					continue;
				}
				conditionCase = @case as ConditionCase;
				condition = conditionCase.Condition as LiteralExpression;
				if (condition == null)
				{
					throw new NotSupportedException("Case should have literal condition.");
				}
				return condition.ExpressionType;
			}
			condition = conditionCase.Condition as LiteralExpression;
			if (condition == null)
			{
				throw new NotSupportedException("Case should have literal condition.");
			}
			return condition.ExpressionType;
		}

		private bool IsUnconditionalJump(GotoStatement jump)
		{
			BlockStatement parent = jump.Parent as BlockStatement;
			if (parent == null)
			{
				throw new ArgumentOutOfRangeException("Goto statement outside of block.");
			}
			if (parent.Parent == null)
			{
				return true;
			}
			if (!(parent.Parent is IfStatement))
			{
				return true;
			}
			IfStatement ifStatement = parent.Parent as IfStatement;
			if (ifStatement.Then == parent && parent.Statements.Count == 1 && ifStatement.Else == null)
			{
				return false;
			}
			return true;
		}

		private void LiftGoto(IfStatement gotoStatement, Statement labelContainingStatement, string label)
		{
			BlockStatement outerBlock = this.GetOuterBlock(gotoStatement);
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			this.ExtractConditionIntoVariable(variableReferenceExpression, gotoStatement, outerBlock);
			int num = outerBlock.Statements.IndexOf(gotoStatement);
			int num1 = outerBlock.Statements.IndexOf(labelContainingStatement);
			BlockStatement blockStatement = this.CollectStatements(num1, num, outerBlock);
			blockStatement.AddStatementAt(0, gotoStatement);
			num = outerBlock.Statements.IndexOf(gotoStatement);
			outerBlock.Statements.Remove(gotoStatement);
			DoWhileStatement doWhileStatement = new DoWhileStatement(gotoStatement.Condition.CloneExpressionOnly(), blockStatement);
			outerBlock.AddStatementAt(num, doWhileStatement);
		}

		private void MoveIn(IfStatement gotoStatement, Statement targetStatement, string label)
		{
			BlockStatement parent = gotoStatement.Parent as BlockStatement;
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			this.ExtractConditionIntoVariable(variableReferenceExpression.CloneExpressionOnly() as VariableReferenceExpression, gotoStatement, parent);
			int num = parent.Statements.IndexOf(gotoStatement);
			int num1 = parent.Statements.IndexOf(targetStatement);
			BlockStatement blockStatement = this.CollectStatements(num + 1, num1, parent);
			IfStatement ifStatement = new IfStatement(new UnaryExpression(UnaryOperator.LogicalNot, variableReferenceExpression.CloneExpressionOnly(), null), blockStatement, null);
			if (ifStatement.Then.Statements.Count > 0)
			{
				parent.AddStatementAt(num, ifStatement);
			}
			parent.Statements.Remove(gotoStatement);
			if (targetStatement is DoWhileStatement)
			{
				(targetStatement as DoWhileStatement).Body.AddStatementAt(0, gotoStatement);
				return;
			}
			if (targetStatement is IfStatement)
			{
				IfStatement ifStatement1 = targetStatement as IfStatement;
				ifStatement1.Condition = this.UpdateCondition(ifStatement1.Condition, variableReferenceExpression.CloneExpressionOnly() as VariableReferenceExpression);
				ifStatement1.Then.AddStatementAt(0, gotoStatement);
				return;
			}
			if (targetStatement is SwitchCase)
			{
				this.MoveInCase(gotoStatement, targetStatement as SwitchCase, label);
				return;
			}
			if (!(targetStatement is WhileStatement))
			{
				throw new NotSupportedException("Unsupported target statement for goto jump.");
			}
			WhileStatement whileStatement = targetStatement as WhileStatement;
			whileStatement.Body.AddStatementAt(0, gotoStatement);
			whileStatement.Condition = this.UpdateCondition(whileStatement.Condition, variableReferenceExpression.CloneExpressionOnly() as VariableReferenceExpression);
		}

		private void MoveInCase(IfStatement gotoStatement, SwitchCase switchCase, string label)
		{
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			BlockStatement outerBlock = this.GetOuterBlock(gotoStatement);
			SwitchStatement parent = switchCase.Parent as SwitchStatement;
			int num = outerBlock.Statements.IndexOf(gotoStatement);
			int num1 = outerBlock.Statements.IndexOf(parent);
			int offset = parent.ConditionBlock.First.get_Offset();
			string str = String.Concat("switch", offset.ToString());
			TypeReference switchType = this.GetSwitchType(parent);
			VariableDefinition variableDefinition = new VariableDefinition(str, switchType, this.methodContext.Method);
			this.switchVariables.Add(variableDefinition);
			VariableReferenceExpression variableReferenceExpression1 = new VariableReferenceExpression(variableDefinition, null);
			this.ExtractConditionIntoVariable(variableReferenceExpression1, parent, outerBlock);
			BlockStatement blockStatement = this.CollectStatements(num + 1, num1 + 1, outerBlock);
			BlockStatement blockStatement1 = new BlockStatement();
			BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, variableReferenceExpression1.CloneExpressionOnly(), this.GetCaseConditionExpression(switchCase), this.typeSystem, null, false);
			blockStatement1.AddStatement(new ExpressionStatement(binaryExpression));
			IfStatement ifStatement = new IfStatement(new UnaryExpression(UnaryOperator.LogicalNot, variableReferenceExpression, null), blockStatement, blockStatement1);
			if (ifStatement.Then.Statements.Count != 0)
			{
				outerBlock.AddStatementAt(num, ifStatement);
			}
			outerBlock.Statements.Remove(gotoStatement);
			switchCase.Body.AddStatementAt(0, gotoStatement);
		}

		private void MoveOut(IfStatement gotoStatement, string label)
		{
			BlockStatement parent = gotoStatement.Parent as BlockStatement;
			BlockStatement outerBlock = this.GetOuterBlock(parent);
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(this.GetLabelVariable(label), null);
			this.ExtractConditionIntoVariable(variableReferenceExpression.CloneExpressionOnly() as VariableReferenceExpression, gotoStatement, parent);
			Statement statement = parent.Parent;
			if (statement is SwitchCase)
			{
				statement = statement.Parent;
			}
			if (parent.Parent is SwitchCase || parent.Parent is WhileStatement || parent.Parent is DoWhileStatement || parent.Parent is ForStatement || parent.Parent is ForEachStatement)
			{
				BlockStatement blockStatement = new BlockStatement();
				blockStatement.AddStatement(new BreakStatement(null));
				IfStatement ifStatement = new IfStatement(variableReferenceExpression.CloneExpressionOnly(), blockStatement, null);
				int num = parent.Statements.IndexOf(gotoStatement);
				parent.Statements.Remove(gotoStatement);
				parent.AddStatementAt(num, ifStatement);
			}
			else
			{
				if (!(parent.Parent is IfStatement) && !(parent.Parent is TryStatement) && !(parent.Parent is IfElseIfStatement))
				{
					throw new ArgumentOutOfRangeException("Goto statement can not leave this parent construct.");
				}
				int num1 = parent.Statements.IndexOf(gotoStatement) + 1;
				BlockStatement blockStatement1 = new BlockStatement();
				while (num1 < parent.Statements.Count)
				{
					blockStatement1.AddStatement(parent.Statements[num1]);
					parent.Statements.RemoveAt(num1);
				}
				IfStatement ifStatement1 = new IfStatement(new UnaryExpression(UnaryOperator.LogicalNot, variableReferenceExpression.CloneExpressionOnly(), null), blockStatement1, null);
				parent.Statements.Remove(gotoStatement);
				if (ifStatement1.Then.Statements.Count != 0)
				{
					parent.AddStatement(ifStatement1);
				}
			}
			outerBlock.AddStatementAt(outerBlock.Statements.IndexOf(statement) + 1, gotoStatement);
		}

		private bool Precedes(Statement first, Statement second)
		{
			bool flag;
			Stack<Statement> parentsChain = this.GetParentsChain(first);
			Stack<Statement> statements = this.GetParentsChain(second);
			Statement statement = null;
			while (parentsChain.Peek() == statements.Peek())
			{
				statement = parentsChain.Pop();
				statements.Pop();
			}
			if (!(statement is SwitchStatement))
			{
				if (!(statement is BlockStatement))
				{
					throw new ArgumentException("No common block found.");
				}
				int num = (statement as BlockStatement).Statements.IndexOf(parentsChain.Peek());
				int num1 = (statement as BlockStatement).Statements.IndexOf(statements.Peek());
				return num < num1;
			}
			using (IEnumerator<SwitchCase> enumerator = (statement as SwitchStatement).Cases.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != parentsChain.Peek())
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		internal IEnumerable<KeyValuePair<IfStatement, Statement>> Preprocess()
		{
			this.ReplaceUnconditionalGoto();
			this.AddLabelVariables();
			IOrderedEnumerable<KeyValuePair<IfStatement, Statement>> gotoPairs = 
				from x in this.GetGotoPairs()
				orderby x.Value.Label
				select x;
			this.breakVariable = new VariableDefinition("breakCondition", this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean(), this.methodContext.Method);
			this.methodContext.VariablesToRename.Add(this.breakVariable.Resolve());
			this.continueVariable = new VariableDefinition("continueCondition", this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean(), this.methodContext.Method);
			this.methodContext.VariablesToRename.Add(this.continueVariable.Resolve());
			return gotoPairs;
		}

		public virtual BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.typeSystem = this.methodContext.Method.get_Module().get_TypeSystem();
			this.body = body;
			this.RemoveGotoStatements();
			this.methodContext.Variables.AddRange(this.switchVariables);
			this.methodContext.VariablesToRename.UnionWith(this.switchVariables);
			this.methodContext.Variables.AddRange(this.labelToVariable.Values);
			this.methodContext.VariablesToRename.UnionWith(this.labelToVariable.Values);
			this.CleanupUnneededVariables();
			this.AddDefaultAssignmentsToNewConditionalVariables(body);
			return body;
		}

		private void RemoveGotoStatements()
		{
			foreach (KeyValuePair<IfStatement, Statement> keyValuePair in this.Preprocess())
			{
				this.EliminateGotoPair(keyValuePair.Key, keyValuePair.Value);
			}
			this.ClearLabelStatements();
		}

		private void ReplaceUnconditionalGoto()
		{
			foreach (GotoStatement gotoStatement in this.methodContext.GotoStatements)
			{
				if (!this.IsUnconditionalJump(gotoStatement))
				{
					continue;
				}
				this.EmbedIntoDefaultIf(gotoStatement);
			}
		}

		private TotalGotoEliminationStep.GotoToLabelRelation ResolveRelation(IfStatement gotoStatement, Statement labeledStatement)
		{
			BlockStatement lowestCommonParent = this.GetLowestCommonParent(gotoStatement, labeledStatement) as BlockStatement;
			if (lowestCommonParent == null)
			{
				return TotalGotoEliminationStep.GotoToLabelRelation.IndirectlyRelated;
			}
			if (lowestCommonParent.Statements.Contains(gotoStatement) && lowestCommonParent.Statements.Contains(labeledStatement))
			{
				return TotalGotoEliminationStep.GotoToLabelRelation.Siblings;
			}
			if (!lowestCommonParent.Statements.Contains(gotoStatement) && !lowestCommonParent.Statements.Contains(labeledStatement))
			{
				return TotalGotoEliminationStep.GotoToLabelRelation.IndirectlyRelated;
			}
			return TotalGotoEliminationStep.GotoToLabelRelation.DirectlyRelated;
		}

		private bool ShouldCheck(IfStatement gotoStatement)
		{
			for (Statement i = gotoStatement.Parent; i != null; i = i.Parent)
			{
				if (i is SwitchStatement || i is ForStatement || i is ForEachStatement || i is WhileStatement || i is DoWhileStatement)
				{
					return true;
				}
			}
			return false;
		}

		private BinaryExpression UpdateCondition(Expression oldCondition, VariableReferenceExpression conditionVar)
		{
			if (oldCondition is BinaryExpression)
			{
				BinaryExpression binaryExpression = oldCondition as BinaryExpression;
				if (binaryExpression.Left is VariableReferenceExpression && (object)(binaryExpression.Left as VariableReferenceExpression).Variable == (object)conditionVar.Variable)
				{
					return binaryExpression;
				}
			}
			return new BinaryExpression(BinaryOperator.LogicalOr, conditionVar, oldCondition, this.typeSystem, null, false)
			{
				ExpressionType = this.methodContext.Method.get_Module().get_TypeSystem().get_Boolean()
			};
		}

		private enum GotoToLabelRelation
		{
			Siblings,
			DirectlyRelated,
			IndirectlyRelated
		}
	}
}