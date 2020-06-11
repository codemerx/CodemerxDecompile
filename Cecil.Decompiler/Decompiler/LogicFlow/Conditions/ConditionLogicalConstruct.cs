using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
    internal class ConditionLogicalConstruct : LogicalConstructBase
    {
        private ConditionLogicalConstruct(CFGBlockLogicalConstruct cfgConditionBlock)
        {
            Entry = cfgConditionBlock;

            SetTrueAndFalseSuccessors(cfgConditionBlock);

            ConditionExpression = cfgConditionBlock.LogicalConstructExpressions[0];

            RedirectChildrenToNewParent(new ILogicalConstruct[] { cfgConditionBlock });

            AddTrueFalseSuccessors();

			LogicalContainer = null;
        }

        public ConditionLogicalConstruct(ConditionLogicalConstruct entry, ConditionLogicalConstruct lastNode, HashSet<ConditionLogicalConstruct> body,
            Expression conditionExpression)
        {
            Entry = entry.FirstBlock;

            TrueCFGSuccessor = lastNode.TrueCFGSuccessor;
            FalseCFGSuccessor = lastNode.FalseCFGSuccessor;

            ConditionExpression = conditionExpression;

            RedirectChildrenToNewParent(RestoreOriginalCFGNodes(body));

            AddTrueFalseSuccessors();

			LogicalContainer = null;
        }

        /// <summary>
        /// Gets the CFG entry of the true successor.
        /// </summary>
		public CFGBlockLogicalConstruct TrueCFGSuccessor { get; private set; }

        /// <summary>
        /// Gets the CFG entry of the false successor.
        /// </summary>
		public CFGBlockLogicalConstruct FalseCFGSuccessor { get; private set; }

        /// <summary>
        /// Gets the true successor (same parent) of the condition.
        /// </summary>
		public ILogicalConstruct TrueSuccessor
		{
			get
			{
				ILogicalConstruct trueSuccessorConstruct;
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(TrueCFGSuccessor, parent as ILogicalConstruct, out trueSuccessorConstruct))
				{
					return trueSuccessorConstruct;
				}

				return null;
			}
		}

        /// <summary>
        /// Gets the false successor (same parent) of the condition.
        /// </summary>
		public ILogicalConstruct FalseSuccessor
		{
			get
			{
				ILogicalConstruct falseSuccessorConstruct;
				if (LogicalFlowUtilities.TryGetParentConstructWithGivenParent(FalseCFGSuccessor, parent as ILogicalConstruct, out falseSuccessorConstruct))
				{
					return falseSuccessorConstruct;
				}

				return null;
			}
		}


        /// <summary>
        /// Gets the condition expression.
        /// </summary>
        public Expression ConditionExpression { get; private set; }

		/// <summary>
		/// Will return the IfLogicalConstruct or LoopLogicalConstruct this is a condition to.
		/// Returns null if this is a standalone condition.
		/// </summary>
		public ILogicalConstruct LogicalContainer { get; set; }

        /// <summary>
        /// Removes the condition logical constructs from the logical tree, returning their CFG children in the tree. Done to reduce the depth of the tree
        /// </summary>
        /// <param name="body"></param>
        /// <returns>The cfg blocks that were in the given condition constructs.</returns>
        private HashSet<ILogicalConstruct> RestoreOriginalCFGNodes(HashSet<ConditionLogicalConstruct> body)
        {
            HashSet<ILogicalConstruct> newConditionBody = new HashSet<ILogicalConstruct>();

            foreach (ConditionLogicalConstruct conditionConstruct in body)
            {
                if (conditionConstruct.Parent.Entry == conditionConstruct)
                {
                    //Fix the entry if needed
                    conditionConstruct.Parent.Entry = conditionConstruct.FirstBlock;
                }

                //We know that each condition construct will have only CFG constructs for children.
                foreach (CFGBlockLogicalConstruct cfgChild in conditionConstruct.Children)
                {
                    //We set the parent of the CFG childs to the parent of the conditionConstruct since this will be the parent of the new construct.
                    cfgChild.Parent = conditionConstruct.Parent;
                    //Remove the conditionConstruct from the tree.
                    cfgChild.Parent.Children.Remove(conditionConstruct);
                    newConditionBody.Add(cfgChild);
                    //No need to add the CFG constructs to the children of the parent since the RedirectChildrenToNewParent will remove them anyways.
                }
            }

            return newConditionBody;
        }

        /// <summary>
        /// Adds the true and false successors as successors to this construct.
        /// </summary>
        private void AddTrueFalseSuccessors()
        {
            //This is done in case that one of the successors is the entry of the condition, in which case the RedirectChildrenToNewParent method will not add
            //the entry as a successor.
            
            this.ManageSuccessorInCaseOfLoop(this.TrueCFGSuccessor);
            this.ManageSuccessorInCaseOfLoop(this.FalseCFGSuccessor);
        }

        /// <summary>
        /// If the given successor is the entry of the condition we update the predecessor and successor collections of the condition.
        /// </summary>
        /// <param name="cfgSuccessor"></param>
        private void ManageSuccessorInCaseOfLoop(CFGBlockLogicalConstruct cfgSuccessor)
        {
            if(this.FirstBlock == cfgSuccessor)
            {
                this.AddToSuccessors(cfgSuccessor);
                foreach (CFGBlockLogicalConstruct cfgChild in this.Children)
                {
                    if(cfgChild.CFGSuccessors.Contains(cfgSuccessor))
                    {
                        this.AddToPredecessors(cfgChild);
                    }
                }
            }
        }

        /// <summary>
        /// Negates the expression of the construct and switches the true and false successors.
        /// </summary>
        public void Negate(TypeSystem typeSystem)
        {
            CFGBlockLogicalConstruct successorHolder = TrueCFGSuccessor;
            TrueCFGSuccessor = FalseCFGSuccessor;
            FalseCFGSuccessor = successorHolder;

            ConditionExpression = Negator.Negate(ConditionExpression, typeSystem);
        }

        /// <summary>
        /// Sets the true and false CFG successors of the construct.
        /// </summary>
        /// <param name="cfgConditionBlock"></param>
        private void SetTrueAndFalseSuccessors(CFGBlockLogicalConstruct cfgConditionBlock)
        {
            InstructionBlock block = cfgConditionBlock.TheBlock;

            //The successor at the last index is the near successor (by design). The ExpressionDecompilerStep ensures that the far successor will be
            //the true successor and the near successor will be the false successor.
            InstructionBlock trueSuccessorBlock = block.Successors[0]; //The far successor
            InstructionBlock falseSuccessorBlock = block.Successors[1]; //The near successor

            foreach (CFGBlockLogicalConstruct cfgSuccessor in cfgConditionBlock.CFGSuccessors)
            {
                if(cfgSuccessor.TheBlock == trueSuccessorBlock)
                {
                    this.TrueCFGSuccessor = cfgSuccessor;
                }
                if(cfgSuccessor.TheBlock == falseSuccessorBlock)
                {
                    this.FalseCFGSuccessor = cfgSuccessor;
                }
            }
        }

        /// <summary>
        /// Creates a condition construct from the specified CFG construct and inserts it to the logical tree.
        /// </summary>
        /// <param name="cfgConditionBlock"></param>
        /// <returns>The created condition construct.</returns>
        public static ConditionLogicalConstruct GroupInSimpleConditionConstruct(CFGBlockLogicalConstruct cfgConditionBlock)
        {
            return new ConditionLogicalConstruct(cfgConditionBlock);
        }

        protected override string ToString(string constructName, HashSet<CFGBlockLogicalConstruct> printedBlocks, LogicalFlowBuilderContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ConditionLogicalConstruct");
            sb.AppendLine("{");
            sb.Append(string.Format("\t{0}: ", NodeILOffset(context, this.FirstBlock)));
            sb.AppendLine(ConditionExpression.ToCodeString());
            sb.AppendLine(string.Format("\tTrueSuccessorBlock: {0}", NodeILOffset(context, TrueCFGSuccessor)));
            sb.AppendLine(string.Format("\tFalseSuccessorBlock: {0}", NodeILOffset(context, FalseCFGSuccessor)));

            string followNodeString = string.Format("\tFollowNode: {0}", NodeILOffset(context, CFGFollowNode));
            sb.AppendLine(followNodeString);
            sb.AppendLine("}");
            printedBlocks.UnionWith(this.CFGBlocks);
            return sb.ToString();
        }
    }
}
