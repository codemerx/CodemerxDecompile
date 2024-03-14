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
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(this.Body.Value))
			{
				return "";
			}
			return this.Body.Value;
		}

		public override string GetTitle()
		{
			bool value;
			TextField body = this.Body;
			if (body != null)
			{
				value = body.Value;
			}
			else
			{
				value = false;
			}
			if (!value)
			{
				return "Empty";
			}
			return this.Body.Value;
		}
	}
}