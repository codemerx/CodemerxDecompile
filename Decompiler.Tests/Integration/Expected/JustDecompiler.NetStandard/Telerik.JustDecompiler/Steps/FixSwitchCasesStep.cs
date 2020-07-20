using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixSwitchCasesStep
	{
		public FixSwitchCasesStep()
		{
			base();
			return;
		}

		public void FixCases(SwitchStatement theSwitch)
		{
			V_0 = new FixSwitchCasesStep.u003cu003ec__DisplayClass0_0();
			V_1 = theSwitch.get_Cases().LastOrDefault<SwitchCase>() as DefaultCase;
			V_2 = 0;
			V_3 = null;
			if (V_1 != null && !this.TryGetSimpleCaseStatementType(V_1, out V_2, out V_3))
			{
				return;
			}
			V_0.casesToRemove = new HashSet<SwitchCase>();
			V_4 = new List<SwitchCase>(theSwitch.get_Cases());
			V_5 = V_4.GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					if (V_6 != V_1)
					{
						if (V_6.get_Body() != null)
						{
							if (this.TryGetSimpleCaseStatementType(V_6, out V_7, out V_8))
							{
								if (V_1 == null)
								{
									if (V_7 == 2)
									{
										dummyVar2 = V_0.casesToRemove.Add(V_6);
										goto Label0;
									}
								}
								else
								{
									if (V_2 == V_7 && String.op_Equality(V_3, V_8))
									{
										dummyVar1 = V_0.casesToRemove.Add(V_6);
										goto Label0;
									}
								}
							}
							V_0.casesToRemove.Clear();
						}
						else
						{
							dummyVar0 = V_0.casesToRemove.Add(V_6);
						}
					}
					else
					{
						goto Label0;
					}
				}
			}
			finally
			{
				((IDisposable)V_5).Dispose();
			}
		Label0:
			if (V_0.casesToRemove.get_Count() > 0)
			{
				theSwitch.set_Cases(V_4.Where<SwitchCase>(new Func<SwitchCase, bool>(V_0.u003cFixCasesu003eb__0)));
			}
			return;
		}

		private bool TryGetSimpleCaseStatementType(SwitchCase theCase, out FixSwitchCasesStep.StatementType statementType, out string gotoLabel)
		{
			gotoLabel = null;
			statementType = 0;
			if (theCase.get_Body().get_Statements().get_Count() != 1 || !String.IsNullOrEmpty(theCase.get_Body().get_Statements().get_Item(0).get_Label()))
			{
				return false;
			}
			V_0 = theCase.get_Body().get_Statements().get_Item(0);
			if (V_0 as GotoStatement == null)
			{
				if (V_0 as BreakStatement == null)
				{
					if (V_0 as ContinueStatement != null)
					{
						statementType = 3;
					}
				}
				else
				{
					statementType = 2;
				}
			}
			else
			{
				statementType = 1;
				gotoLabel = (V_0 as GotoStatement).get_TargetLabel();
			}
			return (int)statementType != 0;
		}

		private enum StatementType
		{
			None,
			Goto,
			Break,
			Continue
		}
	}
}