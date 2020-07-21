using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class RaiseEventPattern : ICodePattern
	{
		public RaiseEventPattern()
		{
			base();
			return;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			startIndex = 0;
			result = null;
			replacedStatementsCount = 0;
			V_0 = 0;
			while (V_0 < statements.get_Count() - 1)
			{
				if (this.TryMatchInternal(statements, V_0, out result))
				{
					startIndex = V_0;
					replacedStatementsCount = 2;
					return true;
				}
				V_0 = V_0 + 1;
			}
			return false;
		}

		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result)
		{
			result = null;
			if (startIndex + 1 >= statements.get_Count())
			{
				return false;
			}
			if (statements.get_Item(startIndex).get_CodeNodeType() != 5 || statements.get_Item(startIndex + 1).get_CodeNodeType() != 3)
			{
				return false;
			}
			V_0 = statements.get_Item(startIndex) as ExpressionStatement;
			if (V_0.get_Expression().get_CodeNodeType() != 24)
			{
				return false;
			}
			V_1 = V_0.get_Expression() as BinaryExpression;
			if (V_1.get_Left().get_CodeNodeType() != 26 || V_1.get_Right().get_CodeNodeType() != 48)
			{
				return false;
			}
			V_2 = V_1.get_Left() as VariableReferenceExpression;
			V_3 = V_1.get_Right() as EventReferenceExpression;
			V_4 = statements.get_Item(startIndex + 1) as IfStatement;
			if (V_4.get_Then() == null || V_4.get_Else() != null || V_4.get_Condition().get_CodeNodeType() != 24)
			{
				return false;
			}
			V_5 = V_4.get_Condition() as BinaryExpression;
			if (V_5.get_Left().get_CodeNodeType() != 26 || V_5.get_Right().get_CodeNodeType() != 22 || V_5.get_Operator() != 10)
			{
				return false;
			}
			V_6 = V_5.get_Left() as VariableReferenceExpression;
			if ((object)V_2.get_Variable() != (object)V_6.get_Variable())
			{
				return false;
			}
			if ((V_5.get_Right() as LiteralExpression).get_Value() != null)
			{
				return false;
			}
			V_7 = V_4.get_Then().get_Statements();
			if (V_7.get_Count() != 1 || V_7.get_Item(0).get_CodeNodeType() != 5)
			{
				return false;
			}
			V_8 = V_7.get_Item(0) as ExpressionStatement;
			if (V_8.get_Expression().get_CodeNodeType() != 51)
			{
				return false;
			}
			V_9 = V_8.get_Expression() as DelegateInvokeExpression;
			if (V_9.get_Target() == null || V_9.get_Target().get_CodeNodeType() != 26)
			{
				return false;
			}
			V_10 = V_9.get_Target() as VariableReferenceExpression;
			if ((object)V_10.get_Variable() != (object)V_2.get_Variable())
			{
				return false;
			}
			V_11 = new List<Instruction>();
			V_11.AddRange(V_0.get_UnderlyingSameMethodInstructions());
			V_11.AddRange(V_5.get_UnderlyingSameMethodInstructions());
			V_11.AddRange(V_9.get_MappedInstructions());
			V_11.AddRange(V_10.get_UnderlyingSameMethodInstructions());
			result = new ExpressionStatement(new RaiseEventExpression(V_3.get_Event(), V_9.get_InvokeMethodReference(), V_9.get_Arguments(), V_11));
			return true;
		}
	}
}