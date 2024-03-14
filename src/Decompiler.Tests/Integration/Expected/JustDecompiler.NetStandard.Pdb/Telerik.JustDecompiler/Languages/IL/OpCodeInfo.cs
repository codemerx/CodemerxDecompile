using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal sealed class OpCodeInfo
	{
		private readonly static OpCodeInfo[] knownOpCodes;

		private readonly static Dictionary<Code, OpCodeInfo> knownOpCodeDict;

		private Mono.Cecil.Cil.OpCode opcode;

		public bool CanThrow
		{
			get;
			private set;
		}

		public bool IsMoveInstruction
		{
			get;
			private set;
		}

		public Mono.Cecil.Cil.OpCode OpCode
		{
			get
			{
				return this.opcode;
			}
		}

		static OpCodeInfo()
		{
			OpCodeInfo.knownOpCodes = new OpCodeInfo[] { new OpCodeInfo(OpCodes.Add)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Add_Ovf)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Add_Ovf_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.And)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Arglist)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Beq)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Beq_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bge)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bge_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bge_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bge_Un_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bgt)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bgt_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bgt_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bgt_Un_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ble)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ble_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ble_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ble_Un_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Blt)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Blt_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Blt_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Blt_Un_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bne_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Bne_Un_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Br)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Br_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Break)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Brfalse)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Brfalse_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Brtrue)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Brtrue_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Call)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Calli)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ceq)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Cgt)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Cgt_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ckfinite)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Clt)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Clt_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_I1)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_I2)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_I4)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_I8)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_R4)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_R8)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_U1)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_U2)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_U4)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_U8)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_I)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_U)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_R_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I1)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I2)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I8)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U1)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U2)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U8)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I1_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I2_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I4_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I8_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U1_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U2_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U4_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U8_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_I_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Conv_Ovf_U_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Div)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Div_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Dup)
			{
				CanThrow = true,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Endfilter)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Endfinally)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldarg)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldarg_0)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldarg_1)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldarg_2)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldarg_3)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldarg_S)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldarga)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldarga_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_M1)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_0)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_1)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_2)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_3)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_4)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_5)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_6)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_7)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_8)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I4_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_I8)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_R4)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldc_R8)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldftn)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldind_I1)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_I2)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_I4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_I8)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_U1)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_U2)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_U4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_R4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_R8)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_I)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldind_Ref)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ldloc)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldloc_0)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldloc_1)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldloc_2)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldloc_3)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldloc_S)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Ldloca)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldloca_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldnull)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Leave)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Leave_S)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Localloc)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Mul)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Mul_Ovf)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Mul_Ovf_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Neg)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Nop)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Not)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Or)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Pop)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Rem)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Rem_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Ret)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Shl)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Shr)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Shr_Un)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Starg)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Starg_S)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Stind_I1)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_I2)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_I4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_I8)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_R4)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_R8)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_I)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stind_Ref)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Stloc)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Stloc_0)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Stloc_1)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Stloc_2)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Stloc_3)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Stloc_S)
			{
				CanThrow = false,
				IsMoveInstruction = true
			}, new OpCodeInfo(OpCodes.Sub)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Sub_Ovf)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Sub_Ovf_Un)
			{
				CanThrow = true
			}, new OpCodeInfo(OpCodes.Switch)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Xor)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Box), new OpCodeInfo(OpCodes.Callvirt), new OpCodeInfo(OpCodes.Castclass), new OpCodeInfo(OpCodes.Cpobj), new OpCodeInfo(OpCodes.Initobj)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Isinst)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldelem_Any), new OpCodeInfo(OpCodes.Ldelem_I), new OpCodeInfo(OpCodes.Ldelem_I1), new OpCodeInfo(OpCodes.Ldelem_I2), new OpCodeInfo(OpCodes.Ldelem_I4), new OpCodeInfo(OpCodes.Ldelem_I8), new OpCodeInfo(OpCodes.Ldelem_R4), new OpCodeInfo(OpCodes.Ldelem_R8), new OpCodeInfo(OpCodes.Ldelem_Ref), new OpCodeInfo(OpCodes.Ldelem_U1), new OpCodeInfo(OpCodes.Ldelem_U2), new OpCodeInfo(OpCodes.Ldelem_U4), new OpCodeInfo(OpCodes.Ldelema), new OpCodeInfo(OpCodes.Ldfld), new OpCodeInfo(OpCodes.Ldflda), new OpCodeInfo(OpCodes.Ldlen), new OpCodeInfo(OpCodes.Ldobj), new OpCodeInfo(OpCodes.Ldsfld), new OpCodeInfo(OpCodes.Ldsflda), new OpCodeInfo(OpCodes.Ldstr)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldtoken)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Ldvirtftn), new OpCodeInfo(OpCodes.Mkrefany), new OpCodeInfo(OpCodes.Newarr), new OpCodeInfo(OpCodes.Newobj), new OpCodeInfo(OpCodes.Refanytype)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Refanyval), new OpCodeInfo(OpCodes.Rethrow), new OpCodeInfo(OpCodes.Sizeof)
			{
				CanThrow = false
			}, new OpCodeInfo(OpCodes.Stelem_Any), new OpCodeInfo(OpCodes.Stelem_I1), new OpCodeInfo(OpCodes.Stelem_I2), new OpCodeInfo(OpCodes.Stelem_I4), new OpCodeInfo(OpCodes.Stelem_I8), new OpCodeInfo(OpCodes.Stelem_R4), new OpCodeInfo(OpCodes.Stelem_R8), new OpCodeInfo(OpCodes.Stelem_Ref), new OpCodeInfo(OpCodes.Stfld), new OpCodeInfo(OpCodes.Stobj), new OpCodeInfo(OpCodes.Stsfld), new OpCodeInfo(OpCodes.Throw), new OpCodeInfo(OpCodes.Unbox), new OpCodeInfo(OpCodes.Unbox_Any) };
			OpCodeInfo.knownOpCodeDict = OpCodeInfo.knownOpCodes.ToDictionary<OpCodeInfo, Code>((OpCodeInfo info) => info.OpCode.get_Code());
		}

		private OpCodeInfo(Mono.Cecil.Cil.OpCode opcode)
		{
			this.opcode = opcode;
			this.CanThrow = true;
		}

		public static OpCodeInfo Get(Mono.Cecil.Cil.OpCode opCode)
		{
			return OpCodeInfo.Get(opCode.get_Code());
		}

		public static OpCodeInfo Get(Code code)
		{
			OpCodeInfo opCodeInfo;
			if (!OpCodeInfo.knownOpCodeDict.TryGetValue(code, out opCodeInfo))
			{
				throw new NotSupportedException(code.ToString());
			}
			return opCodeInfo;
		}

		public static bool IsUnconditionalBranch(Mono.Cecil.Cil.OpCode opcode)
		{
			FlowControl flowControl;
			if (opcode.get_OpCodeType() == 4)
			{
				return false;
			}
			switch (opcode.get_FlowControl())
			{
				case 0:
				case 7:
				case 8:
				{
					return true;
				}
				case 1:
				case 4:
				case 6:
				{
					flowControl = opcode.get_FlowControl();
					throw new NotSupportedException(flowControl.ToString());
				}
				case 2:
				case 3:
				case 5:
				{
					return false;
				}
				default:
				{
					flowControl = opcode.get_FlowControl();
					throw new NotSupportedException(flowControl.ToString());
				}
			}
		}
	}
}