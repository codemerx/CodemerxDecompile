using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildUsingStatements : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public RebuildUsingStatements()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				RebuildUsingStatements.UsingMatcher usingMatcher = new RebuildUsingStatements.UsingMatcher(node.Statements[i], node.Statements[i + 1]);
				if (usingMatcher.Match())
				{
					if (usingMatcher.VariableReference != null)
					{
						this.context.MethodContext.RemoveVariable(usingMatcher.VariableReference);
					}
					if (!usingMatcher.RemoveExpression)
					{
						int num = i + (usingMatcher.HasExpression ? 1 : 0);
						node.Statements.RemoveAt(num);
						node.AddStatementAt(num, usingMatcher.Using);
					}
					else
					{
						node.Statements.RemoveAt(i);
						node.Statements.RemoveAt(i);
						node.AddStatementAt(i, usingMatcher.Using);
					}
					this.ProcessBlock(usingMatcher.Using.Body);
				}
			}
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			this.ProcessBlock(node);
			foreach (Statement statement in node.Statements)
			{
				this.Visit(statement);
			}
		}

		private class UsingMatcher
		{
			private readonly Statement statement;

			private readonly Statement nextStatement;

			private UsingStatement @using;

			private readonly BlockStatement blockStatement;

			private Expression expression;

			private VariableReference variableReference;

			private bool hasExpression;

			private bool removeExpression;

			private TryStatement theTry;

			public bool HasExpression
			{
				get
				{
					return this.hasExpression;
				}
			}

			public bool RemoveExpression
			{
				get
				{
					return this.removeExpression;
				}
			}

			public UsingStatement Using
			{
				get
				{
					this.@using = this.@using ?? new UsingStatement(this.expression.CloneExpressionOnly(), this.blockStatement, this.theTry.Finally.UnderlyingSameMethodInstructions);
					return this.@using;
				}
			}

			public VariableReference VariableReference
			{
				get
				{
					return this.variableReference;
				}
			}

			public UsingMatcher(Statement statement, Statement nextStatement)
			{
				this.statement = statement;
				this.nextStatement = nextStatement;
			}

			private bool CheckStandardUsingBlock(IfStatement ifStatement, TryStatement @try, ExpressionStatement expressionStatement)
			{
				if (ifStatement.Condition.CodeNodeType != CodeNodeType.BinaryExpression)
				{
					return false;
				}
				BinaryExpression condition = (BinaryExpression)ifStatement.Condition;
				MethodReferenceExpression disposeMethodReference = this.GetDisposeMethodReference(ifStatement, condition);
				if (disposeMethodReference == null)
				{
					return false;
				}
				Expression target = disposeMethodReference.Target;
				if (target != null && target.CodeNodeType == CodeNodeType.ExplicitCastExpression && (target as ExplicitCastExpression).TargetType.get_FullName() == "System.IDisposable")
				{
					target = (target as ExplicitCastExpression).Expression;
				}
				if (!condition.Left.CheckInnerReferenceExpressions(target))
				{
					return false;
				}
				this.PrepareUsingBlock(condition, @try, expressionStatement);
				return true;
			}

			private void FixExpression()
			{
				if (this.variableReference == null)
				{
					return;
				}
				VariableFinder variableFinder = new VariableFinder(this.variableReference);
				BinaryExpression binaryExpression = this.expression as BinaryExpression;
				if (binaryExpression.Right.IsReferenceExpression() && !variableFinder.FindVariable(this.theTry.Try))
				{
					List<Instruction> instructions = new List<Instruction>(binaryExpression.Left.UnderlyingSameMethodInstructions);
					instructions.AddRange(binaryExpression.MappedInstructions);
					this.expression = binaryExpression.Right.CloneAndAttachInstructions(instructions);
				}
			}

			private MethodReferenceExpression GetDisposeMethodReference(IfStatement ifStatement, BinaryExpression binaryExpression)
			{
				if (binaryExpression.Operator != BinaryOperator.ValueInequality)
				{
					return null;
				}
				if (!binaryExpression.Left.IsReferenceExpression() || !RebuildUsingStatements.UsingMatcher.IsNullLiteralExpression(binaryExpression.Right))
				{
					return null;
				}
				return this.GetDisposeMethodReference(ifStatement);
			}

			private MethodReferenceExpression GetDisposeMethodReference(IfStatement ifStatement)
			{
				if (ifStatement.Else != null)
				{
					return null;
				}
				if (ifStatement.Then.Statements.Count != 1)
				{
					return null;
				}
				Statement item = ifStatement.Then.Statements[0];
				if (!(item is ExpressionStatement))
				{
					return null;
				}
				return this.GetDisposeMethodReference(item);
			}

			private MethodReferenceExpression GetDisposeMethodReference(Statement stmt)
			{
				ExpressionStatement expressionStatement = (ExpressionStatement)stmt;
				if (!(expressionStatement.Expression is MethodInvocationExpression))
				{
					return null;
				}
				MethodInvocationExpression expression = (MethodInvocationExpression)expressionStatement.Expression;
				if (expression.Arguments.Count != 0)
				{
					return null;
				}
				if (expression.MethodExpression == null)
				{
					return null;
				}
				MethodReferenceExpression methodExpression = expression.MethodExpression;
				TypeReference declaringType = expression.MethodExpression.Method.get_DeclaringType();
				if (declaringType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = Utilities.GetCorlibTypeReference(typeof(IDisposable), declaringType.get_Module()).Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				if (methodExpression.Method.IsImplementationOf(typeDefinition))
				{
					return methodExpression;
				}
				return null;
			}

			private static bool IsNullLiteralExpression(Expression expression)
			{
				if (!(expression is LiteralExpression))
				{
					return false;
				}
				return (object)((LiteralExpression)expression).Value == (object)null;
			}

			private bool IsTryFinallyStatement(TryStatement @try)
			{
				if (@try.CatchClauses.Count != 0)
				{
					return false;
				}
				return @try.Finally != null;
			}

			internal bool Match()
			{
				ExpressionStatement expressionStatement = null;
				this.theTry = null;
				if (this.statement.CodeNodeType == CodeNodeType.TryStatement)
				{
					this.theTry = this.statement as TryStatement;
				}
				else if (this.nextStatement.CodeNodeType == CodeNodeType.TryStatement)
				{
					this.theTry = this.nextStatement as TryStatement;
					expressionStatement = this.statement as ExpressionStatement;
					this.hasExpression = true;
				}
				if (this.theTry == null)
				{
					return false;
				}
				if (!this.IsTryFinallyStatement(this.theTry))
				{
					return false;
				}
				if (this.theTry.Finally.Body.Statements.Count != 1)
				{
					return false;
				}
				Statement item = this.theTry.Finally.Body.Statements[0];
				if (item.CodeNodeType != CodeNodeType.IfStatement)
				{
					return false;
				}
				if (!this.CheckStandardUsingBlock((IfStatement)item, this.theTry, expressionStatement))
				{
					return false;
				}
				this.FixExpression();
				return true;
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
					this.VisitAssignExpression(expressionStatement);
				}
			}

			private void VisitAssignExpression(ExpressionStatement expressionStatement)
			{
				if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
				{
					return;
				}
				BinaryExpression expression = (BinaryExpression)expressionStatement.Expression;
				this.expression = expression;
				this.VisitVariableReference(expression);
			}

			private void VisitVariableReference(BinaryExpression assingExpression)
			{
				if (assingExpression.Left is VariableReferenceExpression)
				{
					this.variableReference = ((VariableReferenceExpression)assingExpression.Left).Variable;
					this.removeExpression = true;
				}
			}
		}
	}
}