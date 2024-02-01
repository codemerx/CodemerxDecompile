using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Two Cols", Category="Content", Icon="fab fa-html5", Component="html-column-block", IsUnlisted=true)]
	public class HtmlColumnBlock : Block, ISearchable
	{
		public HtmlField Column1
		{
			get;
			set;
		}

		public HtmlField Column2
		{
			get;
			set;
		}

		public HtmlColumnBlock()
		{
		}

		public string GetIndexedContent()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!String.IsNullOrEmpty(this.Column1.Value))
			{
				stringBuilder.AppendLine(this.Column1.Value);
			}
			if (!String.IsNullOrEmpty(this.Column2.Value))
			{
				stringBuilder.AppendLine(this.Column2.Value);
			}
			return stringBuilder.ToString();
		}
	}
}