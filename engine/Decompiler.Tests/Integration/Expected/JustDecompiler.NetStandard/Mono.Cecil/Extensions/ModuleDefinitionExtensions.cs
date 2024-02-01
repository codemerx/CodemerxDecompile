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
			foreach (ModuleDefinition module in self.get_Modules())
			{
				foreach (TypeDefinition type in module.get_Types())
				{
					if (type.get_Namespace() == String.Empty && type.get_Name() == "<Module>")
					{
						continue;
					}
					strs.Add(type.get_Namespace());
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
			if (self.get_Assembly().get_Name().get_Name() == coreLibraryName)
			{
				return self.get_Assembly().get_Name();
			}
			Collection<AssemblyNameReference>.Enumerator enumerator = self.get_AssemblyReferences().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AssemblyNameReference current = enumerator.get_Current();
					if (current.get_Name() != coreLibraryName)
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
				enumerator.Dispose();
			}
			return assemblyNameReference;
		}

		public static ModuleDefinition ReferencedMscorlib(this ModuleDefinition self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("Module definition is null.");
			}
			if (self.get_Assembly().get_Name().get_Name() == "mscorlib")
			{
				return self;
			}
			AssemblyNameReference assemblyNameReference = self.ReferencedMscorlibRef();
			IAssemblyResolver assemblyResolver = self.get_AssemblyResolver();
			SpecialTypeAssembly specialTypeAssembly = (Mono.Cecil.AssemblyResolver.Extensions.IsReferenceAssembly(self) ? 1 : 0);
			AssemblyDefinition assemblyDefinition = assemblyResolver.Resolve(assemblyNameReference, "", ModuleDefinitionExtensions.GetModuleArchitecture(self), specialTypeAssembly, true);
			if (assemblyDefinition == null)
			{
				return null;
			}
			return assemblyDefinition.get_MainModule();
		}

		public static AssemblyNameReference ReferencedMscorlibRef(this ModuleDefinition self)
		{
			return self.GetReferencedCoreLibraryRef("mscorlib");
		}
	}
}