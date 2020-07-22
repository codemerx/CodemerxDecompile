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

		public string Icon
		{
			get;
			set;
		}

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

		public int SortOrder
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public RegionAttribute()
		{
			this.u003cSortOrderu003ek__BackingField = 0x7fffffff;
			this.u003cIconu003ek__BackingField = "fas fa-table";
			base();
			return;
		}
	}
}