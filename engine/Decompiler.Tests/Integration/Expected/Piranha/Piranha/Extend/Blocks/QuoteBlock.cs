using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Quote", Category="Content", Icon="fas fa-quote-right", Component="quote-block")]
	public class QuoteBlock : Block, ISearchable
	{
		public StringField Author
		{
			get;
			set;
		}

		public TextField Body
		{
			get;
			set;
		}

		public QuoteBlock()
		{
			base();
			return;
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(this.get_Body().get_Value()))
			{
				return "";
			}
			return this.get_Body().get_Value();
		}

		public override string GetTitle()
		{
			stackVariable1 = this.get_Body();
			if (stackVariable1 != null)
			{
				stackVariable2 = stackVariable1.get_Value();
			}
			else
			{
				dummyVar0 = stackVariable1;
				stackVariable2 = false;
			}
			if (!stackVariable2)
			{
				return "Empty";
			}
			return this.get_Body().get_Value();
		}
	}
}