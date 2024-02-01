using System;

namespace Piranha
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class NoCoverageAttribute : Attribute
	{
		public NoCoverageAttribute()
		{
		}
	}
}