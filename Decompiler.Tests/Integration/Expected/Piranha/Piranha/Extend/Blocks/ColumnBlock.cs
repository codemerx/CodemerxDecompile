using Piranha.Extend;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Piranha.Extend.Blocks
{
	[BlockGroupType(Name="Columns", Category="Content", Icon="fas fa-columns", Display=BlockDisplayMode.Horizontal)]
	public class ColumnBlock : BlockGroup, ISearchable
	{
		public ColumnBlock()
		{
			base();
			return;
		}

		public string GetIndexedContent()
		{
			V_0 = new StringBuilder();
			V_1 = this.get_Items().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current() as ISearchable;
					if (V_2 == null)
					{
						continue;
					}
					V_3 = V_2.GetIndexedContent();
					if (String.IsNullOrEmpty(V_3))
					{
						continue;
					}
					dummyVar0 = V_0.AppendLine(V_3);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0.ToString();
		}
	}
}