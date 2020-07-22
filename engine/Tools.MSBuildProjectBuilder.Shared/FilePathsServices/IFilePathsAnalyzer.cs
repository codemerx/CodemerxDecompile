using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices
{
	public interface IFilePathsAnalyzer
	{
		int GetMinimumNeededRelativeFilePathLength(string projFileName);
	}
}
