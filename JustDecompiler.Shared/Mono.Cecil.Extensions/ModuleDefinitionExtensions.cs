using System;
using System.Collections.Generic;
using Mono.Cecil.AssemblyResolver;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;

namespace Mono.Cecil.Extensions
{
    [DoNotPrune]
    [DoNotObfuscateType]
    public static class ModuleDefinitionExtensions
    {
		public static NamespaceHierarchyTree BuildNamespaceHierarchyTree(this AssemblyDefinition self)
		{
			HashSet<string> namespaces = new HashSet<string>();
			foreach (ModuleDefinition module in self.Modules)
			{
				foreach (TypeDefinition typeDef in module.Types)
				{
					if (typeDef.Namespace == string.Empty && typeDef.Name == "<Module>")
					{
						continue;
					}
					namespaces.Add(typeDef.Namespace);
				}
			}

			return NamespaceHierarchyTreeBuilder.BuildTree(namespaces);
		}

		public static ModuleDefinition ReferencedMscorlib(this ModuleDefinition self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("Module definition is null.");
			}

			if (self.Assembly.Name.Name == "mscorlib")
			{
				return self;
			}

			AssemblyNameReference assemblyRef = self.ReferencedMscorlibRef();
            IAssemblyResolver resolver = self.AssemblyResolver;

            SpecialTypeAssembly special = self.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;
            AssemblyDefinition a = resolver.Resolve(assemblyRef,"",self.GetModuleArchitecture(), special);
			if (a != null)
			{
				return a.MainModule;
			}

			return null;
		}

		public static AssemblyNameReference ReferencedMscorlibRef(this ModuleDefinition self)
		{
            return self.GetReferencedCoreLibraryRef("mscorlib");
		}

        public static AssemblyNameReference GetReferencedCoreLibraryRef(this ModuleDefinition self, string coreLibraryName)
        {
            if (self == null)
            {
                throw new ArgumentNullException("Module definition is null.");
            }

            if (self.Assembly.Name.Name == coreLibraryName)
            {
                return self.Assembly.Name;
            }

            foreach (AssemblyNameReference assemblyRef in self.AssemblyReferences)
            {
                if (assemblyRef.Name == coreLibraryName)
                {
                    return assemblyRef;
                }
            }

            return null;
        }
    }
}
