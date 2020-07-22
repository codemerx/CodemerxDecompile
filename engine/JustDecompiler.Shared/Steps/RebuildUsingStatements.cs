using Mono.Cecil;
using Telerik.JustDecompiler.Ast;

using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildUsingStatements : BaseCodeVisitor, IDecompilationStep
	{
		DecompilationContext context;

		public override void VisitBlockStatement (BlockStatement node)
		{
			ProcessBlock (node);

			foreach (var statement in node.Statements)
				Visit (statement);
		}

		void ProcessBlock (BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				var matcher = new UsingMatcher(node.Statements[i], node.Statements[i + 1]);
				if (!matcher.Match ())
					continue;

                if (matcher.VariableReference != null)
                {
                    context.MethodContext.RemoveVariable(matcher.VariableReference);
                }
				if (matcher.RemoveExpression)
				{
					node.Statements.RemoveAt(i); // declaration
					node.Statements.RemoveAt(i); // try
					node.AddStatementAt(i, matcher.Using);
				}
				else
				{
					int index = i + (matcher.HasExpression ? 1 : 0);
					node.Statements.RemoveAt(index); // try
					node.AddStatementAt(index, matcher.Using);
				}
				ProcessBlock(matcher.Using.Body);
			}
		}

		public BlockStatement Process (DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			Visit (body);
			return body;
		}

		class UsingMatcher
		{
            readonly Statement statement;
            readonly Statement nextStatement;
            UsingStatement @using;
            readonly BlockStatement blockStatement = new BlockStatement();
            Telerik.JustDecompiler.Ast.Expressions.Expression expression;
            VariableReference variableReference;
			bool hasExpression;
            bool removeExpression;
            private TryStatement theTry;

			public UsingMatcher(Statement statement, Statement nextStatement)
			{
				this.statement = statement;
				this.nextStatement = nextStatement;
			}

			public VariableReference VariableReference
			{
				get { return variableReference; }
			}

			public bool RemoveExpression
			{
                get { return removeExpression; }
			}

			public bool HasExpression
			{
				get { return hasExpression; }
			}

			public UsingStatement Using
			{
				get
				{
					@using = @using ?? new UsingStatement(expression.CloneExpressionOnly(), blockStatement, theTry.Finally.UnderlyingSameMethodInstructions);
					return @using;
				}
			}

            internal bool Match()
            {
                ExpressionStatement expressionStatement = null;
                theTry = null;

                if (statement.CodeNodeType == CodeNodeType.TryStatement)
                {
                    theTry = statement as TryStatement;
                }
                else if (nextStatement.CodeNodeType == CodeNodeType.TryStatement)
                {
                    theTry = nextStatement as TryStatement;
                    expressionStatement = statement as ExpressionStatement;
                    hasExpression = true;
                }

                if (theTry == null)
                    return false;

                if (!IsTryFinallyStatement(theTry))
                    return false;

                if (theTry.Finally.Body.Statements.Count != 1)
                    return false;

                var firstFinallyStatement = theTry.Finally.Body.Statements[0];

                if (firstFinallyStatement.CodeNodeType != CodeNodeType.IfStatement)
                    return false;

                var ifStatement = (IfStatement)firstFinallyStatement;

                if (!CheckStandardUsingBlock(ifStatement, theTry, expressionStatement))
                {
                    return false;
                }

                FixExpression();
                return true;
            }

            private void FixExpression()
            {
                if (variableReference == null)
                {
                    return;
                }

                VariableFinder finder = new VariableFinder(this.variableReference);
                BinaryExpression assignExpression = this.expression as BinaryExpression;
                if (assignExpression.Right.IsReferenceExpression() && !finder.FindVariable(theTry.Try))
                {
                    List<Instruction> instructions = new List<Instruction>(assignExpression.Left.UnderlyingSameMethodInstructions);
                    instructions.AddRange(assignExpression.MappedInstructions);
                    this.expression = assignExpression.Right.CloneAndAttachInstructions(instructions);
                }
            }

			private bool CheckStandardUsingBlock(IfStatement ifStatement, TryStatement @try, ExpressionStatement expressionStatement)
			{
				if (ifStatement.Condition.CodeNodeType != CodeNodeType.BinaryExpression)
					return false;

				var binaryExpression = (BinaryExpression) ifStatement.Condition;

				var methodReference = GetDisposeMethodReference(ifStatement, binaryExpression);

				if (methodReference == null)
				{
					return false;
				}

                Expression methodTarget = methodReference.Target;
                if (methodTarget != null && methodTarget.CodeNodeType == CodeNodeType.ExplicitCastExpression &&
                    (methodTarget as ExplicitCastExpression).TargetType.FullName == "System.IDisposable")
                {
                    methodTarget = (methodTarget as ExplicitCastExpression).Expression;
                }

				if (binaryExpression.Left.CheckInnerReferenceExpressions(methodTarget))
				{
					PrepareUsingBlock(binaryExpression, @try, expressionStatement);
					return true;
				}
				return false;
			}

			private void PrepareUsingBlock(BinaryExpression binaryExpression, TryStatement @try, ExpressionStatement expressionStatement)
			{
                this.expression = binaryExpression.Left;
				for (int i = 0; i < @try.Try.Statements.Count; i++)
				{
					this.blockStatement.AddStatement(@try.Try.Statements[i]);
				}
				if (expressionStatement != null)
				{
					VisitAssignExpression(expressionStatement);
				}
			}

			private void VisitAssignExpression(ExpressionStatement expressionStatement)
			{
				if (!(expressionStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression &&
                      (expressionStatement.Expression as BinaryExpression).IsAssignmentExpression))
					return;

				BinaryExpression assingExpression = (BinaryExpression) expressionStatement.Expression;
                
				this.expression = assingExpression;
				VisitVariableReference(assingExpression);
			}

			private void VisitVariableReference(BinaryExpression assingExpression)
			{
                if (assingExpression.Left is VariableReferenceExpression)
                {
                    var variableExpression = (VariableReferenceExpression) assingExpression.Left;
					this.variableReference = variableExpression.Variable;
                    this.removeExpression = true;
				}
			}

			private bool IsTryFinallyStatement(TryStatement @try)
			{
				return (@try.CatchClauses.Count == 0) && (@try.Finally != null);
			}

            private MethodReferenceExpression GetDisposeMethodReference(IfStatement ifStatement, BinaryExpression binaryExpression)
            {
				if (binaryExpression.Operator != BinaryOperator.ValueInequality)
					return null;

				if ((!binaryExpression.Left.IsReferenceExpression()) ||
					(!IsNullLiteralExpression(binaryExpression.Right)))
					return null;

				return GetDisposeMethodReference(ifStatement);
			}

            private MethodReferenceExpression GetDisposeMethodReference(IfStatement ifStatement)
            {
				if (ifStatement.Else != null)
					return null;

				if (ifStatement.Then.Statements.Count != 1)
					return null;

				var thenStatement = ifStatement.Then.Statements[0];
				if (!(thenStatement is ExpressionStatement))
					return null;

				return GetDisposeMethodReference(thenStatement);
			}

            private MethodReferenceExpression GetDisposeMethodReference(Statement stmt)
            {
                ExpressionStatement expressionStatement = (ExpressionStatement)stmt;
                if (!(expressionStatement.Expression is MethodInvocationExpression))
                {
                    return null;
                }

                MethodInvocationExpression methodInvocation = (MethodInvocationExpression)expressionStatement.Expression;
                if (methodInvocation.Arguments.Count != 0)
                {
                    return null;
                }

                if (!(methodInvocation.MethodExpression is MethodReferenceExpression))
                {
                    return null;
                }

                MethodReferenceExpression methodReference = methodInvocation.MethodExpression;
                TypeReference declaringType = methodInvocation.MethodExpression.Method.DeclaringType;
                if(declaringType == null)
                {
                    return null;
                }

                TypeDefinition iDisposable = Utilities.GetCorlibTypeReference(typeof(System.IDisposable), declaringType.Module).Resolve();
                if (iDisposable == null)
                {
                    return null;
                }

                if (methodReference.Method.IsImplementationOf(iDisposable))
                {
                    return methodReference;
                }

                return null;
            }

            private static bool IsNullLiteralExpression(Expression expression)
            {
				return ((expression is LiteralExpression) &&
						(((LiteralExpression) expression).Value == null));
			}
		}
	}
}