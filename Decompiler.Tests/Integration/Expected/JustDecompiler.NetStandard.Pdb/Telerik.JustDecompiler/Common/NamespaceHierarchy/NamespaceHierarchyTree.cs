using System;
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
				return this.root.get_Children().get_Count() == 1;
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
				if (!this.get_HasRootNamespace())
				{
					return null;
				}
				V_0 = this.root.get_Children().get_Keys().GetEnumerator();
				dummyVar0 = V_0.MoveNext();
				stackVariable15 = V_0.get_Current();
				V_1 = stackVariable15;
				this.rootNamespace = stackVariable15;
				return V_1;
			}
		}

		internal NamespaceHierarchyTree(NamespaceHierarchyNode root)
		{
			base();
			this.root = root;
			return;
		}

		public string[] GetSpecialPathTokens(string namespace, bool skipFirstIfCommon = true)
		{
			V_0 = new List<string>();
			stackVariable3 = new Char[1];
			stackVariable3[0] = '.';
			V_1 = namespace.Split(stackVariable3);
			if (!skipFirstIfCommon || this.root.get_Children().get_Count() != 1)
			{
				V_2 = 0;
				V_3 = this.root;
			}
			else
			{
				if (!this.root.get_Children().TryGetValue(V_1[0], out V_3))
				{
					return null;
				}
				V_2 = 1;
			}
			V_4 = new StringBuilder();
			while (V_2 < (int)V_1.Length)
			{
				if (!V_3.get_Children().TryGetValue(V_1[V_2], out V_3))
				{
					return null;
				}
				dummyVar0 = V_4.Append(V_3.get_Name());
				if (!V_3.get_ContainsClasses())
				{
					dummyVar1 = V_4.Append('.');
				}
				else
				{
					V_0.Add(V_4.ToString());
					V_4 = new StringBuilder();
				}
				V_2 = V_2 + 1;
			}
			if (V_4.get_Length() != 0)
			{
				return null;
			}
			return V_0.ToArray();
		}
	}
}