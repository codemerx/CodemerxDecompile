using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil;
using JustDecompile.Tools.MSBuildProjectBuilder;

namespace JustDecompile.External.JustAssembly
{
	class JustAssemblyFilePathsAnalyzer : JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices.DefaultFilePathsAnalyzer
	{
		public JustAssemblyFilePathsAnalyzer(AssemblyDefinition assembly, ILanguage language, bool decompileDangerousResources)
			: base(Utilities.GetUserDefinedTypes(assembly, decompileDangerousResources), Utilities.GetResources(assembly), language, decompileDangerousResources)
		{
		}

		public int GetMaximumPossibleTargetPathLength()
		{
			return MSBuildProjectBuilder.MaxPathLength - GetMinimumNeededRelativeFilePathLength("");
		}

		protected override int GetProjFileMaximumLength(string projFileName)
		{
			return 0; // project file is not written in JustAssemblyProjectBuilder
		}
	}
}
