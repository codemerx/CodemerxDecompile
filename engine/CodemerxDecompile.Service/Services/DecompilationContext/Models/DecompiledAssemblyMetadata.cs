using System.Collections.Generic;

namespace CodemerxDecompile.Service.Services.DecompilationContext.Models
{
    public class DecompiledAssemblyMetadata
    {
        public Dictionary<string, DecompiledAssemblyModuleMetadata> ModuleNameToModuleMetadata { get; } = new Dictionary<string, DecompiledAssemblyModuleMetadata>();
    }
}
