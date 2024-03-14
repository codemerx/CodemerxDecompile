using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public interface IDecompilationStep
	{
		BlockStatement Process(DecompilationContext context, BlockStatement body);
	}
}