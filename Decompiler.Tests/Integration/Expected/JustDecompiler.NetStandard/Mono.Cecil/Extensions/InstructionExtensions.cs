using Mono.Cecil.Cil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class InstructionExtensions
	{
		internal static bool IsLoadRegister(this Instruction instruction, out int register)
		{
			Code code = instruction.OpCode.Code;
			switch (code)
			{
				case Code.Ldloc_0:
				{
					register = 0;
					return true;
				}
				case Code.Ldloc_1:
				{
					register = 1;
					return true;
				}
				case Code.Ldloc_2:
				{
					register = 2;
					return true;
				}
				case Code.Ldloc_3:
				{
					register = 3;
					return true;
				}
				default:
				{
					if ((int)code - (int)Code.Ldloc_S <= (int)Code.Break || (int)code - (int)Code.Ldloc <= (int)Code.Break)
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
			register = ((VariableReference)instruction.Operand).Index;
			return true;
		}

		internal static bool IsStoreRegister(this Instruction instruction, out int register)
		{
			Code code = instruction.OpCode.Code;
			switch (code)
			{
				case Code.Stloc_0:
				{
					register = 0;
					return true;
				}
				case Code.Stloc_1:
				{
					register = 1;
					return true;
				}
				case Code.Stloc_2:
				{
					register = 2;
					return true;
				}
				case Code.Stloc_3:
				{
					register = 3;
					return true;
				}
				default:
				{
					if (code == Code.Stloc_S || code == Code.Stloc)
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
			register = ((VariableReference)instruction.Operand).Index;
			return true;
		}
	}
}