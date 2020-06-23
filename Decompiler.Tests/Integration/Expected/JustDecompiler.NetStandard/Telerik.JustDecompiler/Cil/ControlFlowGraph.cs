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
				return this.MethodBody.ExceptionHandlers;
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
			if (!method.HasBody)
			{
				throw new ArgumentException();
			}
			return (new ControlFlowGraphBuilder(method)).CreateGraph();
		}

		internal void RemoveBlockAt(int index)
		{
			InstructionBlock blocks = this.Blocks[index];
			Instruction next = blocks.Last.Next;
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
				if (this.Blocks[i].First.Previous == blocks.Last)
				{
					this.Blocks[i].First.Previous = blocks.First.Previous;
				}
				if (this.Blocks[i].Last.Next == blocks.First)
				{
					this.Blocks[i].Last.Next = blocks.Last.Next;
				}
			}
			this.InstructionToBlockMapping.Remove(blocks.First.Offset);
			this.SwitchBlocksInformation.Remove(blocks);
			blocks.Successors = new InstructionBlock[0];
			foreach (Instruction block in blocks)
			{
				this.OffsetToInstruction.Remove(block.Offset);
			}
			foreach (ExceptionHandler exceptionHandler in this.MethodBody.ExceptionHandlers)
			{
				if (exceptionHandler.TryStart == blocks.First)
				{
					exceptionHandler.TryStart = next;
				}
				if (exceptionHandler.TryEnd == blocks.First)
				{
					exceptionHandler.TryEnd = next;
				}
				if (exceptionHandler.HandlerStart == blocks.First)
				{
					exceptionHandler.HandlerStart = next;
				}
				if (exceptionHandler.HandlerEnd == blocks.First)
				{
					exceptionHandler.HandlerEnd = next;
				}
				if (exceptionHandler.FilterStart == blocks.First)
				{
					exceptionHandler.FilterStart = next;
				}
				if (exceptionHandler.FilterEnd != blocks.First)
				{
					continue;
				}
				exceptionHandler.FilterEnd = next;
			}
			blocks.Index = -1;
		}
	}
}