using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	interface ICodePattern
	{
		bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount);
	}
}
