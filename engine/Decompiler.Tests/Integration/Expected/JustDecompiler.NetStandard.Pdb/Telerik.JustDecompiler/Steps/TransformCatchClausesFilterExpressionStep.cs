using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class TransformCatchClausesFilterExpressionStep : BaseCodeTransformer, IDecompilationStep
	{
		private TypeSystem typeSystem;

		public TransformCatchClausesFilterExpressionStep()
		{
		}

		private static BinaryExpression GetBinaryExpression(bool isNestedBinaryInUnary, Expression binaryAsExpression)
		{
			BinaryExpression binaryExpression;
			binaryExpression = (!isNestedBinaryInUnary ? binaryAsExpression as BinaryExpression : (binaryAsExpression as UnaryExpression).Operand as BinaryExpression);
			return binaryExpression;
		}

		private static Expression GetResultExpression(bool isNestedBinaryInUnary, Expression binaryAsExpression, BinaryExpression newBinaryExpression)
		{
			Expression expression;
			if (!isNestedBinaryInUnary)
			{
				expression = newBinaryExpression;
			}
			else
			{
				UnaryExpression unaryExpression = binaryAsExpression as UnaryExpression;
				unaryExpression.Operand = newBinaryExpression;
				expression = unaryExpression;
			}
			return expression;
		}

		private bool IsBinaryExpression(Expression expression, out Expression binaryExpression, out bool isWrappedInUnary)
		{
			binaryExpression = null;
			isWrappedInUnary = false;
			BinaryExpression binaryExpression1 = expression as BinaryExpression;
			UnaryExpression unaryExpression = expression as UnaryExpression;
			if (binaryExpression1 != null)
			{
				binaryExpression = binaryExpression1;
				return true;
			}
			if (unaryExpression == null || unaryExpression.Operator != UnaryOperator.None && unaryExpression.Operator != UnaryOperator.LogicalNot || !(unaryExpression.Operand is BinaryExpression))
			{
				return false;
			}
			if (unaryExpression.Operator != UnaryOperator.LogicalNot)
			{
				binaryExpression = unaryExpression.Operand;
			}
			else
			{
				binaryExpression = Negator.Negate(unaryExpression.Operand, this.typeSystem);
				if (!(binaryExpression is BinaryExpression))
				{
					isWrappedInUnary = true;
				}
			}
			return true;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			return (BlockStatement)this.Visit(body);
		}

		private Expression Transform(Expression expression)
		{
			Expression resultExpression = null;
			ConditionExpression conditionExpression = expression as ConditionExpression;
			Expression expression1 = null;
			bool flag = false;
			bool flag1 = false;
			if (this.IsBinaryExpression(expression, out expression1, out flag))
			{
				BinaryExpression binaryExpression = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(flag, expression1);
				if (this.TryTransformBinary(binaryExpression))
				{
					resultExpression = TransformCatchClausesFilterExpressionStep.GetResultExpression(flag, expression1, binaryExpression);
					flag1 = true;
				}
			}
			else if (conditionExpression != null && this.TryTransformTernary(conditionExpression, out resultExpression))
			{
				flag1 = true;
			}
			if (!flag1)
			{
				resultExpression = expression;
			}
			return resultExpression;
		}

		private bool TryTransform(BinaryExpression node)
		{
			Expression expression;
			Expression expression1;
			if (!node.IsAssignmentExpression)
			{
				return false;
			}
			if (node.ExpressionType.get_FullName() != "System.Boolean")
			{
				return false;
			}
			if (!(node.Left is VariableReferenceExpression))
			{
				return false;
			}
			BinaryExpression right = node.Right as BinaryExpression;
			if (right == null)
			{
				return false;
			}
			if (right.Operator != BinaryOperator.ValueInequality)
			{
				return false;
			}
			LiteralExpression literalExpression = right.Right as LiteralExpression;
			if (literalExpression == null || !(literalExpression.Value is Int32) || (Int32)literalExpression.Value != 0)
			{
				return false;
			}
			bool flag = false;
			if (this.IsBinaryExpression(right.Left, out expression, out flag))
			{
				BinaryExpression binaryExpression = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(flag, expression);
				if (this.TryTransformBinary(binaryExpression))
				{
					node.Right = TransformCatchClausesFilterExpressionStep.GetResultExpression(flag, expression, binaryExpression);
					return true;
				}
			}
			ConditionExpression left = right.Left as ConditionExpression;
			if (left != null && this.TryTransformTernary(left, out expression1))
			{
				node.Right = expression1;
				return true;
			}
			if (right.Left.ExpressionType.get_FullName() != "System.Boolean")
			{
				return false;
			}
			node.Right = right.Left;
			return true;
		}

		private bool TryTransformBinary(BinaryExpression binaryExpression)
		{
			Expression expression;
			Expression expression1;
			Expression expression2;
			Expression expression3;
			bool flag = false;
			if (binaryExpression.Left.ExpressionType.get_FullName() != "System.Boolean" || binaryExpression.Right.ExpressionType.get_FullName() != "System.Boolean")
			{
				return false;
			}
			bool flag1 = false;
			if (this.IsBinaryExpression(binaryExpression.Left, out expression, out flag1))
			{
				BinaryExpression binaryExpression1 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(flag1, expression);
				if (this.TryTransformBinary(binaryExpression1))
				{
					binaryExpression.Left = TransformCatchClausesFilterExpressionStep.GetResultExpression(flag1, expression, binaryExpression1);
					flag = true;
				}
			}
			flag1 = false;
			if (this.IsBinaryExpression(binaryExpression.Right, out expression1, out flag1))
			{
				BinaryExpression binaryExpression2 = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(flag1, expression1);
				if (this.TryTransformBinary(binaryExpression2))
				{
					binaryExpression.Right = TransformCatchClausesFilterExpressionStep.GetResultExpression(flag1, expression1, binaryExpression2);
					flag = true;
				}
			}
			ConditionExpression left = binaryExpression.Left as ConditionExpression;
			if (left != null && this.TryTransformTernary(left, out expression2))
			{
				binaryExpression.Left = expression2;
				flag = true;
			}
			ConditionExpression right = binaryExpression.Right as ConditionExpression;
			if (right != null && this.TryTransformTernary(right, out expression3))
			{
				binaryExpression.Right = expression3;
				flag = true;
			}
			return flag;
		}

		private bool TryTransformTernary(ConditionExpression ternary, out Expression transformed)
		{
			LiteralExpression literalExpression;
			bool flag;
			bool value;
			BinaryOperator binaryOperator;
			Expression expression;
			transformed = null;
			LiteralExpression then = ternary.Then as LiteralExpression;
			LiteralExpression @else = ternary.Else as LiteralExpression;
			if (then != null && @else != null)
			{
				bool flag1 = false;
				bool flag2 = false;
				if (then.ExpressionType.get_FullName() == "System.Int32" && @else.ExpressionType.get_FullName() == "System.Int32")
				{
					int num = (Int32)then.Value;
					int value1 = (Int32)@else.Value;
					if (num == 0 && value1 == 1 || num == 1 && value1 == 0)
					{
						flag1 = true;
						if (num == 0 && value1 == 1)
						{
							flag2 = true;
						}
					}
				}
				else if (then.ExpressionType.get_FullName() == "System.Boolean" && @else.ExpressionType.get_FullName() == "System.Boolean")
				{
					bool value2 = (Boolean)then.Value;
					bool value3 = (Boolean)@else.Value;
					if (!value2 && value3 || value2 && !value3)
					{
						flag1 = true;
						if (!value2 && value3)
						{
							flag2 = true;
						}
					}
				}
				if (flag1)
				{
					if (!flag2)
					{
						transformed = this.Transform(ternary.Condition);
					}
					else
					{
						transformed = this.Transform(Negator.Negate(ternary.Condition, this.typeSystem));
					}
					return true;
				}
			}
			if (then == null)
			{
				if (@else == null)
				{
					ExplicitCastExpression explicitCastExpression = ternary.Then as ExplicitCastExpression;
					ExplicitCastExpression else1 = ternary.Else as ExplicitCastExpression;
					if (explicitCastExpression == null || else1 == null || !(explicitCastExpression.TargetType.get_FullName() == "System.Int32") || !(explicitCastExpression.Expression.ExpressionType.get_FullName() == "System.Boolean") || !(else1.TargetType.get_FullName() == "System.Int32") || !(else1.Expression.ExpressionType.get_FullName() == "System.Boolean"))
					{
						return false;
					}
					ternary.Then = explicitCastExpression.Expression;
					ternary.Else = else1.Expression;
					transformed = ternary;
					return true;
				}
				literalExpression = @else;
				flag = false;
			}
			else
			{
				literalExpression = then;
				flag = true;
			}
			if (literalExpression.ExpressionType.get_FullName() != "System.Int32")
			{
				if (literalExpression.ExpressionType.get_FullName() != "System.Boolean")
				{
					return false;
				}
				value = (Boolean)literalExpression.Value;
			}
			else
			{
				value = ((Int32)literalExpression.Value != 0 ? true : false);
			}
			Expression expression1 = null;
			Expression resultExpression = null;
			expression1 = this.Transform(ternary.Condition);
			expression = (!flag ? ternary.Then : ternary.Else);
			ConditionExpression conditionExpression = expression as ConditionExpression;
			ExplicitCastExpression explicitCastExpression1 = expression as ExplicitCastExpression;
			Expression expression2 = null;
			bool flag3 = false;
			if (conditionExpression == null && (explicitCastExpression1 == null || explicitCastExpression1.TargetType.get_FullName() != "System.Int32" || explicitCastExpression1.Expression.ExpressionType.get_FullName() != "System.Boolean") && !this.IsBinaryExpression(expression, out expression2, out flag3) && expression.ExpressionType.get_FullName() != "System.Boolean")
			{
				return false;
			}
			bool flag4 = false;
			if (explicitCastExpression1 != null)
			{
				resultExpression = explicitCastExpression1.Expression;
				flag4 = true;
			}
			else if (expression2 != null)
			{
				BinaryExpression binaryExpression = TransformCatchClausesFilterExpressionStep.GetBinaryExpression(flag3, expression2);
				if (this.TryTransformBinary(binaryExpression))
				{
					resultExpression = TransformCatchClausesFilterExpressionStep.GetResultExpression(flag3, expression2, binaryExpression);
					flag4 = true;
				}
			}
			else if (conditionExpression != null && this.TryTransformTernary(conditionExpression, out resultExpression))
			{
				flag4 = true;
			}
			if (!flag4)
			{
				resultExpression = expression;
			}
			if (expression1 == null || resultExpression == null)
			{
				return false;
			}
			if (!value)
			{
				binaryOperator = BinaryOperator.LogicalAnd;
				expression1 = (flag ? Negator.Negate(expression1, this.typeSystem) : expression1);
			}
			else
			{
				binaryOperator = BinaryOperator.LogicalOr;
				expression1 = (flag ? expression1 : Negator.Negate(expression1, this.typeSystem));
			}
			transformed = new BinaryExpression(binaryOperator, expression1, resultExpression, this.typeSystem, ternary.MappedInstructions, false);
			return true;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (this.TryTransform(node))
			{
				return node;
			}
			return base.VisitBinaryExpression(node);
		}
	}
}