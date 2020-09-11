using System.Collections.Generic;

namespace CodemerxDecompile.Service.Services.DecompilationContext.Models
{
    public class DecompiledAssemblyModuleMetadata
    {
        public Dictionary<string, DecompiledTypeMetadata> TypeNameToTypeMetadata { get; } = new Dictionary<string, DecompiledTypeMetadata>();
    }
}
