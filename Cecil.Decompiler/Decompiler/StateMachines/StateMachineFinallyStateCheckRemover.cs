using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    /// <summary>
    /// Removes the blocks that check whether a finally handler should be executed.
    /// <remarks>
    /// The new C# 6.0 compiler no longer generates the doFinally variable. The check wheter the finally body should be executed or not
    /// is now done by checking if the value in the state variable is less than zero. If so - the finally body SHOULD be executed.
    /// </remarks>
    /// </summary>
    class StateMachineFinallyStateCheckRemover : StateMachineFinallyCheckRemoverBase
    {
        private VariableReference stateVariable;

        public StateMachineFinallyStateCheckRemover(MethodSpecificContext moveNextMethodContext)
            : base(moveNextMethodContext)
        {
        }

        public override void MarkFinallyConditionsForRemoval(VariableReference stateVariable)
        {
            this.stateVariable = stateVariable;
            MarkFinallyConditionsForRemovalInternal();
        }

        protected override bool IsFinallyCheckBlock(InstructionBlock finallyEntry)
        {
            Instruction current = finallyEntry.First;
            VariableReference loadedVariable;
            if (!StateMachineUtilities.TryGetVariableFromInstruction(current, methodVariables, out loadedVariable) ||
                loadedVariable != this.stateVariable)
            {
                return false;
            }

            current = current.Next;
            if (current.OpCode.Code != Code.Ldc_I4_0)
            {
                return false;
            }

            current = current.Next;
            if (current.OpCode.Code != Code.Bge && current.OpCode.Code != Code.Bge_S)
            {
                return false;
            }

            return true;
        }
    }
}
