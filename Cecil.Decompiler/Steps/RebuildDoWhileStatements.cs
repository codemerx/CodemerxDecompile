using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	class RebuildDoWhileStatements : BaseCodeVisitor, IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				if (node.Statements[i] is ForStatement)
				{
					ForStatement forStatement = node.Statements[i] as ForStatement;

					BinaryExpression assingExpression = GetForAssignExpression(forStatement.Initializer);
					BinaryExpression binaryExpression = GetForBinaryExpression(forStatement.Condition);
					Expression incrementExpression = GetForIncrementExpression(forStatement.Increment);

					if (assingExpression == null || binaryExpression == null || incrementExpression == null)
					{
						DoWhileStatement doWhileStatement = new DoWhileStatement(forStatement.Condition, forStatement.Body);
						doWhileStatement.Body.AddStatement(new ExpressionStatement(forStatement.Increment));
						node.Statements[i] = new ExpressionStatement(forStatement.Initializer);
						node.Statements.Insert(i + 1, doWhileStatement);
					}
				}
			}

			base.VisitBlockStatement(node);
		}

		private BinaryExpression GetForAssignExpression(Expression expression)
		{
			if (expression == null)
				return null;

			if (expression.CodeNodeType != CodeNodeType.BinaryExpression ||
				!(expression as BinaryExpression).IsAssignmentExpression)
				return null;

			BinaryExpression assignExpression = (BinaryExpression)expression;

			if ((assignExpression.Left.CodeNodeType != CodeNodeType.VariableDeclarationExpression) &&
				(assignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression) &&
				(assignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression))
				return null;

			if (assignExpression.IsAssignmentExpression)
			{
				return assignExpression;
			}
			return null;
		}

		private BinaryExpression GetForBinaryExpression(Expression condition)
		{
			if (condition.CodeNodeType != CodeNodeType.BinaryExpression)
				return null;

			var binaryExpression = (BinaryExpression)condition;
			if ((binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression) &&
				(binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression))
				return null;

			if (!IsForComparisonOperator(binaryExpression.Operator))
				return null;

			if (binaryExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
				return null;

			return binaryExpression;
		}

		private Expression GetForIncrementExpression(Expression expression)
		{
			if (expression == null)
				return null;

			if (expression.CodeNodeType == CodeNodeType.UnaryExpression)
			{
				var unaryExpression = (UnaryExpression)expression;

				if ((unaryExpression.Operator != UnaryOperator.PostDecrement) &&
					(unaryExpression.Operator != UnaryOperator.PreDecrement) &&
					(unaryExpression.Operator != UnaryOperator.PostIncrement) &&
					(unaryExpression.Operator != UnaryOperator.PreIncrement))
					return null;

				return unaryExpression;
			}
			if ((expression.CodeNodeType == CodeNodeType.BinaryExpression &&
				(expression as BinaryExpression).Operator == BinaryOperator.AddAssign) ||
				(expression.CodeNodeType == CodeNodeType.BinaryExpression &&
				(expression as BinaryExpression).Operator == BinaryOperator.SubtractAssign))
			{
				return expression;
			}
			if (expression.CodeNodeType == CodeNodeType.BinaryExpression &&
				(expression as BinaryExpression).IsAssignmentExpression)
			{
				var assingExpression = (BinaryExpression)expression;
				bool isSelfAssignExpression = IsSelfAssignExpression(assingExpression);
				if (isSelfAssignExpression)
				{
					return assingExpression;
				}
			}
			return null;
		}

		private bool IsSelfAssignExpression(BinaryExpression assingExpression)
		{
			if (!assingExpression.IsAssignmentExpression)
			{
				return false;
			}
			var binaryExpression = assingExpression.Right as BinaryExpression;
			if (binaryExpression == null)
				return false;

			if (binaryExpression.Left.CodeNodeType != assingExpression.Left.CodeNodeType)
				return false;

			if ((binaryExpression.Operator != BinaryOperator.Add) &&
				(binaryExpression.Operator != BinaryOperator.Subtract))
				return false;

			if (binaryExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
				return false;

			if (binaryExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
			{
				return (((FieldReferenceExpression)binaryExpression.Left).Field == ((FieldReferenceExpression)assingExpression.Left).Field);
			}

			if (binaryExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				return (((VariableReferenceExpression)binaryExpression.Left).Variable == ((VariableReferenceExpression)assingExpression.Left).Variable);
			}
			return true;
		}

		private bool IsForComparisonOperator(BinaryOperator op)
		{
			switch (op)
			{
				case BinaryOperator.GreaterThan:
				case BinaryOperator.LessThan:
				case BinaryOperator.GreaterThanOrEqual:
				case BinaryOperator.LessThanOrEqual:
					return true;
			}
			return false;
		}
	}
}
