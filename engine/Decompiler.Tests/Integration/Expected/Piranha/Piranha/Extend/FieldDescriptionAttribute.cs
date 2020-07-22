using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FieldDescriptionAttribute : Attribute
	{
		public string Text
		{
			get;
			set;
		}

		public FieldDescriptionAttribute()
		{
			base();
			return;
		}

		public FieldDescriptionAttribute(string text)
		{
			base();
			this.set_Text(text);
			return;
		}
	}
}