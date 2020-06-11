using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildAsyncStatementsStep : BaseCodeTransformer, IDecompilationStep
    {
        private readonly Dictionary<FieldDefinition, Expression> parameterMappings = new Dictionary<FieldDefinition, Expression>();

        private DecompilationContext context;
        private MethodSpecificContext methodContext;
        private TypeDefinition stateMachineTypeDef;
        private FieldDefinition builderField;
        private MatcherState matcherState;
        private StatementCollection asyncStatements;
        private StatementCollection originalStatements;
        private AsyncData asyncData;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.context = context;
            this.methodContext = context.MethodContext;
            this.originalStatements = body.Statements;
            if (Match())
            {
                body.Statements = asyncStatements;
            }

            return body;
        }

        private bool Match()
        {
			if (this.originalStatements.Count == 0)
			{
				return false;
			}

			if (!this.methodContext.Method.IsAsync(out this.stateMachineTypeDef))
			{
				if (!this.methodContext.Method.HasAsyncAttributes() || !IsAsyncFirstAssignmentStatement(this.originalStatements[0], out this.stateMachineTypeDef) ||
					!this.methodContext.Method.HasAsyncStateMachineVariable())
				{
					return false;
				}
			}

            if (!GetBuilderField())
            {
                return false;
            }

			asyncStatements = GetMoveNextStatements();
            if (asyncStatements == null || !TryRemoveOuterTryCatch(asyncStatements))
            {
                return false;
            }

            SetParameterMappings(originalStatements);

            matcherState = MatcherState.FindAwaitExpression;
            asyncStatements = (StatementCollection)Visit(asyncStatements);

            // The C# compiler that comes with MSBuild 15.0 (VS 2017 and .NET Core) seems to produce different code that the one coming with
            // MSBuild 14.0 (VS 2015) and the ones before that. In the new one there is one missing object initialization (when matcherState
            // is MatcherState.FindInitObj this step is searching for it) and this is causing our pattern matching to fail.
            // "|| matcherState == MatcherState.FindInitObj" is quick and dirty fix, which can be improved but due to the limited time right now,
            // I'm leaving it that way.
            bool result = matcherState == MatcherState.FindAwaitExpression || matcherState == MatcherState.FindInitObj;
            return result;
        }

		private bool IsAsyncFirstAssignmentStatement(Statement statement, out TypeDefinition asyncStateMachineType)
		{
			asyncStateMachineType = null;

			if (statement is ExpressionStatement)
			{
				ExpressionStatement expressionStatement = statement as ExpressionStatement;

				if (expressionStatement.Expression is BinaryExpression)
				{
					BinaryExpression binary = expressionStatement.Expression as BinaryExpression;

					if (binary.Right is ThisReferenceExpression && binary.Left is FieldReferenceExpression)
					{
						FieldReferenceExpression fieldExpression = binary.Left as FieldReferenceExpression;

						TypeReference typeReference = fieldExpression.Field.DeclaringType;
						if (typeReference == null)
						{
							return false;
						}

						TypeDefinition typeDef = typeReference.Resolve();
						if (typeDef == null || typeDef.DeclaringType != this.methodContext.Method.DeclaringType || !typeDef.IsAsyncStateMachine())
						{
							return false;
						}

						asyncStateMachineType = typeDef;
						return true;
					}
				}
			}

			return false;
		}

        public override ICodeNode Visit(ICodeNode node)
        {
            if (matcherState == MatcherState.Stopped)
            {
                return node;
            }

            return base.Visit(node);
        }

        private void SetParameterMappings(StatementCollection statements)
        {
            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i].IsAssignmentStatement())
                {
                    BinaryExpression theAssignExpression = (statements[i] as ExpressionStatement).Expression as BinaryExpression;
                    if (theAssignExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                    {
                        FieldReference assignedField = (theAssignExpression.Left as FieldReferenceExpression).Field;
                        if (assignedField.DeclaringType.Resolve() == stateMachineTypeDef)
                        {
                            parameterMappings[assignedField.Resolve()] = theAssignExpression.Right;
                        }
                    }
                }
            }
        }

        private StatementCollection GetMoveNextStatements()
        {
            MethodDefinition moveNextMethod = GetStateMachineMethod("MoveNext");

            if (moveNextMethod == null || moveNextMethod.Body == null)
            {
                return null;
            }

            BlockStatement moveNextStatements = moveNextMethod.Body.DecompileAsyncStateMachine(this.context, out this.asyncData);
            if (moveNextStatements == null)
            {
                return null;
            }

            return moveNextStatements.Statements;
        }

        private bool TryRemoveOuterTryCatch(StatementCollection statements)
        {
            int index = 0;
            for (; index < statements.Count; index++)
            {
                Statement currentStatement = statements[index];
                if (currentStatement.CodeNodeType == CodeNodeType.TryStatement && (currentStatement as TryStatement).CatchClauses.Count == 1)
                {
                    break;
                }
            }

            if (index == statements.Count)
            {
                return false;
            }

            TryStatement theTry = statements[index] as TryStatement;
            StatementCollection tryStatements = theTry.Try.Statements;

            if (asyncData.StateField == null && !TryGetStateField(theTry.CatchClauses[0].Body))
            {
                return false;
            }

            statements.RemoveAt(index);
            for (int i = 0; i < tryStatements.Count; i++)
            {
                statements.Insert(index + i, tryStatements[i]);
            }

            return true;
        }

        private bool TryGetStateField(BlockStatement theCatch)
        {
            foreach (Statement statement in theCatch.Statements)
            {
                if (statement.IsAssignmentStatement())
                {
                    BinaryExpression theAssignExpression = (statement as ExpressionStatement).Expression as BinaryExpression;
                    if (theAssignExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                        theAssignExpression.Right.CodeNodeType == CodeNodeType.LiteralExpression)
                    {
                        FieldDefinition fieldDef = (theAssignExpression.Left as FieldReferenceExpression).Field.Resolve();
                        if (fieldDef == null || fieldDef.DeclaringType == null)
                        {
                            return false;
                        }

                        TypeDefinition fieldDeclaringType = fieldDef.DeclaringType.Resolve();
                        if (fieldDeclaringType != stateMachineTypeDef)
                        {
                            return false;
                        }

                        asyncData.StateField = fieldDef;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the current type has builder field or not.
        /// </summary>
        /// <remarks>
        /// Since the new C# 6.0 compiler doesn't generate body of the SetStateMachine method, the field is now
        /// taken by name from type's fields.
        /// </remarks>
        private bool GetBuilderField()
        {
            MethodDefinition setStateMachineMethod = GetStateMachineMethod("SetStateMachine") ??
                GetStateMachineMethod("System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine");

            if (setStateMachineMethod == null || setStateMachineMethod.Body == null)
            {
                return false;
            }

            if (setStateMachineMethod.Body.Instructions.Count > 1)
            {
                foreach (Instruction instruction in setStateMachineMethod.Body.Instructions)
                {
                    if (instruction.OpCode.Code == Code.Ldflda)
                    {
                        builderField = ((FieldReference)instruction.Operand).Resolve();
                        return true;
                    }
                }
            }
            else
            {
                foreach (FieldDefinition field in stateMachineTypeDef.Fields)
                {
                    if (field.FieldType.Name == "AsyncVoidMethodBuilder" ||
                        field.FieldType.Name == "AsyncTaskMethodBuilder" ||
                        field.FieldType.Name == "AsyncTaskMethodBuilder`1")
                    {
                        builderField = field;
                        return true;
                    }
                }
            }

            return false;
        }

        private MethodDefinition GetStateMachineMethod(string name)
        {
            foreach (MethodDefinition methodDef in stateMachineTypeDef.Methods)
            {
                if (methodDef.Name == name)
                {
                    return methodDef;
                }
            }

            return null;
        }

        public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
        {
            node.Expression = (Expression)Visit(node.Expression);
            if (node.Expression == null)
            {
                if (node.Label != null && node.Label != string.Empty)
                {
                    Statement nextStatement = node.GetNextStatement();
                    if (nextStatement == null || nextStatement.Label != null && nextStatement.Label != string.Empty)
                    {
						var result = new EmptyStatement() { Label = node.Label };
						this.methodContext.GotoLabels[node.Label] = result;
						return result;
                    }

                    nextStatement.Label = node.Label;
					this.methodContext.GotoLabels[node.Label] = nextStatement;
                }
                return null;
            }

            return node;
        }

        VariableReference currentAwaiterVariable = null;
        Expression awaitedExpression = null;
        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.Operator == BinaryOperator.Assign)
            {
                if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    asyncData.AwaiterVariables.Contains((node.Left as VariableReferenceExpression).Variable))
                {
                    VariableReference awaiterVariable = (node.Left as VariableReferenceExpression).Variable;

                    if (node.Right.CodeNodeType == CodeNodeType.MethodInvocationExpression &&
                        (node.Right as MethodInvocationExpression).MethodExpression.Method.Name == "GetAwaiter")
                    {
                        Expression expression = null;//(Expression)Visit((node.Right as MethodInvocationExpression).MethodExpression.Target);
                        MethodInvocationExpression methodInvocation = node.Right as MethodInvocationExpression;
                        if (methodInvocation.MethodExpression.Target != null)
                        {
                            if(methodInvocation.Arguments.Count == 0)
                            {
                                expression = (Expression)Visit(methodInvocation.MethodExpression.Target);
                            }
                        }
                        else
                        {
                            if (methodInvocation.Arguments.Count == 1)
                            {
                                expression = (Expression)Visit(methodInvocation.Arguments[0]);
                            }
                        }

                        if (expression != null && (matcherState == MatcherState.FindAwaitExpression || matcherState == MatcherState.FindInitObj))
                        {
                            currentAwaiterVariable = awaiterVariable;
                            awaitedExpression = expression;
                            matcherState = MatcherState.FindIsCompletedInvoke;
                            return null;
                        }
                    }
                    else if ( (node.Right.CodeNodeType == CodeNodeType.ObjectCreationExpression) || 
						((node.Right.CodeNodeType == CodeNodeType.LiteralExpression) && ((node.Right as LiteralExpression).Value == null)) )
                    {
                        if ((matcherState & MatcherState.FindInitObj) == MatcherState.FindInitObj &&
                            currentAwaiterVariable == awaiterVariable)
                        {
                            matcherState ^= MatcherState.FindInitObj;
                            return null;
                        }
                    }

					matcherState = MatcherState.Stopped;
					return node;
                }
                else if (node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                    (node.Left as FieldReferenceExpression).Field.Resolve() == asyncData.StateField ||
                    node.Right.CodeNodeType == CodeNodeType.ThisReferenceExpression)
                {
                    return null;
                }
            }
            return base.VisitBinaryExpression(node);
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            MethodReferenceExpression methodRefExpression = node.MethodExpression;
            if (methodRefExpression.Target != null)
            {
                if (methodRefExpression.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    asyncData.AwaiterVariables.Contains((methodRefExpression.Target as VariableReferenceExpression).Variable))
                {
                    VariableReference awaiterVariable = (methodRefExpression.Target as VariableReferenceExpression).Variable;
                    if (currentAwaiterVariable == awaiterVariable)
                    {
                        if (methodRefExpression.Method.Name == "get_IsCompleted")
                        {
                            if (matcherState == MatcherState.FindIsCompletedInvoke)
                            {
                                matcherState = MatcherState.FindInitObj | MatcherState.FindGetResultInvoke;
                                return null;
                            }
                        }
                        else if (methodRefExpression.Method.Name == "GetResult")
                        {
                            if ((matcherState & MatcherState.FindGetResultInvoke) == MatcherState.FindGetResultInvoke)
                            {
                                matcherState ^= MatcherState.FindGetResultInvoke;
                                return new AwaitExpression((Expression)Visit(awaitedExpression), methodRefExpression.Method.ReturnType, node.UnderlyingSameMethodInstructions);
                            }
                        }
                    }

                    matcherState = MatcherState.Stopped;
                    return node;
                }
                else if (methodRefExpression.Target.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                    (methodRefExpression.Target as FieldReferenceExpression).Field.Resolve() == builderField)
                {
                    if (methodRefExpression.Method.Name == "SetResult")
                    {
                        Expression returnValue = node.Arguments.Count > 0 ? node.Arguments[0] : null;
                        return new ReturnExpression(returnValue, methodRefExpression.UnderlyingSameMethodInstructions);
                    }
                }
            }
            return base.VisitMethodInvocationExpression(node);
        }

		public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			MethodReferenceExpression methodRefExpression = node.MethodExpression;
			if (methodRefExpression.Target != null)
			{
				if (methodRefExpression.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
					asyncData.AwaiterVariables.Contains((methodRefExpression.Target as VariableReferenceExpression).Variable))
				{
					VariableReference awaiterVariable = (methodRefExpression.Target as VariableReferenceExpression).Variable;
					if (currentAwaiterVariable == awaiterVariable)
					{
						if (methodRefExpression.Method.Name == "get_IsCompleted")
						{
							if (matcherState == MatcherState.FindIsCompletedInvoke)
							{
                                matcherState = MatcherState.FindInitObj | MatcherState.FindGetResultInvoke;
                                return null;
							}
						}
					}
					matcherState = MatcherState.Stopped;
					return node;
				}
			}
			return base.VisitPropertyReferenceExpression(node);
		}

        public override ICodeNode VisitUnaryExpression(UnaryExpression node)
        {
            node.Operand = (Expression)Visit(node.Operand);
            if (node.Operand == null)
            {
                return null;
            }

            return node;
        }

        public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            if (node.Field.DeclaringType.Resolve() != stateMachineTypeDef)
            {
                return base.VisitFieldReferenceExpression(node);
            }

            FieldDefinition fieldDef = node.Field.Resolve();
            if (parameterMappings.ContainsKey(fieldDef))
            {
                return parameterMappings[fieldDef].CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
            }

            VariableDefinition variableDefinition = new VariableDefinition(GetFriendlyName(fieldDef.Name), fieldDef.FieldType, this.methodContext.Method);
            this.methodContext.Variables.Add(variableDefinition);
            this.methodContext.VariableAssignmentData.Add(variableDefinition, asyncData.FieldAssignmentData[fieldDef]);
            this.methodContext.VariablesToRename.Add(variableDefinition);

            VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(variableDefinition, node.UnderlyingSameMethodInstructions);
            parameterMappings[fieldDef] = variableReferenceExpression;
            return variableReferenceExpression;
        }

        private string GetFriendlyName(string name)
        {
            int index = name.LastIndexOf('_');
            if(index != -1 && index + 1 < name.Length)
            {
                name = name.Substring(index + 1);
            }

            return name;
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if (this.asyncData.VariableToFieldMap.ContainsKey(node.Variable))
            {
                FieldDefinition fieldDefinition = this.asyncData.VariableToFieldMap[node.Variable].Resolve();
                if (fieldDefinition != null &&
                    this.parameterMappings.ContainsKey(fieldDefinition))
                {
                    return this.parameterMappings[fieldDefinition].CloneExpressionOnly();
                }
            }

            this.methodContext.VariablesToRename.Add(node.Variable.Resolve());
            return base.VisitVariableReferenceExpression(node);
        }

        private enum MatcherState
        {
            FindAwaitExpression = 0,
            FindIsCompletedInvoke = 1,
            FindGetResultInvoke = 2,
            FindInitObj = 4,
            Stopped = 8
        }
    }
}
