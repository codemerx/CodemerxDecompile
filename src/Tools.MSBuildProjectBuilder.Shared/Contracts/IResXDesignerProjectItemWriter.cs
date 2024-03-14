using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IResXDesignerProjectItemWriter
	{
		void WriteResXDesignerResourceProjectItem(string relativeResourcePath, string relativeDesignerPath);

		void WriteResXDesignerSourceEntryProjectItem(string relativeDesignerPath, string relativeResourcePath);
	}
}
