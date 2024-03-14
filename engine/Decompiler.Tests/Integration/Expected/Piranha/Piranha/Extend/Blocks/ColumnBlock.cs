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
		}

		public string GetIndexedContent()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Block item in base.Items)
			{
				ISearchable searchable = item as ISearchable;
				if (searchable == null)
				{
					continue;
				}
				string indexedContent = searchable.GetIndexedContent();
				if (String.IsNullOrEmpty(indexedContent))
				{
					continue;
				}
				stringBuilder.AppendLine(indexedContent);
			}
			return stringBuilder.ToString();
		}
	}
}