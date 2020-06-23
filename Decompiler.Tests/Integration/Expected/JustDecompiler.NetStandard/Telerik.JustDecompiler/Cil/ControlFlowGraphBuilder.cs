using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Cil
{
	internal class ControlFlowGraphBuilder
	{
		private readonly MethodBody body;

		private readonly Dictionary<int, InstructionBlock> blocks = new Dictionary<int, InstructionBlock>();

		private readonly HashSet<int> exceptionObjectsOffsets;

		private readonly Dictionary<InstructionBlock, SwitchData> switchBlocksInformation = new Dictionary<InstructionBlock, SwitchData>();

		private Dictionary<int, Instruction> offsetToInstruction;

		internal ControlFlowGraphBuilder(MethodDefinition method)
		{
			this.body = method.Body;
			if (this.body.ExceptionHandlers.Count > 0)
			{
				this.exceptionObjectsOffsets = new HashSet<int>();
			}
		}

		private static InstructionBlock[] AddBlock(InstructionBlock block, InstructionBlock[] blocks)
		{
			InstructionBlock[] instructionBlockArrays = new InstructionBlock[(int)blocks.Length + 1];
			Array.Copy(blocks, instructionBlockArrays, (int)blocks.Length);
			instructionBlockArrays[(int)instructionBlockArrays.Length - 1] = block;
			return instructionBlockArrays;
		}

		private static void ComputeIndexes(InstructionBlock[] blocks)
		{
			for (int i = 0; i < (int)blocks.Length; i++)
			{
				blocks[i].Index = i;
			}
		}

		private void ConnectBlock(InstructionBlock block)
		{
			OpCode opCode;
			if (block.Last == null)
			{
				int offset = block.First.Offset;
				throw new ArgumentException(String.Concat("Undelimited block at offset ", offset.ToString()));
			}
			Instruction last = block.Last;
			switch (last.OpCode.FlowControl)
			{
				case FlowControl.Branch:
				case FlowControl.Cond_Branch:
				{
					if (!ControlFlowGraphBuilder.HasMultipleBranches(last))
					{
						InstructionBlock branchTargetBlock = this.GetBranchTargetBlock(last);
						if (last.OpCode.FlowControl != FlowControl.Cond_Branch || last.Next == null)
						{
							block.Successors = new InstructionBlock[] { branchTargetBlock };
							return;
						}
						block.Successors = new InstructionBlock[] { branchTargetBlock, this.GetBlock(last.Next) };
						return;
					}
					InstructionBlock[] branchTargetsBlocks = this.GetBranchTargetsBlocks(last);
					InstructionBlock instructionBlocks = null;
					if (last.Next != null)
					{
						instructionBlocks = this.GetBlock(last.Next);
					}
					this.switchBlocksInformation.Add(block, new SwitchData(block, instructionBlocks, branchTargetsBlocks));
					if (instructionBlocks != null)
					{
						branchTargetsBlocks = ControlFlowGraphBuilder.AddBlock(instructionBlocks, branchTargetsBlocks);
					}
					block.Successors = branchTargetsBlocks;
					return;
				}
				case FlowControl.Break:
				case FlowControl.Meta:
				case FlowControl.Phi:
				{
					opCode = last.OpCode;
					throw new NotSupportedException(String.Format("Unhandled instruction flow behavior {0}: {1}", opCode.FlowControl, Formatter.FormatInstruction(last)));
				}
				case FlowControl.Call:
				case FlowControl.Next:
				{
					if (last.Next == null)
					{
						return;
					}
					block.Successors = new InstructionBlock[] { this.GetBlock(last.Next) };
					return;
				}
				case FlowControl.Return:
				case FlowControl.Throw:
				{
					return;
				}
				default:
				{
					opCode = last.OpCode;
					throw new NotSupportedException(String.Format("Unhandled instruction flow behavior {0}: {1}", opCode.FlowControl, Formatter.FormatInstruction(last)));
				}
			}
		}

		private void ConnectBlocks()
		{
			foreach (InstructionBlock value in this.blocks.Values)
			{
				this.ConnectBlock(value);
			}
		}

		public ControlFlowGraph CreateGraph()
		{
			this.FillOffsetToInstruction();
			this.DelimitBlocks();
			this.ConnectBlocks();
			return new ControlFlowGraph(this.body, this.ToArray(), this.blocks, this.switchBlocksInformation, this.offsetToInstruction);
		}

		private void DelimitBlocks()
		{
			Collection<Instruction> instructions = this.body.Instructions;
			this.MarkBlockStarts(instructions);
			this.MarkBlockStarts(this.body.ExceptionHandlers);
			this.MarkBlockEnds(instructions);
		}

		private void FillOffsetToInstruction()
		{
			this.offsetToInstruction = new Dictionary<int, Instruction>();
			foreach (Instruction instruction in this.body.Instructions)
			{
				this.offsetToInstruction.Add(instruction.Offset, instruction);
			}
		}

		private InstructionBlock GetBlock(Instruction firstInstruction)
		{
			InstructionBlock instructionBlocks;
			this.blocks.TryGetValue(firstInstruction.Offset, out instructionBlocks);
			return instructionBlocks;
		}

		private static Instruction GetBranchTarget(Instruction instruction)
		{
			return (Instruction)instruction.Operand;
		}

		private InstructionBlock GetBranchTargetBlock(Instruction instruction)
		{
			return this.GetBlock(ControlFlowGraphBuilder.GetBranchTarget(instruction));
		}

		private static Instruction[] GetBranchTargets(Instruction instruction)
		{
			return (Instruction[])instruction.Operand;
		}

		private InstructionBlock[] GetBranchTargetsBlocks(Instruction instruction)
		{
			Instruction[] branchTargets = ControlFlowGraphBuilder.GetBranchTargets(instruction);
			InstructionBlock[] block = new InstructionBlock[(int)branchTargets.Length];
			for (int i = 0; i < (int)branchTargets.Length; i++)
			{
				block[i] = this.GetBlock(branchTargets[i]);
			}
			return block;
		}

		private static bool HasMultipleBranches(Instruction instruction)
		{
			return instruction.OpCode.Code == Code.Switch;
		}

		private static bool IsBlockDelimiter(Instruction instruction)
		{
			switch (instruction.OpCode.FlowControl)
			{
				case FlowControl.Branch:
				case FlowControl.Break:
				case FlowControl.Cond_Branch:
				case FlowControl.Return:
				case FlowControl.Throw:
				{
					return true;
				}
				case FlowControl.Call:
				case FlowControl.Meta:
				case FlowControl.Next:
				case FlowControl.Phi:
				{
					return false;
				}
				default:
				{
					return false;
				}
			}
		}

		private void MarkBlockEnds(Collection<Instruction> instructions)
		{
			InstructionBlock[] array = this.ToArray();
			if (array.Length == 0)
			{
				return;
			}
			InstructionBlock previous = array[0];
			for (int i = 1; i < (int)array.Length; i++)
			{
				InstructionBlock instructionBlocks = array[i];
				previous.Last = instructionBlocks.First.Previous;
				previous = instructionBlocks;
			}
			previous.Last = instructions[instructions.Count - 1];
		}

		private void MarkBlockStart(Instruction instruction)
		{
			if (this.GetBlock(instruction) != null)
			{
				return;
			}
			this.RegisterBlock(new InstructionBlock(instruction));
		}

		private void MarkBlockStarts(Collection<ExceptionHandler> handlers)
		{
			for (int i = 0; i < handlers.Count; i++)
			{
				ExceptionHandler item = handlers[i];
				this.MarkBlockStart(item.TryStart);
				this.MarkBlockStart(item.HandlerStart);
				if (item.HandlerType == ExceptionHandlerType.Filter)
				{
					this.MarkExceptionObjectPosition(item.FilterStart);
					this.MarkBlockStart(item.FilterStart);
				}
				else if (item.HandlerType == ExceptionHandlerType.Catch)
				{
					this.MarkExceptionObjectPosition(item.HandlerStart);
				}
			}
		}

		private void MarkBlockStarts(Collection<Instruction> instructions)
		{
			for (int i = 0; i < instructions.Count; i++)
			{
				Instruction item = instructions[i];
				if (i == 0)
				{
					this.MarkBlockStart(item);
				}
				if (ControlFlowGraphBuilder.IsBlockDelimiter(item))
				{
					if (!ControlFlowGraphBuilder.HasMultipleBranches(item))
					{
						Instruction branchTarget = ControlFlowGraphBuilder.GetBranchTarget(item);
						if (branchTarget != null)
						{
							this.MarkBlockStart(branchTarget);
						}
					}
					else
					{
						Instruction[] branchTargets = ControlFlowGraphBuilder.GetBranchTargets(item);
						for (int j = 0; j < (int)branchTargets.Length; j++)
						{
							Instruction instruction = branchTargets[j];
							if (instruction != null)
							{
								this.MarkBlockStart(instruction);
							}
						}
					}
					if (item.Next != null)
					{
						this.MarkBlockStart(item.Next);
					}
				}
			}
		}

		private void MarkExceptionObjectPosition(Instruction instruction)
		{
			this.exceptionObjectsOffsets.Add(instruction.Offset);
		}

		private void RegisterBlock(InstructionBlock block)
		{
			this.blocks.Add(block.First.Offset, block);
		}

		private InstructionBlock[] ToArray()
		{
			InstructionBlock[] instructionBlockArrays;
			try
			{
				InstructionBlock[] instructionBlockArrays1 = new InstructionBlock[this.blocks.Count];
				this.blocks.Values.CopyTo(instructionBlockArrays1, 0);
				Array.Sort<InstructionBlock>(instructionBlockArrays1);
				ControlFlowGraphBuilder.ComputeIndexes(instructionBlockArrays1);
				instructionBlockArrays = instructionBlockArrays1;
			}
			catch (Exception exception)
			{
				throw new InvalidProgramException(exception.Message);
			}
			return instructionBlockArrays;
		}
	}
}