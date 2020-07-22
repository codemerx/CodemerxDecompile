using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Pattern
{
	public class VariableReference : CodePattern<VariableReferenceExpression>
	{
		public ICodePattern Variable
		{
			get;
			set;
		}

		public VariableReference()
		{
			base();
			return;
		}

		protected override bool OnMatch(MatchContext context, VariableReferenceExpression node)
		{
			return this.get_Variable().TryMatch(context, node.get_Variable());
		}
	}
}