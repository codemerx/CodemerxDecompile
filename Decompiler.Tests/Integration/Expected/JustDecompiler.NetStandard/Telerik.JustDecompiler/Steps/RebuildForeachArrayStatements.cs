using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForeachArrayStatements : BaseCodeVisitor, IDecompilationStep
	{
		private readonly Stack<VariableDefinition> currentForIndeces = new Stack<VariableDefinition>();

		private readonly Stack<bool> currentForIndecesUsed = new Stack<bool>();

		private DecompilationContext context;

		public RebuildForeachArrayStatements()
		{
		}

		private bool CheckForIndexUsages(RebuildForeachArrayStatements.ForeachArrayMatcher matcher)
		{
			this.currentForIndeces.Push(matcher.Incrementor);
			this.currentForIndecesUsed.Push(false);
			foreach (Statement statement in matcher.Foreach.Body.Statements)
			{
				this.Visit(statement);
			}
			this.currentForIndeces.Pop();
			return this.currentForIndecesUsed.Pop();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			this.context = null;
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				RebuildForeachArrayStatements.ForeachArrayMatcher foreachArrayMatcher = new RebuildForeachArrayStatements.ForeachArrayMatcher(node.Statements[i], node.Statements[i + 1], this.context.MethodContext);
				if (foreachArrayMatcher.Match() && !this.CheckForIndexUsages(foreachArrayMatcher))
				{
					this.context.MethodContext.RemoveVariable(foreachArrayMatcher.Incrementor);
					if (foreachArrayMatcher.CurrentVariable != null)
					{
						this.context.MethodContext.RemoveVariable(foreachArrayMatcher.CurrentVariable);
					}
					node.Statements.RemoveAt(i);
					node.Statements.RemoveAt(i);
					node.AddStatementAt(i, foreachArrayMatcher.Foreach);
					this.ProcessBlock(foreachArrayMatcher.Foreach.Body);
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

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.currentForIndeces.Count > 0 && this.currentForIndeces.Peek() == node.Variable && !this.currentForIndecesUsed.Peek())
			{
				this.currentForIndecesUsed.Pop();
				this.currentForIndecesUsed.Push(true);
			}
			base.VisitVariableReferenceExpression(node);
		}

		private class ForeachArrayMatcher
		{
			private readonly Statement statement;

			private readonly Statement nextStatement;

			private readonly MethodSpecificContext methodContext;

			private ForEachStatement @foreach;

			private Expression source;

			private BlockStatement statementBody;

			private IEnumerable<Instruction> foreachConditionInstructions;

			public VariableReference CurrentVariable
			{
				get;
				private set;
			}

			public ForEachStatement Foreach
			{
				get
				{
					this.@foreach = this.@foreach ?? new ForEachStatement(new VariableDeclarationExpression(this.CurrentVariable.Resolve(), null), this.source, this.statementBody, this.foreachConditionInstructions, null);
					return this.@foreach;
				}
			}

			public VariableDefinition Incrementor
			{
				get;
				private set;
			}

			public ForeachArrayMatcher(Statement statement, Statement nextStatement, MethodSpecificContext methodContext)
			{
				this.statement = statement;
				this.nextStatement = nextStatement;
				this.methodContext = methodContext;
			}

			private bool CheckArrayIndexer(Expression expression, VariableReference variableReference, Expression arrayExpression)
			{
				if (!(expression is ArrayIndexerExpression))
				{
					return false;
				}
				ArrayIndexerExpression arrayIndexerExpression = (ArrayIndexerExpression)expression;
				if (arrayIndexerExpression.Indices.Count != 1)
				{
					return false;
				}
				if (!(arrayIndexerExpression.Indices[0] is VariableReferenceExpression))
				{
					return false;
				}
				if ((object)((VariableReferenceExpression)arrayIndexerExpression.Indices[0]).Variable != (object)variableReference)
				{
					return false;
				}
				if (!arrayIndexerExpression.Target.CheckInnerReferenceExpressions(arrayExpression))
				{
					return false;
				}
				return true;
			}

			private VariableReference CheckAssingExpression(Statement statement, VariableReference variableReference, Expression arrayExpression)
			{
				if (!(statement is ExpressionStatement))
				{
					return null;
				}
				ExpressionStatement expressionStatement = (ExpressionStatement)statement;
				if (this.CheckArrayIndexer(expressionStatement.Expression, variableReference, arrayExpression))
				{
					TypeReference arrayElementType = this.GetArrayElementType(arrayExpression);
					if (arrayElementType != null)
					{
						VariableDefinition variableDefinition = new VariableDefinition(arrayElementType, this.methodContext.Method);
						this.methodContext.VariablesToRename.Add(variableDefinition);
						return variableDefinition;
					}
				}
				if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
				{
					return null;
				}
				BinaryExpression expression = (BinaryExpression)expressionStatement.Expression;
				if (!(expression.Left is VariableReferenceExpression))
				{
					return null;
				}
				if (!this.CheckArrayIndexer(expression.Right, variableReference, arrayExpression))
				{
					return null;
				}
				return (expression.Left as VariableReferenceExpression).Variable;
			}

			private bool CheckLiteralExpressionValue(Expression expression, int value)
			{
				if (!(expression is LiteralExpression))
				{
					return false;
				}
				LiteralExpression literalExpression = (LiteralExpression)expression;
				if (literalExpression.Value is Int32 && (Int32)literalExpression.Value == value)
				{
					return true;
				}
				return false;
			}

			private void CopyWhileBodyStatements(WhileStatement whileStatement)
			{
				this.statementBody = new BlockStatement();
				for (int i = 1; i < whileStatement.Body.Statements.Count - 1; i++)
				{
					this.statementBody.AddStatement(whileStatement.Body.Statements[i]);
				}
			}

			private TypeReference GetArrayElementType(Expression arrayExpression)
			{
				TypeReference targetTypeReference = arrayExpression.GetTargetTypeReference();
				if (!targetTypeReference.get_IsArray())
				{
					return null;
				}
				return targetTypeReference.GetElementType();
			}

			private PropertyReferenceExpression GetPropertyReferenceFromCast(Expression expression)
			{
				if (!(expression is ExplicitCastExpression))
				{
					return null;
				}
				ExplicitCastExpression explicitCastExpression = (ExplicitCastExpression)expression;
				if (explicitCastExpression.TargetType.get_FullName() != "System.Int32")
				{
					return null;
				}
				if (!(explicitCastExpression.Expression is PropertyReferenceExpression))
				{
					return null;
				}
				PropertyReferenceExpression propertyReferenceExpression = (PropertyReferenceExpression)explicitCastExpression.Expression;
				if (propertyReferenceExpression.Property.get_FullName() != "Int32.System Length()")
				{
					return null;
				}
				return propertyReferenceExpression;
			}

			private VariableReference GetVariableReference()
			{
				ExpressionStatement expressionStatement = (ExpressionStatement)this.statement;
				if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
				{
					return null;
				}
				BinaryExpression expression = (BinaryExpression)expressionStatement.Expression;
				if (!(expression.Left is VariableReferenceExpression))
				{
					return null;
				}
				if (!this.CheckLiteralExpressionValue(expression.Right, 0))
				{
					return null;
				}
				return ((VariableReferenceExpression)expression.Left).Variable;
			}

			private static bool IsArrayExpression(Expression expression)
			{
				TypeReference targetTypeReference = expression.GetTargetTypeReference();
				if (targetTypeReference == null)
				{
					return false;
				}
				return targetTypeReference.get_IsArray();
			}

			private bool IsIncrementExpression(Statement statement, VariableReference variableReference)
			{
				if (!(statement is ExpressionStatement))
				{
					return false;
				}
				ExpressionStatement expressionStatement = (ExpressionStatement)statement;
				if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
				{
					return false;
				}
				BinaryExpression expression = (BinaryExpression)expressionStatement.Expression;
				if (!(expression.Left is VariableReferenceExpression))
				{
					return false;
				}
				if ((object)(expression.Left as VariableReferenceExpression).Variable != (object)variableReference)
				{
					return false;
				}
				if (!(expression.Right is BinaryExpression))
				{
					return false;
				}
				BinaryExpression right = (BinaryExpression)expression.Right;
				if (right.Operator != BinaryOperator.Add)
				{
					return false;
				}
				if (!this.CheckLiteralExpressionValue(right.Right, 1))
				{
					return false;
				}
				if (!(right.Left is VariableReferenceExpression))
				{
					return false;
				}
				if ((object)(right.Left as VariableReferenceExpression).Variable != (object)variableReference)
				{
					return false;
				}
				return true;
			}

			internal bool Match()
			{
				if (!(this.nextStatement is WhileStatement))
				{
					return false;
				}
				WhileStatement whileStatement = (WhileStatement)this.nextStatement;
				if (whileStatement.Body.Statements.Count < 2)
				{
					return false;
				}
				if (!(this.statement is ExpressionStatement))
				{
					return false;
				}
				VariableReference variableReference = this.GetVariableReference();
				if (!(whileStatement.Condition is BinaryExpression))
				{
					return false;
				}
				BinaryExpression condition = (BinaryExpression)whileStatement.Condition;
				if (condition.Operator != BinaryOperator.LessThan)
				{
					return false;
				}
				if (!(condition.Left is VariableReferenceExpression))
				{
					return false;
				}
				if ((object)((VariableReferenceExpression)condition.Left).Variable != (object)variableReference)
				{
					return false;
				}
				PropertyReferenceExpression propertyReferenceFromCast = this.GetPropertyReferenceFromCast(condition.Right);
				if (propertyReferenceFromCast == null)
				{
					return false;
				}
				if (!RebuildForeachArrayStatements.ForeachArrayMatcher.IsArrayExpression(propertyReferenceFromCast.Target))
				{
					return false;
				}
				this.CurrentVariable = this.CheckAssingExpression(whileStatement.Body.Statements[0], variableReference, propertyReferenceFromCast.Target);
				if (this.CurrentVariable == null)
				{
					return false;
				}
				if (!this.IsIncrementExpression(whileStatement.Body.Statements[whileStatement.Body.Statements.Count - 1], variableReference))
				{
					return false;
				}
				this.Incrementor = variableReference.Resolve();
				this.source = propertyReferenceFromCast.Target;
				if (this.Incrementor == null)
				{
					return false;
				}
				this.CopyWhileBodyStatements(whileStatement);
				this.foreachConditionInstructions = whileStatement.Condition.UnderlyingSameMethodInstructions;
				return true;
			}
		}
	}
}