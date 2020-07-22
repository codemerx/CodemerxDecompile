using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
    class FixSwitchCasesStep
    {
        public void FixCases(SwitchStatement theSwitch)
        {
            DefaultCase defaultCase = theSwitch.Cases.LastOrDefault() as DefaultCase;
            StatementType defaultCaseStatementType = StatementType.None;
            string defaultCaseGotoLabel = null;
            if (defaultCase != null && !TryGetSimpleCaseStatementType(defaultCase, out defaultCaseStatementType, out defaultCaseGotoLabel))
            {
                return;
            }

            HashSet<SwitchCase> casesToRemove = new HashSet<SwitchCase>();
            List<SwitchCase> allCases = new List<SwitchCase>(theSwitch.Cases);
            foreach (SwitchCase @case in allCases)
            {
                if (@case == defaultCase)
                {
                    break;
                }

                if (@case.Body == null)
                {
                    casesToRemove.Add(@case);
                    continue;
                }

                StatementType caseStatementType;
                string caseGotoLabel;
                if (TryGetSimpleCaseStatementType(@case, out caseStatementType, out caseGotoLabel))
                {
                    if (defaultCase != null)
                    {
                        if (defaultCaseStatementType == caseStatementType && defaultCaseGotoLabel == caseGotoLabel)
                        {
                            casesToRemove.Add(@case);
                            break;
                        }
                    }
                    else
                    {
                        if (caseStatementType == StatementType.Break)
                        {
                            casesToRemove.Add(@case);
                            break;
                        }
                    }
                }

                casesToRemove.Clear();
            }

            if (casesToRemove.Count > 0)
            {
                theSwitch.Cases = allCases.Where(@case => !casesToRemove.Contains(@case));
            }
        }

        private bool TryGetSimpleCaseStatementType(SwitchCase theCase, out StatementType statementType, out string gotoLabel)
        {
            gotoLabel = null;
            statementType = StatementType.None;
            if (theCase.Body.Statements.Count != 1 || !string.IsNullOrEmpty(theCase.Body.Statements[0].Label))
            {
                return false;
            }

            Statement statement = theCase.Body.Statements[0];
            if (statement is GotoStatement)
            {
                statementType = StatementType.Goto;
                gotoLabel = (statement as GotoStatement).TargetLabel;
            }
            else if (statement is BreakStatement)
            {
                statementType = StatementType.Break;
            }
            else if (statement is ContinueStatement)
            {
                statementType = StatementType.Continue;
            }

            return statementType != StatementType.None;
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
