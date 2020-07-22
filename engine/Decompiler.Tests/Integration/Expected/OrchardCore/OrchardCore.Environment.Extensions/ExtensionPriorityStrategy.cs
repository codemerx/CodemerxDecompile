using OrchardCore.Environment.Extensions.Features;
using System;

namespace OrchardCore.Environment.Extensions
{
	public class ExtensionPriorityStrategy : IExtensionPriorityStrategy
	{
		public ExtensionPriorityStrategy()
		{
			base();
			return;
		}

		public int GetPriority(IFeatureInfo feature)
		{
			return feature.get_Priority();
		}
	}
}