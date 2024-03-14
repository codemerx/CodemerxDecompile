using OrchardCore.Environment.Extensions.Features;
using System;
using System.Linq;

namespace OrchardCore.Environment.Extensions
{
	public class ExtensionDependencyStrategy : IExtensionDependencyStrategy
	{
		public ExtensionDependencyStrategy()
		{
		}

		public bool HasDependency(IFeatureInfo observer, IFeatureInfo subject)
		{
			return observer.get_Dependencies().Contains<string>(subject.get_Id());
		}
	}
}