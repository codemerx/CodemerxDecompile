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
					this.body.set_Parent(this);
				}
				return;
			}
		}

		public SwitchCase()
		{
			base();
			return;
		}

		public SwitchCase(BlockStatement body)
		{
			base();
			this.set_Body(body);
			return;
		}
	}
}