using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.TestCaseWriters;

namespace Telerik.JustDecompiler.Ast
{
	public static class StatementExtensions
	{
		public static Statement GetNextStatement(this Statement self)
		{
			BlockStatement parent = self.Parent as BlockStatement;
			if (parent == null)
			{
				throw new Exception("Unable to get next statement.");
			}
			int num = parent.Statements.IndexOf(self);
			if (num < 0 || num >= parent.Statements.Count - 1)
			{
				return null;
			}
			return parent.Statements[num + 1];
		}

		public static bool IsAssignmentStatement(this Statement statement)
		{
			if (statement.CodeNodeType != CodeNodeType.ExpressionStatement || (statement as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			return ((statement as ExpressionStatement).Expression as BinaryExpression).IsAssignmentExpression;
		}

		public static string ToCodeString(this Statement statement)
		{
			string str;
			using (StringWriter stringWriter = new StringWriter())
			{
				((ILanguageTestCaseWriter)(new IntermediateDecompilationCSharpLanguageWriter(new PlainTextFormatter(stringWriter)))).Write(statement);
				str = stringWriter.ToString();
			}
			return str;
		}
	}
}