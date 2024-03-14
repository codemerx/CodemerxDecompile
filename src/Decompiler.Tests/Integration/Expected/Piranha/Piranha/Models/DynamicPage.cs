using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class DynamicPage : GenericPage<DynamicPage>, IDynamicContent
	{
		public dynamic Regions { get; set; } = new ExpandoObject();

		public DynamicPage()
		{
		}
	}
}