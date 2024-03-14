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
		}

		public FieldDescriptionAttribute(string text)
		{
			this.Text = text;
		}
	}
}