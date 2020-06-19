using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
    static class StateMachineUtilities
    {
        /// <summary>
        /// Tries to get the integer operand of the given ldc.i4* instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="operand"></param>
        /// <returns>False if the given instruction is not ldc.i4*, otherwise returns true.</returns>
        public static bool TryGetOperandOfLdc(Instruction instruction, out int operand)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldc_I4_M1:
                    operand = -1;
                    break;
                case Code.Ldc_I4_0:
                    operand = 0;
                    break;
                case Code.Ldc_I4_1:
                    operand = 1;
                    break;
                case Code.Ldc_I4_2:
                    operand = 2;
                    break;
                case Code.Ldc_I4_3:
                    operand = 3;
                    break;
                case Code.Ldc_I4_4:
                    operand = 4;
                    break;
                case Code.Ldc_I4_5:
                    operand = 5;
                    break;
                case Code.Ldc_I4_6:
                    operand = 6;
                    break;
                case Code.Ldc_I4_7:
                    operand = 7;
                    break;
                case Code.Ldc_I4_8:
                    operand = 8;
                    break;
                case Code.Ldc_I4_S:
                    operand = (sbyte)instruction.Operand;
                    break;
                case Code.Ldc_I4:
                    operand = (int)instruction.Operand;
                    break;
                default:
                    operand = 0;
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to get the variable that is used by the specified instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="varReference"></param>
        /// <returns>Flase if the instruction does not handle variables.</returns>
        public static bool TryGetVariableFromInstruction(Instruction instruction, IList<VariableDefinition> variableCollection,
            out VariableReference varReference)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldloc_0:
                case Code.Stloc_0:
                    varReference = variableCollection[0];
                    break;
                case Code.Ldloc_1:
                case Code.Stloc_1:
                    varReference = variableCollection[1];
                    break;
                case Code.Ldloc_2:
                case Code.Stloc_2:
                    varReference = variableCollection[2];
                    break;
                case Code.Ldloc_3:
                case Code.Stloc_3:
                    varReference = variableCollection[3];
                    break;
                case Code.Ldloc_S:
                case Code.Ldloca_S:
                case Code.Stloc_S:
                    varReference = instruction.Operand as VariableReference ?? variableCollection[(sbyte)instruction.Operand];
                    break;
                case Code.Ldloc:
                case Code.Ldloca:
                case Code.Stloc:
                    varReference = variableCollection[(int)instruction.Operand];
                    break;
                default:
                    varReference = null;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the given instruction is unconditional branch.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public static bool IsUnconditionalBranch(Instruction instruction)
        {
            Code code = instruction.OpCode.Code;
            return code == Code.Br || code == Code.Br_S || code == Code.Leave || code == Code.Leave_S;
        }

        /// <summary>
        /// Fixes the connection of the instructions that are left in the CFG.
        /// </summary>
        /// <remarks>
        /// When building the CFG some of the nop blocks are removed by the CFGBuilder, but some of the remaining instructions continue to point at
        /// instructions from the removed blocks, which causes problems when removing blocks from the state machine. This method fixes the problem.
        /// 
        /// Example of the problem:
        /// normal block        - end of exception handler points to first instruction of this block
        /// nop block           - removed by the CFG builder
        /// 
        /// If we remove the normal block then the end of the handler will point to the begining of the nop block, which is not in the CFG.
        /// This will cause the GuardedBlocksBuilderStep to fail.
        /// </remarks>
        /// <param name="orderedBlocks"></param>
        public static void FixInstructionConnections(InstructionBlock[] orderedBlocks)
        {
            orderedBlocks[0].First.Previous = null;
            for (int i = 0; i < orderedBlocks.Length - 1; i++)
            {
                orderedBlocks[i].Last.Next = orderedBlocks[i + 1].First;
                orderedBlocks[i + 1].First.Previous = orderedBlocks[i].Last;
            }
            orderedBlocks[orderedBlocks.Length - 1].Last.Next = null;
        }
    }
}
