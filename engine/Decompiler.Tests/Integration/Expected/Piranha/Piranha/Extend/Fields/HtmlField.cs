using Piranha.Extend;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Html", Shorthand="Html", Component="html-field")]
	public class HtmlField : SimpleField<string>, ISearchable
	{
		public HtmlField()
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
			return this.get_Value();
		}

		public override string GetTitle()
		{
			if (this.get_Value() == null)
			{
				return null;
			}
			V_0 = Regex.Replace(this.get_Value(), "<[^>]*>", "");
			if (V_0.get_Length() > 40)
			{
				V_0 = String.Concat(V_0.Substring(0, 40), "...");
			}
			return V_0;
		}

		public static implicit operator HtmlField(string str)
		{
			stackVariable0 = new HtmlField();
			stackVariable0.set_Value(str);
			return stackVariable0;
		}

		public static implicit operator String(HtmlField field)
		{
			return field.get_Value();
		}
	}
}