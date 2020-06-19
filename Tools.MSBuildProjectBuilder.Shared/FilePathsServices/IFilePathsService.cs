using System;
using System.Linq;
using Mono.Cecil;
using System.Collections.Generic;
using Telerik.JustDecompiler.External.Interfaces;

namespace JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices
{
	public interface IFilePathsService : IExceptionThrownNotifier
	{
		Dictionary<ModuleDefinition, string> GetModulesToProjectsFilePathsMap();
		Dictionary<TypeDefinition, string> GetTypesToFilePathsMap();
		Dictionary<Resource, string> GetResourcesToFilePathsMap();
		Dictionary<string, string> GetXamlResourcesToFilePathsMap();
		string GetAssemblyInfoRelativePath();
		string GetResourceDesignerRelativePath(string resourceRelativePath);
		string GetSolutionRelativePath();
	}
}
