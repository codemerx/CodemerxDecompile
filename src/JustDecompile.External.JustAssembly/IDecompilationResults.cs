using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace JustDecompile.External.JustAssembly
{
	public interface IDecompilationResults
	{
		string FilePath { get; }
		ICodeViewerResults CodeViewerResults { get; }
		Dictionary<uint, IOffsetSpan> MemberDeclarationToCodePostionMap { get; }
		Dictionary<uint, IOffsetSpan> MemberTokenToDocumentationMap { get; }
		Dictionary<uint, IOffsetSpan> MemberTokenToAttributesMap { get; }
		Dictionary<uint, IOffsetSpan> MemberTokenToDecompiledCodeMap { get; }
		ICollection<uint> MembersWithExceptions { get; }
	}
}
