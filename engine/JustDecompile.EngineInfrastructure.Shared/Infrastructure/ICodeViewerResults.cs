using JustDecompile.EngineInfrastructure;
using Mono.Cecil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
using Telerik.Windows.Documents.Code.Text;
#endif

namespace JustDecompile.EngineInfrastructure
{
	public interface ICodeViewerResults
	{
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		IList<WritingInfo> WritingInfos { get; set; }

		Span NamespaceSpan { get; }

		Span UsingSpan { get; }
#endif
		string NewLine { get; }
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		IList<Span> GetSpans();
#endif
		string GetSourceCode();
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		ITabCodeSettings TabCodeSettings { get; set; }

		IEnumerable<PositionToken> GetReferencesOnlySpansFromLine(int lineNumebr);

		IEnumerable<PositionToken> GetSpansFromLine(int lineNumebr);

		IEnumerable<ICodeClassificationSpan> GetIntersectionSpans(int lineNumebr, Span spanBoundaries);

		IMemberDefinition GetMemberDefinitionFromLine(int lineNumber);

		int GetLineFromMemberDefinition(IMemberDefinition memberDefinition);
#endif
	}
}
