using System;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public abstract class SwitchCase : Statement
	{
		private BlockStatement body;

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
				if (this.body != null)
				{
					this.body.Parent = this;
				}
			}
		}

		public SwitchCase()
		{
		}

		public SwitchCase(BlockStatement body)
		{
			this.Body = body;
		}
	}
}