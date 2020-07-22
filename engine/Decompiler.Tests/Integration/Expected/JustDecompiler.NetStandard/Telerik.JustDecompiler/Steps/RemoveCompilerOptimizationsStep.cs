using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			base();
			return;
		}

		private bool IsCaseBlock(IList<Expression> expressions, Expression switchExpression)
		{
			if (expressions.get_Count() != 1 || expressions.get_Item(0).get_CodeNodeType() != 23)
			{
				return false;
			}
			V_0 = expressions.get_Item(0) as UnaryExpression;
			if (V_0.get_Operator() != 11 || V_0.get_Operand().get_CodeNodeType() != 19)
			{
				return false;
			}
			V_1 = V_0.get_Operand() as MethodInvocationExpression;
			if (String.op_Inequality(V_1.get_MethodExpression().get_Method().get_Name(), "op_Equality") || V_1.get_Arguments().get_Item(1).get_CodeNodeType() != 22 || String.op_Inequality((V_1.get_Arguments().get_Item(1) as LiteralExpression).get_ExpressionType().get_FullName(), "System.String"))
			{
				return false;
			}
			if (!V_1.get_Arguments().get_Item(0).Equals(switchExpression))
			{
				return false;
			}
			return true;
		}

		private bool IsEmptyStringCaseBlock(IList<Expression> expressions, Expression switchExpression)
		{
			if (expressions.get_Count() != 1 || expressions.get_Item(0).get_CodeNodeType() != 24)
			{
				return false;
			}
			V_0 = expressions.get_Item(0) as BinaryExpression;
			if (V_0.get_Operator() != 9 || V_0.get_Left().get_CodeNodeType() != 19 || V_0.get_Right().get_CodeNodeType() != 22)
			{
				return false;
			}
			V_1 = V_0.get_Left() as MethodInvocationExpression;
			if (String.op_Inequality(V_1.get_MethodExpression().get_Method().get_FullName(), "System.Int32 System.String::get_Length()") || V_1.get_MethodExpression().get_Target() == null)
			{
				return false;
			}
			V_2 = V_0.get_Right() as LiteralExpression;
			if (String.op_Inequality(V_2.get_ExpressionType().get_FullName(), "System.Int32") && (Int32)V_2.get_Value() != 0)
			{
				return false;
			}
			if (!V_1.get_MethodExpression().get_Target().Equals(switchExpression))
			{
				return false;
			}
			return true;
		}

		private bool IsNullCaseBlock(IList<Expression> expressions, Expression switchExpression)
		{
			if (expressions.get_Count() != 1)
			{
				return false;
			}
			if (expressions.get_Item(0).get_CodeNodeType() != 24)
			{
				return false;
			}
			V_0 = expressions.get_Item(0) as BinaryExpression;
			if (V_0.get_Operator() != 9 || V_0.get_Right().get_CodeNodeType() != 22)
			{
				return false;
			}
			V_1 = V_0.get_Right() as LiteralExpression;
			if (String.op_Inequality(V_1.get_ExpressionType().get_FullName(), "System.Object") || V_1.get_Value() != null)
			{
				return false;
			}
			if (!V_0.get_Left().Equals(switchExpression))
			{
				return false;
			}
			return true;
		}

		private bool IsOptimizationBlock(IList<Expression> expressions, VariableReference optimizationVariable)
		{
			if (expressions.get_Count() != 1 || expressions.get_Item(0).get_CodeNodeType() != 24)
			{
				return false;
			}
			V_0 = expressions.get_Item(0) as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() == 26 && V_0.get_Right().get_CodeNodeType() == 22 && V_0.get_IsComparisonExpression() && (object)(V_0.get_Left() as VariableReferenceExpression).get_Variable() == (object)optimizationVariable)
			{
				return true;
			}
			return false;
		}

		private bool IsUnconditionJumpBlock(InstructionBlock block)
		{
			if ((object)block.get_First() != (object)block.get_Last() || block.get_First().get_OpCode().get_Code() != 55 && block.get_First().get_OpCode().get_Code() != 42)
			{
				return false;
			}
			return (int)block.get_Successors().Length == 1;
		}

		private void MarkOptimizationAndCaseBlocks(InstructionBlock block, RemoveCompilerOptimizationsStep.SwitchData data)
		{
			V_0 = new Queue<InstructionBlock>();
			V_1 = new HashSet<int>();
			V_2 = block.get_Successors();
			V_3 = 0;
			while (V_3 < (int)V_2.Length)
			{
				V_0.Enqueue(V_2[V_3]);
				V_3 = V_3 + 1;
			}
			while (V_0.get_Count() > 0)
			{
				V_5 = V_0.Dequeue();
				dummyVar0 = V_1.Add(V_5.get_First().get_Offset());
				if (!this.IsOptimizationBlock(this.blockExpressions.get_Item(V_5.get_First().get_Offset()), data.get_OptimizationVariable()))
				{
					if (!this.IsCaseBlock(this.blockExpressions.get_Item(V_5.get_First().get_Offset()), data.get_SwitchExpression()) && !this.IsNullCaseBlock(this.blockExpressions.get_Item(V_5.get_First().get_Offset()), data.get_SwitchExpression()))
					{
						continue;
					}
					this.switchBlocksToCasesMap.get_Item(block.get_First().get_Offset()).Add(V_5.get_First().get_Offset());
					V_8 = V_5.get_Successors()[1];
					if (this.IsEmptyStringCaseBlock(this.blockExpressions.get_Item(V_8.get_First().get_Offset()), data.get_SwitchExpression()))
					{
						V_5.set_Last(V_8.get_Last());
						V_5.set_Successors(V_8.get_Successors());
						V_9 = this.blockExpressions.get_Item(V_5.get_First().get_Offset()).get_Item(0) as BinaryExpression;
						V_9.set_Right(new LiteralExpression("", this.methodContext.get_Method().get_Module().get_TypeSystem(), null));
						V_10 = this.blockExpressions.get_Item(V_8.get_First().get_Offset()).get_Item(0).get_UnderlyingSameMethodInstructions();
						V_9 = V_9.CloneAndAttachInstructions(V_10) as BinaryExpression;
						this.blockExpressions.get_Item(V_5.get_First().get_Offset()).set_Item(0, new UnaryExpression(11, V_9, null));
						this.blocksToBeRemoved.Add(V_8.get_First().get_Offset());
					}
					dummyVar1 = this.MarkSecondSuccessorForRemovalIfItIsUnconditionalJump(V_5);
				}
				else
				{
					V_6 = V_5.get_Successors()[0];
					if (!V_1.Contains(V_6.get_First().get_Offset()))
					{
						V_0.Enqueue(V_6);
					}
					V_7 = this.MarkSecondSuccessorForRemovalIfItIsUnconditionalJump(V_5);
					if (!V_1.Contains(V_7.get_First().get_Offset()) && this.IsOptimizationBlock(this.blockExpressions.get_Item(V_5.get_First().get_Offset()), data.get_OptimizationVariable()))
					{
						V_0.Enqueue(V_7);
					}
					this.blocksToBeRemoved.Add(V_5.get_First().get_Offset());
				}
			}
			return;
		}

		private InstructionBlock MarkSecondSuccessorForRemovalIfItIsUnconditionalJump(InstructionBlock block)
		{
			V_0 = block.get_Successors()[1];
			if (this.IsUnconditionJumpBlock(V_0))
			{
				block.RemoveFromSuccessors(V_0);
				block.AddToSuccessors(V_0.get_Successors()[0]);
				this.blocksToBeRemoved.Add(V_0.get_First().get_Offset());
				V_0 = V_0.get_Successors()[0];
			}
			return V_0;
		}

		private void MergeFirstCFGBlockWithFirstCaseIf(int index)
		{
			V_0 = this.instructionToBlockMapping.get_Item(this.switchBlocksToCasesMap.get_Item(index).get_Item(0));
			stackVariable12 = this.instructionToBlockMapping.get_Item(index);
			stackVariable12.set_Last(V_0.get_Last());
			stackVariable12.set_Successors(V_0.get_Successors());
			V_1 = this.blockExpressions.get_Item(V_0.get_First().get_Offset()).GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					this.blockExpressions.get_Item(index).Add(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.blocksToBeRemoved.Add(V_0.get_First().get_Offset());
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.blockExpressions = this.methodContext.get_Expressions().get_BlockExpressions();
			this.instructionToBlockMapping = this.methodContext.get_ControlFlowGraph().get_InstructionToBlockMapping();
			this.blocksToBeRemoved = new List<int>();
			this.switchBlocksToCasesMap = this.methodContext.get_SwitchByStringData().get_SwitchBlocksToCasesMap();
			this.RemoveSwitchByStringOptimizations();
			return body;
		}

		private void ReconnectBlocks(int index)
		{
			V_0 = this.instructionToBlockMapping.get_Item(index);
			V_1 = 1;
			while (V_1 < this.switchBlocksToCasesMap.get_Item(index).get_Count())
			{
				V_2 = this.instructionToBlockMapping.get_Item(this.switchBlocksToCasesMap.get_Item(index).get_Item(V_1));
				V_0.RemoveFromSuccessors(V_0.get_Successors().Last<InstructionBlock>());
				V_0.AddToSuccessors(V_2);
				V_0 = V_2;
				V_1 = V_1 + 1;
			}
			return;
		}

		private void RemoveExpressionsFromFirstBlock(int index)
		{
			this.blockExpressions.get_Item(index).RemoveAt(this.blockExpressions.get_Item(index).get_Count() - 1);
			this.blockExpressions.get_Item(index).RemoveAt(this.blockExpressions.get_Item(index).get_Count() - 1);
			return;
		}

		private bool RemoveOptimizationBlocks()
		{
			this.blocksToBeRemoved.Sort();
			V_0 = this.blocksToBeRemoved.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.methodContext.get_ControlFlowGraph().RemoveBlockAt(this.instructionToBlockMapping.get_Item(V_1).get_Index());
					dummyVar0 = this.blockExpressions.Remove(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return this.blocksToBeRemoved.get_Count() > 0;
		}

		private void RemoveSwitchByStringOptimizations()
		{
			V_0 = this.methodContext.get_SwitchByStringData().get_SwitchBlocksStartInstructions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.TryGetSwitchData(this.blockExpressions.get_Item(V_1), out V_2))
					{
						continue;
					}
					this.switchBlocksToCasesMap.Add(V_1, new List<int>());
					this.MarkOptimizationAndCaseBlocks(this.instructionToBlockMapping.get_Item(V_1), V_2);
					this.switchBlocksToCasesMap.get_Item(V_1).Sort();
					this.RemoveExpressionsFromFirstBlock(V_1);
					this.MergeFirstCFGBlockWithFirstCaseIf(V_1);
					this.ReconnectBlocks(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			if (this.RemoveOptimizationBlocks())
			{
				this.methodContext.set_IsMethodBodyChanged(true);
			}
			return;
		}

		private bool TryGetSwitchData(IList<Expression> expressions, out RemoveCompilerOptimizationsStep.SwitchData data)
		{
			data = null;
			if (expressions.get_Count() < 2 || expressions.get_Item(expressions.get_Count() - 2).get_CodeNodeType() != 24)
			{
				return false;
			}
			V_0 = expressions.get_Item(expressions.get_Count() - 2) as BinaryExpression;
			if (!V_0.get_IsAssignmentExpression())
			{
				return false;
			}
			if (V_0.get_Left().get_CodeNodeType() != 26 || V_0.get_Right().get_CodeNodeType() != 19)
			{
				return false;
			}
			V_1 = V_0.get_Right() as MethodInvocationExpression;
			if (!Utilities.IsComputeStringHashMethod(V_1.get_MethodExpression().get_Method()))
			{
				return false;
			}
			data = new RemoveCompilerOptimizationsStep.SwitchData((V_0.get_Left() as VariableReferenceExpression).get_Variable(), V_1.get_Arguments().get_Item(0));
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
				base();
				this.set_OptimizationVariable(optimizationVariable);
				this.set_SwitchExpression(switchExpression);
				return;
			}
		}
	}
}