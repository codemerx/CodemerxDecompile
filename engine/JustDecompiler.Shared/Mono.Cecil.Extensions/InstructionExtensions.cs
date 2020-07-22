using Mono.Cecil.Cil;

namespace Mono.Cecil.Extensions
{
    public static class InstructionExtensions
    {
        internal static bool IsStoreRegister(this Instruction instruction, out int register)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Stloc:
                case Code.Stloc_S:
                    register = ((VariableReference)instruction.Operand).Index;
                    return true;
                case Code.Stloc_0:
                    register = 0;
                    return true;
                case Code.Stloc_1:
                    register = 1;
                    return true;
                case Code.Stloc_2:
                    register = 2;
                    return true;
                case Code.Stloc_3:
                    register = 3;
                    return true;
                default:
                    register = -1;
                    return false;
            }
        }

        internal static bool IsLoadRegister(this Instruction instruction, out int register)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldloc:
                case Code.Ldloc_S:
                case Code.Ldloca:
                case Code.Ldloca_S:
                    register = ((VariableReference)instruction.Operand).Index;
                    return true;
                case Code.Ldloc_0:
                    register = 0;
                    return true;
                case Code.Ldloc_1:
                    register = 1;
                    return true;
                case Code.Ldloc_2:
                    register = 2;
                    return true;
                case Code.Ldloc_3:
                    register = 3;
                    return true;
                default:
                    register = -1;
                    return false;
            }
        }
    }
}