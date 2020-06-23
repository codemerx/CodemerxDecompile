using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Pattern
{
	public class SafeCast : CodePattern<SafeCastExpression>
	{
		public ICodePattern Expression
		{
			get;
			set;
		}

		public ICodePattern TargetType
		{
			get;
			set;
		}

		public SafeCast()
		{
		}

		protected override bool OnMatch(MatchContext context, SafeCastExpression node)
		{
			if (!this.TargetType.TryMatch(context, node.TargetType))
			{
				return false;
			}
			return this.Expression.TryMatch(context, node.Expression);
		}
	}
}