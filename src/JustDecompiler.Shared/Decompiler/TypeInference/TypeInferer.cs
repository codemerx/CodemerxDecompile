using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    /// <summary>
    /// This class is responsible for the inference of the types of all the Phi Variables. It implements the algorithm
    /// described in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>(see DecompilationPapers).
    /// As this paper is written for Java bytecode, some changes are made in this implementation, since MSIL has more metadata 
    /// regarding types.
    /// </summary>
    class TypeInferer
    {
        protected ICollection<ClassHierarchyNode> inferenceGraph;
        protected readonly DecompilationContext context;
        protected readonly Dictionary<int, Expression> offsetToExpression;

        public TypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression)
        {
            this.context = context;
            this.offsetToExpression = offsetToExpression;

        }

        /// <summary>
        /// Adds casts where the type inference resolved too general type. Only downcasts should be introduced.
        /// </summary>
        private void AddCasts()
        {
            StackUsageData stackData = context.MethodContext.StackData;
            foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> pair in stackData.VariableToDefineUseInfo)
            {
                foreach (int usageOffset in pair.Value.UsedAt)
                {
                    Expression usingExpression;
                    if (offsetToExpression.TryGetValue(usageOffset, out usingExpression))
                    {
                        AddCastIfNeeded(usingExpression, pair.Key);
                    }
                }

                foreach (int defineOffset in pair.Value.DefinedAt)
                {
                    Expression value = offsetToExpression[defineOffset];
                    InstructionBlock definingBlock = GetInstructionBlock(defineOffset);
                    IList<Expression> expressionsList = context.MethodContext.Expressions.BlockExpressions[definingBlock.First.Offset];

                    FixAssignmentInList(expressionsList, value);
                }
            }
        }

        private void FixAssignmentInList(IList<Expression> expressionList, Expression value)
        {
            foreach (Expression expression in expressionList)
            {
                if (expression.CodeNodeType == CodeNodeType.BinaryExpression)
                {
                    BinaryExpression binaryExpression = expression as BinaryExpression;
                    if (binaryExpression.IsAssignmentExpression && binaryExpression.Right == value)
                    {
                        AddAssignmentCastIfNeeded(binaryExpression);
                        return;
                    }
                }
            }
        }

        private InstructionBlock GetInstructionBlock(int instructionOffset)
        {
            Instruction instruction = context.MethodContext.ControlFlowGraph.OffsetToInstruction[instructionOffset];
            do
            {
                InstructionBlock block;
                if (context.MethodContext.ControlFlowGraph.InstructionToBlockMapping.TryGetValue(instruction.Offset, out block))
                {
                    return block;
                }
                instruction = instruction.Previous;
            } while (true);
        }

        private Expression AddAssignmentCastIfNeeded(Expression expr)
        {
			var currentTypeSystem = context.MethodContext.Method.Module.TypeSystem;
            if (expr is BinaryExpression && (expr as BinaryExpression).IsAssignmentExpression)
            {
                BinaryExpression assignment = expr as BinaryExpression;
                if (assignment.Left.ExpressionType != assignment.Right.ExpressionType &&
                    assignment.Left.ExpressionType.IsPrimitive && assignment.Right.ExpressionType.IsPrimitive &&
                    !IsSubtype(assignment.Right.ExpressionType, assignment.Left.ExpressionType)
                    && assignment.Right.CodeNodeType != CodeNodeType.LiteralExpression) // literal expressions are fixed at a later stage
                {
                    // might need cast
					if (assignment.Right.CodeNodeType == CodeNodeType.ExplicitCastExpression && assignment.Right.ExpressionType.FullName == currentTypeSystem.UInt16.FullName &&
						assignment.Left.ExpressionType.FullName == currentTypeSystem.Char.FullName)
					{
						((ExplicitCastExpression)assignment.Right).TargetType = currentTypeSystem.Char;
						return expr;
					}
                    assignment.Right = new ExplicitCastExpression(assignment.Right, assignment.Left.ExpressionType, null);
                }
            }
            return expr;
        }

        /// <summary>
        /// Determines if the use of <paramref name="variable"/> in <paramref name="useExpression"/> requires a cast.
        /// </summary>
        /// <param name="useExpression">The expression being checked.</param>
        /// <param name="variable">The variable that might need to be casted.</param>
        private void AddCastIfNeeded(Expression useExpression, VariableReference variable)
        {
            switch (useExpression.CodeNodeType)
            {
                case CodeNodeType.MethodInvocationExpression:
                    MethodInvocationExpression miEx = useExpression as MethodInvocationExpression;
                    Expression argument = miEx.Arguments.FirstOrDefault(x => x.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                                                                                    (x as VariableReferenceExpression).Variable == variable);
                    if (argument != null)
                    {
                        ///The variable is passed as argument to the method.
                        int argumentIndex = miEx.Arguments.IndexOf(argument);
                        TypeReference argumentType = miEx.MethodExpression.Method.Parameters[argumentIndex].ResolveParameterType(miEx.MethodExpression.Method);
                        if (!IsSubtype(argumentType, variable.VariableType))
                        {
                            if (argumentType.IsPrimitive && variable.VariableType.IsPrimitive)
                            {
                                ///Integer values are not in inheritance relations. Some of them, however, can be expanded to bigger types 
                                ///automatically, without the addition of a cast, i.e. Byte variable can be passed as int parameter without the
                                ///need to include a cast.
                                TypeReference containingType = ExpressionTypeInferer.GetContainingType(argumentType.Resolve(), variable.VariableType.Resolve());
                                if (containingType.FullName == argumentType.FullName)
                                {
                                    ///Then the type of the argument contains the type of the variable, thus no cast is needed.
                                    return;
                                }
                            }

                            ///Then a cast is needed.
                            miEx.Arguments[argumentIndex] = new ExplicitCastExpression(argument, argumentType, null);
                            ///This should be enough to update the expression everywhere it is seen.
                        }
                    }
                    else
                    {
                        /// Then the variable is the object from which the method is called
                        /// variable.SomeMethod(...);
                        Expression target = miEx.MethodExpression.Target;
                        if (target.CodeNodeType == CodeNodeType.VariableReferenceExpression && (target as VariableReferenceExpression).Variable == variable)
                        {
                            TypeReference targetType = miEx.MethodExpression.Method.DeclaringType;
                            if (!IsSubtype(targetType, variable.VariableType))
                            {
                                miEx.MethodExpression.Target = new ExplicitCastExpression(target, targetType, null);
                            }
                        }
                        else
                        {
                            ///This should not be reachable, but anyway.
                            AddCastIfNeeded(target, variable);
                        }

                    }
                    break;
                case CodeNodeType.BinaryExpression:
                    BinaryExpression binEx = useExpression as BinaryExpression;
                    if (binEx.Operator == BinaryOperator.Assign)
                    {
                        if (binEx.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                            (binEx.Right as VariableReferenceExpression).Variable == variable)
                        {
                            TypeReference assignedAs = binEx.Left.ExpressionType;
                            ///binex.Right should be VariableReferenceExpression to 'variable'.
                            if (!IsSubtype(assignedAs, variable.VariableType))
                            {
                                binEx.Right = new ExplicitCastExpression(binEx.Right, assignedAs, null);
                            }
                        }
                    }
                    break;
                //default:
                //throw new NotSupportedException("Not supported cast expression.");
            }
        }

        /// <summary>
        /// Checks if <paramref name="supposedSubType"/> is subtype of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The supposed parent type.</param>
        /// <param name="supposedSubType">The supposed inheriting type.</param>
        /// <returns>Returns True if <paramref name="supposedSubType"/> is subtype of <paramref name="type"/>.</returns>
        private bool IsSubtype(TypeReference type, TypeReference supposedSubType)
        {
            type = RemoveModifiers(type);
            supposedSubType = RemoveModifiers(supposedSubType);

            if (supposedSubType.GetFriendlyFullName(null) == type.GetFriendlyFullName(null) || type.FullName == "System.Object")
            {
                ///The types are the same, or the check is if a type inherits Object
                return true;
            }
            if (IsArrayAssignable(type, supposedSubType))
            {
                return true;
            }
            TypeDefinition supposedSubTypeDef = supposedSubType.Resolve();
            if (supposedSubTypeDef == null)
            {
                //happens with generics only;
                return true;
            }
            if (type is GenericInstanceType)
            {
                type = type.GetElementType();
            }
            while (supposedSubTypeDef != null)
            {
                if (TypeNamesComparer.AreEqual(type, supposedSubTypeDef))
                {
                    ///Must be here, since mono fails on resolving types sometimes.
                    return true;
                }
                foreach (TypeReference @interface in supposedSubTypeDef.Interfaces)
                {
                    if (TypeNamesComparer.AreEqual(type, @interface))
                    {
                        return true;
                    }
                }

                if (supposedSubTypeDef.BaseType == null)
                {
                    ///We are at the root of the class hierarcy, and no match was made.
                    return false;
                }
                supposedSubTypeDef = supposedSubTypeDef.BaseType.Resolve();
            }
            return false;
        }

        /// <summary>
        /// Returns the <paramref name="type"/> without the IL modifiers.
        /// </summary>
        /// <param name="type">TypeReference to be cleaned.</param>
        /// <returns>Returns the <paramref name="type"/> without the IL modifiers.</returns>
        private TypeReference RemoveModifiers(TypeReference type)
        {
            if (type is OptionalModifierType)
            {
                return (type as OptionalModifierType).ElementType;
            }
            if (type is RequiredModifierType)
            {
                return (type as RequiredModifierType).ElementType;
            }
            return type;
        }

        /// <summary>
        /// Checks array assignability between two types.See <see cref= "Chapter 5 - Array Constraints" /> 
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>
        /// for more information.
        /// </summary>
        /// <param name="type">The supposed parent type.</param>
        /// <param name="supposedSubType">The supposed subtype.</param>
        /// <returns>Returns true if <paramref name="supposedSubType"/> can be assigned to <paramref name="type"/>.</returns>
        private bool IsArrayAssignable(TypeReference type, TypeReference supposedSubType)
        {
            if (!(type is ArrayType) || !(supposedSubType is ArrayType))
            {
                return false;
            }
            ArrayType parentType = type as ArrayType;
            ArrayType subType = supposedSubType as ArrayType;
            if (!IsSubtype(parentType.ElementType, subType.ElementType))
            {
                return false;
            }
            return parentType.Dimensions.Count == subType.Dimensions.Count;
        }

        /// <summary>
        /// The entry point of the class. After this method ends, all variables will be typed.
        /// </summary>
        public void InferTypes()
        {
            ///This method realises the first step of the 3-stage algorithm described in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf" />.
            ///All variables that are left without type after it are given type "System.Object" and casts are introduced as needed.

            GreedyTypeInferer gti = new GreedyTypeInferer(context, offsetToExpression);
            var resolvedVariables = gti.InferTypes();

            ClassHierarchyBuilder chb = new ClassHierarchyBuilder(offsetToExpression, context.MethodContext.ControlFlowGraph.OffsetToInstruction, context);
            this.inferenceGraph = chb.BuildHierarchy(resolvedVariables);

            MergeConnectedComponents();
            RemoveTransitiveEdges();
            ProcessSingleConstraints();

            ClassHierarchyNode objectNode = null;
            foreach (ClassHierarchyNode node in inferenceGraph)
            {
                if (node.IsHardNode && node.NodeType.FullName == "System.Object")
                {
                    objectNode = node;
                    break;
                }
            }
            List<ClassHierarchyNode> allSoftNodes = new List<ClassHierarchyNode>();
            allSoftNodes.Add(objectNode);
            foreach (ClassHierarchyNode node in inferenceGraph)
            {
                if (!node.IsHardNode)
                {
                    allSoftNodes.Add(node);
                }
            }
            MergeNodes(allSoftNodes);

            InferIntegerTypes(resolvedVariables);

            this.AddCasts();
        }

        /// <summary>
        /// All primitive types of size less than 4 bytes are represented as 32-bit integers on the evaluation stack. Thus, for infering the types of these 
        /// variables, a special step is needed. For more information on the logics here see <see cref="Chapter 6 - Integer Types"/> 
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>
        /// </summary>
        private void InferIntegerTypes(HashSet<VariableReference> resolvedVariables)
        {
            IntegerTypeInferer iti = new IntegerTypeInferer(context, offsetToExpression);
            iti.InferIntegerTypes(resolvedVariables);
        }

        /// <summary>
        /// Processes single constraints as described in <see cref="Chapter 4.2 Stage 1" in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>./>
        /// </summary>
        private void ProcessSingleConstraints()
        {
            /*
            * taken from "Efficent Inference of Static Types for Java Bytecode" by Gagnon et al, 
            * 1. Merge single child constraints x <- y where x is soft node and y is any node.
            * 2. Merge with LCA, when x is soft node and it contains only Hard nodes as children.
            * 3. Merge x and y when x is single parent to y and x and y are soft nodes.
            * 4. Merge x and y when x is soft and is single parent to any node y.
            */
            bool merged = true;
            while (merged)
            {
                merged = false;
                MergeSingleChildConstraints();

                if (MergeWithLowestCommonAncestor())
                {
                    merged = true;
                    continue;
                }
                if (MergeSingleSoftParent())
                {
                    merged = true;
                    continue;
                }
                if (MergeAnySingleParent())
                {
                    merged = true;
                }
            }
        }

        /// <summary>
        /// Merges single soft parent constraints as described in <see cref="Single Constraints (page 11)"/> in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        /// <returns>Returns true if a merge was made.</returns>
        private bool MergeSingleSoftParent()
        {
            return MergeSingleParent(x => !x.IsHardNode);
        }

        /// <summary>
        /// Merges Single Child constraints as described in <see cref="Single Constraints (page 11)"/> in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        private void MergeSingleChildConstraints()
        {
            bool changed = true;
            while (changed)
            {
                ClassHierarchyNode[] toMerge = null;
                changed = false;
                foreach (ClassHierarchyNode node in inferenceGraph)
                {
                    if (node.SubTypes.Count == 1 && !node.IsHardNode)
                    {
                        ClassHierarchyNode childNode = node.SubTypes.First();
                        toMerge = new ClassHierarchyNode[] { node, childNode };
                        changed = true;
                        break;
                    }
                }
                if (changed)
                {
                    MergeNodes(toMerge);
                }
            }
        }

        /// <summary>
        /// Merges nodes with LowestCommonAncestor constraints as described in <see cref="Single Constraints (page 11)"/> 
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        /// <returns>Returns true if a merge was made.</returns>
        private bool MergeWithLowestCommonAncestor()
        {
            bool shouldMerge = false;
            ClassHierarchyNode[] toMerge = null;
            foreach (ClassHierarchyNode node in inferenceGraph)
            {
                if (node.IsHardNode)
                {
                    continue;
                }
                shouldMerge = true;
                HashSet<ClassHierarchyNode> classNodes = new HashSet<ClassHierarchyNode>();
                foreach (ClassHierarchyNode childNode in node.SubTypes)
                {
                    ///Only class nodes should be able to assign to the type of the current node.
                    if (!childNode.IsClassNode)
                    {
                        shouldMerge = false;
                        break;
                    }
                    classNodes.Add(childNode);
                }
                if (shouldMerge)
                {
                    ClassHierarchyNode lcaNode = FindLowestCommonAncestor(classNodes);
                    if (lcaNode == null || lcaNode == node)
                    {
                        shouldMerge = false;
                        continue;
                    }
                    toMerge = new ClassHierarchyNode[] { lcaNode, node };
                    break;
                }
            }
            if (shouldMerge)
            {
                MergeNodes(toMerge);
            }
            return shouldMerge;
        }

        /// <summary>
        /// Finds the lowest common antcestor from the type nodes.
        /// </summary>
        /// <param name="typeNodes">The collection of type nodes.</param>
        /// <returns>The lowest type, that is parent type for all types provided in <paramref name="typeNodes"/>.</returns>
        protected virtual ClassHierarchyNode FindLowestCommonAncestor(ICollection<ClassHierarchyNode> typeNodes)
        {
            int count = 0;
            ClassHierarchyNode lastNode = null;
            foreach (ClassHierarchyNode node in typeNodes)
            {
                count++;
                lastNode = node;
            }

            if (count == 1)
            {
                return lastNode;
            }

            Queue<ClassHierarchyNode> possibleLcaOrdered = new Queue<ClassHierarchyNode>();
            HashSet<ClassHierarchyNode> possibleLcaSearchable = new HashSet<ClassHierarchyNode>();
            foreach (ClassHierarchyNode node in typeNodes)
            {
                if (possibleLcaOrdered.Count == 0) //first pass
                {
                    ClassHierarchyNode currentSuperclass = node;
                    while (currentSuperclass != null)
                    {
                        possibleLcaOrdered.Enqueue(currentSuperclass);
                        possibleLcaSearchable.Add(currentSuperclass);
                        ClassHierarchyNode nextSuperclass = null;
                        foreach (ClassHierarchyNode x in currentSuperclass.CanAssignTo)
                        {
                            if (x.IsHardNode)
                            {
                                nextSuperclass = x;
                                break;
                            }
                        }
                        currentSuperclass = nextSuperclass;
                    }
                }
                else
                {
                    ClassHierarchyNode firstSuperclass = node;
                    while (!possibleLcaSearchable.Contains(firstSuperclass))
                    {
                        if (node.CanAssignTo.Count(x => x.IsHardNode) > 1)
                        {
                            return null;
                        }
                        //should not reach null, as Object is the ultimate LCA
                        firstSuperclass = firstSuperclass.CanAssignTo.FirstOrDefault(x => x.IsHardNode);
                    }

                    while (possibleLcaOrdered.Peek() != firstSuperclass)
                    {
                        ClassHierarchyNode removed = possibleLcaOrdered.Dequeue();
                        possibleLcaSearchable.Remove(removed);
                    }
                }
            }
            return possibleLcaOrdered.Peek();
        }

        /// <summary>
        /// Merges anu single parent constraints as described in <see cref="Single Constraints (page 11)"/> 
        /// in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        /// <returns>Returns true if a merge was made.</returns>
        private bool MergeAnySingleParent()
        {
            return MergeSingleParent(x => true);
        }

        /// <summary>
        /// Realises the logic for merging with single parent.
        /// </summary>
        /// <param name="chooseParent">Predicate that determines if the parent node is legal to merge.</param>
        /// <returns>Returns true if there was a merge.</returns>
        private bool MergeSingleParent(Func<ClassHierarchyNode, bool> chooseParentPred)
        {
            bool result = false;
            ClassHierarchyNode[] toMerge = null;
            foreach (ClassHierarchyNode childNode in inferenceGraph)
            {
                if (childNode.IsHardNode)
                {
                    continue;
                }
                if (childNode.SubTypes.Count == 1)
                {
                    ClassHierarchyNode parentNode = childNode.SubTypes.First();
                    if (chooseParentPred(parentNode))
                    {
                        result = true;
                        toMerge = new ClassHierarchyNode[] { childNode, parentNode };
                        break;
                    }
                }
            }
            if (result)
            {
                MergeNodes(toMerge);
            }
            return result;
        }

        #region Removal of transitive edges

        /// <summary>
        /// Performs the second part of the algorithm - The removal of transitive constraints. For more information see
        /// <see cref="4.2 Stage 1"/> in <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        private void RemoveTransitiveEdges()
        {
            //Build the transitivity clousure matrix.
            Dictionary<ClassHierarchyNode, int> nodeToIndex = GenerateNodeToIndex();
            bool[,] matrix = GeenrateAdjacencyMatrix(nodeToIndex);
            WarsawTransitiveClosure(matrix);

            ///Removal of transitive edges
            ///At this point no loops in the graph should be present, as all connected components have already been merged
            foreach (ClassHierarchyNode x in inferenceGraph)
            {
                foreach (ClassHierarchyNode y in inferenceGraph)
                {
                    foreach (ClassHierarchyNode z in inferenceGraph)
                    {
                        int xIndex = nodeToIndex[x];
                        int yIndex = nodeToIndex[y];
                        int zIndex = nodeToIndex[z];

                        if (matrix[xIndex, yIndex] && matrix[yIndex, zIndex])
                        {
                            if (x.IsHardNode && z.IsHardNode)
                            {
                                ///Transitive constraints between hard nodes should be left, since this is part of the type hierarchy
                                continue;
                            }
                            if (x.SubTypes.Contains(z))
                            {
                                RemoveSubtype(x, z);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes <paramref name="subType"/> from <paramref name="superType"/>'s collection of nodes, <paramref name="superType"/> can be assigned from.
        /// Also updates the collection of nodes <paramref name="subType"/> can assign to.
        /// </summary>
        /// <param name="superType"></param>
        /// <param name="subType"></param>
        private void RemoveSubtype(ClassHierarchyNode superType, ClassHierarchyNode subType)
        {
            if (!superType.SubTypes.Contains(subType))
            {
                throw new ArgumentOutOfRangeException(string.Format("No such relation between {0} and {1}.", this, subType));
            }
            superType.SubTypes.Remove(subType);
            subType.CanAssignTo.Remove(superType);
        }

        /// <summary>
        /// Populates the <paramref name="matrix"/> supplied, so that each cell matrix[a,b] contains "true" if there is a path from a to b.
        /// </summary>
        /// <param name="matrix">The starting matrix. It should contain 'trur' where there are edges in the graph.</param>
        private void WarsawTransitiveClosure(bool[,] matrix)
        {
            int count = matrix.GetLength(0);
            ///Warsaw's algorithm for finding transitive closure of a graph.
            for (int k = 0; k < count; k++)
            {
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        matrix[i, j] = matrix[i, j] || (matrix[i, k] && matrix[k, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Generates a matrix representation of the TypeInference graph. result[a,b] == true when there is edge from a to b in the original graph.
        /// </summary>
        /// <param name="nodeToIndex">The dictionary holding the relation between ClassHierarchyNode and its integer index.</param>
        /// <returns>Returns the generated matrix.</returns>
        private bool[,] GeenrateAdjacencyMatrix(Dictionary<ClassHierarchyNode, int> nodeToIndex)
        {
            int numberOfNodes = inferenceGraph.Count;
            bool[,] result = new bool[numberOfNodes, numberOfNodes];

            foreach (ClassHierarchyNode node in inferenceGraph)
            {
                int row = nodeToIndex[node];
                foreach (ClassHierarchyNode successor in node.CanAssignTo)
                {
                    int col = nodeToIndex[successor];
                    result[row, col] = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Processes all nodes in the TpeInference graph and creates a map beween the node and an integer value. This will easen the next steps.
        /// </summary>
        private Dictionary<ClassHierarchyNode, int> GenerateNodeToIndex()
        {
            int index = 0;
            Dictionary<ClassHierarchyNode, int> nodeToIndex = new Dictionary<ClassHierarchyNode, int>();
            foreach (ClassHierarchyNode node in inferenceGraph)
            {
                nodeToIndex.Add(node, index);
                index++;
            }
            return nodeToIndex;
        }

        #endregion

        /// <summary>
        /// Performs the first step of the algorithm - Merging the connected components in the TypeInference graph.
        /// For more details, see <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        protected void MergeConnectedComponents()
        {
            ConnectedComponentsFinder finder = new ConnectedComponentsFinder(inferenceGraph);
            IEnumerable<ICollection<ClassHierarchyNode>> components = finder.GetConnectedComponents();

            foreach (ICollection<ClassHierarchyNode> component in components)
            {
                ///Merge component.
                MergeNodes(component);
            }
        }

        /// <summary>
        /// Merges the nodes in <paramref name="nodeCollection"/> in one node and attaches the result to the graph.
        /// </summary>
        /// <param name="nodeCollection">The collection of nodes to be merged.</param>
        protected void MergeNodes(ICollection<ClassHierarchyNode> nodeCollection)
        {
            if (nodeCollection.Count <= 1)
            {
                return;
            }

            ///Merge component.
            ClassHierarchyNode mergedNode = new ClassHierarchyNode(nodeCollection);

            ///Attach the merged node.
            foreach (ClassHierarchyNode oldNode in mergedNode.ContainedNodes)
            {
                inferenceGraph.Remove(oldNode);
            }
            inferenceGraph.Add(mergedNode);
        }
    }
}