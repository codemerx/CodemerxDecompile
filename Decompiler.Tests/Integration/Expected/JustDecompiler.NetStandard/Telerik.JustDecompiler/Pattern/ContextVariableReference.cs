using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Pattern
{
	public class ContextVariableReference : CodePattern<VariableReferenceExpression>
	{
		public string Name
		{
			get;
			set;
		}

		public ContextVariableReference()
		{
			base();
			return;
		}

		protected override bool OnMatch(MatchContext context, VariableReferenceExpression node)
		{
			if (!context.TryGetData(this.get_Name(), out V_0))
			{
				return false;
			}
			return node.get_Variable() == V_0;
		}
	}
}