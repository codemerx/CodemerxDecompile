using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
			stackVariable1 = new OpCodeInfo[209];
			stackVariable4 = new OpCodeInfo(OpCodes.Add);
			stackVariable4.set_CanThrow(false);
			stackVariable1[0] = stackVariable4;
			stackVariable8 = new OpCodeInfo(OpCodes.Add_Ovf);
			stackVariable8.set_CanThrow(true);
			stackVariable1[1] = stackVariable8;
			stackVariable12 = new OpCodeInfo(OpCodes.Add_Ovf_Un);
			stackVariable12.set_CanThrow(true);
			stackVariable1[2] = stackVariable12;
			stackVariable16 = new OpCodeInfo(OpCodes.And);
			stackVariable16.set_CanThrow(false);
			stackVariable1[3] = stackVariable16;
			stackVariable20 = new OpCodeInfo(OpCodes.Arglist);
			stackVariable20.set_CanThrow(false);
			stackVariable1[4] = stackVariable20;
			stackVariable24 = new OpCodeInfo(OpCodes.Beq);
			stackVariable24.set_CanThrow(false);
			stackVariable1[5] = stackVariable24;
			stackVariable28 = new OpCodeInfo(OpCodes.Beq_S);
			stackVariable28.set_CanThrow(false);
			stackVariable1[6] = stackVariable28;
			stackVariable32 = new OpCodeInfo(OpCodes.Bge);
			stackVariable32.set_CanThrow(false);
			stackVariable1[7] = stackVariable32;
			stackVariable36 = new OpCodeInfo(OpCodes.Bge_S);
			stackVariable36.set_CanThrow(false);
			stackVariable1[8] = stackVariable36;
			stackVariable40 = new OpCodeInfo(OpCodes.Bge_Un);
			stackVariable40.set_CanThrow(false);
			stackVariable1[9] = stackVariable40;
			stackVariable44 = new OpCodeInfo(OpCodes.Bge_Un_S);
			stackVariable44.set_CanThrow(false);
			stackVariable1[10] = stackVariable44;
			stackVariable48 = new OpCodeInfo(OpCodes.Bgt);
			stackVariable48.set_CanThrow(false);
			stackVariable1[11] = stackVariable48;
			stackVariable52 = new OpCodeInfo(OpCodes.Bgt_S);
			stackVariable52.set_CanThrow(false);
			stackVariable1[12] = stackVariable52;
			stackVariable56 = new OpCodeInfo(OpCodes.Bgt_Un);
			stackVariable56.set_CanThrow(false);
			stackVariable1[13] = stackVariable56;
			stackVariable60 = new OpCodeInfo(OpCodes.Bgt_Un_S);
			stackVariable60.set_CanThrow(false);
			stackVariable1[14] = stackVariable60;
			stackVariable64 = new OpCodeInfo(OpCodes.Ble);
			stackVariable64.set_CanThrow(false);
			stackVariable1[15] = stackVariable64;
			stackVariable68 = new OpCodeInfo(OpCodes.Ble_S);
			stackVariable68.set_CanThrow(false);
			stackVariable1[16] = stackVariable68;
			stackVariable72 = new OpCodeInfo(OpCodes.Ble_Un);
			stackVariable72.set_CanThrow(false);
			stackVariable1[17] = stackVariable72;
			stackVariable76 = new OpCodeInfo(OpCodes.Ble_Un_S);
			stackVariable76.set_CanThrow(false);
			stackVariable1[18] = stackVariable76;
			stackVariable80 = new OpCodeInfo(OpCodes.Blt);
			stackVariable80.set_CanThrow(false);
			stackVariable1[19] = stackVariable80;
			stackVariable84 = new OpCodeInfo(OpCodes.Blt_S);
			stackVariable84.set_CanThrow(false);
			stackVariable1[20] = stackVariable84;
			stackVariable88 = new OpCodeInfo(OpCodes.Blt_Un);
			stackVariable88.set_CanThrow(false);
			stackVariable1[21] = stackVariable88;
			stackVariable92 = new OpCodeInfo(OpCodes.Blt_Un_S);
			stackVariable92.set_CanThrow(false);
			stackVariable1[22] = stackVariable92;
			stackVariable96 = new OpCodeInfo(OpCodes.Bne_Un);
			stackVariable96.set_CanThrow(false);
			stackVariable1[23] = stackVariable96;
			stackVariable100 = new OpCodeInfo(OpCodes.Bne_Un_S);
			stackVariable100.set_CanThrow(false);
			stackVariable1[24] = stackVariable100;
			stackVariable104 = new OpCodeInfo(OpCodes.Br);
			stackVariable104.set_CanThrow(false);
			stackVariable1[25] = stackVariable104;
			stackVariable108 = new OpCodeInfo(OpCodes.Br_S);
			stackVariable108.set_CanThrow(false);
			stackVariable1[26] = stackVariable108;
			stackVariable112 = new OpCodeInfo(OpCodes.Break);
			stackVariable112.set_CanThrow(true);
			stackVariable1[27] = stackVariable112;
			stackVariable116 = new OpCodeInfo(OpCodes.Brfalse);
			stackVariable116.set_CanThrow(false);
			stackVariable1[28] = stackVariable116;
			stackVariable120 = new OpCodeInfo(OpCodes.Brfalse_S);
			stackVariable120.set_CanThrow(false);
			stackVariable1[29] = stackVariable120;
			stackVariable124 = new OpCodeInfo(OpCodes.Brtrue);
			stackVariable124.set_CanThrow(false);
			stackVariable1[30] = stackVariable124;
			stackVariable128 = new OpCodeInfo(OpCodes.Brtrue_S);
			stackVariable128.set_CanThrow(false);
			stackVariable1[31] = stackVariable128;
			stackVariable132 = new OpCodeInfo(OpCodes.Call);
			stackVariable132.set_CanThrow(true);
			stackVariable1[32] = stackVariable132;
			stackVariable136 = new OpCodeInfo(OpCodes.Calli);
			stackVariable136.set_CanThrow(true);
			stackVariable1[33] = stackVariable136;
			stackVariable140 = new OpCodeInfo(OpCodes.Ceq);
			stackVariable140.set_CanThrow(false);
			stackVariable1[34] = stackVariable140;
			stackVariable144 = new OpCodeInfo(OpCodes.Cgt);
			stackVariable144.set_CanThrow(false);
			stackVariable1[35] = stackVariable144;
			stackVariable148 = new OpCodeInfo(OpCodes.Cgt_Un);
			stackVariable148.set_CanThrow(false);
			stackVariable1[36] = stackVariable148;
			stackVariable152 = new OpCodeInfo(OpCodes.Ckfinite);
			stackVariable152.set_CanThrow(true);
			stackVariable1[37] = stackVariable152;
			stackVariable156 = new OpCodeInfo(OpCodes.Clt);
			stackVariable156.set_CanThrow(false);
			stackVariable1[38] = stackVariable156;
			stackVariable160 = new OpCodeInfo(OpCodes.Clt_Un);
			stackVariable160.set_CanThrow(false);
			stackVariable1[39] = stackVariable160;
			stackVariable164 = new OpCodeInfo(OpCodes.Conv_I1);
			stackVariable164.set_CanThrow(false);
			stackVariable1[40] = stackVariable164;
			stackVariable168 = new OpCodeInfo(OpCodes.Conv_I2);
			stackVariable168.set_CanThrow(false);
			stackVariable1[41] = stackVariable168;
			stackVariable172 = new OpCodeInfo(OpCodes.Conv_I4);
			stackVariable172.set_CanThrow(false);
			stackVariable1[42] = stackVariable172;
			stackVariable176 = new OpCodeInfo(OpCodes.Conv_I8);
			stackVariable176.set_CanThrow(false);
			stackVariable1[43] = stackVariable176;
			stackVariable180 = new OpCodeInfo(OpCodes.Conv_R4);
			stackVariable180.set_CanThrow(false);
			stackVariable1[44] = stackVariable180;
			stackVariable184 = new OpCodeInfo(OpCodes.Conv_R8);
			stackVariable184.set_CanThrow(false);
			stackVariable1[45] = stackVariable184;
			stackVariable188 = new OpCodeInfo(OpCodes.Conv_U1);
			stackVariable188.set_CanThrow(false);
			stackVariable1[46] = stackVariable188;
			stackVariable192 = new OpCodeInfo(OpCodes.Conv_U2);
			stackVariable192.set_CanThrow(false);
			stackVariable1[47] = stackVariable192;
			stackVariable196 = new OpCodeInfo(OpCodes.Conv_U4);
			stackVariable196.set_CanThrow(false);
			stackVariable1[48] = stackVariable196;
			stackVariable200 = new OpCodeInfo(OpCodes.Conv_U8);
			stackVariable200.set_CanThrow(false);
			stackVariable1[49] = stackVariable200;
			stackVariable204 = new OpCodeInfo(OpCodes.Conv_I);
			stackVariable204.set_CanThrow(false);
			stackVariable1[50] = stackVariable204;
			stackVariable208 = new OpCodeInfo(OpCodes.Conv_U);
			stackVariable208.set_CanThrow(false);
			stackVariable1[51] = stackVariable208;
			stackVariable212 = new OpCodeInfo(OpCodes.Conv_R_Un);
			stackVariable212.set_CanThrow(false);
			stackVariable1[52] = stackVariable212;
			stackVariable216 = new OpCodeInfo(OpCodes.Conv_Ovf_I1);
			stackVariable216.set_CanThrow(true);
			stackVariable1[53] = stackVariable216;
			stackVariable220 = new OpCodeInfo(OpCodes.Conv_Ovf_I2);
			stackVariable220.set_CanThrow(true);
			stackVariable1[54] = stackVariable220;
			stackVariable224 = new OpCodeInfo(OpCodes.Conv_Ovf_I4);
			stackVariable224.set_CanThrow(true);
			stackVariable1[55] = stackVariable224;
			stackVariable228 = new OpCodeInfo(OpCodes.Conv_Ovf_I8);
			stackVariable228.set_CanThrow(true);
			stackVariable1[56] = stackVariable228;
			stackVariable232 = new OpCodeInfo(OpCodes.Conv_Ovf_U1);
			stackVariable232.set_CanThrow(true);
			stackVariable1[57] = stackVariable232;
			stackVariable236 = new OpCodeInfo(OpCodes.Conv_Ovf_U2);
			stackVariable236.set_CanThrow(true);
			stackVariable1[58] = stackVariable236;
			stackVariable240 = new OpCodeInfo(OpCodes.Conv_Ovf_U4);
			stackVariable240.set_CanThrow(true);
			stackVariable1[59] = stackVariable240;
			stackVariable244 = new OpCodeInfo(OpCodes.Conv_Ovf_U8);
			stackVariable244.set_CanThrow(true);
			stackVariable1[60] = stackVariable244;
			stackVariable248 = new OpCodeInfo(OpCodes.Conv_Ovf_I);
			stackVariable248.set_CanThrow(true);
			stackVariable1[61] = stackVariable248;
			stackVariable252 = new OpCodeInfo(OpCodes.Conv_Ovf_U);
			stackVariable252.set_CanThrow(true);
			stackVariable1[62] = stackVariable252;
			stackVariable256 = new OpCodeInfo(OpCodes.Conv_Ovf_I1_Un);
			stackVariable256.set_CanThrow(true);
			stackVariable1[63] = stackVariable256;
			stackVariable260 = new OpCodeInfo(OpCodes.Conv_Ovf_I2_Un);
			stackVariable260.set_CanThrow(true);
			stackVariable1[64] = stackVariable260;
			stackVariable264 = new OpCodeInfo(OpCodes.Conv_Ovf_I4_Un);
			stackVariable264.set_CanThrow(true);
			stackVariable1[65] = stackVariable264;
			stackVariable268 = new OpCodeInfo(OpCodes.Conv_Ovf_I8_Un);
			stackVariable268.set_CanThrow(true);
			stackVariable1[66] = stackVariable268;
			stackVariable272 = new OpCodeInfo(OpCodes.Conv_Ovf_U1_Un);
			stackVariable272.set_CanThrow(true);
			stackVariable1[67] = stackVariable272;
			stackVariable276 = new OpCodeInfo(OpCodes.Conv_Ovf_U2_Un);
			stackVariable276.set_CanThrow(true);
			stackVariable1[68] = stackVariable276;
			stackVariable280 = new OpCodeInfo(OpCodes.Conv_Ovf_U4_Un);
			stackVariable280.set_CanThrow(true);
			stackVariable1[69] = stackVariable280;
			stackVariable284 = new OpCodeInfo(OpCodes.Conv_Ovf_U8_Un);
			stackVariable284.set_CanThrow(true);
			stackVariable1[70] = stackVariable284;
			stackVariable288 = new OpCodeInfo(OpCodes.Conv_Ovf_I_Un);
			stackVariable288.set_CanThrow(true);
			stackVariable1[71] = stackVariable288;
			stackVariable292 = new OpCodeInfo(OpCodes.Conv_Ovf_U_Un);
			stackVariable292.set_CanThrow(true);
			stackVariable1[72] = stackVariable292;
			stackVariable296 = new OpCodeInfo(OpCodes.Div);
			stackVariable296.set_CanThrow(true);
			stackVariable1[73] = stackVariable296;
			stackVariable300 = new OpCodeInfo(OpCodes.Div_Un);
			stackVariable300.set_CanThrow(true);
			stackVariable1[74] = stackVariable300;
			stackVariable304 = new OpCodeInfo(OpCodes.Dup);
			stackVariable304.set_CanThrow(true);
			stackVariable304.set_IsMoveInstruction(true);
			stackVariable1[75] = stackVariable304;
			stackVariable309 = new OpCodeInfo(OpCodes.Endfilter);
			stackVariable309.set_CanThrow(false);
			stackVariable1[76] = stackVariable309;
			stackVariable313 = new OpCodeInfo(OpCodes.Endfinally);
			stackVariable313.set_CanThrow(false);
			stackVariable1[77] = stackVariable313;
			stackVariable317 = new OpCodeInfo(OpCodes.Ldarg);
			stackVariable317.set_CanThrow(false);
			stackVariable317.set_IsMoveInstruction(true);
			stackVariable1[78] = stackVariable317;
			stackVariable322 = new OpCodeInfo(OpCodes.Ldarg_0);
			stackVariable322.set_CanThrow(false);
			stackVariable322.set_IsMoveInstruction(true);
			stackVariable1[79] = stackVariable322;
			stackVariable327 = new OpCodeInfo(OpCodes.Ldarg_1);
			stackVariable327.set_CanThrow(false);
			stackVariable327.set_IsMoveInstruction(true);
			stackVariable1[80] = stackVariable327;
			stackVariable332 = new OpCodeInfo(OpCodes.Ldarg_2);
			stackVariable332.set_CanThrow(false);
			stackVariable332.set_IsMoveInstruction(true);
			stackVariable1[81] = stackVariable332;
			stackVariable337 = new OpCodeInfo(OpCodes.Ldarg_3);
			stackVariable337.set_CanThrow(false);
			stackVariable337.set_IsMoveInstruction(true);
			stackVariable1[82] = stackVariable337;
			stackVariable342 = new OpCodeInfo(OpCodes.Ldarg_S);
			stackVariable342.set_CanThrow(false);
			stackVariable342.set_IsMoveInstruction(true);
			stackVariable1[83] = stackVariable342;
			stackVariable347 = new OpCodeInfo(OpCodes.Ldarga);
			stackVariable347.set_CanThrow(false);
			stackVariable1[84] = stackVariable347;
			stackVariable351 = new OpCodeInfo(OpCodes.Ldarga_S);
			stackVariable351.set_CanThrow(false);
			stackVariable1[85] = stackVariable351;
			stackVariable355 = new OpCodeInfo(OpCodes.Ldc_I4);
			stackVariable355.set_CanThrow(false);
			stackVariable1[86] = stackVariable355;
			stackVariable359 = new OpCodeInfo(OpCodes.Ldc_I4_M1);
			stackVariable359.set_CanThrow(false);
			stackVariable1[87] = stackVariable359;
			stackVariable363 = new OpCodeInfo(OpCodes.Ldc_I4_0);
			stackVariable363.set_CanThrow(false);
			stackVariable1[88] = stackVariable363;
			stackVariable367 = new OpCodeInfo(OpCodes.Ldc_I4_1);
			stackVariable367.set_CanThrow(false);
			stackVariable1[89] = stackVariable367;
			stackVariable371 = new OpCodeInfo(OpCodes.Ldc_I4_2);
			stackVariable371.set_CanThrow(false);
			stackVariable1[90] = stackVariable371;
			stackVariable375 = new OpCodeInfo(OpCodes.Ldc_I4_3);
			stackVariable375.set_CanThrow(false);
			stackVariable1[91] = stackVariable375;
			stackVariable379 = new OpCodeInfo(OpCodes.Ldc_I4_4);
			stackVariable379.set_CanThrow(false);
			stackVariable1[92] = stackVariable379;
			stackVariable383 = new OpCodeInfo(OpCodes.Ldc_I4_5);
			stackVariable383.set_CanThrow(false);
			stackVariable1[93] = stackVariable383;
			stackVariable387 = new OpCodeInfo(OpCodes.Ldc_I4_6);
			stackVariable387.set_CanThrow(false);
			stackVariable1[94] = stackVariable387;
			stackVariable391 = new OpCodeInfo(OpCodes.Ldc_I4_7);
			stackVariable391.set_CanThrow(false);
			stackVariable1[95] = stackVariable391;
			stackVariable395 = new OpCodeInfo(OpCodes.Ldc_I4_8);
			stackVariable395.set_CanThrow(false);
			stackVariable1[96] = stackVariable395;
			stackVariable399 = new OpCodeInfo(OpCodes.Ldc_I4_S);
			stackVariable399.set_CanThrow(false);
			stackVariable1[97] = stackVariable399;
			stackVariable403 = new OpCodeInfo(OpCodes.Ldc_I8);
			stackVariable403.set_CanThrow(false);
			stackVariable1[98] = stackVariable403;
			stackVariable407 = new OpCodeInfo(OpCodes.Ldc_R4);
			stackVariable407.set_CanThrow(false);
			stackVariable1[99] = stackVariable407;
			stackVariable411 = new OpCodeInfo(OpCodes.Ldc_R8);
			stackVariable411.set_CanThrow(false);
			stackVariable1[100] = stackVariable411;
			stackVariable415 = new OpCodeInfo(OpCodes.Ldftn);
			stackVariable415.set_CanThrow(false);
			stackVariable1[101] = stackVariable415;
			stackVariable419 = new OpCodeInfo(OpCodes.Ldind_I1);
			stackVariable419.set_CanThrow(true);
			stackVariable1[102] = stackVariable419;
			stackVariable423 = new OpCodeInfo(OpCodes.Ldind_I2);
			stackVariable423.set_CanThrow(true);
			stackVariable1[103] = stackVariable423;
			stackVariable427 = new OpCodeInfo(OpCodes.Ldind_I4);
			stackVariable427.set_CanThrow(true);
			stackVariable1[104] = stackVariable427;
			stackVariable431 = new OpCodeInfo(OpCodes.Ldind_I8);
			stackVariable431.set_CanThrow(true);
			stackVariable1[105] = stackVariable431;
			stackVariable435 = new OpCodeInfo(OpCodes.Ldind_U1);
			stackVariable435.set_CanThrow(true);
			stackVariable1[106] = stackVariable435;
			stackVariable439 = new OpCodeInfo(OpCodes.Ldind_U2);
			stackVariable439.set_CanThrow(true);
			stackVariable1[107] = stackVariable439;
			stackVariable443 = new OpCodeInfo(OpCodes.Ldind_U4);
			stackVariable443.set_CanThrow(true);
			stackVariable1[108] = stackVariable443;
			stackVariable447 = new OpCodeInfo(OpCodes.Ldind_R4);
			stackVariable447.set_CanThrow(true);
			stackVariable1[109] = stackVariable447;
			stackVariable451 = new OpCodeInfo(OpCodes.Ldind_R8);
			stackVariable451.set_CanThrow(true);
			stackVariable1[110] = stackVariable451;
			stackVariable455 = new OpCodeInfo(OpCodes.Ldind_I);
			stackVariable455.set_CanThrow(true);
			stackVariable1[111] = stackVariable455;
			stackVariable459 = new OpCodeInfo(OpCodes.Ldind_Ref);
			stackVariable459.set_CanThrow(true);
			stackVariable1[112] = stackVariable459;
			stackVariable463 = new OpCodeInfo(OpCodes.Ldloc);
			stackVariable463.set_CanThrow(false);
			stackVariable463.set_IsMoveInstruction(true);
			stackVariable1[113] = stackVariable463;
			stackVariable468 = new OpCodeInfo(OpCodes.Ldloc_0);
			stackVariable468.set_CanThrow(false);
			stackVariable468.set_IsMoveInstruction(true);
			stackVariable1[114] = stackVariable468;
			stackVariable473 = new OpCodeInfo(OpCodes.Ldloc_1);
			stackVariable473.set_CanThrow(false);
			stackVariable473.set_IsMoveInstruction(true);
			stackVariable1[115] = stackVariable473;
			stackVariable478 = new OpCodeInfo(OpCodes.Ldloc_2);
			stackVariable478.set_CanThrow(false);
			stackVariable478.set_IsMoveInstruction(true);
			stackVariable1[116] = stackVariable478;
			stackVariable483 = new OpCodeInfo(OpCodes.Ldloc_3);
			stackVariable483.set_CanThrow(false);
			stackVariable483.set_IsMoveInstruction(true);
			stackVariable1[117] = stackVariable483;
			stackVariable488 = new OpCodeInfo(OpCodes.Ldloc_S);
			stackVariable488.set_CanThrow(false);
			stackVariable488.set_IsMoveInstruction(true);
			stackVariable1[118] = stackVariable488;
			stackVariable493 = new OpCodeInfo(OpCodes.Ldloca);
			stackVariable493.set_CanThrow(false);
			stackVariable1[119] = stackVariable493;
			stackVariable497 = new OpCodeInfo(OpCodes.Ldloca_S);
			stackVariable497.set_CanThrow(false);
			stackVariable1[120] = stackVariable497;
			stackVariable501 = new OpCodeInfo(OpCodes.Ldnull);
			stackVariable501.set_CanThrow(false);
			stackVariable1[121] = stackVariable501;
			stackVariable505 = new OpCodeInfo(OpCodes.Leave);
			stackVariable505.set_CanThrow(false);
			stackVariable1[122] = stackVariable505;
			stackVariable509 = new OpCodeInfo(OpCodes.Leave_S);
			stackVariable509.set_CanThrow(false);
			stackVariable1[123] = stackVariable509;
			stackVariable513 = new OpCodeInfo(OpCodes.Localloc);
			stackVariable513.set_CanThrow(true);
			stackVariable1[124] = stackVariable513;
			stackVariable517 = new OpCodeInfo(OpCodes.Mul);
			stackVariable517.set_CanThrow(false);
			stackVariable1[125] = stackVariable517;
			stackVariable521 = new OpCodeInfo(OpCodes.Mul_Ovf);
			stackVariable521.set_CanThrow(true);
			stackVariable1[126] = stackVariable521;
			stackVariable525 = new OpCodeInfo(OpCodes.Mul_Ovf_Un);
			stackVariable525.set_CanThrow(true);
			stackVariable1[127] = stackVariable525;
			stackVariable529 = new OpCodeInfo(OpCodes.Neg);
			stackVariable529.set_CanThrow(false);
			stackVariable1[128] = stackVariable529;
			stackVariable533 = new OpCodeInfo(OpCodes.Nop);
			stackVariable533.set_CanThrow(false);
			stackVariable1[129] = stackVariable533;
			stackVariable537 = new OpCodeInfo(OpCodes.Not);
			stackVariable537.set_CanThrow(false);
			stackVariable1[130] = stackVariable537;
			stackVariable541 = new OpCodeInfo(OpCodes.Or);
			stackVariable541.set_CanThrow(false);
			stackVariable1[131] = stackVariable541;
			stackVariable545 = new OpCodeInfo(OpCodes.Pop);
			stackVariable545.set_CanThrow(false);
			stackVariable1[132] = stackVariable545;
			stackVariable549 = new OpCodeInfo(OpCodes.Rem);
			stackVariable549.set_CanThrow(true);
			stackVariable1[133] = stackVariable549;
			stackVariable553 = new OpCodeInfo(OpCodes.Rem_Un);
			stackVariable553.set_CanThrow(true);
			stackVariable1[134] = stackVariable553;
			stackVariable557 = new OpCodeInfo(OpCodes.Ret);
			stackVariable557.set_CanThrow(false);
			stackVariable1[135] = stackVariable557;
			stackVariable561 = new OpCodeInfo(OpCodes.Shl);
			stackVariable561.set_CanThrow(false);
			stackVariable1[136] = stackVariable561;
			stackVariable565 = new OpCodeInfo(OpCodes.Shr);
			stackVariable565.set_CanThrow(false);
			stackVariable1[137] = stackVariable565;
			stackVariable569 = new OpCodeInfo(OpCodes.Shr_Un);
			stackVariable569.set_CanThrow(false);
			stackVariable1[138] = stackVariable569;
			stackVariable573 = new OpCodeInfo(OpCodes.Starg);
			stackVariable573.set_CanThrow(false);
			stackVariable573.set_IsMoveInstruction(true);
			stackVariable1[139] = stackVariable573;
			stackVariable578 = new OpCodeInfo(OpCodes.Starg_S);
			stackVariable578.set_CanThrow(false);
			stackVariable578.set_IsMoveInstruction(true);
			stackVariable1[140] = stackVariable578;
			stackVariable583 = new OpCodeInfo(OpCodes.Stind_I1);
			stackVariable583.set_CanThrow(true);
			stackVariable1[141] = stackVariable583;
			stackVariable587 = new OpCodeInfo(OpCodes.Stind_I2);
			stackVariable587.set_CanThrow(true);
			stackVariable1[142] = stackVariable587;
			stackVariable591 = new OpCodeInfo(OpCodes.Stind_I4);
			stackVariable591.set_CanThrow(true);
			stackVariable1[143] = stackVariable591;
			stackVariable595 = new OpCodeInfo(OpCodes.Stind_I8);
			stackVariable595.set_CanThrow(true);
			stackVariable1[144] = stackVariable595;
			stackVariable599 = new OpCodeInfo(OpCodes.Stind_R4);
			stackVariable599.set_CanThrow(true);
			stackVariable1[145] = stackVariable599;
			stackVariable603 = new OpCodeInfo(OpCodes.Stind_R8);
			stackVariable603.set_CanThrow(true);
			stackVariable1[146] = stackVariable603;
			stackVariable607 = new OpCodeInfo(OpCodes.Stind_I);
			stackVariable607.set_CanThrow(true);
			stackVariable1[147] = stackVariable607;
			stackVariable611 = new OpCodeInfo(OpCodes.Stind_Ref);
			stackVariable611.set_CanThrow(true);
			stackVariable1[148] = stackVariable611;
			stackVariable615 = new OpCodeInfo(OpCodes.Stloc);
			stackVariable615.set_CanThrow(false);
			stackVariable615.set_IsMoveInstruction(true);
			stackVariable1[149] = stackVariable615;
			stackVariable620 = new OpCodeInfo(OpCodes.Stloc_0);
			stackVariable620.set_CanThrow(false);
			stackVariable620.set_IsMoveInstruction(true);
			stackVariable1[150] = stackVariable620;
			stackVariable625 = new OpCodeInfo(OpCodes.Stloc_1);
			stackVariable625.set_CanThrow(false);
			stackVariable625.set_IsMoveInstruction(true);
			stackVariable1[151] = stackVariable625;
			stackVariable630 = new OpCodeInfo(OpCodes.Stloc_2);
			stackVariable630.set_CanThrow(false);
			stackVariable630.set_IsMoveInstruction(true);
			stackVariable1[152] = stackVariable630;
			stackVariable635 = new OpCodeInfo(OpCodes.Stloc_3);
			stackVariable635.set_CanThrow(false);
			stackVariable635.set_IsMoveInstruction(true);
			stackVariable1[153] = stackVariable635;
			stackVariable640 = new OpCodeInfo(OpCodes.Stloc_S);
			stackVariable640.set_CanThrow(false);
			stackVariable640.set_IsMoveInstruction(true);
			stackVariable1[154] = stackVariable640;
			stackVariable645 = new OpCodeInfo(OpCodes.Sub);
			stackVariable645.set_CanThrow(false);
			stackVariable1[155] = stackVariable645;
			stackVariable649 = new OpCodeInfo(OpCodes.Sub_Ovf);
			stackVariable649.set_CanThrow(true);
			stackVariable1[156] = stackVariable649;
			stackVariable653 = new OpCodeInfo(OpCodes.Sub_Ovf_Un);
			stackVariable653.set_CanThrow(true);
			stackVariable1[157] = stackVariable653;
			stackVariable657 = new OpCodeInfo(OpCodes.Switch);
			stackVariable657.set_CanThrow(false);
			stackVariable1[158] = stackVariable657;
			stackVariable661 = new OpCodeInfo(OpCodes.Xor);
			stackVariable661.set_CanThrow(false);
			stackVariable1[159] = stackVariable661;
			stackVariable1[160] = new OpCodeInfo(OpCodes.Box);
			stackVariable1[161] = new OpCodeInfo(OpCodes.Callvirt);
			stackVariable1[162] = new OpCodeInfo(OpCodes.Castclass);
			stackVariable1[163] = new OpCodeInfo(OpCodes.Cpobj);
			stackVariable677 = new OpCodeInfo(OpCodes.Initobj);
			stackVariable677.set_CanThrow(false);
			stackVariable1[164] = stackVariable677;
			stackVariable681 = new OpCodeInfo(OpCodes.Isinst);
			stackVariable681.set_CanThrow(false);
			stackVariable1[165] = stackVariable681;
			stackVariable1[166] = new OpCodeInfo(OpCodes.Ldelem_Any);
			stackVariable1[167] = new OpCodeInfo(OpCodes.Ldelem_I);
			stackVariable1[168] = new OpCodeInfo(OpCodes.Ldelem_I1);
			stackVariable1[169] = new OpCodeInfo(OpCodes.Ldelem_I2);
			stackVariable1[170] = new OpCodeInfo(OpCodes.Ldelem_I4);
			stackVariable1[171] = new OpCodeInfo(OpCodes.Ldelem_I8);
			stackVariable1[172] = new OpCodeInfo(OpCodes.Ldelem_R4);
			stackVariable1[173] = new OpCodeInfo(OpCodes.Ldelem_R8);
			stackVariable1[174] = new OpCodeInfo(OpCodes.Ldelem_Ref);
			stackVariable1[175] = new OpCodeInfo(OpCodes.Ldelem_U1);
			stackVariable1[176] = new OpCodeInfo(OpCodes.Ldelem_U2);
			stackVariable1[177] = new OpCodeInfo(OpCodes.Ldelem_U4);
			stackVariable1[178] = new OpCodeInfo(OpCodes.Ldelema);
			stackVariable1[179] = new OpCodeInfo(OpCodes.Ldfld);
			stackVariable1[180] = new OpCodeInfo(OpCodes.Ldflda);
			stackVariable1[181] = new OpCodeInfo(OpCodes.Ldlen);
			stackVariable1[182] = new OpCodeInfo(OpCodes.Ldobj);
			stackVariable1[183] = new OpCodeInfo(OpCodes.Ldsfld);
			stackVariable1[184] = new OpCodeInfo(OpCodes.Ldsflda);
			stackVariable742 = new OpCodeInfo(OpCodes.Ldstr);
			stackVariable742.set_CanThrow(false);
			stackVariable1[185] = stackVariable742;
			stackVariable746 = new OpCodeInfo(OpCodes.Ldtoken);
			stackVariable746.set_CanThrow(false);
			stackVariable1[186] = stackVariable746;
			stackVariable1[187] = new OpCodeInfo(OpCodes.Ldvirtftn);
			stackVariable1[188] = new OpCodeInfo(OpCodes.Mkrefany);
			stackVariable1[189] = new OpCodeInfo(OpCodes.Newarr);
			stackVariable1[190] = new OpCodeInfo(OpCodes.Newobj);
			stackVariable762 = new OpCodeInfo(OpCodes.Refanytype);
			stackVariable762.set_CanThrow(false);
			stackVariable1[191] = stackVariable762;
			stackVariable1[192] = new OpCodeInfo(OpCodes.Refanyval);
			stackVariable1[193] = new OpCodeInfo(OpCodes.Rethrow);
			stackVariable772 = new OpCodeInfo(OpCodes.Sizeof);
			stackVariable772.set_CanThrow(false);
			stackVariable1[194] = stackVariable772;
			stackVariable1[195] = new OpCodeInfo(OpCodes.Stelem_Any);
			stackVariable1[196] = new OpCodeInfo(OpCodes.Stelem_I1);
			stackVariable1[197] = new OpCodeInfo(OpCodes.Stelem_I2);
			stackVariable1[198] = new OpCodeInfo(OpCodes.Stelem_I4);
			stackVariable1[199] = new OpCodeInfo(OpCodes.Stelem_I8);
			stackVariable1[200] = new OpCodeInfo(OpCodes.Stelem_R4);
			stackVariable1[201] = new OpCodeInfo(OpCodes.Stelem_R8);
			stackVariable1[202] = new OpCodeInfo(OpCodes.Stelem_Ref);
			stackVariable1[203] = new OpCodeInfo(OpCodes.Stfld);
			stackVariable1[204] = new OpCodeInfo(OpCodes.Stobj);
			stackVariable1[205] = new OpCodeInfo(OpCodes.Stsfld);
			stackVariable1[206] = new OpCodeInfo(OpCodes.Throw);
			stackVariable1[207] = new OpCodeInfo(OpCodes.Unbox);
			stackVariable1[208] = new OpCodeInfo(OpCodes.Unbox_Any);
			OpCodeInfo.knownOpCodes = stackVariable1;
			OpCodeInfo.knownOpCodeDict = OpCodeInfo.knownOpCodes.ToDictionary<OpCodeInfo, Code>(new Func<OpCodeInfo, Code>(OpCodeInfo.u003cu003ec.u003cu003e9.u003cu002ecctoru003eb__17_0));
			return;
		}

		private OpCodeInfo(Mono.Cecil.Cil.OpCode opcode)
		{
			base();
			this.opcode = opcode;
			this.set_CanThrow(true);
			return;
		}

		public static OpCodeInfo Get(Mono.Cecil.Cil.OpCode opCode)
		{
			return OpCodeInfo.Get(opCode.get_Code());
		}

		public static OpCodeInfo Get(Code code)
		{
			if (!OpCodeInfo.knownOpCodeDict.TryGetValue(code, out V_0))
			{
				throw new NotSupportedException(code.ToString());
			}
			return V_0;
		}

		public static bool IsUnconditionalBranch(Mono.Cecil.Cil.OpCode opcode)
		{
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
				Label0:
					V_1 = opcode.get_FlowControl();
					throw new NotSupportedException(V_1.ToString());
				}
				case 2:
				case 3:
				case 5:
				{
					return false;
				}
				default:
				{
					goto Label0;
				}
			}
		}
	}
}