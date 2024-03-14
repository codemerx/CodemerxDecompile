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
			this.body = method.get_Body();
			if (this.body.get_ExceptionHandlers().get_Count() > 0)
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
				int offset = block.First.get_Offset();
				throw new ArgumentException(String.Concat("Undelimited block at offset ", offset.ToString()));
			}
			Instruction last = block.Last;
			switch (last.get_OpCode().get_FlowControl())
			{
				case 0:
				case 3:
				{
					if (!ControlFlowGraphBuilder.HasMultipleBranches(last))
					{
						InstructionBlock branchTargetBlock = this.GetBranchTargetBlock(last);
						if (last.get_OpCode().get_FlowControl() != 3 || last.get_Next() == null)
						{
							block.Successors = new InstructionBlock[] { branchTargetBlock };
							return;
						}
						block.Successors = new InstructionBlock[] { branchTargetBlock, this.GetBlock(last.get_Next()) };
						return;
					}
					InstructionBlock[] branchTargetsBlocks = this.GetBranchTargetsBlocks(last);
					InstructionBlock instructionBlocks = null;
					if (last.get_Next() != null)
					{
						instructionBlocks = this.GetBlock(last.get_Next());
					}
					this.switchBlocksInformation.Add(block, new SwitchData(block, instructionBlocks, branchTargetsBlocks));
					if (instructionBlocks != null)
					{
						branchTargetsBlocks = ControlFlowGraphBuilder.AddBlock(instructionBlocks, branchTargetsBlocks);
					}
					block.Successors = branchTargetsBlocks;
					return;
				}
				case 1:
				case 4:
				case 6:
				{
					opCode = last.get_OpCode();
					throw new NotSupportedException(String.Format("Unhandled instruction flow behavior {0}: {1}", opCode.get_FlowControl(), Formatter.FormatInstruction(last)));
				}
				case 2:
				case 5:
				{
					if (last.get_Next() == null)
					{
						return;
					}
					block.Successors = new InstructionBlock[] { this.GetBlock(last.get_Next()) };
					return;
				}
				case 7:
				case 8:
				{
					return;
				}
				default:
				{
					opCode = last.get_OpCode();
					throw new NotSupportedException(String.Format("Unhandled instruction flow behavior {0}: {1}", opCode.get_FlowControl(), Formatter.FormatInstruction(last)));
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
			Collection<Instruction> instructions = this.body.get_Instructions();
			this.MarkBlockStarts(instructions);
			this.MarkBlockStarts(this.body.get_ExceptionHandlers());
			this.MarkBlockEnds(instructions);
		}

		private void FillOffsetToInstruction()
		{
			this.offsetToInstruction = new Dictionary<int, Instruction>();
			foreach (Instruction instruction in this.body.get_Instructions())
			{
				this.offsetToInstruction.Add(instruction.get_Offset(), instruction);
			}
		}

		private InstructionBlock GetBlock(Instruction firstInstruction)
		{
			InstructionBlock instructionBlocks;
			this.blocks.TryGetValue(firstInstruction.get_Offset(), out instructionBlocks);
			return instructionBlocks;
		}

		private static Instruction GetBranchTarget(Instruction instruction)
		{
			return (Instruction)instruction.get_Operand();
		}

		private InstructionBlock GetBranchTargetBlock(Instruction instruction)
		{
			return this.GetBlock(ControlFlowGraphBuilder.GetBranchTarget(instruction));
		}

		private static Instruction[] GetBranchTargets(Instruction instruction)
		{
			return (Instruction[])instruction.get_Operand();
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
			return instruction.get_OpCode().get_Code() == 68;
		}

		private static bool IsBlockDelimiter(Instruction instruction)
		{
			switch (instruction.get_OpCode().get_FlowControl())
			{
				case 0:
				case 1:
				case 3:
				case 7:
				case 8:
				{
					return true;
				}
				case 2:
				case 4:
				case 5:
				case 6:
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
				previous.Last = instructionBlocks.First.get_Previous();
				previous = instructionBlocks;
			}
			previous.Last = instructions.get_Item(instructions.get_Count() - 1);
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
			for (int i = 0; i < handlers.get_Count(); i++)
			{
				ExceptionHandler item = handlers.get_Item(i);
				this.MarkBlockStart(item.get_TryStart());
				this.MarkBlockStart(item.get_HandlerStart());
				if (item.get_HandlerType() == 1)
				{
					this.MarkExceptionObjectPosition(item.get_FilterStart());
					this.MarkBlockStart(item.get_FilterStart());
				}
				else if (item.get_HandlerType() == null)
				{
					this.MarkExceptionObjectPosition(item.get_HandlerStart());
				}
			}
		}

		private void MarkBlockStarts(Collection<Instruction> instructions)
		{
			for (int i = 0; i < instructions.get_Count(); i++)
			{
				Instruction item = instructions.get_Item(i);
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
					if (item.get_Next() != null)
					{
						this.MarkBlockStart(item.get_Next());
					}
				}
			}
		}

		private void MarkExceptionObjectPosition(Instruction instruction)
		{
			this.exceptionObjectsOffsets.Add(instruction.get_Offset());
		}

		private void RegisterBlock(InstructionBlock block)
		{
			this.blocks.Add(block.First.get_Offset(), block);
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