using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler
{
	public class ModuleSpecificContext
	{
		public Dictionary<string, List<string>> CollisionTypesData
		{
			get;
			private set;
		}

		public ModuleDefinition Module
		{
			get;
			private set;
		}

		public ICollection<string> ModuleNamespaceUsings
		{
			get;
			private set;
		}

		public Dictionary<string, HashSet<string>> NamespaceHieararchy
		{
			get;
			private set;
		}

		public ICollection<uint> RenamedMembers
		{
			get;
			private set;
		}

		public Dictionary<uint, string> RenamedMembersMap
		{
			get;
			private set;
		}

		public Dictionary<string, string> RenamedNamespacesMap
		{
			get;
			private set;
		}

		public ModuleSpecificContext()
		{
			base();
			this.set_Module(null);
			this.set_ModuleNamespaceUsings(new HashSet<string>());
			this.set_CollisionTypesData(new Dictionary<string, List<string>>());
			this.set_NamespaceHieararchy(new Dictionary<string, HashSet<string>>());
			this.set_RenamedNamespacesMap(new Dictionary<string, string>());
			this.set_RenamedMembers(new HashSet<uint>());
			this.set_RenamedMembersMap(new Dictionary<uint, string>());
			return;
		}

		public ModuleSpecificContext(ModuleDefinition module, ICollection<string> moduleNamespaceUsings, Dictionary<string, List<string>> collisionTypeData, Dictionary<string, HashSet<string>> namespaceHierarchy, Dictionary<string, string> renamedNamespacesMap, ICollection<uint> renamedMembers, Dictionary<uint, string> renamedMembersMap)
		{
			base();
			this.set_Module(module);
			this.set_ModuleNamespaceUsings(moduleNamespaceUsings);
			this.set_CollisionTypesData(collisionTypeData);
			this.set_NamespaceHieararchy(namespaceHierarchy);
			this.set_RenamedNamespacesMap(renamedNamespacesMap);
			this.set_RenamedMembers(renamedMembers);
			this.set_RenamedMembersMap(renamedMembersMap);
			return;
		}
	}
}