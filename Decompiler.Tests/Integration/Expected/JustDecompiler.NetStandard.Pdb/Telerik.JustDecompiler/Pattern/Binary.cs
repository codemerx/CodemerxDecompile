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
			base();
			return;
		}

		protected override bool OnMatch(MatchContext context, BinaryExpression node)
		{
			if (!this.get_Left().TryMatch(context, node.get_Left()))
			{
				return false;
			}
			if (!this.get_Operator().TryMatch(context, node.get_Operator()))
			{
				return false;
			}
			if (!this.get_Right().TryMatch(context, node.get_Right()))
			{
				return false;
			}
			if (!this.get_IsChecked().get_HasValue())
			{
				return true;
			}
			V_0 = this.get_IsChecked();
			return V_0.get_Value() == node.get_IsChecked();
		}
	}
}