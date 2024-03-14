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
			this.collection = new Dictionary<TypeReference, ICollection<string>>();
		}

		public void Add(TypeReference @interface, string memberFullName)
		{
			ICollection<string> strs;
			if (!this.collection.ContainsKey(@interface))
			{
				strs = new HashSet<string>();
				this.collection.Add(@interface, strs);
			}
			else
			{
				strs = this.collection[@interface];
			}
			strs.Add(memberFullName);
		}

		public bool Contains(TypeReference @interface, string memberFullName)
		{
			ICollection<string> strs;
			if (!this.collection.TryGetValue(@interface, out strs))
			{
				return false;
			}
			return strs.Contains(memberFullName);
		}
	}
}