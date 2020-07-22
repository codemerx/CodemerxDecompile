using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Pattern
{
	public class Literal : CodePattern<LiteralExpression>
	{
		private object value;

		private bool check_value;

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
				this.check_value = true;
				return;
			}
		}

		public Literal()
		{
			base();
			return;
		}

		protected override bool OnMatch(MatchContext context, LiteralExpression node)
		{
			if (!this.check_value)
			{
				return true;
			}
			return Object.Equals(this.value, node.get_Value());
		}
	}
}