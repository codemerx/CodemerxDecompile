using System.Collections.Generic;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface ITypeGeneratedInfo : IFileGeneratedInfo
	{
		IUniqueMemberIdentifier UniqueMemberIdentifier { get; }

		Dictionary<MemberIdentifier, CodeSpan> MemberMapping { get; }

		bool HasStructuralProblems { get; }
	}
}