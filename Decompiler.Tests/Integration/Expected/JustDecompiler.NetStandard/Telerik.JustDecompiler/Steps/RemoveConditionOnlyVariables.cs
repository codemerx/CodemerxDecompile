using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveConditionOnlyVariables : BaseCodeVisitor, IDecompilationStep
	{
		private readonly List<RemoveConditionOnlyVariables.VariableStateAndExpression> variables;

		private readonly Stack<RemoveConditionOnlyVariables.Step> states;

		private readonly Stack<Statement> statements;

		private RemoveConditionOnlyVariables.ProcessStep processStep;

		private MethodSpecificContext methodContext;

		private VariableReference currentVariable;

		public RemoveConditionOnlyVariables()
		{
			this.variables = new List<RemoveConditionOnlyVariables.VariableStateAndExpression>();
			this.states = new Stack<RemoveConditionOnlyVariables.Step>();
			this.statements = new Stack<Statement>();
			base();
			this.states.Push(0);
			return;
		}

		private void ChangeVariableState(RemoveConditionOnlyVariables.Step state, VariableReferenceExpression node)
		{
			V_0 = this.GetValue(node.get_Variable());
			if (state == 2 && V_0.get_VariableState() != 8 && V_0.get_NumberOfTimesAssigned() <= 1)
			{
				V_0.set_VariableState(2);
				return;
			}
			if (state != 1)
			{
				V_0.set_VariableState(8);
				return;
			}
			if (V_0.get_VariableState() == 2)
			{
				V_0.set_VariableState(3);
				return;
			}
			V_0.set_VariableState(1);
			stackVariable15 = V_0;
			stackVariable15.set_NumberOfTimesAssigned(stackVariable15.get_NumberOfTimesAssigned() + 1);
			return;
		}

		private bool ContainsKey(VariableReference variableReference)
		{
			V_0 = new RemoveConditionOnlyVariables.u003cu003ec__DisplayClass10_0();
			V_0.variableReference = variableReference;
			return this.variables.Any<RemoveConditionOnlyVariables.VariableStateAndExpression>(new Func<RemoveConditionOnlyVariables.VariableStateAndExpression, bool>(V_0.u003cContainsKeyu003eb__0));
		}

		private Expression GetCurrentStatementExpression(RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression)
		{
			if (variableStateAndExpression.get_StatementExpressions().get_Count() == 0)
			{
				return null;
			}
			V_0 = variableStateAndExpression.get_StatementExpressions().get_Item(variableStateAndExpression.get_StatementExpressions().get_Count() - 1);
			dummyVar0 = variableStateAndExpression.get_StatementExpressions().Remove(V_0);
			return V_0.get_Expression().CloneExpressionOnly();
		}

		private RemoveConditionOnlyVariables.VariableState GetInitialVariableState(RemoveConditionOnlyVariables.Step state)
		{
			switch (state - 1)
			{
				case 0:
				{
					return 1;
				}
				case 1:
				{
					return 2;
				}
				case 2:
				{
					return 4;
				}
			}
			return 0;
		}

		private Statement GetParentWhileStatement()
		{
			return this.GetParentWhileStatementAtLevel(0);
		}

		private Statement GetParentWhileStatementAtLevel(int level)
		{
			V_0 = this.statements.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 as WhileStatement == null && V_1 as DoWhileStatement == null)
					{
						continue;
					}
					if (level <= 0)
					{
						V_2 = V_1;
						goto Label1;
					}
					else
					{
						level = level - 1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return null;
		}

		private int GetParentWhileStatementIndex(List<RemoveConditionOnlyVariables.StatementExpression> statementExpressions, Statement parentWhileStatement)
		{
			V_0 = 0;
			while (V_0 < statementExpressions.get_Count())
			{
				if (statementExpressions.get_Item(V_0).get_Statement() == parentWhileStatement)
				{
					return V_0;
				}
				V_0 = V_0 + 1;
			}
			return 0;
		}

		private RemoveConditionOnlyVariables.VariableStateAndExpression GetValue(VariableReference variableReference)
		{
			V_0 = new RemoveConditionOnlyVariables.u003cu003ec__DisplayClass12_0();
			V_0.variableReference = variableReference;
			return this.variables.Where<RemoveConditionOnlyVariables.VariableStateAndExpression>(new Func<RemoveConditionOnlyVariables.VariableStateAndExpression, bool>(V_0.u003cGetValueu003eb__0)).FirstOrDefault<RemoveConditionOnlyVariables.VariableStateAndExpression>();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.processStep = 0;
			this.Visit(body);
			this.RemoveNonConditionVariables();
			if (this.variables.get_Count() > 0)
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
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count())
			{
				V_1 = node.get_Statements().get_Item(V_0);
				this.TryRemoveConditionVariable(node, V_1, V_0);
				this.TryRemoveReturnStatement(node, V_1, V_0);
				V_0 = V_0 + 1;
			}
			return;
		}

		private void RemoveNonConditionVariables()
		{
			V_0 = new HashSet<RemoveConditionOnlyVariables.VariableStateAndExpression>();
			V_1 = this.variables.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_VariableState() != 2 && V_2.get_VariableState() != 4)
					{
						dummyVar0 = V_0.Add(V_2);
					}
					if (V_2.get_VariableState() != 2)
					{
						continue;
					}
					this.RemoveNotSetStatementExpressions(V_2, V_0);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			V_3 = V_0.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					dummyVar1 = this.variables.Remove(V_4);
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			return;
		}

		private void RemoveNotSetStatementExpressions(RemoveConditionOnlyVariables.VariableStateAndExpression keyValuePair, HashSet<RemoveConditionOnlyVariables.VariableStateAndExpression> variablesToRemove)
		{
			stackVariable2 = new Stack<RemoveConditionOnlyVariables.StatementExpression>(keyValuePair.get_StatementExpressions());
			keyValuePair.get_StatementExpressions().Clear();
			V_0 = stackVariable2.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_Statement() == null)
					{
						continue;
					}
					if (V_1.get_ParentWhileStatement() == null)
					{
						keyValuePair.get_StatementExpressions().Add(V_1);
					}
					else
					{
						V_2 = this.GetParentWhileStatementIndex(keyValuePair.get_StatementExpressions(), V_1.get_ParentWhileStatement());
						keyValuePair.get_StatementExpressions().Insert(V_2, V_1);
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			if (keyValuePair.get_StatementExpressions().get_Count() == 0)
			{
				dummyVar0 = variablesToRemove.Add(keyValuePair);
			}
			return;
		}

		private void ReplaceConditionOnlyVariables(BlockStatement body)
		{
			this.processStep = 1;
			this.Visit(body);
			this.processStep = 0;
			this.variables.Clear();
			return;
		}

		private void SetVariableAssignmentExpression(Expression expression)
		{
			V_0 = this.variables.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_VariableState() & 1 != 1)
					{
						continue;
					}
					V_1.get_StatementExpressions().Add(new RemoveConditionOnlyVariables.StatementExpression(expression, this.GetParentWhileStatement()));
					if (V_1.get_VariableState() & 2 != 2)
					{
						V_1.set_VariableState(0);
					}
					else
					{
						V_1.set_VariableState(2);
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void SetVariablesExpressionStatements(ConditionStatement node)
		{
			V_0 = this.variables.get_Count() - 1;
			while (V_0 >= 0)
			{
				V_1 = this.variables.get_Item(V_0);
				if (V_1.get_VariableState() != 8)
				{
					V_2 = V_1.get_StatementExpressions().get_Count() - 1;
					while (V_2 >= 0)
					{
						V_3 = V_1.get_StatementExpressions().get_Item(V_2);
						if (V_3.get_Statement() == null)
						{
							if (node == V_3.get_ParentWhileStatement())
							{
								V_3.set_ParentWhileStatement(this.GetParentWhileStatementAtLevel(1));
							}
							V_3.set_Statement(node);
						}
						V_2 = V_2 - 1;
					}
				}
				V_0 = V_0 - 1;
			}
			return;
		}

		private bool TryGetValue(VariableReference variableReference, out RemoveConditionOnlyVariables.VariableStateAndExpression variableStateAndExpression)
		{
			V_0 = new RemoveConditionOnlyVariables.u003cu003ec__DisplayClass11_0();
			V_0.variableReference = variableReference;
			V_1 = this.variables.Where<RemoveConditionOnlyVariables.VariableStateAndExpression>(new Func<RemoveConditionOnlyVariables.VariableStateAndExpression, bool>(V_0.u003cTryGetValueu003eb__0)).FirstOrDefault<RemoveConditionOnlyVariables.VariableStateAndExpression>();
			variableStateAndExpression = V_1;
			return V_1 != null;
		}

		private bool TryGetVariableExpression(Expression variableExpression, out Expression expression)
		{
			if (variableExpression as VariableReferenceExpression == null || !this.TryGetValue(((VariableReferenceExpression)variableExpression).get_Variable(), out V_1) || V_1.get_VariableState() != 2)
			{
				expression = null;
				return false;
			}
			expression = this.GetCurrentStatementExpression(V_1);
			return expression != null;
		}

		private void TryProcessConditionExpression(ConditionExpression node)
		{
			if (this.processStep == 1 && this.TryGetVariableExpression(node.get_Condition(), out V_0))
			{
				node.set_Condition(V_0);
			}
			return;
		}

		private void TryProcessConditionStatement(ConditionStatement node)
		{
			if (this.processStep == 1 && this.TryGetVariableExpression(node.get_Condition(), out V_0))
			{
				node.set_Condition(V_0);
			}
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search && node as WhileStatement == null && node as DoWhileStatement == null)
			{
				this.SetVariablesExpressionStatements(node);
			}
			return;
		}

		private void TryProcessUnaryExpression(UnaryExpression node)
		{
			if (this.processStep == 1 && this.TryGetVariableExpression(node.get_Operand(), out V_0))
			{
				node.set_Operand(V_0);
			}
			return;
		}

		private void TryRemoveConditionVariable(BlockStatement node, Statement statement, int index)
		{
			if (statement as ExpressionStatement == null)
			{
				return;
			}
			V_0 = (ExpressionStatement)statement;
			if (V_0.get_Expression().get_CodeNodeType() != 24 || !(V_0.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
			{
				return;
			}
			V_1 = (BinaryExpression)V_0.get_Expression();
			if (V_1.get_Left() as VariableReferenceExpression == null)
			{
				return;
			}
			if (V_1.get_Right() as MethodInvocationExpression != null && String.op_Inequality((V_1.get_Left() as VariableReferenceExpression).get_Variable().get_VariableType().get_Name(), 3.ToString()))
			{
				return;
			}
			V_2 = (VariableReferenceExpression)V_1.get_Left();
			if (this.ContainsKey(V_2.get_Variable()))
			{
				this.methodContext.RemoveVariable(V_2.get_Variable());
				node.get_Statements().RemoveAt(index);
			}
			return;
		}

		private void TryRemoveReturnStatement(BlockStatement node, Statement statement, int index)
		{
			if (statement.get_CodeNodeType() == 5 && (statement as ExpressionStatement).get_Expression().get_CodeNodeType() == 57)
			{
				V_0 = (ReturnExpression)(statement as ExpressionStatement).get_Expression();
				if (V_0.get_Value() as VariableReferenceExpression == null)
				{
					return;
				}
				V_1 = V_0.get_Value() as VariableReferenceExpression;
				if (V_1 == null)
				{
					return;
				}
				if (this.TryGetValue(V_1.get_Variable(), out V_2) && V_2.get_VariableState() == 4)
				{
					this.methodContext.RemoveVariable(V_1.get_Variable());
					node.get_Statements().RemoveAt(index);
				}
			}
			return;
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.states.Push(0);
			this.VisitArrayIndexerExpression(node);
			dummyVar0 = this.states.Pop();
			return;
		}

		private void VisitAssignExpression(BinaryExpression node)
		{
			this.currentVariable = null;
			this.states.Push(1);
			this.Visit(node.get_Left());
			dummyVar0 = this.states.Pop();
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				this.SetVariableAssignmentExpression(node.get_Right());
			}
			if (node.get_Right().get_CodeNodeType() == 22 && this.currentVariable != null)
			{
				V_0 = this.GetValue(this.currentVariable);
				if (V_0 != null && V_0.get_VariableState() != RemoveConditionOnlyVariables.VariableState.Declaration)
				{
					stackVariable29 = V_0;
					stackVariable29.set_NumberOfTimesAssigned(stackVariable29.get_NumberOfTimesAssigned() + 1);
				}
			}
			this.Visit(node.get_Right());
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression())
			{
				this.VisitAssignExpression(node);
				return;
			}
			this.states.Push(0);
			this.VisitBinaryExpression(node);
			dummyVar0 = this.states.Pop();
			return;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			this.ProcessBlock(node);
			V_0 = node.get_Statements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.statements.Push(V_1);
					this.Visit(V_1);
					dummyVar0 = this.statements.Pop();
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public override void VisitConditionExpression(ConditionExpression node)
		{
			this.TryProcessConditionExpression(node);
			this.states.Push(2);
			this.Visit(node.get_Condition());
			dummyVar0 = this.states.Pop();
			this.Visit(node.get_Then());
			this.Visit(node.get_Else());
			return;
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(2);
			this.Visit(node.get_Condition());
			dummyVar0 = this.states.Pop();
			this.Visit(node.get_Body());
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				this.SetVariablesExpressionStatements(node);
			}
			return;
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.Visit(node.get_Initializer());
			this.states.Push(2);
			this.Visit(node.get_Condition());
			dummyVar0 = this.states.Pop();
			this.Visit(node.get_Increment());
			this.Visit(node.get_Body());
			return;
		}

		public override void VisitIfStatement(IfStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(2);
			this.Visit(node.get_Condition());
			dummyVar0 = this.states.Pop();
			this.Visit(node.get_Then());
			this.Visit(node.get_Else());
			return;
		}

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			this.states.Push(0);
			this.VisitMethodReferenceExpression(node);
			dummyVar0 = this.states.Pop();
			return;
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.states.Push(3);
			this.VisitReturnExpression(node);
			dummyVar0 = this.states.Pop();
			return;
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(2);
			this.Visit(node.get_Condition());
			dummyVar0 = this.states.Pop();
			this.Visit(node.get_Cases());
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() == 9)
			{
				return;
			}
			this.TryProcessUnaryExpression(node);
			this.states.Push(2);
			this.VisitUnaryExpression(node);
			dummyVar0 = this.states.Pop();
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.processStep == 1)
			{
				return;
			}
			V_0 = this.states.Peek();
			if (!this.ContainsKey(node.get_Variable()))
			{
				V_1 = this.GetInitialVariableState(V_0);
				V_2 = new RemoveConditionOnlyVariables.VariableStateAndExpression(node.get_Variable(), V_1);
				if (V_0 == 1)
				{
					stackVariable27 = V_2;
					stackVariable27.set_NumberOfTimesAssigned(stackVariable27.get_NumberOfTimesAssigned() + 1);
				}
				this.variables.Add(V_2);
			}
			else
			{
				this.ChangeVariableState(V_0, node);
			}
			this.VisitVariableReferenceExpression(node);
			this.currentVariable = node.get_Variable();
			return;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.TryProcessConditionStatement(node);
			this.states.Push(2);
			this.Visit(node.get_Condition());
			dummyVar0 = this.states.Pop();
			this.Visit(node.get_Body());
			if (this.processStep == RemoveConditionOnlyVariables.ProcessStep.Search)
			{
				this.SetVariablesExpressionStatements(node);
			}
			return;
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
					return;
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
					return;
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
					return;
				}
			}

			public StatementExpression(Expression expression, Statement parentWhileStatement)
			{
				base();
				this.expression = expression;
				this.parentWhileStatement = parentWhileStatement;
				return;
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
					return;
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
					return;
				}
			}

			public VariableStateAndExpression(VariableReference variableReference, RemoveConditionOnlyVariables.VariableState variableState)
			{
				base();
				this.variableState = variableState;
				this.variableReference = variableReference;
				this.statementExpressions = new List<RemoveConditionOnlyVariables.StatementExpression>();
				return;
			}
		}
	}
}