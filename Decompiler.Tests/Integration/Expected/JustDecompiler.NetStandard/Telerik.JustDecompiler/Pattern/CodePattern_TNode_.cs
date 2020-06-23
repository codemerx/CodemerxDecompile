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
		}

		public override bool Match(MatchContext context, object node)
		{
			TNode tNode = (TNode)(node as TNode);
			if (tNode == null)
			{
				return false;
			}
			if (this.Bind != null)
			{
				context.AddData(this.Bind(tNode));
			}
			return this.OnMatch(context, tNode);
		}

		protected abstract bool OnMatch(MatchContext context, TNode node);
	}
}