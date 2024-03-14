using System;
/* AGPL */
using System.Linq;
/* End AGPL */
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast
{
    public static class StatementExtensions
    {
        public static bool IsAssignmentStatement(this Statement statement)
        {
            return statement.CodeNodeType == CodeNodeType.ExpressionStatement &&
                (statement as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.BinaryExpression &&
                ((statement as ExpressionStatement).Expression as BinaryExpression).IsAssignmentExpression;
        }

        public static string ToCodeString(this Statement statement)
        {
            using (System.IO.StringWriter statementDecompilerStrWriter = new System.IO.StringWriter())
            {
                /* AGPL */
                Languages.ILanguageTestCaseWriter statementDecompilerLanguageWriter =
                    new Languages.TestCaseWriters.IntermediateDecompilationCSharpLanguageWriter(new Languages.PlainTextFormatter(statementDecompilerStrWriter), statement.UnderlyingSameMethodInstructions.First().ContainingMethod);
                /* End AGPL */
                statementDecompilerLanguageWriter.Write(statement);
                return statementDecompilerStrWriter.ToString();
            }
        }

        public static Statement GetNextStatement(this Statement self)
        {
            BlockStatement parent = self.Parent as BlockStatement;
            if (parent != null)
            {
                int index = parent.Statements.IndexOf(self);
                if (index >= 0 && index < parent.Statements.Count - 1)
                {
                    return parent.Statements[index + 1];
                }
                return null;
            }

            throw new Exception("Unable to get next statement.");
        }
    }
}
