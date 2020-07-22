using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Markdown", Shorthand="Markdown", Component="markdown-field")]
	public class MarkdownField : SimpleField<string>, ISearchable
	{
		public MarkdownField()
		{
			base();
			return;
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(this.get_Value()))
			{
				return "";
			}
			return this.ToHtml();
		}

		public static implicit operator MarkdownField(string str)
		{
			stackVariable0 = new MarkdownField();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public static implicit operator String(MarkdownField field)
		{
			return field.ToHtml();
		}

		public string ToHtml()
		{
			return App.get_Markdown().Transform(this.get_Value());
		}
	}
}