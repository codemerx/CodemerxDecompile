using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal class ILBlock
	{
		public readonly ILBlockType Type;

		public readonly int StartOffset;

		public readonly int EndOffset;

		public readonly Mono.Cecil.Cil.ExceptionHandler ExceptionHandler;

		public readonly Instruction LoopEntryPoint;

		public readonly List<ILBlock> Children = new List<ILBlock>();

		public ILBlock(MethodBody body) : this(ILBlockType.Root, 0, body.CodeSize, (Mono.Cecil.Cil.ExceptionHandler)null)
		{
			this.AddExceptionHandlerBlocks(body);
			this.AddBlocks(this.FindAllBranches(body));
			this.SortChildren();
		}

		public ILBlock(ILBlockType type, int startOffset, int endOffset, Mono.Cecil.Cil.ExceptionHandler handler = null)
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

		private void AddBlocks(List<ILBlock.InstructionPair> allBranches)
		{
			for (int i = allBranches.Count - 1; i >= 0; i--)
			{
				int offset = allBranches[i].FirstInstruction.Offset + allBranches[i].FirstInstruction.GetSize();
				int num = allBranches[i].SecondInstruction.Offset;
				if (num < offset)
				{
					Instruction secondInstruction = null;
					Instruction previous = allBranches[i].SecondInstruction.Previous;
					if (previous != null && !OpCodeInfo.IsUnconditionalBranch(previous.OpCode))
					{
						secondInstruction = allBranches[i].SecondInstruction;
					}
					bool flag = false;
					foreach (ILBlock.InstructionPair allBranch in allBranches)
					{
						if (allBranch.FirstInstruction.Offset >= num && allBranch.FirstInstruction.Offset < offset || num > allBranch.SecondInstruction.Offset || allBranch.SecondInstruction.Offset >= offset)
						{
							continue;
						}
						if (secondInstruction != null)
						{
							if (allBranch.SecondInstruction == secondInstruction)
							{
								continue;
							}
							flag = true;
						}
						else
						{
							secondInstruction = allBranch.SecondInstruction;
						}
					}
					if (!flag)
					{
						this.AddNestedBlock(new ILBlock(ILBlockType.Loop, num, offset, secondInstruction));
					}
				}
			}
		}

		private void AddExceptionHandlerBlocks(MethodBody body)
		{
			for (int i = 0; i < body.ExceptionHandlers.Count; i++)
			{
				Mono.Cecil.Cil.ExceptionHandler item = body.ExceptionHandlers[i];
				if (!body.ExceptionHandlers.Take<Mono.Cecil.Cil.ExceptionHandler>(i).Any<Mono.Cecil.Cil.ExceptionHandler>((Mono.Cecil.Cil.ExceptionHandler oldEh) => {
					if (oldEh.TryStart != item.TryStart)
					{
						return false;
					}
					return oldEh.TryEnd == item.TryEnd;
				}))
				{
					this.AddNestedBlock(new ILBlock(ILBlockType.Try, item.TryStart.Offset, item.TryEnd.Offset, item));
				}
				if (item.HandlerType == ExceptionHandlerType.Filter)
				{
					this.AddNestedBlock(new ILBlock(ILBlockType.Filter, item.FilterStart.Offset, item.HandlerStart.Offset, item));
				}
				this.AddNestedBlock(new ILBlock(ILBlockType.Handler, item.HandlerStart.Offset, (item.HandlerEnd == null ? body.CodeSize : item.HandlerEnd.Offset), item));
			}
		}

		private bool AddNestedBlock(ILBlock newStructure)
		{
			bool flag;
			if (this.Type == ILBlockType.Loop && newStructure.Type == ILBlockType.Loop && newStructure.StartOffset == this.StartOffset)
			{
				return false;
			}
			if (this.ShouldAddNestedBlock(newStructure, out flag))
			{
				return flag;
			}
			this.AddNestedBlocks(newStructure);
			this.Children.Add(newStructure);
			return true;
		}

		private void AddNestedBlocks(ILBlock newStructure)
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				ILBlock item = this.Children[i];
				if (newStructure.StartOffset <= item.StartOffset && item.EndOffset <= newStructure.EndOffset)
				{
					int num = i;
					i = num - 1;
					this.Children.RemoveAt(num);
					newStructure.Children.Add(item);
				}
			}
		}

		private List<ILBlock.InstructionPair> FindAllBranches(MethodBody body)
		{
			List<ILBlock.InstructionPair> instructionPairs = new List<ILBlock.InstructionPair>();
		Label0:
			foreach (Instruction instruction in body.Instructions)
			{
				OperandType operandType = instruction.OpCode.OperandType;
				if (operandType != OperandType.InlineBrTarget)
				{
					if (operandType == OperandType.InlineSwitch)
					{
						Instruction[] operand = (Instruction[])instruction.Operand;
						for (int i = 0; i < (int)operand.Length; i++)
						{
							instructionPairs.Add(new ILBlock.InstructionPair(instruction, operand[i]));
						}
						goto Label0;
					}
					else if (operandType != OperandType.ShortInlineBrTarget)
					{
						continue;
					}
				}
				instructionPairs.Add(new ILBlock.InstructionPair(instruction, (Instruction)instruction.Operand));
			}
			return instructionPairs;
		}

		public ILBlock GetInnermost(int offset)
		{
			ILBlock innermost;
			List<ILBlock>.Enumerator enumerator = this.Children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ILBlock current = enumerator.Current;
					if (current.StartOffset > offset || offset >= current.EndOffset)
					{
						continue;
					}
					innermost = current.GetInnermost(offset);
					return innermost;
				}
				return this;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return innermost;
		}

		private bool ShouldAddNestedBlock(ILBlock newStructure, out bool returnValue)
		{
			bool flag;
			returnValue = false;
			List<ILBlock>.Enumerator enumerator = this.Children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ILBlock current = enumerator.Current;
					if (current.StartOffset > newStructure.StartOffset || newStructure.EndOffset > current.EndOffset)
					{
						if (current.EndOffset <= newStructure.StartOffset || newStructure.EndOffset <= current.StartOffset || newStructure.StartOffset <= current.StartOffset && current.EndOffset <= newStructure.EndOffset)
						{
							continue;
						}
						returnValue = false;
						flag = true;
						return flag;
					}
					else
					{
						returnValue = current.AddNestedBlock(newStructure);
						flag = true;
						return flag;
					}
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void SortChildren()
		{
			this.Children.Sort((ILBlock a, ILBlock b) => a.StartOffset.CompareTo(b.StartOffset));
			foreach (ILBlock child in this.Children)
			{
				child.SortChildren();
			}
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
}