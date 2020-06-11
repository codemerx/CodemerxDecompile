using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class TernaryConditionPattern : CommonPatterns, ICodePattern
	{
		public TernaryConditionPattern(CodePatternsContext patternsContext, TypeSystem typeSystem) :  base(patternsContext, typeSystem)
		{
		}

		//x = cond ? y : z;
		//
		//==
		//
		//if(cond)
		//{
		//      x = y;
		//}
		//else
		//{
		//      x = z;
		//}
		//
		//x - phi variable
		//y, z - expressions

		public bool TryMatchInternal(IfStatement theIfStatement, out Statement result)
		{
			result = null;

			if (theIfStatement == null)
			{
				return false;
			}

			VariableReference xVariableReference = null;
			VariableReference x1VariableReference = null;
			Expression yExpressionValue;
			Expression zExpressionValue;

			if (theIfStatement.Else == null ||
				theIfStatement.Then.Statements.Count != 1 || theIfStatement.Else.Statements.Count != 1 ||
				theIfStatement.Then.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement ||
				theIfStatement.Else.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}

			BinaryExpression thenAssignExpression = (theIfStatement.Then.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			BinaryExpression elseAssignExpression = (theIfStatement.Else.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (!IsAssignToVariableExpression(thenAssignExpression, out x1VariableReference) || 
				!IsAssignToVariableExpression(elseAssignExpression, out xVariableReference))
			{
				return false;
			}

			if (xVariableReference != x1VariableReference)
			{
				return false;
			}
			if (!ShouldInlineExpressions(thenAssignExpression, elseAssignExpression))
			{
				/// Although correct syntax, nesting ternary expressions makes the code very unreadable.
				return false;
			}

			yExpressionValue = GetRightExpressionMapped(thenAssignExpression);
            zExpressionValue = GetRightExpressionMapped(elseAssignExpression);

			ConditionExpression ternaryConditionExpression = new ConditionExpression(theIfStatement.Condition, yExpressionValue, zExpressionValue, null);
			BinaryExpression ternaryAssign = new BinaryExpression(BinaryOperator.Assign,
				new VariableReferenceExpression(xVariableReference, null), ternaryConditionExpression, this.typeSystem, null);

            result = new ExpressionStatement(ternaryAssign) { Parent = theIfStatement.Parent };
            FixContext(xVariableReference.Resolve(), 1, 0, result as ExpressionStatement);
			return true;
		}

        private Expression GetRightExpressionMapped(BinaryExpression assignExpression)
        {
            List<Instruction> instructions = new List<Instruction>(assignExpression.Left.UnderlyingSameMethodInstructions);
            instructions.AddRange(assignExpression.MappedInstructions);
            return assignExpression.Right.CloneAndAttachInstructions(instructions);
        }

		protected virtual bool ShouldInlineExpressions(BinaryExpression thenAssignExpression, BinaryExpression elseAssignExpression)
		{
			/// For code readability reasons the nesting of ternary expressions is not allowed.
			bool result = thenAssignExpression.Right.CodeNodeType != CodeNodeType.ConditionExpression &&
				   elseAssignExpression.Right.CodeNodeType != CodeNodeType.ConditionExpression;
			if (!result)
			{
				return false;
			}
			if (!thenAssignExpression.Right.HasType || !elseAssignExpression.Right.HasType)
			{
				return false;
			}
			/// This condition can be further precised.
			/// For more information on concrete limitations of the conditional operator, check ECMA-334 standart, section 14.13
			return thenAssignExpression.Right.ExpressionType.FullName == elseAssignExpression.Right.ExpressionType.FullName;
		}

		public bool TryMatch(StatementCollection statements,out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 1;
			for (startIndex = 0; startIndex < statements.Count; startIndex++)
			{
				if (statements[startIndex].CodeNodeType != CodeNodeType.IfStatement)
				{
					continue;
				}

                if (TryMatchInternal(statements[startIndex] as IfStatement, out result))
				{
					return true;
				}
			}
			return false;
		}
	}
}
