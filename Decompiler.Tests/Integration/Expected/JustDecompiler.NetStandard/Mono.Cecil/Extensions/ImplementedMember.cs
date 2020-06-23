using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public class ImplementedMember : IEquatable<ImplementedMember>
	{
		public TypeReference DeclaringType
		{
			get;
			private set;
		}

		public MemberReference Member
		{
			get;
			private set;
		}

		public ImplementedMember(TypeReference declaringType, MemberReference member)
		{
			this.DeclaringType = declaringType;
			this.Member = member;
		}

		public bool Equals(ImplementedMember other)
		{
			if (this.DeclaringType != other.DeclaringType)
			{
				return false;
			}
			return this.Member == other.Member;
		}
	}
}