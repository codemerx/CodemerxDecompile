using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IAppDefinitionProjectItemWriter : IXamlProjectItemWriter
	{
		void WriteAppDefinitionXamlEntryProjectItem(string relativeXamlPath);
	}
}
