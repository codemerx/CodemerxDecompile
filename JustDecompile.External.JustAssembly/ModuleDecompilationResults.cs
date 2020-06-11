using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	class ModuleDecompilationResults : IModuleDecompilationResults
	{
		public uint ModuleToken { get; private set; }

		public string ModuleFilePath { get; private set; }

		public Dictionary<uint, IDecompilationResults> TypeDecompilationResults { get; private set; }

		public ModuleDecompilationResults(uint moduleToken, string moduleFilePath, Dictionary<uint, IDecompilationResults> typeDecompilationResults)
		{
			this.ModuleToken = moduleToken;
			this.ModuleFilePath = moduleFilePath;
			this.TypeDecompilationResults = typeDecompilationResults;
		}
	}
}
