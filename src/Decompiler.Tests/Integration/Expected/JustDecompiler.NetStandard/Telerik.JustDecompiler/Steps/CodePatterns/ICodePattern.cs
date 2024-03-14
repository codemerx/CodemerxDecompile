using System;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal interface ICodePattern
	{
		bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount);
	}
}