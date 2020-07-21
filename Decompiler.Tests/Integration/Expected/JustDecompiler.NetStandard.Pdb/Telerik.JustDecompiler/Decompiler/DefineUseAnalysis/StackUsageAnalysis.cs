using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.DefineUseAnalysis
{
	internal class StackUsageAnalysis : IDecompilationStep
	{
		private ICollection<ExceptionHandler> exceptionHandlers;

		private const string StackVariablePrefix = "stackVariable";

		private const string ExceptionVariablePrefix = "exception_";

		private int stackVariablesCount;

		private int exceptionVariableCount;

		private ControlFlowGraph controlFlowGraph;

		private int[][] blockToInitialStackMap;

		private int[][] blockToExitStackMap;

		private readonly Dictionary<int, VariableDefinition> instructionOffsetToVariableDefinitionMap;

		private readonly Dictionary<int, Stack<int>> instructionOffsetToUsedInstructionsMap;

		private readonly UnionFinder<int> unionFinder;

		private readonly HashSet<int> stackVariableInstructionsSet;

		private readonly HashSet<int> exceptionVariableInstructionsSet;

		private bool[] traversed;

		private MethodSpecificContext methodContext;

		public StackUsageAnalysis()
		{
			this.instructionOffsetToVariableDefinitionMap = new Dictionary<int, VariableDefinition>();
			this.instructionOffsetToUsedInstructionsMap = new Dictionary<int, Stack<int>>();
			this.unionFinder = new UnionFinder<int>();
			this.stackVariableInstructionsSet = new HashSet<int>();
			this.exceptionVariableInstructionsSet = new HashSet<int>();
			base();
			this.stackVariablesCount = 0;
			this.exceptionVariableCount = 0;
			return;
		}

		private void AnalyzeStackUsage()
		{
			this.traversed = new Boolean[(int)this.controlFlowGraph.get_Blocks().Length];
			this.RecursiveDfs(this.controlFlowGraph.get_Blocks()[0], new Int32[0]);
			V_0 = this.exceptionHandlers.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.controlFlowGraph.get_InstructionToBlockMapping().get_Item(V_1.get_HandlerStart().get_Offset());
					if (V_1.get_HandlerType() == 4 || V_1.get_HandlerType() == 2)
					{
						this.RecursiveDfs(V_2, new Int32[0]);
					}
					else
					{
						if (V_1.get_HandlerType() == 1)
						{
							V_3 = this.controlFlowGraph.get_InstructionToBlockMapping().get_Item(V_1.get_FilterStart().get_Offset());
							stackVariable96 = new Int32[1];
							stackVariable96[0] = -V_1.get_FilterStart().get_Offset();
							this.RecursiveDfs(V_3, stackVariable96);
							stackVariable103 = this.instructionOffsetToVariableDefinitionMap;
							stackVariable107 = -V_1.get_FilterStart().get_Offset();
							V_4 = this.exceptionVariableCount;
							this.exceptionVariableCount = V_4 + 1;
							stackVariable103.set_Item(stackVariable107, new VariableDefinition(String.Concat("exception_", V_4.ToString()), Utilities.GetCorlibTypeReference(Type.GetTypeFromHandle(// 
							// Current member / type: System.Void Telerik.JustDecompiler.Decompiler.DefineUseAnalysis.StackUsageAnalysis::AnalyzeStackUsage()
							// Exception in: System.Void AnalyzeStackUsage()
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		private void ComputeStackUsage(InstructionBlock block)
		{
			V_0 = new List<int>(this.blockToInitialStackMap[block.get_Index()]);
			V_1 = block.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_OpCode().get_Code() != 36)
					{
						V_3 = this.GetPopDelta(V_0.get_Count(), V_2);
						V_4 = this.GetPushDelta(V_2);
						V_5 = new List<int>();
						V_7 = 0;
						while ((long)V_7 < (ulong)V_3)
						{
							V_8 = V_0.get_Count() - 1;
							V_5.Add(V_0.get_Item(V_8));
							V_0.RemoveAt(V_8);
							V_7 = V_7 + 1;
						}
						if (V_4 > 0)
						{
							dummyVar0 = this.stackVariableInstructionsSet.Add(V_2.get_Offset());
						}
						V_9 = 0;
						while ((long)V_9 < (ulong)V_4)
						{
							V_0.Add(V_2.get_Offset());
							V_9 = V_9 + 1;
						}
						this.instructionOffsetToUsedInstructionsMap.set_Item(V_2.get_Offset(), new Stack<int>(V_5));
					}
					else
					{
						if (V_0.get_Count() == 0)
						{
							throw new Exception("Invalid evaluation stack");
						}
						V_0.Add(V_0.get_Item(V_0.get_Count() - 1));
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.blockToExitStackMap[block.get_Index()] = V_0.ToArray();
			return;
		}

		private void GenerateStackData()
		{
			V_0 = new StackUsageData();
			V_1 = this.instructionOffsetToUsedInstructionsMap.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = new List<VariableDefinition>(V_2.get_Value().Select<int, VariableDefinition>(new Func<int, VariableDefinition>(this.u003cGenerateStackDatau003eb__25_0)));
					V_0.get_InstructionOffsetToUsedStackVariablesMap().set_Item(V_2.get_Key(), V_3);
					V_4 = V_3.GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							this.GetDefineUseInfo(V_0, V_5).get_UsedAt().Add(V_2.get_Key());
						}
					}
					finally
					{
						((IDisposable)V_4).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			V_6 = this.instructionOffsetToVariableDefinitionMap.GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = V_6.get_Current();
					if (V_7.get_Key() >= 0)
					{
						V_0.get_InstructionOffsetToAssignedVariableMap().Add(V_7.get_Key(), V_7.get_Value());
						dummyVar0 = this.GetDefineUseInfo(V_0, V_7.get_Value()).get_DefinedAt().Add(V_7.get_Key());
					}
					else
					{
						V_0.get_ExceptionHandlerStartToExceptionVariableMap().Add(-V_7.get_Key(), V_7.get_Value());
					}
				}
			}
			finally
			{
				((IDisposable)V_6).Dispose();
			}
			this.methodContext.set_StackData(V_0);
			return;
		}

		private void GenerateStackVariables()
		{
			V_0 = this.stackVariableInstructionsSet.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_3 = this.unionFinder.Find(V_1);
					if (!this.instructionOffsetToVariableDefinitionMap.TryGetValue(V_3, out V_2))
					{
						V_4 = this.stackVariablesCount;
						this.stackVariablesCount = V_4 + 1;
						V_2 = new VariableDefinition(String.Concat("stackVariable", V_4.ToString()), null, this.methodContext.get_Method());
						this.instructionOffsetToVariableDefinitionMap.Add(V_3, V_2);
					}
					if (V_3 == V_1)
					{
						continue;
					}
					this.instructionOffsetToVariableDefinitionMap.Add(V_1, V_2);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			V_0 = this.exceptionVariableInstructionsSet.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_5 = V_0.get_Current();
					if (this.unionFinder.Find(V_5) == V_5)
					{
						continue;
					}
					throw new Exception("Invalid stack usage");
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private StackVariableDefineUseInfo GetDefineUseInfo(StackUsageData stackData, VariableDefinition varDef)
		{
			if (!stackData.get_VariableToDefineUseInfo().TryGetValue(varDef, out V_0))
			{
				V_0 = new StackVariableDefineUseInfo();
				stackData.get_VariableToDefineUseInfo().Add(varDef, V_0);
			}
			return V_0;
		}

		private uint GetPopDelta(uint stackHeight, Instruction instruction)
		{
			V_0 = instruction.get_OpCode();
			switch (V_0.get_StackBehaviourPop())
			{
				case 0:
				{
					return 0;
				}
				case 1:
				case 3:
				case 10:
				{
					return 1;
				}
				case 2:
				case 4:
				case 5:
				case 6:
				case 8:
				case 9:
				case 11:
				case 12:
				{
					return 2;
				}
				case 7:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				{
					return 3;
				}
				case 18:
				{
					return stackHeight;
				}
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				{
				Label0:
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
				case 27:
				{
					if (V_0.get_FlowControl() == 2)
					{
						V_2 = (IMethodSignature)instruction.get_Operand();
						V_3 = V_2.get_Parameters().get_Count();
						if (OpCodes.Newobj.get_Value() != V_0.get_Value() && V_2.get_HasThis())
						{
							V_3 = V_3 + 1;
						}
						if (V_0.get_Code() == 40)
						{
							V_3 = V_3 + 1;
						}
						return V_3;
					}
					if (V_0.get_Code() != 41)
					{
						goto Label0;
					}
					return stackHeight;
				}
				default:
				{
					goto Label0;
				}
			}
		}

		private uint GetPushDelta(Instruction instruction)
		{
			V_0 = instruction.get_OpCode();
			switch (V_0.get_StackBehaviourPush() - 19)
			{
				case 0:
				{
					return 0;
				}
				case 1:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				{
					return 1;
				}
				case 2:
				{
					return 2;
				}
				case 8:
				{
				Label0:
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
				case 9:
				{
					if (V_0.get_FlowControl() != 2)
					{
						goto Label0;
					}
					if (!this.IsVoid(((IMethodSignature)instruction.get_Operand()).get_ReturnType()))
					{
						return 1;
					}
					return 0;
				}
				default:
				{
					goto Label0;
				}
			}
		}

		private bool IsVoid(TypeReference type)
		{
			if (type.get_IsPointer())
			{
				return false;
			}
			if (type as IModifierType != null)
			{
				return this.IsVoid((type as IModifierType).get_ElementType());
			}
			if (String.op_Equality(type.get_FullName(), "System.Void"))
			{
				return true;
			}
			return false;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.controlFlowGraph = this.methodContext.get_ControlFlowGraph();
			this.exceptionHandlers = this.controlFlowGraph.get_RawExceptionHandlers();
			this.blockToInitialStackMap = new int[(int)this.controlFlowGraph.get_Blocks().Length][];
			this.blockToExitStackMap = new int[(int)this.controlFlowGraph.get_Blocks().Length][];
			this.AnalyzeStackUsage();
			this.GenerateStackVariables();
			this.GenerateStackData();
			return body;
		}

		private void RecursiveDfs(InstructionBlock currentBlock, int[] initialStack)
		{
			this.traversed[currentBlock.get_Index()] = true;
			this.blockToInitialStackMap[currentBlock.get_Index()] = initialStack;
			this.ComputeStackUsage(currentBlock);
			V_0 = this.blockToExitStackMap[currentBlock.get_Index()];
			V_1 = currentBlock.get_Successors();
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (this.traversed[V_3.get_Index()])
				{
					this.UpdateCurrentStackVariables(currentBlock, V_3);
				}
				else
				{
					this.RecursiveDfs(V_3, V_0);
				}
				V_2 = V_2 + 1;
			}
			return;
		}

		private void UpdateCurrentStackVariables(InstructionBlock parent, InstructionBlock successor)
		{
			V_0 = this.blockToExitStackMap[parent.get_Index()];
			V_1 = this.blockToInitialStackMap[successor.get_Index()];
			if ((int)V_0.Length != (int)V_1.Length)
			{
				throw new ArgumentException("Two paths with different stack states encountered.");
			}
			V_2 = 0;
			while (V_2 < (int)V_0.Length)
			{
				this.unionFinder.Union(V_1[V_2], V_0[V_2]);
				V_2 = V_2 + 1;
			}
			return;
		}
	}
}