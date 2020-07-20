using Mono.Cecil.Cil;
using System;

namespace Telerik.JustDecompiler.Cil
{
	public class BaseInstructionVisitor : IInstructionVisitor
	{
		public BaseInstructionVisitor()
		{
			base();
			return;
		}

		public virtual void OnAdd(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnAdd_Ovf(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnAdd_Ovf_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnAnd(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnArglist(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBeq(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBge(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBge_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBgt(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBgt_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBle(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBle_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBlt(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBlt_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBne_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBox(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBr(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBreak(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBrfalse(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnBrtrue(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCall(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCalli(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCallvirt(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCastclass(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCeq(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCgt(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCgt_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCkfinite(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnClt(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnClt_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConstrained(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_I(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_I1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_I2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I1_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I2_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I4_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_I8_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U1_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U2_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U4_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_Ovf_U8_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_R_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_R4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_R8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_U(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_U1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_U2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_U4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnConv_U8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCpblk(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnCpobj(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnDiv(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnDiv_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnDup(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnEndfilter(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnEndfinally(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnInitblk(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnInitobj(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnIsinst(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnJmp(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdarg(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdarg_0(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdarg_1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdarg_2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdarg_3(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdarga(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_0(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_3(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_5(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_6(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_7(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I4_M1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_R4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdc_R8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_Any(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_I(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_I1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_I2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_R4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_R8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_Ref(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_U1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_U2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelem_U4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdelema(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdfld(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdflda(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdftn(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_I(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_I1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_I2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_R4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_R8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_Ref(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_U1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_U2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdind_U4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdlen(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdloc(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdloc_0(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdloc_1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdloc_2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdloc_3(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdloca(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdnull(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdobj(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdsfld(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdsflda(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdstr(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLdtoken(Instruction instruction)
		{
			return;
		}

		public virtual void OnLdvirtftn(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLeave(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnLocalloc(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnMkrefany(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnMul(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnMul_Ovf(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnMul_Ovf_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnNeg(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnNewarr(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnNewobj(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnNop(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnNot(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnOr(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnPop(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnRefanytype(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnRefanyval(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnRem(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnRem_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnRet(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnRethrow(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnShl(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnShr(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnShr_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnSizeof(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStarg(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_Any(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_I(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_I1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_I2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_R4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_R8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStelem_Ref(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStfld(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_I(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_I1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_I2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_I4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_I8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_R4(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_R8(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStind_Ref(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStloc(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStloc_0(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStloc_1(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStloc_2(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStloc_3(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStobj(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnStsfld(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnSub(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnSub_Ovf(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnSub_Ovf_Un(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnSwitch(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnTail(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnThrow(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnUnaligned(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnUnbox(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnUnbox_Any(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnVolatile(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public virtual void OnXor(Instruction instruction)
		{
			throw new NotImplementedException(Formatter.FormatInstruction(instruction));
		}

		public void Visit(Instruction instruction)
		{
			InstructionDispatcher.Dispatch(instruction, this);
			return;
		}
	}
}