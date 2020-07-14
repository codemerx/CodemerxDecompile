using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class RaiseEventPattern : ICodePattern
	{
		public RaiseEventPattern()
		{
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			startIndex = 0;
			result = null;
			replacedStatementsCount = 0;
			for (int i = 0; i < statements.Count - 1; i++)
			{
				if (this.TryMatchInternal(statements, i, out result))
				{
					startIndex = i;
					replacedStatementsCount = 2;
					return true;
				}
			}
			return false;
		}

		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result)
		{
			result = null;
			if (startIndex + 1 >= statements.Count)
			{
				return false;
			}
			if (statements[startIndex].CodeNodeType != CodeNodeType.ExpressionStatement || statements[startIndex + 1].CodeNodeType != CodeNodeType.IfStatement)
			{
				return false;
			}
			ExpressionStatement item = statements[startIndex] as ExpressionStatement;
			if (item.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression expression = item.Expression as BinaryExpression;
			if (expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.EventReferenceExpression)
			{
				return false;
			}
			VariableReferenceExpression left = expression.Left as VariableReferenceExpression;
			EventReferenceExpression right = expression.Right as EventReferenceExpression;
			IfStatement ifStatement = statements[startIndex + 1] as IfStatement;
			if (ifStatement.Then == null || ifStatement.Else != null || ifStatement.Condition.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression condition = ifStatement.Condition as BinaryExpression;
			if (condition.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || condition.Right.CodeNodeType != CodeNodeType.LiteralExpression || condition.Operator != BinaryOperator.ValueInequality)
			{
				return false;
			}
			VariableReferenceExpression variableReferenceExpression = condition.Left as VariableReferenceExpression;
			if ((object)left.Variable != (object)variableReferenceExpression.Variable)
			{
				return false;
			}
			if ((condition.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			StatementCollection statementCollection = ifStatement.Then.Statements;
			if (statementCollection.Count != 1 || statementCollection[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			ExpressionStatement expressionStatement = statementCollection[0] as ExpressionStatement;
			if (expressionStatement.Expression.CodeNodeType != CodeNodeType.DelegateInvokeExpression)
			{
				return false;
			}
			DelegateInvokeExpression delegateInvokeExpression = expressionStatement.Expression as DelegateInvokeExpression;
			if (delegateInvokeExpression.Target == null || delegateInvokeExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return false;
			}
			VariableReferenceExpression target = delegateInvokeExpression.Target as VariableReferenceExpression;
			if ((object)target.Variable != (object)left.Variable)
			{
				return false;
			}
			List<Instruction> instructions = new List<Instruction>();
			instructions.AddRange(item.UnderlyingSameMethodInstructions);
			instructions.AddRange(condition.UnderlyingSameMethodInstructions);
			instructions.AddRange(delegateInvokeExpression.MappedInstructions);
			instructions.AddRange(target.UnderlyingSameMethodInstructions);
			result = new ExpressionStatement(new RaiseEventExpression(right.Event, delegateInvokeExpression.InvokeMethodReference, delegateInvokeExpression.Arguments, instructions));
			return true;
		}
	}
}