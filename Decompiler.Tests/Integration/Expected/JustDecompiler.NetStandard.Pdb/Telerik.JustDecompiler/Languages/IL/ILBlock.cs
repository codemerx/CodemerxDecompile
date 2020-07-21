using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
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

		public readonly List<ILBlock> Children;

		public ILBlock(MethodBody body)
		{
			this(0, 0, body.get_CodeSize(), null);
			this.AddExceptionHandlerBlocks(body);
			this.AddBlocks(this.FindAllBranches(body));
			this.SortChildren();
			return;
		}

		public ILBlock(ILBlockType type, int startOffset, int endOffset, Mono.Cecil.Cil.ExceptionHandler handler = null)
		{
			this.Children = new List<ILBlock>();
			base();
			this.Type = type;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.ExceptionHandler = handler;
			return;
		}

		public ILBlock(ILBlockType type, int startOffset, int endOffset, Instruction loopEntryPoint)
		{
			this.Children = new List<ILBlock>();
			base();
			this.Type = type;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.LoopEntryPoint = loopEntryPoint;
			return;
		}

		private void AddBlocks(List<ILBlock.InstructionPair> allBranches)
		{
			V_0 = allBranches.get_Count() - 1;
			while (V_0 >= 0)
			{
				V_1 = allBranches.get_Item(V_0).get_FirstInstruction().get_Offset() + allBranches.get_Item(V_0).get_FirstInstruction().GetSize();
				V_2 = allBranches.get_Item(V_0).get_SecondInstruction().get_Offset();
				if (V_2 < V_1)
				{
					V_3 = null;
					V_4 = allBranches.get_Item(V_0).get_SecondInstruction().get_Previous();
					if (V_4 != null && !OpCodeInfo.IsUnconditionalBranch(V_4.get_OpCode()))
					{
						V_3 = allBranches.get_Item(V_0).get_SecondInstruction();
					}
					V_5 = false;
					V_6 = allBranches.GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_7 = V_6.get_Current();
							if (V_7.get_FirstInstruction().get_Offset() >= V_2 && V_7.get_FirstInstruction().get_Offset() < V_1 || V_2 > V_7.get_SecondInstruction().get_Offset() || V_7.get_SecondInstruction().get_Offset() >= V_1)
							{
								continue;
							}
							if (V_3 != null)
							{
								if ((object)V_7.get_SecondInstruction() == (object)V_3)
								{
									continue;
								}
								V_5 = true;
							}
							else
							{
								V_3 = V_7.get_SecondInstruction();
							}
						}
					}
					finally
					{
						((IDisposable)V_6).Dispose();
					}
					if (!V_5)
					{
						dummyVar0 = this.AddNestedBlock(new ILBlock(1, V_2, V_1, V_3));
					}
				}
				V_0 = V_0 - 1;
			}
			return;
		}

		private void AddExceptionHandlerBlocks(MethodBody body)
		{
			V_0 = 0;
			while (V_0 < body.get_ExceptionHandlers().get_Count())
			{
				V_1 = new ILBlock.u003cu003ec__DisplayClass8_0();
				V_1.eh = body.get_ExceptionHandlers().get_Item(V_0);
				if (!body.get_ExceptionHandlers().Take<Mono.Cecil.Cil.ExceptionHandler>(V_0).Any<Mono.Cecil.Cil.ExceptionHandler>(new Func<Mono.Cecil.Cil.ExceptionHandler, bool>(V_1.u003cAddExceptionHandlerBlocksu003eb__0)))
				{
					dummyVar0 = this.AddNestedBlock(new ILBlock(2, V_1.eh.get_TryStart().get_Offset(), V_1.eh.get_TryEnd().get_Offset(), V_1.eh));
				}
				if (V_1.eh.get_HandlerType() == 1)
				{
					dummyVar1 = this.AddNestedBlock(new ILBlock(4, V_1.eh.get_FilterStart().get_Offset(), V_1.eh.get_HandlerStart().get_Offset(), V_1.eh));
				}
				stackVariable28 = V_1.eh.get_HandlerStart().get_Offset();
				if (V_1.eh.get_HandlerEnd() == null)
				{
					stackVariable33 = body.get_CodeSize();
				}
				else
				{
					stackVariable33 = V_1.eh.get_HandlerEnd().get_Offset();
				}
				dummyVar2 = this.AddNestedBlock(new ILBlock(3, stackVariable28, stackVariable33, V_1.eh));
				V_0 = V_0 + 1;
			}
			return;
		}

		private bool AddNestedBlock(ILBlock newStructure)
		{
			if (this.Type == 1 && newStructure.Type == 1 && newStructure.StartOffset == this.StartOffset)
			{
				return false;
			}
			if (this.ShouldAddNestedBlock(newStructure, out V_0))
			{
				return V_0;
			}
			this.AddNestedBlocks(newStructure);
			this.Children.Add(newStructure);
			return true;
		}

		private void AddNestedBlocks(ILBlock newStructure)
		{
			V_0 = 0;
			while (V_0 < this.Children.get_Count())
			{
				V_1 = this.Children.get_Item(V_0);
				if (newStructure.StartOffset <= V_1.StartOffset && V_1.EndOffset <= newStructure.EndOffset)
				{
					stackVariable22 = V_0;
					V_0 = stackVariable22 - 1;
					this.Children.RemoveAt(stackVariable22);
					newStructure.Children.Add(V_1);
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private List<ILBlock.InstructionPair> FindAllBranches(MethodBody body)
		{
			V_0 = new List<ILBlock.InstructionPair>();
			V_1 = body.get_Instructions().GetEnumerator();
			try
			{
			Label0:
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_OpCode().get_OperandType();
					if (V_3 != null)
					{
						if (V_3 == 10)
						{
							V_5 = (Instruction[])V_2.get_Operand();
							V_6 = 0;
							while (V_6 < (int)V_5.Length)
							{
								V_0.Add(new ILBlock.InstructionPair(V_2, V_5[V_6]));
								V_6 = V_6 + 1;
							}
							goto Label0;
						}
						else
						{
							if (V_3 != 15)
							{
								continue;
							}
						}
					}
					V_0.Add(new ILBlock.InstructionPair(V_2, (Instruction)V_2.get_Operand()));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		public ILBlock GetInnermost(int offset)
		{
			V_0 = this.Children.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.StartOffset > offset || offset >= V_1.EndOffset)
					{
						continue;
					}
					V_2 = V_1.GetInnermost(offset);
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return this;
		}

		private bool ShouldAddNestedBlock(ILBlock newStructure, out bool returnValue)
		{
			returnValue = false;
			V_0 = this.Children.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.StartOffset > newStructure.StartOffset || newStructure.EndOffset > V_1.EndOffset)
					{
						if (V_1.EndOffset <= newStructure.StartOffset || newStructure.EndOffset <= V_1.StartOffset || newStructure.StartOffset <= V_1.StartOffset && V_1.EndOffset <= newStructure.EndOffset)
						{
							continue;
						}
						returnValue = false;
						V_2 = true;
						goto Label1;
					}
					else
					{
						returnValue = V_1.AddNestedBlock(newStructure);
						V_2 = true;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}

		private void SortChildren()
		{
			stackVariable1 = this.Children;
			stackVariable2 = ILBlock.u003cu003ec.u003cu003e9__15_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Comparison<ILBlock>(ILBlock.u003cu003ec.u003cu003e9.u003cSortChildrenu003eb__15_0);
				ILBlock.u003cu003ec.u003cu003e9__15_0 = stackVariable2;
			}
			stackVariable1.Sort(stackVariable2);
			V_0 = this.Children.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_0.get_Current().SortChildren();
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
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
				base();
				this.set_FirstInstruction(firstInstruction);
				this.set_SecondInstruction(secondInstruction);
				return;
			}
		}
	}
}