using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class DynamicSiteContent : SiteContent<DynamicSiteContent>, IDynamicContent
	{
		public dynamic Regions { get; set; } = new ExpandoObject();

		public DynamicSiteContent()
		{
		}
	}
}