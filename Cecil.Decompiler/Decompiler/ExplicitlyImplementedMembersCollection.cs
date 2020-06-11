using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler
{
	public class ExplicitlyImplementedMembersCollection
	{
		private readonly Dictionary<TypeReference, ICollection<string>> collection;

		public ExplicitlyImplementedMembersCollection()
		{
			this.collection = new Dictionary<TypeReference, ICollection<string>>();
		}

		public bool Contains(TypeReference @interface, string memberFullName)
		{
			ICollection<string> interfaceExplicitlyImplementedMembers;

			if (!collection.TryGetValue(@interface, out interfaceExplicitlyImplementedMembers))
			{
				return false;
			}

			return interfaceExplicitlyImplementedMembers.Contains(memberFullName);
		}

		public void Add(TypeReference @interface, string memberFullName)
		{
			ICollection<string> interfaceExplicitlyImplementedMembers;

			if (collection.ContainsKey(@interface))
			{
				interfaceExplicitlyImplementedMembers = collection[@interface];
			}
			else
			{
				interfaceExplicitlyImplementedMembers = new HashSet<string>();
				collection.Add(@interface, interfaceExplicitlyImplementedMembers);
			}

			interfaceExplicitlyImplementedMembers.Add(memberFullName);
		}

	}
}
