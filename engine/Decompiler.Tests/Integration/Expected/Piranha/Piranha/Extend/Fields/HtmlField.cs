using Piranha.Extend;
using System;
using System.Text.RegularExpressions;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Html", Shorthand="Html", Component="html-field")]
	public class HtmlField : SimpleField<string>, ISearchable
	{
		public HtmlField()
		{
		}

		public string GetIndexedContent()
		{
			if (String.IsNullOrEmpty(base.Value))
			{
				return "";
			}
			return base.Value;
		}

		public override string GetTitle()
		{
			if (base.Value == null)
			{
				return null;
			}
			string str = Regex.Replace(base.Value, "<[^>]*>", "");
			if (str.Length > 40)
			{
				str = String.Concat(str.Substring(0, 40), "...");
			}
			return str;
		}

		public static implicit operator HtmlField(string str)
		{
			return new HtmlField()
			{
				Value = str
			};
		}

		public static implicit operator String(HtmlField field)
		{
			return field.Value;
		}
	}
}