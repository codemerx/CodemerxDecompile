using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast;
namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    /// <summary>
    /// This class is responsible for building the type hierarchy graph for IntegerTypeInference step.
    /// </summary>
    class IntegerTypesHierarchyBuilder : ClassHierarchyBuilder
    {
        private HashSet<ClassHierarchyNode> notPossibleBooleanNodes;

        public IntegerTypesHierarchyBuilder(Dictionary<int, Expression> offsetToExpression, DecompilationContext context)
            : base(offsetToExpression, context.MethodContext.ControlFlowGraph.OffsetToInstruction, context)
        {
            notPossibleBooleanNodes = new HashSet<ClassHierarchyNode>();
        }

        /// <summary>
        /// Locates the node in the graph, holding <paramref name="assignedType"/>. If no such node exists, it's created.
        /// </summary>
        /// <param name="assignedType">The type we want to get the node for.</param>
        /// <returns>Returns the graph node.</returns>
        protected override ClassHierarchyNode GetTypeNode(TypeReference assignedType)
        {
            string typeName = assignedType.FullName;
            if (!typeNameToNode.ContainsKey(typeName))
            {
                ClassHierarchyNode typeNode = new ClassHierarchyNode(assignedType);
                typeNameToNode.Add(typeName, typeNode);
            }
            return typeNameToNode[typeName];
        }

        /// <summary>
        /// Handles the assignment of phi variables.
        /// </summary>
        /// <param name="instructionOffset">The offset of the instruction, that put the value on the stack.</param>
        /// <param name="variableNode">The graph node corresponding to the phi variable.</param>
        /// <param name="nodes">The graph.</param>
        protected override void OnPhiVariableAssigned(int instructionOffset, ClassHierarchyNode variableNode)
        {
            /// This key should allways be present.
            Expression instructionExpression = offsetToExpression[instructionOffset];
            /// Determine the type of the assigned value.
            TypeReference assignedType = instructionExpression.ExpressionType;
            if (instructionExpression is LiteralExpression)
            {
                int literal = (int)((instructionExpression as LiteralExpression).Value);
                if (literal == 0 || literal == 1)
                {
                    assignedType = typeSystem.Boolean;
                }
                else if (literal <= byte.MaxValue && literal >= byte.MinValue)
                {
                    assignedType = typeSystem.Byte;
                }
                else if (literal < char.MaxValue && literal >= char.MinValue)
                {
                    /// Both char and short can be used here.
                    assignedType = typeSystem.Char;
                }
            }
            /// Add the edge in the graph.
            ClassHierarchyNode supertypeNode = GetTypeNode(assignedType);
            supertypeNode.AddSupertype(variableNode);
            resultingGraph.Add(supertypeNode);
        }

        /// <summary>
        /// Handles the usage of phi variables.
        /// </summary>
        /// <param name="instructionOffset">The offset of the instruction, that pops the phi variable from the stack.</param>
        /// <param name="record">The VariableDefineUseRecord associated with the instruction.</param>
        /// <param name="variableNode">The node in the craph corresponding to the phi variable.</param>
        /// <param name="nodes"> The graph. </param>
        protected override void OnPhiVariableUsed(int instructionOffset, ClassHierarchyNode variableNode)
        {
            Instruction instr = offsetToInstruction[instructionOffset];
            if (instr.OpCode.Code == Code.Dup || instr.OpCode.Code == Code.Pop)
            {
                ///No information regarding the type of the phi variable can be extracted from pop/dup instructions
                return;
            }
            ///Get the use type of the phi variable.
            ClassHierarchyNode typeNode = GetUseExpressionTypeNode(instr, variableNode.Variable);

            if (instr.OpCode.Code == Code.Switch)
            {
                variableNode.AddSupertype(typeNode);
                resultingGraph.Add(typeNode);
                return;
            }

            ///It is possible, that a phi variable is being used with another one. This case needs special care.
            if (typeNode.NodeType.FullName == "System.Int32" && OnlyPhiVariablesUsed(offsetToExpression[instr.Offset]))
            {
                ///Only phi variables were part of the expression.
                List<ClassHierarchyNode> phiVariableNodes = GetUsedPhiVariableNodes(instr.Offset);
                for (int i = 0; i < phiVariableNodes.Count; i++)
                {
                    for (int j = i + 1; j < phiVariableNodes.Count; j++)
                    {
                        ///Add edge between the phi variables.
                        phiVariableNodes[i].AddSupertype(phiVariableNodes[j]);
                        phiVariableNodes[j].AddSupertype(phiVariableNodes[i]);
                    }
                }

                if (IsArithmeticOperation(instr.OpCode.Code))
                {
                    for (int i = 0; i < phiVariableNodes.Count; i++)
                    {
                        notPossibleBooleanNodes.Add(phiVariableNodes[i]);

                        ClassHierarchyNode integerTypeNode = GetTypeNode(typeSystem.Int32);
                        if (!integerTypeNode.CanAssignTo.Contains(phiVariableNodes[i]))
                        {
                            integerTypeNode.CanAssignTo.Add(phiVariableNodes[i]);
                            phiVariableNodes[i].SubTypes.Add(integerTypeNode);
                        }
                    }
                }

                return;
            }

            variableNode.AddSupertype(typeNode);
            resultingGraph.Add(typeNode);
        }

        private bool IsArithmeticOperation(Code code)
        {
            return (code == Code.Add || code == Code.Sub || code == Code.Mul || code == Code.Div || code == Code.Rem
                || code == Code.Add_Ovf || code == Code.Add_Ovf_Un || code == Code.Sub_Ovf || code == Code.Sub_Ovf_Un ||
                code == Code.Rem_Un || code == Code.Mul_Ovf || code == Code.Mul_Ovf_Un || code == Code.Div_Un);
        }


        protected override void RemoveImpossibleEdges()
        {
            foreach (ClassHierarchyNode variableNode in notPossibleBooleanNodes)
            {
                RemoveBooleanAsASubtype(variableNode);
            }
        }

        private void RemoveBooleanAsASubtype(ClassHierarchyNode variableNode)
        {
            ClassHierarchyNode booleanNode = GetTypeNode(typeSystem.Boolean);
            if (variableNode.SubTypes.Contains(booleanNode))
            {
                variableNode.SubTypes.Remove(booleanNode);
                booleanNode.CanAssignTo.Remove(variableNode);
            }
        }

        /// <summary>
        /// Gets all the phi variables, used in the expression generated for the instruction with the supplied offset.
        /// </summary>
        /// <param name="offset">The offset of the instruction.</param>
        /// <returns>Returns a list of all used Phi variables.</returns>
        private List<ClassHierarchyNode> GetUsedPhiVariableNodes(int offset)
        {
            List<ClassHierarchyNode> result = new List<ClassHierarchyNode>();
            VariableDefinition assignedVariable;
            if (methodContext.StackData.InstructionOffsetToAssignedVariableMap.TryGetValue(offset, out assignedVariable) &&
                methodContext.StackData.VariableToDefineUseInfo.ContainsKey(assignedVariable))
            {
                result.Add(GetVariableNode(assignedVariable));
            }
            List<VariableDefinition> usedVariables;
            if (methodContext.StackData.InstructionOffsetToUsedStackVariablesMap.TryGetValue(offset, out usedVariables))
            {
                result.AddRange(usedVariables.Where(variable => methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variable)).Select(variable => GetVariableNode(variable)));
            }

            return result;
        }
        /// <summary>
        /// Checks if the expression was composed only by Phi variables.
        /// </summary>
        /// <param name="expression">The expression to be checked.</param>
        /// <returns>Returns true if only phi expressions were used in the expression.</returns>
        private bool OnlyPhiVariablesUsed(Expression expression)
        {
            switch (expression.CodeNodeType)
            {
                case CodeNodeType.LiteralExpression:
                    return true;
                case CodeNodeType.BinaryExpression:
                    {
                        BinaryExpression binEx = expression as BinaryExpression;
                        if (binEx.Left is VariableReferenceExpression)
                        {
                            VariableReferenceExpression leftVar = binEx.Left as VariableReferenceExpression;
                            if (IsStackVariable(leftVar.Variable))
                            {
                                return OnlyPhiVariablesUsed(binEx.Right);
                            }
                        }
                        else if (binEx.Left is VariableReferenceExpression)
                        {
                            VariableReferenceExpression rightVar = binEx.Right as VariableReferenceExpression;
                            if (IsStackVariable(rightVar.Variable))
                            {
                                return OnlyPhiVariablesUsed(binEx.Left);
                            }
                        }
                        return false;
                    }
                case CodeNodeType.VariableReferenceExpression:
                    {
                        VariableReferenceExpression varRef = expression as VariableReferenceExpression;
                        return IsStackVariable(varRef.Variable);
                    }
                case CodeNodeType.VariableDeclarationExpression:
                    {
                        VariableDeclarationExpression varRef = expression as VariableDeclarationExpression;
                        return IsStackVariable(varRef.Variable);
                    }
                default:
                    return false;
            }
        }

        private bool IsStackVariable(VariableReference varRef)
        {
            return methodContext.StackData.VariableToDefineUseInfo.ContainsKey(varRef.Resolve());
        }

        /// <summary>
        /// Checks if the type of <paramref name="variableReference"/> needs to be infered.
        /// </summary>
        /// <param name="variableReference">The variable in question.</param>
        /// <returns>Returns true, if the type of <paramref name="variableReference"/> needs to be infered.</returns>
        protected override bool ShouldConsiderVariable(VariableReference variableReference)
        {
            ///All phi variables, that were assigned Int32 at the total type inference might possibly have toghter types
            ///and thus need to be included.
            return variableReference.VariableType.FullName == "System.Int32";
        }

        /// <summary>
        /// Add all the hard nodes and their respective hierarchy. Adds nodes for bool, byte, sbyte, short, ushort, char and int32
        /// if they are not already present.
        /// </summary>
        /// <param name="hardNodes">Legacy from the parent class. Not actually needed in this method.</param>
        protected override void BuildUpHardNodesHierarchy(IEnumerable<ClassHierarchyNode> hardNodes)
        {
            ClassHierarchyNode smallerTypeNode;
            ClassHierarchyNode biggerTypeNode;
            //add edge bool  -> byte
            smallerTypeNode = GetTypeNode(typeSystem.Boolean);
            biggerTypeNode = GetTypeNode(typeSystem.Byte);
            AddEdge(biggerTypeNode, smallerTypeNode);

            //add edge byte  -> char
            smallerTypeNode = GetTypeNode(typeSystem.Byte);
            biggerTypeNode = GetTypeNode(typeSystem.Char);
            AddEdge(biggerTypeNode, smallerTypeNode);

            //add edge byte  -> short
            smallerTypeNode = GetTypeNode(typeSystem.Byte);
            biggerTypeNode = GetTypeNode(typeSystem.Int16);
            AddEdge(biggerTypeNode, smallerTypeNode);

            //add edge char  -> int
            smallerTypeNode = GetTypeNode(typeSystem.Char);
            biggerTypeNode = GetTypeNode(typeSystem.Int32);
            AddEdge(biggerTypeNode, smallerTypeNode);

            //add edge short -> int
            smallerTypeNode = GetTypeNode(typeSystem.Int16);
            biggerTypeNode = GetTypeNode(typeSystem.Int32);
            AddEdge(biggerTypeNode, smallerTypeNode);
        }

        protected override ClassHierarchyNode MergeWithVariableTypeIfNeeded(VariableReference variable, ClassHierarchyNode variableNode)
        {
            /// The variable node should not be merged with its type in integer inference, because
            /// that will efectively assume all phi variables are integers.
            return variableNode;
        }

        /// <summary>
        /// Adds an edge from <paramref name="smallerType"/> to <paramref name="biggerType"/>.
        /// </summary>
        /// <param name="biggerTypeNode">The type the edge points to.</param>
        /// <param name="smallerTypeNode">The type the edge comes from.</param>
        private void AddEdge(ClassHierarchyNode biggerTypeNode, ClassHierarchyNode smallerTypeNode)
        {
            // smallerTypeNode can assign to BiggerTypeNode
            smallerTypeNode.AddSupertype(biggerTypeNode);
            resultingGraph.Add(smallerTypeNode);
            resultingGraph.Add(biggerTypeNode);
        }
    }
}