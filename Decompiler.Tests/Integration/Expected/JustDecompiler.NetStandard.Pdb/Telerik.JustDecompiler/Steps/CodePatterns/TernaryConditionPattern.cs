using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class TernaryConditionPattern : CommonPatterns, ICodePattern
	{
		public TernaryConditionPattern(CodePatternsContext patternsContext, TypeSystem typeSystem)
		{
			base(patternsContext, typeSystem);
			return;
		}

		private Expression GetRightExpressionMapped(BinaryExpression assignExpression)
		{
			V_0 = new List<Instruction>(assignExpression.get_Left().get_UnderlyingSameMethodInstructions());
			V_0.AddRange(assignExpression.get_MappedInstructions());
			return assignExpression.get_Right().CloneAndAttachInstructions(V_0);
		}

		protected virtual bool ShouldInlineExpressions(BinaryExpression thenAssignExpression, BinaryExpression elseAssignExpression)
		{
			if (thenAssignExpression.get_Right().get_CodeNodeType() == 36)
			{
				stackVariable4 = false;
			}
			else
			{
				stackVariable4 = elseAssignExpression.get_Right().get_CodeNodeType() != 36;
			}
			if (!stackVariable4)
			{
				return false;
			}
			if (!thenAssignExpression.get_Right().get_HasType() || !elseAssignExpression.get_Right().get_HasType())
			{
				return false;
			}
			return String.op_Equality(thenAssignExpression.get_Right().get_ExpressionType().get_FullName(), elseAssignExpression.get_Right().get_ExpressionType().get_FullName());
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 1;
			startIndex = 0;
			while (startIndex < statements.get_Count())
			{
				if (statements.get_Item(startIndex).get_CodeNodeType() == 3 && this.TryMatchInternal(statements.get_Item(startIndex) as IfStatement, out result))
				{
					return true;
				}
				startIndex = startIndex + 1;
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
			V_0 = null;
			V_1 = null;
			if (theIfStatement.get_Else() == null || theIfStatement.get_Then().get_Statements().get_Count() != 1 || theIfStatement.get_Else().get_Statements().get_Count() != 1 || theIfStatement.get_Then().get_Statements().get_Item(0).get_CodeNodeType() != 5 || theIfStatement.get_Else().get_Statements().get_Item(0).get_CodeNodeType() != 5)
			{
				return false;
			}
			V_4 = (theIfStatement.get_Then().get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			V_5 = (theIfStatement.get_Else().get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (!this.IsAssignToVariableExpression(V_4, out V_1) || !this.IsAssignToVariableExpression(V_5, out V_0))
			{
				return false;
			}
			if ((object)V_0 != (object)V_1)
			{
				return false;
			}
			if (!this.ShouldInlineExpressions(V_4, V_5))
			{
				return false;
			}
			V_2 = this.GetRightExpressionMapped(V_4);
			V_3 = this.GetRightExpressionMapped(V_5);
			V_6 = new ConditionExpression(theIfStatement.get_Condition(), V_2, V_3, null);
			V_7 = new BinaryExpression(26, new VariableReferenceExpression(V_0, null), V_6, this.typeSystem, null, false);
			stackVariable87 = new ExpressionStatement(V_7);
			stackVariable87.set_Parent(theIfStatement.get_Parent());
			result = stackVariable87;
			this.FixContext(V_0.Resolve(), 1, 0, result as ExpressionStatement);
			return true;
		}
	}
}