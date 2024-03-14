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
		}

		protected override bool OnMatch(MatchContext context, VariableReferenceExpression node)
		{
			object obj;
			if (!context.TryGetData(this.Name, out obj))
			{
				return false;
			}
			return node.Variable == obj;
		}
	}
}