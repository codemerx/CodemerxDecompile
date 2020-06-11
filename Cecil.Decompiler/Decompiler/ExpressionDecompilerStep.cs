using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler.Inlining;
using System.Reflection;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
    /// <summary>
    /// Takes care of the translation of IL instructions to C#-like expressions.
    /// For detailed information on what each IL instruction does, see the ECMA 355 standart (ECMA-355.pdf in DecompilationPapers).
    /// </summary>
    class ExpressionDecompilerStep : BaseInstructionVisitor, IDecompilationStep
    {
        /// PhiVariable is a variable, that represents a value left on the stack between different blocks.

        private Stack<Expression> expressionStack = new Stack<Expression>();

        private readonly Dictionary<int, Expression> offsetToExpression;
        private int currentOffset;
        private readonly HashSet<Expression> used;
        private DecompilationContext context;
        private MethodSpecificContext methodContext;
        private TypeSystem currentTypeSystem;
        private TypeSpecificContext typeContext;
        private readonly ExpressionDecompilerData results;
        private readonly Dictionary<VariableReference, KeyValuePair<int, bool>> exceptionVariables;
        private int dummyVarCounter = 0;
        private readonly HashSet<VariableDefinition> stackVariableAssignmentsToRemove = new HashSet<VariableDefinition>();
        private InstructionBlock currentBlock;

        private Instruction CurrentInstruction
        {
            get
            {
                return this.methodContext.ControlFlowGraph.OffsetToInstruction[currentOffset];
            }
        }

        public ExpressionDecompilerStep()
        {
            this.offsetToExpression = new Dictionary<int, Expression>();
            this.results = new ExpressionDecompilerData();
            this.used = new HashSet<Expression>();
            this.exceptionVariables = new Dictionary<VariableReference, KeyValuePair<int, bool>>();
        }

        /// <summary>
        /// This is the entry point of this step. The method is processed block by block. The blocks must be visited in tophological order,
        /// as some of the blocks might produce expressions, used by the others.
        /// </summary>
        /// <param name="theContext">The decompilation context. At this point it must contain the results of the DefineUseAnalysis.</param>
        /// <param name="body"></param>
        /// <returns>Updates <paramref name="theContext"/> so that it holds the expressions for each block. 
        /// The body is being unchanged, since statements are not introduced yet at this point.</returns>
        public BlockStatement Process(DecompilationContext theContext, BlockStatement body)
        {
            context = theContext;
            methodContext = theContext.MethodContext;
            currentTypeSystem = methodContext.Method.Module.TypeSystem;
            typeContext = theContext.TypeContext;

            CreateExpressions();

            methodContext.Expressions = results;

            StackVariablesInliner stackVariablesInliningStep = new StackVariablesInliner(this.methodContext, this.offsetToExpression, this.context.Language.VariablesToNotInlineFinder);
            stackVariablesInliningStep.InlineVariables();

            AddUninlinedStackVariablesToContext();
            ///After all expressions have been made, a type inference is needed.
            ///This is due the fact, that some additional variables might be introduced in the process
            ///for the values, that were left on the stack between blocks.
            ///In order to resolve the type of these variables, all expressions in which those variables take place must be built.
            TypeInference.TypeInferer ti = new TypeInference.TypeInferer(theContext, offsetToExpression);
            ti.InferTypes();

            ///This step passes through the binary expressions in the code, and does transformations in them, so that they are typewise correct.
            ///this should be done after the type inference, since expressions containing variables with infered type might need to be fixed in this step.
            FixBinaryExpressionsStep bef = new FixBinaryExpressionsStep(methodContext.Method.Module.TypeSystem);
            bef.Process(theContext, body);

            MethodVariablesInliner methodVariablesInliningStep = new MethodVariablesInliner(this.methodContext, this.context.Language.VariablesToNotInlineFinder);
            methodVariablesInliningStep.InlineVariables();

            UsageBasedExpressionFixer literalsFixer = new UsageBasedExpressionFixer(methodContext);
            literalsFixer.FixLiterals();

            //ExpressionPropagation.ExpressionPropagationStep eps = new ExpressionPropagation.ExpressionPropagationStep();
            //eps.Process(theContext, body);

            FindAutoBoxesStep fabs = new FindAutoBoxesStep();
            fabs.Process(theContext, body);
            return body;
        }

        private void AddUninlinedStackVariablesToContext()
        {
            foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> pair in methodContext.StackData.VariableToDefineUseInfo)
            {
                if (pair.Value.DefinedAt.Count > 0)
                {
                    methodContext.Variables.Add(pair.Key);
                    methodContext.VariablesToRename.Add(pair.Key);
                }
            }
        }

        private void CreateExpressions()
        {
            foreach (InstructionBlock instructionBlock in methodContext.ControlFlowGraph.Blocks)
            {
                this.currentBlock = instructionBlock;

                List<Expression> resultingExpressions = new List<Expression>();
                foreach (Instruction instruction in instructionBlock)
                {
                    this.currentOffset = instruction.Offset;

                    SetInitialStack(instruction);

                    Visit(instruction);

                    if (expressionStack.Count == 0)
                        continue;

                    offsetToExpression.Add(instruction.Offset, expressionStack.Peek());

                    VariableDefinition stackVariable;
                    if (instruction.OpCode.Code != Code.Pop && methodContext.StackData.InstructionOffsetToAssignedVariableMap.TryGetValue(instruction.Offset, out stackVariable))
                    {
                        Expression expression = expressionStack.Pop();
                        TypeReference expressionType = methodContext.StackData.VariableToDefineUseInfo[stackVariable].DefinedAt.Count == 1 ? expression.ExpressionType : null;
                        stackVariable.VariableType = expressionType;
                        expressionStack.Push(new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(stackVariable, null), expression, currentTypeSystem, null));

                        AddDupInstructionsToMapping(instructionBlock, instruction, expression);
                    }

                    Expression instructionResult = expressionStack.Pop();
                    resultingExpressions.Add(instructionResult);
                }

                if (methodContext.YieldData != null)
                {
                    ProcessYieldBlock(instructionBlock, resultingExpressions);
                }

                results.BlockExpressions.Add(instructionBlock.First.Offset, resultingExpressions);
            }

            foreach (IList<Expression> expressions in results.BlockExpressions.Values)
            {
                for (int i = 0; i < expressions.Count; i++)
                {
                    BinaryExpression binaryExpression = expressions[i] as BinaryExpression;
                    if (binaryExpression != null && binaryExpression.IsAssignmentExpression && binaryExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                        stackVariableAssignmentsToRemove.Contains((binaryExpression.Left as VariableReferenceExpression).Variable.Resolve()))
                    {
                        expressions.RemoveAt(i);
                    }
                }
            }

            foreach (KeyValuePair<int, VariableDefinition> pair in methodContext.StackData.ExceptionHandlerStartToExceptionVariableMap)
            {
                results.ExceptionHandlerStartToVariable.Add(pair.Key, new VariableReferenceExpression(pair.Value, null));
            }
        }

        private void AddDupInstructionsToMapping(InstructionBlock instructionBlock, Instruction instruction, Expression expression)
        {
            if (instruction.OpCode.Code != Code.Pop)
            {
                List<Instruction> dupInstructions = new List<Instruction>();
                Instruction currentInstruction = instruction;
                while (currentInstruction != instructionBlock.Last && currentInstruction.Next.OpCode.Code == Code.Dup)
                {
                    currentInstruction = currentInstruction.Next;
                    dupInstructions.Add(currentInstruction);
                }
                if (dupInstructions.Count > 0)
                {
                    expression.MapDupInstructions(dupInstructions);
                }
            }
        }

        private void SetInitialStack(Instruction instruction)
        {
            List<VariableDefinition> instructionOperands;
            if (methodContext.StackData.InstructionOffsetToUsedStackVariablesMap.TryGetValue(instruction.Offset, out instructionOperands))
            {
                expressionStack = new Stack<Expression>(instructionOperands.Select(varDef => (Expression)new VariableReferenceExpression(varDef, null)));
            }
            else
            {
                expressionStack = new Stack<Expression>();
            }
        }

        /// <summary>
        /// Handles the nop instruction. For more information on nop, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The nop instruction.</param>
        public override void OnNop(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the ret instruction. For more information on ret, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ret instruction.</param>
        public override void OnRet(Instruction instruction)
        {
            ReturnExpression returnEx;
            if (expressionStack.Count == 0)
            {
                returnEx = new ReturnExpression(null, new Instruction[] { instruction });
            }
            else
            {
                if (methodContext.Method.ReturnType.IsByReference)
                {
                    returnEx = new RefReturnExpression(Pop(), new Instruction[] { instruction });
                }
                else
                {
                    returnEx = new ReturnExpression(Pop(), new Instruction[] { instruction });
                }
            }
            Push(returnEx);
        }

        /// <summary>
        /// Handles the br and br.s instructions. For more information on br and br.s, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnBr(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the leave and leave.s instructions. For more information on leave and leave.s, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnLeave(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the endfinally instruction. For more information on endfinally, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnEndfinally(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the endfilter instruction. For more information on endfilter, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnEndfilter(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the stloc instruction when it has operand. For more information on stloc, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStloc(Instruction instruction)
        {
            PushVariableAssignement((VariableReference)instruction.Operand, instruction);
        }

        /// <summary>
        /// Handles the stloc.0 instruction. For more information on stloc.0, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStloc_0(Instruction instruction)
        {
            OnStloc(0, instruction);
        }

        /// <summary>
        /// Handles the stloc.1 instruction. For more information on stloc.1, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStloc_1(Instruction instruction)
        {
            OnStloc(1, instruction);
        }

        /// <summary>
        /// Handles the stloc.2 instruction. For more information on stloc.2, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStloc_2(Instruction instruction)
        {
            OnStloc(2, instruction);
        }

        /// <summary>
        /// Handles the stloc.3 instruction. For more information on stloc.3, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStloc_3(Instruction instruction)
        {
            OnStloc(3, instruction);
        }

        /// <summary>
        /// Handles the stloc.0 to stloc.3 instructions.
        /// </summary>
        /// <param name="index">The index of the local variable</param>
        private void OnStloc(int index, Instruction instruction)
        {
            PushVariableAssignement(index, instruction);
        }

        /// <summary>
        /// Handles the starg instruction. For more information on starg, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStarg(Instruction instruction)
        {
            PushArgumentReference((ParameterReference)instruction.Operand);
            PushAssignment(Pop(), Pop());
        }

        /// <summary>
        /// Pushes assignment to the variable.
        /// </summary>
        /// <param name="variable">Reference to the variable.</param>
        private void PushVariableAssignement(VariableReference variable, Instruction instruction)
        {
            PushVariableAssignement(variable.Index, instruction);
        }

        /// <summary>
        /// Pushes assignment to the local variable referenced by <paramref name="index"/>. Assigned value is the expression on the top of the stack.
        /// </summary>
        /// <param name="index">The index of the local variable.</param>
        private void PushVariableAssignement(int index, Instruction instruction)
        {
            VariableReferenceExpression left = new VariableReferenceExpression(methodContext.Method.Body.Variables[index].Resolve(), new Instruction[] { instruction });
            Expression right = Pop();
            PushAssignment(left, right);
        }

        /// <summary>
        /// Handles the stsfld instruction. For more information on stsfld, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStsfld(Instruction instruction)
        {
            PushAssignment(new FieldReferenceExpression(null, (FieldReference)instruction.Operand, IncludePrefixIfPresent(instruction, Code.Volatile)) { IsSimpleStore = true }, Pop());
        }

        /// <summary>
        /// Handles the stfld instruction. For more information on stfld, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnStfld(Instruction instruction)
        {
            Expression expression = Pop();
            Expression target = Pop();

            //Expression newTarget;
            if (IsPointerType(target))
            {
                target = new UnaryExpression(UnaryOperator.AddressDereference, target, null);
            }

            PushAssignment(new FieldReferenceExpression(target, (FieldReference)instruction.Operand, IncludePrefixIfPresent(instruction, Code.Volatile)) { IsSimpleStore = true }, expression);
        }

        /// <summary>
        /// Pushes BinaryExpression with operator assign, using the parameters suplied as operands in the expression.
        /// </summary>
        /// <param name="left">The expression it is assigned to.</param>
        /// <param name="right">The expression that is being assigned.</param>
        void PushAssignment(Expression left, Expression right, IEnumerable<Instruction> instructions = null)
        {
            BinaryExpression toPush = new BinaryExpression(BinaryOperator.Assign, left, right, currentTypeSystem, instructions);
            Push(toPush);
        }

        /// <summary>
        /// Handles the callvirt instruction. For more information on callvirt, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnCallvirt(Instruction instruction)
        {
            ///Generate the MethodInvocationExpression.
            OnCall(instruction);
            Expression ex = Peek();
            if (ex != null && ex.CodeNodeType == CodeNodeType.MethodInvocationExpression)
            {
                ///Add the flag, that marks this is virtual vall.
                MethodInvocationExpression methodInvocation = ex as MethodInvocationExpression;
                methodInvocation.VirtualCall = true;
                if (instruction.Previous != null && instruction.Previous.OpCode.Code == Code.Constrained && instruction.Previous.Operand != null)
                {
                    methodInvocation.ConstraintType = instruction.Previous.Operand as TypeReference;
                }
            }
        }

        /// <summary>
        /// Handles the castclass instruction. For more information on castclass, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnCastclass(Instruction instruction)
        {
            PushCastExpression((TypeReference)instruction.Operand, instruction);
        }

        /// <summary>
        /// Handles the isinst instruction. For more information on isinst, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnIsinst(Instruction instruction)
        {
            Push(new SafeCastExpression(Pop(), (TypeReference)instruction.Operand, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the call instruction. For more information on call, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnCall(Instruction instruction)
        {
            MethodReference targetMethod = (MethodReference)instruction.Operand;

            int stLoc;
            ExpressionCollection arguments = ProcessArguments(targetMethod);

            ///As chained constructor calls are realised via calling the constructor like normal method, we must take special care of it.
            if ((targetMethod.Name != null) && targetMethod.Name == ".ctor")
            {
                TypeReference targetMethodTypeRef = targetMethod.DeclaringType.GetElementType();
                TypeReference currentMethodBaseTypeRef = methodContext.Method.DeclaringType.BaseType != null ? //null if current type is <Module>
                                                        methodContext.Method.DeclaringType.BaseType.GetElementType() : null;
                TypeReference currentMethodTypeRef = methodContext.Method.DeclaringType.GetElementType();

                if (methodContext.Method.IsConstructor &&
                    (currentMethodBaseTypeRef != null && currentMethodBaseTypeRef.GetFriendlyFullName(null) == targetMethodTypeRef.GetFriendlyFullName(null) || // this ctor
                    currentMethodTypeRef.GetFriendlyFullName(null) == targetMethodTypeRef.GetFriendlyFullName(null)) &&		// base ctor
                    !instruction.Next.IsStoreRegister(out stLoc))
                {
                    Expression target = targetMethod.HasThis ? Pop() : null;
                    target = FixCallTarget(instruction, target);

                    MethodReferenceExpression theMethodRefExpression = new MethodReferenceExpression(target, (MethodReference)instruction.Operand, null);

                    MethodInvocationExpression invocation;
                    if (currentMethodTypeRef.GetFriendlyFullName(null) == targetMethodTypeRef.GetFriendlyFullName(null))
                    {
                        invocation = new ThisCtorExpression(theMethodRefExpression, IncludePrefixIfPresent(instruction, Code.Constrained)) { InstanceReference = target };
                    }
                    else
                    {
                        invocation = new BaseCtorExpression(theMethodRefExpression, IncludePrefixIfPresent(instruction, Code.Constrained)) { InstanceReference = target };
                    }

                    ///Adds the arguments to the constructor call
                    AddRange(invocation.Arguments, arguments);

                    Push(invocation);
                }
                else
                {
                    Expression constructor = ProcessConstructor(instruction, arguments);
                    PushAssignment(new UnaryExpression(UnaryOperator.AddressDereference, Pop(), null), constructor, null);
                }
            }
            else
            {
                ///This is regular method call.

                Expression target = targetMethod.HasThis ? Pop() : null;
                target = FixCallTarget(instruction, target);

                MethodInvocationExpression invocation =
                    new MethodInvocationExpression(new MethodReferenceExpression(target, (MethodReference)instruction.Operand, null), IncludePrefixIfPresent(instruction, Code.Constrained));

                AddRange(invocation.Arguments, arguments);
                if (!TryProcessRuntimeHelpersInitArray(invocation) && !TryProcessMultidimensionalIndexing(invocation))
                {
                    invocation.VirtualCall = false;
                    Push(invocation);

                    if (Utilities.IsComputeStringHashMethod(instruction.Operand as MethodReference))
                    {
                        this.context.MethodContext.SwitchByStringData.SwitchBlocksStartInstructions.Add(this.currentBlock.First.Offset);
                    }
                }
            }
        }

        private Expression FixCallTarget(Instruction instruction, Expression target)
        {
            if (target != null)
            {
                if (IsPointerType(target))
                {
                    target = new UnaryExpression(UnaryOperator.AddressDereference, target, null);
                }

                if (target.HasType && target.ExpressionType != null)
                {
                    MethodReference methodRef = (MethodReference)instruction.Operand;
                    MethodDefinition methodDef = methodRef.Resolve();
                    if (methodRef.DeclaringType != null && methodDef != null && methodDef.DeclaringType != null && methodDef.DeclaringType.IsInterface)
                    {
                        TypeDefinition targetTypeDef = target.ExpressionType.Resolve();
                        if (targetTypeDef != null && targetTypeDef != methodDef.DeclaringType && !targetTypeDef.IsInterface)
                        {
                            return new ExplicitCastExpression(target, methodRef.DeclaringType, null) { IsExplicitInterfaceCast = true };
                        }
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// Handles the case, where method invocation is to a compiler-generated method, dealing with multidimentional array indexing.
        /// If this is the case, the method invocation is being replaced with the corresponding ArrayIndexingExpression
        /// </summary>
        /// <param name="invocation">The method invocation expression.</param>
        /// <returns>Returns true if the method invocation is replaced by array indexing.</returns>
        private bool TryProcessMultidimensionalIndexing(MethodInvocationExpression invocation)
        {
            if (invocation.MethodExpression == null || invocation.Arguments == null || invocation.Arguments.Count < 1)
            {
                return false;
            }
            MethodReferenceExpression methodRef = invocation.MethodExpression;

            Expression target = null;
            string methodName = null;

            if (TryGetTargetArray(methodRef, out target) && TryGetArrayIndexingMethodName(methodRef, out methodName))
            {
                ArrayIndexerExpression arrayIndexer = new ArrayIndexerExpression(target, invocation.InvocationInstructions);
                arrayIndexer.Indices = invocation.Arguments;

                if (methodName == "Set")
                {
                    ///This is setter method. The last argument is not an index, but the value being assigned.
                    arrayIndexer.IsSimpleStore = true;
                    Expression value = arrayIndexer.Indices[arrayIndexer.Indices.Count - 1];
                    arrayIndexer.Indices.RemoveAt(arrayIndexer.Indices.Count - 1);
                    PushAssignment(arrayIndexer, value);
                }
                else if (methodName == "Address")
                {
                    Push(new UnaryExpression(UnaryOperator.AddressReference, arrayIndexer, null));
                }
                else
                {
                    ///This is getter method. The array indexing expression is complete at this point.
                    Push(arrayIndexer);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resolves the target of the MemberReferenceExpression
        /// </summary>
        /// <param name="memberRef">The expression which target is being resolved</param>
        /// <param name="target">Placeholder for the resolved array target.</param>
        /// <returns>Returns true if the member target is array.</returns>
        private bool TryGetTargetArray(MethodReferenceExpression methodRef, out Expression target)
        {
            if (methodRef == null || !(methodRef.Member.DeclaringType is ArrayType))
            {
                target = null;
                return false;
            }
            target = methodRef.Target;
            return true;
        }

        /// <summary>
        /// Resolves the method name.
        /// </summary>
        /// <param name="methodRef">The method.</param>
        /// <param name="methodName">Placeholderfor the name.</param>
        /// <returns>Returns true if the method name is one of teh compiler generated for array indexing methods.</returns>
        private bool TryGetArrayIndexingMethodName(MethodReferenceExpression methodRef, out string methodName)
        {
            methodName = methodRef.Member.Name;
            return methodName == "Get" || methodName == "Set" || methodName == "Address";
        }

        /// <summary>
        /// Processess compiler-generated runtime helpers used in the initialization of arrays.
        /// </summary>
        /// <param name="invocation">The method invocation</param>
        /// <returns>Returns true if the method invocation has been converted to short-form array creation.</returns>
        private bool TryProcessRuntimeHelpersInitArray(MethodInvocationExpression invocation)
        {
            string initializeArrayName = "System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle)";
            MethodReferenceExpression methodRef = invocation.MethodExpression;
            if (methodRef != null && methodRef.Method.FullName == initializeArrayName)
            {
                ArrayCreationExpression initArray = GetClosestArrayCreationExpression();
                if (initArray == null)
                {
                    throw new Exception("The expression at the top of the expression stack is not ArrayCreationExpression");
                }
				var blockExpression = new BlockExpression(invocation.InvocationInstructions);
				initArray.Initializer = new InitializerExpression(blockExpression, InitializerType.ArrayInitializer);
				Expression initializationInfo;
                if (!TryGetVariableValueAndMarkForRemoval(invocation.Arguments[1] as VariableReferenceExpression, out initializationInfo))
                {
                    throw new Exception("Invalid array initialization info");
                }
                initArray.Initializer.Expressions.Add(initializationInfo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds the closest previous ArrayCreationExpression.
        /// </summary>
        /// <returns>Returns the closest previous ArrayCreationExpression.</returns>
        private ArrayCreationExpression GetClosestArrayCreationExpression()
        {
            //might need big fixes if array initialization is from another block
            for (int i = currentOffset; i >= 0; i--)
            {
                if (offsetToExpression.ContainsKey(i))
                {
                    if (offsetToExpression[i] is ArrayCreationExpression)
                    {
                        return offsetToExpression[i] as ArrayCreationExpression;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Handles the calli instruction. For more information on calli, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <exception cref="DecompilationException">Thrown when the pointer can not be resolved to a method.</exception>
        /// <param name="instruction">The calli instruction.</param>
        public override void OnCalli(Instruction instruction)
        {
            Expression value;
            if (!TryGetVariableValueAndMarkForRemoval(Pop() as VariableReferenceExpression, out value) || value.CodeNodeType != CodeNodeType.MethodReferenceExpression)
            {
                throw new DecompilationException("Method pointer cannot be resolved to a method definition.");
            }

            MethodReferenceExpression methodRef = value as MethodReferenceExpression;
            ExpressionCollection arguments = ProcessArguments(methodRef.Method);
            if (methodRef.Method.HasThis)
            {
                methodRef.Target = Pop();
            }
            MethodInvocationExpression invocation = new MethodInvocationExpression(methodRef, new Instruction[] { instruction });
            invocation.Arguments = arguments;
            Push(invocation);
        }

        private bool TryGetVariableValueAndMarkForRemoval(VariableReferenceExpression variableRefExpression, out Expression value)
        {
            StackVariableDefineUseInfo defineUseInfo;
            if (variableRefExpression == null || !methodContext.StackData.VariableToDefineUseInfo.TryGetValue(variableRefExpression.Variable.Resolve(), out defineUseInfo) ||
                defineUseInfo.DefinedAt.Count != 1 || defineUseInfo.UsedAt.Count != 1 || !offsetToExpression.TryGetValue(defineUseInfo.DefinedAt.First(), out value))
            {
                value = null;
                return false;
            }

            VariableDefinition varDef = variableRefExpression.Variable.Resolve();
            methodContext.StackData.VariableToDefineUseInfo.Remove(varDef);
            stackVariableAssignmentsToRemove.Add(varDef);
            return true;
        }

        /// <summary>
        /// Handles the newobj instruction. For more information on newobj, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The newobj instruction.</param>
        public override void OnNewobj(Instruction instruction)
        {
            MethodReference targetMethod = (MethodReference)instruction.Operand;
            ObjectCreationExpression objectCreationExpression = ProcessConstructor(instruction, ProcessArguments(targetMethod));

            /// In IL multidimentional arrays are created via newObj instruction, rather than newarr. This is why special care must be taken in this case.
            Expression creationExpression = ConvertObjectToArray(objectCreationExpression);
            if (creationExpression == null)
            {
                creationExpression = objectCreationExpression;
            }
            Push(creationExpression);
        }

        /// <summary>
        /// Checks if the object created is multidimentional array.
        /// </summary>
        /// <param name="objectCreation">The object creation expression.</param>
        /// <returns>Returns corresponding ArrayCreationExpression if the object is multidimentional array, or null if it's not.</returns>
        private ArrayCreationExpression ConvertObjectToArray(ObjectCreationExpression objectCreation)
        {
            TypeReference arrayType = GetElementTypeFromCtor(objectCreation.Constructor);
            if (arrayType == null)
            {
                return null;
            }

            ArrayCreationExpression arrayCreation = new ArrayCreationExpression(arrayType, null, objectCreation.MappedInstructions);
            arrayCreation.Dimensions = CopyExpressionCollection(objectCreation.Arguments);

            return arrayCreation;
        }

        /// <summary>
        /// Gets the element type if the array, that is constructed by the provided method.
        /// </summary>
        /// <param name="constructor">The constructor of the array.</param>
        /// <returns>Returns the element type, or null if the method doens't construct an array.</returns>
        private TypeReference GetElementTypeFromCtor(MethodReference constructor)
        {
            ArrayType arrayType = constructor.DeclaringType as ArrayType;
            if (arrayType == null)
            {
                return null;
            }

            return arrayType.ElementType;
        }

        /// <summary>
        /// Shallow clones the expression collection.
        /// </summary>
        /// <param name="arguments">The expression collection to copy.</param>
        /// <returns>The copy.</returns>
        private ExpressionCollection CopyExpressionCollection(ExpressionCollection arguments)
        {
            ExpressionCollection result = new ExpressionCollection();
            foreach (Expression dimention in arguments)
            {
                result.Add(dimention);
            }
            return result;
        }

        /// <summary>
        /// Handles the initobj instruction. For more information on initobj, see <see cref="ECMA-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction"></param>
        public override void OnInitobj(Instruction instruction)
        {
            // the destination is always a pointer, according to the standart (see page 438 of ECMA-355 - "4.5 initobj – initialize the value at an address" )
            Expression destination = new UnaryExpression(UnaryOperator.AddressDereference, Pop(), null);

            TypeReference type = (TypeReference)instruction.Operand;

            Expression @new = new ObjectCreationExpression(null, type, null, null);

            if (type.IsGenericInstance)
            {
                if (type.GetFriendlyFullName(null).IndexOf("System.Nullable<") >= 0)
                    @new = new LiteralExpression(null, currentTypeSystem, null);
            }
            else if (type.IsGenericParameter)
            {
                @new = new DefaultObjectExpression((TypeReference)instruction.Operand, null);
            }

            PushAssignment(destination, @new, new Instruction[] { instruction });
        }

        /// <summary>
        /// Generates the corresponding ObjectCreationExpression.
        /// </summary>
        /// <param name="instruction">The instruction, that creates the object.</param>
        /// <param name="arguments">The collection of arguments, needed for the construction of the object.</param>
        /// <returns>Returns the generated ObjectCreationExpression.</returns>
        private ObjectCreationExpression ProcessConstructor(Instruction instruction, ExpressionCollection arguments)
        {
            IEnumerable<Instruction> instructions = instruction.OpCode.Code == Code.Callvirt ? IncludePrefixIfPresent(instruction, Code.Constrained) : new Instruction[] { instruction };
            MethodReference constructor = (MethodReference)instruction.Operand;
            ObjectCreationExpression @new = new ObjectCreationExpression(constructor, constructor != null ? constructor.DeclaringType : null, null, instructions);
            AddRange(@new.Arguments, arguments);
            return @new;
        }

        /// <summary>
        /// Adds range of items from <paramref name="range"/> to <paramref name="list"/>
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="list">The collection, in which to add items.</param>
        /// <param name="range">The collection containing the items to add.</param>
        static void AddRange<T>(IList<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
                list.Add(item);
        }

        /// <summary>
        /// Handles the dup instruction. For more information on dup, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The dup instruction.</param>
        public override void OnDup(Instruction instruction)
        {
            //Handled by the VariableDefineUseAnalysis

            //Expression duppedExpression = Pop();
            //if (duppedExpression.HasType)
            //{
            //    ///For each dup instruction, a new phi variable is created.
            //    ///Since the type of that variable depends only on the dup expression, it can be set at this point.
            //    PhiFunction pf = methodContext.DefineUseAnalysisData.InstructionData[instruction.Offset].StackAfter.Peek().PhiFunction;
            //    VariableReference dupVar = methodContext.DefineUseAnalysisData.PhiToVariable[pf];
            //    dupVar.VariableType = duppedExpression.ExpressionType;

            //    if (duppedExpression is VariableReferenceExpression)
            //    {
            //        ///This handles the cases with Catch clauses, where the exception is first being dupped.
            //        VariableReference theVar = (duppedExpression as VariableReferenceExpression).Variable;
            //        if (!results.ExceptionHandlerStartToVariable.ContainsKey(currentBlock.First.Offset))
            //        {
            //            return;
            //        }
            //        if (results.ExceptionHandlerStartToVariable[currentBlock.First.Offset].Variable == theVar)
            //        {
            //            PushAssignment(new VariableReferenceExpression(dupVar, new Instruction[] { instruction }), duppedExpression);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Handles the pop instruction. For more information on pop, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The pop instruction.</param>
        public override void OnPop(Instruction instruction)
        {
            Expression toPop = Pop();
            string dummyVarName = "dummyVar" + this.dummyVarCounter;
            dummyVarCounter++;
            TypeReference dummyVarType = null;
            if (toPop.HasType)
            {
                dummyVarType = toPop.ExpressionType;
            }
            VariableDefinition dummyVar = new VariableDefinition(dummyVarName, dummyVarType, this.methodContext.Method);
            
            StackVariableDefineUseInfo defineUseInfo = new StackVariableDefineUseInfo();
            defineUseInfo.DefinedAt.Add(instruction.Offset);
            methodContext.StackData.VariableToDefineUseInfo.Add(dummyVar, defineUseInfo);
            methodContext.StackData.InstructionOffsetToAssignedVariableMap.Add(instruction.Offset, dummyVar);

            methodContext.VariablesToRename.Add(dummyVar);
            VariableReferenceExpression dummyVarRef = new VariableReferenceExpression(dummyVar, new Instruction[] { instruction });
            BinaryExpression dummyAssignment = new BinaryExpression(BinaryOperator.Assign, dummyVarRef, toPop, currentTypeSystem, null);
            Push(dummyAssignment);
            return;
        }

        /// <summary>
        /// Checks if MethodInvocationExpression is embeded in CastExpression.
        /// </summary>
        /// <param name="toPop">Expression in question.</param>
        /// <returns>Returns true if there is MethodInvocationExpression embeded in <paramref name="expression"/>.</returns>
        public bool CheckForCastBecauseForeach(Expression expression)
        {
            ExplicitCastExpression castExpression = expression as ExplicitCastExpression;
            if (castExpression != null && castExpression.Expression is MethodInvocationExpression)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the throw instruction. For more information on throw, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The throw instruction.</param>
        public override void OnThrow(Instruction instruction)
        {
            ThrowExpression throwEx = new ThrowExpression(Pop(), new Instruction[] { instruction });
            Push(throwEx);
        }

        /// <summary>
        /// Handles the rethrow instruction. For more information on rethrow, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The rethrow instruction.</param>
        public override void OnRethrow(Instruction instruction)
        {
            ///in C# rethrow is translated as empty throw inside ExceptionHandling block.
            Push(new ThrowExpression(null, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the add instruction. For more information on add, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The add instruction.</param>
        public override void OnAdd(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Add, instruction);
        }

        /// <summary>
        /// Determines if a target of an expression is a pointer type.
        /// </summary>
        /// <param name="expression">The expression in question</param>
        /// <returns>Returns true, if the target of the expression is of pointer type.</returns>
        private bool IsPointerType(Expression expression)
        {
            if (!expression.HasType)
            {
                return false;
            }
            TypeReference typeRef = expression.ExpressionType;
            if (typeRef == null)
            {
                return false;
            }
            if (typeRef.IsPointer)
            {
                return true;
            }
            if (typeRef.IsPinned)
            {
                PinnedType pt = typeRef as PinnedType;
                return pt.ElementType.IsByReference || pt.ElementType.IsPointer;
            }
            if (typeRef.IsByReference)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the add.ovf instruction. For more information on add.ovf, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The add.ovf instruction.</param>
        public override void OnAdd_Ovf(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Add, instruction);
        }

        /// <summary>
        /// Handles the add.ovf.un instruction. For more information on add.ovf.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The add.ovf.un instruction.</param>
        public override void OnAdd_Ovf_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Add, instruction);
        }

        /// <summary>
        /// Handles the sub instruction. For more information on sub, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The sub instruction.</param>
        public override void OnSub(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Subtract, instruction);
        }

        /// <summary>
        /// Handles the sub.ovf instruction. For more information on sub.ovf, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The sub.ovf instruction.</param>
        public override void OnSub_Ovf(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Subtract, instruction);
        }

        /// <summary>
        /// Handles the sub.ovf.un instruction. For more information on sub.ovf.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The sub.ovf.un instruction.</param>
        public override void OnSub_Ovf_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Subtract, instruction);
        }

        /// <summary>
        /// Handles the mul instruction. For more information on mul, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The mul instruction.</param>
        public override void OnMul(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Multiply, instruction);
        }

        /// <summary>
        /// Handles the mul.ovf instruction. For more information on mul.ovf, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The mul.ovf instruction.</param>
        public override void OnMul_Ovf(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Multiply, instruction);
        }

        /// <summary>
        /// Handles the mul.ovf.un instruction. For more information on mul.ovf.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The mul.ovf.un instruction.</param>
        public override void OnMul_Ovf_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Multiply, instruction);
        }

        /// <summary>
        /// Handles the div instruction. For more information on div, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The div instruction.</param>
        public override void OnDiv(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Divide, instruction);
        }

        /// <summary>
        /// Handles the div.un instruction. For more information on div.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The div.un instruction.</param>
        public override void OnDiv_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Divide, instruction);
        }

        /// <summary>
        /// Creates and pushes on the stack a binary expression using the given operator.
        /// The right operand of the expression is the top of the stack, the left one is the element under it.
        /// </summary>
        /// <param name="op">The operator of the binary expression.</param>
        public void PushBinaryExpression(BinaryOperator op, Instruction instruction)
        {
            Expression right = Pop();
            Expression left = Pop();
            BinaryExpression toPush = new BinaryExpression(op, left, right, currentTypeSystem, new Instruction[] { instruction });
            if (toPush.IsComparisonExpression || toPush.IsLogicalExpression)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.Boolean;
            }
            Push(toPush);
        }

        /// <summary>
        /// Handles the ldstr instruction. For more information on ldstr, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldstr instruction.</param>
        public override void OnLdstr(Instruction instruction)
        {
            PushLiteral(instruction.Operand, instruction);
        }

        /// <summary>
        /// Handles the ldc.r4 instruction. For more information on ldc.r4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.r4 instruction.</param>
        public override void OnLdc_R4(Instruction instruction)
        {
            PushLiteral(Convert.ToSingle(instruction.Operand), instruction);
        }

        /// <summary>
        /// Handles the ldc.r8 instruction. For more information on ldc.r8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.r8 instruction.</param>
        public override void OnLdc_R8(Instruction instruction)
        {
            PushLiteral(Convert.ToDouble(instruction.Operand), instruction);
        }

        /// <summary>
        /// Handles the ldc.i8 instruction. For more information on ldc.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i8 instruction.</param>
        public override void OnLdc_I8(Instruction instruction)
        {
            PushLiteral(Convert.ToInt64(instruction.Operand), instruction);
        }

        /// <summary>
        /// Handles the ldc.i4 instruction. For more information on ldc.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4 instruction.</param>
        public override void OnLdc_I4(Instruction instruction)
        {
            PushLiteral(Convert.ToInt32(instruction.Operand), instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.m1 instruction. For more information on ldc.i4.m1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.m1 instruction.</param>
        public override void OnLdc_I4_M1(Instruction instruction)
        {
            PushLiteral(-1, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.0 instruction. For more information on ldc.i4.0, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.0 instruction.</param>
        public override void OnLdc_I4_0(Instruction instruction)
        {
            PushLiteral(0, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.1 instruction. For more information on ldc.i4.1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.1 instruction.</param>
        public override void OnLdc_I4_1(Instruction instruction)
        {
            PushLiteral(1, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.2 instruction. For more information on ldc.i4.2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.2 instruction.</param>
        public override void OnLdc_I4_2(Instruction instruction)
        {
            PushLiteral(2, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.3 instruction. For more information on ldc.i4.3, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.3 instruction.</param>
        public override void OnLdc_I4_3(Instruction instruction)
        {
            PushLiteral(3, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.4 instruction. For more information on ldc.i4.4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.4 instruction.</param>
        public override void OnLdc_I4_4(Instruction instruction)
        {
            PushLiteral(4, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.5 instruction. For more information on ldc.i4.5, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.5 instruction.</param>
        public override void OnLdc_I4_5(Instruction instruction)
        {
            PushLiteral(5, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.6 instruction. For more information on ldc.i4.6, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.6 instruction.</param>
        public override void OnLdc_I4_6(Instruction instruction)
        {
            PushLiteral(6, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.7 instruction. For more information on ldc.i4.7, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.7 instruction.</param>
        public override void OnLdc_I4_7(Instruction instruction)
        {
            PushLiteral(7, instruction);
        }

        /// <summary>
        /// Handles the ldc.i4.8 instruction. For more information on ldc.i4.8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldc.i4.8 instruction.</param>
        public override void OnLdc_I4_8(Instruction instruction)
        {
            PushLiteral(8, instruction);
        }

        /// <summary>
        /// Handles the ldloc.0 instruction. For more information on ldloc.0, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldloc.0 instruction.</param>
        public override void OnLdloc_0(Instruction instruction)
        {
            PushVariableReference(0, instruction);
        }

        /// <summary>
        /// Handles the ldloc.1 instruction. For more information on ldloc.1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldloc.1 instruction.</param>
        public override void OnLdloc_1(Instruction instruction)
        {
            PushVariableReference(1, instruction);
        }

        /// <summary>
        /// Handles the ldloc.2 instruction. For more information on ldloc.2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldloc.2 instruction.</param>
        public override void OnLdloc_2(Instruction instruction)
        {
            PushVariableReference(2, instruction);
        }

        /// <summary>
        /// Handles the ldloc.3 instruction. For more information on ldloc.3, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldloc.3 instruction.</param>
        public override void OnLdloc_3(Instruction instruction)
        {
            PushVariableReference(3, instruction);
        }

        /// <summary>
        /// Handles the ldloc instruction. For more information on ldloc, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldloc instruction.</param>
        public override void OnLdloc(Instruction instruction)
        {
            PushVariableReference((VariableReference)instruction.Operand, instruction);
        }

        /// <summary>
        /// Handles the ldloca instruction. For more information on ldloca, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldloca instruction.</param>
        public override void OnLdloca(Instruction instruction)
        {
            VariableReference variableReference = instruction.Operand as VariableReference;
            //PushVariableReference(variableReference, instruction);
            Expression variable = new VariableReferenceExpression(variableReference, new Instruction[] { instruction });
            UnaryExpression address = new UnaryExpression(UnaryOperator.AddressReference, variable, null);
            //PushAddressOf();
            Push(address);
        }

        /// <summary>
        /// Pushes VariableReferenceExpression on the expressionStack.
        /// </summary>
        /// <param name="index">The IL index of the variable to be referenced.</param>
        private void PushVariableReference(int index, Instruction instruction)
        {
            PushVariableReference(methodContext.Method.Body.Variables[index].Resolve(), instruction);
        }

        /// <summary>
        /// Pushes VariableReferenceExpression on the expressionStack.
        /// </summary>
        /// <param name="variable">The variable to be referenced.</param>
        private void PushVariableReference(VariableReference variable, Instruction instruction)
        {
            Push(new VariableReferenceExpression(variable, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the newarr instruction. For more information on newarr, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The newarr instruction.</param>
        public override void OnNewarr(Instruction instruction)
        {
			ArrayCreationExpression creation = new ArrayCreationExpression(
				(TypeReference)instruction.Operand,
				new InitializerExpression(new BlockExpression(null), InitializerType.ArrayInitializer),
				new Instruction[] { instruction });

            creation.Dimensions.Add(Pop());

            Push(creation);
        }

        /// <summary>
        /// Handles the ldlen instruction. For more information on ldlen, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldlen instruction.</param>
        public override void OnLdlen(Instruction instruction)
        {
            //PropertyDefinition arrayLength = new PropertyDefinition("Length", PropertyAttributes.SpecialName | PropertyAttributes.RTSpecialName,
            //        methodContext.Method.Module.TypeSystem.Int32);
            //var target = Pop();
            Push(new ArrayLengthExpression(Pop(), methodContext.Method.Module.TypeSystem, new Instruction[] { instruction }));
            //Push(new PropertyReferenceExpression(null));
        }

        /// <summary>
        /// Handles the ldelem instruction. For more information on ldelem, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem instruction.</param>
        public override void OnLdelem_Any(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelema instruction. For more information on ldelema, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelema instruction.</param>
        public override void OnLdelema(Instruction instruction)
        {
            PushArrayIndexer(true, instruction);
        }

        /// <summary>
        /// Handles the ldelem.i instruction. For more information on ldelem.i, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.i instruction.</param>
        public override void OnLdelem_I(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.i1 instruction. For more information on ldelem.i1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.i1 instruction.</param>
        public override void OnLdelem_I1(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.i2 instruction. For more information on ldelem.i2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.i2 instruction.</param>
        public override void OnLdelem_I2(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.i4 instruction. For more information on ldelem.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.i4 instruction.</param>
        public override void OnLdelem_I4(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.i8 instruction. For more information on ldelem.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.i8 instruction.</param>
        public override void OnLdelem_I8(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.r4 instruction. For more information on ldelem.r4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.r4 instruction.</param>
        public override void OnLdelem_R4(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.r8 instruction. For more information on ldelem.r8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.r8 instruction.</param>
        public override void OnLdelem_R8(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.ref instruction. For more information on ldelem.ref, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.ref instruction.</param>
        public override void OnLdelem_Ref(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.u1 instruction. For more information on ldelem.u1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.u1 instruction.</param>
        public override void OnLdelem_U1(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.u2 instruction. For more information on ldelem.u2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.u2 instruction.</param>
        public override void OnLdelem_U2(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the ldelem.u4 instruction. For more information on ldelem.u4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldelem.u4 instruction.</param>
        public override void OnLdelem_U4(Instruction instruction)
        {
            PushArrayIndexer(false, instruction);
        }

        /// <summary>
        /// Handles the stelem instruction. For more information on stelem, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem instruction.</param>
        public override void OnStelem_Any(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.i instruction. For more information on stelem.i, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i instruction.</param>
        public override void OnStelem_I(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.i1 instruction. For more information on stelem.i1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i1 instruction.</param>
        public override void OnStelem_I1(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.i2 instruction. For more information on stelem.i2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i2 instruction.</param>
        public override void OnStelem_I2(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.i4 instruction. For more information on stelem.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i4 instruction.</param>
        public override void OnStelem_I4(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.i8 instruction. For more information on stelem.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i8 instruction.</param>
        public override void OnStelem_I8(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.ref instruction. For more information on stelem.ref, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.ref instruction.</param>
        public override void OnStelem_Ref(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.r4 instruction. For more information on stelem.r4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.r4 instruction.</param>
        public override void OnStelem_R4(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the stelem.r8 instruction. For more information on stelem.r8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.r8 instruction.</param>
        public override void OnStelem_R8(Instruction instruction)
        {
            PushArrayStore(instruction);
        }

        /// <summary>
        /// Handles the sizeof instruction. For more information on sizeof, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The sizeof instruction.</param>
        public override void OnSizeof(Instruction instruction)
        {
            Push(new SizeOfExpression((TypeReference)instruction.Operand, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the stind.i instruction. For more information on stind.i, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i instruction.</param>
        public override void OnStind_I(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.i1 instruction. For more information on stind.i1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i1 instruction.</param>
        public override void OnStind_I1(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.i2 instruction. For more information on stind.i2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i2 instruction.</param>
        public override void OnStind_I2(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.i4 instruction. For more information on stind.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i4 instruction.</param>
        public override void OnStind_I4(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.i8 instruction. For more information on stind.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.i8 instruction.</param>
        public override void OnStind_I8(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.ref instruction. For more information on stind.ref, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.ref instruction.</param>
        public override void OnStind_Ref(Instruction instruction)
        {
            //TODO: Should check if a cast is needed because of the <token> operand. This check should be done to all ldind*, stind*, stobj, ldobj, cpobj, etc.. instructions
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.r4 instruction. For more information on stind.r4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.r4 instruction.</param>
        public override void OnStind_R4(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the stind.r8 instruction. For more information on stind.r8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stelem.r8 instruction.</param>
        public override void OnStind_R8(Instruction instruction)
        {
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the refanytype instruction. For more information on refanytype, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The refanytype instruction.</param>
        public override void OnRefanytype(Instruction instruction)
        {
            PushCastExpression(Import(typeof(RuntimeTypeHandle)), instruction);
        }

        /// <summary>
        /// The method commonly used by stind.* instructions. Pushes array elemet assignment on the expression stack.
        /// </summary>
        private void PushArrayStore(Instruction instruction)
        {
            Expression right = Pop();
            PushArrayIndexer(false, instruction);
            ArrayIndexerExpression left = Pop() as ArrayIndexerExpression;
            left.IsSimpleStore = true;

            BinaryExpression assign = new BinaryExpression(BinaryOperator.Assign, left, right, currentTypeSystem, null);
            Push(assign);
        }

        /// <summary>
        /// The method commonly used by ldind.* instructions. Pushes array element reference on the expression stack.
        /// </summary>
        /// <param name="isAddress">True if the reference to the array element is pushed on the stack. False if the element itself is pushed on the stack.</param>
        private void PushArrayIndexer(bool isAddress, Instruction instruction)
        {
            Expression indices = Pop();
            ArrayIndexerExpression indexer = new ArrayIndexerExpression(Pop(), new Instruction[] { instruction });
            indexer.Indices.Add(indices);
            if (isAddress)
            {
                Push(new UnaryExpression(UnaryOperator.AddressReference, indexer, null));
            }
            else
            {
                Push(indexer);
            }
        }

        /// <summary>
        /// Handles the box instruction. For more information on box, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The box instruction.</param>
        public override void OnBox(Instruction instruction)
        {
            Expression expression = Pop();
            TypeReference boxingTypeRef = (TypeReference)instruction.Operand;

            if (expression.ExpressionType != null && expression.ExpressionType.GetFriendlyFullName(null) != boxingTypeRef.GetFriendlyFullName(null))
            {
                // Happens if there was a cast and a box of an enum value to its underlying type - it's transalated as boxing the underlying type
                expression = new ExplicitCastExpression(expression, boxingTypeRef, null);
            }
            else if (expression.ExpressionType == null && expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                //Phi variable
                VariableReferenceExpression varRefExpression = expression as VariableReferenceExpression;
                varRefExpression.Variable.VariableType = boxingTypeRef;
            }

            expression = new BoxExpression(expression, methodContext.Method.Module.TypeSystem, (TypeReference)instruction.Operand, new Instruction[] { instruction });
            Push(expression);
            //Push(Pop());
        }

        /// <summary>
        /// Handles the unbox instruction. For more information on unbox, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The unbox instruction.</param>
        public override void OnUnbox(Instruction instruction)
        {
            PushCastExpression(new ByReferenceType((TypeReference)instruction.Operand), instruction);
        }

        /// <summary>
        /// Handles the unbox.any instruction. For more information on unbox.any, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The unbox.any instruction.</param>
        public override void OnUnbox_Any(Instruction instruction)
        {
            PushCastExpression((TypeReference)instruction.Operand, instruction);
        }

        /// <summary>
        /// Imports the supplied type and returns TypeReference to it. <paramref name="type"/> must be a type from mscorlib.dll.
        /// </summary>
        /// <param name="type">The type that needs to be imported.</param>
        /// <returns>TypeReference to the suplied type.</returns>
        private TypeReference Import(Type type)
        {
            return Utilities.GetCorlibTypeReference(type, methodContext.Method.Module);
        }

        /// <summary>
        /// Pushes CastExpression to the supplied <paramref name="targetType"/>. Takes the top of the expression stack as argument.
        /// </summary>
        /// <param name="targetType">The type to be casted to.</param>
        private void PushCastExpression(TypeReference targetType, Instruction instruction)
        {
            Instruction[] instructionArray = instruction != null ? new Instruction[] { instruction } : null;
            Push(new ExplicitCastExpression(Pop(), targetType, instructionArray));
        }

        /// <summary>
        /// Handles the conv.i instruction. For more information on conv.i, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.i instruction.</param>
        public override void OnConv_I(Instruction instruction)
        {
            ///In IL this instruction converts the top of the stack to native int. As native ints are used for pointers in the CLR,
            ///special care must be taken.
            Expression expression = expressionStack.Peek();

            if (IsPointerType(expression) ||
                expression.CodeNodeType == CodeNodeType.UnaryExpression && (expression as UnaryExpression).Operator == UnaryOperator.AddressOf)
            {
                Push(Pop());
                return;
            }

            Expression toPush = Pop();
            if (toPush.HasType && (toPush.ExpressionType.FullName == methodContext.Method.Module.TypeSystem.Int32.FullName ||
                toPush.ExpressionType.FullName == methodContext.Method.Module.TypeSystem.UInt32.FullName))
            {
                Push(toPush);
                return;
            }

            if (toPush.HasType && IsIntegerType(toPush.ExpressionType))
            {
                ExplicitCastExpression theCast = new ExplicitCastExpression(toPush, new PointerType(methodContext.Method.Module.TypeSystem.Void), new Instruction[] { instruction });
                Push(theCast);
                return;
            }

            UnaryExpression result = new UnaryExpression(UnaryOperator.AddressOf, toPush, new Instruction[] { instruction });
            Push(result);
            //if (toPush.HasType && toPush.ExpressionType.IsPinned)
            //{
            //    //pinned type, but not pinned pointer
            //    Push(new UnaryExpression(UnaryOperator.AddressOf, toPush, new Instruction[] { instruction }));
            //}
            //else
            //{
            //    ///Native int pointers are represented as void* pointers
            //    TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            //    Push(new CastExpression(toPush, voidPtr, instruction));
            //}
        }

        private bool IsIntegerType(TypeReference expressionType)
        {
            string typeName = expressionType.FullName;
            if (typeName == methodContext.Method.Module.TypeSystem.Byte.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.SByte.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.Int16.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.UInt16.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.Int32.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.UInt16.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.Int64.FullName ||
                typeName == methodContext.Method.Module.TypeSystem.UInt64.FullName
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the conv.i1 instruction. For more information on conv.i1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.i1 instruction.</param>
        public override void OnConv_I1(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.SByte, instruction);
        }

        /// <summary>
        /// Handles the conv.i2 instruction. For more information on conv.i2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.i2 instruction.</param>
        public override void OnConv_I2(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int16, instruction);
        }

        /// <summary>
        /// Handles the conv.i4 instruction. For more information on conv.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.i4 instruction.</param>
        public override void OnConv_I4(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int32, instruction);
        }

        /// <summary>
        /// Handles the conv.i8 instruction. For more information on conv.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.i8 instruction.</param>
        public override void OnConv_I8(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int64, instruction);
        }

        /// <summary>
        /// Handles the conv.u instruction. For more information on conv.u, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.u instruction.</param>
        public override void OnConv_U(Instruction instruction)
        {
            ///No difference is made between signed and unsigned native ints, as both of them are mostly used as pointers
            OnConv_I(instruction);
        }

        /// <summary>
        /// Handles the conv.u1 instruction. For more information on conv.u1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.u1 instruction.</param>
        public override void OnConv_U1(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Byte, instruction);
        }

        /// <summary>
        /// Handles the conv.u2 instruction. For more information on conv.u2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.u2 instruction.</param>
        public override void OnConv_U2(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt16, instruction);
        }

        /// <summary>
        /// Handles the conv.u4 instruction. For more information on conv.u4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.u4 instruction.</param>
        public override void OnConv_U4(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt32, instruction);
        }

        /// <summary>
        /// Handles the conv.u8 instruction. For more information on conv.u8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.u8 instruction.</param>
        public override void OnConv_U8(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt64, instruction);
        }

        /// <summary>
        /// Handles the conv.r.un instruction. For more information on conv.r.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.r.un instruction.</param>
        public override void OnConv_R_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Single, instruction);
        }

        /// <summary>
        /// Handles the conv.r4 instruction. For more information on conv.r4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.r4 instruction.</param>
        public override void OnConv_R4(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Single, instruction);
        }

        /// <summary>
        /// Handles the conv.r8 instruction. For more information on conv.r8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.r8 instruction.</param>
        public override void OnConv_R8(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Double, instruction);
        }

        /// <summary>
        /// Handles the arglist instruction. For more information on arglist, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The arglist instruction.</param>
        public override void OnArglist(Instruction instruction)
        {
            TypeReference typeReference = Import(typeof(RuntimeArgumentHandle));
            ParameterDefinition parameterRef = new ParameterDefinition("__arglist", Mono.Cecil.ParameterAttributes.Unused, typeReference);

            methodContext.Method.Parameters.Add(parameterRef);

            PushArgumentReference(parameterRef);
        }

        /// <summary>
        /// Handles the ldind.ref instruction. For more information on ldind.ref, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.ref instruction.</param>
        public override void OnLdind_Ref(Instruction instruction)
        {
            OnLdind(methodContext.Method.Module.TypeSystem.Object, instruction);
        }

        /// <summary>
        /// Handles the ldind.* instructions.
        /// </summary>
        /// <param name="type">The type, to which is being loaded indirectly.</param>
        private void OnLdind(TypeReference type, Instruction instruction)
        {
            Expression toDereference = Pop();

            UnaryExpression addressDereference = new UnaryExpression(UnaryOperator.AddressDereference, toDereference, IncludePrefixIfPresent(instruction, Code.Volatile));
            if (!addressDereference.HasType ||
                ((addressDereference.ExpressionType.FullName != "System.Boolean") &&
                (addressDereference.ExpressionType.FullName != type.FullName && type.FullName != "System.Object")))
            {
                ExplicitCastExpression resultingCastExpression = new ExplicitCastExpression(addressDereference, type, null);
                Push(resultingCastExpression);
            }
            else
            {
                Push(addressDereference);
            }
        }

        private IEnumerable<Instruction> IncludePrefixIfPresent(Instruction instruction, Code prefixCode)
        {
            if (instruction.Previous != null && instruction.Previous.OpCode.Code == prefixCode)
            {
                return new Instruction[] { instruction.Previous, instruction };
            }

            return new Instruction[] { instruction };
        }

        /// <summary>
        /// Handles the ldind.i instruction. For more information on ldind.i, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.i instruction.</param>
        public override void OnLdind_I(Instruction instruction)
        {
            TypeReference voidPtrType = new PointerType(currentTypeSystem.Void);
            OnLdind(voidPtrType, instruction);
        }

        /// <summary>
        /// Handles the ldind.i1 instruction. For more information on ldind.i1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.i1 instruction.</param>
        public override void OnLdind_I1(Instruction instruction)
        {
            OnLdind(currentTypeSystem.SByte, instruction);
        }

        /// <summary>
        /// Handles the ldind.i2 instruction. For more information on ldind.i2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.i2 instruction.</param>
        public override void OnLdind_I2(Instruction instruction)
        {
            OnLdind(currentTypeSystem.Int16, instruction);
        }

        /// <summary>
        /// Handles the ldind.i4 instruction. For more information on ldind.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.i4 instruction.</param>
        public override void OnLdind_I4(Instruction instruction)
        {
            OnLdind(currentTypeSystem.Int32, instruction);
        }

        /// <summary>
        /// Handles the ldind.i8 instruction. For more information on ldind.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.i8 instruction.</param>
        public override void OnLdind_I8(Instruction instruction)
        {
            OnLdind(currentTypeSystem.Int64, instruction);
        }

        /// <summary>
        /// Handles the ldind.u1 instruction. For more information on ldind.u1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.u1 instruction.</param>
        public override void OnLdind_U1(Instruction instruction)
        {
            OnLdind(currentTypeSystem.Byte, instruction);
        }

        /// <summary>
        /// Handles the ldind.u2 instruction. For more information on ldind.u2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.u2 instruction.</param>
        public override void OnLdind_U2(Instruction instruction)
        {
            OnLdind(currentTypeSystem.UInt16, instruction);
        }

        /// <summary>
        /// Handles the ldind.u4 instruction. For more information on ldind.u4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.u4 instruction.</param>
        public override void OnLdind_U4(Instruction instruction)
        {
            OnLdind(currentTypeSystem.UInt32, instruction);
        }

        /// <summary>
        /// Handles the ldind.r4 instruction. For more information on ldind.r4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.r4 instruction.</param>
        public override void OnLdind_R4(Instruction instruction)
        {
            OnLdind(currentTypeSystem.Single, instruction);
        }

        /// <summary>
        /// Handles the ldind.r8 instruction. For more information on ldind.r8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldind.r8 instruction.</param>
        public override void OnLdind_R8(Instruction instruction)
        {
            OnLdind(currentTypeSystem.Double, instruction);
        }

        /// <summary>
        /// Handles the ldobj instruction. For more information on ldobj, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldobj instruction.</param>
        public override void OnLdobj(Instruction instruction)
        {
            Expression argument = Pop();
            argument = new UnaryExpression(UnaryOperator.AddressDereference, argument, instruction != null ? IncludePrefixIfPresent(instruction, Code.Volatile) : null);
            Push(argument);
        }

        /// <summary>
        /// Handles the neg instruction. For more information on neg, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The neg instruction.</param>
        public override void OnNeg(Instruction instruction)
        {
            Push(new UnaryExpression(UnaryOperator.Negate, Pop(), new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the stobj instruction. For more information on stobj, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The stobj instruction.</param>
        public override void OnStobj(Instruction instruction)
        {
            Expression value = Pop();
            Expression destination = Pop();

            destination = new UnaryExpression(UnaryOperator.AddressDereference, destination, null);

            PushAssignment(destination, value, IncludePrefixIfPresent(instruction, Code.Volatile));
        }

        /// <summary>
        /// Handles the ldftn instruction. For more information on ldftn, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldftn instruction.</param>
        public override void OnLdftn(Instruction instruction)
        {
            //if the method expects object as target, then it's up to the calling function to provide it
            Push(new MethodReferenceExpression(null, (MethodReference)instruction.Operand, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the ldvirtftn instruction. For more information on ldvirtftn, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldvirtftn instruction.</param>
        public override void OnLdvirtftn(Instruction instruction)
        {
            Expression target = Pop();
            if (IsPointerType(target))
            {
                target = new UnaryExpression(UnaryOperator.AddressDereference, target, new Instruction[] { instruction });
            }
            Push(new MethodReferenceExpression(target, (MethodReference)instruction.Operand, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the volatile instruction. For more information on volatile, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The volatile instruction.</param>
        public override void OnVolatile(Instruction instruction)
        {
            //TODO: implement logic
        }

        /// <summary>
        /// Handles the localloc instruction. For more information on localloc, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The localloc instruction.</param>
        public override void OnLocalloc(Instruction instruction)
        {
            Push(new StackAllocExpression(Pop(), new PointerType(methodContext.Method.Module.TypeSystem.IntPtr), new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the ceq instruction. For more information on ceq, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ceq instruction.</param>
        public override void OnCeq(Instruction instruction)
        {
            // XXX: ceq might be used for reference equality as well
            Expression right = Pop();
            Expression left = Pop();

            BinaryExpression comparison = new BinaryExpression(BinaryOperator.ValueEquality, left, right, currentTypeSystem, new Instruction[] { instruction });
            comparison.ExpressionType = methodContext.Method.Module.TypeSystem.Boolean;
            Push(comparison);
        }

        /// <summary>
        /// Handles the clt instruction. For more information on clt, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The clt instruction.</param>
        public override void OnClt(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LessThan, instruction);
        }

        /// <summary>
        /// Handles the clt.un instruction. For more information on clt.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The clt.un instruction.</param>
        public override void OnClt_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LessThan, instruction);
        }

        /// <summary>
        /// Handles the cgt instruction. For more information on cgt, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The cgt instruction.</param>
        public override void OnCgt(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
        }

        /// <summary>
        /// Handles the cgt.un instruction. For more information on cgt.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The cgt.un instruction.</param>
        public override void OnCgt_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
        }

        /// <summary>
        /// Handles the beq instruction. For more information on beq, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The beq instruction.</param>
        public override void OnBeq(Instruction instruction)
        {
            OnCeq(instruction);
        }

        /// <summary>
        /// Handles the bne.un instruction. For more information on bne.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The bne.un instruction.</param>
        public override void OnBne_Un(Instruction instruction)
        {
            //Expression right = Pop();
            //Expression left = Pop();
            //BinaryExpression comparison = new BinaryExpression(BinaryOperator.ValueInequality, left, right, currentTypeSystem, instruction);
            //comparison.ExpressionType = methodContext.Method.Module.TypeSystem.Boolean;
            //Push(comparison);
            PushBinaryExpression(BinaryOperator.ValueInequality, instruction);
        }

        /// <summary>
        /// Handles the ble instruction. For more information on ble, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ble instruction.</param>
        public override void OnBle(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LessThanOrEqual, instruction);
        }

        /// <summary>
        /// Handles the ble.un instruction. For more information on ble.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ble.un instruction.</param>
        public override void OnBle_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LessThanOrEqual, instruction);
        }

        /// <summary>
        /// Handles the bgt instruction. For more information on bgt, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The bgt instruction.</param>
        public override void OnBgt(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
        }

        /// <summary>
        /// Handles the bgt.un instruction. For more information on bgt.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The bgt.un instruction.</param>
        public override void OnBgt_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.GreaterThan, instruction);
        }

        /// <summary>
        /// Handles the bge instruction. For more information on bge, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The bge instruction.</param>
        public override void OnBge(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.GreaterThanOrEqual, instruction);
        }

        /// <summary>
        /// Handles the bge.un instruction. For more information on bge.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The bge.un instruction.</param>
        public override void OnBge_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.GreaterThanOrEqual, instruction);
        }

        /// <summary>
        /// Handles the blt instruction. For more information on blt, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The blt instruction.</param>
        public override void OnBlt(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LessThan, instruction);
        }

        /// <summary>
        /// Handles the blt.un instruction. For more information on blt.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The blt.un instruction.</param>
        public override void OnBlt_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LessThan, instruction);
        }

        /// <summary>
        /// Handles the shr instruction. For more information on shr, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The shr instruction.</param>
        public override void OnShr(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.RightShift, instruction);
        }

        /// <summary>
        /// Handles the shr.un instruction. For more information on shr.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The shr.un instruction.</param>
        public override void OnShr_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.RightShift, instruction);
        }

        /// <summary>
        /// Handles the shl instruction. For more information on shl, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The shl instruction.</param>
        public override void OnShl(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.LeftShift, instruction);
        }

        /// <summary>
        /// Handles the or instruction. For more information on or, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The or instruction.</param>
        public override void OnOr(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.BitwiseOr, instruction);
        }

        /// <summary>
        /// Handles the and instruction. For more information on and, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The and instruction.</param>
        public override void OnAnd(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.BitwiseAnd, instruction);
        }

        /// <summary>
        /// Handles the xor instruction. For more information on xor, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The xor instruction.</param>
        public override void OnXor(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.BitwiseXor, instruction);
        }

        /// <summary>
        /// Handles the rem instruction. For more information on rem, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The rem instruction.</param>
        public override void OnRem(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Modulo, instruction);
        }

        /// <summary>
        /// Handles the rem.un instruction. For more information on rem.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The rem.un instruction.</param>
        public override void OnRem_Un(Instruction instruction)
        {
            PushBinaryExpression(BinaryOperator.Modulo, instruction);
        }

        /// <summary>
        /// Handles the not instruction. For more information on not, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The not instruction.</param>
        public override void OnNot(Instruction instruction)
        {
            UnaryExpression expr = new UnaryExpression(UnaryOperator.BitwiseNot, Pop(), new Instruction[] { instruction });
            Push(expr);
        }

        /// <summary>
        /// Handles the brtrue and brtrue.s instructions. For more information on brtrue and brtrue.s, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The brtrue or brtrue.s instruction.</param>
        public override void OnBrtrue(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the brfalse and brfalse.s instructions. For more information on brfalse and brfalse.s, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The brfalse or brfalse.s instruction.</param>
        public override void OnBrfalse(Instruction instruction)
        {
        }

        /// <summary>
        /// Handles the switch instruction. For more information on switch, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The switch instruction.</param>
        public override void OnSwitch(Instruction instruction)
        {
        }

        /// <summary>
        /// Pushes ArgumentReferenceExpression on the expression stack.
        /// </summary>
        /// <param name="parameter">The argument that is refered to.</param>
        private void PushArgumentReference(ParameterReference parameter)
        {
            Push(new ArgumentReferenceExpression(parameter, new Instruction[] { CurrentInstruction }));
        }

        /// <summary>
        /// Handles the ldfld instruction. For more information on ldfld, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldfld instruction.</param>
        public override void OnLdfld(Instruction instruction)
        {
            PushFieldReference(instruction, Pop());
        }

        /// <summary>
        /// Handles the ldsfld instruction. For more information on ldsfld, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldsfld instruction.</param>
        public override void OnLdsfld(Instruction instruction)
        {
            PushFieldReference(instruction);
        }

        /// <summary>
        /// Pushes reference to the field, specified by the instruction.
        /// </summary>
        /// <param name="instruction">The instruction that pushes the field onto the execution stack.</param>
        private void PushFieldReference(Instruction instruction)
        {
            PushFieldReference(instruction, null);
        }

        /// <summary>
        /// Pushes reference to the field of the suplied target, specified by the instruction.
        /// </summary>
        /// <param name="instruction">The instruction that pushes the field onto the execution stack.</param>
        /// <param name="target">The targeted object, which contains the field.</param>
        private void PushFieldReference(Instruction instruction, Expression target)
        {
            EventDefinition eventDef;
            FieldDefinition fieldDef = (instruction.Operand as FieldReference).Resolve();
            if (fieldDef != null && fieldDef.DeclaringType == methodContext.Method.DeclaringType && methodContext.EnableEventAnalysis &&
                typeContext.GetFieldToEventMap(this.context.Language).TryGetValue(fieldDef, out eventDef))
            {
                Push(new EventReferenceExpression(target, eventDef, IncludePrefixIfPresent(instruction, Code.Volatile)));
            }
            else
            {
                if (target != null)
                {
                    if (IsPointerType(target))
                    {
                        target = new UnaryExpression(UnaryOperator.AddressDereference, target, null);
                    }
                }
                Push(new FieldReferenceExpression(target, (FieldReference)instruction.Operand, IncludePrefixIfPresent(instruction, Code.Volatile)));
            }
        }

        /// <summary>
        /// Handles the ldflda instruction. For more information on ldflda, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldflda instruction.</param>
        public override void OnLdflda(Instruction instruction)
        {
            Expression target = Pop();
            UnaryOperator @operator = UnaryOperator.AddressReference;
            if (IsUnmanagedPointerType(target))
            {
                @operator = UnaryOperator.AddressOf;
            }
            else
            {
                @operator = UnaryOperator.AddressReference;
            }
            PushFieldReference(instruction, target);
            UnaryExpression reference = new UnaryExpression(@operator, Pop(), null);
            Push(reference);
        }

        private bool IsUnmanagedPointerType(Expression ex)
        {
            if (ex == null)
            {
                return false;
            }
            if (!ex.HasType)
            {
                return false;
            }
            if (ex.ExpressionType.IsPointer)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the ldsflda instruction. For more information on ldsflda, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldsflda instruction.</param>
        public override void OnLdsflda(Instruction instruction)
        {
            PushFieldReference(instruction);
            Expression fieldRef = Pop();
            Push(new UnaryExpression(UnaryOperator.AddressReference, fieldRef, null));
        }

        /// <summary>
        /// Handles the ldnull instruction. For more information on ldnull, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldnull instruction.</param>
        public override void OnLdnull(Instruction instruction)
        {
            PushLiteral(null, instruction);
        }

        /// <summary>
        /// Handles the ldarg.0 instruction. For more information on ldarg.0, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldarg.0 instruction.</param>
        public override void OnLdarg_0(Instruction instruction)
        {
            PushArgumentReference(0, instruction);
        }

        /// <summary>
        /// Handles the ldarg.1 instruction. For more information on ldarg.1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldarg.1 instruction.</param>
        public override void OnLdarg_1(Instruction instruction)
        {
            PushArgumentReference(1, instruction);
        }

        /// <summary>
        /// Handles the ldarg.2 instruction. For more information on ldarg.2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldarg.2 instruction.</param>
        public override void OnLdarg_2(Instruction instruction)
        {
            PushArgumentReference(2, instruction);
        }

        /// <summary>
        /// Handles the ldarg.3 instruction. For more information on ldarg.3, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldarg.3 instruction.</param>
        public override void OnLdarg_3(Instruction instruction)
        {
            PushArgumentReference(3, instruction);
        }

        /// <summary>
        /// Handles the ldarg instruction. For more information on ldarg, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldarg instruction.</param>
        public override void OnLdarg(Instruction instruction)
        {
            ParameterDefinition parameter = (ParameterDefinition)instruction.Operand;
            if (parameter == methodContext.Method.Body.ThisParameter) // happens when instead of ldarg.0 is used ldarg 0 in an instance method
            {
                Push(new ThisReferenceExpression(methodContext.Method.DeclaringType, new Instruction[] { instruction }));
                return;
            }
            PushArgumentReference(parameter);
        }

        /// <summary>
        /// Handles the ldarga instruction. For more information on ldarga, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldarga instruction.</param>
        public override void OnLdarga(Instruction instruction)
        {
            ArgumentReferenceExpression operand = new ArgumentReferenceExpression((ParameterDefinition)instruction.Operand, new Instruction[] { instruction });
            UnaryExpression unary = new UnaryExpression(UnaryOperator.AddressReference, operand, null);
            Push(unary);
            //PushArgumentReference(((ParameterDefinition)instruction.Operand));
            //PushAddressOf();
        }

        /// <summary>
        /// Handles the constrained instruction. For more information on constrained, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The constrained instruction.</param>
        public override void OnConstrained(Instruction instruction)
        {
            // NOTE: Should be implemented properly.
        }

        /// <summary>
        /// Handles the conv.ovf.i instruction. For more information on conv.ovf.i, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i instruction.</param>
        public override void OnConv_Ovf_I(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.IntPtr, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.i1 instruction. For more information on conv.ovf.i1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i1 instruction.</param>
        public override void OnConv_Ovf_I1(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.SByte, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.i2 instruction. For more information on conv.ovf.i2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i2 instruction.</param>
        public override void OnConv_Ovf_I2(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int16, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.i4 instruction. For more information on conv.ovf.i4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i4 instruction.</param>
        public override void OnConv_Ovf_I4(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int32, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.i8 instruction. For more information on conv.ovf.i8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i8 instruction.</param>
        public override void OnConv_Ovf_I8(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int64, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.u instruction. For more information on conv.ovf.u, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u instruction.</param>
        public override void OnConv_Ovf_U(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UIntPtr, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.u1 instruction. For more information on conv.ovf.u1, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u1 instruction.</param>
        public override void OnConv_Ovf_U1(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Byte, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.u2 instruction. For more information on conv.ovf.u2, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u2 instruction.</param>
        public override void OnConv_Ovf_U2(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt16, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.u4 instruction. For more information on conv.ovf.u4, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u4 instruction.</param>
        public override void OnConv_Ovf_U4(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt32, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.u8 instruction. For more information on conv.ovf.u8, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u8 instruction.</param>
        public override void OnConv_Ovf_U8(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt64, instruction);
        }

        /// <summary>
        /// Handles the conv.ovf.i1.un instruction. For more information on conv.ovf.i1.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i1.un instruction.</param>
        public override void OnConv_Ovf_I1_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.SByte, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.i2.un instruction. For more information on conv.ovf.i2.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i2.un instruction.</param>
        public override void OnConv_Ovf_I2_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int16, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.i4.un instruction. For more information on conv.ovf.i4.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i4.un instruction.</param>
        public override void OnConv_Ovf_I4_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int32, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.i8.un instruction. For more information on conv.ovf.i8.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i8.un instruction.</param>
        public override void OnConv_Ovf_I8_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Int64, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.u1.un instruction. For more information on conv.ovf.u1.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u1.un instruction.</param>
        public override void OnConv_Ovf_U1_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.Byte, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.u2.un instruction. For more information on conv.ovf.u2.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u2.un instruction.</param>
        public override void OnConv_Ovf_U2_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt16, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.u4.un instruction. For more information on conv.ovf.u4.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u4.un instruction.</param>
        public override void OnConv_Ovf_U4_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt32, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.u8.un instruction. For more information on conv.ovf.u8.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u8.un instruction.</param>
        public override void OnConv_Ovf_U8_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UInt64, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.i.un instruction. For more information on conv.ovf.i.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.i.un instruction.</param>
        public override void OnConv_Ovf_I_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.IntPtr, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the conv.ovf.u.un instruction. For more information on conv.ovf.u.un, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The conv.ovf.u.un instruction.</param>
        public override void OnConv_Ovf_U_Un(Instruction instruction)
        {
            PushCastExpression(methodContext.Method.Module.TypeSystem.UIntPtr, instruction);
            TypeReference voidPtr = new PointerType(currentTypeSystem.Void);
            PushCastExpression(voidPtr, null);
        }

        /// <summary>
        /// Handles the cpobj instruction. For more information on cpobj, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The cpobj instruction.</param>
        public override void OnCpobj(Instruction instruction)
        {
            OnLdobj(null);
            OnStobj(instruction);
        }

        /// <summary>
        /// Handles the initblk instruction. For more information on initblk, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The initblk instruction.</param>
        public override void OnInitblk(Instruction instruction)
        {
            // TODO: Should check whether we need to push an expression for this instruction in some cases.
            // This instruction can have the volatile. prefix
        }

        /// <summary>
        /// Handles the ldtoken instruction. For more information on ldtoken, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The ldtoken instruction.</param>
        public override void OnLdtoken(Instruction instruction)
        {
            ///Tokens in IL are one of the following:
            ///1. Reference to a type
            ///2. Reference to a method
            ///3. Reference to a field

            MemberReference memberRef = instruction.Operand as MemberReference;
            if (memberRef == null)
            {
                throw new NotSupportedException();
            }

            Push(new MemberHandleExpression(memberRef, new Instruction[] { instruction }));
        }

        /// <summary>
        /// Handles the mkrefany instruction. For more information on mkrefany, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The mkrefany instruction.</param>
        public override void OnMkrefany(Instruction instruction)
        {
            Push(new MakeRefExpression(Pop(), (TypeReference)instruction.Operand, new Instruction[] { instruction }));
        }

        ///// <summary>
        ///// Creates UnaryExpression and pushes it onto the expressionStack.
        ///// </summary>
        ///// <param name="op">The unary operation.</param>
        ///// <param name="expression">The operand.</param>
        //private void PushUnaryExpression(UnaryOperator op, Expression expression, Instruction instruction)
        //{
        //    Push(new UnaryExpression(op, expression, new Instruction[] { instruction }));
        //}

        /// <summary>
        /// Creates ArgumentReferenceExpression and pushes it onto the stack.
        /// </summary>
        /// <param name="index">The index of the argument.</param>
        private void PushArgumentReference(int index, Instruction instruction)
        {
            if (methodContext.Method.HasThis)
            {
                if (index == 0)
                {
                    Push(new ThisReferenceExpression(methodContext.Method.DeclaringType, new Instruction[] { instruction }));
                    return;
                }
                index -= 1; // the Parameters collection does not contain the implict this argument
            }
            Push(new ArgumentReferenceExpression(methodContext.Method.Parameters[index], new Instruction[] { CurrentInstruction }));
        }

        /// <summary>
        /// Creates new LIteralExpression and pushes it onto the stack.
        /// </summary>
        /// <param name="value">The value of the literal expression.</param>
        private void PushLiteral(object value, Instruction instruction)
        {
            LiteralExpression toPush = new LiteralExpression(value, currentTypeSystem, new Instruction[] { instruction });

            ///All values, taking less space than 4 bytes are treated as integers (int32).
            ///Thus, at this point there is no difference between them.
            ///Fine tuning of the type of the literal expression is done by TypeInferer and IntegerTypeInferer.
            if (toPush.Value == null)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.Object;
            }
            else if (toPush.Value is int)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.Int32;
            }
            else if (toPush.Value is string)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.String;
            }
            else if (toPush.Value is float)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.Single;
            }
            else if (toPush.Value is double)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.Double;
            }
            else if (toPush.Value is long)
            {
                toPush.ExpressionType = methodContext.Method.Module.TypeSystem.Int64;
            }
            Push(toPush);
        }

        /// <summary>
        /// Pushes the expression on the evaluation stack. Also populates the offsetToExpression dictionary.
        /// </summary>
        /// <param name="expression">The expression to be pushed.</param>
        public void Push(Expression expression)
        {
            if (null == expression)
            {
                throw new ArgumentNullException("expression");
            }

            expressionStack.Push(expression);
        }

        /// <summary>
        /// Pops an expression from the expressionStack. Marks the expression as used.
        /// </summary>
        /// <returns>The popped expression.</returns>
        private Expression Pop()
        {
            Expression result = expressionStack.Pop();
            used.Add(result);
            ///Marks if an exceptionVariable has been used.
            ///Unused variables are cleaned after the whole method is processed
            if (result is VariableReferenceExpression)
            {
                VariableReference variable = (result as VariableReferenceExpression).Variable;
                if (exceptionVariables.ContainsKey(variable))
                {
                    KeyValuePair<int, bool> oldPair = exceptionVariables[variable];
                    exceptionVariables[variable] = new KeyValuePair<int, bool>(oldPair.Key, true);
                }
            }
            if (result is VariableDeclarationExpression)
            {
                VariableReference variable = (result as VariableDeclarationExpression).Variable;
                if (exceptionVariables.ContainsKey(variable))
                {
                    KeyValuePair<int, bool> oldPair = exceptionVariables[variable];
                    exceptionVariables[variable] = new KeyValuePair<int, bool>(oldPair.Key, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Extracts the arguments needed for given method invocation.
        /// </summary>
        /// <param name="method">The method invocation.</param>
        /// <returns>Collection of the ordered arguments.</returns>
        private ExpressionCollection ProcessArguments(MethodReference method)
        {
            ExpressionCollection range = new ExpressionCollection();

            int paramLen = method.Parameters.Count;

            ///Arguments are on the stack in backwards order. That is, the last argument is on top, the second to last is below it and so on.
            for (int index = paramLen - 1; index >= 0; index--)
            {
                range.Insert(0, Pop());
            }

            return range;
        }

        /// <summary>
        /// Peeks at the expressionStack.
        /// </summary>
        /// <returns>The expression on top of the ExpressionStack</returns>
        private Expression Peek()
        {
            return expressionStack.Peek();
        }

        /// <summary>
        /// Handles the break instruction. For more information on break, see <see cref="Ecma-355.pdf"/> in DecompilationPapers.
        /// </summary>
        /// <param name="instruction">The break instruction.</param>
        public override void OnBreak(Instruction instruction)
        {
        }

        /// <summary>
        /// Takes care of InstructionBlocks generating 'yield return' and 'yield break' expressions.
        /// </summary>
        /// <param name="theBlock">The IL Block generating the yield expressions.</param>
        /// <param name="blockExpressions">The list of expressions resulting from the block.</param>
        private void ProcessYieldBlock(InstructionBlock theBlock, List<Expression> blockExpressions)
        {
            int expressionsCount = blockExpressions.Count;

            if (methodContext.YieldData.YieldBreaks.Contains(theBlock))
            {
                if (blockExpressions[expressionsCount - 1].CodeNodeType == CodeNodeType.ReturnExpression)
                {
                    Instruction underlyingInstruction = (blockExpressions[expressionsCount - 1] as ReturnExpression).UnderlyingSameMethodInstructions.Last();
                    // the return instruction should always be the last one
                    blockExpressions[expressionsCount - 1] = new YieldBreakExpression(new Instruction[] { underlyingInstruction });
                }
                else
                {
                    throw new Exception("No return at the end of yield break block");
                }
            }
            else if (methodContext.YieldData.YieldReturns.Contains(theBlock))
            {
                Expression returnValue = GetYieldReturnItem(blockExpressions);
                int fieldAssignmentIndex = GetYieldReturnAssignmentIndex(blockExpressions);

                blockExpressions[fieldAssignmentIndex] = new YieldReturnExpression(returnValue, blockExpressions[fieldAssignmentIndex].UnderlyingSameMethodInstructions);

                expressionsCount = blockExpressions.Count;
                if (blockExpressions[expressionsCount - 1].CodeNodeType == CodeNodeType.ReturnExpression)
                {
                    blockExpressions.RemoveAt(expressionsCount - 1);
                }
            }
        }

        /// <summary>
        /// Returns the field, that is the value of 'yield return' expression
        /// </summary>
        /// <param name="blockExpressions">The block of expressions, produced for the yield return block.</param>
        /// <returns>The expression, argument of the yield return.</returns>
        private Expression GetYieldReturnItem(List<Expression> blockExpressions)
        {
            FieldReference currentItemField = methodContext.YieldData.FieldsInfo.CurrentItemField;
            foreach (Expression expression in blockExpressions)
            {
                if (expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expression as BinaryExpression).IsAssignmentExpression)
                {
                    continue;
                }

                FieldReferenceExpression fieldRefExpression = (expression as BinaryExpression).Left as FieldReferenceExpression;


                if (fieldRefExpression != null && fieldRefExpression.Field.Resolve() == currentItemField)
                {
                    return (expression as BinaryExpression).Right;
                }
            }

            throw new Exception("No assignment of the current field");
        }

        private int GetYieldReturnAssignmentIndex(List<Expression> blockExpressions)
        {
            FieldReference currentItemField = methodContext.YieldData.FieldsInfo.CurrentItemField;

            int index = 0;
            foreach (Expression expression in blockExpressions)
            {
                if (expression.CodeNodeType != CodeNodeType.BinaryExpression || !(expression as BinaryExpression).IsAssignmentExpression)
                {
                    index++;
                    continue;
                }

                FieldReferenceExpression fieldRefExpression = (expression as BinaryExpression).Left as FieldReferenceExpression;

                if (fieldRefExpression != null && fieldRefExpression.Field.Resolve() == currentItemField)
                {
                    break;
                }
                index++;
            }

            if (index == blockExpressions.Count)
            {
                throw new Exception("No assignment of the current field");
            }

            return index;
        }
    }
}