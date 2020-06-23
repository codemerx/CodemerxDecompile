using System;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Steps
{
	internal abstract class Matcher : BaseCodeVisitor
	{
		private bool @continue = true;

		private bool match;

		public bool Continue
		{
			get
			{
				return this.@continue;
			}
			set
			{
				this.@continue = value;
			}
		}

		public bool Match
		{
			get
			{
				return this.match;
			}
			set
			{
				this.match = value;
			}
		}

		protected Matcher()
		{
		}

		public override void Visit(ICodeNode node)
		{
			if (this.@continue)
			{
				base.Visit(node);
			}
		}
	}
}