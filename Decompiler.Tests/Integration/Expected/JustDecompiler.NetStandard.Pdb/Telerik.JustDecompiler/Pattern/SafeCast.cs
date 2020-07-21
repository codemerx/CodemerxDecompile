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
			base();
			return;
		}

		protected override bool OnMatch(MatchContext context, SafeCastExpression node)
		{
			if (!this.get_TargetType().TryMatch(context, node.get_TargetType()))
			{
				return false;
			}
			return this.get_Expression().TryMatch(context, node.get_Expression());
		}
	}
}