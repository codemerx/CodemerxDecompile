using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class NullCoalescingPattern : CommonPatterns, ICodePattern
	{
		private readonly MethodSpecificContext methodContext;

		public NullCoalescingPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext)
		{
			base(patternsContext, methodContext.get_Method().get_Module().get_TypeSystem());
			this.methodContext = methodContext;
			return;
		}

		private bool ContainsDummyAssignment(BlockStatement theThen, VariableReference xVariableReference)
		{
			if (theThen.get_Statements().get_Count() == 0 || theThen.get_Statements().get_Item(0).get_CodeNodeType() != 5 || !String.IsNullOrEmpty(theThen.get_Statements().get_Item(0).get_Label()))
			{
				return false;
			}
			V_0 = (theThen.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0 == null || V_0.get_Left().get_CodeNodeType() != 26 || V_0.get_Right().get_CodeNodeType() != 26 || (object)(V_0.get_Right() as VariableReferenceExpression).get_Variable() != (object)xVariableReference)
			{
				return false;
			}
			if (!this.methodContext.get_StackData().get_VariableToDefineUseInfo().TryGetValue((V_0.get_Left() as VariableReferenceExpression).get_Variable().Resolve(), out V_1))
			{
				return false;
			}
			return V_1.get_UsedAt().get_Count() == 0;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 2;
			startIndex = 0;
			while (startIndex + 1 < statements.get_Count())
			{
				if (this.TryMatchInternal(statements, startIndex, out result))
				{
					return true;
				}
				startIndex = startIndex + 1;
			}
			return false;
		}

		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result)
		{
			result = null;
			if (statements.get_Count() < startIndex + 2)
			{
				return false;
			}
			if (statements.get_Item(startIndex).get_CodeNodeType() != 5 || statements.get_Item(startIndex + 1).get_CodeNodeType() != 3)
			{
				return false;
			}
			if (!String.IsNullOrEmpty(statements.get_Item(startIndex + 1).get_Label()))
			{
				return false;
			}
			V_3 = (statements.get_Item(startIndex) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (!this.IsAssignToVariableExpression(V_3, out V_0))
			{
				return false;
			}
			V_1 = V_3.get_Right();
			V_4 = statements.get_Item(startIndex + 1) as IfStatement;
			if (this.ContainsDummyAssignment(V_4.get_Then(), V_0))
			{
				stackVariable50 = 1;
			}
			else
			{
				stackVariable50 = 0;
			}
			V_5 = stackVariable50;
			if (V_4.get_Else() != null || V_4.get_Then().get_Statements().get_Count() != 1 + V_5 || V_4.get_Then().get_Statements().get_Item(V_5).get_CodeNodeType() != 5 || !String.IsNullOrEmpty(V_4.get_Then().get_Statements().get_Item(V_5).get_Label()))
			{
				return false;
			}
			V_6 = V_4.get_Condition() as BinaryExpression;
			if (V_6 == null || V_6.get_Operator() != 9 || V_6.get_Left().get_CodeNodeType() != 26 || (object)(V_6.get_Left() as VariableReferenceExpression).get_Variable() != (object)V_0 || V_6.get_Right().get_CodeNodeType() != 22 || (V_6.get_Right() as LiteralExpression).get_Value() != null)
			{
				return false;
			}
			V_7 = (V_4.get_Then().get_Statements().get_Item(V_5) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_7 == null || !this.IsAssignToVariableExpression(V_7, out V_8) || (object)V_8 != (object)V_0)
			{
				return false;
			}
			V_2 = V_7.get_Right();
			if (!V_1.get_HasType() || !V_2.get_HasType() || String.op_Inequality(V_1.get_ExpressionType().get_FullName(), V_2.get_ExpressionType().get_FullName()))
			{
				return false;
			}
			V_9 = new BinaryExpression(27, V_1, V_2, this.typeSystem, null, false);
			V_10 = new BinaryExpression(26, new VariableReferenceExpression(V_0, null), V_9, this.typeSystem, null, false);
			stackVariable150 = new ExpressionStatement(V_10);
			stackVariable150.set_Parent(statements.get_Item(startIndex).get_Parent());
			result = stackVariable150;
			this.FixContext(V_0.Resolve(), 1, V_5 + 1, result as ExpressionStatement);
			return true;
		}
	}
}