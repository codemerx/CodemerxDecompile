using OrchardCore.Environment.Extensions.Features;
using System;

namespace OrchardCore.Environment.Extensions
{
	public class ExtensionDependencyStrategy : IExtensionDependencyStrategy
	{
		public ExtensionDependencyStrategy()
		{
			base();
			return;
		}

		public bool HasDependency(IFeatureInfo observer, IFeatureInfo subject)
		{
			return observer.get_Dependencies().Contains<string>(subject.get_Id());
		}
	}
}