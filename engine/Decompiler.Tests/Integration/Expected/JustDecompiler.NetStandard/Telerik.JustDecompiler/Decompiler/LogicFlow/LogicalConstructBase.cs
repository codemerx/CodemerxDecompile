using System;
using System.Collections.Generic;
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
				V_0 = new HashSet<ISingleEntrySubGraph>();
				V_1 = this.predecessors.GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						stackVariable9 = new ISingleEntrySubGraph[2];
						stackVariable9[0] = this.parent;
						stackVariable9[1] = V_2;
						if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_2, LogicalFlowUtilities.FindFirstCommonParent((IEnumerable<ISingleEntrySubGraph>)stackVariable9) as ILogicalConstruct, out V_4))
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_4);
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
				return V_0;
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> AllSuccessors
		{
			get
			{
				V_0 = new HashSet<ISingleEntrySubGraph>();
				V_1 = this.successors.GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						stackVariable9 = new ISingleEntrySubGraph[2];
						stackVariable9[0] = this.parent;
						stackVariable9[1] = V_2;
						if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_2, LogicalFlowUtilities.FindFirstCommonParent((IEnumerable<ISingleEntrySubGraph>)stackVariable9) as ILogicalConstruct, out V_4))
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_4);
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
				return V_0;
			}
		}

		public virtual HashSet<CFGBlockLogicalConstruct> CFGBlocks
		{
			get
			{
				V_0 = new HashSet<CFGBlockLogicalConstruct>();
				V_1 = this.get_Children().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = (LogicalConstructBase)V_1.get_Current();
						V_0.UnionWith(V_2.get_CFGBlocks());
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
				return V_0;
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
				return (this.get_Entry() as ILogicalConstruct).get_FirstBlock();
			}
		}

		public ILogicalConstruct FollowNode
		{
			get
			{
				if (this.get_CFGFollowNode() == null)
				{
					return null;
				}
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this.get_CFGFollowNode(), this.parent as ILogicalConstruct, out V_0))
				{
					return V_0;
				}
				this.set_CFGFollowNode(null);
				return null;
			}
		}

		public virtual int Index
		{
			get
			{
				return this.get_FirstBlock().get_Index();
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
				return;
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> SameParentPredecessors
		{
			get
			{
				V_0 = new HashSet<ISingleEntrySubGraph>();
				V_1 = this.predecessors.GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_1.get_Current(), this.get_Parent() as ILogicalConstruct, out V_2))
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_2);
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
				return V_0;
			}
		}

		public virtual HashSet<ISingleEntrySubGraph> SameParentSuccessors
		{
			get
			{
				V_0 = new HashSet<ISingleEntrySubGraph>();
				V_1 = this.successors.GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						if (!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_1.get_Current(), this.get_Parent() as LogicalConstructBase, out V_2))
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_2);
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
				return V_0;
			}
		}

		static LogicalConstructBase()
		{
			LogicalConstructBase.EmptyISingleEntrySubGraphSet = new HashSet<ISingleEntrySubGraph>();
			return;
		}

		protected LogicalConstructBase()
		{
			base();
			this.predecessors = new HashSet<CFGBlockLogicalConstruct>();
			this.successors = new HashSet<CFGBlockLogicalConstruct>();
			this.children = new HashSet<ISingleEntrySubGraph>();
			return;
		}

		public void AddToPredecessors(CFGBlockLogicalConstruct predecessor)
		{
			dummyVar0 = this.predecessors.Add(predecessor);
			return;
		}

		public void AddToSuccessors(ILogicalConstruct successor)
		{
			dummyVar0 = this.successors.Add(successor.get_FirstBlock());
			return;
		}

		private void CleanUpPredecessors()
		{
			V_0 = this.get_CFGBlocks().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					dummyVar0 = this.RemoveFromPredecessors(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public virtual int CompareTo(ISingleEntrySubGraph other)
		{
			return this.get_FirstBlock().CompareTo((other as ILogicalConstruct).get_FirstBlock());
		}

		private void CopyPredecessors()
		{
			V_0 = (this.get_Entry() as LogicalConstructBase).predecessors.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.HasForParent(this))
					{
						continue;
					}
					dummyVar0 = this.predecessors.Add(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		protected ILogicalConstruct[] GetSortedArrayFromCollection<T>(ICollection<T> collection)
		where T : ISingleEntrySubGraph
		{
			V_0 = new ILogicalConstruct[collection.get_Count()];
			V_1 = 0;
			V_2 = collection.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = (ILogicalConstruct)(object)V_2.get_Current();
					stackVariable13 = V_1;
					V_1 = stackVariable13 + 1;
					V_0[stackVariable13] = V_3;
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			Array.Sort<ISingleEntrySubGraph>(V_0);
			return V_0;
		}

		private bool HasForParent(ILogicalConstruct supposedParent)
		{
			return LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this, supposedParent, out V_0);
		}

		protected string NodeILOffset(LogicalFlowBuilderContext context, CFGBlockLogicalConstruct node)
		{
			if (node == null)
			{
				return "null";
			}
			V_0 = node as PartialCFGBlockLogicalConstruct;
			if (V_0 == null)
			{
				V_2 = node.get_TheBlock().get_First().get_Offset();
				return String.Format("IL_{0}", V_2.ToString("x4"));
			}
			V_1 = Array.IndexOf<CFGBlockLogicalConstruct>(context.get_CFGBlockToLogicalConstructMap().get_Item(V_0.get_TheBlock()), V_0);
			if (V_1 == -1)
			{
				throw new Exception("Invalid partial block data.");
			}
			V_2 = V_0.get_TheBlock().get_First().get_Offset();
			return String.Format("IL_{0}_{1}", V_2.ToString("x4"), V_1);
		}

		protected void RedirectChildrenToNewParent(IEnumerable<ILogicalConstruct> body)
		{
			V_0 = this.get_Children().get_Count() == 0;
			if (this.successors.get_Count() != 0)
			{
				V_1 = body.GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = (LogicalConstructBase)V_1.get_Current();
						dummyVar0 = this.RemoveFromSuccessors(V_2);
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			this.RedirectParents(body);
			if (!V_0)
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
			V_1 = body.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_3 = ((LogicalConstructBase)V_1.get_Current()).successors.GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							if (V_4.HasForParent(this))
							{
								continue;
							}
							dummyVar1 = this.successors.Add(V_4);
						}
					}
					finally
					{
						((IDisposable)V_3).Dispose();
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
			return;
		}

		private void RedirectParentEntry()
		{
			if (this.parent.get_Entry().get_Parent() != this.parent)
			{
				if (this.parent.get_Entry().get_Parent() != this)
				{
					throw new InvalidOperationException("Invalid entry of parent");
				}
				if (this.parent.get_Entry() != this.get_Entry())
				{
					throw new InvalidOperationException("Invalid entry of new construct");
				}
				this.parent.set_Entry(this);
			}
			return;
		}

		private void RedirectParents(IEnumerable<ILogicalConstruct> childrenCollection)
		{
			V_0 = null;
			V_1 = childrenCollection.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Parent() == this)
					{
						continue;
					}
					if (V_0 != null)
					{
						if (V_0 != V_2.get_Parent())
						{
							throw new InvalidOperationException("The nodes in the child collection does not have the same parent");
						}
					}
					else
					{
						V_0 = V_2.get_Parent();
					}
					if (V_0 != null)
					{
						dummyVar0 = V_0.get_Children().Remove(V_2);
					}
					V_2.set_Parent(this);
					dummyVar1 = this.get_Children().Add(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			if (V_0 != null)
			{
				dummyVar2 = V_0.get_Children().Add(this);
				if (this.parent != null && this.parent != V_0)
				{
					throw new InvalidOperationException("The nodes you are trying to add are not from the same logical construct");
				}
				if (this.parent == null)
				{
					this.parent = V_0;
				}
			}
			return;
		}

		public bool RemoveFromPredecessors(CFGBlockLogicalConstruct predecessor)
		{
			return this.predecessors.Remove(predecessor);
		}

		public bool RemoveFromSuccessors(ILogicalConstruct successor)
		{
			return this.successors.Remove(successor.get_FirstBlock());
		}

		public virtual string ToString(LogicalFlowBuilderContext context)
		{
			return this.ToString(this.GetType().get_Name(), new HashSet<CFGBlockLogicalConstruct>(), context);
		}

		protected virtual string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedCFGBlocks, LogicalFlowBuilderContext context)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.AppendLine(constructName);
			dummyVar1 = V_0.AppendLine("{");
			V_1 = this.GetSortedArrayFromCollection<CFGBlockLogicalConstruct>(this.get_CFGBlocks());
			V_2 = new Stack<CFGBlockLogicalConstruct>();
			V_4 = 0;
			while (V_4 < (int)V_1.Length)
			{
				V_5 = V_1[V_4] as CFGBlockLogicalConstruct;
				if (!printedCFGBlocks.Contains(V_5))
				{
					V_2.Push(V_5);
					while (V_2.get_Count() > 0)
					{
						V_6 = V_2.Pop();
						if (printedCFGBlocks.Contains(V_6) || !LogicalFlowUtilities.TryGetParentConstructWithGivenParent(V_6, this, out V_7))
						{
							continue;
						}
						stackVariable48 = ((LogicalConstructBase)V_7).ToString(V_7.GetType().get_Name(), printedCFGBlocks, context);
						stackVariable50 = new String[1];
						stackVariable50[0] = Environment.get_NewLine();
						V_9 = stackVariable48.Split(stackVariable50, 1);
						V_10 = 0;
						while (V_10 < (int)V_9.Length)
						{
							V_11 = V_9[V_10];
							dummyVar2 = V_0.AppendLine(String.Format("\t{0}", V_11));
							V_10 = V_10 + 1;
						}
						V_8 = this.GetSortedArrayFromCollection<ISingleEntrySubGraph>(V_7.get_SameParentSuccessors());
						V_12 = (int)V_8.Length - 1;
						while (V_12 >= 0)
						{
							if (!printedCFGBlocks.Contains(V_8[V_12].get_FirstBlock()))
							{
								V_2.Push(V_8[V_12].get_FirstBlock());
							}
							V_12 = V_12 - 1;
						}
					}
				}
				V_4 = V_4 + 1;
			}
			V_3 = String.Format("\tFollowNode: {0}", this.NodeILOffset(context, this.get_CFGFollowNode()));
			dummyVar3 = V_0.AppendLine(V_3);
			dummyVar4 = V_0.AppendLine("}");
			return V_0.ToString();
		}
	}
}