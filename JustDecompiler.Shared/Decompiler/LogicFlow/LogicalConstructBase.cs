using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using System.Text;
using System;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    public abstract class LogicalConstructBase : ILogicalConstruct
    {
        protected static HashSet<ISingleEntrySubGraph> EmptyISingleEntrySubGraphSet = new HashSet<ISingleEntrySubGraph>();

        private readonly HashSet<CFGBlockLogicalConstruct> predecessors, successors;
		private readonly HashSet<ISingleEntrySubGraph> children;
        protected ISingleEntrySubGraph parent;
        
        protected LogicalConstructBase()
        {
			predecessors = new HashSet<CFGBlockLogicalConstruct>();
			successors = new HashSet<CFGBlockLogicalConstruct>();
            children = new HashSet<ISingleEntrySubGraph>();
        }

        /// <summary>
        /// Gets the follow construct that has the same parent as this construct. The follow node will be the next sibling of this construct in the AST generated
        /// by the StatementDecompilerStep.
        /// </summary>
		public ILogicalConstruct FollowNode
		{
            get
            {
                ILogicalConstruct followNode;
                if (CFGFollowNode == null)
                {
                    return null;
                }
                else if(LogicalFlowUtilities.TryGetParentConstructWithGivenParent(CFGFollowNode, parent as ILogicalConstruct, out followNode))
                {
                    return followNode;
                }

                //If the CFGFollow node does not have 
                CFGFollowNode = null;
                return null;
            }
		}

        /// <summary>
        /// Gets or sets the CFG block logical construct that is entry of the follow node.
        /// </summary>
        public CFGBlockLogicalConstruct CFGFollowNode { get; set; }

        /// <summary>
        /// Gets the index of the construct. Used for total ordering when traversing the constructs. Determined by the index of the entry instruction block.
        /// That's why partial CFG logical constructs that represent the same block will have the same index.
        /// </summary>
        public virtual int Index
        {
            get
            {
                return FirstBlock.Index;
            }
        }

        /// <summary>
        /// Gets or sets the control flow entry child of the construct.
        /// </summary>
        public virtual ISingleEntrySubGraph Entry { get; set; }

        /// <summary>
        /// Gets the predecessor constructs that have the same parent as this construct.
        /// </summary>
        public virtual HashSet<ISingleEntrySubGraph> SameParentPredecessors
        {
            get
            {
                //For each CFG predecessor we try to get the parent construct that has the same parent as this construct
                //If there is such construct we add it to the set
                HashSet<ISingleEntrySubGraph> logicalConstructPredecessors = new HashSet<ISingleEntrySubGraph>();
                foreach (ILogicalConstruct predecessor in predecessors)
                {
                    ILogicalConstruct predecessorWithSameParent;
                    if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(predecessor, this.Parent as ILogicalConstruct, out predecessorWithSameParent))
                    {
                        logicalConstructPredecessors.Add(predecessorWithSameParent);
                    }
                }
                return logicalConstructPredecessors;
            }
        }

        /// <summary>
        /// Gets the successor constructs that have the same parent as this construct.
        /// </summary>
		public virtual HashSet<ISingleEntrySubGraph> SameParentSuccessors
		{
			get
			{
                //For each CFG successor we try to get the parent construct that has the same parent as this construct
                //If there is such construct we add it to the set
				HashSet<ISingleEntrySubGraph> logicalConstructSuccessors = new HashSet<ISingleEntrySubGraph>();
				foreach (ILogicalConstruct successor in successors)
				{
					ILogicalConstruct successorWithSameParent;
					if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(successor, this.Parent as LogicalConstructBase, out successorWithSameParent))
					{
						logicalConstructSuccessors.Add(successorWithSameParent);
					}
				}
				return logicalConstructSuccessors;
			}
		}

        /// <summary>
        /// Gets the topmost constructs in the logical tree that are predecessors of this construct.
        /// </summary>
        public virtual HashSet<ISingleEntrySubGraph> AllPredecessors
        {
            get
            {
                HashSet<ISingleEntrySubGraph> allPredecessors = new HashSet<ISingleEntrySubGraph>();
                foreach (CFGBlockLogicalConstruct cfgPredecessor in predecessors)
                {
                    ISingleEntrySubGraph commonParent = LogicalFlowUtilities.FindFirstCommonParent(new ISingleEntrySubGraph[] { this.parent, cfgPredecessor });
                    ILogicalConstruct predecessor;
                    if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(cfgPredecessor, commonParent as ILogicalConstruct, out predecessor))
                    {
                        allPredecessors.Add(predecessor);
                    }
                }

                return allPredecessors;
            }
        }

        /// <summary>
        /// Gets the topmost constructs in the logical tree that are successors of this construct.
        /// </summary>
        public virtual HashSet<ISingleEntrySubGraph> AllSuccessors
        {
            get
            {
                HashSet<ISingleEntrySubGraph> allSuccessors = new HashSet<ISingleEntrySubGraph>();
                foreach (CFGBlockLogicalConstruct cfgSuccessor in successors)
                {
                    ISingleEntrySubGraph commonParent = LogicalFlowUtilities.FindFirstCommonParent(new ISingleEntrySubGraph[] { this.parent, cfgSuccessor });
                    ILogicalConstruct successor;
                    if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(cfgSuccessor, commonParent as ILogicalConstruct, out successor))
                    {
                        allSuccessors.Add(successor);
                    }
                }

                return allSuccessors;
            }
        }

        /// <summary>
        /// Adds the given construct as a successor of this construct. This is the only legal way to do so.
        /// </summary>
        /// <param name="successor"></param>
        public void AddToSuccessors(ILogicalConstruct successor)
        {
            //The only thing we need to know about the constructs is their entry CFG
            successors.Add(successor.FirstBlock);
        }

        /// <summary>
        /// Adds the given CFG construct as a predecessor of this construct. This is the only legal way to do so.
        /// </summary>
        /// <param name="predecessor"></param>
		public void AddToPredecessors(CFGBlockLogicalConstruct predecessor)
        {
            predecessors.Add(predecessor);
        }

        /// <summary>
        /// Gets the children of this construct.
        /// </summary>
        public virtual HashSet<ISingleEntrySubGraph> Children
        {
            get
            {
                return children;
            }
        }

        /// <summary>
        /// Gets or sets the parent of this construct.
        /// </summary>
        public ISingleEntrySubGraph Parent
        {
            get
            {
                return parent;
            }
            set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// Copies the predecessors of the entry, so that they are now predecessors of this construct.
        /// </summary>
        /// <remarks>
        /// It does not copy the predecessors that are in this construct.
        /// </remarks>
        private void CopyPredecessors()
        {
            LogicalConstructBase entry = Entry as LogicalConstructBase;
			foreach (CFGBlockLogicalConstruct predecessor in entry.predecessors)
            {
                if(!predecessor.HasForParent(this))
                {
                    this.predecessors.Add(predecessor);
                }
            }
        }

        /// <summary>
        /// Sets the parents of the constructs in the <paramref name="childrenCollection"/> to this construct.
        /// </summary>
        /// <remarks>
        /// All of the constructs in the <paramref name="childrenCollection"/> should have the same parent.
        /// If this construct is attached to the logical tree, then it should have the same parent as the nodes in the <paramref name="childrenCollection"/>.
        /// Otherwise if this construct is not yet attached, then its parent is the common parent of the given nodes.
        /// </remarks>
        /// <param name="childrenCollection"></param>
        private void RedirectParents(IEnumerable<ILogicalConstruct> childrenCollection)
        {
            //In order to change the parents, all the constructs in childrenCollection should have the same parent that is different from this construct
            ISingleEntrySubGraph commonParent = null;
            foreach (ILogicalConstruct childNode in childrenCollection)
            {
                if (childNode.Parent == this)
                {
                    continue;
                }

                //If the common parent is not yet set, we set it
                if(commonParent == null)
                {
                    commonParent = childNode.Parent;
                }
                else if (commonParent != childNode.Parent)
                {
                    //sanity check
                    throw new InvalidOperationException("The nodes in the child collection does not have the same parent");
                }

                //When we create the initial block all the cfg blocks have null parent, so commonParent will remain null
				if(commonParent != null)
				{
                    //If the commonParent is not null we remove the childNode from it's children collection
					commonParent.Children.Remove(childNode);
				}

                //Finally we set the parent of the child node to this construct and we add it to the children collection
                childNode.Parent = this;
                this.Children.Add(childNode);
            }

			if (commonParent!=null)
			{
				commonParent.Children.Add(this);
                
                //If the parent of this construct was already set and it's not the same as the common parent we throw exception, because we can add
                //a construct as a child of another construct only if they share the same parent
				if (this.parent != null && this.parent != commonParent)
				{
					throw new InvalidOperationException("The nodes you are trying to add are not from the same logical construct");
				}
				else if (this.parent == null)
				{
					this.parent = commonParent;
				}
			}
        }

        /// <summary>
        /// Redirects the entry of the parent construct.
        /// </summary>
        /// <remarks>
        /// If the entry of the parent construct is moved in this construct, we make this construct the entry of the parent. Otherwise we do nothing.
        /// </remarks>
        private void RedirectParentEntry()
        {
            if(parent.Entry.Parent != parent)
            {
                //sanity check
                if(parent.Entry.Parent != this)
                {
                    throw new InvalidOperationException("Invalid entry of parent");
                }
                else if(parent.Entry != this.Entry) //In order to make this the new entry, both constructs should have the same entry
                {
                    throw new InvalidOperationException("Invalid entry of new construct");
                }
                parent.Entry = this;
            }
        }

        /// <summary>
        /// Removes the given CFG construct from the predecessors of this construct. This is the only legal way to do so.
        /// </summary>
        /// <param name="predecessor"></param>
		public bool RemoveFromPredecessors(CFGBlockLogicalConstruct predecessor)
        {
            return predecessors.Remove(predecessor);
        }

        /// <summary>
        /// Removes the given construct from the successors of this construct. This is the only legal way to do so.
        /// </summary>
        /// <param name="successor"></param>
        public bool RemoveFromSuccessors(ILogicalConstruct successor)
        {
            return successors.Remove(successor.FirstBlock);
        }

        /// <summary>
        /// Makes this construct the parent of all the logical constructs in the specified <paramref name="body"/> collection.
        /// </summary>
        /// <remarks>
        /// Attaches this construct to the logical construct tree, if it is not already attached.
        /// </remarks>
        /// <param name="body"></param>
        protected void RedirectChildrenToNewParent(IEnumerable<ILogicalConstruct> body)
        {
            //If the children collection is empty, then we are adding nodes to a new construct.
            bool isInitialization = Children.Count == 0;

            if(successors.Count != 0)
            {
                //If the successors collection is not empty, then the current construct might have for successors some of the nodes we are adding now.
                foreach (LogicalConstructBase possibleSuccessor in body)
                {
                    RemoveFromSuccessors(possibleSuccessor);
                }
            }

            RedirectParents(body);

            if (isInitialization)
            {
                if (parent != null)
                {
                    //We might have taken the entry of the parent.
                    RedirectParentEntry();
                }

                //Since the constructs are single entry subgraphs, the only predecessors of this construct will be the predecessors of its entry.
                CopyPredecessors();
            }
            else
            {
                //Some of the added nodes can have for successor the entry of the construct.
                CleanUpPredecessors();
            }

            //If a child's successor is not in this construct, we add it as a successor.
            foreach (LogicalConstructBase child in body)
            {
				foreach (CFGBlockLogicalConstruct successor in child.successors)
                {
                    if(!successor.HasForParent(this))
                    {
                        this.successors.Add(successor);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the CFG predecessors that are inside this construct.
        /// </summary>
        private void CleanUpPredecessors()
        {
            foreach (CFGBlockLogicalConstruct cfgBlock in this.CFGBlocks)
            {
                RemoveFromPredecessors(cfgBlock);
            }
        }

        /// <summary>
        /// Determines whether the <paramref name="supposedParent"/> is parent (not necessarily direct) of this construct.
        /// </summary>
        /// <param name="supposedParent"></param>
        /// <returns></returns>
        private bool HasForParent(ILogicalConstruct supposedParent)
        {
            ILogicalConstruct construct;
            return LogicalFlowUtilities.TryGetParentConstructWithGivenParent(this, supposedParent, out construct);
        }

        /// <summary>
        /// Gets all CFG logical construct predecessors of this construct.
        /// </summary>
		public HashSet<CFGBlockLogicalConstruct> CFGPredecessors
        {
            get
            {
				HashSet<CFGBlockLogicalConstruct> cfgPredecessors = new HashSet<CFGBlockLogicalConstruct>(predecessors);
                return cfgPredecessors;
            }
        }

        /// <summary>
        /// Gets all CFG logical construct successors of this construct.
        /// </summary>
		public HashSet<CFGBlockLogicalConstruct> CFGSuccessors
        {
            get
            {
				HashSet<CFGBlockLogicalConstruct> cfgSuccessors = new HashSet<CFGBlockLogicalConstruct>(successors);
                return cfgSuccessors;
            }
        }

        /// <summary>
        /// Gets the CFG logical construct that is control flow entry of this construct.
        /// </summary>
        public virtual CFGBlockLogicalConstruct FirstBlock
        {
            get
            {
                return (Entry as ILogicalConstruct).FirstBlock;
            }
        }

        public virtual int CompareTo(ISingleEntrySubGraph other)
        {
            return this.FirstBlock.CompareTo((other as ILogicalConstruct).FirstBlock);
        }

        /// <summary>
        /// Gets all CFG block logical constructs that are contained in this logical construct.
        /// </summary>
        public virtual HashSet<CFGBlockLogicalConstruct> CFGBlocks
        {
            get
            {
                HashSet<CFGBlockLogicalConstruct> cfgBlocks = new HashSet<CFGBlockLogicalConstruct>();
                foreach (LogicalConstructBase child in Children)
                {
                    cfgBlocks.UnionWith(child.CFGBlocks);
                }

                return cfgBlocks;
            }
        }

        public virtual string ToString(LogicalFlowBuilderContext context)
        {
            return ToString(this.GetType().Name,new HashSet<CFGBlockLogicalConstruct>(),context);
        }

        /// <summary>
        /// Gets a string representation of this logical construct.
        /// </summary>
        /// <remarks>
        /// Used for testing purposes.
        /// </remarks>
        /// <param name="constructName">The name of the construct.</param>
        /// <param name="printedCFGBlocks">A set containing all of the CFG block logical constructs that are already traversed.</param>
        /// <param name="context">The current logical flow builder context.</param>
        /// <returns></returns>
        protected virtual string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedCFGBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(constructName);
            sb.AppendLine("{");

            ILogicalConstruct[] cfgBlocksArray = GetSortedArrayFromCollection(this.CFGBlocks);

            Stack<CFGBlockLogicalConstruct> printingStack = new Stack<CFGBlockLogicalConstruct>();

            for (int i = 0; i < cfgBlocksArray.Length; i++)
            {
                CFGBlockLogicalConstruct currentCFGBlock = cfgBlocksArray[i] as CFGBlockLogicalConstruct;

                //If the current node is already printed we skip it.
                if (printedCFGBlocks.Contains(currentCFGBlock))
                {
                    continue;
                }

                //We use a stack to print the constructs in preorder.
                printingStack.Push(currentCFGBlock);
                while (printingStack.Count > 0)
                {
                    CFGBlockLogicalConstruct blockToPrint = printingStack.Pop();
                    if (printedCFGBlocks.Contains(blockToPrint))
                    {
                        continue;
                    }

                    //Foreach CFG construct that is not printed we get the parent (not necessarily direct) logical construct that is child of this construct
                    //(i.e. childConstructToPrint is the child of this construct that contains the blockToPrint).
                    ILogicalConstruct childConstructToPrint;
                    if(!LogicalFlowUtilities.TryGetParentConstructWithGivenParent(blockToPrint, this, out childConstructToPrint))
                    {
                        continue;
                    }

                    string childString = ((LogicalConstructBase)childConstructToPrint).ToString(childConstructToPrint.GetType().Name, printedCFGBlocks, context);

                    //Indent each line of child's strings by one tab, so that they appear visually better
                    string[] childStringLines = childString.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in childStringLines)
                    {
                        sb.AppendLine(string.Format("\t{0}", line));
                    }

                    //We get a sorted array ot the child's same parent successors and we push their CFG logical construct entries in reverse order on the
                    //printing stack, so that the closest successor is going to be printed next.
                    ILogicalConstruct[] sameParentSuccessors = GetSortedArrayFromCollection(childConstructToPrint.SameParentSuccessors);

                    for (int j = sameParentSuccessors.Length - 1; j >= 0; j--)
                    {
                        if (!printedCFGBlocks.Contains(sameParentSuccessors[j].FirstBlock))
                        {
                            printingStack.Push(sameParentSuccessors[j].FirstBlock);
                        }
                    }
                }
            }

            string followNodeString = string.Format("\tFollowNode: {0}", NodeILOffset(context, CFGFollowNode));
            sb.AppendLine(followNodeString);
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Gets a string representation of the instruction block offset that this CFG block logical construct node represents.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="node"/> is partial CFG block logical construct then the string representing its offset is:
        /// "{offset of the instruction block}_{partial construct index}"
        /// Where the partial construct index is the index of the partial construct in the <paramref name="context"/>.CFGBlockToLogicalConstructMap.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected string NodeILOffset(LogicalFlowBuilderContext context, CFGBlockLogicalConstruct node)
        {
            if(node == null)
            {
                return "null";
            }

            //If the CFG construct is a partial block, the offset is: "blockOffset_indexOfPartialBlock"
            PartialCFGBlockLogicalConstruct partialNode = node as PartialCFGBlockLogicalConstruct;
            if (partialNode != null)
            {
                int indexOfPartialConstruct = Array.IndexOf(context.CFGBlockToLogicalConstructMap[partialNode.TheBlock], partialNode);

                if(indexOfPartialConstruct == -1)
                {
                    //sanity check
                    throw new Exception("Invalid partial block data.");
                }

                return string.Format("IL_{0}_{1}", partialNode.TheBlock.First.Offset.ToString("x4"), indexOfPartialConstruct);
            }
            else
            {
                return string.Format("IL_{0}", node.TheBlock.First.Offset.ToString("x4"));
            }
        }

        /// <summary>
        /// Gets a sorted array of logical constructs from the collection using the comparison of ISingleEntrySubgraph
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected ILogicalConstruct[] GetSortedArrayFromCollection<T>(ICollection<T> collection)
            where T : ISingleEntrySubGraph
        {
            ILogicalConstruct[] array = new ILogicalConstruct[collection.Count];
            int index = 0;
            foreach (ILogicalConstruct item in collection)
            {
                array[index++] = item;
            }

            Array.Sort<ISingleEntrySubGraph>(array);
            return array;
        }
    }
}
