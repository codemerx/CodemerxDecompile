using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Pattern
{
	public class Assignment : CodePattern<BinaryExpression>
	{
		public ICodePattern Expression
		{
			get;
			set;
		}

		public ICodePattern Target
		{
			get;
			set;
		}

		public Assignment()
		{
		}

		protected override bool OnMatch(MatchContext context, BinaryExpression node)
		{
			if (!this.Target.TryMatch(context, node.Left))
			{
				return false;
			}
			return this.Expression.TryMatch(context, node.Right);
		}
	}
}