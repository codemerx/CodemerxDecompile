using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    abstract class StateMachineFinallyCheckRemoverBase
    {
        private readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

        protected readonly ControlFlowGraph theCFG;
        protected readonly Collection<VariableDefinition> methodVariables;

        public StateMachineFinallyCheckRemoverBase(MethodSpecificContext methodContext)
        {
            this.methodVariables = methodContext.Body.Variables;
            this.theCFG = methodContext.ControlFlowGraph;
        }

        /// <summary>
        /// Gets the blocks that were marked for removal during the cleaning of the CFG.
        /// </summary>
        public HashSet<InstructionBlock> BlocksMarkedForRemoval
        {
            get
            {
                return toBeRemoved;
            }
        }

        public abstract void MarkFinallyConditionsForRemoval(VariableReference checkVariable);

        protected void MarkFinallyConditionsForRemovalInternal()
        {
            foreach (ExceptionHandler exHandler in theCFG.RawExceptionHandlers)
            {
                InstructionBlock finallyEntry;
                if (exHandler.HandlerType == ExceptionHandlerType.Finally &&
                    theCFG.InstructionToBlockMapping.TryGetValue(exHandler.HandlerStart.Offset, out finallyEntry) &&
                    IsFinallyCheckBlock(finallyEntry))
                {
                    toBeRemoved.Add(finallyEntry);
                    finallyEntry.Successors = new InstructionBlock[0];
                }
            }
        }

        protected abstract bool IsFinallyCheckBlock(InstructionBlock finallyEntry);
    }
}
