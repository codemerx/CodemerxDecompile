using Piranha;
using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Markdown", Shorthand="Markdown", Component="markdown-field")]
	public class MarkdownField : SimpleField<string>, ISearchable
	{
		public MarkdownField()
		{
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(base.Value))
			{
				return "";
			}
			return this.ToHtml();
		}

		public static implicit operator MarkdownField(string str)
		{
			return new MarkdownField()
			{
				Value = str
			};
		}

		public static implicit operator String(MarkdownField field)
		{
			return field.ToHtml();
		}

		public string ToHtml()
		{
			return App.Markdown.Transform(base.Value);
		}
	}
}