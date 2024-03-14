using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class DynamicPost : Post<DynamicPost>, IDynamicContent
	{
		public dynamic Regions { get; set; } = new ExpandoObject();

		public DynamicPost()
		{
		}
	}
}