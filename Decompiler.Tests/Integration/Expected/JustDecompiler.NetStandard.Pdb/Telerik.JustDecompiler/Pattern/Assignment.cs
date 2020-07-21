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
			base();
			return;
		}

		protected override bool OnMatch(MatchContext context, BinaryExpression node)
		{
			if (!this.get_Target().TryMatch(context, node.get_Left()))
			{
				return false;
			}
			return this.get_Expression().TryMatch(context, node.get_Right());
		}
	}
}