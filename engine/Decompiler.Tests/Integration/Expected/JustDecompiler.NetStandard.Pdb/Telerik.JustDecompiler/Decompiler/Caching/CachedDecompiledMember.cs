using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class CachedDecompiledMember
	{
		public Dictionary<string, InitializationAssignment> FieldAssignmentData
		{
			get;
			private set;
		}

		public DecompiledMember Member
		{
			get;
			private set;
		}

		public CachedDecompiledMember(DecompiledMember decompiledMember)
		{
			base();
			this.set_Member(decompiledMember);
			this.set_FieldAssignmentData(new Dictionary<string, InitializationAssignment>());
			return;
		}

		public CachedDecompiledMember(DecompiledMember decompiledMember, Dictionary<string, InitializationAssignment> fieldToAssignedExpression)
		{
			base();
			this.set_Member(decompiledMember);
			this.set_FieldAssignmentData(fieldToAssignedExpression);
			return;
		}

		public CachedDecompiledMember(DecompiledMember decompiledMember, TypeSpecificContext typeContext)
		{
			this(decompiledMember, typeContext.get_AssignmentData());
			return;
		}
	}
}