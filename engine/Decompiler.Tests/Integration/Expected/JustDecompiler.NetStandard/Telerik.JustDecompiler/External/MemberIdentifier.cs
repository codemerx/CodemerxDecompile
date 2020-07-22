using JustDecompile.SmartAssembly.Attributes;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	[DoNotSealType]
	public class MemberIdentifier : IComparable<MemberIdentifier>
	{
		public AssemblyIdentifier Assembly
		{
			get;
			private set;
		}

		public IUniqueMemberIdentifier UniqueMemberIdentifier
		{
			get;
			private set;
		}

		public MemberIdentifier(AssemblyIdentifier declaringAssembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
		{
			base();
			this.set_Assembly(declaringAssembly);
			this.set_UniqueMemberIdentifier(uniqueMemberIdentifier);
			return;
		}

		public int CompareTo(MemberIdentifier other)
		{
			if (String.op_Inequality(other.get_Assembly().get_AssemblyPath(), this.get_Assembly().get_AssemblyPath()))
			{
				stackVariable34 = this.get_Assembly().get_AssemblyPath();
				V_0 = other.get_Assembly();
				return stackVariable34.CompareTo(V_0.get_AssemblyPath());
			}
			if (String.op_Inequality(other.get_UniqueMemberIdentifier().get_ModuleFilePath(), this.get_UniqueMemberIdentifier().get_ModuleFilePath()))
			{
				return this.get_UniqueMemberIdentifier().get_ModuleFilePath().CompareTo(other.get_UniqueMemberIdentifier().get_ModuleFilePath());
			}
			V_1 = this.get_UniqueMemberIdentifier().get_MetadataToken();
			return V_1.CompareTo(other.get_UniqueMemberIdentifier().get_MetadataToken());
		}

		public override bool Equals(object obj)
		{
			V_0 = obj as MemberIdentifier;
			if (V_0 == null)
			{
				return false;
			}
			if (!String.Equals(this.get_Assembly().get_AssemblyPath(), V_0.get_Assembly().get_AssemblyPath(), 5))
			{
				return false;
			}
			if (!String.Equals(this.get_UniqueMemberIdentifier().get_ModuleFilePath(), V_0.get_UniqueMemberIdentifier().get_ModuleFilePath(), 5))
			{
				return false;
			}
			return this.get_UniqueMemberIdentifier().get_MetadataToken() == V_0.get_UniqueMemberIdentifier().get_MetadataToken();
		}

		public override int GetHashCode()
		{
			V_0 = this.get_Assembly();
			stackVariable8 = V_0.GetHashCode() ^ this.get_UniqueMemberIdentifier().get_ModuleFilePath().GetHashCode();
			V_1 = this.get_UniqueMemberIdentifier().get_MetadataToken();
			return stackVariable8 ^ V_1.GetHashCode();
		}

		public override string ToString()
		{
			V_0 = this.get_Assembly();
			return String.Format("{0} 0x{1:x8}", V_0.get_AssemblyPath(), this.get_UniqueMemberIdentifier());
		}
	}
}