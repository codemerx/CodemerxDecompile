using Piranha.Models;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RegionAttribute : Attribute
	{
		public RegionDisplayMode Display
		{
			get;
			set;
		}

		public string Icon { get; set; } = "fas fa-table";

		public bool ListExpand
		{
			get;
			set;
		}

		public string ListPlaceholder
		{
			get;
			set;
		}

		public string ListTitle
		{
			get;
			set;
		}

		public int SortOrder { get; set; } = 0x7fffffff;

		public string Title
		{
			get;
			set;
		}

		public RegionAttribute()
		{
		}
	}
}