using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public class MemberRenamingData
	{
		public HashSet<uint> RenamedMembers { get; private set; }
		public Dictionary<uint, string> RenamedMembersMap { get; private set; }

		public MemberRenamingData()
		{
			this.RenamedMembers = new HashSet<uint>();
			this.RenamedMembersMap = new Dictionary<uint, string>();
		}

		public MemberRenamingData(HashSet<uint> renamedMembers, Dictionary<uint, string> renamedMembersMap)
		{
			this.RenamedMembers = renamedMembers;
			this.RenamedMembersMap = renamedMembersMap;
		}
	}
}
