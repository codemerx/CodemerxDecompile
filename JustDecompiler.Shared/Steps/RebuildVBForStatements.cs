using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
	class RebuildVBForStatements : RebuildForStatements
	{
		protected override bool CheckTheLoop(WhileStatement theWhile, VariableReference forVariable)
		{
			bool isProperForVBForLoop = base.CheckTheLoop(theWhile, forVariable) && theWhile.Condition is BinaryExpression;

			if (!isProperForVBForLoop)
			{
				return false;
			}

			ExpressionStatement incrementCandidate = theWhile.Body.Statements[theWhile.Body.Statements.Count - 1] as ExpressionStatement;
			BinaryExpression assignmentExpression = incrementCandidate.Expression as BinaryExpression;
			if (assignmentExpression != null)
			{
				BinaryExpression incrementExpression = assignmentExpression.Right as BinaryExpression;
				if (incrementExpression != null && (incrementExpression.Operator == Ast.BinaryOperator.Add || incrementExpression.Operator == Ast.BinaryOperator.Subtract))
				{
					VariableReferenceExpression incrementVariableExpression = incrementExpression.Left as VariableReferenceExpression;
					if (incrementVariableExpression != null)
					{
						if (incrementVariableExpression.Variable == forVariable)
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
