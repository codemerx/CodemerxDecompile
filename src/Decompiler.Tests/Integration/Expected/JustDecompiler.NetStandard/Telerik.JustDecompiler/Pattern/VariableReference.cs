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
		}

		protected override bool OnMatch(MatchContext context, VariableReferenceExpression node)
		{
			return this.Variable.TryMatch(context, node.Variable);
		}
	}
}