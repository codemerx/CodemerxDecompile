using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
	internal class NamespaceHierarchyNode
	{
		public Dictionary<string, NamespaceHierarchyNode> Children
		{
			get;
			private set;
		}

		public bool ContainsClasses
		{
			get;
			set;
		}

		public string Name
		{
			get;
			private set;
		}

		public NamespaceHierarchyNode(string name)
		{
			base();
			this.set_Name(name);
			this.set_Children(new Dictionary<string, NamespaceHierarchyNode>());
			return;
		}
	}
}