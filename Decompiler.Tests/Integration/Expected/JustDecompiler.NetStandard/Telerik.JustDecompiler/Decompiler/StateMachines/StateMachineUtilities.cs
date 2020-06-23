using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal static class StateMachineUtilities
	{
		public static void FixInstructionConnections(InstructionBlock[] orderedBlocks)
		{
			orderedBlocks[0].First.Previous = null;
			for (int i = 0; i < (int)orderedBlocks.Length - 1; i++)
			{
				orderedBlocks[i].Last.Next = orderedBlocks[i + 1].First;
				orderedBlocks[i + 1].First.Previous = orderedBlocks[i].Last;
			}
			orderedBlocks[(int)orderedBlocks.Length - 1].Last.Next = null;
		}

		public static bool IsUnconditionalBranch(Instruction instruction)
		{
			Code code = instruction.OpCode.Code;
			if (code == Code.Br || code == Code.Br_S || code == Code.Leave)
			{
				return true;
			}
			return code == Code.Leave_S;
		}

		public static bool TryGetOperandOfLdc(Instruction instruction, out int operand)
		{
			switch (instruction.OpCode.Code)
			{
				case Code.Ldc_I4_M1:
				{
					operand = -1;
					break;
				}
				case Code.Ldc_I4_0:
				{
					operand = 0;
					break;
				}
				case Code.Ldc_I4_1:
				{
					operand = 1;
					break;
				}
				case Code.Ldc_I4_2:
				{
					operand = 2;
					break;
				}
				case Code.Ldc_I4_3:
				{
					operand = 3;
					break;
				}
				case Code.Ldc_I4_4:
				{
					operand = 4;
					break;
				}
				case Code.Ldc_I4_5:
				{
					operand = 5;
					break;
				}
				case Code.Ldc_I4_6:
				{
					operand = 6;
					break;
				}
				case Code.Ldc_I4_7:
				{
					operand = 7;
					break;
				}
				case Code.Ldc_I4_8:
				{
					operand = 8;
					break;
				}
				case Code.Ldc_I4_S:
				{
					operand = (SByte)instruction.Operand;
					break;
				}
				case Code.Ldc_I4:
				{
					operand = (Int32)instruction.Operand;
					break;
				}
				default:
				{
					operand = 0;
					return false;
				}
			}
			return true;
		}

		public static bool TryGetVariableFromInstruction(Instruction instruction, IList<VariableDefinition> variableCollection, out VariableReference varReference)
		{
			Code code = instruction.OpCode.Code;
			switch (code)
			{
				case Code.Ldloc_0:
				case Code.Stloc_0:
				{
					varReference = variableCollection[0];
					break;
				}
				case Code.Ldloc_1:
				case Code.Stloc_1:
				{
					varReference = variableCollection[1];
					break;
				}
				case Code.Ldloc_2:
				case Code.Stloc_2:
				{
					varReference = variableCollection[2];
					break;
				}
				case Code.Ldloc_3:
				case Code.Stloc_3:
				{
					varReference = variableCollection[3];
					break;
				}
				case Code.Ldarg_S:
				case Code.Ldarga_S:
				case Code.Starg_S:
				{
					varReference = null;
					return false;
				}
				case Code.Ldloc_S:
				case Code.Ldloca_S:
				case Code.Stloc_S:
				{
					Object operand = instruction.Operand as VariableReference;
					if (operand == null)
					{
						operand = variableCollection[(SByte)instruction.Operand];
					}
					varReference = (VariableReference)operand;
					break;
				}
				default:
				{
					if ((int)code - (int)Code.Ldloc <= (int)Code.Ldarg_0)
					{
						varReference = variableCollection[(Int32)instruction.Operand];
						break;
					}
					else
					{
						varReference = null;
						return false;
					}
				}
			}
			return true;
		}
	}
}