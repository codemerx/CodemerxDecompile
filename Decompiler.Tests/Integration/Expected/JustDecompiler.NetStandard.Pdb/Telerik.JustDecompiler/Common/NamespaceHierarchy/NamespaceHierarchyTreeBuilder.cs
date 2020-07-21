using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
	public static class NamespaceHierarchyTreeBuilder
	{
		private static void AddNamespaceToTree(NamespaceHierarchyNode root, string namespace)
		{
			V_0 = root;
			stackVariable3 = new Char[1];
			stackVariable3[0] = '.';
			V_1 = namespace.Split(stackVariable3);
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (!V_0.get_Children().TryGetValue(V_3, out V_4))
				{
					V_4 = new NamespaceHierarchyNode(V_3);
					V_0.get_Children().set_Item(V_3, V_4);
				}
				V_0 = V_4;
				V_2 = V_2 + 1;
			}
			V_0.set_ContainsClasses(true);
			return;
		}

		public static NamespaceHierarchyTree BuildTree(HashSet<string> namespaces)
		{
			V_0 = new NamespaceHierarchyNode(null);
			V_1 = namespaces.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					NamespaceHierarchyTreeBuilder.AddNamespaceToTree(V_0, V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return new NamespaceHierarchyTree(V_0);
		}
	}
}