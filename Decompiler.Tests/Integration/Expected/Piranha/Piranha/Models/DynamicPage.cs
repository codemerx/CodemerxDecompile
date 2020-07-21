using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class DynamicPage : GenericPage<DynamicPage>, IDynamicContent
	{
		public dynamic Regions
		{
			get;
			set;
		}

		public DynamicPage()
		{
			this.u003cRegionsu003ek__BackingField = new ExpandoObject();
			base();
			return;
		}
	}
}