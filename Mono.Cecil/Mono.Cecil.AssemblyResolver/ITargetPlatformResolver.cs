
using System.Collections.Generic;

namespace Mono.Cecil.AssemblyResolver
{
	/*Telerik Authorship*/
	public interface ITargetPlatformResolver
	{
        AssemblyFrameworkResolverCache ResolverCache { get; }

		TargetPlatform GetTargetPlatform(ModuleDefinition module);

        TargetPlatform GetTargetPlatform(string assemblyFilePath, ModuleDefinition moduleDef);

        TargetPlatform GetTargetPlatform(string assemblyFilePath, IAssemblyResolver assemblyResolver);

        void AddPartCacheResult(IEnumerable<string> resultPaths, TargetPlatform runtime);

        bool IsCLR4Assembly(ModuleDefinition module);

        void ClearCache();
    }
}
