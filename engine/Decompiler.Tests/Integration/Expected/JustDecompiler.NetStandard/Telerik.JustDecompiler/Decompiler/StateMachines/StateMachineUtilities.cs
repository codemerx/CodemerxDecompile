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
			orderedBlocks[0].First.set_Previous(null);
			for (int i = 0; i < (int)orderedBlocks.Length - 1; i++)
			{
				orderedBlocks[i].Last.set_Next(orderedBlocks[i + 1].First);
				orderedBlocks[i + 1].First.set_Previous(orderedBlocks[i].Last);
			}
			orderedBlocks[(int)orderedBlocks.Length - 1].Last.set_Next(null);
		}

		public static bool IsUnconditionalBranch(Instruction instruction)
		{
			Code code = instruction.get_OpCode().get_Code();
			if (code == 55 || code == 42 || code == 187)
			{
				return true;
			}
			return code == 188;
		}

		public static bool TryGetOperandOfLdc(Instruction instruction, out int operand)
		{
			switch (instruction.get_OpCode().get_Code())
			{
				case 21:
				{
					operand = -1;
					break;
				}
				case 22:
				{
					operand = 0;
					break;
				}
				case 23:
				{
					operand = 1;
					break;
				}
				case 24:
				{
					operand = 2;
					break;
				}
				case 25:
				{
					operand = 3;
					break;
				}
				case 26:
				{
					operand = 4;
					break;
				}
				case 27:
				{
					operand = 5;
					break;
				}
				case 28:
				{
					operand = 6;
					break;
				}
				case 29:
				{
					operand = 7;
					break;
				}
				case 30:
				{
					operand = 8;
					break;
				}
				case 31:
				{
					operand = (SByte)instruction.get_Operand();
					break;
				}
				case 32:
				{
					operand = (Int32)instruction.get_Operand();
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
			// 
			// Current member / type: System.Boolean Telerik.JustDecompiler.Decompiler.StateMachines.StateMachineUtilities::TryGetVariableFromInstruction(Mono.Cecil.Cil.Instruction,System.Collections.Generic.IList`1<Mono.Cecil.Cil.VariableDefinition>,Mono.Cecil.Cil.VariableReference&)
			// Exception in: System.Boolean TryGetVariableFromInstruction(Mono.Cecil.Cil.Instruction,System.Collections.Generic.IList<Mono.Cecil.Cil.VariableDefinition>,Mono.Cecil.Cil.VariableReference&)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}