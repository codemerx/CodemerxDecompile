using System;
using System.IO;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
	public class ProjectFileCreated : EventArgs
	{
		public string Name { get; private set; }
		public bool HasErrors { get; private set; }

		public ProjectFileCreated(string fullName, bool hasErrors)
		{
			Name = Path.GetFileName(fullName);
			HasErrors = hasErrors;
		}
	}
}
