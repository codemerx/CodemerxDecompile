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
				return this.get_MethodBody().get_ExceptionHandlers();
			}
		}

		public Dictionary<InstructionBlock, SwitchData> SwitchBlocksInformation
		{
			get;
			private set;
		}

		public ControlFlowGraph(Mono.Cecil.Cil.MethodBody body, InstructionBlock[] blocks, Dictionary<int, InstructionBlock> instructiontoBlockMapping, Dictionary<InstructionBlock, SwitchData> switchBlocksInformation, Dictionary<int, Instruction> offsetToInstruction)
		{
			base();
			this.set_MethodBody(body);
			this.set_Blocks(blocks);
			this.set_InstructionToBlockMapping(instructiontoBlockMapping);
			this.set_SwitchBlocksInformation(switchBlocksInformation);
			this.set_OffsetToInstruction(offsetToInstruction);
			return;
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
			V_0 = this.get_Blocks()[index];
			V_1 = V_0.get_Last().get_Next();
			if (V_0.get_Predecessors().get_Count() > 0)
			{
				throw new Exception("The block to be removed cannot have predecessors");
			}
			V_2 = this.get_Blocks();
			this.set_Blocks(new InstructionBlock[(int)this.get_Blocks().Length - 1]);
			V_3 = 0;
			while (V_3 < (int)this.get_Blocks().Length)
			{
				if (V_3 >= index)
				{
					stackVariable29 = 1;
				}
				else
				{
					stackVariable29 = 0;
				}
				V_4 = stackVariable29;
				this.get_Blocks()[V_3] = V_2[V_3 + V_4];
				this.get_Blocks()[V_3].set_Index(V_3);
				dummyVar0 = this.get_Blocks()[V_3].get_Predecessors().Remove(V_0);
				if ((object)this.get_Blocks()[V_3].get_First().get_Previous() == (object)V_0.get_Last())
				{
					this.get_Blocks()[V_3].get_First().set_Previous(V_0.get_First().get_Previous());
				}
				if ((object)this.get_Blocks()[V_3].get_Last().get_Next() == (object)V_0.get_First())
				{
					this.get_Blocks()[V_3].get_Last().set_Next(V_0.get_Last().get_Next());
				}
				V_3 = V_3 + 1;
			}
			dummyVar1 = this.get_InstructionToBlockMapping().Remove(V_0.get_First().get_Offset());
			dummyVar2 = this.get_SwitchBlocksInformation().Remove(V_0);
			V_0.set_Successors(new InstructionBlock[0]);
			V_5 = V_0.GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					dummyVar3 = this.get_OffsetToInstruction().Remove(V_6.get_Offset());
				}
			}
			finally
			{
				if (V_5 != null)
				{
					V_5.Dispose();
				}
			}
			V_7 = this.get_MethodBody().get_ExceptionHandlers().GetEnumerator();
			try
			{
				while (V_7.MoveNext())
				{
					V_8 = V_7.get_Current();
					if ((object)V_8.get_TryStart() == (object)V_0.get_First())
					{
						V_8.set_TryStart(V_1);
					}
					if ((object)V_8.get_TryEnd() == (object)V_0.get_First())
					{
						V_8.set_TryEnd(V_1);
					}
					if ((object)V_8.get_HandlerStart() == (object)V_0.get_First())
					{
						V_8.set_HandlerStart(V_1);
					}
					if ((object)V_8.get_HandlerEnd() == (object)V_0.get_First())
					{
						V_8.set_HandlerEnd(V_1);
					}
					if ((object)V_8.get_FilterStart() == (object)V_0.get_First())
					{
						V_8.set_FilterStart(V_1);
					}
					if ((object)V_8.get_FilterEnd() != (object)V_0.get_First())
					{
						continue;
					}
					V_8.set_FilterEnd(V_1);
				}
			}
			finally
			{
				V_7.Dispose();
			}
			V_0.set_Index(-1);
			return;
		}
	}
}