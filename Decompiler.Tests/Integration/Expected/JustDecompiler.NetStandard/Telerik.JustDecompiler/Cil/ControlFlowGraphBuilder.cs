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

		private readonly Dictionary<int, InstructionBlock> blocks;

		private readonly HashSet<int> exceptionObjectsOffsets;

		private readonly Dictionary<InstructionBlock, SwitchData> switchBlocksInformation;

		private Dictionary<int, Instruction> offsetToInstruction;

		internal ControlFlowGraphBuilder(MethodDefinition method)
		{
			this.blocks = new Dictionary<int, InstructionBlock>();
			this.switchBlocksInformation = new Dictionary<InstructionBlock, SwitchData>();
			base();
			this.body = method.get_Body();
			if (this.body.get_ExceptionHandlers().get_Count() > 0)
			{
				this.exceptionObjectsOffsets = new HashSet<int>();
			}
			return;
		}

		private static InstructionBlock[] AddBlock(InstructionBlock block, InstructionBlock[] blocks)
		{
			V_0 = new InstructionBlock[(int)blocks.Length + 1];
			Array.Copy(blocks, V_0, (int)blocks.Length);
			V_0[(int)V_0.Length - 1] = block;
			return V_0;
		}

		private static void ComputeIndexes(InstructionBlock[] blocks)
		{
			V_0 = 0;
			while (V_0 < (int)blocks.Length)
			{
				blocks[V_0].set_Index(V_0);
				V_0 = V_0 + 1;
			}
			return;
		}

		private void ConnectBlock(InstructionBlock block)
		{
			if (block.get_Last() == null)
			{
				V_1 = block.get_First().get_Offset();
				throw new ArgumentException(String.Concat("Undelimited block at offset ", V_1.ToString()));
			}
			V_0 = block.get_Last();
			switch (V_0.get_OpCode().get_FlowControl())
			{
				case 0:
				case 3:
				{
					if (!ControlFlowGraphBuilder.HasMultipleBranches(V_0))
					{
						V_4 = this.GetBranchTargetBlock(V_0);
						if (V_0.get_OpCode().get_FlowControl() != 3 || V_0.get_Next() == null)
						{
							stackVariable21 = new InstructionBlock[1];
							stackVariable21[0] = V_4;
							block.set_Successors(stackVariable21);
							return;
						}
						stackVariable28 = new InstructionBlock[2];
						stackVariable28[0] = V_4;
						stackVariable28[1] = this.GetBlock(V_0.get_Next());
						block.set_Successors(stackVariable28);
						return;
					}
					V_5 = this.GetBranchTargetsBlocks(V_0);
					V_6 = null;
					if (V_0.get_Next() != null)
					{
						V_6 = this.GetBlock(V_0.get_Next());
					}
					this.switchBlocksInformation.Add(block, new SwitchData(block, V_6, V_5));
					if (InstructionBlock.op_Inequality(V_6, null))
					{
						V_5 = ControlFlowGraphBuilder.AddBlock(V_6, V_5);
					}
					block.set_Successors(V_5);
					return;
				}
				case 1:
				case 4:
				case 6:
				{
				Label1:
					V_3 = V_0.get_OpCode();
					throw new NotSupportedException(String.Format("Unhandled instruction flow behavior {0}: {1}", V_3.get_FlowControl(), Formatter.FormatInstruction(V_0)));
				}
				case 2:
				case 5:
				{
					if (V_0.get_Next() == null)
					{
						goto Label0;
					}
					stackVariable75 = new InstructionBlock[1];
					stackVariable75[0] = this.GetBlock(V_0.get_Next());
					block.set_Successors(stackVariable75);
					return;
				}
				case 7:
				case 8:
				{
				Label0:
					return;
				}
				default:
				{
					goto Label1;
				}
			}
		}

		private void ConnectBlocks()
		{
			V_0 = this.blocks.get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.ConnectBlock(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
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
			V_0 = this.body.get_Instructions();
			this.MarkBlockStarts(V_0);
			this.MarkBlockStarts(this.body.get_ExceptionHandlers());
			this.MarkBlockEnds(V_0);
			return;
		}

		private void FillOffsetToInstruction()
		{
			this.offsetToInstruction = new Dictionary<int, Instruction>();
			V_0 = this.body.get_Instructions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.offsetToInstruction.Add(V_1.get_Offset(), V_1);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private InstructionBlock GetBlock(Instruction firstInstruction)
		{
			dummyVar0 = this.blocks.TryGetValue(firstInstruction.get_Offset(), out V_0);
			return V_0;
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
			V_0 = ControlFlowGraphBuilder.GetBranchTargets(instruction);
			V_1 = new InstructionBlock[(int)V_0.Length];
			V_2 = 0;
			while (V_2 < (int)V_0.Length)
			{
				V_1[V_2] = this.GetBlock(V_0[V_2]);
				V_2 = V_2 + 1;
			}
			return V_1;
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
				Label0:
					return false;
				}
				default:
				{
					goto Label0;
				}
			}
		}

		private void MarkBlockEnds(Collection<Instruction> instructions)
		{
			V_0 = this.ToArray();
			if (V_0.Length == 0)
			{
				return;
			}
			V_1 = V_0[0];
			V_2 = 1;
			while (V_2 < (int)V_0.Length)
			{
				V_3 = V_0[V_2];
				V_1.set_Last(V_3.get_First().get_Previous());
				V_1 = V_3;
				V_2 = V_2 + 1;
			}
			V_1.set_Last(instructions.get_Item(instructions.get_Count() - 1));
			return;
		}

		private void MarkBlockStart(Instruction instruction)
		{
			if (InstructionBlock.op_Inequality(this.GetBlock(instruction), null))
			{
				return;
			}
			this.RegisterBlock(new InstructionBlock(instruction));
			return;
		}

		private void MarkBlockStarts(Collection<ExceptionHandler> handlers)
		{
			V_0 = 0;
			while (V_0 < handlers.get_Count())
			{
				V_1 = handlers.get_Item(V_0);
				this.MarkBlockStart(V_1.get_TryStart());
				this.MarkBlockStart(V_1.get_HandlerStart());
				if (V_1.get_HandlerType() != 1)
				{
					if (V_1.get_HandlerType() == null)
					{
						this.MarkExceptionObjectPosition(V_1.get_HandlerStart());
					}
				}
				else
				{
					this.MarkExceptionObjectPosition(V_1.get_FilterStart());
					this.MarkBlockStart(V_1.get_FilterStart());
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private void MarkBlockStarts(Collection<Instruction> instructions)
		{
			V_0 = 0;
			while (V_0 < instructions.get_Count())
			{
				V_1 = instructions.get_Item(V_0);
				if (V_0 == 0)
				{
					this.MarkBlockStart(V_1);
				}
				if (ControlFlowGraphBuilder.IsBlockDelimiter(V_1))
				{
					if (!ControlFlowGraphBuilder.HasMultipleBranches(V_1))
					{
						V_5 = ControlFlowGraphBuilder.GetBranchTarget(V_1);
						if (V_5 != null)
						{
							this.MarkBlockStart(V_5);
						}
					}
					else
					{
						V_2 = ControlFlowGraphBuilder.GetBranchTargets(V_1);
						V_3 = 0;
						while (V_3 < (int)V_2.Length)
						{
							V_4 = V_2[V_3];
							if (V_4 != null)
							{
								this.MarkBlockStart(V_4);
							}
							V_3 = V_3 + 1;
						}
					}
					if (V_1.get_Next() != null)
					{
						this.MarkBlockStart(V_1.get_Next());
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private void MarkExceptionObjectPosition(Instruction instruction)
		{
			dummyVar0 = this.exceptionObjectsOffsets.Add(instruction.get_Offset());
			return;
		}

		private void RegisterBlock(InstructionBlock block)
		{
			this.blocks.Add(block.get_First().get_Offset(), block);
			return;
		}

		private InstructionBlock[] ToArray()
		{
			try
			{
				V_0 = new InstructionBlock[this.blocks.get_Count()];
				this.blocks.get_Values().CopyTo(V_0, 0);
				Array.Sort<InstructionBlock>(V_0);
				ControlFlowGraphBuilder.ComputeIndexes(V_0);
				V_1 = V_0;
			}
			catch (Exception exception_0)
			{
				throw new InvalidProgramException(exception_0.get_Message());
			}
			return V_1;
		}
	}
}