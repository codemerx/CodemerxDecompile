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
				InstructionBlock item = this.controlFlowGraph.InstructionToBlockMapping[exceptionHandler.get_HandlerStart().get_Offset()];
				if (exceptionHandler.get_HandlerType() == 4 || exceptionHandler.get_HandlerType() == 2)
				{
					this.RecursiveDfs(item, new Int32[0]);
				}
				else
				{
					if (exceptionHandler.get_HandlerType() == 1)
					{
						InstructionBlock instructionBlocks = this.controlFlowGraph.InstructionToBlockMapping[exceptionHandler.get_FilterStart().get_Offset()];
						this.RecursiveDfs(instructionBlocks, new Int32[] { -exceptionHandler.get_FilterStart().get_Offset() });
						Dictionary<int, VariableDefinition> variableDefinition = this.instructionOffsetToVariableDefinitionMap;
						int offset = -exceptionHandler.get_FilterStart().get_Offset();
						num = this.exceptionVariableCount;
						this.exceptionVariableCount = num + 1;
						variableDefinition[offset] = new VariableDefinition(String.Concat("exception_", num.ToString()), Utilities.GetCorlibTypeReference(typeof(Exception), this.methodContext.Method.get_Module()), this.methodContext.Method);
						this.exceptionVariableInstructionsSet.Add(-exceptionHandler.get_FilterStart().get_Offset());
					}
					this.RecursiveDfs(item, new Int32[] { -exceptionHandler.get_HandlerStart().get_Offset() });
					Dictionary<int, VariableDefinition> nums = this.instructionOffsetToVariableDefinitionMap;
					int offset1 = -exceptionHandler.get_HandlerStart().get_Offset();
					num = this.exceptionVariableCount;
					this.exceptionVariableCount = num + 1;
					nums[offset1] = new VariableDefinition(String.Concat("exception_", num.ToString()), exceptionHandler.get_CatchType() ?? Utilities.GetCorlibTypeReference(typeof(Exception), this.methodContext.Method.get_Module()), this.methodContext.Method);
					this.exceptionVariableInstructionsSet.Add(-exceptionHandler.get_HandlerStart().get_Offset());
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
				if (instruction.get_OpCode().get_Code() != 36)
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
						this.stackVariableInstructionsSet.Add(instruction.get_Offset());
					}
					for (int j = 0; (long)j < (ulong)pushDelta; j++)
					{
						nums.Add(instruction.get_Offset());
					}
					this.instructionOffsetToUsedInstructionsMap[instruction.get_Offset()] = new Stack<int>(nums1);
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
			OpCode opCode = instruction.get_OpCode();
			switch (opCode.get_StackBehaviourPop())
			{
				case 0:
				{
					return (uint)0;
				}
				case 1:
				case 3:
				case 10:
				{
					return (uint)1;
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
					return (uint)2;
				}
				case 7:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				{
					return (uint)3;
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
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
				case 27:
				{
					if (opCode.get_FlowControl() == 2)
					{
						IMethodSignature operand = (IMethodSignature)instruction.get_Operand();
						uint count = (uint)operand.get_Parameters().get_Count();
						if (OpCodes.Newobj.get_Value() != opCode.get_Value() && operand.get_HasThis())
						{
							count++;
						}
						if (opCode.get_Code() == 40)
						{
							count++;
						}
						return count;
					}
					if (opCode.get_Code() != 41)
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
			OpCode opCode = instruction.get_OpCode();
			switch (opCode.get_StackBehaviourPush())
			{
				case 19:
				{
					return (uint)0;
				}
				case 20:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				{
					return (uint)1;
				}
				case 21:
				{
					return (uint)2;
				}
				case 27:
				{
					throw new ArgumentException(Formatter.FormatInstruction(instruction));
				}
				case 28:
				{
					if (opCode.get_FlowControl() != 2)
					{
						throw new ArgumentException(Formatter.FormatInstruction(instruction));
					}
					if (!this.IsVoid(((IMethodSignature)instruction.get_Operand()).get_ReturnType()))
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
			if (type.get_IsPointer())
			{
				return false;
			}
			if (type is IModifierType)
			{
				return this.IsVoid((type as IModifierType).get_ElementType());
			}
			if (type.get_FullName() == "System.Void")
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