using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
    public class GeneratedMethod
    {
        public GeneratedMethod(MethodDefinition method, Statement body, MethodSpecificContext context)
        {
            this.Method = method;
            this.Body = body;
            this.Context = context;
        }

        public MethodDefinition Method { get; private set; }

        public Statement Body { get; private set; }

        public MethodSpecificContext Context { get; private set; }
    }
}
