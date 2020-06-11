using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers
{
	public interface IWinRTProjectManager : IMsBuildProjectManager
	{
		void Initialize(bool isUWPProject, Version minInstalledUAPVersion, Version maxInstalledUAPVersion);
	}
}
