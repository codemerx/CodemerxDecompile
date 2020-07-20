using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public class MemberRenamingData
	{
		public HashSet<uint> RenamedMembers
		{
			get;
			private set;
		}

		public Dictionary<uint, string> RenamedMembersMap
		{
			get;
			private set;
		}

		public MemberRenamingData()
		{
			base();
			this.set_RenamedMembers(new HashSet<uint>());
			this.set_RenamedMembersMap(new Dictionary<uint, string>());
			return;
		}

		public MemberRenamingData(HashSet<uint> renamedMembers, Dictionary<uint, string> renamedMembersMap)
		{
			base();
			this.set_RenamedMembers(renamedMembers);
			this.set_RenamedMembersMap(renamedMembersMap);
			return;
		}
	}
}