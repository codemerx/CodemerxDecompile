using System;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.StateMachines;
using Telerik.JustDecompiler.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
    abstract class BaseStateMachineRemoverStep : IDecompilationStep
    {
        protected readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

        protected MethodSpecificContext moveNextMethodContext;
        protected ControlFlowGraph theCFG;

        public Ast.Statements.BlockStatement Process(DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            this.moveNextMethodContext = context.MethodContext;
            this.theCFG = this.moveNextMethodContext.ControlFlowGraph;
            moveNextMethodContext.IsMethodBodyChanged = true;

            StateMachineUtilities.FixInstructionConnections(theCFG.Blocks);

            if (!ProcessCFG())
            {
                context.StopPipeline();
            }
            return body;
        }

        protected abstract bool ProcessCFG();
    }
}
