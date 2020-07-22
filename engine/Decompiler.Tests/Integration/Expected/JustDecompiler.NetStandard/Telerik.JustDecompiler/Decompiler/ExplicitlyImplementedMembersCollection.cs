using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler
{
	public class ExplicitlyImplementedMembersCollection
	{
		private readonly Dictionary<TypeReference, ICollection<string>> collection;

		public ExplicitlyImplementedMembersCollection()
		{
			base();
			this.collection = new Dictionary<TypeReference, ICollection<string>>();
			return;
		}

		public void Add(TypeReference interface, string memberFullName)
		{
			if (!this.collection.ContainsKey(interface))
			{
				V_0 = new HashSet<string>();
				this.collection.Add(interface, V_0);
			}
			else
			{
				V_0 = this.collection.get_Item(interface);
			}
			V_0.Add(memberFullName);
			return;
		}

		public bool Contains(TypeReference interface, string memberFullName)
		{
			if (!this.collection.TryGetValue(interface, out V_0))
			{
				return false;
			}
			return V_0.Contains(memberFullName);
		}
	}
}