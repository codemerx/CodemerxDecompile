using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;

namespace Mono.Cecil.Extensions
{
	[DoNotObfuscateType]
	[DoNotPrune]
	public static class ModuleDefinitionExtensions
	{
		public static NamespaceHierarchyTree BuildNamespaceHierarchyTree(this AssemblyDefinition self)
		{
			HashSet<string> strs = new HashSet<string>();
			foreach (ModuleDefinition module in self.Modules)
			{
				foreach (TypeDefinition type in module.Types)
				{
					if (type.Namespace == String.Empty && type.Name == "<Module>")
					{
						continue;
					}
					strs.Add(type.Namespace);
				}
			}
			return NamespaceHierarchyTreeBuilder.BuildTree(strs);
		}

		public static AssemblyNameReference GetReferencedCoreLibraryRef(this ModuleDefinition self, string coreLibraryName)
		{
			AssemblyNameReference assemblyNameReference;
			if (self == null)
			{
				throw new ArgumentNullException("Module definition is null.");
			}
			if (self.Assembly.Name.Name == coreLibraryName)
			{
				return self.Assembly.Name;
			}
			Collection<AssemblyNameReference>.Enumerator enumerator = self.AssemblyReferences.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssemblyNameReference current = enumerator.Current;
					if (current.Name != coreLibraryName)
					{
						continue;
					}
					assemblyNameReference = current;
					return assemblyNameReference;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return assemblyNameReference;
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
			AssemblyNameReference assemblyNameReference = self.ReferencedMscorlibRef();
			IAssemblyResolver assemblyResolver = self.AssemblyResolver;
			SpecialTypeAssembly specialTypeAssembly = (self.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None);
			AssemblyDefinition assemblyDefinition = assemblyResolver.Resolve(assemblyNameReference, "", self.GetModuleArchitecture(), specialTypeAssembly, true);
			if (assemblyDefinition == null)
			{
				return null;
			}
			return assemblyDefinition.MainModule;
		}

		public static AssemblyNameReference ReferencedMscorlibRef(this ModuleDefinition self)
		{
			return self.GetReferencedCoreLibraryRef("mscorlib");
		}
	}
}