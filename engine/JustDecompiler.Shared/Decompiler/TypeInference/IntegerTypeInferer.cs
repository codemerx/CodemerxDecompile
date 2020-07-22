using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    /// <summary>
    /// This class implements the algorithm described in the paper <see cref="Efficient Inference of Static Types for Java Bytecode"/> by Gagnon et al.    
    /// at chapter 6 - Integer Types. Only the first step is implemented at the moment.
    /// </summary>
    class IntegerTypeInferer : TypeInferer
    {
		public IntegerTypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression)
			: base(context, offsetToExpression)
		{ }
        /// <summary>
        /// The entry point of the class. At the end of this method all integer values should have refined types.
        /// </summary>
		internal void InferIntegerTypes(HashSet<VariableReference> resolved)
        {
            IntegerTypesHierarchyBuilder ithBuilder = new IntegerTypesHierarchyBuilder(offsetToExpression, context);
            inferenceGraph = ithBuilder.BuildHierarchy(resolved);
            try
            {
                ///Perform the steps described in <see cref="Chapter 6 Integer inference"/> 
                ///in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
                MergeConnectedComponents();
                ReplaceMultipleParentDependencies();
                ReplaceMultipleChildConstraints();
                MergeWithSingleParent();
                MergeWithSingleChild();
            }
            catch (InvalidCastException ex)
            {
                if (ex.Message != "Cannot infer types.")
                {
                    throw;
                }
            }

            return;
        }

        /// <summary>
        /// Replaces multiple parent dependencies as described in <see cref="Chapter 6 Integer inference"/>
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        private void ReplaceMultipleParentDependencies()
        {
            bool merged = false;
            do
            {
                merged = false;
                ICollection<ClassHierarchyNode> toMerge = null;
                foreach (ClassHierarchyNode node in inferenceGraph)
                {
                    bool flag = true;
                    if (!node.IsHardNode)
                    {
                        foreach (ClassHierarchyNode subtype in node.SubTypes)
                        {
                            if (!subtype.IsHardNode)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag || node.SubTypes.Count == 0)
                        {
                            continue;
                        }

                        merged = true;
                        ClassHierarchyNode type = FindLowestCommonAncestor(node.SubTypes);
                        toMerge = new ClassHierarchyNode[] { node, type };
                        break;
                    }
                }
                if (merged)
                {
                    MergeNodes(toMerge);
                }
            } while (merged);
        }

        /// <summary>
        /// Replaces multiple child constraints as described in <see cref="Chapter 6 Integer inference"/>
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        private void ReplaceMultipleChildConstraints()
        {
            bool merged = false;
            do
            {
                merged = false;
                ICollection<ClassHierarchyNode> toMerge = null;
                foreach (ClassHierarchyNode node in inferenceGraph)
                {
                    if (!node.IsHardNode)
                    {
                        bool flag = true;
                        foreach (ClassHierarchyNode canAssignTo in node.CanAssignTo)
                        {
                            if (!canAssignTo.IsHardNode)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag || node.CanAssignTo.Count == 0)
                        {
                            continue;
                        }
                        ///Now it's sure the node has only type predecessors.
                        ///Now find the smallest type it can assign to and assign it to the node.
                        merged = true;
                        ClassHierarchyNode type = FindGreatestCommonDescendant(node.CanAssignTo);
                        toMerge = new ClassHierarchyNode[] { node, type };
                        break;
                    }
                }
                if (merged)
                {
                    MergeNodes(toMerge);
                }
            } while (merged);
        }

        /// <summary>
        /// Handles all nodes that have single parent constraint as described in <see cref="Chapter 6 Integer inference"/>
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        private void MergeWithSingleParent()
        {
            bool merged = false;
            do
            {
                ICollection<ClassHierarchyNode> toMerge = null;
                foreach (ClassHierarchyNode node in inferenceGraph)
                {
                    if (!node.IsHardNode)
                    {
                        if (node.SubTypes.Count == 1 && node.CanAssignTo.Count == 0)
                        {
                            ClassHierarchyNode x = null;
                            //x = node.CanAssignTo.First() without Linq
                            foreach (ClassHierarchyNode p in node.SubTypes)
                            {
                                x = p;
                                break;
                            }
                            toMerge = new ClassHierarchyNode[] { node, x };
                            merged = true;
                            break;
                        }
                    }
                }
                if (merged)
                {
                    MergeNodes(toMerge);
                }
            } while (merged);
        }

        /// <summary>
        /// Handles all nodes that have single child constraint as described in <see cref="Chapter 6 Integer inference"/>
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        private void MergeWithSingleChild()
        {
            bool merged = false;
            do
            {
                ICollection<ClassHierarchyNode> toMerge = null;
                foreach (ClassHierarchyNode node in inferenceGraph)
                {
                    if (!node.IsHardNode)
                    {
                        if (node.CanAssignTo.Count == 1 && node.SubTypes.Count == 0)
                        {
                            ClassHierarchyNode x = null;
                            //x = node.CanAssignTo.First(); w/o Linq
                            foreach (ClassHierarchyNode p in node.CanAssignTo)
                            {
                                x = p; 
                                break;
                            }
                            toMerge = new ClassHierarchyNode[]{node,x};
                            merged = true;
                            break;
                        }
                    }
                }
                if (merged)
                {
                    MergeNodes(toMerge);
                }
            } while (merged);
        }

        /// <summary>
        /// Finds the smallest type from the given types.
        /// </summary>
        /// <param name="typeNodes">The list of type nodes.</param>
        /// <returns>Returns the smallest type.</returns>
        private ClassHierarchyNode FindGreatestCommonDescendant(ICollection<ClassHierarchyNode> typeNodes)
        {
            ClassHierarchyNode result = null;
            int minIndex = Int32.MaxValue;
            foreach (ClassHierarchyNode node in typeNodes)
            {
                int currentIndex = ExpressionTypeInferer.GetTypeIndex(node.NodeType);
                if (currentIndex < minIndex)
                {
                    minIndex = currentIndex;
                    result = node;
                }
            }
            return result;

            //Stack<ClassHierarchyNode> possibleGcdOrdered = new Stack<ClassHierarchyNode>();
            //HashSet<ClassHierarchyNode> possibleGcdSearchable = new HashSet<ClassHierarchyNode>();
            //foreach (ClassHierarchyNode node in typeNodes)
            //{
            //    if (possibleGcdOrdered.Count == 0) //first pass
            //    {
            //        ClassHierarchyNode currentSubtype = node;
            //        while (currentSubtype != null)
            //        {
            //            possibleGcdOrdered.Push(currentSubtype);
            //            possibleGcdSearchable.Add(currentSubtype);
            //            ClassHierarchyNode nextSubtype = null;
            //            foreach (ClassHierarchyNode x in currentSubtype.SubTypes)
            //            {
            //                if (x.IsHardNode)
            //                {
            //                    nextSubtype = x;
            //                    break;
            //                }
            //            }
            //            currentSubtype = nextSubtype;
            //        }
            //    }
            //    else
            //    {
            //        ClassHierarchyNode firstSubtype = node;
            //        while (!possibleGcdSearchable.Contains(firstSubtype))
            //        {
            //            //should not reach null, as Int32 is the ultimate GCD
            //            ClassHierarchyNode nextSubtype = null;
            //            foreach (ClassHierarchyNode x in firstSubtype.SubTypes)
            //            {
            //                if (x.IsHardNode)
            //                {
            //                    nextSubtype = x;
            //                    break;
            //                }
            //            }
            //            firstSubtype = nextSubtype;
            //        }

            //        while (possibleGcdOrdered.Peek() != firstSubtype)
            //        {
            //            ClassHierarchyNode removed = possibleGcdOrdered.Pop();
            //            possibleGcdSearchable.Remove(removed);
            //        }
            //    }
            //}
            //return possibleGcdOrdered.Peek();
        }

        /// <summary>
        /// As integer values can be ordered depending on their size, the graph algorithm used in TypeInferer isn't needed here.
        /// Instead, we might use the indexes to determine which is the smallest type big enough to contain every type provided in 
        /// <paramref name="typeNodes"/>.
        /// </summary>
        /// <param name="typeNodes">The list of types.</param>
        /// <returns>The common parent node.</returns>
        protected override ClassHierarchyNode FindLowestCommonAncestor(ICollection<ClassHierarchyNode> typeNodes)
        {
            ClassHierarchyNode result = null;
            int maxIndex = Int32.MinValue;
            foreach (ClassHierarchyNode node in typeNodes)
            {
                int currentIndex = ExpressionTypeInferer.GetTypeIndex(node.NodeType);
                if (currentIndex > maxIndex)
                {
                    maxIndex = currentIndex;
                    result = node;
                }
            }
            return result;
        }
    }
}
