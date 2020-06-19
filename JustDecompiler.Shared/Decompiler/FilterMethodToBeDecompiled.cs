using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
    class FilterMethodToBeDecompiled
    {
        public FilterMethodToBeDecompiled(MethodDefinition method, CatchClause catchClause, DecompilationContext context, BlockStatement block)
        {
            this.Method = method;
            this.CatchClause = catchClause;
            this.Context = context;
            this.Block = block;
        }

        public MethodDefinition Method { get; private set; }

        public CatchClause CatchClause { get; private set; }

        public DecompilationContext Context { get; private set; }

        public BlockStatement Block { get; private set; }
    }
}
