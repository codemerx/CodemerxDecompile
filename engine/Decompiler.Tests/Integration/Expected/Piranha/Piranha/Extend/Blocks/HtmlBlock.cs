using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

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
			V_0 = Regex.Replace(this.get_Body().get_Value(), "<[^>]*>", "");
			if (V_0.get_Length() > 40)
			{
				V_0 = String.Concat(V_0.Substring(0, 40), "...");
			}
			return V_0;
		}
	}
}