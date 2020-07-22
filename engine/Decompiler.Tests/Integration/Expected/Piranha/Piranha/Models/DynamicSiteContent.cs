using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class DynamicSiteContent : SiteContent<DynamicSiteContent>, IDynamicContent
	{
		public dynamic Regions
		{
			get;
			set;
		}

		public DynamicSiteContent()
		{
			this.u003cRegionsu003ek__BackingField = new ExpandoObject();
			base();
			return;
		}
	}
}