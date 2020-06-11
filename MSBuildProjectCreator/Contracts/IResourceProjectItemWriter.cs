using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IResourceProjectItemWriter
	{
		void AddResourceToOtherEmbeddedResources(string resourceLegalName);

		void AddResourceToOtherXamlResources(string xamlResourceRelativePath);
	}
}
