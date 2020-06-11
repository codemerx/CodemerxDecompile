using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
    class NamespaceHierarchyNode
    {
        public string Name { get; private set; }
        public bool ContainsClasses { get; set; }
        public Dictionary<string, NamespaceHierarchyNode> Children { get; private set; }

        public NamespaceHierarchyNode(string name)
        {
            this.Name = name;
            this.Children = new Dictionary<string, NamespaceHierarchyNode>();
        }
    }
}
