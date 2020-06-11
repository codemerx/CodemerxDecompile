using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	class AssemblyDecompilationResults : IAssemblyDecompilationResults
	{
		public string AssemblyFilePath { get; private set; }

		public IDecompilationResults AssemblyAttributesDecompilationResults { get; private set; }

		public ICollection<IModuleDecompilationResults> ModuleDecompilationResults { get; private set; }

		public ICollection<string> ResourcesFilePaths { get; private set; }

		public AssemblyDecompilationResults(string assemblyFilePath, IDecompilationResults assemblyAttributesDecompilationResults, ICollection<IModuleDecompilationResults> moduleDecompilationResults,
			ICollection<string> resourcesFilepaths)
		{
			this.AssemblyFilePath = assemblyFilePath;
			this.AssemblyAttributesDecompilationResults = assemblyAttributesDecompilationResults;
			this.ModuleDecompilationResults = moduleDecompilationResults;
			this.ResourcesFilePaths = resourcesFilepaths;
		}
	}
}
