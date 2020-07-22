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
			base();
			return;
		}

		public RegionDescriptionAttribute(string text)
		{
			base();
			this.set_Text(text);
			return;
		}
	}
}