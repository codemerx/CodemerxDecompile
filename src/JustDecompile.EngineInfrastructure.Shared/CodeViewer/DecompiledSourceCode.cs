using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Languages;
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
using Telerik.Windows.Documents.Code.Text;
#endif

namespace JustDecompile.EngineInfrastructure
{
	public class DecompiledSourceCode : ICodeViewerResults
	{
		private readonly string code;
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		private readonly IList<Span> bracketsSpans;
#endif
		private readonly IList<Tuple<int, IMemberDefinition>> lineToMemberMapList;
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		private readonly IDictionary<int, IList<CodeClassificationSpan>> classificationSpanToLineMap;

		public DecompiledSourceCode(string code, IDictionary<int, IList<CodeClassificationSpan>> classificationSpanToLineMap, IList<Span> bracketsSpans, IList<Tuple<int, IMemberDefinition>> lineToMemberMap)
		{
			this.code = code;

			this.classificationSpanToLineMap = classificationSpanToLineMap;

			this.lineToMemberMapList = lineToMemberMap;

			this.bracketsSpans = bracketsSpans;
		}
#else
		public DecompiledSourceCode(string code, IList<Tuple<int, IMemberDefinition>> lineToMemberMap)
		{
			this.code = code;

			this.lineToMemberMapList = lineToMemberMap;
		}
#endif
		public string NewLine
		{
			get { return "\n"; }
		}
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		public ITabCodeSettings TabCodeSettings { get; set; }

		public Span NamespaceSpan { get; set; }

		public Span UsingSpan { get; set; }

		public IList<WritingInfo> WritingInfos { get; set; }

		public IDictionary<int, IList<CodeClassificationSpan>> ClassificationSpanToLineMap
		{
			get
			{
				return this.classificationSpanToLineMap;
			}
		}
#endif
		public IList<Tuple<int, IMemberDefinition>> LineToMemberMapList
		{
			get
			{
				return this.lineToMemberMapList;
			}
		}

		public override string ToString()
		{
			return this.code;
		}

		public string GetSourceCode()
		{
			return this.code;
		}
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		public IList<Span> GetSpans()
		{
			return this.bracketsSpans;
		}

		public IEnumerable<PositionToken> GetSpansFromLine(int lineNumebr)
		{
			if (classificationSpanToLineMap.ContainsKey(lineNumebr))
			{
				IList<CodeClassificationSpan> visibleSpans = classificationSpanToLineMap[lineNumebr];

				return visibleSpans;
			}
			return Enumerable.Empty<PositionToken>();
		}

		public IEnumerable<PositionToken> GetReferencesOnlySpansFromLine(int lineNumebr)
		{
			foreach (PositionToken item in GetSpansFromLine(lineNumebr))
			{
				if (item.Kind == TokenKind.Reference || item.Kind == TokenKind.NotResolvedReference || item.Kind == TokenKind.FakeReference)
				{
					yield return item;
				}
			}
		}

		public IEnumerable<ICodeClassificationSpan> GetIntersectionSpans(int lineNumebr, Span spanBoundaries)
		{
			if (classificationSpanToLineMap.ContainsKey(lineNumebr))
			{
				IList<CodeClassificationSpan> visibleSpans = classificationSpanToLineMap[lineNumebr];

				foreach (CodeClassificationSpan codeToken in visibleSpans)
				{
					if (spanBoundaries.Contains(codeToken.Span))
					{
						yield return codeToken;
					}
					else
					{
						Span? intersectionSpan = codeToken.Span.Intersection(spanBoundaries);
						if (intersectionSpan.HasValue)
						{
							yield return new CodeClassificationSpan(codeToken, intersectionSpan.Value);
						}
					}
				}
			}
		}
#endif
		public IMemberDefinition GetMemberDefinitionFromLine(int lineNumber)
		{
			var lineToMemberMap = lineToMemberMapList.FirstOrDefault(t => t.Item1 == lineNumber);

			if (lineToMemberMap != null)
			{
				return lineToMemberMap.Item2;
			}
			return null;
		}

		public int GetLineFromMemberDefinition(IMemberDefinition memberDefinition)
		{
			var lineToMemberMap = lineToMemberMapList.FirstOrDefault(t => t.Item2 == memberDefinition);

			if (lineToMemberMap != null)
			{
				return lineToMemberMap.Item1;
			}
			return 0;
		}
	}
}
