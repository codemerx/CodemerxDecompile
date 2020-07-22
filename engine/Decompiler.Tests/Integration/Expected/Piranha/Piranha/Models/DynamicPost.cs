using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class DynamicPost : Post<DynamicPost>, IDynamicContent
	{
		public dynamic Regions
		{
			get;
			set;
		}

		public DynamicPost()
		{
			this.u003cRegionsu003ek__BackingField = new ExpandoObject();
			base();
			return;
		}
	}
}