using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers
{
	public interface INetCoreProjectManager : IProjectManager, IProjectItemWriter
	{
		void AddPackageReferenceProjectItem(string include, string version);
	}
}
