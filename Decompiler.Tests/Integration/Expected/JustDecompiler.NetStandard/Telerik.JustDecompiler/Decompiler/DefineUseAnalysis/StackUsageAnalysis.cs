using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
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

		private readonly Dictionary<int, VariableDefinition> instructionOffsetToVariableDefinitionMap = new Dictionary<int, VariableDefinition>();

		private readonly Dictionary<int, Stack<int>> instructionOffsetToUsedInstructionsMap = new Dictionary<int, Stack<int>>();

		private readonly UnionFinder<int> unionFinder = new UnionFinder<int>();

		private readonly HashSet<int> stackVariableInstructionsSet = new HashSet<int>();

		private readonly HashSet<int> exceptionVariableInstructionsSet = new HashSet<int>();

		private bool[] traversed;

		private MethodSpecificContext methodContext;

		public StackUsageAnalysis()
		{
			this.stackVariablesCount = 0;
			this.exceptionVariableCount = 0;
		}

		private void AnalyzeStackUsage()
		{
			int num;
			this.traversed = new Boolean[(int)this.controlFlowGraph.Blocks.Length];
			this.RecursiveDfs(this.controlFlowGraph.Blocks[0], new Int32[0]);
			foreach (ExceptionHandler exceptionHandler in this.exceptionHandlers)
			{
				InstructionBlock item = this.controlFlowGraph.InstructionToBlockMapping[exceptionHandler.HandlerStart.Offset];
				if (exceptionHandler.HandlerType == ExceptionHandlerType.Fault || exceptionHandler.HandlerType == ExceptionHandlerType.Finally)
				{
					this.RecursiveDfs(item, new Int32[0]);
				}
				else
				{
					if (exceptionHandler.HandlerType == ExceptionHandlerType.Filter)
					{
						InstructionBlock instructionBlocks = this.controlFlowGraph.InstructionToBlockMapping[exceptionHandler.FilterStart.Offset];
						this.RecursiveDfs(instructionBlocks, new Int32[] { -exceptionHandler.FilterStart.Offset });
						Dictionary<int, VariableDefinition> variableDefinition = this.instructionOffsetToVariableDefinitionMap;
						int offset = -exceptionHandler.FilterStart.Offset;
						num = this.exceptionVariableCount;
						this.exceptionVariableCount = num + 1;
						variableDefinition[offset] = new VariableDefinition(String.Concat("exception_", num.ToString()), Utilities.GetCorlibTypeReference(typeof(Exception), this.methodContext.Method.Module), this.methodContext.Method);
						this.exceptionVariableInstructionsSet.Add(-exceptionHandler.FilterStart.Offset);
					}
					this.RecursiveDfs(item, new Int32[] { -exceptionHandler.HandlerStart.Offset });
					Dictionary<int, VariableDefinition> nums = this.instructionOffsetToVariableDefinitionMap;
					int offset1 = -exceptionHandler.HandlerStart.Offset;
					num = this.exceptionVariableCount;
					this.exceptionVariableCount = num + 1;
					nums[offset1] = new VariableDefinition(String.Concat("exception_", num.ToString()), exceptionHandler.CatchType ?? Utilities.GetCorlibTypeReference(typeof(Exception), this.methodContext.Method.Module), this.methodContext.Method);
					this.exceptionVariableInstructionsSet.Add(-exceptionHandler.HandlerStart.Offset);
				}
			}
			for (int i = 0; i < (int)this.controlFlowGraph.Blocks.Length; i++)
			{
				if (!this.traversed[i])
				{
					throw new Exception("Unreachable block found");
				}
			}
		}

		private void ComputeStackUsage(InstructionBlock block)
		{
			List<int> nums = new List<int>(this.blockToInitialStackMap[block.Index]);
			foreach (Instruction instruction in block)
			{
				if (instruction.OpCode.Code != Code.Dup)
				{
					uint popDelta = this.GetPopDelta((uint)nums.Count, instruction);
					uint pushDelta = this.GetPushDelta(instruction);
					List<int> nums1 = new List<int>();
					for (int i = 0; (long)i < (ulong)popDelta; i++)
					{
						int count = nums.Count - 1;
						nums1.Add(nums[count]);
						nums.RemoveAt(count);
					}
					if (pushDelta > 0)
					{
						this.stackVariableInstructionsSet.Add(instruction.Offset);
					}
					for (int j = 0; (long)j < (ulong)pushDelta; j++)
					{
						nums.Add(instruction.Offset);
					}
					this.instructionOffsetToUsedInstructionsMap[instruction.Offset] = new Stack<int>(nums1);
				}
				else
				{
					if (nums.Count == 0)
					{
						throw new Exception("Invalid evaluation stack");
					}
					nums.Add(nums[nums.Count - 1]);
				}
			}
			this.blockToExitStackMap[block.Index] = nums.ToArray();
		}

		private void GenerateStackData()
		{
			StackUsageData stackUsageDatum = new StackUsageData();
			foreach (KeyValuePair<int, Stack<int>> keyValuePair in this.instructionOffsetToUsedInstructionsMap)
			{
				List<VariableDefinition> variableDefinitions = new List<VariableDefinition>(
					from offset in keyValuePair.Value
					select this.instructionOffsetToVariableDefinitionMap[offset]);
				stackUsageDatum.InstructionOffsetToUsedStackVariablesMap[keyValuePair.Key] = variableDefinitions;
				foreach (VariableDefinition variableDefinition in variableDefinitions)
				{
					this.GetDefineUseInfo(stackUsageDatum, variableDefinition).UsedAt.Add(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<int, VariableDefinition> keyValuePair1 in this.instructionOffsetToVariableDefinitionMap)
			{
				if (keyValuePair1.Key >= 0)
				{
					stackUsageDatum.InstructionOffsetToAssignedVariableMap.Add(keyValuePair1.Key, keyValuePair1.Value);
					this.GetDefineUseInfo(stackUsageDatum, keyValuePair1.Value).DefinedAt.Add(keyValuePair1.Key);
				}
				else
				{
					stackUsageDatum.ExceptionHandlerStartToExceptionVariableMap.Add(-keyValuePair1.Key, keyValuePair1.Value);
				}
			}
			this.methodContext.StackData = stackUsageDatum;
		}

		private void GenerateStackVariables()
		{
			VariableDefinition variableDefinition;
			foreach (int num in this.stackVariableInstructionsSet)
			{
				int num1 = this.unionFinder.Find(num);
				if (!this.instructionOffsetToVariableDefinitionMap.TryGetValue(num1, out variableDefinition))
				{
					int num2 = this.stackVariablesCount;
					this.stackVariablesCount = num2 + 1;
					variableDefinition = new VariableDefinition(String.Concat("stackVariable", num2.ToString()), null, this.methodContext.Method);
					this.instructionOffsetToVariableDefinitionMap.Add(num1, variableDefinition);
				}
				if (num1 == num)
				{
					continue;
				}
				this.instructionOffsetToVariableDefinitionMap.Add(num, variableDefinition);
			}
			foreach (int num3 in this.exceptionVariableInstructionsSet)
			{
				if (this.unionFinder.Find(num3) == num3)
				{
					continue;
				}
				throw new Exception("Invalid stack usage");
			}
		}

		private StackVariableDefineUseInfo GetDefineUseInfo(StackUsageData stackData, VariableDefinition varDef)
		{
			StackVariableDefineUseInfo stackVariableDefineUseInfo;
			if (!stackData.VariableToDefineUseInfo.TryGetValue(varDef, out stackVariableDefineUseInfo))
			{
				stackVariableDefineUseInfo = new StackVariableDefineUseInfo();
				stackData.VariableToDefineUseInfo.Add(varDef, stackVariableDefineUseInfo);
			}
			return stackVariableDefineUseInfo;
		}

		private uint GetPopDelta(uint stackHeight, Instruction instruction)
		{
			OpCode opCode = instruction.OpCode;
			switch (opCode.StackBehaviourPop)
			{
				case StackBehaviour.Pop0:
				{
					return (uint)0;
				}
				case StackBehaviour.Pop1:
				case StackBehaviour.Popi:
				case StackBehaviour.Popref:
				{
					return (uint)1;
				}
				case StackBehaviour.Pop1_pop1:
				case StackBehaviour.Popi_pop1:
				case StackBehaviour.Popi_popi:
				case StackBehaviour.Popi_popi8:
				case StackBehaviour.Popi_popr4:
				case StackBehaviour.Popi_popr8:
				case StackBehaviour.Popref_pop1:
				case StackBehaviour.Popref_popi:
				{
					return (uint)2;
				}
				case StackBehaviour.Popi_popi_popi:
				case StackBehaviour.Popref_popi_popi:
				case StackBehaviour.Popref_popi_popi8:
				case StackBehaviour.Popref_popi_popr4:
				case StackBehaviour.Popref_popi_popr8:
				case StackBehaviour.Popref_popi_popref:
				{
					return (uint)3;
				}
				case StackBehaviour.PopAll:
				{
					return stackHeight;
				}
				case StackBehaviour.Push0:
				case StackBehaviour.Push1:
				case StackBehaviour.Push1_push1:
				case StackBehaviour.Pushi:
				case StackBehaviour.Pushi8:
				case StackBehaviour.Pushr4:
				case StackBehaviour.Pushr8:
				case StackBehaviour.Pushref:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
				case StackBehaviour.Varpop:
				{
					if (opCode.FlowControl == FlowControl.Call)
					{
						IMethodSignature operand = (IMethodSignature)instruction.Operand;
						uint count = (uint)operand.Parameters.Count;
						if (OpCodes.Newobj.Value != opCode.Value && operand.HasThis)
						{
							count++;
						}
						if (opCode.Code == Code.Calli)
						{
							count++;
						}
						return count;
					}
					if (opCode.Code != Code.Ret)
					{
						throw new ArgumentException(Formatter.FormatInstruction(instruction));
					}
					return stackHeight;
				}
				default:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
			}
		}

		private uint GetPushDelta(Instruction instruction)
		{
			OpCode opCode = instruction.OpCode;
			switch (opCode.StackBehaviourPush)
			{
				case StackBehaviour.Push0:
				{
					return (uint)0;
				}
				case StackBehaviour.Push1:
				case StackBehaviour.Pushi:
				case StackBehaviour.Pushi8:
				case StackBehaviour.Pushr4:
				case StackBehaviour.Pushr8:
				case StackBehaviour.Pushref:
				{
					return (uint)1;
				}
				case StackBehaviour.Push1_push1:
				{
					return (uint)2;
				}
				case StackBehaviour.Varpop:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
				case StackBehaviour.Varpush:
				{
					if (opCode.FlowControl != FlowControl.Call)
					{
						throw new ArgumentException(Formatter.FormatInstruction(instruction));
					}
					if (!this.IsVoid(((IMethodSignature)instruction.Operand).ReturnType))
					{
						return (uint)1;
					}
					return (uint)0;
				}
				default:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
			}
		}

		private bool IsVoid(TypeReference type)
		{
			if (type.IsPointer)
			{
				return false;
			}
			if (type is IModifierType)
			{
				return this.IsVoid((type as IModifierType).ElementType);
			}
			if (type.FullName == "System.Void")
			{
				return true;
			}
			return false;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.controlFlowGraph = this.methodContext.ControlFlowGraph;
			this.exceptionHandlers = this.controlFlowGraph.RawExceptionHandlers;
			this.blockToInitialStackMap = new int[(int)this.controlFlowGraph.Blocks.Length][];
			this.blockToExitStackMap = new int[(int)this.controlFlowGraph.Blocks.Length][];
			this.AnalyzeStackUsage();
			this.GenerateStackVariables();
			this.GenerateStackData();
			return body;
		}

		private void RecursiveDfs(InstructionBlock currentBlock, int[] initialStack)
		{
			this.traversed[currentBlock.Index] = true;
			this.blockToInitialStackMap[currentBlock.Index] = initialStack;
			this.ComputeStackUsage(currentBlock);
			int[] numArray = this.blockToExitStackMap[currentBlock.Index];
			InstructionBlock[] successors = currentBlock.Successors;
			for (int i = 0; i < (int)successors.Length; i++)
			{
				InstructionBlock instructionBlocks = successors[i];
				if (this.traversed[instructionBlocks.Index])
				{
					this.UpdateCurrentStackVariables(currentBlock, instructionBlocks);
				}
				else
				{
					this.RecursiveDfs(instructionBlocks, numArray);
				}
			}
		}

		private void UpdateCurrentStackVariables(InstructionBlock parent, InstructionBlock successor)
		{
			int[] numArray = this.blockToExitStackMap[parent.Index];
			int[] numArray1 = this.blockToInitialStackMap[successor.Index];
			if ((int)numArray.Length != (int)numArray1.Length)
			{
				throw new ArgumentException("Two paths with different stack states encountered.");
			}
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				this.unionFinder.Union(numArray1[i], numArray[i]);
			}
		}
	}
}