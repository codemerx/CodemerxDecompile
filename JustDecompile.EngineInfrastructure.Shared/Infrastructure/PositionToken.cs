#if !JUSTASSEMBLY && !ENGINEONLYBUILD
using Telerik.Windows.Documents.Code.Text;
#endif

namespace JustDecompile.EngineInfrastructure
{
	public class PositionToken : Token
	{
		public string Message { get; set; }
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		public Span Span { get; set; }
#endif
	}
}
