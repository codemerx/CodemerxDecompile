#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using System.Collections.Generic;
using System.Linq;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	class RemoveConditionOnlyVariables : Ast.BaseCodeVisitor, IDecompilationStep
	{
        readonly List<VariableStateAndExpression> variables = new List<VariableStateAndExpression>();
        readonly Stack<Step> states = new Stack<Step>();
        readonly Stack<Statement> statements = new Stack<Statement>();
        ProcessStep processStep = ProcessStep.Search;

		private MethodSpecificContext methodContext;
        private VariableReference currentVariable;

		public RemoveConditionOnlyVariables()
		{
			states.Push(Step.Default);
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			ProcessBlock(node);

			foreach (var statement in node.Statements)
			{
				statements.Push(statement);
				Visit(statement);
				statements.Pop();
			}
		}

		void ProcessBlock(BlockStatement node)
		{
			if (processStep == ProcessStep.Search)
				return;

			for (int i = 0; i < node.Statements.Count; i++)
			{
				var statement = node.Statements[i];
				TryRemoveConditionVariable(node, statement, i);
				TryRemoveReturnStatement(node, statement, i);
			}
		}
       
		private void TryRemoveConditionVariable(BlockStatement node, Statement statement, int index)
		{
			if (!(statement is ExpressionStatement))
				return;

			var expressionStatement = (ExpressionStatement) statement;
			if (!(expressionStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression &&
                (expressionStatement.Expression as BinaryExpression).IsAssignmentExpression))
				return;

			var assingExpression = (BinaryExpression) expressionStatement.Expression;

			if (!(assingExpression.Left is VariableReferenceExpression))
				return;

            if (assingExpression.Right is MethodInvocationExpression)
            {
                var variable = assingExpression.Left as VariableReferenceExpression;

                if (variable.Variable.VariableType.Name != TypeCode.Boolean.ToString())
                    return;
            }

			var variableReferenceExpression = (VariableReferenceExpression) assingExpression.Left;
			if (ContainsKey(variableReferenceExpression.Variable))
			{
				methodContext.RemoveVariable(variableReferenceExpression.Variable);
				node.Statements.RemoveAt(index);
			}
		}

		private bool ContainsKey(VariableReference variableReference)
		{
			return variables.Any(v => v.VariableReference == variableReference);
		}

		private bool TryGetValue(VariableReference variableReference, out VariableStateAndExpression variableStateAndExpression)
		{
			var result = variables.Where(v => v.VariableReference == variableReference).FirstOrDefault();
			variableStateAndExpression = result;
			return (result != null);
		}

		private VariableStateAndExpression GetValue(VariableReference variableReference)
		{
			return variables.Where(v => v.VariableReference == variableReference).FirstOrDefault();
		}

        private void TryRemoveReturnStatement(BlockStatement node, Statement statement, int index)
        {
            if (statement.CodeNodeType == CodeNodeType.ExpressionStatement && (statement as ExpressionStatement).Expression.CodeNodeType ==CodeNodeType.ReturnExpression)
            {
                var returnExpression = (ReturnExpression)(statement as ExpressionStatement).Expression;

                if (!(returnExpression.Value is VariableReferenceExpression))
                    return;

                var variableReference = returnExpression.Value as VariableReferenceExpression;

                if (variableReference == null)
                {
                    //Other expressions can be returned as well
                    //for instance MethodInvocation and literal expressions
                    //as this is legacy code, the chesk is here to ensure that no exception is thrown due to null reference
                    //Investigate the logic and provide further fixes.
                    return;
                }

                VariableStateAndExpression variableStateAndExpression; 

                if (TryGetValue(variableReference.Variable, out variableStateAndExpression))
                {
                    if (variableStateAndExpression.VariableState == VariableState.Return)
                    {
                        methodContext.RemoveVariable(variableReference.Variable);
                        node.Statements.RemoveAt(index);
                    }
                }
            
            }
        }

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.processStep = ProcessStep.Search;

			Visit(body);
			RemoveNonConditionVariables();
			if (variables.Count > 0)
			{
				ReplaceConditionOnlyVariables(body);
			}
			this.methodContext = null;
			this.currentVariable = null;

			variables.Clear();

			return body;
		}

		private void ReplaceConditionOnlyVariables(BlockStatement body)
		{
			this.processStep = ProcessStep.Replace;
			Visit(body);
			this.processStep = ProcessStep.Search;
			variables.Clear();
		}

		private void RemoveNonConditionVariables()
		{
			var variablesToRemove = new HashSet<VariableStateAndExpression>();
			foreach (var keyValuePair in variables)
			{
				if ((keyValuePair.VariableState != VariableState.Condition) &&
					(keyValuePair.VariableState != VariableState.Return))
				{
					variablesToRemove.Add(keyValuePair);
				}
				if (keyValuePair.VariableState == VariableState.Condition)
				{
					RemoveNotSetStatementExpressions(keyValuePair, variablesToRemove);
				}
			}
			foreach (var variableToRemove in variablesToRemove)
			{
				variables.Remove(variableToRemove);
			}
		}

		private void RemoveNotSetStatementExpressions(VariableStateAndExpression keyValuePair, HashSet<VariableStateAndExpression> variablesToRemove)
		{
			var statementExpressions = new Stack<StatementExpression>(keyValuePair.StatementExpressions);
			keyValuePair.StatementExpressions.Clear();
			foreach (var statementExpression in statementExpressions)
			{
				if (statementExpression.Statement != null)
				{
					if (statementExpression.ParentWhileStatement != null)
					{
						int index = GetParentWhileStatementIndex(keyValuePair.StatementExpressions, statementExpression.ParentWhileStatement);
						keyValuePair.StatementExpressions.Insert(index, statementExpression);
					}
					else
					{
						keyValuePair.StatementExpressions.Add(statementExpression);
					}
				}
			}
			if (keyValuePair.StatementExpressions.Count == 0)
			{
				variablesToRemove.Add(keyValuePair);
			}
		}

		private int GetParentWhileStatementIndex(List<StatementExpression> statementExpressions, Statement parentWhileStatement)
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

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			states.Push(Step.Default);
			base.VisitMethodReferenceExpression(node);
			states.Pop();
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (processStep == ProcessStep.Replace)
				return;

			var state = states.Peek();
			if (ContainsKey(node.Variable))
			{
				ChangeVariableState(state, node);
			}
			else
			{
				VariableState variableState = GetInitialVariableState(state);
				var variableStateAndExpression = new VariableStateAndExpression(node.Variable, variableState);
                if (state == Step.Assign)
                {
                    variableStateAndExpression.NumberOfTimesAssigned++;
                }
				variables.Add(variableStateAndExpression);
			}
			base.VisitVariableReferenceExpression(node);

            this.currentVariable = node.Variable;
		}

		private void ChangeVariableState(Step state, VariableReferenceExpression node)
		{
			VariableStateAndExpression variableStateAndExpression = GetValue(node.Variable);
			
            if ((state == Step.Expression) 
                && (variableStateAndExpression.VariableState != VariableState.Other)
                && variableStateAndExpression.NumberOfTimesAssigned <= 1)
			{
				variableStateAndExpression.VariableState = VariableState.Condition;
            }
			else if (state == Step.Assign)
			{
                if (variableStateAndExpression.VariableState == VariableState.Condition)
                    variableStateAndExpression.VariableState = VariableState.Assign | VariableState.Condition;
                else
                {
                    variableStateAndExpression.VariableState = VariableState.Assign;
                    variableStateAndExpression.NumberOfTimesAssigned++;
                }
		 	}
			else
			{
				variableStateAndExpression.VariableState = VariableState.Other;
			}
		}

		private VariableState GetInitialVariableState(Step state)
		{
			switch (state)
			{
				case Step.Assign:
				{
					return VariableState.Assign;
				}
				case Step.Return:
				{
					return VariableState.Return;
				}
				case Step.Expression:
				{
					return VariableState.Condition;
				}
				default:
				{
					return VariableState.Declaration;
				}
			}
		}

		private bool TryGetVariableExpression(Expression variableExpression, out Expression expression)
		{
			if (variableExpression is VariableReferenceExpression)
			{
				var variableReference = (VariableReferenceExpression) variableExpression;
				VariableStateAndExpression variableStateAndExpression;
				if (TryGetValue(variableReference.Variable, out variableStateAndExpression))
				{
					if (variableStateAndExpression.VariableState == VariableState.Condition)
					{
						expression = GetCurrentStatementExpression(variableStateAndExpression);
						return (expression != null);
					}
				}
			}
			expression = null;
			return false;
		}

		private Expression GetCurrentStatementExpression(VariableStateAndExpression variableStateAndExpression)
		{
			if (variableStateAndExpression.StatementExpressions.Count == 0)
				return null;

			var lastStatementExpression = variableStateAndExpression.StatementExpressions[variableStateAndExpression.StatementExpressions.Count - 1];
			variableStateAndExpression.StatementExpressions.Remove(lastStatementExpression);
			return lastStatementExpression.Expression.CloneExpressionOnly();
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
            if (node.IsAssignmentExpression)
            {
                VisitAssignExpression(node);
                return;
            }
			states.Push(Step.Default);
			base.VisitBinaryExpression(node);
			states.Pop();
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
            //Added during refactoring and removal of AddressOfExpression class
            if (node.Operator == UnaryOperator.AddressOf)
            {
                return;
            }

			TryProcessUnaryExpression(node);

			states.Push(Step.Expression);
			base.VisitUnaryExpression(node);
			states.Pop();
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			states.Push(Step.Default);
			base.VisitArrayIndexerExpression(node);
			states.Pop();
		}

		private void TryProcessUnaryExpression(UnaryExpression node)
		{
			if (processStep == ProcessStep.Replace)
			{
				Expression expression;
				if (TryGetVariableExpression(node.Operand, out expression))
				{
					node.Operand = expression;
				}
			}
		}

		public override void VisitIfStatement(IfStatement node)
		{
            TryProcessConditionStatement(node);

            states.Push(Step.Expression);
            Visit(node.Condition);
            states.Pop();

            Visit(node.Then);
            Visit(node.Else);
		}

		private void TryProcessConditionStatement(ConditionStatement node)
		{
			if (processStep == ProcessStep.Replace)
			{
				Expression expression;
				if (TryGetVariableExpression(node.Condition, out expression))
				{
					node.Condition = expression;
				}
			}
			if (processStep == ProcessStep.Search)
			{
				if (!(node is WhileStatement) && !(node is DoWhileStatement))
				{
					SetVariablesExpressionStatements(node);
				}
			}
		}

		private void SetVariablesExpressionStatements(ConditionStatement node)
		{
			for (int i = variables.Count - 1; i >= 0; i--)
			{
				var keyValuePair = variables[i];

				if (keyValuePair.VariableState == VariableState.Other)
					continue;

				for (int j = keyValuePair.StatementExpressions.Count - 1; j >= 0; j--)
				{
					var statementExpression = keyValuePair.StatementExpressions[j];
					if (statementExpression.Statement == null)
					{
						if (node == statementExpression.ParentWhileStatement)
						{
							statementExpression.ParentWhileStatement = GetParentWhileStatementAtLevel(1);
						}
						statementExpression.Statement = node;
					}
				}
			}
		}

		private Statement GetParentWhileStatement()
		{
			return GetParentWhileStatementAtLevel(0);
		}

		private Statement GetParentWhileStatementAtLevel(int level)
		{
			foreach (var statement in statements)
			{
				if ((statement is WhileStatement) || (statement is DoWhileStatement))
				{
					if (level > 0)
					{
						level--;
						continue;
					}

					return statement;
				}
			}
			return null;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			TryProcessConditionStatement(node);

			states.Push(Step.Expression);
			Visit(node.Condition);
			states.Pop();

			Visit(node.Body);
			if (processStep == ProcessStep.Search)
			{
				SetVariablesExpressionStatements(node);
			}
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			TryProcessConditionStatement(node);

			states.Push(Step.Expression);
			Visit(node.Condition);
			states.Pop();

			Visit(node.Body);
			if (processStep == ProcessStep.Search)
			{
				SetVariablesExpressionStatements(node);
			}
		}

		public override void VisitForStatement(ForStatement node)
		{
			TryProcessConditionStatement(node);
			Visit(node.Initializer);

			states.Push(Step.Expression);
			Visit(node.Condition);
			states.Pop();

			Visit(node.Increment);
			Visit(node.Body);
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			TryProcessConditionStatement(node);

			states.Push(Step.Expression);
			Visit(node.Condition);
			states.Pop();

			Visit(node.Cases);
		}

        public override void VisitReturnExpression(ReturnExpression node)
        {
            states.Push(Step.Return);
            base.VisitReturnExpression(node);
            states.Pop();
        }

        public override void VisitConditionExpression(ConditionExpression node)
		{
			TryProcessConditionExpression(node);

			states.Push(Step.Expression);
			Visit(node.Condition);
			states.Pop();

			Visit(node.Then);
			Visit(node.Else);
		}

		private void TryProcessConditionExpression(ConditionExpression node)
		{
			if (processStep == ProcessStep.Replace)
			{
				Expression expression;
				if (TryGetVariableExpression(node.Condition, out expression))
				{
					node.Condition = expression;
				}
			}
		}

		private void VisitAssignExpression(BinaryExpression node)
		{
            this.currentVariable = null;

			states.Push(Step.Assign);
			Visit(node.Left);
			states.Pop();

			if (processStep == ProcessStep.Search)
			{
				SetVariableAssignmentExpression(node.Right);
			}

            // next statement must be literal 
            if (node.Right.CodeNodeType == CodeNodeType.LiteralExpression)
            {
                if (this.currentVariable != null)
                {
                    var variableState = GetValue(this.currentVariable);
                    if (variableState != null && variableState.VariableState != VariableState.Declaration)
                    {
                        variableState.NumberOfTimesAssigned++;
                    }
                }
            }

			Visit(node.Right);
		}

		private void SetVariableAssignmentExpression(Expression expression)
		{
			foreach (var keyValuePair in variables)
			{
				if ((keyValuePair.VariableState & VariableState.Assign) == VariableState.Assign)
				{
					keyValuePair.StatementExpressions.Add(new StatementExpression(expression, GetParentWhileStatement()));
					if ((keyValuePair.VariableState & VariableState.Condition) == VariableState.Condition)
					{
						keyValuePair.VariableState = VariableState.Condition;
					}
					else
					{
						keyValuePair.VariableState = VariableState.Declaration;
					}
				}
			}
		}

		enum Step
		{
			Default,
			Assign,
			Expression,
			Return
		}

		[Flags]
		enum VariableState
		{
			Declaration = 0,
			Assign = 1,
			Condition = 2,
			Return = 4,
			Other = 8
		}

		enum ProcessStep
		{
			Search,
			Replace
		}

		class VariableStateAndExpression
		{
			VariableState variableState;
            readonly List<StatementExpression> statementExpressions;
            VariableReference variableReference;

			public VariableStateAndExpression(VariableReference variableReference, VariableState variableState)
			{
				this.variableState = variableState;
				this.variableReference = variableReference;
				statementExpressions = new List<StatementExpression>();
			}

            public int NumberOfTimesAssigned
            {
                get;
                set;
            }

			public VariableReference VariableReference
			{
				get
				{
					return variableReference;
				}
				set
				{
					variableReference = value;
				}
			}

			public VariableState VariableState
			{
				get
				{
					return variableState;
				}
				set
				{
					variableState = value;
				}
			}

			public List<StatementExpression> StatementExpressions
			{
				get
				{
					return statementExpressions;
				}
			}
		}

		class StatementExpression
		{
			Expression expression;
			Statement statement;
			Statement parentWhileStatement;

			public StatementExpression(Expression expression, Statement parentWhileStatement)
			{
				this.expression = expression;
				this.parentWhileStatement = parentWhileStatement;
			}

			public Expression Expression
			{
				get
				{
					return expression;
				}
				set
				{
					expression = value;
				}
			}

			public Statement Statement
			{
				get
				{
					return statement;
				}
				set
				{
					statement = value;
				}
			}

			public Statement ParentWhileStatement
			{
				get
				{
					return parentWhileStatement;
				}
				set
				{
					parentWhileStatement = value;
				}
			}
		}
	}
}