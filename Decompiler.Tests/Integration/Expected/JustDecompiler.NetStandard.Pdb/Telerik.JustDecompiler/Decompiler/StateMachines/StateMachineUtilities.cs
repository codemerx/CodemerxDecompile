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
			orderedBlocks[0].get_First().set_Previous(null);
			V_0 = 0;
			while (V_0 < (int)orderedBlocks.Length - 1)
			{
				orderedBlocks[V_0].get_Last().set_Next(orderedBlocks[V_0 + 1].get_First());
				orderedBlocks[V_0 + 1].get_First().set_Previous(orderedBlocks[V_0].get_Last());
				V_0 = V_0 + 1;
			}
			orderedBlocks[(int)orderedBlocks.Length - 1].get_Last().set_Next(null);
			return;
		}

		public static bool IsUnconditionalBranch(Instruction instruction)
		{
			V_0 = instruction.get_OpCode().get_Code();
			if (V_0 == 55 || V_0 == 42 || V_0 == 187)
			{
				return true;
			}
			return V_0 == 188;
		}

		public static bool TryGetOperandOfLdc(Instruction instruction, out int operand)
		{
			switch (instruction.get_OpCode().get_Code() - 21)
			{
				case 0:
				{
					operand = -1;
					break;
				}
				case 1:
				{
					operand = 0;
					break;
				}
				case 2:
				{
					operand = 1;
					break;
				}
				case 3:
				{
					operand = 2;
					break;
				}
				case 4:
				{
					operand = 3;
					break;
				}
				case 5:
				{
					operand = 4;
					break;
				}
				case 6:
				{
					operand = 5;
					break;
				}
				case 7:
				{
					operand = 6;
					break;
				}
				case 8:
				{
					operand = 7;
					break;
				}
				case 9:
				{
					operand = 8;
					break;
				}
				case 10:
				{
					operand = (SByte)instruction.get_Operand();
					break;
				}
				case 11:
				{
					operand = (Int32)instruction.get_Operand();
					break;
				}
				default:
				{
					goto Label0;
				}
			}
			return true;
		Label0:
			operand = 0;
			return false;
		}

		public static bool TryGetVariableFromInstruction(Instruction instruction, IList<VariableDefinition> variableCollection, out VariableReference varReference)
		{
			// 
			// Current member / type: System.Boolean Telerik.JustDecompiler.Decompiler.StateMachines.StateMachineUtilities::TryGetVariableFromInstruction(Mono.Cecil.Cil.Instruction,System.Collections.Generic.IList`1<Mono.Cecil.Cil.VariableDefinition>,Mono.Cecil.Cil.VariableReference&)
			// Exception in: System.Boolean TryGetVariableFromInstruction(Mono.Cecil.Cil.Instruction,System.Collections.Generic.IList<Mono.Cecil.Cil.VariableDefinition>,Mono.Cecil.Cil.VariableReference&)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}