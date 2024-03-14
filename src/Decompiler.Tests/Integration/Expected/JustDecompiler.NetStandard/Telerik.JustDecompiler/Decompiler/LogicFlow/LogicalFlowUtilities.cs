using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal static class LogicalFlowUtilities
	{
		public static ISingleEntrySubGraph FindFirstCommonParent(IEnumerable<ISingleEntrySubGraph> blocks)
		{
			Queue<ISingleEntrySubGraph> singleEntrySubGraphs = new Queue<ISingleEntrySubGraph>();
			HashSet<ISingleEntrySubGraph> singleEntrySubGraphs1 = new HashSet<ISingleEntrySubGraph>();
			foreach (ISingleEntrySubGraph block in blocks)
			{
				if (singleEntrySubGraphs.Count != 0)
				{
					ISingleEntrySubGraph parent = block;
					while (!singleEntrySubGraphs1.Contains(parent))
					{
						parent = parent.Parent;
					}
					if (!singleEntrySubGraphs1.Contains(parent))
					{
						continue;
					}
					while (singleEntrySubGraphs.Peek() != parent)
					{
						singleEntrySubGraphs1.Remove(singleEntrySubGraphs.Dequeue());
					}
				}
				else
				{
					for (ISingleEntrySubGraph i = block; i != null; i = i.Parent)
					{
						singleEntrySubGraphs.Enqueue(i);
						singleEntrySubGraphs1.Add(i);
					}
				}
			}
			return singleEntrySubGraphs.Peek();
		}

		public static BlockLogicalConstruct GetNearestGuardedBlock(ILogicalConstruct start)
		{
			ILogicalConstruct parent = start;
			while (parent != null && !(parent is ExceptionHandlingLogicalConstruct))
			{
				parent = (ILogicalConstruct)parent.Parent;
			}
			if (parent == null)
			{
				return null;
			}
			if ((parent as ExceptionHandlingLogicalConstruct).Try.CFGBlocks.Contains(start.FirstBlock))
			{
				return (parent as ExceptionHandlingLogicalConstruct).Try;
			}
			if (!(parent is TryCatchFilterLogicalConstruct))
			{
				if (parent is TryFinallyLogicalConstruct)
				{
					return (parent as TryFinallyLogicalConstruct).Finally;
				}
				if (!(parent is TryFaultLogicalConstruct))
				{
					throw new Exception("Unknown type of exception handling logical construct encountered.");
				}
				return (parent as TryFaultLogicalConstruct).Fault;
			}
			IFilteringExceptionHandler[] handlers = (parent as TryCatchFilterLogicalConstruct).Handlers;
			for (int i = 0; i < (int)handlers.Length; i++)
			{
				IFilteringExceptionHandler filteringExceptionHandler = handlers[i];
				if (filteringExceptionHandler.HandlerType == FilteringExceptionHandlerType.Filter)
				{
					if ((filteringExceptionHandler as ExceptionHandlingBlockFilter).Filter.CFGBlocks.Contains(start.FirstBlock))
					{
						return (filteringExceptionHandler as ExceptionHandlingBlockFilter).Filter;
					}
					return (filteringExceptionHandler as ExceptionHandlingBlockFilter).Handler;
				}
				if (filteringExceptionHandler.HandlerType == FilteringExceptionHandlerType.Catch)
				{
					return filteringExceptionHandler as ExceptionHandlingBlockCatch;
				}
			}
			return null;
		}

		public static ICollection<ISingleEntrySubGraph> GetTraversablePredecessors(ISingleEntrySubGraph construct)
		{
			List<ISingleEntrySubGraph> singleEntrySubGraphs = new List<ISingleEntrySubGraph>(construct.SameParentPredecessors);
			singleEntrySubGraphs.Sort();
			return singleEntrySubGraphs;
		}

		public static ICollection<ISingleEntrySubGraph> GetTraversableSuccessors(ISingleEntrySubGraph construct)
		{
			List<ISingleEntrySubGraph> singleEntrySubGraphs = new List<ISingleEntrySubGraph>(construct.SameParentSuccessors);
			singleEntrySubGraphs.Sort();
			return singleEntrySubGraphs;
		}

		public static KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> SplitCFGBlockAt(LogicalFlowBuilderContext logicalContext, CFGBlockLogicalConstruct cfgBlock, int expressionIndex)
		{
			List<Expression> logicalConstructExpressions = cfgBlock.LogicalConstructExpressions;
			if (logicalConstructExpressions == null)
			{
				throw new ArgumentNullException("blockExpressions");
			}
			if (expressionIndex <= 0 || expressionIndex >= logicalConstructExpressions.Count)
			{
				throw new ArgumentOutOfRangeException("expressionIndex");
			}
			CFGBlockLogicalConstruct[] item = logicalContext.CFGBlockToLogicalConstructMap[cfgBlock.TheBlock];
			int length = (int)item.Length;
			int num = 0;
			while (num < length && cfgBlock != item[num])
			{
				num++;
			}
			if (num == length)
			{
				throw new ArgumentException("cfgBlock");
			}
			List<Expression> range = cfgBlock.LogicalConstructExpressions.GetRange(0, expressionIndex);
			PartialCFGBlockLogicalConstruct partialCFGBlockLogicalConstruct = new PartialCFGBlockLogicalConstruct(cfgBlock, range);
			partialCFGBlockLogicalConstruct.RedirectPredecessors();
			List<Expression> expressions = cfgBlock.LogicalConstructExpressions.GetRange(expressionIndex, logicalConstructExpressions.Count - expressionIndex);
			PartialCFGBlockLogicalConstruct partialCFGBlockLogicalConstruct1 = new PartialCFGBlockLogicalConstruct(cfgBlock, expressions);
			partialCFGBlockLogicalConstruct1.RedirectSuccessors();
			partialCFGBlockLogicalConstruct.AddToSuccessors(partialCFGBlockLogicalConstruct1);
			partialCFGBlockLogicalConstruct1.AddToPredecessors(partialCFGBlockLogicalConstruct);
			CFGBlockLogicalConstruct[] cFGBlockLogicalConstructArray = new CFGBlockLogicalConstruct[length + 1];
			int num1 = 0;
			int num2 = 0;
			while (num1 < length)
			{
				if (num1 == num)
				{
					cFGBlockLogicalConstructArray[num2] = partialCFGBlockLogicalConstruct;
					int num3 = num2 + 1;
					num2 = num3;
					cFGBlockLogicalConstructArray[num3] = partialCFGBlockLogicalConstruct1;
				}
				else
				{
					cFGBlockLogicalConstructArray[num2] = item[num1];
				}
				num1++;
				num2++;
			}
			logicalContext.CFGBlockToLogicalConstructMap[cfgBlock.TheBlock] = cFGBlockLogicalConstructArray;
			return new KeyValuePair<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>(partialCFGBlockLogicalConstruct, partialCFGBlockLogicalConstruct1);
		}

		public static bool TryGetParentConstructWithGivenParent(ILogicalConstruct initialNode, ILogicalConstruct parent, out ILogicalConstruct desiredNode)
		{
			if (initialNode == null || parent == null)
			{
				desiredNode = null;
				return false;
			}
			desiredNode = initialNode;
			while (desiredNode.Parent != null)
			{
				if (desiredNode.Parent == parent)
				{
					return true;
				}
				desiredNode = desiredNode.Parent as ILogicalConstruct;
			}
			desiredNode = null;
			return false;
		}
	}
}