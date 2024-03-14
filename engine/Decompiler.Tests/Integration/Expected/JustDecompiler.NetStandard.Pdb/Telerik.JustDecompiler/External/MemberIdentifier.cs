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
			this.Assembly = declaringAssembly;
			this.UniqueMemberIdentifier = uniqueMemberIdentifier;
		}

		public int CompareTo(MemberIdentifier other)
		{
			if (other.Assembly.AssemblyPath != this.Assembly.AssemblyPath)
			{
				return this.Assembly.AssemblyPath.CompareTo(other.Assembly.AssemblyPath);
			}
			if (other.UniqueMemberIdentifier.ModuleFilePath != this.UniqueMemberIdentifier.ModuleFilePath)
			{
				return this.UniqueMemberIdentifier.ModuleFilePath.CompareTo(other.UniqueMemberIdentifier.ModuleFilePath);
			}
			return this.UniqueMemberIdentifier.MetadataToken.CompareTo(other.UniqueMemberIdentifier.MetadataToken);
		}

		public override bool Equals(object obj)
		{
			MemberIdentifier memberIdentifier = obj as MemberIdentifier;
			if (memberIdentifier == null)
			{
				return false;
			}
			if (!String.Equals(this.Assembly.AssemblyPath, memberIdentifier.Assembly.AssemblyPath, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!String.Equals(this.UniqueMemberIdentifier.ModuleFilePath, memberIdentifier.UniqueMemberIdentifier.ModuleFilePath, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return this.UniqueMemberIdentifier.MetadataToken == memberIdentifier.UniqueMemberIdentifier.MetadataToken;
		}

		public override int GetHashCode()
		{
			AssemblyIdentifier assembly = this.Assembly;
			return assembly.GetHashCode() ^ this.UniqueMemberIdentifier.ModuleFilePath.GetHashCode() ^ this.UniqueMemberIdentifier.MetadataToken.GetHashCode();
		}

		public override string ToString()
		{
			AssemblyIdentifier assembly = this.Assembly;
			return String.Format("{0} 0x{1:x8}", (object)assembly.AssemblyPath, this.UniqueMemberIdentifier);
		}
	}
}