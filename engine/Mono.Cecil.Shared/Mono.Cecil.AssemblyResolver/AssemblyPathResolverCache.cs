using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyPathName = System.Collections.Generic.KeyValuePair<Mono.Cecil.AssemblyResolver.AssemblyStrongNameExtended, string>;

namespace Mono.Cecil.AssemblyResolver
{
    public class AssemblyPathResolverCache
    {
        protected List<AssemblyPathName> assemblyPathName;
        protected IDictionary<string, TargetPlatform> assemblyParts;
        protected IDictionary<string, AssemblyName> assemblyNameDefinition;
        protected IDictionary<string, TargetArchitecture> assemblyPathArchitecture;
        protected IClonableCollection<AssemblyStrongNameExtended> assemblyFaildedResolver;

        public AssemblyPathResolverCache()
        {
            assemblyPathName = new List<AssemblyPathName>();
            assemblyParts = new Dictionary<string, TargetPlatform>();
            assemblyNameDefinition = new Dictionary<string, AssemblyName>();
            assemblyPathArchitecture = new Dictionary<string, TargetArchitecture>();
            assemblyFaildedResolver = new UnresolvedAssembliesCollection();
        }

        public IClonableCollection<AssemblyStrongNameExtended> AssemblyFaildedResolverCache
        {
            get { return this.assemblyFaildedResolver; }
        }

        public IDictionary<string, TargetArchitecture> AssemblyPathArchitecture
        {
            get { return this.assemblyPathArchitecture; }
        }

        public IDictionary<string, AssemblyName> AssemblyNameDefinition
        {
            get { return this.assemblyNameDefinition; }
        }

        public IDictionary<string, TargetPlatform> AssemblyParts
        {
            get { return this.assemblyParts; }
        }

        public List<AssemblyPathName> AssemblyPathName
        {
            get { return this.assemblyPathName; }
        }

        internal void Clear()
        {
            assemblyPathName.Clear();
            assemblyParts.Clear();
            assemblyNameDefinition.Clear();
            assemblyPathArchitecture.Clear();
        }
    }
}
