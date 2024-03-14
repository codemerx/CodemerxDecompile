using Mono.Cecil.Cil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class InstructionExtensions
	{
		internal static bool IsLoadRegister(this Instruction instruction, out int register)
		{
			Code code = instruction.get_OpCode().get_Code();
			switch (code)
			{
				case 6:
				{
					register = 0;
					return true;
				}
				case 7:
				{
					register = 1;
					return true;
				}
				case 8:
				{
					register = 2;
					return true;
				}
				case 9:
				{
					register = 3;
					return true;
				}
				default:
				{
					if (code - 17 <= 1 || code - 202 <= 1)
					{
						break;
					}
					else
					{
						register = -1;
						return false;
					}
				}
			}
			register = ((VariableReference)instruction.get_Operand()).get_Index();
			return true;
		}

		internal static bool IsStoreRegister(this Instruction instruction, out int register)
		{
			Code code = instruction.get_OpCode().get_Code();
			switch (code)
			{
				case 10:
				{
					register = 0;
					return true;
				}
				case 11:
				{
					register = 1;
					return true;
				}
				case 12:
				{
					register = 2;
					return true;
				}
				case 13:
				{
					register = 3;
					return true;
				}
				default:
				{
					if (code == 19 || code == 204)
					{
						break;
					}
					else
					{
						register = -1;
						return false;
					}
				}
			}
			register = ((VariableReference)instruction.get_Operand()).get_Index();
			return true;
		}
	}
}