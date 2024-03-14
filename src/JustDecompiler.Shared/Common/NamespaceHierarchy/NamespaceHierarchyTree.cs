using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
    public class NamespaceHierarchyTree
    {
        private readonly NamespaceHierarchyNode root;
        private string rootNamespace;

        internal NamespaceHierarchyTree(NamespaceHierarchyNode root)
        {
            this.root = root;
        }

        public bool HasRootNamespace
        {
            get
            {
                return root.Children.Count == 1;
            }
        }

        public string RootNamespace
        {
            get
            {
                if (rootNamespace != null)
                {
                    return rootNamespace;
                }

                if (HasRootNamespace)
                {
                    IEnumerator<string> enumerator = root.Children.Keys.GetEnumerator();
                    enumerator.MoveNext();
                    return rootNamespace = enumerator.Current;
                }

                return null;
            }
        }

        public string[] GetSpecialPathTokens(string @namespace, bool skipFirstIfCommon = true)
        {
            List<string> path = new List<string>();

            string[] tokens = @namespace.Split('.');

            int tokenIndex;
            NamespaceHierarchyNode currentNode;
            if (skipFirstIfCommon && root.Children.Count == 1)
            {
                if (!root.Children.TryGetValue(tokens[0], out currentNode))
                {
                    return null;
                }

                tokenIndex = 1;
            }
            else
            {
                tokenIndex = 0;
                currentNode = root;
            }

            StringBuilder pathPartBuilder = new StringBuilder();
            for (; tokenIndex < tokens.Length; tokenIndex++)
            {
                if(!currentNode.Children.TryGetValue(tokens[tokenIndex], out currentNode))
                {
                    return null;
                }

                pathPartBuilder.Append(currentNode.Name);
                if (currentNode.ContainsClasses)
                {
                    path.Add(pathPartBuilder.ToString());
                    pathPartBuilder = new StringBuilder();
                }
                else
                {
                    pathPartBuilder.Append('.');
                }
            }

            return pathPartBuilder.Length == 0 ? path.ToArray() : null;
        }
    }
}
