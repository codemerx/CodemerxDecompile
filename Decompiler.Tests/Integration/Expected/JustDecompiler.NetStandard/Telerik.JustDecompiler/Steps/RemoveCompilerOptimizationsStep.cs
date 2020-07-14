using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveCompilerOptimizationsStep : IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private Dictionary<int, IList<Expression>> blockExpressions;

		private Dictionary<int, InstructionBlock> instructionToBlockMapping;

		private List<int> blocksToBeRemoved;

		private Dictionary<int, List<int>> switchBlocksToCasesMap;

		public RemoveCompilerOptimizationsStep()
		{
		}

		private bool IsCaseBlock(IList<Expression> expressions, Expression switchExpression)
		{
			if (expressions.Count != 1 || expressions[0].CodeNodeType != CodeNodeType.UnaryExpression)
			{
				return false;
			}
			UnaryExpression item = expressions[0] as UnaryExpression;
			if (item.Operator != UnaryOperator.None || item.Operand.CodeNodeType != CodeNodeType.MethodInvocationExpression)
			{
				return false;
			}
			MethodInvocationExpression operand = item.Operand as MethodInvocationExpression;
			if (operand.MethodExpression.Method.get_Name() != "op_Equality" || operand.Arguments[1].CodeNodeType != CodeNodeType.LiteralExpression || (operand.Arguments[1] as LiteralExpression).ExpressionType.get_FullName() != "System.String")
			{
				return false;
			}
			if (!operand.Arguments[0].Equals(switchExpression))
			{
				return false;
			}
			return true;
		}

		private bool IsEmptyStringCaseBlock(IList<Expression> expressions, Expression switchExpression)
		{
			if (expressions.Count != 1 || expressions[0].CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression item = expressions[0] as BinaryExpression;
			if (item.Operator != BinaryOperator.ValueEquality || item.Left.CodeNodeType != CodeNodeType.MethodInvocationExpression || item.Right.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}
			MethodInvocationExpression left = item.Left as MethodInvocationExpression;
			if (left.MethodExpression.Method.get_FullName() != "System.Int32 System.String::get_Length()" || left.MethodExpression.Target == null)
			{
				return false;
			}
			LiteralExpression right = item.Right as LiteralExpression;
			if (right.ExpressionType.get_FullName() != "System.Int32" && (Int32)right.Value != 0)
			{
				return false;
			}
			if (!left.MethodExpression.Target.Equals(switchExpression))
			{
				return false;
			}
			return true;
		}

		private bool IsNullCaseBlock(IList<Expression> expressions, Expression switchExpression)
		{
			if (expressions.Count != 1)
			{
				return false;
			}
			if (expressions[0].CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression item = expressions[0] as BinaryExpression;
			if (item.Operator != BinaryOperator.ValueEquality || item.Right.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}
			LiteralExpression right = item.Right as LiteralExpression;
			if (right.ExpressionType.get_FullName() != "System.Object" || right.Value != null)
			{
				return false;
			}
			if (!item.Left.Equals(switchExpression))
			{
				return false;
			}
			return true;
		}

		private bool IsOptimizationBlock(IList<Expression> expressions, VariableReference optimizationVariable)
		{
			if (expressions.Count != 1 || expressions[0].CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression item = expressions[0] as BinaryExpression;
			if (item.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression && item.Right.CodeNodeType == CodeNodeType.LiteralExpression && item.IsComparisonExpression && (object)(item.Left as VariableReferenceExpression).Variable == (object)optimizationVariable)
			{
				return true;
			}
			return false;
		}

		private bool IsUnconditionJumpBlock(InstructionBlock block)
		{
			if ((object)block.First != (object)block.Last || block.First.get_OpCode().get_Code() != 55 && block.First.get_OpCode().get_Code() != 42)
			{
				return false;
			}
			return (int)block.Successors.Length == 1;
		}

		private void MarkOptimizationAndCaseBlocks(InstructionBlock block, RemoveCompilerOptimizationsStep.SwitchData data)
		{
			Queue<InstructionBlock> instructionBlocks = new Queue<InstructionBlock>();
			HashSet<int> nums = new HashSet<int>();
			InstructionBlock[] successors = block.Successors;
			for (int i = 0; i < (int)successors.Length; i++)
			{
				instructionBlocks.Enqueue(successors[i]);
			}
			while (instructionBlocks.Count > 0)
			{
				InstructionBlock last = instructionBlocks.Dequeue();
				nums.Add(last.First.get_Offset());
				if (!this.IsOptimizationBlock(this.blockExpressions[last.First.get_Offset()], data.OptimizationVariable))
				{
					if (!this.IsCaseBlock(this.blockExpressions[last.First.get_Offset()], data.SwitchExpression) && !this.IsNullCaseBlock(this.blockExpressions[last.First.get_Offset()], data.SwitchExpression))
					{
						continue;
					}
					this.switchBlocksToCasesMap[block.First.get_Offset()].Add(last.First.get_Offset());
					InstructionBlock successors1 = last.Successors[1];
					if (this.IsEmptyStringCaseBlock(this.blockExpressions[successors1.First.get_Offset()], data.SwitchExpression))
					{
						last.Last = successors1.Last;
						last.Successors = successors1.Successors;
						BinaryExpression item = this.blockExpressions[last.First.get_Offset()][0] as BinaryExpression;
						item.Right = new LiteralExpression("", this.methodContext.Method.get_Module().get_TypeSystem(), null);
						IEnumerable<Instruction> underlyingSameMethodInstructions = this.blockExpressions[successors1.First.get_Offset()][0].UnderlyingSameMethodInstructions;
						item = item.CloneAndAttachInstructions(underlyingSameMethodInstructions) as BinaryExpression;
						this.blockExpressions[last.First.get_Offset()][0] = new UnaryExpression(UnaryOperator.None, item, null);
						this.blocksToBeRemoved.Add(successors1.First.get_Offset());
					}
					this.MarkSecondSuccessorForRemovalIfItIsUnconditionalJump(last);
				}
				else
				{
					InstructionBlock instructionBlocks1 = last.Successors[0];
					if (!nums.Contains(instructionBlocks1.First.get_Offset()))
					{
						instructionBlocks.Enqueue(instructionBlocks1);
					}
					InstructionBlock instructionBlocks2 = this.MarkSecondSuccessorForRemovalIfItIsUnconditionalJump(last);
					if (!nums.Contains(instructionBlocks2.First.get_Offset()) && this.IsOptimizationBlock(this.blockExpressions[last.First.get_Offset()], data.OptimizationVariable))
					{
						instructionBlocks.Enqueue(instructionBlocks2);
					}
					this.blocksToBeRemoved.Add(last.First.get_Offset());
				}
			}
		}

		private InstructionBlock MarkSecondSuccessorForRemovalIfItIsUnconditionalJump(InstructionBlock block)
		{
			InstructionBlock successors = block.Successors[1];
			if (this.IsUnconditionJumpBlock(successors))
			{
				block.RemoveFromSuccessors(successors);
				block.AddToSuccessors(successors.Successors[0]);
				this.blocksToBeRemoved.Add(successors.First.get_Offset());
				successors = successors.Successors[0];
			}
			return successors;
		}

		private void MergeFirstCFGBlockWithFirstCaseIf(int index)
		{
			InstructionBlock item = this.instructionToBlockMapping[this.switchBlocksToCasesMap[index][0]];
			InstructionBlock last = this.instructionToBlockMapping[index];
			last.Last = item.Last;
			last.Successors = item.Successors;
			foreach (Expression expression in this.blockExpressions[item.First.get_Offset()])
			{
				this.blockExpressions[index].Add(expression);
			}
			this.blocksToBeRemoved.Add(item.First.get_Offset());
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.blockExpressions = this.methodContext.Expressions.BlockExpressions;
			this.instructionToBlockMapping = this.methodContext.ControlFlowGraph.InstructionToBlockMapping;
			this.blocksToBeRemoved = new List<int>();
			this.switchBlocksToCasesMap = this.methodContext.SwitchByStringData.SwitchBlocksToCasesMap;
			this.RemoveSwitchByStringOptimizations();
			return body;
		}

		private void ReconnectBlocks(int index)
		{
			InstructionBlock item = this.instructionToBlockMapping[index];
			for (int i = 1; i < this.switchBlocksToCasesMap[index].Count; i++)
			{
				InstructionBlock instructionBlocks = this.instructionToBlockMapping[this.switchBlocksToCasesMap[index][i]];
				item.RemoveFromSuccessors(item.Successors.Last<InstructionBlock>());
				item.AddToSuccessors(instructionBlocks);
				item = instructionBlocks;
			}
		}

		private void RemoveExpressionsFromFirstBlock(int index)
		{
			this.blockExpressions[index].RemoveAt(this.blockExpressions[index].Count - 1);
			this.blockExpressions[index].RemoveAt(this.blockExpressions[index].Count - 1);
		}

		private bool RemoveOptimizationBlocks()
		{
			this.blocksToBeRemoved.Sort();
			foreach (int num in this.blocksToBeRemoved)
			{
				this.methodContext.ControlFlowGraph.RemoveBlockAt(this.instructionToBlockMapping[num].Index);
				this.blockExpressions.Remove(num);
			}
			return this.blocksToBeRemoved.Count > 0;
		}

		private void RemoveSwitchByStringOptimizations()
		{
			RemoveCompilerOptimizationsStep.SwitchData switchDatum;
			foreach (int switchBlocksStartInstruction in this.methodContext.SwitchByStringData.SwitchBlocksStartInstructions)
			{
				if (!this.TryGetSwitchData(this.blockExpressions[switchBlocksStartInstruction], out switchDatum))
				{
					continue;
				}
				this.switchBlocksToCasesMap.Add(switchBlocksStartInstruction, new List<int>());
				this.MarkOptimizationAndCaseBlocks(this.instructionToBlockMapping[switchBlocksStartInstruction], switchDatum);
				this.switchBlocksToCasesMap[switchBlocksStartInstruction].Sort();
				this.RemoveExpressionsFromFirstBlock(switchBlocksStartInstruction);
				this.MergeFirstCFGBlockWithFirstCaseIf(switchBlocksStartInstruction);
				this.ReconnectBlocks(switchBlocksStartInstruction);
			}
			if (this.RemoveOptimizationBlocks())
			{
				this.methodContext.IsMethodBodyChanged = true;
			}
		}

		private bool TryGetSwitchData(IList<Expression> expressions, out RemoveCompilerOptimizationsStep.SwitchData data)
		{
			data = null;
			if (expressions.Count < 2 || expressions[expressions.Count - 2].CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression item = expressions[expressions.Count - 2] as BinaryExpression;
			if (!item.IsAssignmentExpression)
			{
				return false;
			}
			if (item.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || item.Right.CodeNodeType != CodeNodeType.MethodInvocationExpression)
			{
				return false;
			}
			MethodInvocationExpression right = item.Right as MethodInvocationExpression;
			if (!Utilities.IsComputeStringHashMethod(right.MethodExpression.Method))
			{
				return false;
			}
			data = new RemoveCompilerOptimizationsStep.SwitchData((item.Left as VariableReferenceExpression).Variable, right.Arguments[0]);
			return true;
		}

		private class SwitchData
		{
			public VariableReference OptimizationVariable
			{
				get;
				private set;
			}

			public Expression SwitchExpression
			{
				get;
				private set;
			}

			public SwitchData(VariableReference optimizationVariable, Expression switchExpression)
			{
				this.OptimizationVariable = optimizationVariable;
				this.SwitchExpression = switchExpression;
			}
		}
	}
}