using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildCatchClausesFilterStep : BaseCodeTransformer, IDecompilationStep
    {
        // A metadata token is 4-byte unsigned integer. The most significant byte carries the table index.
        // The other 3 bytes are left for RID. The max value of 3-byte number is 16777215.
        private const uint MaxRID = 16777215;

        private DecompilationContext context;
        private CatchClause currentCatchClause;
        private HashSet<VariableDefinition> variablesUsedOutsideFilters;
        private Dictionary<CatchClause, HashSet<VariableDefinition>> catchClausesUsedVariablesMap;
        private Dictionary<CatchClause, HashSet<ParameterDefinition>> catchClausesUsedParametersMap;
        private Dictionary<CatchClause, Dictionary<VariableDefinition, ParameterDefinition>> catchClausesVariablesToParametersMap;
        private List<FilterMethodToBeDecompiled> methodsToBeDecompiled;

        public RebuildCatchClausesFilterStep()
        {
            this.currentCatchClause = null;
            this.variablesUsedOutsideFilters = new HashSet<VariableDefinition>();
            this.catchClausesUsedVariablesMap = new Dictionary<CatchClause, HashSet<VariableDefinition>>();
            this.catchClausesUsedParametersMap = new Dictionary<CatchClause, HashSet<ParameterDefinition>>();
            this.catchClausesVariablesToParametersMap = new Dictionary<CatchClause, Dictionary<VariableDefinition, ParameterDefinition>>();
            this.methodsToBeDecompiled = new List<FilterMethodToBeDecompiled>();
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            if (!context.MethodContext.Body.ExceptionHandlers.Any(eh => eh.HandlerType == ExceptionHandlerType.Filter))
            {
                return body;
            }

            this.context = context;
            body = (BlockStatement)Visit(body);
            RemoveVariablesUsedOnlyInFilters();
            DecompileMethods();
            return body;
        }

        public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            AddReferencedVariable(node.Variable.Resolve());
            return node;
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            AddReferencedVariable(node.Variable.Resolve());
            return node;
        }

        public override ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
        {
            AddReferencedParameter(node.Parameter.Resolve());
            return node;
        }

        public override ICodeNode VisitCatchClause(CatchClause node)
        {
            // We first visit the body and the filter
            node.Body = (BlockStatement)base.Visit(node.Body);

            if (node.Filter == null || !(node.Filter is BlockStatement))
            {
                return node;
            }

            currentCatchClause = node;
            node.Filter = (Statement)base.Visit(node.Filter);
            currentCatchClause = null;

            VariableDeclarationExpression variableDeclaration;
            Expression filterExpression;
            bool matchSucceed = CatchClausesFilterPattern.TryMatch(node.Filter as BlockStatement, out variableDeclaration, out filterExpression);
            if (!matchSucceed)
            {
                if (variableDeclaration == null || variableDeclaration.ExpressionType.FullName == Constants.Object ||
                    !CatchClausesFilterPattern.TryMatchMethodStructure(node.Filter as BlockStatement))
                {
                    throw new NotSupportedException("Unsupported structure of filter clause.");
                }

                CreateMethod(node, variableDeclaration, out filterExpression);
            }

            this.context.MethodContext.VariablesToNotDeclare.Add(variableDeclaration.Variable);

            // If the pattern matches then there is no additional method and the variable declaration is used as is.
            // If the pattern doesn't match - we use the expressions only, because if we don't, the result is
            // two expressions (one in the catch variable and one in the generated method) with the same instructions.
            if (matchSucceed)
            {
                node.Variable = variableDeclaration;
            }
            else
            {
                node.Variable = variableDeclaration.CloneExpressionOnly() as VariableDeclarationExpression;
            }

            node.Type = variableDeclaration.ExpressionType;
            node.Filter = new ExpressionStatement(filterExpression);

            return node;
        }

        private void DecompileMethods()
        {
            foreach (FilterMethodToBeDecompiled method in methodsToBeDecompiled)
            {
                AddVariablesToNotDeclare(method.Context, method.CatchClause);

                BlockDecompilationPipeline pipeline = this.context.Language.CreateFilterMethodPipeline(method.Context);
                DecompilationContext innerContext = pipeline.Run(method.Method.Body, method.Block, this.context.Language);
                this.context.TypeContext.GeneratedFilterMethods.Add(new GeneratedMethod(method.Method, pipeline.Body, innerContext.MethodContext));
                this.context.TypeContext.GeneratedMethodDefinitionToNameMap.Add(method.Method, method.Method.Name);

                // The following line must be here, after the decompilation, because it uses the renamed nameless parameters.
                FixVariablesNames(innerContext, method.CatchClause);
            }
        }

        private void AddReferencedVariable(VariableDefinition variableDefinition)
        {
            if (this.currentCatchClause != null)
            {
                if (!this.catchClausesUsedVariablesMap.ContainsKey(this.currentCatchClause))
                {
                    this.catchClausesUsedVariablesMap.Add(this.currentCatchClause, new HashSet<VariableDefinition>());
                }

                this.catchClausesUsedVariablesMap[this.currentCatchClause].Add(variableDefinition);
            }
            else
            {
                this.variablesUsedOutsideFilters.Add(variableDefinition);
            }
        }

        private void AddReferencedParameter(ParameterDefinition parameterDefinition)
        {
            if (this.currentCatchClause != null)
            {
                if (!this.catchClausesUsedParametersMap.ContainsKey(this.currentCatchClause))
                {
                    this.catchClausesUsedParametersMap.Add(this.currentCatchClause, new HashSet<ParameterDefinition>());
                }

                this.catchClausesUsedParametersMap[this.currentCatchClause].Add(parameterDefinition);
            }
        }

        private void CreateMethod(CatchClause catchClause, VariableDeclarationExpression variable, out Expression methodInvocationExpression)
        {
            methodInvocationExpression = null;

            BlockStatement filter = catchClause.Filter as BlockStatement;

            // Fixes the variable definition in the first statement.
            ExpressionStatement firstStatement = filter.Statements.First() as ExpressionStatement;
            BinaryExpression assignment = firstStatement.Expression as BinaryExpression;
            SafeCastExpression safeCast = assignment.Right as SafeCastExpression;
            VariableReferenceExpression variableReference = safeCast.Expression as VariableReferenceExpression;
            variableReference.Variable = variable.Variable;

            // Replace the variable reference in the last statement with return statement.
            ExpressionStatement lastStatement = filter.Statements.Last() as ExpressionStatement;
            lastStatement.Expression = new ReturnExpression(lastStatement.Expression, lastStatement.Expression.MappedInstructions);

            int methodNumber = this.context.TypeContext.GeneratedFilterMethods.Count + this.methodsToBeDecompiled.Count;
            string methodName = string.Format(Constants.JustDecompileGeneratedFilterPattern, methodNumber);
            MethodDefinition method = new MethodDefinition(methodName, Mono.Cecil.MethodAttributes.Private, this.context.MethodContext.Method.Module.TypeSystem.Boolean);
            method.Body = new MethodBody(method);
            // Practically no chance for duplicates, because of decrementing the MaxRID
            method.MetadataToken = new MetadataToken(TokenType.Method, MaxRID - (uint)methodNumber);
            method.IsStatic = this.context.MethodContext.Method.IsStatic;
            method.HasThis = !method.IsStatic;
            method.DeclaringType = context.MethodContext.Method.DeclaringType;
            method.SemanticsAttributes = MethodSemanticsAttributes.None;
            method.IsJustDecompileGenerated = true;

            DecompilationContext newContext = new DecompilationContext(CloneAndReplaceMethodBody(this.context.MethodContext, method.Body), this.context.TypeContext, this.context.Language);

            VariableDefinition variableDefinition = variableReference.Variable.Resolve();
            if (!newContext.MethodContext.VariableDefinitionToNameMap.ContainsKey(variableDefinition))
            {
                newContext.MethodContext.VariableDefinitionToNameMap[variableDefinition] = Constants.JustDecompileGeneratedException;
            }
            else
            {
                newContext.MethodContext.VariableDefinitionToNameMap.Add(variableDefinition, Constants.JustDecompileGeneratedException);
            }

            newContext.MethodContext.VariablesToNotDeclare.Add(variable.Variable);

            methodsToBeDecompiled.Add(new FilterMethodToBeDecompiled(method, catchClause, newContext, catchClause.Filter as BlockStatement));

            List<Expression> parameters = AddAllParameters(catchClause, method, variable);
            methodInvocationExpression = CreateMethodInvocation(method, parameters);
        }

        private MethodSpecificContext CloneAndReplaceMethodBody(MethodSpecificContext context, MethodBody methodBody)
        {
            return new MethodSpecificContext(context.AnalysisResults,
                context.YieldData,
                context.AsyncData,
                context.IsMethodBodyChanged,
                new Dictionary<string, Statement>(context.GotoLabels),
                new List<GotoStatement>(context.GotoStatements),
                context.StackData,
                context.IsBaseConstructorInvokingConstructor,
                context.EnableEventAnalysis,
                methodBody,
                new Collection<VariableDefinition>(context.Variables),
                context.ControlFlowGraph,
                context.Expressions,
                context.LogicalConstructsTree,
                context.LogicalConstructsContext,
                context.CtorInvokeExpression,
                new Dictionary<Statement, ILogicalConstruct>(context.StatementToLogicalConstruct),
                new Dictionary<ILogicalConstruct, List<Statement>>(context.LogicalConstructToStatements),
                new Dictionary<VariableDefinition, string>(context.VariableDefinitionToNameMap),
                new HashSet<string>(context.VariableNamesCollection),
                new Dictionary<ParameterDefinition,string>(context.ParameterDefinitionToNameMap),
                new HashSet<VariableDefinition>(context.VariablesToRename),
                new Dictionary<FieldDefinition, Expression>(context.FieldToExpression),
                context.LambdaVariablesCount,
                new Dictionary<VariableDefinition, AssignmentType>(context.VariableAssignmentData),
                new List<ParameterDefinition>(context.OutParametersToAssign),
                context.IsDestructor,
                context.DestructorStatements,
                new HashSet<VariableDefinition>(context.UndeclaredLinqVariables),
                new Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>>(context.ClosureVariableToFieldValue),
                new HashSet<VariableDefinition>(context.VariablesToNotDeclare),
                context.SwitchByStringData.Clone() as CompilerOptimizedSwitchByStringData);
        }

        /// <summary>
        /// This method fixes all variables names that reffer to nameless parameter.
        /// </summary>
        private void FixVariablesNames(DecompilationContext innerContext, CatchClause currentCatch)
        {
            if (catchClausesVariablesToParametersMap.ContainsKey(currentCatch))
            {
                foreach (KeyValuePair<VariableDefinition, ParameterDefinition> pair in catchClausesVariablesToParametersMap[currentCatch])
                {
                    innerContext.MethodContext.VariableDefinitionToNameMap[pair.Key] = innerContext.MethodContext.ParameterDefinitionToNameMap[pair.Value];
                }
            }
        }

        private List<Expression> AddAllParameters(CatchClause catchClause, MethodDefinition method, VariableDeclarationExpression variable)
        {
            List<Expression> methodInvocationArguments = new List<Expression>();

            method.Parameters.Add(CreateParameter(Constants.JustDecompileGeneratedException, variable.Variable.VariableType));
            methodInvocationArguments.Add(new UnaryExpression(UnaryOperator.AddressReference, new VariableReferenceExpression(variable.Variable, null), null));

            if (this.catchClausesUsedVariablesMap.ContainsKey(catchClause))
            {
                foreach (VariableDefinition variableDefinition in this.catchClausesUsedVariablesMap[catchClause])
                {
                    if (variablesUsedOutsideFilters.Contains(variableDefinition) && variableDefinition != variable.Variable)
                    {
                        // If parameter's name is null, it will be ranamed in RenameVariables step.
                        string parameterName = context.MethodContext.VariablesToRename.Contains(variableDefinition) ? null : variableDefinition.Name;
                        ParameterDefinition parameter = CreateParameter(parameterName, variableDefinition.VariableType);
                        if (parameterName == null)
                        {
                            if (!catchClausesVariablesToParametersMap.ContainsKey(catchClause))
                            {
                                catchClausesVariablesToParametersMap.Add(catchClause, new Dictionary<VariableDefinition, ParameterDefinition>());
                            }

                            catchClausesVariablesToParametersMap[catchClause].Add(variableDefinition, parameter);
                        }
                        
                        method.Parameters.Add(parameter);
                        methodInvocationArguments.Add(new UnaryExpression(UnaryOperator.AddressReference, new VariableReferenceExpression(variableDefinition, null), null));
                    }
                }
            }

            if (this.catchClausesUsedParametersMap.ContainsKey(catchClause))
            {
                foreach (ParameterDefinition parameterDefinition in this.catchClausesUsedParametersMap[catchClause])
                {
                    method.Parameters.Add(CreateParameter(parameterDefinition.Name, parameterDefinition.ParameterType));
                    methodInvocationArguments.Add(new UnaryExpression(UnaryOperator.AddressReference, new ArgumentReferenceExpression(parameterDefinition, null), null));
                }
            }
            
            return methodInvocationArguments;
        }

        private void AddVariablesToNotDeclare(DecompilationContext context, CatchClause currentCatch)
        {
            foreach (VariableDefinition variable in context.MethodContext.Variables)
            {
                if (variablesUsedOutsideFilters.Contains(variable))
                {
                    context.MethodContext.VariablesToNotDeclare.Add(variable);
                }

                foreach (KeyValuePair<CatchClause, HashSet<VariableDefinition>> map in catchClausesUsedVariablesMap)
                {
                    if (map.Key.Equals(currentCatch))
                    {
                        continue;
                    }

                    if (map.Value.Contains(variable))
                    {
                        context.MethodContext.VariablesToNotDeclare.Add(variable);
                    }
                }
            }
        }

        private MethodInvocationExpression CreateMethodInvocation(MethodDefinition method, List<Expression> arguments)
        {
            MethodInvocationExpression methodInvocation = new MethodInvocationExpression(
                                                            new MethodReferenceExpression(
                                                                method.IsStatic ? null : new ThisReferenceExpression(method.DeclaringType, null),
                                                                method,
                                                                null),
                                                            null);
            foreach (Expression argument in arguments)
            {
                methodInvocation.Arguments.Add(argument);
            }

            return methodInvocation;
        }

        private ParameterDefinition CreateParameter(string name, TypeReference type)
        {
            return new ParameterDefinition(name, ParameterAttributes.None, new ByReferenceType(type));
        }

        private void RemoveVariablesUsedOnlyInFilters()
        {
            foreach (VariableDefinition variable in context.MethodContext.Variables)
            {
                if (!variablesUsedOutsideFilters.Contains(variable) && !context.MethodContext.VariablesToNotDeclare.Contains(variable))
                {
                    context.MethodContext.VariablesToNotDeclare.Add(variable);
                }
            }
        }
    }
}
