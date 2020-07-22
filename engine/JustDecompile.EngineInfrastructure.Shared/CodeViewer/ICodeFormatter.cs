using Telerik.JustDecompiler.Languages;

namespace JustDecompile.EngineInfrastructure
{
	public interface ICodeFormatter : ITextFormatter, IFormatter
	{
		DecompiledSourceCode GetSourceCode();
	}
}
