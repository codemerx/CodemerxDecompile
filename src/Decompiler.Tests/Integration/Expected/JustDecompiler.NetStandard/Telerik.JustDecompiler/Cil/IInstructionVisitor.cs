using Mono.Cecil.Cil;
using System;

namespace Telerik.JustDecompiler.Cil
{
	internal interface IInstructionVisitor
	{
		void OnAdd(Instruction instruction);

		void OnAdd_Ovf(Instruction instruction);

		void OnAdd_Ovf_Un(Instruction instruction);

		void OnAnd(Instruction instruction);

		void OnArglist(Instruction instruction);

		void OnBeq(Instruction instruction);

		void OnBge(Instruction instruction);

		void OnBge_Un(Instruction instruction);

		void OnBgt(Instruction instruction);

		void OnBgt_Un(Instruction instruction);

		void OnBle(Instruction instruction);

		void OnBle_Un(Instruction instruction);

		void OnBlt(Instruction instruction);

		void OnBlt_Un(Instruction instruction);

		void OnBne_Un(Instruction instruction);

		void OnBox(Instruction instruction);

		void OnBr(Instruction instruction);

		void OnBreak(Instruction instruction);

		void OnBrfalse(Instruction instruction);

		void OnBrtrue(Instruction instruction);

		void OnCall(Instruction instruction);

		void OnCalli(Instruction instruction);

		void OnCallvirt(Instruction instruction);

		void OnCastclass(Instruction instruction);

		void OnCeq(Instruction instruction);

		void OnCgt(Instruction instruction);

		void OnCgt_Un(Instruction instruction);

		void OnCkfinite(Instruction instruction);

		void OnClt(Instruction instruction);

		void OnClt_Un(Instruction instruction);

		void OnConstrained(Instruction instruction);

		void OnConv_I(Instruction instruction);

		void OnConv_I1(Instruction instruction);

		void OnConv_I2(Instruction instruction);

		void OnConv_I4(Instruction instruction);

		void OnConv_I8(Instruction instruction);

		void OnConv_Ovf_I(Instruction instruction);

		void OnConv_Ovf_I_Un(Instruction instruction);

		void OnConv_Ovf_I1(Instruction instruction);

		void OnConv_Ovf_I1_Un(Instruction instruction);

		void OnConv_Ovf_I2(Instruction instruction);

		void OnConv_Ovf_I2_Un(Instruction instruction);

		void OnConv_Ovf_I4(Instruction instruction);

		void OnConv_Ovf_I4_Un(Instruction instruction);

		void OnConv_Ovf_I8(Instruction instruction);

		void OnConv_Ovf_I8_Un(Instruction instruction);

		void OnConv_Ovf_U(Instruction instruction);

		void OnConv_Ovf_U_Un(Instruction instruction);

		void OnConv_Ovf_U1(Instruction instruction);

		void OnConv_Ovf_U1_Un(Instruction instruction);

		void OnConv_Ovf_U2(Instruction instruction);

		void OnConv_Ovf_U2_Un(Instruction instruction);

		void OnConv_Ovf_U4(Instruction instruction);

		void OnConv_Ovf_U4_Un(Instruction instruction);

		void OnConv_Ovf_U8(Instruction instruction);

		void OnConv_Ovf_U8_Un(Instruction instruction);

		void OnConv_R_Un(Instruction instruction);

		void OnConv_R4(Instruction instruction);

		void OnConv_R8(Instruction instruction);

		void OnConv_U(Instruction instruction);

		void OnConv_U1(Instruction instruction);

		void OnConv_U2(Instruction instruction);

		void OnConv_U4(Instruction instruction);

		void OnConv_U8(Instruction instruction);

		void OnCpblk(Instruction instruction);

		void OnCpobj(Instruction instruction);

		void OnDiv(Instruction instruction);

		void OnDiv_Un(Instruction instruction);

		void OnDup(Instruction instruction);

		void OnEndfilter(Instruction instruction);

		void OnEndfinally(Instruction instruction);

		void OnInitblk(Instruction instruction);

		void OnInitobj(Instruction instruction);

		void OnIsinst(Instruction instruction);

		void OnJmp(Instruction instruction);

		void OnLdarg(Instruction instruction);

		void OnLdarg_0(Instruction instruction);

		void OnLdarg_1(Instruction instruction);

		void OnLdarg_2(Instruction instruction);

		void OnLdarg_3(Instruction instruction);

		void OnLdarga(Instruction instruction);

		void OnLdc_I4(Instruction instruction);

		void OnLdc_I4_0(Instruction instruction);

		void OnLdc_I4_1(Instruction instruction);

		void OnLdc_I4_2(Instruction instruction);

		void OnLdc_I4_3(Instruction instruction);

		void OnLdc_I4_4(Instruction instruction);

		void OnLdc_I4_5(Instruction instruction);

		void OnLdc_I4_6(Instruction instruction);

		void OnLdc_I4_7(Instruction instruction);

		void OnLdc_I4_8(Instruction instruction);

