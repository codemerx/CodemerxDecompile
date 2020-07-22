using JustDecompile.SmartAssembly.Attributes;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface ITypeGeneratedInfo : IFileGeneratedInfo
	{
		bool HasStructuralProblems
		{
			get;
		}

		Dictionary<MemberIdentifier, CodeSpan> MemberMapping
		{
			get;
		}

		IUniqueMemberIdentifier UniqueMemberIdentifier
		{
			get;
		}
	}
}