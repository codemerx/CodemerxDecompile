using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IXamlPageProjectItemWriter : IXamlProjectItemWriter
	{
		void WriteXamlPageProjectItem(string relativeXamlPath);
	}
}
