using System;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Steps
{
	internal abstract class Matcher : BaseCodeVisitor
	{
		private bool continue;

		private bool match;

		public bool Continue
		{
			get
			{
				return this.continue;
			}
			set
			{
				this.continue = value;
				return;
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
				return;
			}
		}

		protected Matcher()
		{
			this.continue = true;
			base();
			return;
		}

		public override void Visit(ICodeNode node)
		{
			if (this.continue)
			{
				this.Visit(node);
			}
			return;
		}
	}
}