using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
    public static class NamespaceHierarchyTreeBuilder
    {
        public static NamespaceHierarchyTree BuildTree(HashSet<string> namespaces)
        {
            NamespaceHierarchyNode root = new NamespaceHierarchyNode(null);
            foreach (string @namespace in namespaces)
	        {
                AddNamespaceToTree(root, @namespace);
	        }

            return new NamespaceHierarchyTree(root);
        }

        private static void AddNamespaceToTree(NamespaceHierarchyNode root, string @namespace)
        {
            NamespaceHierarchyNode currentNode = root;
            string[] tokens = @namespace.Split('.');

            foreach (string token in tokens)
            {
                NamespaceHierarchyNode currentChild;
                if(!currentNode.Children.TryGetValue(token, out currentChild))
                {
                    currentChild = new NamespaceHierarchyNode(token);
                    currentNode.Children[token] = currentChild;
                }

                currentNode = currentChild;
            }

            currentNode.ContainsClasses = true;
        }
    }
}
