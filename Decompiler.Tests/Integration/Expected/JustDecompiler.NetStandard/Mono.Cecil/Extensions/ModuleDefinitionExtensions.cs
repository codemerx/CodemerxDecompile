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
			V_0 = new HashSet<string>();
			V_1 = self.get_Modules().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().get_Types().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							if (String.op_Equality(V_3.get_Namespace(), String.Empty) && String.op_Equality(V_3.get_Name(), "<Module>"))
							{
								continue;
							}
							dummyVar0 = V_0.Add(V_3.get_Namespace());
						}
					}
					finally
					{
						V_2.Dispose();
					}
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return NamespaceHierarchyTreeBuilder.BuildTree(V_0);
		}

		public static AssemblyNameReference GetReferencedCoreLibraryRef(this ModuleDefinition self, string coreLibraryName)
		{
			if (self == null)
			{
				throw new ArgumentNullException("Module definition is null.");
			}
			if (String.op_Equality(self.get_Assembly().get_Name().get_Name(), coreLibraryName))
			{
				return self.get_Assembly().get_Name();
			}
			V_0 = self.get_AssemblyReferences().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!String.op_Equality(V_1.get_Name(), coreLibraryName))
					{
						continue;
					}
					V_2 = V_1;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return null;
		}

		public static ModuleDefinition ReferencedMscorlib(this ModuleDefinition self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("Module definition is null.");
			}
			if (String.op_Equality(self.get_Assembly().get_Name().get_Name(), "mscorlib"))
			{
				return self;
			}
			V_0 = self.ReferencedMscorlibRef();
			stackVariable10 = self.get_AssemblyResolver();
			if (Mono.Cecil.AssemblyResolver.Extensions.IsReferenceAssembly(self))
			{
				stackVariable13 = 1;
			}
			else
			{
				stackVariable13 = 0;
			}
			V_1 = stackVariable13;
			V_2 = stackVariable10.Resolve(V_0, "", ModuleDefinitionExtensions.GetModuleArchitecture(self), V_1, true);
			if (V_2 == null)
			{
				return null;
			}
			return V_2.get_MainModule();
		}

		public static AssemblyNameReference ReferencedMscorlibRef(this ModuleDefinition self)
		{
			return self.GetReferencedCoreLibraryRef("mscorlib");
		}
	}
}