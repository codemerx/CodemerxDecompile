using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Pattern
{
	public class Binary : CodePattern<BinaryExpression>
	{
		public bool? IsChecked
		{
			get;
			set;
		}

		public ICodePattern Left
		{
			get;
			set;
		}

		public ICodePattern Operator
		{
			get;
			set;
		}

		public ICodePattern Right
		{
			get;
			set;
		}

		public Binary()
		{
		}

		protected override bool OnMatch(MatchContext context, BinaryExpression node)
		{
			if (!this.Left.TryMatch(context, node.Left))
			{
				return false;
			}
			if (!this.Operator.TryMatch(context, node.Operator))
			{
				return false;
			}
			if (!this.Right.TryMatch(context, node.Right))
			{
				return false;
			}
			if (!this.IsChecked.HasValue)
			{
				return true;
			}
			return this.IsChecked.Value == node.IsChecked;
		}
	}
}