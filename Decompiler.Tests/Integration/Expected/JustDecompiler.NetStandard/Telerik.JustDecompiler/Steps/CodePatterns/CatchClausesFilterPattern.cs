using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal static class CatchClausesFilterPattern
	{
		private static bool TryGetVariableDeclaration(ExpressionStatement statement, VariableReferenceExpression variableReference, ref VariableDefinition variableDefinition, ref IEnumerable<Instruction> instructions)
		{
			if (statement != null)
			{
				BinaryExpression expression = statement.Expression as BinaryExpression;
				if (expression != null && expression.IsAssignmentExpression)
				{
					VariableReferenceExpression left = expression.Left as VariableReferenceExpression;
					VariableReferenceExpression right = expression.Right as VariableReferenceExpression;
					if (left != null && right != null && right.Equals(variableReference))
					{
						variableDefinition = left.Variable.Resolve();
						instructions = left.MappedInstructions;
						return true;
					}
				}
			}
			return false;
		}

		public static bool TryMatch(BlockStatement filter, out VariableDeclarationExpression variableDeclaration, out Expression filterExpression)
		{
			variableDeclaration = null;
			filterExpression = null;
			if (!CatchClausesFilterPattern.TryMatchVariableDeclaration(filter, out variableDeclaration))
			{
				return false;
			}
			if (filter.Statements.Count != 3)
			{
				return false;
			}
			IfStatement item = filter.Statements[1] as IfStatement;
			if (item == null)
			{
				return false;
			}
			BlockStatement @else = null;
			BlockStatement then = null;
			if ((item.Condition as BinaryExpression).Operator != BinaryOperator.ValueInequality)
			{
				@else = item.Else;
				then = item.Then;
			}
			else
			{
				@else = item.Then;
				then = item.Else;
			}
			ExpressionStatement expressionStatement = null;
			ExpressionStatement expressionStatement1 = null;
			if (@else.Statements.Count != 1 && @else.Statements.Count != 2 && @else.Statements.Count != 3 || then.Statements.Count != 1)
			{
				return false;
			}
			if (@else.Statements.Count == 2)
			{
				ExpressionStatement item1 = @else.Statements[0] as ExpressionStatement;
				if (item1 == null)
				{
					return false;
				}
				if (item1.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
				{
					if (item1.Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
					{
						return false;
					}
					expressionStatement1 = item1;
				}
				else
				{
					expressionStatement = item1;
				}
			}
			else if (@else.Statements.Count == 3)
			{
				ExpressionStatement item2 = @else.Statements[0] as ExpressionStatement;
				ExpressionStatement expressionStatement2 = @else.Statements[1] as ExpressionStatement;
				if (item2 == null || expressionStatement2 == null)
				{
					return false;
				}
				if (item2.Expression.CodeNodeType != CodeNodeType.BinaryExpression || expressionStatement2.Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
				{
					if (item2.Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression || expressionStatement2.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
					{
						return false;
					}
					expressionStatement1 = item2;
					expressionStatement = expressionStatement2;
				}
				else
				{
					expressionStatement = item2;
					expressionStatement1 = expressionStatement2;
				}
			}
			if (expressionStatement != null)
			{
				BinaryExpression expression = expressionStatement.Expression as BinaryExpression;
				if (expression == null || !expression.IsAssignmentExpression || expression.ExpressionType.FullName != variableDeclaration.ExpressionType.FullName)
				{
					return false;
				}
				if (expression.Left as VariableReferenceExpression == null || expression.Right as VariableReferenceExpression == null)
				{
					return false;
				}
			}
			if (expressionStatement1 != null)
			{
				MethodInvocationExpression methodInvocationExpression = expressionStatement1.Expression as MethodInvocationExpression;
				if (methodInvocationExpression == null || methodInvocationExpression.MethodExpression.Method.FullName != "System.Void Microsoft.VisualBasic.CompilerServices.ProjectData::SetProjectError(System.Exception)")
				{
					return false;
				}
			}
			ExpressionStatement item3 = filter.Statements[2] as ExpressionStatement;
			if (item3 == null)
			{
				return false;
			}
			VariableReferenceExpression variableReferenceExpression = item3.Expression as VariableReferenceExpression;
			if (variableReferenceExpression == null)
			{
				return false;
			}
			if (!CatchClausesFilterPattern.TryMatchFilterExpression(item, variableDeclaration.Variable.VariableType, variableReferenceExpression, out filterExpression))
			{
				return false;
			}
			return true;
		}

		private static bool TryMatchFilterExpression(IfStatement ifStatement, TypeReference variableType, VariableReferenceExpression lastExpression, out Expression filterExpression)
		{
			ExpressionStatement expressionStatement;
			filterExpression = null;
			expressionStatement = ((ifStatement.Condition as BinaryExpression).Operator != BinaryOperator.ValueInequality ? ifStatement.Else.Statements[ifStatement.Then.Statements.Count - 1] as ExpressionStatement : ifStatement.Then.Statements[ifStatement.Then.Statements.Count - 1] as ExpressionStatement);
			if (expressionStatement == null)
			{
				return false;
			}
			BinaryExpression expression = expressionStatement.Expression as BinaryExpression;
			if (!expression.IsAssignmentExpression)
			{
				return false;
			}
			if (expression.ExpressionType.FullName != "System.Boolean")
			{
				return false;
			}
			VariableReferenceExpression left = expression.Left as VariableReferenceExpression;
			if (left == null)
			{
				return false;
			}
			if (!left.Equals(lastExpression))
			{
				return false;
			}
			filterExpression = expression.Right;
			return true;
		}

		public static bool TryMatchMethodStructure(BlockStatement blockStatement)
		{
			ExpressionStatement expressionStatement = blockStatement.Statements.Last<Statement>() as ExpressionStatement;
			if (expressionStatement == null)
			{
				return false;
			}
			VariableReferenceExpression expression = expressionStatement.Expression as VariableReferenceExpression;
			if (expression == null)
			{
				return false;
			}
			if (expression.ExpressionType.FullName != "System.Boolean")
			{
				return false;
			}
			return true;
		}

		private static bool TryMatchVariableDeclaration(BlockStatement filter, out VariableDeclarationExpression variableDeclaration)
		{
			BlockStatement blockStatement;
			variableDeclaration = null;
			ExpressionStatement item = filter.Statements[0] as ExpressionStatement;
			if (item == null)
			{
				return false;
			}
			BinaryExpression expression = item.Expression as BinaryExpression;
			if (expression == null || !expression.IsAssignmentExpression)
			{
				return false;
			}
			VariableReferenceExpression left = expression.Left as VariableReferenceExpression;
			if (left == null)
			{
				return false;
			}
			if (!(expression.Right is SafeCastExpression))
			{
				return false;
			}
			IfStatement ifStatement = filter.Statements[1] as IfStatement;
			if (ifStatement == null)
			{
				return false;
			}
			BinaryExpression condition = ifStatement.Condition as BinaryExpression;
			if (condition == null)
			{
				return false;
			}
			VariableReferenceExpression variableReferenceExpression = condition.Left as VariableReferenceExpression;
			if (variableReferenceExpression == null)
			{
				return false;
			}
			LiteralExpression right = condition.Right as LiteralExpression;
			if (right == null)
			{
				return false;
			}
			if (!variableReferenceExpression.Equals(left) || right.Value != null || condition.Operator != BinaryOperator.ValueEquality && condition.Operator != BinaryOperator.ValueInequality)
			{
				return false;
			}
			VariableDefinition variableDefinition = left.Variable.Resolve();
			IEnumerable<Instruction> mappedInstructions = left.MappedInstructions;
			blockStatement = (condition.Operator != BinaryOperator.ValueInequality ? ifStatement.Else : ifStatement.Then);
			if (!CatchClausesFilterPattern.TryGetVariableDeclaration(blockStatement.Statements[0] as ExpressionStatement, left, ref variableDefinition, ref mappedInstructions) && blockStatement.Statements.Count >= 2)
			{
				CatchClausesFilterPattern.TryGetVariableDeclaration(blockStatement.Statements[1] as ExpressionStatement, left, ref variableDefinition, ref mappedInstructions);
			}
			variableDeclaration = new VariableDeclarationExpression(variableDefinition, mappedInstructions);
			return true;
		}
	}
}