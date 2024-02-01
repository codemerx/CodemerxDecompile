using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
	public static class NamespaceHierarchyTreeBuilder
	{
		private static void AddNamespaceToTree(NamespaceHierarchyNode root, string @namespace)
		{
			NamespaceHierarchyNode namespaceHierarchyNode;
			NamespaceHierarchyNode namespaceHierarchyNode1 = root;
			string[] strArray = @namespace.Split(new Char[] { '.' });
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (!namespaceHierarchyNode1.Children.TryGetValue(str, out namespaceHierarchyNode))
				{
					namespaceHierarchyNode = new NamespaceHierarchyNode(str);
					namespaceHierarchyNode1.Children[str] = namespaceHierarchyNode;
				}
				namespaceHierarchyNode1 = namespaceHierarchyNode;
			}
			namespaceHierarchyNode1.ContainsClasses = true;
		}

		public static NamespaceHierarchyTree BuildTree(HashSet<string> namespaces)
		{
			NamespaceHierarchyNode namespaceHierarchyNode = new NamespaceHierarchyNode(null);
			foreach (string @namespace in namespaces)
			{
				NamespaceHierarchyTreeBuilder.AddNamespaceToTree(namespaceHierarchyNode, @namespace);
			}
			return new NamespaceHierarchyTree(namespaceHierarchyNode);
		}
	}
}