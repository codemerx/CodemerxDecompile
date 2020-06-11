using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IXamlProjectItemWriter
	{
		void WriteXamlCodeEntryProjectItem(string relativeCodePath, string relativeXamlPath);
	}
}
