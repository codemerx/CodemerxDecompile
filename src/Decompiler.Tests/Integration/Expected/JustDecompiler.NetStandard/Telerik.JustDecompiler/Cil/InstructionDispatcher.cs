using Mono.Cecil.Cil;
using System;

namespace Telerik.JustDecompiler.Cil
{
	internal class InstructionDispatcher
	{
		public InstructionDispatcher()
		{
		}

		public static void Dispatch(Instruction instruction, IInstructionVisitor visitor)
		{
			switch (instruction.get_OpCode().get_Code())
			{
				case 0:
				{
					visitor.OnNop(instruction);
					return;
				}
				case 1:
				{
					visitor.OnBreak(instruction);
					return;
				}
				case 2:
				{
					visitor.OnLdarg_0(instruction);
					return;
				}
				case 3:
				{
					visitor.OnLdarg_1(instruction);
					return;
				}
				case 4:
				{
					visitor.OnLdarg_2(instruction);
					return;
				}
				case 5:
				{
					visitor.OnLdarg_3(instruction);
					return;
				}
				case 6:
				{
					visitor.OnLdloc_0(instruction);
					return;
				}
				case 7:
				{
					visitor.OnLdloc_1(instruction);
					return;
				}
				case 8:
				{
					visitor.OnLdloc_2(instruction);
					return;
				}
				case 9:
				{
					visitor.OnLdloc_3(instruction);
					return;
				}
				case 10:
				{
					visitor.OnStloc_0(instruction);
					return;
				}
				case 11:
				{
					visitor.OnStloc_1(instruction);
					return;
				}
				case 12:
				{
					visitor.OnStloc_2(instruction);
					return;
				}
				case 13:
				{
					visitor.OnStloc_3(instruction);
					return;
				}
				case 14:
				case 199:
				{
					visitor.OnLdarg(instruction);
					return;
				}
				case 15:
				case 200:
				{
					visitor.OnLdarga(instruction);
					return;
				}
				case 16:
				case 201:
				{
					visitor.OnStarg(instruction);
					return;
				}
				case 17:
				case 202:
				{
					visitor.OnLdloc(instruction);
					return;
				}
				case 18:
				case 203:
				{
					visitor.OnLdloca(instruction);
					return;
				}
				case 19:
				case 204:
				{
					visitor.OnStloc(instruction);
					return;
				}
				case 20:
				{
					visitor.OnLdnull(instruction);
					return;
				}
				case 21:
				{
					visitor.OnLdc_I4_M1(instruction);
					return;
				}
				case 22:
				{
					visitor.OnLdc_I4_0(instruction);
					return;
				}
				case 23:
				{
					visitor.OnLdc_I4_1(instruction);
					return;
				}
				case 24:
				{
					visitor.OnLdc_I4_2(instruction);
					return;
				}
				case 25:
				{
					visitor.OnLdc_I4_3(instruction);
					return;
				}
				case 26:
				{
					visitor.OnLdc_I4_4(instruction);
					return;
				}
				case 27:
				{
					visitor.OnLdc_I4_5(instruction);
					return;
				}
				case 28:
				{
					visitor.OnLdc_I4_6(instruction);
					return;
				}
				case 29:
				{
					visitor.OnLdc_I4_7(instruction);
					return;
				}
				case 30:
				{
					visitor.OnLdc_I4_8(instruction);
					return;
				}
				case 31:
				case 32:
				{
					visitor.OnLdc_I4(instruction);
					return;
				}
				case 33:
				{
					visitor.OnLdc_I8(instruction);
					return;
				}
				case 34:
				{
					visitor.OnLdc_R4(instruction);
					return;
				}
				case 35:
				{
					visitor.OnLdc_R8(instruction);
					return;
				}
				case 36:
				{
					visitor.OnDup(instruction);
					return;
				}
				case 37:
				{
					visitor.OnPop(instruction);
					return;
				}
				case 38:
				{
					visitor.OnJmp(instruction);
					return;
				}
				case 39:
				{
					visitor.OnCall(instruction);
					return;
				}
				case 40:
				{
					visitor.OnCalli(instruction);
					return;
				}
				case 41:
				{
					visitor.OnRet(instruction);
					return;
				}
				case 42:
				case 55:
				{
					visitor.OnBr(instruction);
					return;
				}
				case 43:
				case 56:
				{
					visitor.OnBrfalse(instruction);
					return;
				}
				case 44:
				case 57:
				{
					visitor.OnBrtrue(instruction);
					return;
				}
				case 45:
				case 58:
				{
					visitor.OnBeq(instruction);
					return;
				}
				case 46:
				case 59:
				{
					visitor.OnBge(instruction);
					return;
				}
				case 47:
				case 60:
				{
					visitor.OnBgt(instruction);
					return;
				}
				case 48:
				case 61:
				{
					visitor.OnBle(instruction);
					return;
				}
				case 49:
				case 62:
				{
					visitor.OnBlt(instruction);
					return;
				}
				case 50:
				case 63:
				{
					visitor.OnBne_Un(instruction);
					return;
				}
				case 51:
				case 64:
				{
					visitor.OnBge_Un(instruction);
					return;
				}
				case 52:
				case 65:
				{
					visitor.OnBgt_Un(instruction);
					return;
				}
				case 53:
				case 66:
				{
					visitor.OnBle_Un(instruction);
					return;
				}
				case 54:
				case 67:
				{
					visitor.OnBlt_Un(instruction);
					return;
				}
				case 68:
				{
					visitor.OnSwitch(instruction);
					return;
				}
				case 69:
				{
					visitor.OnLdind_I1(instruction);
					return;
				}
				case 70:
				{
					visitor.OnLdind_U1(instruction);
					return;
				}
				case 71:
				{
					visitor.OnLdind_I2(instruction);
					return;
				}
				case 72:
				{
					visitor.OnLdind_U2(instruction);
					return;
				}
				case 73:
				{
					visitor.OnLdind_I4(instruction);
					return;
				}
				case 74:
				{
					visitor.OnLdind_U4(instruction);
					return;
				}
				case 75:
				{
					visitor.OnLdind_I8(instruction);
					return;
				}
				case 76:
				{
					visitor.OnLdind_I(instruction);
					return;
				}
				case 77:
				{
					visitor.OnLdind_R4(instruction);
					return;
				}
				case 78:
				{
					visitor.OnLdind_R8(instruction);
					return;
				}
				case 79:
				{
					visitor.OnLdind_Ref(instruction);
					return;
				}
				case 80:
				{
					visitor.OnStind_Ref(instruction);
					return;
				}
				case 81:
				{
					visitor.OnStind_I1(instruction);
					return;
				}
				case 82:
				{
					visitor.OnStind_I2(instruction);
					return;
				}
				case 83:
				{
					visitor.OnStind_I4(instruction);
					return;
				}
				case 84:
				{
					visitor.OnStind_I8(instruction);
					return;
				}
				case 85:
				{
					visitor.OnStind_R4(instruction);
					return;
				}
				case 86:
				{
					visitor.OnStind_R8(instruction);
					return;
				}
				case 87:
				{
					visitor.OnAdd(instruction);
					return;
				}
				case 88:
				{
					visitor.OnSub(instruction);
					return;
				}
				case 89:
				{
					visitor.OnMul(instruction);
					return;
				}
				case 90:
				{
					visitor.OnDiv(instruction);
					return;
				}
				case 91:
				{
					visitor.OnDiv_Un(instruction);
					return;
				}
				case 92:
				{
					visitor.OnRem(instruction);
					return;
				}
				case 93:
				{
					visitor.OnRem_Un(instruction);
					return;
				}
				case 94:
				{
					visitor.OnAnd(instruction);
					return;
				}
				case 95:
				{
					visitor.OnOr(instruction);
					return;
				}
				case 96:
				{
					visitor.OnXor(instruction);
					return;
				}
				case 97:
				{
					visitor.OnShl(instruction);
					return;
				}
				case 98:
				{
					visitor.OnShr(instruction);
					return;
				}
				case 99:
				{
					visitor.OnShr_Un(instruction);
					return;
				}
				case 100:
				{
					visitor.OnNeg(instruction);
					return;
				}
				case 101:
				{
					visitor.OnNot(instruction);
					return;
				}
				case 102:
				{
					visitor.OnConv_I1(instruction);
					return;
				}
				case 103:
				{
					visitor.OnConv_I2(instruction);
					return;
				}
				case 104:
				{
					visitor.OnConv_I4(instruction);
					return;
				}
				case 105:
				{
					visitor.OnConv_I8(instruction);
					return;
				}
				case 106:
				{
					visitor.OnConv_R4(instruction);
					return;
				}
				case 107:
				{
					visitor.OnConv_R8(instruction);
					return;
				}
				case 108:
				{
					visitor.OnConv_U4(instruction);
					return;
				}
				case 109:
				{
					visitor.OnConv_U8(instruction);
					return;
				}
				case 110:
				{
					visitor.OnCallvirt(instruction);
					return;
				}
				case 111:
				{
					visitor.OnCpobj(instruction);
					return;
				}
				case 112:
				{
					visitor.OnLdobj(instruction);
					return;
				}
				case 113:
				{
					visitor.OnLdstr(instruction);
					return;
				}
				case 114:
				{
					visitor.OnNewobj(instruction);
					return;
				}
				case 115:
				{
					visitor.OnCastclass(instruction);
					return;
				}
				case 116:
				{
					visitor.OnIsinst(instruction);
					return;
				}
				case 117:
				{
					visitor.OnConv_R_Un(instruction);
					return;
				}
				case 118:
				{
					visitor.OnUnbox(instruction);
					return;
				}
				case 119:
				{
					visitor.OnThrow(instruction);
					return;
				}
				case 120:
				{
					visitor.OnLdfld(instruction);
					return;
				}
				case 121:
				{
					visitor.OnLdflda(instruction);
					return;
				}
				case 122:
				{
					visitor.OnStfld(instruction);
					return;
				}
				case 123:
				{
					visitor.OnLdsfld(instruction);
					return;
				}
				case 124:
				{
					visitor.OnLdsflda(instruction);
					return;
				}
				case 125:
				{
					visitor.OnStsfld(instruction);
					return;
				}
				case 126:
				{
					visitor.OnStobj(instruction);
					return;
				}
				case 127:
				{
					visitor.OnConv_Ovf_I1_Un(instruction);
					return;
				}
				case 128:
				{
					visitor.OnConv_Ovf_I2_Un(instruction);
					return;
				}
				case 129:
				{
					visitor.OnConv_Ovf_I4_Un(instruction);
					return;
				}
				case 130:
				{
					visitor.OnConv_Ovf_I8_Un(instruction);
					return;
				}
				case 131:
				{
					visitor.OnConv_Ovf_U1_Un(instruction);
					return;
				}
				case 132:
				{
					visitor.OnConv_Ovf_U2_Un(instruction);
					return;
				}
				case 133:
				{
					visitor.OnConv_Ovf_U4_Un(instruction);
					return;
				}
				case 134:
				{
					visitor.OnConv_Ovf_U8_Un(instruction);
					return;
				}
				case 135:
				{
					visitor.OnConv_Ovf_I_Un(instruction);
					return;
				}
				case 136:
				{
					visitor.OnConv_Ovf_U_Un(instruction);
					return;
				}
				case 137:
				{
					visitor.OnBox(instruction);
					return;
				}
				case 138:
				{
					visitor.OnNewarr(instruction);
					return;
				}
				case 139:
				{
					visitor.OnLdlen(instruction);
					return;
				}
				case 140:
				{
					visitor.OnLdelema(instruction);
					return;
				}
				case 141:
				{
					visitor.OnLdelem_I1(instruction);
					return;
				}
				case 142:
				{
					visitor.OnLdelem_U1(instruction);
					return;
				}
				case 143:
				{
					visitor.OnLdelem_I2(instruction);
					return;
				}
				case 144:
				{
					visitor.OnLdelem_U2(instruction);
					return;
				}
				case 145:
				{
					visitor.OnLdelem_I4(instruction);
					return;
				}
				case 146:
				{
					visitor.OnLdelem_U4(instruction);
					return;
				}
				case 147:
				{
					visitor.OnLdelem_I8(instruction);
					return;
				}
				case 148:
				{
					visitor.OnLdelem_I(instruction);
					return;
				}
				case 149:
				{
					visitor.OnLdelem_R4(instruction);
					return;
				}
				case 150:
				{
					visitor.OnLdelem_R8(instruction);
					return;
				}
				case 151:
				{
					visitor.OnLdelem_Ref(instruction);
					return;
				}
				case 152:
				{
					visitor.OnStelem_I(instruction);
					return;
				}
				case 153:
				{
					visitor.OnStelem_I1(instruction);
					return;
				}
				case 154:
				{
					visitor.OnStelem_I2(instruction);
					return;
				}
				case 155:
				{
					visitor.OnStelem_I4(instruction);
					return;
				}
				case 156:
				{
					visitor.OnStelem_I8(instruction);
					return;
				}
				case 157:
				{
					visitor.OnStelem_R4(instruction);
					return;
				}
				case 158:
				{
					visitor.OnStelem_R8(instruction);
					return;
				}
				case 159:
				{
					visitor.OnStelem_Ref(instruction);
					return;
				}
				case 160:
				{
					visitor.OnLdelem_Any(instruction);
					return;
				}
				case 161:
				{
					visitor.OnStelem_Any(instruction);
					return;
				}
				case 162:
				{
					visitor.OnUnbox_Any(instruction);
					return;
				}
				case 163:
				{
					visitor.OnConv_Ovf_I1(instruction);
					return;
				}
				case 164:
				{
					visitor.OnConv_Ovf_U1(instruction);
					return;
				}
				case 165:
				{
					visitor.OnConv_Ovf_I2(instruction);
					return;
				}
				case 166:
				{
					visitor.OnConv_Ovf_U2(instruction);
					return;
				}
				case 167:
				{
					visitor.OnConv_Ovf_I4(instruction);
					return;
				}
				case 168:
				{
					visitor.OnConv_Ovf_U4(instruction);
					return;
				}
				case 169:
				{
					visitor.OnConv_Ovf_I8(instruction);
					return;
				}
				case 170:
				{
					visitor.OnConv_Ovf_U8(instruction);
					return;
				}
				case 171:
				{
					visitor.OnRefanyval(instruction);
					return;
				}
				case 172:
				{
					visitor.OnCkfinite(instruction);
					return;
				}
				case 173:
				{
					visitor.OnMkrefany(instruction);
					return;
				}
				case 174:
				{
					visitor.OnLdtoken(instruction);
					return;
				}
				case 175:
				{
					visitor.OnConv_U2(instruction);
					return;
				}
				case 176:
				{
					visitor.OnConv_U1(instruction);
					return;
				}
				case 177:
				{
					visitor.OnConv_I(instruction);
					return;
				}
				case 178:
				{
					visitor.OnConv_Ovf_I(instruction);
					return;
				}
				case 179:
				{
					visitor.OnConv_Ovf_U(instruction);
					return;
				}
				case 180:
				{
					visitor.OnAdd_Ovf(instruction);
					return;
				}
				case 181:
				{
					visitor.OnAdd_Ovf_Un(instruction);
					return;
				}
				case 182:
				{
					visitor.OnMul_Ovf(instruction);
					return;
				}
				case 183:
				{
					visitor.OnMul_Ovf_Un(instruction);
					return;
				}
				case 184:
				{
					visitor.OnSub_Ovf(instruction);
					return;
				}
				case 185:
				{
					visitor.OnSub_Ovf_Un(instruction);
					return;
				}
				case 186:
				{
					visitor.OnEndfinally(instruction);
					return;
				}
				case 187:
				case 188:
				{
					visitor.OnLeave(instruction);
					return;
				}
				case 189:
				{
					visitor.OnStind_I(instruction);
					return;
				}
				case 190:
				{
					visitor.OnConv_U(instruction);
					return;
				}
				case 191:
				{
					visitor.OnArglist(instruction);
					return;
				}
				case 192:
				{
					visitor.OnCeq(instruction);
					return;
				}
				case 193:
				{
					visitor.OnCgt(instruction);
					return;
				}
				case 194:
				{
					visitor.OnCgt_Un(instruction);
					return;
				}
				case 195:
				{
					visitor.OnClt(instruction);
					return;
				}
				case 196:
				{
					visitor.OnClt_Un(instruction);
					return;
				}
				case 197:
				{
					visitor.OnLdftn(instruction);
					return;
				}
				case 198:
				{
					visitor.OnLdvirtftn(instruction);
					return;
				}
				case 205:
				{
					visitor.OnLocalloc(instruction);
					return;
				}
				case 206:
				{
					visitor.OnEndfilter(instruction);
					return;
				}
				case 207:
				{
					visitor.OnUnaligned(instruction);
					return;
				}
				case 208:
				{
					visitor.OnVolatile(instruction);
					return;
				}
				case 209:
				{
					visitor.OnTail(instruction);
					return;
				}
				case 210:
				{
					visitor.OnInitobj(instruction);
					return;
				}
				case 211:
				{
					visitor.OnConstrained(instruction);
					return;
				}
				case 212:
				{
					visitor.OnCpblk(instruction);
					return;
				}
				case 213:
				{
					visitor.OnInitblk(instruction);
					return;
				}
				case 214:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction), "instruction");
				}
				case 215:
				{
					visitor.OnRethrow(instruction);
					return;
				}
				case 216:
				{
					visitor.OnSizeof(instruction);
					return;
				}
				case 217:
				{
					visitor.OnRefanytype(instruction);
					return;
				}
				case 218:
				{
					return;
				}
				default:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction), "instruction");
				}
			}
		}
	}
}