using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Content", Category="Content", Icon="fas fa-paragraph", Component="html-block")]
	public class HtmlBlock : Block, ISearchable
	{
		public HtmlField Body
		{
			get;
			set;
		}

		public HtmlBlock()
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
			HtmlField body = this.Body;
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
			string str = Regex.Replace(this.Body.Value, "<[^>]*>", "");
			if (str.Length > 40)
			{
				str = String.Concat(str.Substring(0, 40), "...");
			}
			return str;
		}
	}
}