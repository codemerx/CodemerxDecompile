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
			base();
			return;
		}

		public string GetIndexedContent()
		{
			V_0 = new StringBuilder();
			if (!String.IsNullOrEmpty(this.get_Column1().get_Value()))
			{
				dummyVar0 = V_0.AppendLine(this.get_Column1().get_Value());
			}
			if (!String.IsNullOrEmpty(this.get_Column2().get_Value()))
			{
				dummyVar1 = V_0.AppendLine(this.get_Column2().get_Value());
			}
			return V_0.ToString();
		}
	}
}