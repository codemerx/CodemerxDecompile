using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public abstract class BasePdbStatement : Statement, IPdbCodeNode
    {
        private MethodDefinition cachedInstructionsContainer;

        public MethodDefinition UnderlyingInstructionsMember
        {
            get
            {
                if (cachedInstructionsContainer == null)
                {
                    Instruction firstInstruction = this.UnderlyingSameMethodInstructions.FirstOrDefault();
                    cachedInstructionsContainer = firstInstruction != null ? firstInstruction.ContainingMethod : null;
                }
                return cachedInstructionsContainer;
            }
        }
    }
}
