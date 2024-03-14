using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Pattern
{
	public class ExpressionStatement : CodePattern<Telerik.JustDecompiler.Ast.Statements.ExpressionStatement>
	{
		public ICodePattern Expression
		{
			get;
			set;
		}

		public ExpressionStatement()
		{
		}

		protected override bool OnMatch(MatchContext context, Telerik.JustDecompiler.Ast.Statements.ExpressionStatement node)
		{
			return this.Expression.TryMatch(context, node.Expression);
		}
	}
}