using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public abstract class LogicalConstructBase : ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{
		protected static HashSet<ISingleEntrySubGraph> EmptyISingleEntrySubGraphSet;

		private readonly HashSet<CFGBlockLogicalConstruct> predecessors;

		private readonly HashSet<CFGBlockLogicalConstruct> successors;

		private readonly HashSet<ISingleEntrySubGraph> children;

		protected ISingleEntrySubGraph parent;

		public virtual HashSet<ISingleEntrySubGraph> AllPredecessors
		{
			get
			{
				ILogicalConstruct logicalConstruct;
				HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
				foreach (CFGBlockLogicalConstruct predecessor in this.predecessors)
				{
					if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(predecessor, LogicalFlowUtilities.FindFirstCommonParent((IEnumerable<ISingleEntrySubGraph>)(new ISingleEntrySubGraph[] { this.parent, predecessor })) as ILogicalConstruct, out logicalConstruct))
					{
						continue;
					}
					singleEntrySubGraphs.Add(logicalConstruct);
				}
				return singleEntrySubGraphs;
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> AllSuccessors
		{
			get
			{
				ILogicalConstruct logicalConstruct;
				HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
				foreach (CFGBlockLogicalConstruct successor in this.successors)
				{
					if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(successor, LogicalFlowUtilities.FindFirstCommonParent((IEnumerable<ISingleEntrySubGraph>)(new ISingleEntrySubGraph[] { this.parent, successor })) as ILogicalConstruct, out logicalConstruct))
					{
						continue;
					}
					singleEntrySubGraphs.Add(logicalConstruct);
				}
				return singleEntrySubGraphs;
			}
		}

		public virtual HashSet<CFGBlockLogicalConstruct> CFGBlocks
		{
			get
			{
				HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs = new HashSet<CFGBlockLogicalConstruct>();
				foreach (LogicalConstructBase child in this.Children)
				{
					cFGBlockLogicalConstructs.UnionWith(child.CFGBlocks);
				}
				return cFGBlockLogicalConstructs;
			}
		}

		public CFGBlockLogicalConstruct CFGFollowNode
		{
			get;
			set;
		}

		public HashSet<CFGBlockLogicalConstruct> CFGPredecessors
		{
			get
			{
				return new HashSet<CFGBlockLogicalConstruct>(this.predecessors);
			}
		}

		public HashSet<CFGBlockLogicalConstruct> CFGSuccessors
		{
			get
			{
				return new HashSet<CFGBlockLogicalConstruct>(this.successors);
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> Children
		{
			get
			{
				return this.children;
			}
		}

		public virtual ISingleEntrySubGraph Entry
		{
			get;
			set;
		}

		public virtual CFGBlockLogicalConstruct FirstBlock
		{
			get
			{
				return (this.Entry as ILogicalConstruct).FirstBlock;
			}
		}

		public ILogicalConstruct FollowNode
		{
			get
			{
				ILogicalConstruct logicalConstruct;
				if (this.CFGFollowNode == null)
				{
					return null;
				}
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this.CFGFollowNode, this.parent as ILogicalConstruct, out logicalConstruct))
				{
					return logicalConstruct;
				}
				this.CFGFollowNode = null;
				return null;
			}
		}

		public virtual int Index
		{
			get
			{
				return this.FirstBlock.Index;
			}
		}

		public ISingleEntrySubGraph Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> SameParentPredecessors
		{
			get
			{
				ILogicalConstruct logicalConstruct;
				HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
				foreach (CFGBlockLogicalConstruct predecessor in this.predecessors)
				{
					if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(predecessor, this.Parent as ILogicalConstruct, out logicalConstruct))
					{
						continue;
					}
					singleEntrySubGraphs.Add(logicalConstruct);
				}
				return singleEntrySubGraphs;
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> SameParentSuccessors
		{
			get
			{
				ILogicalConstruct logicalConstruct;
				HashSet<ISingleEntrySubGraph> singleEntrySubGraphs = new HashSet<ISingleEntrySubGraph>();
				foreach (CFGBlockLogicalConstruct successor in this.successors)
				{
					if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(successor, this.Parent as LogicalConstructBase, out logicalConstruct))
					{
						continue;
					}
					singleEntrySubGraphs.Add(logicalConstruct);
				}
				return singleEntrySubGraphs;
			}
		}

		static LogicalConstructBase()
		{
			LogicalConstructBase.EmptyISingleEntrySubGraphSet = new HashSet<ISingleEntrySubGraph>();
		}

		protected LogicalConstructBase()
		{
			this.predecessors = new HashSet<CFGBlockLogicalConstruct>();
			this.successors = new HashSet<CFGBlockLogicalConstruct>();
			this.children = new HashSet<ISingleEntrySubGraph>();
		}

		public void AddToPredecessors(CFGBlockLogicalConstruct predecessor)
		{
			this.predecessors.Add(predecessor);
		}

		public void AddToSuccessors(ILogicalConstruct successor)
		{
			this.successors.Add(successor.FirstBlock);
		}

		private void CleanUpPredecessors()
		{
			foreach (CFGBlockLogicalConstruct cFGBlock in this.CFGBlocks)
			{
				this.RemoveFromPredecessors(cFGBlock);
			}
		}

		public virtual int CompareTo(ISingleEntrySubGraph other)
		{
			return this.FirstBlock.CompareTo((other as ILogicalConstruct).FirstBlock);
		}

		private void CopyPredecessors()
		{
			foreach (CFGBlockLogicalConstruct predecessor in (this.Entry as LogicalConstructBase).predecessors)
			{
				if (predecessor.HasForParent(this))
				{
					continue;
				}
				this.predecessors.Add(predecessor);
			}
		}

		protected ILogicalConstruct[] GetSortedArrayFromCollection<T>(ICollection<T> collection)
		where T : ISingleEntrySubGraph
		{
			ILogicalConstruct[] logicalConstructArray = new ILogicalConstruct[collection.Count];
			int num = 0;
			foreach (T t in collection)
			{
				ILogicalConstruct logicalConstruct = (ILogicalConstruct)(object)t;
				int num1 = num;
				num = num1 + 1;
				logicalConstructArray[num1] = logicalConstruct;
			}
			Array.Sort<ISingleEntrySubGraph>(logicalConstructArray);
			return logicalConstructArray;
		}

		private bool HasForParent(ILogicalConstruct supposedParent)
		{
			ILogicalConstruct logicalConstruct;
			return LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this, supposedParent, out logicalConstruct);
		}

		protected string NodeILOffset(LogicalFlowBuilderContext context, CFGBlockLogicalConstruct node)
		{
			int offset;
			if (node == null)
			{
				return "null";
			}
			PartialCFGBlockLogicalConstruct partialCFGBlockLogicalConstruct = node as PartialCFGBlockLogicalConstruct;
			if (partialCFGBlockLogicalConstruct == null)
			{
				offset = node.TheBlock.First.get_Offset();
				return String.Format("IL_{0}", offset.ToString("x4"));
			}
			int num = Array.IndexOf<CFGBlockLogicalConstruct>(context.CFGBlockToLogicalConstructMap[partialCFGBlockLogicalConstruct.TheBlock], partialCFGBlockLogicalConstruct);
			if (num == -1)
			{
				throw new Exception("Invalid partial block data.");
			}
			offset = partialCFGBlockLogicalConstruct.TheBlock.First.get_Offset();
			return String.Format("IL_{0}_{1}", (object)offset.ToString("x4"), num);
		}

		protected void RedirectChildrenToNewParent(IEnumerable<ILogicalConstruct> body)
		{
			bool count = this.Children.Count == 0;
			if (this.successors.Count != 0)
			{
				foreach (LogicalConstructBase logicalConstructBase in body)
				{
					this.RemoveFromSuccessors(logicalConstructBase);
				}
			}
			this.RedirectParents(body);
			if (!count)
			{
				this.CleanUpPredecessors();
			}
			else
			{
				if (this.parent != null)
				{
					this.RedirectParentEntry();
				}
				this.CopyPredecessors();
			}
			foreach (ILogicalConstruct logicalConstruct in body)
			{
				foreach (CFGBlockLogicalConstruct successor in ((LogicalConstructBase)logicalConstruct).successors)
				{
					if (successor.HasForParent(this))
					{
						continue;
					}
					this.successors.Add(successor);
				}
			}
		}

		private void RedirectParentEntry()
		{
			if (this.parent.Entry.Parent != this.parent)
			{
				if (this.parent.Entry.Parent != this)
				{
					throw new InvalidOperationException("Invalid entry of parent");
				}
				if (this.parent.Entry != this.Entry)
				{
					throw new InvalidOperationException("Invalid entry of new construct");
				}
				this.parent.Entry = this;
			}
		}

		private void RedirectParents(IEnumerable<ILogicalConstruct> childrenCollection)
		{
			ISingleEntrySubGraph parent = null;
			foreach (ILogicalConstruct logicalConstruct in childrenCollection)
			{
				if (logicalConstruct.Parent == this)
				{
					continue;
				}
				if (parent == null)
				{
					parent = logicalConstruct.Parent;
				}
				else if (parent != logicalConstruct.Parent)
				{
					throw new InvalidOperationException("The nodes in the child collection does not have the same parent");
				}
				if (parent != null)
				{
					parent.Children.Remove(logicalConstruct);
				}
				logicalConstruct.Parent = this;
				this.Children.Add(logicalConstruct);
			}
			if (parent != null)
			{
				parent.Children.Add(this);
				if (this.parent != null && this.parent != parent)
				{
					throw new InvalidOperationException("The nodes you are trying to add are not from the same logical construct");
				}
				if (this.parent == null)
				{
					this.parent = parent;
				}
			}
		}

		public bool RemoveFromPredecessors(CFGBlockLogicalConstruct predecessor)
		{
			return this.predecessors.Remove(predecessor);
		}

		public bool RemoveFromSuccessors(ILogicalConstruct successor)
		{
			return this.successors.Remove(successor.FirstBlock);
		}

		public virtual string ToString(LogicalFlowBuilderContext context)
		{
			return this.ToString(this.GetType().Name, new HashSet<CFGBlockLogicalConstruct>(), context);
		}

		protected virtual string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedCFGBlocks, LogicalFlowBuilderContext context)
		{
			ILogicalConstruct logicalConstruct;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(constructName);
			stringBuilder.AppendLine("{");
			ILogicalConstruct[] sortedArrayFromCollection = this.GetSortedArrayFromCollection<CFGBlockLogicalConstruct>(this.CFGBlocks);
			Stack<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs = new Stack<CFGBlockLogicalConstruct>();
			for (int i = 0; i < (int)sortedArrayFromCollection.Length; i++)
			{
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct = sortedArrayFromCollection[i] as CFGBlockLogicalConstruct;
				if (!printedCFGBlocks.Contains(cFGBlockLogicalConstruct))
				{
					cFGBlockLogicalConstructs.Push(cFGBlockLogicalConstruct);
					while (cFGBlockLogicalConstructs.Count > 0)
					{
						CFGBlockLogicalConstruct cFGBlockLogicalConstruct1 = cFGBlockLogicalConstructs.Pop();
						if (printedCFGBlocks.Contains(cFGBlockLogicalConstruct1) || !LogicalFlowUtilities.TryGetParentConstructWithGivenParent(cFGBlockLogicalConstruct1, this, out logicalConstruct))
						{
							continue;
						}
						string[] strArray = ((LogicalConstructBase)logicalConstruct).ToString(logicalConstruct.GetType().Name, printedCFGBlocks, context).Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
						for (int j = 0; j < (int)strArray.Length; j++)
						{
							string str = strArray[j];
							stringBuilder.AppendLine(String.Format("\t{0}", str));
						}
						ILogicalConstruct[] logicalConstructArray = this.GetSortedArrayFromCollection<ISingleEntrySubGraph>(logicalConstruct.SameParentSuccessors);
						for (int k = (int)logicalConstructArray.Length - 1; k >= 0; k--)
						{
							if (!printedCFGBlocks.Contains(logicalConstructArray[k].FirstBlock))
							{
								cFGBlockLogicalConstructs.Push(logicalConstructArray[k].FirstBlock);
							}
						}
					}
				}
			}
			string str1 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.CFGFollowNode));
			stringBuilder.AppendLine(str1);
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}
}