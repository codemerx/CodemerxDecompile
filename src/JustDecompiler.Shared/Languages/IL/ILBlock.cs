using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal enum ILBlockType
	{
		Root,
		Loop,
		Try,
		Handler,
		Filter
	}

	internal class ILBlock
	{
		public readonly ILBlockType Type;

		public readonly int StartOffset;

		public readonly int EndOffset;

		public readonly ExceptionHandler ExceptionHandler;

		public readonly Instruction LoopEntryPoint;

		public readonly List<ILBlock> Children = new List<ILBlock>();
		
		public ILBlock(MethodBody body) : this(ILBlockType.Root, 0, body.CodeSize)
		{
			AddExceptionHandlerBlocks(body);
			List<InstructionPair> allBranches = FindAllBranches(body);

			AddBlocks(allBranches);
			SortChildren();
		}
  
		private void AddBlocks(List<InstructionPair> allBranches)
		{
			for (int i = allBranches.Count - 1; i >= 0; i--)
			{
				int loopEnd = allBranches[i].FirstInstruction.Offset + allBranches[i].FirstInstruction.GetSize();
				int loopStart = allBranches[i].SecondInstruction.Offset;
				if (loopStart < loopEnd)
				{
					Instruction entryPoint = null;
  					
					Instruction prev = allBranches[i].SecondInstruction.Previous;
					if (prev != null && !OpCodeInfo.IsUnconditionalBranch(prev.OpCode))
						entryPoint = allBranches[i].SecondInstruction;
  					
					bool multipleEntryPoints = false;
					foreach (var pair in allBranches)
					{
						if (pair.FirstInstruction.Offset < loopStart || pair.FirstInstruction.Offset >= loopEnd)
						{
							if (loopStart <= pair.SecondInstruction.Offset && pair.SecondInstruction.Offset < loopEnd)
							{
								if (entryPoint == null)
									entryPoint = pair.SecondInstruction;
								else if (pair.SecondInstruction != entryPoint)
									multipleEntryPoints = true;
							}
						}
					}
					if (!multipleEntryPoints)
					{
						AddNestedBlock(new ILBlock(ILBlockType.Loop, loopStart, loopEnd, entryPoint));
					}
				}
			}
		}
  
		private void AddExceptionHandlerBlocks(MethodBody body)
		{
			for (int i = 0; i < body.ExceptionHandlers.Count; i++)
			{
				ExceptionHandler eh = body.ExceptionHandlers[i];
				if (!body.ExceptionHandlers.Take(i).Any(oldEh => oldEh.TryStart == eh.TryStart && oldEh.TryEnd == eh.TryEnd))
				{
					AddNestedBlock(new ILBlock(ILBlockType.Try, eh.TryStart.Offset, eh.TryEnd.Offset, eh));
				}
				if (eh.HandlerType == ExceptionHandlerType.Filter)
				{
					AddNestedBlock(new ILBlock(ILBlockType.Filter, eh.FilterStart.Offset, eh.HandlerStart.Offset, eh));
				}
				AddNestedBlock(new ILBlock(ILBlockType.Handler, eh.HandlerStart.Offset, eh.HandlerEnd == null ? body.CodeSize : eh.HandlerEnd.Offset, eh));
			}
		}
		
		public ILBlock(ILBlockType type, int startOffset, int endOffset, ExceptionHandler handler = null)
		{
			this.Type = type;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.ExceptionHandler = handler;
		}
		
		public ILBlock(ILBlockType type, int startOffset, int endOffset, Instruction loopEntryPoint)
		{
			this.Type = type;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.LoopEntryPoint = loopEntryPoint;
		}
		
		bool AddNestedBlock(ILBlock newStructure)
		{
			if (this.Type == ILBlockType.Loop && newStructure.Type == ILBlockType.Loop && newStructure.StartOffset == this.StartOffset)
				return false;
			
			bool returnValue;
			if (ShouldAddNestedBlock(newStructure, out returnValue))
			{
				return returnValue;
			}
			AddNestedBlocks(newStructure);
			this.Children.Add(newStructure);
			return true;
		}
  
		private void AddNestedBlocks(ILBlock newStructure)
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				ILBlock child = this.Children[i];
				if (newStructure.StartOffset <= child.StartOffset && child.EndOffset <= newStructure.EndOffset)
				{
					this.Children.RemoveAt(i--);
					newStructure.Children.Add(child);
				}
			}
		}
  
		private bool ShouldAddNestedBlock(ILBlock newStructure, out bool returnValue)
		{
			returnValue = false;
			foreach (ILBlock child in this.Children)
			{
				if (child.StartOffset <= newStructure.StartOffset && newStructure.EndOffset <= child.EndOffset)
				{
					returnValue = child.AddNestedBlock(newStructure);
					return true;
				}
				else if (!(child.EndOffset <= newStructure.StartOffset || newStructure.EndOffset <= child.StartOffset))
				{
					if (!(newStructure.StartOffset <= child.StartOffset && child.EndOffset <= newStructure.EndOffset))
					{
						returnValue = false;
						return true;
					}
				}
			}
			return false;
		}

		List<InstructionPair> FindAllBranches(MethodBody body)
		{
			var result = new List<InstructionPair>();
			foreach (Instruction inst in body.Instructions)
			{
				switch (inst.OpCode.OperandType)
				{
					case OperandType.InlineBrTarget:
					case OperandType.ShortInlineBrTarget:
						result.Add(new InstructionPair(inst, (Instruction)inst.Operand));
						break;
					case OperandType.InlineSwitch:
						foreach (Instruction target in (Instruction[])inst.Operand)
							result.Add(new InstructionPair(inst, target));
						break;
				}
			}
			return result;
		}
		
		void SortChildren()
		{
			Children.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
			foreach (ILBlock child in Children)
				child.SortChildren();
		}
		
		public ILBlock GetInnermost(int offset)
		{
			foreach (ILBlock child in this.Children)
			{
				if (child.StartOffset <= offset && offset < child.EndOffset)
					return child.GetInnermost(offset);
			}
			return this;
		}

		private class InstructionPair
		{
			internal Instruction FirstInstruction
			{
				get;
				set;
			}

			internal Instruction SecondInstruction
			{
				get;
				set;
			}

			public InstructionPair(Instruction firstInstruction, Instruction secondInstruction)
			{
				this.FirstInstruction = firstInstruction;
				this.SecondInstruction = secondInstruction;
			}
		}
	}

	sealed class OpCodeInfo
	{
		public static bool IsUnconditionalBranch(OpCode opcode)
		{
			if (opcode.OpCodeType == OpCodeType.Prefix)
				return false;
			switch (opcode.FlowControl)
			{
				case FlowControl.Branch:
				case FlowControl.Throw:
				case FlowControl.Return:
					return true;
				case FlowControl.Next:
				case FlowControl.Call:
				case FlowControl.Cond_Branch:
					return false;
				default:
					throw new NotSupportedException(opcode.FlowControl.ToString());
			}
		}
		
		static readonly OpCodeInfo[] knownOpCodes =
		{
			#region Base Instructions
			new OpCodeInfo(OpCodes.Add) { CanThrow = false },
			new OpCodeInfo(OpCodes.Add_Ovf) { CanThrow = true },
			new OpCodeInfo(OpCodes.Add_Ovf_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.And) { CanThrow = false },
			new OpCodeInfo(OpCodes.Arglist) { CanThrow = false },
			new OpCodeInfo(OpCodes.Beq) { CanThrow = false },
			new OpCodeInfo(OpCodes.Beq_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bge) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bge_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bge_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bge_Un_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bgt) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bgt_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bgt_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bgt_Un_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ble) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ble_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ble_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ble_Un_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Blt) { CanThrow = false },
			new OpCodeInfo(OpCodes.Blt_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Blt_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Blt_Un_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bne_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Bne_Un_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Br) { CanThrow = false },
			new OpCodeInfo(OpCodes.Br_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Break) { CanThrow = true },
			new OpCodeInfo(OpCodes.Brfalse) { CanThrow = false },
			new OpCodeInfo(OpCodes.Brfalse_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Brtrue) { CanThrow = false },
			new OpCodeInfo(OpCodes.Brtrue_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Call) { CanThrow = true },
			new OpCodeInfo(OpCodes.Calli) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ceq) { CanThrow = false },
			new OpCodeInfo(OpCodes.Cgt) { CanThrow = false },
			new OpCodeInfo(OpCodes.Cgt_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ckfinite) { CanThrow = true },
			new OpCodeInfo(OpCodes.Clt) { CanThrow = false },
			new OpCodeInfo(OpCodes.Clt_Un) { CanThrow = false },
			// conv.<to type>
			new OpCodeInfo(OpCodes.Conv_I1) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_I2) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_I4) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_I8) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_R4) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_R8) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_U1) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_U2) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_U4) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_U8) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_I) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_U) { CanThrow = false },
			new OpCodeInfo(OpCodes.Conv_R_Un) { CanThrow = false },
			// conv.ovf.<to type>
			new OpCodeInfo(OpCodes.Conv_Ovf_I1) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I2) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I8) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U1) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U2) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U8) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U) { CanThrow = true },
			// conv.ovf.<to type>.un
			new OpCodeInfo(OpCodes.Conv_Ovf_I1_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I2_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I4_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I8_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U1_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U2_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U4_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U8_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_I_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Conv_Ovf_U_Un) { CanThrow = true },
			
			new OpCodeInfo(OpCodes.Div) { CanThrow = true },
			new OpCodeInfo(OpCodes.Div_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Dup) { CanThrow = true, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Endfilter) { CanThrow = false },
			new OpCodeInfo(OpCodes.Endfinally) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldarg) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldarg_0) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldarg_1) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldarg_2) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldarg_3) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldarg_S) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldarga) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldarga_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_M1) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_0) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_1) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_2) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_3) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_4) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_5) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_6) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_7) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_8) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I4_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_I8) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_R4) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldc_R8) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldftn) { CanThrow = false },
			// ldind.<type>
			new OpCodeInfo(OpCodes.Ldind_I1) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_I2) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_I4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_I8) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_U1) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_U2) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_U4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_R4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_R8) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_I) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldind_Ref) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ldloc) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldloc_0) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldloc_1) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldloc_2) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldloc_3) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldloc_S) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Ldloca) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldloca_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldnull) { CanThrow = false },
			new OpCodeInfo(OpCodes.Leave) { CanThrow = false },
			new OpCodeInfo(OpCodes.Leave_S) { CanThrow = false },
			new OpCodeInfo(OpCodes.Localloc) { CanThrow = true },
			new OpCodeInfo(OpCodes.Mul) { CanThrow = false },
			new OpCodeInfo(OpCodes.Mul_Ovf) { CanThrow = true },
			new OpCodeInfo(OpCodes.Mul_Ovf_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Neg) { CanThrow = false },
			new OpCodeInfo(OpCodes.Nop) { CanThrow = false },
			new OpCodeInfo(OpCodes.Not) { CanThrow = false },
			new OpCodeInfo(OpCodes.Or) { CanThrow = false },
			new OpCodeInfo(OpCodes.Pop) { CanThrow = false },
			new OpCodeInfo(OpCodes.Rem) { CanThrow = true },
			new OpCodeInfo(OpCodes.Rem_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Ret) { CanThrow = false },
			new OpCodeInfo(OpCodes.Shl) { CanThrow = false },
			new OpCodeInfo(OpCodes.Shr) { CanThrow = false },
			new OpCodeInfo(OpCodes.Shr_Un) { CanThrow = false },
			new OpCodeInfo(OpCodes.Starg) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Starg_S) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Stind_I1) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_I2) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_I4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_I8) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_R4) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_R8) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_I) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stind_Ref) { CanThrow = true },
			new OpCodeInfo(OpCodes.Stloc) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Stloc_0) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Stloc_1) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Stloc_2) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Stloc_3) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Stloc_S) { CanThrow = false, IsMoveInstruction = true },
			new OpCodeInfo(OpCodes.Sub) { CanThrow = false },
			new OpCodeInfo(OpCodes.Sub_Ovf) { CanThrow = true },
			new OpCodeInfo(OpCodes.Sub_Ovf_Un) { CanThrow = true },
			new OpCodeInfo(OpCodes.Switch) { CanThrow = false },
			new OpCodeInfo(OpCodes.Xor) { CanThrow = false },
			#endregion
			#region Object model instructions
			new OpCodeInfo(OpCodes.Box),
			new OpCodeInfo(OpCodes.Callvirt),
			new OpCodeInfo(OpCodes.Castclass),
			new OpCodeInfo(OpCodes.Cpobj),
			new OpCodeInfo(OpCodes.Initobj) { CanThrow = false },
			new OpCodeInfo(OpCodes.Isinst) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldelem_Any),
			// ldelem.<type>
			new OpCodeInfo(OpCodes.Ldelem_I) ,
			new OpCodeInfo(OpCodes.Ldelem_I1),
			new OpCodeInfo(OpCodes.Ldelem_I2),
			new OpCodeInfo(OpCodes.Ldelem_I4),
			new OpCodeInfo(OpCodes.Ldelem_I8),
			new OpCodeInfo(OpCodes.Ldelem_R4),
			new OpCodeInfo(OpCodes.Ldelem_R8),
			new OpCodeInfo(OpCodes.Ldelem_Ref),
			new OpCodeInfo(OpCodes.Ldelem_U1),
			new OpCodeInfo(OpCodes.Ldelem_U2),
			new OpCodeInfo(OpCodes.Ldelem_U4),
			new OpCodeInfo(OpCodes.Ldelema)  ,
			new OpCodeInfo(OpCodes.Ldfld) ,
			new OpCodeInfo(OpCodes.Ldflda),
			new OpCodeInfo(OpCodes.Ldlen) ,
			new OpCodeInfo(OpCodes.Ldobj) ,
			new OpCodeInfo(OpCodes.Ldsfld),
			new OpCodeInfo(OpCodes.Ldsflda),
			new OpCodeInfo(OpCodes.Ldstr) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldtoken) { CanThrow = false },
			new OpCodeInfo(OpCodes.Ldvirtftn),
			new OpCodeInfo(OpCodes.Mkrefany),
			new OpCodeInfo(OpCodes.Newarr),
			new OpCodeInfo(OpCodes.Newobj),
			new OpCodeInfo(OpCodes.Refanytype) { CanThrow = false },
			new OpCodeInfo(OpCodes.Refanyval),
			new OpCodeInfo(OpCodes.Rethrow),
			new OpCodeInfo(OpCodes.Sizeof) { CanThrow = false },
			new OpCodeInfo(OpCodes.Stelem_Any),
			new OpCodeInfo(OpCodes.Stelem_I1),
			new OpCodeInfo(OpCodes.Stelem_I2),
			new OpCodeInfo(OpCodes.Stelem_I4),
			new OpCodeInfo(OpCodes.Stelem_I8),
			new OpCodeInfo(OpCodes.Stelem_R4),
			new OpCodeInfo(OpCodes.Stelem_R8),
			new OpCodeInfo(OpCodes.Stelem_Ref),
			new OpCodeInfo(OpCodes.Stfld),
			new OpCodeInfo(OpCodes.Stobj),
			new OpCodeInfo(OpCodes.Stsfld),
			new OpCodeInfo(OpCodes.Throw),
			new OpCodeInfo(OpCodes.Unbox),
			new OpCodeInfo(OpCodes.Unbox_Any),
			#endregion
		};
		static readonly Dictionary<Code, OpCodeInfo> knownOpCodeDict = knownOpCodes.ToDictionary(info => info.OpCode.Code);
		
		public static OpCodeInfo Get(OpCode opCode)
		{
			return Get(opCode.Code);
		}
		
		public static OpCodeInfo Get(Code code)
		{
			OpCodeInfo info;
			if (knownOpCodeDict.TryGetValue(code, out info))
				return info;
			else
				throw new NotSupportedException(code.ToString());
		}
		
		OpCode opcode;
		
		private OpCodeInfo(OpCode opcode)
		{
			this.opcode = opcode;
			this.CanThrow = true;
		}
		
		public OpCode OpCode
		{
			get
			{
				return opcode;
			}
		}
		
		public bool IsMoveInstruction { get; private set; }
		
		public bool CanThrow { get; private set; }
	}
}