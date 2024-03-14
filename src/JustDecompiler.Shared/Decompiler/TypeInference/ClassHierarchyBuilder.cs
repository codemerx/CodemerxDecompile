using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast;
using System;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    /// <summary>
    /// Builds the graph representing the type hirarchy for TypeInferer.
    /// </summary>
    class ClassHierarchyBuilder
    {
        /// <summary>
        /// For more information, see <see cref="Efficient Inference of Static Types for Java Bytecode.pdf"/>.
        /// </summary>
        protected readonly Dictionary<string, ClassHierarchyNode> typeNameToNode;
        protected readonly Dictionary<int, Expression> offsetToExpression;
        protected readonly Dictionary<string, ClassHierarchyNode> variableNameToNode;
        protected readonly Dictionary<int, Instruction> offsetToInstruction;
        protected readonly HashSet<ClassHierarchyNode> resultingGraph;
        protected readonly MethodSpecificContext methodContext;
        protected readonly TypeSystem typeSystem;

        public ClassHierarchyBuilder(Dictionary<int, Expression> offsetToExpression, Dictionary<int, Instruction> offsetToInstruction, DecompilationContext context)
        {
            this.typeNameToNode = new Dictionary<string, ClassHierarchyNode>();
            this.variableNameToNode = new Dictionary<string, ClassHierarchyNode>();
            this.resultingGraph = new HashSet<ClassHierarchyNode>();
            this.offsetToExpression = offsetToExpression;
            this.offsetToInstruction = offsetToInstruction;
            this.methodContext = context.MethodContext;
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
        }

        /// <summary>
        /// The entry point of the class.
        /// </summary>
        /// <returns>Returns a collection of all nodes in the built graph.</returns>
        internal ICollection<ClassHierarchyNode> BuildHierarchy(HashSet<VariableReference> resolvedVariables)
        {
            StackUsageData stackData = methodContext.StackData;
            foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> pair in stackData.VariableToDefineUseInfo)
            {
                VariableDefinition variableDef = pair.Key;
                if (resolvedVariables.Contains(variableDef))
                {
                    continue;
                }
                ///As this method is shared with IntegerTypesHierarchyByilder here is where the check if the phi variable should be used comes in place.
                ///For the purposes of TypeInferer all variables that don't have type yet should be considered.
                ///for the purposes of IntegerTypeInferer only variabless assigned Int32 as their type should be considered.
                if (!ShouldConsiderVariable(variableDef))
                {
                    continue;
                }

                ///Add a node for each variable and process all its appearances in the code.

                ClassHierarchyNode variableNode = GetVariableNode(variableDef);
                resultingGraph.Add(variableNode);

                foreach (int defineOffset in pair.Value.DefinedAt)
                {
                    OnPhiVariableAssigned(defineOffset, variableNode);
                }

                foreach (int usageOffset in pair.Value.UsedAt)
                {
                    OnPhiVariableUsed(usageOffset, variableNode);
                }
            }

            RemoveImpossibleEdges();

            HashSet<ClassHierarchyNode> hardNodes = new HashSet<ClassHierarchyNode>();
            foreach (ClassHierarchyNode node in resultingGraph)
            {
                if (node.IsHardNode)
                {
                    hardNodes.Add(node);
                }
            }

            BuildUpHardNodesHierarchy(hardNodes);

            return resultingGraph;
        }

        protected virtual void RemoveImpossibleEdges()
        {
        }

        /// <summary>
        /// Checks if a variable should be included in the type inference.
        /// </summary>
        /// <param name="variableReference">The variable in question.</param>
        /// <returns>Returns true if the type of the variable needs to be infered.</returns>
        protected virtual bool ShouldConsiderVariable(VariableReference variableReference)
        {
            return variableReference.VariableType == null;
        }

        /// <summary>
        /// Handles the case of PhiVariable usages.
        /// </summary>
        /// <param name="instructionOffset">The offset of the instruction that pops the variable from the stack.</param>
        /// <param name="variableNode">The graph node for the variable.</param>
        protected virtual void OnPhiVariableUsed(int instructionOffset, ClassHierarchyNode variableNode)
        {
            ///The usages of the variable bring no value to this algorithm. In the paper they are included, because in Java Bytecode the local method variables 
            ///don't have type, and can be potentionally used before being assigned. Phi variables, however, are always assigned first and used later. Thus, the
            ///information from the assignments should be enough for legal type inference.
            return;
            //Instruction instr = offsetToInstruction[instructionOffset];
            //if (instr.OpCode.Code == Code.Dup || instr.OpCode.Code == Code.Pop)
            //{
            //    return;
            //}
            //ClassHierarchyNode typeNode = GetUseExpressionTypeNode(instr, record.Variable);


            ////this should not be happening when the array support is added
            //if (typeNode == null)
            //{
            //    //only phi variables were part of the expression
            //    List<ClassHierarchyNode> phiVariableNodes = GetUsedPhiVariableNodes(instr.Offset);
            //    for (int i = 0; i < phiVariableNodes.Count; i++)
            //    {
            //        for (int j = i + 1; j < phiVariableNodes.Count; j++)
            //        {
            //            //phi1 <-> phi2
            //            phiVariableNodes[i].AddSupertype(phiVariableNodes[j]);
            //            phiVariableNodes[j].AddSupertype(phiVariableNodes[i]);
            //        }
            //    }
            //    return;
            //}

            //variableNode.AddSupertype(typeNode);
            //nodes.Add(typeNode);
        }

        /// <summary>
        /// Handles the assignment of phi variables.
        /// </summary>
        /// <param name="instructionOffset">The instruction that pushes the value on the stack.</param>
        /// <param name="variableNode">The graph node corresponding to the variable being assigned.</param>
        protected virtual void OnPhiVariableAssigned(int instructionOffset, ClassHierarchyNode variableNode)
        {
            Expression instructionExpression = offsetToExpression[instructionOffset]; // this key should allways be present
            //TypeReference assignedType = ExpressionTypeInferer.GetExpressionType(instructionExpression, context.Method.Module.TypeSystem);
            TypeReference assignedType = instructionExpression.ExpressionType;
            ClassHierarchyNode supertypeNode;
            if (assignedType == null)
            {
                ///Then an expression, containing one or more phi variables is being assigned to another phi variable
                IEnumerable<VariableReference> usedPhiVariables = GetVariables(instructionOffset);

                //VariableReference assigningVariable = (instructionExpression as VariableReferenceExpression).Variable;
                foreach (VariableReference assigningVariable in usedPhiVariables)
                {
                    supertypeNode = GetVariableNode(assigningVariable);
                    supertypeNode.AddSupertype(variableNode);
                    resultingGraph.Add(supertypeNode);
                }
                return;
                //supertypeNode = GetVariableNode(assigningVariable);
            }
            supertypeNode = GetTypeNode(assignedType);
            supertypeNode.AddSupertype(variableNode);
            resultingGraph.Add(supertypeNode);
        }

        /// <summary>
        /// Generates a collection of the phi variables, being used in <paramref name="instructionExpression"/>.
        /// </summary>
        /// <param name="instructionExpression">The instruction, composed of phi variables.</param>
        /// <returns></returns>
        private IEnumerable<VariableReference> GetVariables(int offset)
        {
            List<VariableDefinition> usedVariables;
            if (methodContext.StackData.InstructionOffsetToUsedStackVariablesMap.TryGetValue(offset, out usedVariables))
            {
                List<VariableReference> result = new List<VariableReference>();
                foreach (VariableDefinition varDef in usedVariables)
                {
                    if (methodContext.StackData.VariableToDefineUseInfo.ContainsKey(varDef))
                    {
                        result.Add(varDef);
                    }
                }
                return result;
            }

            return new VariableReference[0];
        }

        /// <summary>
        /// Adds the node, representing System.Object to the graph.
        /// </summary>
        private void AddObjectClassNodeIfMIssing()
        {
            TypeReference tr = typeSystem.Object;
            ClassHierarchyNode objectNode = GetTypeNode(tr);
            resultingGraph.Add(objectNode);
        }

        /// <summary>
        /// Builds the hierarchy between the hard nodes.
        /// </summary>
        /// <param name="hardNodes">Collection of the hard nodes.</param>
        protected virtual void BuildUpHardNodesHierarchy(IEnumerable<ClassHierarchyNode> hardNodes)
        {
            Queue<ClassHierarchyNode> queuedNodes = new Queue<ClassHierarchyNode>(hardNodes);
            HashSet<ClassHierarchyNode> processedNodes = new HashSet<ClassHierarchyNode>();

            while (queuedNodes.Count > 0)
            {
                ClassHierarchyNode currentType = queuedNodes.Dequeue();
                processedNodes.Add(currentType);
                resultingGraph.Add(currentType);
                TypeDefinition currentNodeType = currentType.NodeType.Resolve();
                TypeReference baseTypeRef = null;
                if (currentNodeType == null)
                {
                    continue;
                }
                baseTypeRef = currentNodeType.BaseType;
                if (baseTypeRef != null)
                {
                    ClassHierarchyNode baseType = GetTypeNode(baseTypeRef);
                    currentType.AddSupertype(baseType);
                    if (!processedNodes.Contains(baseType))
                    {
                        queuedNodes.Enqueue(baseType);
                    }
                }

                if (currentNodeType.IsInterface)
                {
                    ClassHierarchyNode objectNode = GetTypeNode(typeSystem.Object);
                    currentType.AddSupertype(objectNode);
                }

                IEnumerable<TypeReference> interfaces = currentType.NodeType.Resolve().Interfaces;
                foreach (TypeReference interfaceRef in interfaces)
                {
                    ClassHierarchyNode implementedInterface = GetTypeNode(interfaceRef);
                    currentType.AddSupertype(implementedInterface);
                    if (!processedNodes.Contains(implementedInterface))
                    {
                        queuedNodes.Enqueue(implementedInterface);
                    }
                }
            }

            AddObjectClassNodeIfMIssing();
        }

        /// <summary>
        /// Locates the graph node corresponding to the supplied <paramref name="type"/>. If it doesn't exist, it's created.
        /// </summary>
        /// <param name="type">A type whose graph node is being searched.</param>
        /// <returns>Returns the graph node.</returns>
        protected virtual ClassHierarchyNode GetTypeNode(TypeReference type)
        {
            string typeName = type.FullName;
            ///All types, smaller than 4 bytes are represented by Int32 at this point.
            if (typeName == "System.Byte" || typeName == "System.SByte" || typeName == "System.Char" ||
                typeName == "System.Int16" || typeName == "System.UInt16" || typeName == "System.Boolean")
            {
                typeName = "System.Int32";
            }
            if (!typeNameToNode.ContainsKey(typeName))
            {
                ClassHierarchyNode typeNode = new ClassHierarchyNode(type);
                typeNameToNode.Add(typeName, typeNode);
            }
            return typeNameToNode[typeName];
        }

        /// <summary>
        /// Finds the ClassHierarchyNode corresponding to the <paramref name="variable"/>. If no such node exists, it's created.
        /// </summary>
        /// <param name="variable">The variable we search the node for.</param>
        /// <returns>Returns the corresponding node in the graph.</returns>
        protected ClassHierarchyNode GetVariableNode(VariableReference variable)
        {
            string variableName = variable.Name;
            if (!variableNameToNode.ContainsKey(variableName))
            {
                ClassHierarchyNode variableNode = new ClassHierarchyNode(variable);
                variableNode = MergeWithVariableTypeIfNeeded(variable, variableNode);
                variableNameToNode.Add(variableName, variableNode);

            }
            return variableNameToNode[variableName];
        }

        protected virtual ClassHierarchyNode MergeWithVariableTypeIfNeeded(VariableReference variable, ClassHierarchyNode variableNode)
        {
            /// The node should be merget with its type in the normal inference.
            if (variable.VariableType != null)
            {
                ClassHierarchyNode variableTypeNode = GetTypeNode(variable.VariableType);
                variableNode = new ClassHierarchyNode(new ClassHierarchyNode[] { variableNode, variableTypeNode });
            }
            return variableNode;
        }

        #region GetUseExpressionType

        /// <summary>
        /// Determines the type of the <paramref name="variable"/> based on its usage.
        /// </summary>
        /// <param name="instruction">The instruction that uses the variable.</param>
        /// <param name="variable">Tha variable.</param>
        /// <returns>Returns the ClassHierarchyNode for the found type.</returns>
        protected ClassHierarchyNode GetUseExpressionTypeNode(Instruction instruction, VariableReference variable)
        {
            Code instrOpCode = instruction.OpCode.Code;
            if (instrOpCode == Code.Ldobj)
            {
                TypeReference tr = instruction.Operand as TypeReference;
                return GetTypeNode(tr);
            }
            if (IsConditionalBranch(instrOpCode))
            {
                return GetTypeNode(typeSystem.Boolean);
            }
            if (instrOpCode == Code.Switch)
            {
                return GetTypeNode(typeSystem.UInt32);
            }
            Expression instructionExpression = offsetToExpression[instruction.Offset];
            return GetUseExpressionTypeNode(instructionExpression, variable);
        }

        /// <summary>
        /// Checks if <paramref name="instructionOpCode"/> is code of conditional jump instruction. For more information see
        /// <see cref="Ecma-355.pdf"/>.
        /// </summary>
        /// <param name="instructionOpCode">The operation code of the instruction.</param>
        /// <returns>Returns true, if the operation conde is for conditional jump.</returns>
        private static bool IsConditionalBranch(Code instructionOpCode)
        {
            return instructionOpCode == Code.Brtrue || instructionOpCode == Code.Brtrue_S || instructionOpCode == Code.Brfalse || instructionOpCode == Code.Brfalse_S;
        }

        /// <summary>
        /// Determines the type which the <paramref name="variable"/> should have, according to its usage in the <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The expression that uses the variable.</param>
        /// <param name="variable">The variable whose type is being infered.</param>
        /// <returns>Returns the ClassHierarchyNode coresponding to the infered type.</returns>
        private ClassHierarchyNode GetUseExpressionTypeNode(Expression expression, VariableReference variable)
        {
            switch (expression.CodeNodeType)
            {
                case CodeNodeType.BaseCtorExpression:
                case CodeNodeType.ThisCtorExpression:
                case CodeNodeType.PropertyReferenceExpression:
                case CodeNodeType.MethodInvocationExpression:
                    return GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
                case CodeNodeType.BinaryExpression:
                    return GetUseInBinaryExpression(expression as BinaryExpression, variable);
                case CodeNodeType.VariableDeclarationExpression:
                    VariableDeclarationExpression vdEx = expression as VariableDeclarationExpression;
                    return GetTypeNode(vdEx.Variable.VariableType);
                case CodeNodeType.VariableReferenceExpression:
                    VariableReferenceExpression vrEx = expression as VariableReferenceExpression;
                    return GetTypeNode(vrEx.Variable.VariableType);
                case CodeNodeType.ExplicitCastExpression:
                case CodeNodeType.SafeCastExpression:
                    return GetTypeNode(typeSystem.Object);
                case CodeNodeType.ArrayIndexerExpression:
                    return GetUseInArrayIndexer(expression as ArrayIndexerExpression, variable);
                case CodeNodeType.ObjectCreationExpression:
                    return GetUseInObjectCreation(expression as ObjectCreationExpression, variable);
                case CodeNodeType.ReturnExpression:
                    return GetTypeNode(methodContext.Method.FixedReturnType);
                case CodeNodeType.ArrayCreationExpression:
                    return GetUseInArrayCreation(expression as ArrayCreationExpression, variable);
                case CodeNodeType.UnaryExpression:
                    return GetUseExpressionTypeNode((expression as UnaryExpression).Operand, variable);
                case CodeNodeType.BoxExpression:
                    return GetTypeNode((expression as BoxExpression).BoxedAs);
                case CodeNodeType.StackAllocExpression:
                    return GetTypeNode((expression as StackAllocExpression).ExpressionType);
                default:
                    throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
            }
        }

        /// <summary>
        /// Resolves the type of usage of <paramref name="variable"/>, when the expression it's used in is ArrayCreationExpression
        /// </summary>
        /// <param name="arrayCreationExpression">The expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode coresponding go the infered type.</returns>
        private ClassHierarchyNode GetUseInArrayCreation(ArrayCreationExpression arrayCreationExpression, VariableReference variable)
        {
            foreach (Expression expr in arrayCreationExpression.Dimensions)
            {
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    ///If the variable is used as an index, then it's type is Int32.
                    return GetTypeNode(typeSystem.Int32);
                }
            }
            foreach (Expression ex in arrayCreationExpression.Initializer.Expressions)
            {
                if (ex is VariableReferenceExpression && (ex as VariableReferenceExpression).Variable == variable)
                {
                    ///If the variable is directly referenced in the Initializer, then its type is the element type of the array.
                    return GetTypeNode(arrayCreationExpression.ElementType);
                }
            }
            throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="objectCreationExpression"/>.
        /// </summary>
        /// <param name="objectCreationExpression">The object creataion expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private ClassHierarchyNode GetUseInObjectCreation(ObjectCreationExpression objectCreationExpression, VariableReference variable)
        {
            Expression arg = null;
            foreach (Expression expr in objectCreationExpression.Arguments)
            {
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    arg = expr;
                }
            }
            return GetTypeNode(objectCreationExpression.Constructor.Parameters[objectCreationExpression.Arguments.IndexOf(arg)].ParameterType);
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="arrayIndexerExpression"/>.
        /// </summary>
        /// <param name="arrayIndexerExpression">The array indexer expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private ClassHierarchyNode GetUseInArrayIndexer(ArrayIndexerExpression arrayIndexerExpression, VariableReference variable)
        {
            foreach (Expression expr in arrayIndexerExpression.Indices)
            {
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    return GetTypeNode(typeSystem.Int32);
                }
            }

            ///If the variable is not indexer, it might be of any array type.
            ///System.Array is returned, since it's the parent type of every array type.
            TypeReference result = new TypeReference("System", "Array", typeSystem.Object.Module, typeSystem.Object.Scope);
            return GetTypeNode(result);
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="binaryExpression"/>.
        /// </summary>
        /// <param name="binaryExpression">The binary expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private ClassHierarchyNode GetUseInBinaryExpression(BinaryExpression binaryExpression, VariableReference variable)
        {
            if (binaryExpression.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                (binaryExpression.Right as VariableReferenceExpression).Variable == variable)
            {
                if (binaryExpression.Left is VariableReferenceExpression && !binaryExpression.Left.HasType)
                {
                    ///If the binary expression is composed of two phi variables, then their types must be the same.
                    ///This covers if we look up for type of the right variable.
                    VariableReference leftVar = (binaryExpression.Left as VariableReferenceExpression).Variable;
                    return GetVariableNode(leftVar);
                }
                return GetTypeNode(binaryExpression.Left.ExpressionType);
            }
            else
            {
                if (binaryExpression.Left is VariableReferenceExpression && (binaryExpression.Left as VariableReferenceExpression).Variable == variable)
                {
                    if (binaryExpression.Right is VariableReferenceExpression && !binaryExpression.Right.HasType)
                    {
                        ///If the binary expression is composed of two phi variables, then their types must be the same.
                        ///This covers if we look up for type of the left variable.
                        return GetVariableNode((binaryExpression.Right as VariableReferenceExpression).Variable);
                    }
                    return GetTypeNode(binaryExpression.Right.ExpressionType);
                }

                ///The variable can be used on the left side as more things than just a refference,
                ///for instance it can be indexer to array.
                return GetUseExpressionTypeNode(binaryExpression.Left, variable);
            }
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="methodInvocationExpression"/>.
        /// </summary>
        /// <param name="methodInvocationExpression">The method invocation expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private ClassHierarchyNode GetUseInMethodInvocation(MethodInvocationExpression methodInvocationExpression, VariableReference variable)
        {
            Expression ex = null;
            foreach (Expression expr in methodInvocationExpression.Arguments)
            {
                //Make a better check if the variable is actually taking place in the argument.
                //Expressions different from VariableReferenceExpression can also be part of method's arguments
                //Sample test: RayTracer.StartRender
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    ex = expr;
                }
            }
            if (ex != null)
            {
                //check if variable is one of method's arguments
                int index = methodInvocationExpression.Arguments.IndexOf(ex);
                return GetTypeNode(methodInvocationExpression.MethodExpression.Method.Parameters[index].ParameterType);
            }
            else
            {
                //variable should be the method target
                Expression target = (methodInvocationExpression.MethodExpression as MemberReferenceExpresion).Target;
                if ((target as VariableReferenceExpression).Variable == variable)
                {
                    //the target should be of the method's declared type
                    //not sure how this handles extension methods
                    return GetTypeNode((methodInvocationExpression.MethodExpression as MemberReferenceExpresion).Member.DeclaringType);
                }
            }
            return null;
        }
        #endregion
    }
}
