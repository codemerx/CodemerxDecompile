using System;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	[DoNotPruneType]
    [DoNotObfuscateType]
	[DoNotSealType]
    public class MemberIdentifier : IComparable<MemberIdentifier>
	{
        /// <summary>
        /// Creates a MemberIdentifier object, used to identify a member (type, method, property, event or field).
        /// </summary>
		/// <param name="declaringAssembly"> An AssemblyIdentifier to the assembly, containing the member.</param>
		/// <param name="memberMetadataToken">The metadata token of the member.</param>
		public MemberIdentifier(AssemblyIdentifier declaringAssembly, IUniqueMemberIdentifier uniqueMemberIdentifier)
        {
			this.Assembly = declaringAssembly;
			this.UniqueMemberIdentifier = uniqueMemberIdentifier;
        }

		public IUniqueMemberIdentifier UniqueMemberIdentifier { get; private set; }

		public AssemblyIdentifier Assembly { get; private set; }

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
			MemberIdentifier temp = obj as MemberIdentifier;
			if (temp == null)
			{
				return false;
			}

			if (!String.Equals(this.Assembly.AssemblyPath, temp.Assembly.AssemblyPath, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (!String.Equals(this.UniqueMemberIdentifier.ModuleFilePath, temp.UniqueMemberIdentifier.ModuleFilePath, StringComparison.OrdinalIgnoreCase))
			{
				return false;

			}

			return this.UniqueMemberIdentifier.MetadataToken == temp.UniqueMemberIdentifier.MetadataToken;
		}

		public override int GetHashCode()
		{
			// Consider possible caching of the hash code for eventual speed improvement
			return Assembly.GetHashCode() ^ UniqueMemberIdentifier.ModuleFilePath.GetHashCode() ^ UniqueMemberIdentifier.MetadataToken.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} 0x{1:x8}", Assembly.AssemblyPath, UniqueMemberIdentifier);
		}
	}
}
