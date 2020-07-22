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
			base();
			this.set_DeclaringType(declaringType);
			this.set_Member(member);
			return;
		}

		public bool Equals(ImplementedMember other)
		{
			if ((object)this.get_DeclaringType() != (object)other.get_DeclaringType())
			{
				return false;
			}
			return (object)this.get_Member() == (object)other.get_Member();
		}
	}
}