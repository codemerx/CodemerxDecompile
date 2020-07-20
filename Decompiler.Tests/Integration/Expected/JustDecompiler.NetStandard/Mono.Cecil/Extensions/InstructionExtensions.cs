using Mono.Cecil.Cil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class InstructionExtensions
	{
		internal static bool IsLoadRegister(this Instruction instruction, out int register)
		{
			V_0 = instruction.get_OpCode().get_Code();
			switch (V_0 - 6)
			{
				case 0:
				{
					register = 0;
					return true;
				}
				case 1:
				{
					register = 1;
					return true;
				}
				case 2:
				{
					register = 2;
					return true;
				}
				case 3:
				{
					register = 3;
					return true;
				}
				default:
				{
					if (V_0 - 17 <= 1 || V_0 - 202 <= 1)
					{
						break;
					}
					else
					{
						goto Label0;
					}
				}
			}
			register = ((VariableReference)instruction.get_Operand()).get_Index();
			return true;
		Label0:
			register = -1;
			return false;
		}

		internal static bool IsStoreRegister(this Instruction instruction, out int register)
		{
			V_0 = instruction.get_OpCode().get_Code();
			switch (V_0 - 10)
			{
				case 0:
				{
					register = 0;
					return true;
				}
				case 1:
				{
					register = 1;
					return true;
				}
				case 2:
				{
					register = 2;
					return true;
				}
				case 3:
				{
					register = 3;
					return true;
				}
				default:
				{
					if (V_0 == 19 || V_0 == 204)
					{
						break;
					}
					else
					{
						goto Label0;
					}
				}
			}
			register = ((VariableReference)instruction.get_Operand()).get_Index();
			return true;
		Label0:
			register = -1;
			return false;
		}
	}
}