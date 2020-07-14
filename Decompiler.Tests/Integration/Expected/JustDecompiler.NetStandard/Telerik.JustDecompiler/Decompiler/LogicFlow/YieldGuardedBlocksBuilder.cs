using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Common;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DFST;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class YieldGuardedBlocksBuilder
	{
		private readonly LogicalFlowBuilderContext logicalContext;

		private readonly MethodSpecificContext methodContext;

		private BlockLogicalConstruct theBlock;

		private FieldReference stateFieldRef;

		private readonly List<CFGBlockLogicalConstruct> orderedCFGNodes = new List<CFGBlockLogicalConstruct>();

		private readonly Dictionary<TryFinallyLogicalConstruct, YieldExceptionHandlerInfo> createdConstructsToIntervalMap = new Dictionary<TryFinallyLogicalConstruct, YieldExceptionHandlerInfo>();

		private HashSet<ILogicalConstruct> newTryBody;

		private CFGBlockLogicalConstruct entryOfTry;

		private CFGBlockLogicalConstruct newFinallyBody;

		private HashSet<ILogicalConstruct> finallyBlocks;

		private CFGBlockLogicalConstruct finallyEntryBlock;

		private CFGBlockLogicalConstruct conditionBlock;

		private CFGBlockLogicalConstruct disposeCallBlock;

		public YieldGuardedBlocksBuilder(LogicalFlowBuilderContext logicalContext, DecompilationContext decompilationContext)
		{
			this.logicalContext = logicalContext;
			this.methodContext = decompilationContext.MethodContext;
		}

		public void BuildGuardedBlocks(BlockLogicalConstruct theBlock)
		{
			if (this.methodContext.YieldData == null)
			{
				return;
			}
			this.theBlock = this.DetermineTheBlock(theBlock);
			this.stateFieldRef = this.methodContext.YieldData.FieldsInfo.StateHolderField;
			YieldExceptionHandlerInfo[] exceptionHandlers = this.methodContext.YieldData.ExceptionHandlers;
			Array.Sort<YieldExceptionHandlerInfo>(exceptionHandlers);
			this.GetOrderedCFGNodes();
			YieldExceptionHandlerInfo[] yieldExceptionHandlerInfoArray = exceptionHandlers;
			for (int i = 0; i < (int)yieldExceptionHandlerInfoArray.Length; i++)
			{
				this.GenerateTryFinallyHandler(yieldExceptionHandlerInfoArray[i]);
			}
		}

		private void BuildTryBody(YieldExceptionHandlerInfo handlerInfo)
		{
			this.newTryBody = new HashSet<ILogicalConstruct>();
			this.newTryBody.Add(this.entryOfTry);
			this.newFinallyBody = null;
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			Queue<ILogicalConstruct> logicalConstructs1 = new Queue<ILogicalConstruct>();
			foreach (ILogicalConstruct sameParentSuccessor in this.entryOfTry.SameParentSuccessors)
			{
				logicalConstructs1.Enqueue(sameParentSuccessor);
			}
			while (logicalConstructs1.Count > 0)
			{
				ILogicalConstruct logicalConstruct = logicalConstructs1.Dequeue();
				if (!logicalConstructs.Add(logicalConstruct) || this.finallyBlocks.Contains(logicalConstruct))
				{
					continue;
				}
				this.ProcessCurrentNode(handlerInfo, logicalConstructs1, logicalConstruct);
			}
		}

		private void CleanUpOrderedNodes(TryFinallyLogicalConstruct theNewTryConstruct)
		{
			foreach (CFGBlockLogicalConstruct cFGBlock in theNewTryConstruct.CFGBlocks)
			{
				this.orderedCFGNodes.Remove(cFGBlock);
			}
		}

		private void DetachFromLogicalTree(CFGBlockLogicalConstruct node)
		{
			if (node.CFGPredecessors.Count > 0 || node.CFGSuccessors.Count > 0)
			{
				throw new Exception("This node cannot be detached from the logical tree.");
			}
			node.Parent.Children.Remove(node);
			CFGBlockLogicalConstruct[] item = this.logicalContext.CFGBlockToLogicalConstructMap[node.TheBlock];
			if ((int)item.Length == 1)
			{
				if (item[0] != node)
				{
					throw new Exception("Logical tree is inconsistent.");
				}
				this.logicalContext.CFGBlockToLogicalConstructMap.Remove(node.TheBlock);
			}
			CFGBlockLogicalConstruct[] cFGBlockLogicalConstructArray = new CFGBlockLogicalConstruct[(int)item.Length - 1];
			int num = 0;
			int num1 = 0;
			while (num < (int)item.Length)
			{
				if (item[num] != node)
				{
					if (num1 == (int)item.Length)
					{
						throw new Exception("Logical tree is inconsistent.");
					}
					cFGBlockLogicalConstructArray[num1] = item[num];
				}
				else
				{
					num1--;
				}
				num++;
				num1++;
			}
			this.logicalContext.CFGBlockToLogicalConstructMap[node.TheBlock] = cFGBlockLogicalConstructArray;
		}

		private BlockLogicalConstruct DetermineTheBlock(BlockLogicalConstruct theBlock)
		{
			if (!(theBlock.Entry is TryFaultLogicalConstruct) || theBlock.Children.Count > 2)
			{
				return theBlock;
			}
			return (theBlock.Entry as TryFaultLogicalConstruct).Try;
		}

		private BlockLogicalConstruct GenerateFinallyBlock()
		{
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct = this.ProcessFinallyNode(this.finallyEntryBlock, this.disposeCallBlock);
			cFGBlockLogicalConstruct.RemoveFromPredecessors(this.conditionBlock);
			this.conditionBlock.RemoveFromSuccessors(cFGBlockLogicalConstruct);
			LogicalFlowBuilderContext logicalFlowBuilderContext = this.logicalContext;
			int maxBlockIndex = logicalFlowBuilderContext.MaxBlockIndex + 1;
			logicalFlowBuilderContext.MaxBlockIndex = maxBlockIndex;
			EmptyBlockLogicalConstruct emptyBlockLogicalConstruct = new EmptyBlockLogicalConstruct(maxBlockIndex);
			emptyBlockLogicalConstruct.AddToPredecessors(this.disposeCallBlock);
			emptyBlockLogicalConstruct.AddToPredecessors(this.conditionBlock);
			this.disposeCallBlock.AddToSuccessors(emptyBlockLogicalConstruct);
			this.conditionBlock.AddToSuccessors(emptyBlockLogicalConstruct);
			emptyBlockLogicalConstruct.Parent = this.finallyEntryBlock.Parent;
			emptyBlockLogicalConstruct.Parent.Children.Add(emptyBlockLogicalConstruct);
			for (int i = 0; i < (int)this.conditionBlock.TheBlock.Successors.Length; i++)
			{
				if (this.conditionBlock.TheBlock.Successors[i] == cFGBlockLogicalConstruct.TheBlock)
				{
					this.conditionBlock.TheBlock.Successors[i] = null;
				}
			}
			this.finallyBlocks.Add(emptyBlockLogicalConstruct);
			return new BlockLogicalConstruct(this.finallyEntryBlock, this.finallyBlocks);
		}

		private void GenerateTryFinallyHandler(YieldExceptionHandlerInfo handlerInfo)
		{
			BlockLogicalConstruct blockLogicalConstruct;
			this.finallyBlocks = new HashSet<ILogicalConstruct>();
			this.entryOfTry = this.GetStateBeginBlockConstruct(handlerInfo.TryStates);
			this.BuildTryBody(handlerInfo);
			if (handlerInfo.HandlerType != YieldExceptionHandlerType.Method)
			{
				blockLogicalConstruct = this.GenerateFinallyBlock();
			}
			else
			{
				if (this.newFinallyBody == null)
				{
					throw new Exception("Could not determine the end ot the try block");
				}
				this.RemoveExcessNodesFromTheTryBlock();
				this.ProcessFinallyNodes();
				blockLogicalConstruct = new BlockLogicalConstruct(this.newFinallyBody, new ILogicalConstruct[] { this.newFinallyBody });
			}
			TryFinallyLogicalConstruct tryFinallyLogicalConstruct = new TryFinallyLogicalConstruct(new BlockLogicalConstruct(this.entryOfTry, this.newTryBody), blockLogicalConstruct);
			this.createdConstructsToIntervalMap[tryFinallyLogicalConstruct] = handlerInfo;
			this.CleanUpOrderedNodes(tryFinallyLogicalConstruct);
		}

		private void GetOrderedCFGNodes()
		{
			foreach (DFSTNode reversePostOrder in DFSTBuilder.BuildTree(this.theBlock).ReversePostOrder)
			{
				CFGBlockLogicalConstruct construct = reversePostOrder.Construct as CFGBlockLogicalConstruct;
				if (construct == null)
				{
					continue;
				}
				this.orderedCFGNodes.Add(construct);
			}
		}

		private CFGBlockLogicalConstruct GetStateBeginBlockConstruct(HashSet<int> tryStates)
		{
			int num;
			for (int i = 0; i < this.orderedCFGNodes.Count; i++)
			{
				CFGBlockLogicalConstruct item = this.orderedCFGNodes[i];
				List<Expression> logicalConstructExpressions = item.LogicalConstructExpressions;
				for (int j = 0; j < logicalConstructExpressions.Count; j++)
				{
					if (this.TryGetStateAssignValue(logicalConstructExpressions[j], out num) && tryStates.Contains(num))
					{
						if (j == 0)
						{
							return item;
						}
						KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> keyValuePair = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, item, j);
						this.orderedCFGNodes[i] = keyValuePair.Key;
						this.orderedCFGNodes.Insert(i + 1, keyValuePair.Value);
						return keyValuePair.Value;
					}
				}
			}
			throw new Exception("Invalid state value");
		}

		private void ProcessCurrentNode(YieldExceptionHandlerInfo handlerInfo, Queue<ILogicalConstruct> bfsQueue, ILogicalConstruct currentNode)
		{
			int num;
			CFGBlockLogicalConstruct key;
			YieldExceptionHandlerInfo yieldExceptionHandlerInfo;
			if (currentNode is CFGBlockLogicalConstruct)
			{
				CFGBlockLogicalConstruct value = currentNode as CFGBlockLogicalConstruct;
				for (int i = 0; i < value.LogicalConstructExpressions.Count; i++)
				{
					Expression item = value.LogicalConstructExpressions[i];
					if (this.TryGetStateAssignValue(item, out num))
					{
						if (!handlerInfo.TryStates.Contains(num))
						{
							if (handlerInfo.HandlerType == YieldExceptionHandlerType.Method || !this.TryProcessConditionalDisposeHandler(handlerInfo, value))
							{
								throw new Exception("Invalid state value");
							}
							return;
						}
					}
					else if (handlerInfo.HandlerType == YieldExceptionHandlerType.Method && item.CodeNodeType == CodeNodeType.MethodInvocationExpression && (object)(item as MethodInvocationExpression).MethodExpression.MethodDefinition == (object)handlerInfo.FinallyMethodDefinition)
					{
						if (value.LogicalConstructExpressions.Count == 1)
						{
							if (this.newFinallyBody == null)
							{
								this.newFinallyBody = value;
							}
							this.finallyBlocks.Add(this.newFinallyBody);
							this.orderedCFGNodes.Remove(value);
							return;
						}
						if (i == 0)
						{
							KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> keyValuePair = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, value, i + 1);
							key = keyValuePair.Key;
							this.orderedCFGNodes[this.orderedCFGNodes.IndexOf(value)] = keyValuePair.Value;
						}
						else if (i >= value.LogicalConstructExpressions.Count - 1)
						{
							KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> keyValuePair1 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, value, i);
							this.newTryBody.Add(keyValuePair1.Key);
							key = keyValuePair1.Value;
							this.orderedCFGNodes.Remove(value);
						}
						else
						{
							KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> keyValuePair2 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, value, i);
							this.newTryBody.Add(keyValuePair2.Key);
							KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> keyValuePair3 = LogicalFlowUtilities.SplitCFGBlockAt(this.logicalContext, keyValuePair2.Value, 1);
							key = keyValuePair3.Key;
							this.orderedCFGNodes[this.orderedCFGNodes.IndexOf(value)] = keyValuePair3.Value;
						}
						if (this.newFinallyBody == null)
						{
							this.newFinallyBody = key;
						}
						this.finallyBlocks.Add(key);
						return;
					}
				}
			}
			else if (currentNode is TryFinallyLogicalConstruct && this.createdConstructsToIntervalMap.TryGetValue(currentNode as TryFinallyLogicalConstruct, out yieldExceptionHandlerInfo) && yieldExceptionHandlerInfo.TryStates.IsProperSupersetOf(handlerInfo.TryStates))
			{
				throw new Exception("This try/finally construct cannot be nested in the current construct");
			}
			this.newTryBody.Add(currentNode);
			foreach (ILogicalConstruct sameParentSuccessor in currentNode.SameParentSuccessors)
			{
				bfsQueue.Enqueue(sameParentSuccessor);
			}
		}

		private void ProcessFinallyNode(CFGBlockLogicalConstruct finallyCFGBlock)
		{
			this.ProcessFinallyNode(finallyCFGBlock, finallyCFGBlock);
		}

		private CFGBlockLogicalConstruct ProcessFinallyNode(CFGBlockLogicalConstruct finallyBlockEntry, CFGBlockLogicalConstruct finallyBlockEnd)
		{
			CFGBlockLogicalConstruct current;
			foreach (ILogicalConstruct sameParentPredecessor in finallyBlockEntry.SameParentPredecessors)
			{
				if (this.newTryBody.Contains(sameParentPredecessor))
				{
					continue;
				}
				throw new Exception("Invalid entry to the finally block");
			}
			using (IEnumerator<CFGBlockLogicalConstruct> enumerator = finallyBlockEnd.CFGSuccessors.GetEnumerator())
			{
				enumerator.MoveNext();
				current = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new Exception("Invalid count of successors");
				}
			}
			foreach (CFGBlockLogicalConstruct cFGBlockLogicalConstruct in new HashSet<CFGBlockLogicalConstruct>(finallyBlockEntry.CFGPredecessors))
			{
				if (cFGBlockLogicalConstruct.TheBlock != finallyBlockEntry.TheBlock && (int)cFGBlockLogicalConstruct.TheBlock.Successors.Length > 1)
				{
					this.ProcessMultiWayCFGPredecessor(finallyBlockEntry, cFGBlockLogicalConstruct.TheBlock, current.TheBlock);
				}
				for (LogicalConstructBase i = cFGBlockLogicalConstruct; i != finallyBlockEntry.Parent; i = i.Parent as LogicalConstructBase)
				{
					i.RemoveFromSuccessors(finallyBlockEntry);
					i.AddToSuccessors(current);
				}
				current.AddToPredecessors(cFGBlockLogicalConstruct);
				finallyBlockEntry.RemoveFromPredecessors(cFGBlockLogicalConstruct);
			}
			current.RemoveFromPredecessors(finallyBlockEnd);
			finallyBlockEnd.RemoveFromSuccessors(current);
			return current;
		}

		private void ProcessFinallyNodes()
		{
			foreach (CFGBlockLogicalConstruct finallyBlock in this.finallyBlocks)
			{
				this.ProcessFinallyNode(finallyBlock);
				if (finallyBlock == this.newFinallyBody)
				{
					continue;
				}
				this.DetachFromLogicalTree(finallyBlock);
			}
		}

		private void ProcessMultiWayCFGPredecessor(CFGBlockLogicalConstruct finallyBody, InstructionBlock theBlock, InstructionBlock theNewSuccessor)
		{
			SwitchData switchDatum;
			InstructionBlock instructionBlocks = finallyBody.TheBlock;
			for (int i = 0; i < (int)theBlock.Successors.Length; i++)
			{
				if (theBlock.Successors[i] == instructionBlocks)
				{
					theBlock.Successors[i] = theNewSuccessor;
				}
			}
			if (this.methodContext.ControlFlowGraph.SwitchBlocksInformation.TryGetValue(theBlock, out switchDatum))
			{
				InstructionBlock[] orderedCasesArray = switchDatum.OrderedCasesArray;
				for (int j = 0; j < (int)orderedCasesArray.Length; j++)
				{
					if (orderedCasesArray[j] == instructionBlocks)
					{
						orderedCasesArray[j] = theNewSuccessor;
					}
				}
				if (switchDatum.DefaultCase == instructionBlocks)
				{
					switchDatum.DefaultCase = theNewSuccessor;
				}
			}
		}

		private void RemoveExcessNodesFromTheTryBlock()
		{
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>(this.finallyBlocks);
			Queue<ILogicalConstruct> logicalConstructs1 = new Queue<ILogicalConstruct>(this.finallyBlocks);
			while (logicalConstructs1.Count > 0)
			{
				foreach (ILogicalConstruct sameParentSuccessor in logicalConstructs1.Dequeue().SameParentSuccessors)
				{
					if (logicalConstructs.Contains(sameParentSuccessor) || sameParentSuccessor == this.entryOfTry)
					{
						continue;
					}
					logicalConstructs.Add(sameParentSuccessor);
					logicalConstructs1.Enqueue(sameParentSuccessor);
				}
			}
			foreach (ILogicalConstruct logicalConstruct in logicalConstructs)
			{
				if (logicalConstruct == this.entryOfTry)
				{
					continue;
				}
				this.newTryBody.Remove(logicalConstruct);
			}
		}

		private bool TryGetStateAssignValue(Expression expression, out int value)
		{
			if (expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expression as BinaryExpression).IsAssignmentExpression)
			{
				value = -1;
				return false;
			}
			BinaryExpression binaryExpression = expression as BinaryExpression;
			if (binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || (binaryExpression.Left as FieldReferenceExpression).Field.Resolve() != this.stateFieldRef)
			{
				value = -1;
				return false;
			}
			if (binaryExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				throw new Exception("Incorrect value for state field");
			}
			value = Convert.ToInt32((binaryExpression.Right as LiteralExpression).Value);
			return true;
		}

		private bool TryProcessConditionalDisposeHandler(YieldExceptionHandlerInfo yieldExceptionHandler, CFGBlockLogicalConstruct startBlock)
		{
			CFGBlockLogicalConstruct current;
			MethodInvocationExpression item;
			bool flag;
			bool flag1;
			bool flag2;
			if (this.finallyBlocks.Count > 0)
			{
				return false;
			}
			if (!(startBlock is PartialCFGBlockLogicalConstruct) || startBlock.CFGSuccessors.Count != 1)
			{
				return false;
			}
			if (startBlock.LogicalConstructExpressions.Count == 0)
			{
				return false;
			}
			BinaryExpression binaryExpression = startBlock.LogicalConstructExpressions[0] as BinaryExpression;
			if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || (binaryExpression.Left as FieldReferenceExpression).Field.Resolve() != this.stateFieldRef || binaryExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression || (Int32)(binaryExpression.Right as LiteralExpression).Value != yieldExceptionHandler.NextState)
			{
				return false;
			}
			if (startBlock.LogicalConstructExpressions.Count == 2 && yieldExceptionHandler.HandlerType == YieldExceptionHandlerType.ConditionalDispose)
			{
				BinaryExpression item1 = startBlock.LogicalConstructExpressions[1] as BinaryExpression;
				if (item1 == null || !item1.IsAssignmentExpression || item1.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(item1.Left as FieldReferenceExpression).Field != (object)yieldExceptionHandler.DisposableField || item1.Right.CodeNodeType != CodeNodeType.SafeCastExpression || (item1.Right as SafeCastExpression).Expression.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)((item1.Right as SafeCastExpression).Expression as FieldReferenceExpression).Field != (object)yieldExceptionHandler.EnumeratorField)
				{
					return false;
				}
			}
			else if (startBlock.LogicalConstructExpressions.Count != 1 || yieldExceptionHandler.HandlerType != YieldExceptionHandlerType.SimpleConditionalDispose)
			{
				return false;
			}
			IEnumerator<CFGBlockLogicalConstruct> enumerator = startBlock.CFGSuccessors.GetEnumerator();
			using (enumerator)
			{
				enumerator.MoveNext();
				current = enumerator.Current;
			}
			if (current.LogicalConstructExpressions.Count != 1)
			{
				return false;
			}
			BinaryExpression binaryExpression1 = current.LogicalConstructExpressions[0] as BinaryExpression;
			if (binaryExpression1 == null || binaryExpression1.Operator != BinaryOperator.ValueEquality || binaryExpression1.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(binaryExpression1.Left as FieldReferenceExpression).Field != (object)yieldExceptionHandler.DisposableField || binaryExpression1.Right.CodeNodeType != CodeNodeType.LiteralExpression || (binaryExpression1.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct = null;
			foreach (CFGBlockLogicalConstruct cFGSuccessor in current.CFGSuccessors)
			{
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct1 = cFGSuccessor as CFGBlockLogicalConstruct;
				if (cFGBlockLogicalConstruct1 == null || cFGBlockLogicalConstruct1.CFGPredecessors.Count != 1)
				{
					continue;
				}
				cFGBlockLogicalConstruct = cFGBlockLogicalConstruct1;
				if (cFGBlockLogicalConstruct == null || cFGBlockLogicalConstruct.LogicalConstructExpressions.Count != 1)
				{
					return false;
				}
				item = cFGBlockLogicalConstruct.LogicalConstructExpressions[0] as MethodInvocationExpression;
				if (item == null || !item.VirtualCall || item.MethodExpression.Target.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(item.MethodExpression.Target as FieldReferenceExpression).Field != (object)yieldExceptionHandler.DisposableField || item.MethodExpression.Method.get_Name() != "Dispose")
				{
					return false;
				}
				this.finallyEntryBlock = startBlock;
				flag = this.finallyBlocks.Add(startBlock);
				this.conditionBlock = current;
				flag1 = this.finallyBlocks.Add(current);
				this.disposeCallBlock = cFGBlockLogicalConstruct;
				flag2 = this.finallyBlocks.Add(cFGBlockLogicalConstruct);
				return true;
			}
			if (cFGBlockLogicalConstruct == null || cFGBlockLogicalConstruct.LogicalConstructExpressions.Count != 1)
			{
				return false;
			}
			item = cFGBlockLogicalConstruct.LogicalConstructExpressions[0] as MethodInvocationExpression;
			if (item == null || !item.VirtualCall || item.MethodExpression.Target.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(item.MethodExpression.Target as FieldReferenceExpression).Field != (object)yieldExceptionHandler.DisposableField || item.MethodExpression.Method.get_Name() != "Dispose")
			{
				return false;
			}
			this.finallyEntryBlock = startBlock;
			flag = this.finallyBlocks.Add(startBlock);
			this.conditionBlock = current;
			flag1 = this.finallyBlocks.Add(current);
			this.disposeCallBlock = cFGBlockLogicalConstruct;
			flag2 = this.finallyBlocks.Add(cFGBlockLogicalConstruct);
			return true;
		}
	}
}