using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IRegularProjectItemWriter
	{
		void WriteRegularProjectItem(string relativePath);
	}
}
