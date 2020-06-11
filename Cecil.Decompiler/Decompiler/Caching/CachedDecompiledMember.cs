using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class CachedDecompiledMember
	{
		public DecompiledMember Member { get; private set; }
		public Dictionary<string, InitializationAssignment> FieldAssignmentData { get; private set; }

		public CachedDecompiledMember(DecompiledMember decompiledMember)
		{
			this.Member = decompiledMember;
			this.FieldAssignmentData = new Dictionary<string, InitializationAssignment>();
		}

		public CachedDecompiledMember(DecompiledMember decompiledMember, Dictionary<string, InitializationAssignment> fieldToAssignedExpression)
		{
			this.Member = decompiledMember;
			this.FieldAssignmentData = fieldToAssignedExpression;
		}

		public CachedDecompiledMember(DecompiledMember decompiledMember, TypeSpecificContext typeContext) : this(decompiledMember, typeContext.AssignmentData)
		{ 
		}
	}
}
