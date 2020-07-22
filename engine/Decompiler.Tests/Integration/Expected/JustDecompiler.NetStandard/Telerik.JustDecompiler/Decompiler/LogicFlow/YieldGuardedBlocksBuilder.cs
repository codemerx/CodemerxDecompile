using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class YieldGuardedBlocksBuilder
	{
		private readonly LogicalFlowBuilderContext logicalContext;

		private readonly MethodSpecificContext methodContext;

		private BlockLogicalConstruct theBlock;

		private FieldReference stateFieldRef;

		private readonly List<CFGBlockLogicalConstruct> orderedCFGNodes;

		private readonly Dictionary<TryFinallyLogicalConstruct, YieldExceptionHandlerInfo> createdConstructsToIntervalMap;

		private HashSet<ILogicalConstruct> newTryBody;

		private CFGBlockLogicalConstruct entryOfTry;

		private CFGBlockLogicalConstruct newFinallyBody;

		private HashSet<ILogicalConstruct> finallyBlocks;

		private CFGBlockLogicalConstruct finallyEntryBlock;

		private CFGBlockLogicalConstruct conditionBlock;

		private CFGBlockLogicalConstruct disposeCallBlock;

		public YieldGuardedBlocksBuilder(LogicalFlowBuilderContext logicalContext, DecompilationContext decompilationContext)
		{
			this.orderedCFGNodes = new List<CFGBlockLogicalConstruct>();
			this.createdConstructsToIntervalMap = new Dictionary<TryFinallyLogicalConstruct, YieldExceptionHandlerInfo>();
			base();
			this.logicalContext = logicalContext;
			this.methodContext = decompilationContext.get_MethodContext();
			return;
		}

		public void BuildGuardedBlocks(BlockLogicalConstruct theBlock)
		{
			if (this.methodContext.get_YieldData() == null)
			{
				return;
			}
			this.theBlock = this.DetermineTheBlock(theBlock);
			V_0 = this.methodContext.get_YieldData().get_FieldsInfo();
			this.stateFieldRef = V_0.get_StateHolderField();
			stackVariable17 = this.methodContext.get_YieldData().get_ExceptionHandlers();
			Array.Sort<YieldExceptionHandlerInfo>(stackVariable17);
			this.GetOrderedCFGNodes();
			V_1 = stackVariable17;
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				this.GenerateTryFinallyHandler(V_1[V_2]);
				V_2 = V_2 + 1;
			}
			return;
		}

		private void BuildTryBody(YieldExceptionHandlerInfo handlerInfo)
		{
			this.newTryBody = new HashSet<ILogicalConstruct>();
			dummyVar0 = this.newTryBody.Add(this.entryOfTry);
			this.newFinallyBody = null;
			V_0 = new HashSet<ILogicalConstruct>();
			V_1 = new Queue<ILogicalConstruct>();
			V_2 = this.entryOfTry.get_SameParentSuccessors().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = (ILogicalConstruct)V_2.get_Current();
					V_1.Enqueue(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			while (V_1.get_Count() > 0)
			{
				V_4 = V_1.Dequeue();
				if (!V_0.Add(V_4) || this.finallyBlocks.Contains(V_4))
				{
					continue;
				}
				this.ProcessCurrentNode(handlerInfo, V_1, V_4);
			}
			return;
		}

		private void CleanUpOrderedNodes(TryFinallyLogicalConstruct theNewTryConstruct)
		{
			V_0 = theNewTryConstruct.get_CFGBlocks().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					dummyVar0 = this.orderedCFGNodes.Remove(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void DetachFromLogicalTree(CFGBlockLogicalConstruct node)
		{
			if (node.get_CFGPredecessors().get_Count() > 0 || node.get_CFGSuccessors().get_Count() > 0)
			{
				throw new Exception("This node cannot be detached from the logical tree.");
			}
			dummyVar0 = node.get_Parent().get_Children().Remove(node);
			V_0 = this.logicalContext.get_CFGBlockToLogicalConstructMap().get_Item(node.get_TheBlock());
			if ((int)V_0.Length == 1)
			{
				if (V_0[0] != node)
				{
					throw new Exception("Logical tree is inconsistent.");
				}
				dummyVar1 = this.logicalContext.get_CFGBlockToLogicalConstructMap().Remove(node.get_TheBlock());
			}
			V_1 = new CFGBlockLogicalConstruct[(int)V_0.Length - 1];
			V_2 = 0;
			V_3 = 0;
			while (V_2 < (int)V_0.Length)
			{
				if (V_0[V_2] != node)
				{
					if (V_3 == (int)V_0.Length)
					{
						throw new Exception("Logical tree is inconsistent.");
					}
					V_1[V_3] = V_0[V_2];
				}
				else
				{
					V_3 = V_3 - 1;
				}
				V_2 = V_2 + 1;
				V_3 = V_3 + 1;
			}
			this.logicalContext.get_CFGBlockToLogicalConstructMap().set_Item(node.get_TheBlock(), V_1);
			return;
		}

		private BlockLogicalConstruct DetermineTheBlock(BlockLogicalConstruct theBlock)
		{
			if (theBlock.get_Entry() as TryFaultLogicalConstruct == null || theBlock.get_Children().get_Count() > 2)
			{
				return theBlock;
			}
			return (theBlock.get_Entry() as TryFaultLogicalConstruct).get_Try();
		}

		private BlockLogicalConstruct GenerateFinallyBlock()
		{
			V_0 = this.ProcessFinallyNode(this.finallyEntryBlock, this.disposeCallBlock);
			dummyVar0 = V_0.RemoveFromPredecessors(this.conditionBlock);
			dummyVar1 = this.conditionBlock.RemoveFromSuccessors(V_0);
			stackVariable15 = this.logicalContext;
			V_2 = stackVariable15.get_MaxBlockIndex() + 1;
			stackVariable15.set_MaxBlockIndex(V_2);
			V_1 = new EmptyBlockLogicalConstruct(V_2);
			V_1.AddToPredecessors(this.disposeCallBlock);
			V_1.AddToPredecessors(this.conditionBlock);
			this.disposeCallBlock.AddToSuccessors(V_1);
			this.conditionBlock.AddToSuccessors(V_1);
			V_1.set_Parent(this.finallyEntryBlock.get_Parent());
			dummyVar2 = V_1.get_Parent().get_Children().Add(V_1);
			V_3 = 0;
			while (V_3 < (int)this.conditionBlock.get_TheBlock().get_Successors().Length)
			{
				if (InstructionBlock.op_Equality(this.conditionBlock.get_TheBlock().get_Successors()[V_3], V_0.get_TheBlock()))
				{
					this.conditionBlock.get_TheBlock().get_Successors()[V_3] = null;
				}
				V_3 = V_3 + 1;
			}
			dummyVar3 = this.finallyBlocks.Add(V_1);
			return new BlockLogicalConstruct(this.finallyEntryBlock, this.finallyBlocks);
		}

		private void GenerateTryFinallyHandler(YieldExceptionHandlerInfo handlerInfo)
		{
			this.finallyBlocks = new HashSet<ILogicalConstruct>();
			this.entryOfTry = this.GetStateBeginBlockConstruct(handlerInfo.get_TryStates());
			this.BuildTryBody(handlerInfo);
			if (handlerInfo.get_HandlerType() != YieldExceptionHandlerType.Method)
			{
				V_0 = this.GenerateFinallyBlock();
			}
			else
			{
				if (this.newFinallyBody == null)
				{
					throw new Exception("Could not determine the end ot the try block");
				}
				this.RemoveExcessNodesFromTheTryBlock();
				this.ProcessFinallyNodes();
				stackVariable31 = this.newFinallyBody;
				stackVariable33 = new ILogicalConstruct[1];
				stackVariable33[0] = this.newFinallyBody;
				V_0 = new BlockLogicalConstruct(stackVariable31, stackVariable33);
			}
			V_1 = new TryFinallyLogicalConstruct(new BlockLogicalConstruct(this.entryOfTry, this.newTryBody), V_0);
			this.createdConstructsToIntervalMap.set_Item(V_1, handlerInfo);
			this.CleanUpOrderedNodes(V_1);
			return;
		}

		private void GetOrderedCFGNodes()
		{
			V_0 = DFSTBuilder.BuildTree(this.theBlock).get_ReversePostOrder().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current().get_Construct() as CFGBlockLogicalConstruct;
					if (V_1 == null)
					{
						continue;
					}
					this.orderedCFGNodes.Add(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private CFGBlockLogicalConstruct GetStateBeginBlockConstruct(HashSet<int> tryStates)
		{
			V_0 = 0;
			while (V_0 < this.orderedCFGNodes.get_Count())
			{
				V_1 = this.orderedCFGNodes.get_Item(V_0);
				V_2 = V_1.get_LogicalConstructExpressions();
				V_3 = 0;
				while (V_3 < V_2.get_Count())
				{
					if (this.TryGetStateAssignValue(V_2.get_Item(V_3), out V_4) && tryStates.Contains(V_4))
					{
						if (V_3 == 0)
						{
							return V_1;
						}
						V_5 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, V_1, V_3);
						this.orderedCFGNodes.set_Item(V_0, V_5.get_Key());
						this.orderedCFGNodes.Insert(V_0 + 1, V_5.get_Value());
						return V_5.get_Value();
					}
					V_3 = V_3 + 1;
				}
				V_0 = V_0 + 1;
			}
			throw new Exception("Invalid state value");
		}

		private void ProcessCurrentNode(YieldExceptionHandlerInfo handlerInfo, Queue<ILogicalConstruct> bfsQueue, ILogicalConstruct currentNode)
		{
			if (currentNode as CFGBlockLogicalConstruct == null)
			{
				if (currentNode as TryFinallyLogicalConstruct != null && this.createdConstructsToIntervalMap.TryGetValue(currentNode as TryFinallyLogicalConstruct, out V_10) && V_10.get_TryStates().IsProperSupersetOf(handlerInfo.get_TryStates()))
				{
					throw new Exception("This try/finally construct cannot be nested in the current construct");
				}
			}
			else
			{
				V_0 = currentNode as CFGBlockLogicalConstruct;
				V_1 = 0;
				while (V_1 < V_0.get_LogicalConstructExpressions().get_Count())
				{
					V_2 = V_0.get_LogicalConstructExpressions().get_Item(V_1);
					if (!this.TryGetStateAssignValue(V_2, out V_3))
					{
						if (handlerInfo.get_HandlerType() == YieldExceptionHandlerType.Method && V_2.get_CodeNodeType() == 19 && (object)(V_2 as MethodInvocationExpression).get_MethodExpression().get_MethodDefinition() == (object)handlerInfo.get_FinallyMethodDefinition())
						{
							if (V_0.get_LogicalConstructExpressions().get_Count() == 1)
							{
								if (this.newFinallyBody == null)
								{
									this.newFinallyBody = V_0;
								}
								dummyVar0 = this.finallyBlocks.Add(this.newFinallyBody);
								dummyVar1 = this.orderedCFGNodes.Remove(V_0);
								return;
							}
							if (V_1 != 0)
							{
								if (V_1 >= V_0.get_LogicalConstructExpressions().get_Count() - 1)
								{
									V_8 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, V_0, V_1);
									dummyVar3 = this.newTryBody.Add(V_8.get_Key());
									V_4 = V_8.get_Value();
									dummyVar4 = this.orderedCFGNodes.Remove(V_0);
								}
								else
								{
									V_6 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, V_0, V_1);
									dummyVar2 = this.newTryBody.Add(V_6.get_Key());
									V_7 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, V_6.get_Value(), 1);
									V_4 = V_7.get_Key();
									this.orderedCFGNodes.set_Item(this.orderedCFGNodes.IndexOf(V_0), V_7.get_Value());
								}
							}
							else
							{
								V_5 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, V_0, V_1 + 1);
								V_4 = V_5.get_Key();
								this.orderedCFGNodes.set_Item(this.orderedCFGNodes.IndexOf(V_0), V_5.get_Value());
							}
							if (this.newFinallyBody == null)
							{
								this.newFinallyBody = V_4;
							}
							dummyVar5 = this.finallyBlocks.Add(V_4);
							return;
						}
					}
					else
					{
						if (!handlerInfo.get_TryStates().Contains(V_3))
						{
							if (handlerInfo.get_HandlerType() == YieldExceptionHandlerType.Method || !this.TryProcessConditionalDisposeHandler(handlerInfo, V_0))
							{
								throw new Exception("Invalid state value");
							}
							return;
						}
					}
					V_1 = V_1 + 1;
				}
			}
			dummyVar6 = this.newTryBody.Add(currentNode);
			V_11 = currentNode.get_SameParentSuccessors().GetEnumerator();
			try
			{
				while (V_11.MoveNext())
				{
					V_12 = (ILogicalConstruct)V_11.get_Current();
					bfsQueue.Enqueue(V_12);
				}
			}
			finally
			{
				((IDisposable)V_11).Dispose();
			}
			return;
		}

		private void ProcessFinallyNode(CFGBlockLogicalConstruct finallyCFGBlock)
		{
			dummyVar0 = this.ProcessFinallyNode(finallyCFGBlock, finallyCFGBlock);
			return;
		}

		private CFGBlockLogicalConstruct ProcessFinallyNode(CFGBlockLogicalConstruct finallyBlockEntry, CFGBlockLogicalConstruct finallyBlockEnd)
		{
			V_1 = finallyBlockEntry.get_SameParentPredecessors().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = (ILogicalConstruct)V_1.get_Current();
					if (this.newTryBody.Contains(V_2))
					{
						continue;
					}
					throw new Exception("Invalid entry to the finally block");
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			V_3 = finallyBlockEnd.get_CFGSuccessors().GetEnumerator();
			try
			{
				dummyVar0 = V_3.MoveNext();
				V_0 = V_3.get_Current();
				if (V_3.MoveNext())
				{
					throw new Exception("Invalid count of successors");
				}
			}
			finally
			{
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			V_4 = (new HashSet<CFGBlockLogicalConstruct>(finallyBlockEntry.get_CFGPredecessors())).GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (InstructionBlock.op_Inequality(V_5.get_TheBlock(), finallyBlockEntry.get_TheBlock()) && (int)V_5.get_TheBlock().get_Successors().Length > 1)
					{
						this.ProcessMultiWayCFGPredecessor(finallyBlockEntry, V_5.get_TheBlock(), V_0.get_TheBlock());
					}
					V_6 = V_5;
					while (V_6 != finallyBlockEntry.get_Parent())
					{
						dummyVar1 = V_6.RemoveFromSuccessors(finallyBlockEntry);
						V_6.AddToSuccessors(V_0);
						V_6 = V_6.get_Parent() as LogicalConstructBase;
					}
					V_0.AddToPredecessors(V_5);
					dummyVar2 = finallyBlockEntry.RemoveFromPredecessors(V_5);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			dummyVar3 = V_0.RemoveFromPredecessors(finallyBlockEnd);
			dummyVar4 = finallyBlockEnd.RemoveFromSuccessors(V_0);
			return V_0;
		}

		private void ProcessFinallyNodes()
		{
			V_0 = this.finallyBlocks.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (CFGBlockLogicalConstruct)V_0.get_Current();
					this.ProcessFinallyNode(V_1);
					if (V_1 == this.newFinallyBody)
					{
						continue;
					}
					this.DetachFromLogicalTree(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void ProcessMultiWayCFGPredecessor(CFGBlockLogicalConstruct finallyBody, InstructionBlock theBlock, InstructionBlock theNewSuccessor)
		{
			V_0 = finallyBody.get_TheBlock();
			V_2 = 0;
			while (V_2 < (int)theBlock.get_Successors().Length)
			{
				if (InstructionBlock.op_Equality(theBlock.get_Successors()[V_2], V_0))
				{
					theBlock.get_Successors()[V_2] = theNewSuccessor;
				}
				V_2 = V_2 + 1;
			}
			if (this.methodContext.get_ControlFlowGraph().get_SwitchBlocksInformation().TryGetValue(theBlock, out V_1))
			{
				V_3 = V_1.get_OrderedCasesArray();
				V_4 = 0;
				while (V_4 < (int)V_3.Length)
				{
					if (InstructionBlock.op_Equality(V_3[V_4], V_0))
					{
						V_3[V_4] = theNewSuccessor;
					}
					V_4 = V_4 + 1;
				}
				if (InstructionBlock.op_Equality(V_1.get_DefaultCase(), V_0))
				{
					V_1.set_DefaultCase(theNewSuccessor);
				}
			}
			return;
		}

		private void RemoveExcessNodesFromTheTryBlock()
		{
			V_0 = new HashSet<ILogicalConstruct>(this.finallyBlocks);
			V_1 = new Queue<ILogicalConstruct>(this.finallyBlocks);
			while (V_1.get_Count() > 0)
			{
				V_2 = V_1.Dequeue().get_SameParentSuccessors().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = (ILogicalConstruct)V_2.get_Current();
						if (V_0.Contains(V_3) || V_3 == this.entryOfTry)
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_3);
						V_1.Enqueue(V_3);
					}
				}
				finally
				{
					((IDisposable)V_2).Dispose();
				}
			}
			V_4 = V_0.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (V_5 == this.entryOfTry)
					{
						continue;
					}
					dummyVar1 = this.newTryBody.Remove(V_5);
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return;
		}

		private bool TryGetStateAssignValue(Expression expression, out int value)
		{
			if (expression.get_CodeNodeType() != 24 || !(expression as BinaryExpression).get_IsAssignmentExpression())
			{
				value = -1;
				return false;
			}
			V_0 = expression as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() != 30 || (V_0.get_Left() as FieldReferenceExpression).get_Field().Resolve() != this.stateFieldRef)
			{
				value = -1;
				return false;
			}
			if (V_0.get_Right().get_CodeNodeType() != 22)
			{
				throw new Exception("Incorrect value for state field");
			}
			value = Convert.ToInt32((V_0.get_Right() as LiteralExpression).get_Value());
			return true;
		}

		private bool TryProcessConditionalDisposeHandler(YieldExceptionHandlerInfo yieldExceptionHandler, CFGBlockLogicalConstruct startBlock)
		{
			if (this.finallyBlocks.get_Count() > 0)
			{
				return false;
			}
			if (startBlock as PartialCFGBlockLogicalConstruct == null || startBlock.get_CFGSuccessors().get_Count() != 1)
			{
				return false;
			}
			if (startBlock.get_LogicalConstructExpressions().get_Count() == 0)
			{
				return false;
			}
			V_0 = startBlock.get_LogicalConstructExpressions().get_Item(0) as BinaryExpression;
			if (V_0 == null || !V_0.get_IsAssignmentExpression() || V_0.get_Left().get_CodeNodeType() != 30 || (V_0.get_Left() as FieldReferenceExpression).get_Field().Resolve() != this.stateFieldRef || V_0.get_Right().get_CodeNodeType() != 22 || (Int32)(V_0.get_Right() as LiteralExpression).get_Value() != yieldExceptionHandler.get_NextState())
			{
				return false;
			}
			if (startBlock.get_LogicalConstructExpressions().get_Count() != 2 || yieldExceptionHandler.get_HandlerType() != 2)
			{
				if (startBlock.get_LogicalConstructExpressions().get_Count() != 1 || yieldExceptionHandler.get_HandlerType() != 1)
				{
					return false;
				}
			}
			else
			{
				V_6 = startBlock.get_LogicalConstructExpressions().get_Item(1) as BinaryExpression;
				if (V_6 == null || !V_6.get_IsAssignmentExpression() || V_6.get_Left().get_CodeNodeType() != 30 || (object)(V_6.get_Left() as FieldReferenceExpression).get_Field() != (object)yieldExceptionHandler.get_DisposableField() || V_6.get_Right().get_CodeNodeType() != 33 || (V_6.get_Right() as SafeCastExpression).get_Expression().get_CodeNodeType() != 30 || (object)((V_6.get_Right() as SafeCastExpression).get_Expression() as FieldReferenceExpression).get_Field() != (object)yieldExceptionHandler.get_EnumeratorField())
				{
					return false;
				}
			}
			V_2 = startBlock.get_CFGSuccessors().GetEnumerator();
			V_7 = V_2;
			try
			{
				dummyVar0 = V_2.MoveNext();
				V_1 = V_2.get_Current();
			}
			finally
			{
				if (V_7 != null)
				{
					V_7.Dispose();
				}
			}
			if (V_1.get_LogicalConstructExpressions().get_Count() != 1)
			{
				return false;
			}
			V_3 = V_1.get_LogicalConstructExpressions().get_Item(0) as BinaryExpression;
			if (V_3 == null || V_3.get_Operator() != 9 || V_3.get_Left().get_CodeNodeType() != 30 || (object)(V_3.get_Left() as FieldReferenceExpression).get_Field() != (object)yieldExceptionHandler.get_DisposableField() || V_3.get_Right().get_CodeNodeType() != 22 || (V_3.get_Right() as LiteralExpression).get_Value() != null)
			{
				return false;
			}
			V_4 = null;
			V_8 = V_1.get_CFGSuccessors().GetEnumerator();
			try
			{
				while (V_8.MoveNext())
				{
					V_9 = V_8.get_Current() as CFGBlockLogicalConstruct;
					if (V_9 == null || V_9.get_CFGPredecessors().get_Count() != 1)
					{
						continue;
					}
					V_4 = V_9;
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_8).Dispose();
			}
		Label0:
			if (V_4 == null || V_4.get_LogicalConstructExpressions().get_Count() != 1)
			{
				return false;
			}
			V_5 = V_4.get_LogicalConstructExpressions().get_Item(0) as MethodInvocationExpression;
			if (V_5 == null || !V_5.get_VirtualCall() || V_5.get_MethodExpression().get_Target().get_CodeNodeType() != 30 || (object)(V_5.get_MethodExpression().get_Target() as FieldReferenceExpression).get_Field() != (object)yieldExceptionHandler.get_DisposableField() || String.op_Inequality(V_5.get_MethodExpression().get_Method().get_Name(), "Dispose"))
			{
				return false;
			}
			this.finallyEntryBlock = startBlock;
			dummyVar1 = this.finallyBlocks.Add(startBlock);
			this.conditionBlock = V_1;
			dummyVar2 = this.finallyBlocks.Add(V_1);
			this.disposeCallBlock = V_4;
			dummyVar3 = this.finallyBlocks.Add(V_4);
			return true;
		}
	}
}