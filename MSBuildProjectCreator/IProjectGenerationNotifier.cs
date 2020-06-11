using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
	public interface IProjectGenerationNotifier
	{
		void OnProjectGenerationFinished();

		void OnProjectGenerationFailure(Exception ex);

		void OnTypeWritingFailure(string typeName, Exception ex);

		void OnResourceWritingFailure(string resourceName, Exception ex);
	}
}
