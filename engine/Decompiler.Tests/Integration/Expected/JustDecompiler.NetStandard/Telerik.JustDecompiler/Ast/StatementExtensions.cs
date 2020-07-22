using System;
using System.IO;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public static class StatementExtensions
	{
		public static Statement GetNextStatement(this Statement self)
		{
			V_0 = self.get_Parent() as BlockStatement;
			if (V_0 == null)
			{
				throw new Exception("Unable to get next statement.");
			}
			V_1 = V_0.get_Statements().IndexOf(self);
			if (V_1 < 0 || V_1 >= V_0.get_Statements().get_Count() - 1)
			{
				return null;
			}
			return V_0.get_Statements().get_Item(V_1 + 1);
		}

		public static bool IsAssignmentStatement(this Statement statement)
		{
			if (statement.get_CodeNodeType() != 5 || (statement as ExpressionStatement).get_Expression().get_CodeNodeType() != 24)
			{
				return false;
			}
			return ((statement as ExpressionStatement).get_Expression() as BinaryExpression).get_IsAssignmentExpression();
		}

		public static string ToCodeString(this Statement statement)
		{
			V_0 = new StringWriter();
			try
			{
				((ILanguageTestCaseWriter)(new IntermediateDecompilationCSharpLanguageWriter(new PlainTextFormatter(V_0)))).Write(statement);
				V_1 = V_0.ToString();
			}
			finally
			{
				if (V_0 != null)
				{
					((IDisposable)V_0).Dispose();
				}
			}
			return V_1;
		}
	}
}