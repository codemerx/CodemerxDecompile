using System;
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
			base();
			return;
		}

		private BinaryExpression GetForAssignExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}
			if (expression.get_CodeNodeType() != 24 || !(expression as BinaryExpression).get_IsAssignmentExpression())
			{
				return null;
			}
			V_0 = (BinaryExpression)expression;
			if (V_0.get_Left().get_CodeNodeType() != 27 && V_0.get_Left().get_CodeNodeType() != 26 && V_0.get_Left().get_CodeNodeType() != 30)
			{
				return null;
			}
			if (V_0.get_IsAssignmentExpression())
			{
				return V_0;
			}
			return null;
		}

		private BinaryExpression GetForBinaryExpression(Expression condition)
		{
			if (condition.get_CodeNodeType() != 24)
			{
				return null;
			}
			V_0 = (BinaryExpression)condition;
			if (V_0.get_Left().get_CodeNodeType() != 26 && V_0.get_Left().get_CodeNodeType() != 30)
			{
				return null;
			}
			if (!this.IsForComparisonOperator(V_0.get_Operator()))
			{
				return null;
			}
			if (V_0.get_Right().get_CodeNodeType() != 22)
			{
				return null;
			}
			return V_0;
		}

		private Expression GetForIncrementExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}
			if (expression.get_CodeNodeType() == 23)
			{
				V_0 = (UnaryExpression)expression;
				if (V_0.get_Operator() != 3 && V_0.get_Operator() != 5 && V_0.get_Operator() != 4 && V_0.get_Operator() != 6)
				{
					return null;
				}
				return V_0;
			}
			if (expression.get_CodeNodeType() == 24 && (expression as BinaryExpression).get_Operator() == 2 || expression.get_CodeNodeType() == 24 && (expression as BinaryExpression).get_Operator() == 4)
			{
				return expression;
			}
			if (expression.get_CodeNodeType() == 24 && (expression as BinaryExpression).get_IsAssignmentExpression())
			{
				V_1 = (BinaryExpression)expression;
				if (this.IsSelfAssignExpression(V_1))
				{
					return V_1;
				}
			}
			return null;
		}

		private bool IsForComparisonOperator(BinaryOperator op)
		{
			if (op - 13 <= 3)
			{
				return true;
			}
			return false;
		}

		private bool IsSelfAssignExpression(BinaryExpression assingExpression)
		{
			if (!assingExpression.get_IsAssignmentExpression())
			{
				return false;
			}
			V_0 = assingExpression.get_Right() as BinaryExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_Left().get_CodeNodeType() != assingExpression.get_Left().get_CodeNodeType())
			{
				return false;
			}
			if (V_0.get_Operator() != 1 && V_0.get_Operator() != 3)
			{
				return false;
			}
			if (V_0.get_Right().get_CodeNodeType() != 22)
			{
				return false;
			}
			if (V_0.get_Left().get_CodeNodeType() == 30)
			{
				return (object)((FieldReferenceExpression)V_0.get_Left()).get_Field() == (object)((FieldReferenceExpression)assingExpression.get_Left()).get_Field();
			}
			if (V_0.get_Left().get_CodeNodeType() != 26)
			{
				return true;
			}
			return (object)((VariableReferenceExpression)V_0.get_Left()).get_Variable() == (object)((VariableReferenceExpression)assingExpression.get_Left()).get_Variable();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count() - 1)
			{
				if (node.get_Statements().get_Item(V_0) as ForStatement != null)
				{
					V_1 = node.get_Statements().get_Item(V_0) as ForStatement;
					stackVariable23 = this.GetForAssignExpression(V_1.get_Initializer());
					V_2 = this.GetForBinaryExpression(V_1.get_Condition());
					V_3 = this.GetForIncrementExpression(V_1.get_Increment());
					if (stackVariable23 == null || V_2 == null || V_3 == null)
					{
						V_4 = new DoWhileStatement(V_1.get_Condition(), V_1.get_Body());
						V_4.get_Body().AddStatement(new ExpressionStatement(V_1.get_Increment()));
						node.get_Statements().set_Item(V_0, new ExpressionStatement(V_1.get_Initializer()));
						node.get_Statements().Insert(V_0 + 1, V_4);
					}
				}
				V_0 = V_0 + 1;
			}
			this.VisitBlockStatement(node);
			return;
		}
	}
}