using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Mono.Cecil.AssemblyResolver
{
    public class AssemblyFrameworkResolverCache
    {
        public AssemblyFrameworkResolverCache()
        {
            this.InitializeNewCache();
        }

        public IDictionary<string, FrameworkName> AssemblyPathToFrameworkName { get; private set; }

        public IDictionary<string, FrameworkVersion> AssemblyPathToFrameworkVersion { get; private set; }

        public IDictionary<string, TargetPlatform> AssemblyPathToTargetPlatform { get; private set; }

        public IDictionary<string, AssemblyNameReference> AssemblyPathToSystemRuntimeReference { get; private set; }

        public void Clear()
        {
            this.InitializeNewCache();
        }

        public void AddAssemblySystemRuntimeReference(string assemblyFilePath, AssemblyNameReference reference)
        {
            if (!this.AssemblyPathToSystemRuntimeReference.ContainsKey(assemblyFilePath))
            {
                this.AssemblyPathToSystemRuntimeReference.Add(assemblyFilePath, reference);
            }
        }

        public void AddAssemblyTargetPlatformToCache(string assemblyFilePath, TargetPlatform targetPlatform)
        {
            if (!this.AssemblyPathToTargetPlatform.ContainsKey(assemblyFilePath))
            {
                this.AssemblyPathToTargetPlatform.Add(assemblyFilePath, targetPlatform);
            }
        }

        private void InitializeNewCache()
        {
            this.AssemblyPathToFrameworkName = new Dictionary<string, FrameworkName>();
            this.AssemblyPathToSystemRuntimeReference = new Dictionary<string, AssemblyNameReference>();
            this.AssemblyPathToTargetPlatform = new Dictionary<string, TargetPlatform>();
        }
    }
}
