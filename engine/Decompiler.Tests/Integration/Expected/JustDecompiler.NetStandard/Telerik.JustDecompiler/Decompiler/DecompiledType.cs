using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompiledType
	{
		public Dictionary<string, DecompiledMember> DecompiledMembers
		{
			get;
			private set;
		}

		public TypeDefinition Type
		{
			get;
			private set;
		}

		public TypeSpecificContext TypeContext
		{
			get;
			private set;
		}

		public DecompiledType(TypeDefinition type)
		{
			this.Type = type;
			this.DecompiledMembers = new Dictionary<string, DecompiledMember>();
			this.TypeContext = new TypeSpecificContext(type);
		}
	}
}