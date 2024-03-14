using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Common
{
    internal static class SwitchHelpers
    {
        /// <summary>
        /// Checks if after the execution of the statements in <paramref name="caseBody"/> the control flow will be transfered to the next statement.
        /// </summary>
        /// <param name="caseBody">The block to be checked.</param>
        /// <returns>Returns false if all code paths in the block exit at a point, different from the follow node (possibly even out of the method via
        /// throw ot return). Otherwise returns true.</returns>
        public static bool BlockHasFallThroughSemantics(BlockStatement caseBody)
        {
            if (caseBody == null)
            {
                return false;
            }

            if (caseBody.Statements.Count == 0)
            {
                return true;
            }

            var lastStatement = caseBody.Statements[caseBody.Statements.Count - 1];
            if (lastStatement.CodeNodeType == CodeNodeType.ExpressionStatement)
            {
                Expression expression = (lastStatement as ExpressionStatement).Expression;
                if (expression != null && expression.CodeNodeType == CodeNodeType.ReturnExpression || expression.CodeNodeType == CodeNodeType.ThrowExpression)
                {
                    return false;
                }
            }
            else if (lastStatement.CodeNodeType == CodeNodeType.BreakStatement ||
                     lastStatement.CodeNodeType == CodeNodeType.ContinueStatement ||
                     lastStatement.CodeNodeType == CodeNodeType.GotoStatement)
            {
                return false;
            }
            else if (lastStatement.CodeNodeType == CodeNodeType.IfStatement)
            {
                IfStatement theIf = lastStatement as IfStatement;
                if (theIf.Else != null)
                {
                    return BlockHasFallThroughSemantics(theIf.Else) || BlockHasFallThroughSemantics(theIf.Then);
                }
            }
            else if (lastStatement.CodeNodeType == CodeNodeType.IfElseIfStatement)
            {
                IfElseIfStatement theIfElseIf = lastStatement as IfElseIfStatement;
                if (theIfElseIf.Else == null)
                {
                    return true;
                }
                bool result = BlockHasFallThroughSemantics(theIfElseIf.Else);
                if (result == false)
                {
                    return false;
                }
                foreach (KeyValuePair<Expression, BlockStatement> pair in theIfElseIf.ConditionBlocks)
                {
                    result |= BlockHasFallThroughSemantics(pair.Value);
                    if (result == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
