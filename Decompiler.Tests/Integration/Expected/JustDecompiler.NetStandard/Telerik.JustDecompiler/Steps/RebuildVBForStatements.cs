using Mono.Cecil.Cil;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildVBForStatements : RebuildForStatements
	{
		public RebuildVBForStatements()
		{
		}

		protected override bool CheckTheLoop(WhileStatement theWhile, VariableReference forVariable)
		{
			if ((!base.CheckTheLoop(theWhile, forVariable) ? true : !(theWhile.Condition is BinaryExpression)))
			{
				return false;
			}
			BinaryExpression expression = (theWhile.Body.Statements[theWhile.Body.Statements.Count - 1] as ExpressionStatement).Expression as BinaryExpression;
			if (expression != null)
			{
				BinaryExpression right = expression.Right as BinaryExpression;
				if (right != null && (right.Operator == BinaryOperator.Add || right.Operator == BinaryOperator.Subtract))
				{
					VariableReferenceExpression left = right.Left as VariableReferenceExpression;
					if (left != null && left.Variable == forVariable)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}