using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Cil
{
	public class ControlFlowGraph
	{
		public InstructionBlock[] Blocks
		{
			get;
			private set;
		}

		public Dictionary<int, InstructionBlock> InstructionToBlockMapping
		{
			get;
			private set;
		}

		public Mono.Cecil.Cil.MethodBody MethodBody
		{
			get;
			private set;
		}

		public Dictionary<int, Instruction> OffsetToInstruction
		{
			get;
			private set;
		}

		public Collection<ExceptionHandler> RawExceptionHandlers
		{
			get
			{
				return this.MethodBody.get_ExceptionHandlers();
			}
		}

		public Dictionary<InstructionBlock, SwitchData> SwitchBlocksInformation
		{
			get;
			private set;
		}

		public ControlFlowGraph(Mono.Cecil.Cil.MethodBody body, InstructionBlock[] blocks, Dictionary<int, InstructionBlock> instructiontoBlockMapping, Dictionary<InstructionBlock, SwitchData> switchBlocksInformation, Dictionary<int, Instruction> offsetToInstruction)
		{
			this.MethodBody = body;
			this.Blocks = blocks;
			this.InstructionToBlockMapping = instructiontoBlockMapping;
			this.SwitchBlocksInformation = switchBlocksInformation;
			this.OffsetToInstruction = offsetToInstruction;
		}

		public static ControlFlowGraph Create(MethodDefinition method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (!method.get_HasBody())
			{
				throw new ArgumentException();
			}
			return (new ControlFlowGraphBuilder(method)).CreateGraph();
		}

		internal void RemoveBlockAt(int index)
		{
			InstructionBlock blocks = this.Blocks[index];
			Instruction next = blocks.Last.get_Next();
			if (blocks.Predecessors.Count > 0)
			{
				throw new Exception("The block to be removed cannot have predecessors");
			}
			InstructionBlock[] instructionBlockArrays = this.Blocks;
			this.Blocks = new InstructionBlock[(int)this.Blocks.Length - 1];
			for (int i = 0; i < (int)this.Blocks.Length; i++)
			{
				int num = (i >= index ? 1 : 0);
				this.Blocks[i] = instructionBlockArrays[i + num];
				this.Blocks[i].Index = i;
				this.Blocks[i].Predecessors.Remove(blocks);
				if ((object)this.Blocks[i].First.get_Previous() == (object)blocks.Last)
				{
					this.Blocks[i].First.set_Previous(blocks.First.get_Previous());
				}
				if ((object)this.Blocks[i].Last.get_Next() == (object)blocks.First)
				{
					this.Blocks[i].Last.set_Next(blocks.Last.get_Next());
				}
			}
			this.InstructionToBlockMapping.Remove(blocks.First.get_Offset());
			this.SwitchBlocksInformation.Remove(blocks);
			blocks.Successors = new InstructionBlock[0];
			foreach (Instruction block in blocks)
			{
				this.OffsetToInstruction.Remove(block.get_Offset());
			}
			foreach (ExceptionHandler exceptionHandler in this.MethodBody.get_ExceptionHandlers())
			{
				if ((object)exceptionHandler.get_TryStart() == (object)blocks.First)
				{
					exceptionHandler.set_TryStart(next);
				}
				if ((object)exceptionHandler.get_TryEnd() == (object)blocks.First)
				{
					exceptionHandler.set_TryEnd(next);
				}
				if ((object)exceptionHandler.get_HandlerStart() == (object)blocks.First)
				{
					exceptionHandler.set_HandlerStart(next);
				}
				if ((object)exceptionHandler.get_HandlerEnd() == (object)blocks.First)
				{
					exceptionHandler.set_HandlerEnd(next);
				}
				if ((object)exceptionHandler.get_FilterStart() == (object)blocks.First)
				{
					exceptionHandler.set_FilterStart(next);
				}
				if ((object)exceptionHandler.get_FilterEnd() != (object)blocks.First)
				{
					continue;
				}
				exceptionHandler.set_FilterEnd(next);
			}
			blocks.Index = -1;
		}
	}
}