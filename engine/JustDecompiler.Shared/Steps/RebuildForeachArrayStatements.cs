using System;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForeachArrayStatements : Ast.BaseCodeVisitor, IDecompilationStep
	{
        private readonly Stack<VariableDefinition> currentForIndeces = new Stack<VariableDefinition>();
        private readonly Stack<bool> currentForIndecesUsed = new Stack<bool>();

		DecompilationContext context;

		public override void VisitBlockStatement (BlockStatement node)
		{
			ProcessBlock (node);

			foreach (Statement statement in node.Statements)
			{
				Visit(statement);
			}
		}

		void ProcessBlock (BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				ForeachArrayMatcher matcher = new ForeachArrayMatcher(node.Statements[i], node.Statements[i + 1], this.context.MethodContext);
				if (!matcher.Match())
				{
					continue;
				}

				if (CheckForIndexUsages(matcher))
				{
					continue;
				}

				context.MethodContext.RemoveVariable(matcher.Incrementor);
				if (matcher.CurrentVariable != null)
				{
					context.MethodContext.RemoveVariable(matcher.CurrentVariable);
				}

				node.Statements.RemoveAt(i);
				node.Statements.RemoveAt(i);
				node.AddStatementAt(i, matcher.Foreach);
				ProcessBlock(matcher.Foreach.Body);
			}
		}

		private bool CheckForIndexUsages(ForeachArrayMatcher matcher)
		{
			currentForIndeces.Push(matcher.Incrementor);
			currentForIndecesUsed.Push(false);
			foreach (var childStatement in matcher.Foreach.Body.Statements)
			{
				Visit(childStatement);
			}
			currentForIndeces.Pop();
			return currentForIndecesUsed.Pop();
		}

        public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
			if (currentForIndeces.Count > 0)
			{
				var currentForIndex = currentForIndeces.Peek();
				if (currentForIndex == node.Variable)
				{
					if (!currentForIndecesUsed.Peek())
					{
						currentForIndecesUsed.Pop();
						currentForIndecesUsed.Push(true);
					}
				}
			}
			base.VisitVariableReferenceExpression(node);
		}

		public BlockStatement Process (DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			Visit (body);
			this.context = null;
			return body;
		}

		class ForeachArrayMatcher
		{
            readonly Statement statement;
            readonly Statement nextStatement;
            private readonly MethodSpecificContext methodContext;

			ForEachStatement @foreach;

            Expression source;
			BlockStatement statementBody;
            IEnumerable<Instruction> foreachConditionInstructions;

			public ForEachStatement Foreach
			{
				get
				{
					@foreach = @foreach ?? new ForEachStatement(
						new VariableDeclarationExpression(this.CurrentVariable.Resolve(), null),
						this.source,
						this.statementBody, foreachConditionInstructions,
                        null);
					return @foreach;
				}
			}

			public VariableDefinition Incrementor { get; private set; }

			public VariableReference CurrentVariable { get; private set; }

			public ForeachArrayMatcher(Statement statement, Statement nextStatement, MethodSpecificContext methodContext)
			{
				this.statement = statement;
				this.nextStatement = nextStatement;
                this.methodContext = methodContext;
			}

			internal bool Match()
			{
				if (!(nextStatement is WhileStatement))
					return false;

				var whileStatement = (WhileStatement) nextStatement;

				if (whileStatement.Body.Statements.Count < 2)
					return false;
				
				if (!(statement is ExpressionStatement))
					return false;

				var variableReference = GetVariableReference();

				if (!(whileStatement.Condition is BinaryExpression))
					return false;

				var binaryExpression = (BinaryExpression) whileStatement.Condition;

				if (binaryExpression.Operator != BinaryOperator.LessThan)
					return false;

                if (!(binaryExpression.Left is VariableReferenceExpression))
                    return false;

                var conditionVariableReference = ((VariableReferenceExpression) binaryExpression.Left).Variable;
                if (conditionVariableReference != variableReference)
					return false;

				var propertyReference = GetPropertyReferenceFromCast(binaryExpression.Right);

				if (propertyReference == null)
					return false;

				if (!IsArrayExpression(propertyReference.Target))
					return false;

				CurrentVariable = CheckAssingExpression(whileStatement.Body.Statements[0], variableReference, propertyReference.Target);

				if (CurrentVariable == null)
					return false;

				if (!IsIncrementExpression(whileStatement.Body.Statements[whileStatement.Body.Statements.Count - 1], variableReference))
					return false;

				Incrementor = variableReference.Resolve();
				source = propertyReference.Target;

				if (Incrementor == null)
					return false;

				CopyWhileBodyStatements(whileStatement);
                foreachConditionInstructions = whileStatement.Condition.UnderlyingSameMethodInstructions;

				return true;
			}

			private void CopyWhileBodyStatements(WhileStatement whileStatement)
			{
				statementBody = new BlockStatement();
				for (int i = 1; i < whileStatement.Body.Statements.Count - 1; i++)
				{
					statementBody.AddStatement(whileStatement.Body.Statements[i]);
				}
			}

            private VariableReference GetVariableReference()
            {
				var expressionStatement = (ExpressionStatement) statement;

				if (!(expressionStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression && 
                    (expressionStatement.Expression as BinaryExpression).IsAssignmentExpression))
					return null;

				var assignExpression = (BinaryExpression) expressionStatement.Expression;

                if (!(assignExpression.Left is VariableReferenceExpression))
                    return null;

				if (!CheckLiteralExpressionValue(assignExpression.Right, 0))
					return null;

                return ((VariableReferenceExpression) assignExpression.Left).Variable;
            }

            private PropertyReferenceExpression GetPropertyReferenceFromCast(Expression expression)
            {
				if (!(expression is ExplicitCastExpression))
					return null;

				var castExpression = (ExplicitCastExpression) expression;

				if (castExpression.TargetType.FullName != "System.Int32")
					return null;

				if (!(castExpression.Expression is PropertyReferenceExpression))
					return null;

				var propertyReference = (PropertyReferenceExpression) castExpression.Expression;

				if (propertyReference.Property.FullName != "Int32.System Length()")
					return null;

				return propertyReference;
			}

            private bool CheckLiteralExpressionValue(Expression expression, int value)
            {
				if (!(expression is LiteralExpression))
					return false;

				var literalExpression = (LiteralExpression) expression;

				if (!(literalExpression.Value is int) || ((int) literalExpression.Value != value))
					return false;

				return true;
			}

			private bool IsIncrementExpression(Statement statement, VariableReference variableReference)
			{
				if (!(statement is ExpressionStatement))
					return false;

				var lastStatement = (ExpressionStatement) statement;

				if (!(lastStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression && 
                     (lastStatement.Expression as BinaryExpression).IsAssignmentExpression))
					return false;

				var assignExpression = (BinaryExpression) lastStatement.Expression;

                if (!(assignExpression.Left is VariableReferenceExpression))
                    return false;

                if ((assignExpression.Left as VariableReferenceExpression).Variable != variableReference)
                    return false;

				if (!(assignExpression.Right is BinaryExpression))
					return false;

				var binaryExpression = (BinaryExpression) assignExpression.Right;

				if (binaryExpression.Operator != BinaryOperator.Add)
					return false;

				if (!CheckLiteralExpressionValue(binaryExpression.Right, 1))
					return false;

                if (!(binaryExpression.Left is VariableReferenceExpression))
                    return false;

                if ((binaryExpression.Left as VariableReferenceExpression).Variable != variableReference)
                    return false;

				return true;
			}

            private VariableReference CheckAssingExpression(Statement statement, VariableReference variableReference, Expression arrayExpression)
            {
				if (!(statement is ExpressionStatement))
					return null;

				var firstStatement = (ExpressionStatement) statement;

				if (CheckArrayIndexer(firstStatement.Expression, variableReference, arrayExpression))
				{
					TypeReference arrayElementType = GetArrayElementType(arrayExpression);
					if (arrayElementType != null)
					{
                        VariableDefinition newVariable = new VariableDefinition(arrayElementType, this.methodContext.Method);
                        methodContext.VariablesToRename.Add(newVariable);
                        return newVariable;
					}
				}

				if (!(firstStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression &&
                     (firstStatement.Expression as BinaryExpression).IsAssignmentExpression))
					return null;

				var firstAssingment = (BinaryExpression) firstStatement.Expression;
				
                if (!(firstAssingment.Left is VariableReferenceExpression))
                    return null;

				if (!CheckArrayIndexer(firstAssingment.Right, variableReference, arrayExpression))
					return null;

                return (firstAssingment.Left as VariableReferenceExpression).Variable;
            }

            private TypeReference GetArrayElementType(Expression arrayExpression)
            {
				var typeReference = arrayExpression.GetTargetTypeReference();
				if (typeReference.IsArray)
				{
					var elementType = typeReference.GetElementType();
					return elementType;
				}
				return null;
			}

            private bool CheckArrayIndexer(Expression expression, VariableReference variableReference, Expression arrayExpression)
            {
				if (!(expression is ArrayIndexerExpression))
					return false;

				var arrayIndexer = (ArrayIndexerExpression) expression;

				if (arrayIndexer.Indices.Count != 1)
					return false;

                if (!(arrayIndexer.Indices[0] is VariableReferenceExpression))
                    return false;

                var indexerVariable = ((VariableReferenceExpression) arrayIndexer.Indices[0]).Variable;

				if (indexerVariable != variableReference)
					return false;

				if (!arrayIndexer.Target.CheckInnerReferenceExpressions(arrayExpression))
					return false;

				return true;
			}

            private static bool IsArrayExpression(Expression expression)
            {
				var typeReference = expression.GetTargetTypeReference();
				if (typeReference != null)
				{
					return typeReference.IsArray;
				}
				return false;
			}
		}
	}
}