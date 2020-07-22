using System;
using System.Linq;
using Mono.Cecil;

namespace Mono.Cecil.Extensions
{
	public class ImplementedMember : IEquatable<ImplementedMember>
	{
		public TypeReference DeclaringType { get; private set; }
		public MemberReference Member { get; private set; }

		public ImplementedMember(TypeReference declaringType, MemberReference member)
		{
			this.DeclaringType = declaringType;
			this.Member = member;
		}

		public bool Equals(ImplementedMember other)
		{
			return this.DeclaringType == other.DeclaringType && this.Member == other.Member;
		}
	}
}