		void OnLdc_I4_M1(Instruction instruction);

		void OnLdc_I8(Instruction instruction);

		void OnLdc_R4(Instruction instruction);

		void OnLdc_R8(Instruction instruction);

		void OnLdelem_Any(Instruction instruction);

		void OnLdelem_I(Instruction instruction);

		void OnLdelem_I1(Instruction instruction);

		void OnLdelem_I2(Instruction instruction);

		void OnLdelem_I4(Instruction instruction);

		void OnLdelem_I8(Instruction instruction);

		void OnLdelem_R4(Instruction instruction);

		void OnLdelem_R8(Instruction instruction);

		void OnLdelem_Ref(Instruction instruction);

		void OnLdelem_U1(Instruction instruction);

		void OnLdelem_U2(Instruction instruction);

		void OnLdelem_U4(Instruction instruction);

		void OnLdelema(Instruction instruction);

		void OnLdfld(Instruction instruction);

		void OnLdflda(Instruction instruction);

		void OnLdftn(Instruction instruction);

		void OnLdind_I(Instruction instruction);

		void OnLdind_I1(Instruction instruction);

		void OnLdind_I2(Instruction instruction);

		void OnLdind_I4(Instruction instruction);

		void OnLdind_I8(Instruction instruction);

		void OnLdind_R4(Instruction instruction);

		void OnLdind_R8(Instruction instruction);

		void OnLdind_Ref(Instruction instruction);

		void OnLdind_U1(Instruction instruction);

		void OnLdind_U2(Instruction instruction);

		void OnLdind_U4(Instruction instruction);

		void OnLdlen(Instruction instruction);

		void OnLdloc(Instruction instruction);

		void OnLdloc_0(Instruction instruction);

		void OnLdloc_1(Instruction instruction);

		void OnLdloc_2(Instruction instruction);

		void OnLdloc_3(Instruction instruction);

		void OnLdloca(Instruction instruction);

		void OnLdnull(Instruction instruction);

		void OnLdobj(Instruction instruction);

		void OnLdsfld(Instruction instruction);

		void OnLdsflda(Instruction instruction);

		void OnLdstr(Instruction instruction);

		void OnLdtoken(Instruction instruction);

		void OnLdvirtftn(Instruction instruction);

		void OnLeave(Instruction instruction);

		void OnLocalloc(Instruction instruction);

		void OnMkrefany(Instruction instruction);

		void OnMul(Instruction instruction);

		void OnMul_Ovf(Instruction instruction);

		void OnMul_Ovf_Un(Instruction instruction);

		void OnNeg(Instruction instruction);

		void OnNewarr(Instruction instruction);

		void OnNewobj(Instruction instruction);

		void OnNop(Instruction instruction);

		void OnNot(Instruction instruction);

		void OnOr(Instruction instruction);

		void OnPop(Instruction instruction);

		void OnRefanytype(Instruction instruction);

		void OnRefanyval(Instruction instruction);

		void OnRem(Instruction instruction);

		void OnRem_Un(Instruction instruction);

		void OnRet(Instruction instruction);

		void OnRethrow(Instruction instruction);

		void OnShl(Instruction instruction);

		void OnShr(Instruction instruction);

		void OnShr_Un(Instruction instruction);

		void OnSizeof(Instruction instruction);

		void OnStarg(Instruction instruction);

		void OnStelem_Any(Instruction instruction);

		void OnStelem_I(Instruction instruction);

		void OnStelem_I1(Instruction instruction);

		void OnStelem_I2(Instruction instruction);

		void OnStelem_I4(Instruction instruction);

		void OnStelem_I8(Instruction instruction);

		void OnStelem_R4(Instruction instruction);

		void OnStelem_R8(Instruction instruction);

		void OnStelem_Ref(Instruction instruction);

		void OnStfld(Instruction instruction);

		void OnStind_I(Instruction instruction);

		void OnStind_I1(Instruction instruction);

		void OnStind_I2(Instruction instruction);

		void OnStind_I4(Instruction instruction);

		void OnStind_I8(Instruction instruction);

		void OnStind_R4(Instruction instruction);

		void OnStind_R8(Instruction instruction);

		void OnStind_Ref(Instruction instruction);

		void OnStloc(Instruction instruction);

		void OnStloc_0(Instruction instruction);

		void OnStloc_1(Instruction instruction);

		void OnStloc_2(Instruction instruction);

		void OnStloc_3(Instruction instruction);

		void OnStobj(Instruction instruction);

		void OnStsfld(Instruction instruction);

		void OnSub(Instruction instruction);

		void OnSub_Ovf(Instruction instruction);

		void OnSub_Ovf_Un(Instruction instruction);

		void OnSwitch(Instruction instruction);

		void OnTail(Instruction instruction);

		void OnThrow(Instruction instruction);

		void OnUnaligned(Instruction instruction);

		void OnUnbox(Instruction instruction);

		void OnUnbox_Any(Instruction instruction);

		void OnVolatile(Instruction instruction);

		void OnXor(Instruction instruction);
	}
}