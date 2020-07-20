using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Pattern
{
	public abstract class CodePattern<TNode> : CodePattern
	where TNode : class, ICodeNode
	{
		public Func<TNode, MatchData> Bind
		{
			get;
			set;
		}

		protected CodePattern()
		{
			base();
			return;
		}

		public override bool Match(MatchContext context, object node)
		{
			V_0 = (TNode)(node as TNode);
			if (V_0 == null)
			{
				return false;
			}
			if (this.get_Bind() != null)
			{
				context.AddData(this.get_Bind().Invoke(V_0));
			}
			return this.OnMatch(context, V_0);
		}

		protected abstract bool OnMatch(MatchContext context, TNode node);
	}
}