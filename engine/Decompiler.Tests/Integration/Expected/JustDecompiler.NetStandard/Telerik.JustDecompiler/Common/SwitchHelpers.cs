using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Common
{
	internal static class SwitchHelpers
	{
		public static bool BlockHasFallThroughSemantics(BlockStatement caseBody)
		{
			if (caseBody == null)
			{
				return false;
			}
			if (caseBody.get_Statements().get_Count() == 0)
			{
				return true;
			}
			V_0 = caseBody.get_Statements().get_Item(caseBody.get_Statements().get_Count() - 1);
			if (V_0.get_CodeNodeType() != 5)
			{
				if (V_0.get_CodeNodeType() == 9 || V_0.get_CodeNodeType() == 10 || V_0.get_CodeNodeType() == 2)
				{
					return false;
				}
				if (V_0.get_CodeNodeType() != 3)
				{
					if (V_0.get_CodeNodeType() == 4)
					{
						V_3 = V_0 as IfElseIfStatement;
						if (V_3.get_Else() == null)
						{
							return true;
						}
						V_4 = SwitchHelpers.BlockHasFallThroughSemantics(V_3.get_Else());
						if (!V_4)
						{
							return false;
						}
						V_5 = V_3.get_ConditionBlocks().GetEnumerator();
						try
						{
							while (V_5.MoveNext())
							{
								V_6 = V_5.get_Current();
								V_4 = V_4 | SwitchHelpers.BlockHasFallThroughSemantics(V_6.get_Value());
								if (V_4)
								{
									continue;
								}
								V_7 = false;
								goto Label1;
							}
							goto Label0;
						}
						finally
						{
							((IDisposable)V_5).Dispose();
						}
					Label1:
						return V_7;
					}
				}
				else
				{
					V_2 = V_0 as IfStatement;
					if (V_2.get_Else() != null)
					{
						if (SwitchHelpers.BlockHasFallThroughSemantics(V_2.get_Else()))
						{
							return true;
						}
						return SwitchHelpers.BlockHasFallThroughSemantics(V_2.get_Then());
					}
				}
			}
			else
			{
				V_1 = (V_0 as ExpressionStatement).get_Expression();
				if (V_1 != null && V_1.get_CodeNodeType() == 57 || V_1.get_CodeNodeType() == 6)
				{
					return false;
				}
			}
		Label0:
			return true;
		}
	}
}