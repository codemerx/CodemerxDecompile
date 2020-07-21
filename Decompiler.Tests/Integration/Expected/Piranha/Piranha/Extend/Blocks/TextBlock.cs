using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Blocks
{
	[BlockType(Name="Text", Category="Content", Icon="fas fa-font", Component="text-block")]
	public class TextBlock : Block, ISearchable
	{
		public TextField Body
		{
			get;
			set;
		}

		public TextBlock()
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