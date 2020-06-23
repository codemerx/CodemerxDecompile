using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Telerik.JustDecompiler.Common.NamespaceHierarchy
{
	public class NamespaceHierarchyTree
	{
		private readonly NamespaceHierarchyNode root;

		private string rootNamespace;

		public bool HasRootNamespace
		{
			get
			{
				return this.root.Children.Count == 1;
			}
		}

		public string RootNamespace
		{
			get
			{
				if (this.rootNamespace != null)
				{
					return this.rootNamespace;
				}
				if (!this.HasRootNamespace)
				{
					return null;
				}
				IEnumerator<string> enumerator = this.root.Children.Keys.GetEnumerator();
				enumerator.MoveNext();
				string current = enumerator.Current;
				string str = current;
				this.rootNamespace = current;
				return str;
			}
		}

		internal NamespaceHierarchyTree(NamespaceHierarchyNode root)
		{
			this.root = root;
		}

		public string[] GetSpecialPathTokens(string @namespace, bool skipFirstIfCommon = true)
		{
			int num;
			NamespaceHierarchyNode namespaceHierarchyNode;
			List<string> strs = new List<string>();
			string[] strArray = @namespace.Split(new Char[] { '.' });
			if (!skipFirstIfCommon || this.root.Children.Count != 1)
			{
				num = 0;
				namespaceHierarchyNode = this.root;
			}
			else
			{
				if (!this.root.Children.TryGetValue(strArray[0], out namespaceHierarchyNode))
				{
					return null;
				}
				num = 1;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (num < (int)strArray.Length)
			{
				if (!namespaceHierarchyNode.Children.TryGetValue(strArray[num], out namespaceHierarchyNode))
				{
					return null;
				}
				stringBuilder.Append(namespaceHierarchyNode.Name);
				if (!namespaceHierarchyNode.ContainsClasses)
				{
					stringBuilder.Append('.');
				}
				else
				{
					strs.Add(stringBuilder.ToString());
					stringBuilder = new StringBuilder();
				}
				num++;
			}
			if (stringBuilder.Length != 0)
			{
				return null;
			}
			return strs.ToArray();
		}
	}
}