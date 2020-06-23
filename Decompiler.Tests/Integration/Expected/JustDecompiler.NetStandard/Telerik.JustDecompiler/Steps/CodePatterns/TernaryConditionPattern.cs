using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class TernaryConditionPattern : CommonPatterns, ICodePattern
	{
		public TernaryConditionPattern(CodePatternsContext patternsContext, TypeSystem typeSystem) : base(patternsContext, typeSystem)
		{
		}

		private Expression GetRightExpressionMapped(BinaryExpression assignExpression)
		{
			List<Instruction> instructions = new List<Instruction>(assignExpression.Left.UnderlyingSameMethodInstructions);
			instructions.AddRange(assignExpression.MappedInstructions);
			return assignExpression.Right.CloneAndAttachInstructions(instructions);
		}

		protected virtual bool ShouldInlineExpressions(BinaryExpression thenAssignExpression, BinaryExpression elseAssignExpression)
		{
			if ((thenAssignExpression.Right.CodeNodeType == CodeNodeType.ConditionExpression ? true : elseAssignExpression.Right.CodeNodeType == CodeNodeType.ConditionExpression))
			{
				return false;
			}
			if (!thenAssignExpression.Right.HasType || !elseAssignExpression.Right.HasType)
			{
				return false;
			}
			return thenAssignExpression.Right.ExpressionType.FullName == elseAssignExpression.Right.ExpressionType.FullName;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 1;
			startIndex = 0;
			while (startIndex < statements.Count)
			{
				if (statements[startIndex].CodeNodeType == CodeNodeType.IfStatement && this.TryMatchInternal(statements[startIndex] as IfStatement, out result))
				{
					return true;
				}
				startIndex++;
			}
			return false;
		}

		public bool TryMatchInternal(IfStatement theIfStatement, out Statement result)
		{
			result = null;
			if (theIfStatement == null)
			{
				return false;
			}
			VariableReference variableReference = null;
			VariableReference variableReference1 = null;
			if (theIfStatement.Else == null || theIfStatement.Then.Statements.Count != 1 || theIfStatement.Else.Statements.Count != 1 || theIfStatement.Then.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement || theIfStatement.Else.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			BinaryExpression expression = (theIfStatement.Then.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			BinaryExpression binaryExpression = (theIfStatement.Else.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (!base.IsAssignToVariableExpression(expression, out variableReference1) || !base.IsAssignToVariableExpression(binaryExpression, out variableReference))
			{
				return false;
			}
			if (variableReference != variableReference1)
			{
				return false;
			}
			if (!this.ShouldInlineExpressions(expression, binaryExpression))
			{
				return false;
			}
			Expression rightExpressionMapped = this.GetRightExpressionMapped(expression);
			Expression rightExpressionMapped1 = this.GetRightExpressionMapped(binaryExpression);
			ConditionExpression conditionExpression = new ConditionExpression(theIfStatement.Condition, rightExpressionMapped, rightExpressionMapped1, null);
			BinaryExpression binaryExpression1 = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(variableReference, null), conditionExpression, this.typeSystem, null, false);
			result = new ExpressionStatement(binaryExpression1)
			{
				Parent = theIfStatement.Parent
			};
			base.FixContext(variableReference.Resolve(), 1, 0, result as ExpressionStatement);
			return true;
		}
	}
}