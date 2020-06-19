using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IWinFormsProjectItemWriter
	{
		void WriteWinFormsEntryProjectItem(string relativeWinFormPath);

		void WriteWinFormsResourceProjectItem(string relativeWinFormResourcePath, string relativeWinFormPath);
	}
}
