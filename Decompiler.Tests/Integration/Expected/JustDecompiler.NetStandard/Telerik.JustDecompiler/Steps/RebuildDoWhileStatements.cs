using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildDoWhileStatements : BaseCodeVisitor, IDecompilationStep
	{
		public RebuildDoWhileStatements()
		{
		}

		private BinaryExpression GetForAssignExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}
			if (expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expression as BinaryExpression).IsAssignmentExpression)
			{
				return null;
			}
			BinaryExpression binaryExpression = (BinaryExpression)expression;
			if (binaryExpression.Left.CodeNodeType != CodeNodeType.VariableDeclarationExpression && binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression && binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return null;
			}
			if (binaryExpression.IsAssignmentExpression)
			{
				return binaryExpression;
			}
			return null;
		}

		private BinaryExpression GetForBinaryExpression(Expression condition)
		{
			if (condition.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return null;
			}
			BinaryExpression binaryExpression = (BinaryExpression)condition;
			if (binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression && binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return null;
			}
			if (!this.IsForComparisonOperator(binaryExpression.Operator))
			{
				return null;
			}
			if (binaryExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return null;
			}
			return binaryExpression;
		}

		private Expression GetForIncrementExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}
			if (expression.CodeNodeType == CodeNodeType.UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;
				if (unaryExpression.Operator != UnaryOperator.PostDecrement && unaryExpression.Operator != UnaryOperator.PreDecrement && unaryExpression.Operator != UnaryOperator.PostIncrement && unaryExpression.Operator != UnaryOperator.PreIncrement)
				{
					return null;
				}
				return unaryExpression;
			}
			if (expression.CodeNodeType == CodeNodeType.BinaryExpression && (expression as BinaryExpression).Operator == BinaryOperator.AddAssign || expression.CodeNodeType == CodeNodeType.BinaryExpression && (expression as BinaryExpression).Operator == BinaryOperator.SubtractAssign)
			{
				return expression;
			}
			if (expression.CodeNodeType == CodeNodeType.BinaryExpression && (expression as BinaryExpression).IsAssignmentExpression)
			{
				BinaryExpression binaryExpression = (BinaryExpression)expression;
				if (this.IsSelfAssignExpression(binaryExpression))
				{
					return binaryExpression;
				}
			}
			return null;
		}

		private bool IsForComparisonOperator(BinaryOperator op)
		{
			if ((int)op - (int)BinaryOperator.LessThan <= (int)BinaryOperator.Subtract)
			{
				return true;
			}
			return false;
		}

		private bool IsSelfAssignExpression(BinaryExpression assingExpression)
		{
			if (!assingExpression.IsAssignmentExpression)
			{
				return false;
			}
			BinaryExpression right = assingExpression.Right as BinaryExpression;
			if (right == null)
			{
				return false;
			}
			if (right.Left.CodeNodeType != assingExpression.Left.CodeNodeType)
			{
				return false;
			}
			if (right.Operator != BinaryOperator.Add && right.Operator != BinaryOperator.Subtract)
			{
				return false;
			}
			if (right.Right.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}
			if (right.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
			{
				return ((FieldReferenceExpression)right.Left).Field == ((FieldReferenceExpression)assingExpression.Left).Field;
			}
			if (right.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return true;
			}
			return ((VariableReferenceExpression)right.Left).Variable == ((VariableReferenceExpression)assingExpression.Left).Variable;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				if (node.Statements[i] is ForStatement)
				{
					ForStatement item = node.Statements[i] as ForStatement;
					BinaryExpression forAssignExpression = this.GetForAssignExpression(item.Initializer);
					BinaryExpression forBinaryExpression = this.GetForBinaryExpression(item.Condition);
					Expression forIncrementExpression = this.GetForIncrementExpression(item.Increment);
					if (forAssignExpression == null || forBinaryExpression == null || forIncrementExpression == null)
					{
						DoWhileStatement doWhileStatement = new DoWhileStatement(item.Condition, item.Body);
						doWhileStatement.Body.AddStatement(new ExpressionStatement(item.Increment));
						node.Statements[i] = new ExpressionStatement(item.Initializer);
						node.Statements.Insert(i + 1, doWhileStatement);
					}
				}
			}
			base.VisitBlockStatement(node);
		}
	}
}