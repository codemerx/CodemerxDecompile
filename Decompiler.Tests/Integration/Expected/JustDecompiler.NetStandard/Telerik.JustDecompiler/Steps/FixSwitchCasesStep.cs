using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixSwitchCasesStep
	{
		public FixSwitchCasesStep()
		{
		}

		public void FixCases(SwitchStatement theSwitch)
		{
			FixSwitchCasesStep.StatementType statementType;
			string str;
			DefaultCase defaultCase = theSwitch.Cases.LastOrDefault<SwitchCase>() as DefaultCase;
			FixSwitchCasesStep.StatementType statementType1 = FixSwitchCasesStep.StatementType.None;
			string str1 = null;
			if (defaultCase != null && !this.TryGetSimpleCaseStatementType(defaultCase, out statementType1, out str1))
			{
				return;
			}
			HashSet<SwitchCase> switchCases = new HashSet<SwitchCase>();
			List<SwitchCase> switchCases1 = new List<SwitchCase>(theSwitch.Cases);
			foreach (SwitchCase switchCase in switchCases1)
			{
				if (switchCase == defaultCase)
				{
					if (switchCases.Count > 0)
					{
						theSwitch.Cases = 
							from case in switchCases1
							where !switchCases.Contains(case)
							select case;
					}
					return;
				}
				else if (switchCase.Body != null)
				{
					if (this.TryGetSimpleCaseStatementType(switchCase, out statementType, out str))
					{
						if (defaultCase != null)
						{
							if (statementType1 == statementType && str1 == str)
							{
								switchCases.Add(switchCase);
								if (switchCases.Count > 0)
								{
									theSwitch.Cases = 
										from case in switchCases1
										where !switchCases.Contains(case)
										select case;
								}
								return;
							}
						}
						else if (statementType == FixSwitchCasesStep.StatementType.Break)
						{
							switchCases.Add(switchCase);
							if (switchCases.Count > 0)
							{
								theSwitch.Cases = 
									from case in switchCases1
									where !switchCases.Contains(case)
									select case;
							}
							return;
						}
					}
					switchCases.Clear();
				}
				else
				{
					switchCases.Add(switchCase);
				}
			}
			if (switchCases.Count > 0)
			{
				theSwitch.Cases = 
					from case in switchCases1
					where !switchCases.Contains(case)
					select case;
			}
		}

		private bool TryGetSimpleCaseStatementType(SwitchCase theCase, out FixSwitchCasesStep.StatementType statementType, out string gotoLabel)
		{
			gotoLabel = null;
			statementType = FixSwitchCasesStep.StatementType.None;
			if (theCase.Body.Statements.Count != 1 || !String.IsNullOrEmpty(theCase.Body.Statements[0].Label))
			{
				return false;
			}
			Statement item = theCase.Body.Statements[0];
			if (item is GotoStatement)
			{
				statementType = FixSwitchCasesStep.StatementType.Goto;
				gotoLabel = (item as GotoStatement).TargetLabel;
			}
			else if (item is BreakStatement)
			{
				statementType = FixSwitchCasesStep.StatementType.Break;
			}
			else if (item is ContinueStatement)
			{
				statementType = FixSwitchCasesStep.StatementType.Continue;
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