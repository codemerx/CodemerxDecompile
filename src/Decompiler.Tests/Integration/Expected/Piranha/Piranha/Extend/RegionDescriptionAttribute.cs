using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RegionDescriptionAttribute : Attribute
	{
		public string Text
		{
			get;
			set;
		}

		public RegionDescriptionAttribute()
		{
		}

		public RegionDescriptionAttribute(string text)
		{
			this.Text = text;
		}
	}
}