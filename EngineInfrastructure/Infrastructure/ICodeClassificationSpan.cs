#if !JUSTASSEMBLY && !ENGINEONLYBUILD
using Telerik.Windows.Documents.Code.Tagging;
using Telerik.Windows.Documents.Code.Text;
#endif

namespace JustDecompile.EngineInfrastructure
{
	public interface ICodeClassificationSpan
	{
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		ClassificationType ClassificationType { get; set; }

		Span Span { get; set; }
#endif
	}
}
