using System.Collections.Generic;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler
{
	public class ModuleSpecificContext
	{
		public ModuleDefinition Module { get; private set; }
		public ICollection<string> ModuleNamespaceUsings { get; private set; }
		public Dictionary<string, List<string>> CollisionTypesData { get; private set; }
		public Dictionary<string, HashSet<string>> NamespaceHieararchy { get; private set; }
		public Dictionary<string, string> RenamedNamespacesMap { get; private set; }
		public ICollection<uint> RenamedMembers { get; private set; }
		public Dictionary<uint, string> RenamedMembersMap { get; private set; }

		public ModuleSpecificContext()
		{
			this.Module = null;
			this.ModuleNamespaceUsings = new HashSet<string>();
			this.CollisionTypesData = new Dictionary<string, List<string>>();
			this.NamespaceHieararchy = new Dictionary<string, HashSet<string>>();
			this.RenamedNamespacesMap = new Dictionary<string, string>();
			this.RenamedMembers = new HashSet<uint>();
			this.RenamedMembersMap = new Dictionary<uint, string>();
		}

		public ModuleSpecificContext(ModuleDefinition module, ICollection<string> moduleNamespaceUsings, Dictionary<string, List<string>> collisionTypeData, 
			Dictionary<string, HashSet<string>> namespaceHierarchy, Dictionary<string, string> renamedNamespacesMap,  ICollection<uint> renamedMembers, Dictionary<uint, string> renamedMembersMap)
		{
			this.Module = module;
			this.ModuleNamespaceUsings = moduleNamespaceUsings;
			this.CollisionTypesData = collisionTypeData;
			this.NamespaceHieararchy = namespaceHierarchy;
			this.RenamedNamespacesMap = renamedNamespacesMap;
			this.RenamedMembers = renamedMembers;
			this.RenamedMembersMap = renamedMembersMap;
		}
	}
}
