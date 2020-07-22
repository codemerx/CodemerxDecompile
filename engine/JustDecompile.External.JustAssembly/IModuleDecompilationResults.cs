using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public interface IModuleDecompilationResults
	{
		uint ModuleToken { get; }
		string ModuleFilePath { get; }
		Dictionary<uint, IDecompilationResults> TypeDecompilationResults { get; }
	}
}
