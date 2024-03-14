using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveConditionOnlyVariables : BaseCodeVisitor, IDecompilationStep
	{
		private readonly List<RemoveConditionOnlyVariables.VariableStateAndExpression> variables = new List<RemoveConditionOnlyVariables.VariableStateAndExpression>();

		private readonly Stack<RemoveConditionOnlyVariables.Step> states = new Stack<RemoveConditionOnlyVariables.Step>();

		private readonly Stack<Statement> statements = new Stack<Statement>();

		private RemoveConditionOnlyVariables.ProcessStep processStep;

		private MethodSpecificContext methodContext;

		private VariableReference currentVariable;

		public RemoveConditionOnlyVariables()
		{
			this.states.Push(RemoveConditionOnlyVariables.Step.Default);
		}

		private void ChangeVariableState(RemoveConditionOnlyVariables.Step state, VariableReferenceExpression node)
		{
			RemoveConditionOnlyVariables.VariableStateAndExpression value = this.GetValue(node.Variable);
			if (state == RemoveConditionOnlyVariables.Step.Expression && value.VariableState != RemoveConditionOnlyVariables.VariableState.Other && value.NumberOfTimesAssigned <= 1)
			{
				value.VariableState = RemoveConditionOnlyVariables.VariableState.Condition;
				return;
			}
			if (state != RemoveConditionOnlyVariables.Step.Assign)
			{
				value.VariableState = RemoveConditionOnlyVariables.VariableState.Other;
				return;
			}
			if (value.VariableState == RemoveConditionOnlyVariables.VariableState.Condition)
			{
				value.VariableState = RemoveConditionOnlyVariables.VariableState.Assign | RemoveConditionOnlyVariables.VariableState.Condition;
				return;
			}
			value.VariableState = RemoveConditionOnlyVariables.VariableState.Assign;
			RemoveConditionOnlyVariables.VariableStateAndExpression numberOfTimesAssigned = value;
			numberOfTimesAssigned.NumberOfTimesAssigned = numberOfTimesAssigned.NumberOfTimesAssigned + 1;
		}

		private bool ContainsKey(VariableReference variableReference)
		{
			return this.variables.Any<RemoveConditionOnlyVariables.VariableStateAndExpression>((RemoveConditionOnlyVariables.VariableStateAndExpression v) => (object)v.VariableReference == (object)variableReference);
		}

		private Expression GetCurrentStatementExpression(RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression)
		{
			if (variableStateAndExpression.StatementExpressions.Count == 0)
			{
				return null;
			}
			RemoveConditionOnlyVariables.StatementExpression item = variableStateAndExpression.StatementExpressions[variableStateAndExpression.StatementExpressions.Count - 1];
			variableStateAndExpression.StatementExpressions.Remove(item);
			return item.Expression.CloneExpressionOnly();
		}

		private RemoveConditionOnlyVariables.VariableState GetInitialVariableState(RemoveConditionOnlyVariables.Step state)
		{
			switch (state)
			{
				case RemoveConditionOnlyVariables.Step.Assign:
				{
					return RemoveConditionOnlyVariables.VariableState.Assign;
				}
				case RemoveConditionOnlyVariables.Step.Expression:
				{
					return RemoveConditionOnlyVariables.VariableState.Condition;
				}
				case RemoveConditionOnlyVariables.Step.Return:
				{
					return RemoveConditionOnlyVariables.VariableState.Return;
				}
			}
			return RemoveConditionOnlyVariables.VariableState.Declaration;
		}

		private Statement GetParentWhileStatement()
		{
			return this.GetParentWhileStatementAtLevel(0);
		}

		private Statement GetParentWhileStatementAtLevel(int level)
		{
			Statement statement;
			Stack<Statement>.Enumerator enumerator = this.statements.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Statement current = enumerator.Current;
					if (!(current is WhileStatement) && !(current is DoWhileStatement))
					{
						continue;
					}
					if (level <= 0)
					{
						statement = current;
						return statement;
					}
					else
					{
						level--;
					}
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return statement;
		}

		private int GetParentWhileStatementIndex(List<RemoveConditionOnlyVariables.StatementExpression> statementExpressions, Statement parentWhileStatement)
		{
			for (int i = 0; i < statementExpressions.Count; i++)
			{
				if (statementExpressions[i].Statement == parentWhileStatement)
				{
					return i;
				}
			}
			return 0;
		}

		private RemoveConditionOnlyVariables.VariableStateAndExpression GetValue(VariableReference variableReference)
		{
			return (
				from v in this.variables
				where (object)v.VariableReference == (object)variableReference
				select v).FirstOrDefault<RemoveConditionOnlyVariables.VariableStateAndExpression>();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.processStep = RemoveConditionOnlyVariables.ProcessStep.Search;
			this.Visit(body);
			this.RemoveNonConditionVariables();
			if (this.variables.Count > 0)
			{
				this.ReplaceConditionOnlyVariables(body);
			}
			this.methodContext = null;
			this.currentVariable = null;
			this.variables.Clear();
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				return;
			}
			for (int i = 0; i < node.Statements.Count; i++)
			{
				Statement item = node.Statements[i];
				this.TryRemoveConditionVariable(node, item, i);
				this.TryRemoveReturnStatement(node, item, i);
			}
		}

		private void RemoveNonConditionVariables()
		{
			HashSet<RemoveConditionOnlyVariables.VariableStateAndExpression> variableStateAndExpressions = new HashSet<RemoveConditionOnlyVariables.VariableStateAndExpression>();
			foreach (RemoveConditionOnlyVariables.VariableStateAndExpression variable in this.variables)
			{
				if (variable.VariableState != RemoveConditionOnlyVariables.VariableState.Condition && variable.VariableState != RemoveConditionOnlyVariables.VariableState.Return)
				{
					variableStateAndExpressions.Add(variable);
				}
				if (variable.VariableState != RemoveConditionOnlyVariables.VariableState.Condition)
				{
					continue;
				}
				this.RemoveNotSetStatementExpressions(variable, variableStateAndExpressions);
			}
			foreach (RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression in variableStateAndExpressions)
			{
				this.variables.Remove(variableStateAndExpression);
			}
		}

		private void RemoveNotSetStatementExpressions(RemoveConditionOnlyVariables.VariableStateAndExpression keyValuePair, HashSet<RemoveConditionOnlyVariables.VariableStateAndExpression> variablesToRemove)
		{
			Stack<RemoveConditionOnlyVariables.StatementExpression> statementExpressions = new Stack<RemoveConditionOnlyVariables.StatementExpression>(keyValuePair.StatementExpressions);
			keyValuePair.StatementExpressions.Clear();
			foreach (RemoveConditionOnlyVariables.StatementExpression statementExpression in statementExpressions)
			{
				if (statementExpression.Statement == null)
				{
					continue;
				}
				if (statementExpression.ParentWhileStatement == null)
				{
					keyValuePair.StatementExpressions.Add(statementExpression);
				}
				else
				{
					int parentWhileStatementIndex = this.GetParentWhileStatementIndex(keyValuePair.StatementExpressions, statementExpression.ParentWhileStatement);
					keyValuePair.StatementExpressions.Insert(parentWhileStatementIndex, statementExpression);
				}
			}
			if (keyValuePair.StatementExpressions.Count == 0)
			{
				variablesToRemove.Add(keyValuePair);
			}
		}

		private void ReplaceConditionOnlyVariables(BlockStatement body)
		{
			this.processStep = RemoveConditionOnlyVariables.ProcessStep.Replace;
			this.Visit(body);
			this.processStep = RemoveConditionOnlyVariables.ProcessStep.Search;
			this.variables.Clear();
		}

		private void SetVariableAssignmentExpression(Expression expression)
		{
			foreach (RemoveConditionOnlyVariables.VariableStateAndExpression variable in this.variables)
			{
				if ((variable.VariableState & RemoveConditionOnlyVariables.VariableState.Assign) != RemoveConditionOnlyVariables.VariableState.Assign)
				{
					continue;
				}
				variable.StatementExpressions.Add(new RemoveConditionOnlyVariables.StatementExpression(expression, this.GetParentWhileStatement()));
				if ((variable.VariableState & RemoveConditionOnlyVariables.VariableState.Condition) != RemoveConditionOnlyVariables.VariableState.Condition)
				{
					variable.VariableState = RemoveConditionOnlyVariables.VariableState.Declaration;
				}
				else
				{
					variable.VariableState = RemoveConditionOnlyVariables.VariableState.Condition;
				}
			}
		}

		private void SetVariablesExpressionStatements(ConditionStatement node)
		{
			for (int i = this.variables.Count - 1; i >= 0; i--)
			{
				RemoveConditionOnlyVariables.VariableStateAndExpression item = this.variables[i];
				if (item.VariableState != RemoveConditionOnlyVariables.VariableState.Other)
				{
					for (int j = item.StatementExpressions.Count - 1; j >= 0; j--)
					{
						RemoveConditionOnlyVariables.StatementExpression parentWhileStatementAtLevel = item.StatementExpressions[j];
						if (parentWhileStatementAtLevel.Statement == null)
						{
							if (node == parentWhileStatementAtLevel.ParentWhileStatement)
							{
								parentWhileStatementAtLevel.ParentWhileStatement = this.GetParentWhileStatementAtLevel(1);
							}
							parentWhileStatementAtLevel.Statement = node;
						}
					}
				}
			}
		}

		private bool TryGetValue(VariableReference variableReference, out RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression)
		{
			RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression1 = (
				from v in this.variables
				where (object)v.VariableReference == (object)variableReference
				select v).FirstOrDefault<RemoveConditionOnlyVariables.VariableStateAndExpression>();
			variableStateAndExpression = variableStateAndExpression1;
			return variableStateAndExpression1 != null;
		}

		private bool TryGetVariableExpression(Expression variableExpression, out Expression expression)
		{
			RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression;
			if (!(variableExpression is VariableReferenceExpression) || !this.TryGetValue(((VariableReferenceExpression)variableExpression).Variable, out variableStateAndExpression) || variableStateAndExpression.VariableState != RemoveConditionOnlyVariables.VariableState.Condition)
			{
				expression = null;
				return false;
			}
			expression = this.GetCurrentStatementExpression(variableStateAndExpression);
			return expression != null;
		}

		private void TryProcessConditionExpression(ConditionExpression node)
		{
			Expression expression;
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Replace && this.TryGetVariableExpression(node.Condition, out expression))
			{
				node.Condition = expression;
			}
		}

		private void TryProcessConditionStatement(ConditionStatement node)
		{
			Expression expression;
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Replace && this.TryGetVariableExpression(node.Condition, out expression))
			{
				node.Condition = expression;
			}
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search && !(node is WhileStatement) && !(node is DoWhileStatement))
			{
				this.SetVariablesExpressionStatements(node);
			}
		}

		private void TryProcessUnaryExpression(UnaryExpression node)
		{
			Expression expression;
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Replace && this.TryGetVariableExpression(node.Operand, out expression))
			{
				node.Operand = expression;
			}
		}

		private void TryRemoveConditionVariable(BlockStatement node, Statement statement, int index)
		{
			if (!(statement is ExpressionStatement))
			{
				return;
			}
			ExpressionStatement expressionStatement = (ExpressionStatement)statement;
			if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
			{
				return;
			}
			BinaryExpression expression = (BinaryExpression)expressionStatement.Expression;
			if (!(expression.Left is VariableReferenceExpression))
			{
				return;
			}
			if (expression.Right is MethodInvocationExpression && (expression.Left as VariableReferenceExpression).Variable.get_VariableType().get_Name() != TypeCode.Boolean.ToString())
			{
				return;
			}
			VariableReferenceExpression left = (VariableReferenceExpression)expression.Left;
			if (this.ContainsKey(left.Variable))
			{
				this.methodContext.RemoveVariable(left.Variable);
				node.Statements.RemoveAt(index);
			}
		}

		private void TryRemoveReturnStatement(BlockStatement node, Statement statement, int index)
		{
			RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression;
			if (statement.CodeNodeType == CodeNodeType.ExpressionStatement && (statement as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression)
			{
				ReturnExpression expression = (ReturnExpression)(statement as ExpressionStatement).Expression;
				if (!(expression.Value is VariableReferenceExpression))
				{
					return;
				}
				VariableReferenceExpression value = expression.Value as VariableReferenceExpression;
				if (value == null)
				{
					return;
				}
				if (this.TryGetValue(value.Variable, out variableStateAndExpression) && variableStateAndExpression.VariableState == RemoveConditionOnlyVariables.VariableState.Return)
				{
					this.methodContext.RemoveVariable(value.Variable);
					node.Statements.RemoveAt(index);
				}
			}
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.states.Push(RemoveConditionOnlyVariables.Step.Default);
			base.VisitArrayIndexerExpression(node);
			this.states.Pop();
		}

		private void VisitAssignExpression(BinaryExpression node)
		{
			this.currentVariable = null;
			this.states.Push(RemoveConditionOnlyVariables.Step.Assign);
			this.Visit(node.Left);
			this.states.Pop();
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				this.SetVariableAssignmentExpression(node.Right);
			}
			if (node.Right.CodeNodeType == CodeNodeType.LiteralExpression && this.currentVariable != null)
			{
				RemoveConditionOnlyVariables.VariableStateAndExpression value = this.GetValue(this.currentVariable);
				if (value != null && value.VariableState != RemoveConditionOnlyVariables.VariableState.Declaration)
				{
					RemoveConditionOnlyVariables.VariableStateAndExpression numberOfTimesAssigned = value;
					numberOfTimesAssigned.NumberOfTimesAssigned = numberOfTimesAssigned.NumberOfTimesAssigned + 1;
				}
			}
			this.Visit(node.Right);
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression)
			{
				this.VisitAssignExpression(node);
				return;
			}
			this.states.Push(RemoveConditionOnlyVariables.Step.Default);
			base.VisitBinaryExpression(node);
			this.states.Pop();
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			this.ProcessBlock(node);
			foreach (Statement statement in node.Statements)
			{
				this.statements.Push(statement);
				this.Visit(statement);
				this.statements.Pop();
			}
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.TryProcessConditionExpression(node);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			this.Visit(node.Condition);
			this.states.Pop();
			this.Visit(node.Then);
			this.Visit(node.Else);
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			this.Visit(node.Condition);
			this.states.Pop();
			this.Visit(node.Body);
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				this.SetVariablesExpressionStatements(node);
			}
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.Visit(node.Initializer);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			this.Visit(node.Condition);
			this.states.Pop();
			this.Visit(node.Increment);
			this.Visit(node.Body);
		}

		public override void VisitIfStatement(IfStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			this.Visit(node.Condition);
			this.states.Pop();
			this.Visit(node.Then);
			this.Visit(node.Else);
		}

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			this.states.Push(RemoveConditionOnlyVariables.Step.Default);
			base.VisitMethodReferenceExpression(node);
			this.states.Pop();
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.states.Push(RemoveConditionOnlyVariables.Step.Return);
			base.VisitReturnExpression(node);
			this.states.Pop();
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			this.Visit(node.Condition);
			this.states.Pop();
			this.Visit(node.Cases);
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperator.AddressOf)
			{
				return;
			}
			this.TryProcessUnaryExpression(node);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			base.VisitUnaryExpression(node);
			this.states.Pop();
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Replace)
			{
				return;
			}
			RemoveConditionOnlyVariables.Step step = this.states.Peek();
			if (!this.ContainsKey(node.Variable))
			{
				RemoveConditionOnlyVariables.VariableState initialVariableState = this.GetInitialVariableState(step);
				RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression = new RemoveConditionOnlyVariables.VariableStateAndExpression(node.Variable, initialVariableState);
				if (step == RemoveConditionOnlyVariables.Step.Assign)
				{
					RemoveConditionOnlyVariables.VariableStateAndExpression numberOfTimesAssigned = variableStateAndExpression;
					numberOfTimesAssigned.NumberOfTimesAssigned = numberOfTimesAssigned.NumberOfTimesAssigned + 1;
				}
				this.variables.Add(variableStateAndExpression);
			}
			else
			{
				this.ChangeVariableState(step, node);
			}
			base.VisitVariableReferenceExpression(node);
			this.currentVariable = node.Variable;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(RemoveConditionOnlyVariables.Step.Expression);
			this.Visit(node.Condition);
			this.states.Pop();
			this.Visit(node.Body);
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				this.SetVariablesExpressionStatements(node);
			}
		}

		private enum ProcessStep
		{
			Search,
			Replace
		}

		private class StatementExpression
		{
			private Expression expression;

			private Statement statement;

			private Statement parentWhileStatement;

			public Expression Expression
			{
				get
				{
					return this.expression;
				}
				set
				{
					this.expression = value;
				}
			}

			public Statement ParentWhileStatement
			{
				get
				{
					return this.parentWhileStatement;
				}
				set
				{
					this.parentWhileStatement = value;
				}
			}

			public Statement Statement
			{
				get
				{
					return this.statement;
				}
				set
				{
					this.statement = value;
				}
			}

			public StatementExpression(Expression expression, Statement parentWhileStatement)
			{
				this.expression = expression;
				this.parentWhileStatement = parentWhileStatement;
			}
		}

		private enum Step
		{
			Default,
			Assign,
			Expression,
			Return
		}

		[Flags]
		private enum VariableState
		{
			Declaration = 0,
			Assign = 1,
			Condition = 2,
			Return = 4,
			Other = 8
		}

		private class VariableStateAndExpression
		{
			private RemoveConditionOnlyVariables.VariableState variableState;

			private readonly List<RemoveConditionOnlyVariables.StatementExpression> statementExpressions;

			private VariableReference variableReference;

			public int NumberOfTimesAssigned
			{
				get;
				set;
			}

			public List<RemoveConditionOnlyVariables.StatementExpression> StatementExpressions
			{
				get
				{
					return this.statementExpressions;
				}
			}

			public VariableReference VariableReference
			{
				get
				{
					return this.variableReference;
				}
				set
				{
					this.variableReference = value;
				}
			}

			public RemoveConditionOnlyVariables.VariableState VariableState
			{
				get
				{
					return this.variableState;
				}
				set
				{
					this.variableState = value;
				}
			}

			public VariableStateAndExpression(VariableReference variableReference, RemoveConditionOnlyVariables.VariableState variableState)
			{
				this.variableState = variableState;
				this.variableReference = variableReference;
				this.statementExpressions = new List<RemoveConditionOnlyVariables.StatementExpression>();
			}
		}
	}
}