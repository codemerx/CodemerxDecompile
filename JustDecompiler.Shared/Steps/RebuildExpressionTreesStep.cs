using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildExpressionTreesStep : BaseCodeTransformer, IDecompilationStep
    {
        private DecompilationContext context;
        private TypeSystem typeSystem;
        private readonly Dictionary<VariableReference, Expression> variableToValueMap = new Dictionary<VariableReference, Expression>();
        private readonly Dictionary<VariableReference, HashSet<ExpressionStatement>> variableToAssigingStatementsMap = new Dictionary<VariableReference, HashSet<ExpressionStatement>>();
        private readonly HashSet<VariableReference> usedVariables = new HashSet<VariableReference>();
        private readonly Dictionary<VariableReference, int> variableToLastUninitializedIndex = new Dictionary<VariableReference, int>();
        private int conversionDepth = 0;
        private int paramterNameIndex = 0;
        private bool failure;

        public Ast.Statements.BlockStatement Process(Decompiler.DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            if (!new ExpressionTreesFinder().ContainsExpressionTree(body))
            {
                return body;
            }

            this.context = context;
            this.typeSystem = context.TypeContext.CurrentType.Module.TypeSystem;
            this.failure = false;
            BlockStatement clone = (BlockStatement)Visit(body.Clone());
            if (failure || usedVariables.Count == 0 || !TryRemoveUnusedVariableAssignments(clone))
            {
                return body;
            }
            clone = (BlockStatement)new ClosureVariablesRemover(context.MethodContext).Visit(clone);
            clone = new CombinedTransformerStep().Process(context, clone);
            return clone;
        }

        public override ICodeNode Visit(ICodeNode node)
        {
            if (this.failure)
            {
                return node;
            }

            return base.Visit(node);
        }

        public override ICodeNode VisitLambdaExpression(LambdaExpression node)
        {
            return node.IsExpressionTreeLambda ? node : base.VisitLambdaExpression(node);
        }

        private bool TryRemoveUnusedVariableAssignments(BlockStatement body)
        {
            HashSet<VariableReference> removedVariables = new HashSet<VariableReference>();

            bool changed;
            do
            {
                changed = false;
                foreach (VariableReference variable in usedVariables)
                {
                    if (removedVariables.Contains(variable))
                    {
                        continue;
                    }

                    HashSet<ExpressionStatement> assignments = variableToAssigingStatementsMap[variable];
                    if (new VariableUsageFinder(variable, assignments).IsUsed(body))
                    {
                        continue;
                    }

                    foreach (ExpressionStatement statement in assignments)
                    {
                        if (!TryRemoveExpressionStatment(statement))
                        {
                            return false;
                        }
                    }

                    changed = true;
                    removedVariables.Add(variable);
                }
            } while (changed);

            return usedVariables.Count == removedVariables.Count;
        }

        private bool TryRemoveExpressionStatment(ExpressionStatement statement)
        {
            BlockStatement blockParent = statement.Parent as BlockStatement;
            if (blockParent == null)
            {
                return false;
            }

            blockParent.Statements.Remove(statement);
            return true;
        }

        public override ICodeNode VisitBlockStatement(BlockStatement node)
        {
            this.variableToLastUninitializedIndex.Clear();
            this.variableToValueMap.Clear();
            return base.VisitBlockStatement(node);
        }

        public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
        {
            if (node.Expression.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                BinaryExpression binary = node.Expression as BinaryExpression;
                if (binary.IsAssignmentExpression )
                {
                    if (binary.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                    {
                        RecordVariableAssignment(node);
                        return node;
                    }
                    else if (binary.Left.CodeNodeType == CodeNodeType.ArrayIndexerExpression)
                    {
                        if (TryUpdateInitializer(node))
                        {
                            return node;
                        }
                    }
                }
            }
            return base.VisitExpressionStatement(node);
        }

        private bool TryUpdateInitializer(ExpressionStatement node)
        {
            BinaryExpression assignment = node.Expression as BinaryExpression;
            ArrayIndexerExpression indexerExpression = assignment.Left as ArrayIndexerExpression;

            if (indexerExpression.Target == null || indexerExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
            {
                return false;
            }

            VariableReference variable = (indexerExpression.Target as VariableReferenceExpression).Variable;

            int lastUninitializedIndex;
            if (!this.variableToLastUninitializedIndex.TryGetValue(variable, out lastUninitializedIndex))
            {
                return false;
            }

            if (indexerExpression.Indices == null || indexerExpression.Indices.Count != 1)
            {
                return false;
            }

            int index = GetIntegerValue(indexerExpression.Indices[0] as LiteralExpression);
            if (index != lastUninitializedIndex)
            {
                this.variableToLastUninitializedIndex.Remove(variable);
                this.variableToValueMap.Remove(variable);
                this.variableToAssigingStatementsMap.Remove(variable);

                if (this.usedVariables.Contains(variable))
                {
                    this.failure = true;
                }
                return false;
            }

            ArrayCreationExpression arrayCreation = this.variableToValueMap[variable] as ArrayCreationExpression;
            arrayCreation.Initializer.Expressions.Add((Expression)Visit(assignment.Right.CloneExpressionOnly()));
            this.variableToAssigingStatementsMap[variable].Add(node);
            this.variableToLastUninitializedIndex[variable] = index + 1;
            return true;
        }

        private void RecordVariableAssignment(ExpressionStatement node)
        {
            BinaryExpression assignment = node.Expression as BinaryExpression;
            VariableReference variable = (assignment.Left as VariableReferenceExpression).Variable;
            assignment.Right = (Expression)Visit(assignment.Right);
            Expression rightClone = assignment.Right.CloneExpressionOnly();
            this.variableToValueMap[variable] = rightClone;

            HashSet<ExpressionStatement> statements;
            if (!this.variableToAssigingStatementsMap.TryGetValue(variable, out statements))
            {
                statements = new HashSet<ExpressionStatement>();
                this.variableToAssigingStatementsMap[variable] = statements;
            }

            statements.Add(node);

            ArrayCreationExpression arrayCreation = rightClone as ArrayCreationExpression;
            if (arrayCreation == null || arrayCreation.Dimensions == null || arrayCreation.Dimensions.Count != 1 || arrayCreation.Initializer != null && arrayCreation.Initializer.Expressions != null &&
                arrayCreation.Initializer.Expressions.Count > 0)
            {
                return;
            }

			var blockExpression = new BlockExpression(null);
			arrayCreation.Initializer = new InitializerExpression(blockExpression, InitializerType.ArrayInitializer);
			this.variableToLastUninitializedIndex[variable] = 0;
        }

        private int GetIntegerValue(LiteralExpression size)
        {
            if (size != null)
            {
                try
                {
                    return Convert.ToInt32(size.Value);
                }
                catch (InvalidCastException)
                {
                    return -1;
                }
            }
            return -1;
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            this.conversionDepth++;
            ICodeNode result = ConvertInvocation(node);
            this.conversionDepth--;
            return result ?? base.VisitMethodInvocationExpression(node);
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if(this.conversionDepth > 0)
            {
                Expression result;
                if (this.variableToValueMap.TryGetValue(node.Variable, out result))
                {
                    usedVariables.Add(node.Variable);
                    return Visit(result.CloneExpressionOnly());
                }

                this.failure = !this.context.MethodContext.ClosureVariableToFieldValue.ContainsKey(node.Variable);
            }
            return base.VisitVariableReferenceExpression(node);
        }

        private ICodeNode ConvertInvocation(MethodInvocationExpression invocation)
        {
            if(invocation.MethodExpression == null || invocation.MethodExpression.Method == null || invocation.MethodExpression.Method.HasThis ||
                invocation.MethodExpression.Method.DeclaringType == null || invocation.MethodExpression.Method.DeclaringType.FullName != "System.Linq.Expressions.Expression")
            {
                return null;
            }

            //MethodDefinition methodDef = invocation.MethodExpression.MethodDefinition;
            //if (methodDef == null)
            //{
            //    return null;
            //}

            if (this.conversionDepth == 0 && invocation.MethodExpression.Method.Name != "Lambda")
            {
                return null;
            }

            ICodeNode result = null;
            switch (invocation.MethodExpression.Method.Name)
            {
                case "Add":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Add, false);
                    break;
                case "AddChecked":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Add, true);
                    break;
                case "And":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.BitwiseAnd);
                    break;
                case "AndAlso":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.LogicalAnd);
                    break;
                case "ArrayAccess":
                case "ArrayIndex":
                    result = ConvertArrayIndex(invocation);
                    break;
                case "ArrayLength":
                    result = ConvertArrayLength(invocation);
                    break;
                case "Bind":
                    result = ConvertBind(invocation);
                    break;
                case "Call":
                    result = ConvertCall(invocation);
                    break;
                case "Coalesce":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.NullCoalesce);
                    break;
                case "Condition":
                    result = ConvertCondition(invocation);
                    break;
                case "Constant":
                    result = ConvertConstant(invocation);
                    break;
                case "Convert":
                    result = ConvertCast(invocation);
                    break;
                case "ConvertChecked":
                    result = ConvertCastChecked(invocation);
                    break;
                case "Divide":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Divide);
                    break;
                case "ElementInit":
                    result = ConvertElementInit(invocation);
                    break;
                case "Equal":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.ValueEquality);
                    break;
                case "ExclusiveOr":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.BitwiseXor);
                    break;
                case "Field":
                    result = ConvertField(invocation);
                    break;
                case "GreaterThan":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.GreaterThan);
                    break;
                case "GreaterThanOrEqual":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.GreaterThanOrEqual);
                    break;
                case "Invoke":
                    result = ConvertInvoke(invocation);
                    break;
                case "Lambda":
                    result = ConvertLambda(invocation);
                    break;
                case "LeftShift":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.LeftShift);
                    break;
                case "LessThan":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.LessThan);
                    break;
                case "LessThanOrEqual":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.LessThanOrEqual);
                    break;
                case "ListBind":
                    result = ConvertListBind(invocation);
                    break;
                case "ListInit":
                    result = ConvertListInit(invocation);
                    break;
                case "MemberInit":
                    result = ConvertMemberInit(invocation);
                    break;
                case "Modulo":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Modulo);
                    break;
                case "Multiply":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Multiply, false);
                    break;
                case "MultiplyChecked":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Multiply, true);
                    break;
                case "Negate":
                    result = ConvertUnaryOperator(invocation, UnaryOperator.Negate);
                    break;
                case "NegateChecked":
                    result = ConvertUnaryOperatorChecked(invocation, UnaryOperator.Negate);
                    break;
                case "New":
                    result = ConvertNewObject(invocation);
                    break;
                case "NewArrayBounds":
                    result = ConvertNewArrayBounds(invocation);
                    break;
                case "NewArrayInit":
                    result = ConvertNewArrayInit(invocation);
                    break;
                case "Not":
                    result = ConvertUnaryOperator(invocation, UnaryOperator.LogicalNot);
                    break;
                case "NotEqual":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.ValueInequality);
                    break;
                case "OnesComplement":
                    result = ConvertUnaryOperator(invocation, UnaryOperator.BitwiseNot);
                    break;
                case "Or":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.BitwiseOr);
                    break;
                case "OrElse":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.LogicalOr);
                    break;
                case "Parameter":
                    result = ConvertParameter(invocation);
                    break;
                case "Property":
                    result = ConvertProperty(invocation);
                    break;
                case "Quote":
                    if (invocation.Arguments.Count == 1)
                        result = Visit(invocation.Arguments[0]);
                    break;
                case "RightShift":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.RightShift);
                    break;
                case "Subtract":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Subtract, false);
                    break;
                case "SubtractChecked":
                    result = ConvertBinaryOperator(invocation, BinaryOperator.Subtract, true);
                    break;
                case "TypeAs":
                    result = ConvertTypeAs(invocation);
                    break;
                case "TypeIs":
                    result = ConvertTypeIs(invocation);
                    break;
                default:
                    return null;
            }

            this.failure |= result == null;
            return result;
        }

        private ICodeNode ConvertListBind(MethodInvocationExpression invocation)
        {
            MethodReference methodRef;
            if (invocation.Arguments.Count != 2 || !TryGetMethodReference((Expression)Visit(invocation.Arguments[0]), "System.Reflection.MethodInfo", out methodRef))
            {
                return null;
            }

            PropertyDefinition propertyDef = ResolveProperty(methodRef);
            if (propertyDef == null)
            {
                return null;
            }

            ArrayCreationExpression listValues = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if(listValues == null || listValues.Dimensions == null || listValues.Dimensions.Count != 1 || listValues.Initializer == null || listValues.Initializer.Expressions == null)
            {
                return null;
            }

			listValues.Initializer.InitializerType = InitializerType.CollectionInitializer;
			listValues.Initializer.IsMultiLine = true;
            return new BinaryExpression(BinaryOperator.Assign, new PropertyInitializerExpression(propertyDef, propertyDef.PropertyType),
                listValues.Initializer, this.typeSystem, null);
        }

        private ICodeNode ConvertElementInit(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ArrayCreationExpression value = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (value == null || value.Dimensions == null || value.Dimensions.Count != 1 || value.Initializer == null || value.Initializer.Expressions == null ||
                value.Initializer.Expressions.Count == 0)
            {
                return null;
            }

            Visit(invocation.Arguments[0]);

            return value.Initializer.Expressions.Count == 1 ? value.Initializer.Expressions[0] : value.Initializer.Expression;
        }

        private ICodeNode ConvertBind(MethodInvocationExpression invocation)
        {
            MethodReference methodRef;
            if (invocation.Arguments.Count != 2 || !TryGetMethodReference((Expression)Visit(invocation.Arguments[0]), "System.Reflection.MethodInfo", out methodRef))
            {
                return null;
            }

            PropertyDefinition propertyDef = ResolveProperty(methodRef);
            if (propertyDef == null)
            {
                return null;
            }

            return new BinaryExpression(BinaryOperator.Assign, new PropertyInitializerExpression(propertyDef, propertyDef.PropertyType),
                (Expression)Visit(invocation.Arguments[1]), this.typeSystem, null);
        }

        private PropertyDefinition ResolveProperty(MethodReference methodRef)
        {
            MethodDefinition methodDef = methodRef.Resolve();
            if (methodDef == null || methodDef.DeclaringType == null)
            {
                return null;
            }

            return methodDef.DeclaringType.Properties.FirstOrDefault(prop => prop.GetMethod == methodDef || prop.SetMethod == methodDef);
        }

        private ICodeNode ConvertConstant(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count < 1 || invocation.Arguments.Count > 2)
            {
                return null;
            }

            Expression result = (Expression)Visit(invocation.Arguments[0]);
            if (invocation.Arguments.Count == 2)
            {
                //This is done in order to mark the Type argument variable
                Visit(invocation.Arguments[1]);
            }

            BoxExpression boxExpression = result as BoxExpression;
            return boxExpression != null && boxExpression.IsAutoBox ? boxExpression.BoxedExpression : result;
        }

        private ICodeNode ConvertMemberInit(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ObjectCreationExpression creation = Visit(invocation.Arguments[0]) as ObjectCreationExpression;
            if (creation == null || creation.Initializer != null)
            {
                return null;
            }

            ArrayCreationExpression initializerArray = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (initializerArray == null || initializerArray.Dimensions == null || initializerArray.Dimensions.Count != 1 || initializerArray.Initializer == null ||
                initializerArray.Initializer.Expressions == null ||
                initializerArray.Initializer.Expressions.Any(expr => expr.CodeNodeType != CodeNodeType.BinaryExpression || !(expr as BinaryExpression).IsAssignmentExpression))
            {
                return null;
            }

			if (initializerArray.Initializer.Expressions.Count > 0)
			{
				creation.Initializer = new InitializerExpression(initializerArray.Initializer.Expression, InitializerType.ObjectInitializer);
				creation.Initializer.IsMultiLine = true;
			}

			return creation;
        }

        private ICodeNode ConvertListInit(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ObjectCreationExpression creation = Visit(invocation.Arguments[0]) as ObjectCreationExpression;
            if (creation == null || creation.Initializer != null)
            {
                return null;
            }

            ArrayCreationExpression initializerArray = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (initializerArray == null || initializerArray.Dimensions == null || initializerArray.Dimensions.Count != 1 || initializerArray.Initializer == null ||
                initializerArray.Initializer.Expressions == null)
            {
                return null;
            }

            creation.Initializer = new InitializerExpression(initializerArray.Initializer.Expression, InitializerType.CollectionInitializer);
			creation.Initializer.IsMultiLine = true;
            return creation;
        }

        private ICodeNode ConvertLambda(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ArrayCreationExpression arguments = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (arguments == null || arguments.Initializer == null || arguments.Initializer.Expressions == null ||
                arguments.Initializer.Expressions.Any(element => element.CodeNodeType != CodeNodeType.ArgumentReferenceExpression))
            {
                return null;
            }

            List<ArgumentReferenceExpression> parameters = arguments.Initializer.Expressions.Cast<ArgumentReferenceExpression>().ToList();

            bool hasAnonymousParameter = parameters.Any(param => param.Parameter.ParameterType.Resolve().IsAnonymous());

            BlockStatement body = new BlockStatement();
            body.AddStatement(new ExpressionStatement(new ShortFormReturnExpression((Expression)Visit(invocation.Arguments[0]), null)));

            return new LambdaExpression(new ExpressionCollection(parameters.Select(param => new LambdaParameterExpression(param.Parameter, !hasAnonymousParameter, null))),
                body, false, false, parameters.Select(argRef => argRef.Parameter), true, null);
        }

        private ICodeNode ConvertInvoke(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ArrayCreationExpression arguments = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (arguments == null || arguments.Dimensions.Count != 1 || arguments.Initializer == null || arguments.Initializer.Expressions == null)
            {
                return null;
            }

            Expression target = (Expression)Visit(invocation.Arguments[0]);
            MethodReference invokeMethodReference = GetInvokeMethodReference(target);
            return invokeMethodReference != null ? new DelegateInvokeExpression(target, arguments.Initializer.Expressions, invokeMethodReference, null) : null;
        }

        private MethodReference GetInvokeMethodReference(Expression target)
        {
            if (!target.HasType || target.ExpressionType == null)
            {
                return null;
            }

            TypeReference delegateTypeRef = target.ExpressionType;
            TypeDefinition delegateTypeDef = delegateTypeRef.Resolve();
            if (delegateTypeDef == null || !delegateTypeDef.IsDelegate())
            {
                return null;
            }

            MethodDefinition invokeMethodDef = delegateTypeDef.Methods.FirstOrDefault(method => method.Name == "Invoke");
            if(invokeMethodDef == null)
            {
                return null;
            }

            MethodReference result = new MethodReference(invokeMethodDef.Name, invokeMethodDef.ReturnType, delegateTypeRef);
            result.Parameters.AddRange(invokeMethodDef.Parameters);
            return result;
        }

        private ICodeNode ConvertNewArrayBounds(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ArrayCreationExpression arrayCreation = GenerateArrayCreationExpression(invocation.Arguments[0]);
            if (arrayCreation == null)
            {
                return null;
            }

            ArrayCreationExpression boundsArray = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (boundsArray == null || boundsArray.Dimensions.Count != 1 || boundsArray.Initializer == null || boundsArray.Initializer.Expressions == null ||
                boundsArray.Initializer.Expressions.Count == 0)
            {
                return null;
            }

            arrayCreation.Dimensions = boundsArray.Initializer.Expressions;
            return arrayCreation;
        }

        private ICodeNode ConvertNewArrayInit(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            ArrayCreationExpression arrayCreation = GenerateArrayCreationExpression(invocation.Arguments[0]);
            if (arrayCreation == null)
            {
                return null;
            }

            ArrayCreationExpression initializerArray = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (initializerArray == null || initializerArray.Dimensions == null || initializerArray.Initializer == null || initializerArray.Initializer.Expressions == null ||
                initializerArray.Initializer.Expressions.Count == 0)
            {
                return null;
            }

            arrayCreation.Dimensions = initializerArray.Dimensions;
            arrayCreation.Initializer = initializerArray.Initializer;
            return arrayCreation;
        }

        private ArrayCreationExpression GenerateArrayCreationExpression(Expression unprocessedExpression)
        {
            TypeOfExpression typeOfExpression = Visit(unprocessedExpression) as TypeOfExpression;
            return typeOfExpression == null ? null : new ArrayCreationExpression(typeOfExpression.Type, null, null);
        }

        private ICodeNode ConvertNewObject(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count < 1 || invocation.Arguments.Count > 3)
            {
                return null;
            }

            Expression visitedFirst = (Expression)Visit(invocation.Arguments[0]);
            MethodReference constructorReference;
            if (!TryGetMethodReference(visitedFirst, "System.Reflection.ConstructorInfo", out constructorReference))
            {
                if (invocation.Arguments.Count != 1 || visitedFirst.CodeNodeType != CodeNodeType.TypeOfExpression)
                {
                    return null;
                }

                return new ObjectCreationExpression(null, (visitedFirst as TypeOfExpression).Type, null, null);
            }

            ObjectCreationExpression creationExpression = new ObjectCreationExpression(constructorReference, constructorReference.DeclaringType, null, null);

            if (invocation.Arguments.Count == 1)
            {
                return creationExpression;
            }

            ArrayCreationExpression argumentsArray = Visit(invocation.Arguments[1]) as ArrayCreationExpression;
            if (argumentsArray == null || argumentsArray.Dimensions.Count != 1 || argumentsArray.Initializer == null ||
                argumentsArray.Initializer.Expressions == null || argumentsArray.Initializer.Expressions.Count != constructorReference.Parameters.Count)
            {
                return null;
            }

            foreach (Expression argument in argumentsArray.Initializer.Expressions)
            {
                creationExpression.Arguments.Add(argument);
            }

            if (invocation.Arguments.Count == 2)
            {
                return creationExpression;
            }

            Visit(invocation.Arguments[2]);

            return creationExpression;
        }

        private ICodeNode ConvertTypeIs(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            return ConvertCast(invocation, (expression, type) => new CanCastExpression(expression, type, null));
        }

        private ICodeNode ConvertTypeAs(MethodInvocationExpression invocation)
        {
            if (invocation.Arguments.Count != 2)
            {
                return null;
            }

            return ConvertCast(invocation, (expression, type) => new SafeCastExpression(expression, type, null));
        }

        private ICodeNode ConvertCast(MethodInvocationExpression invocation, Func<Expression, TypeReference, Expression> createInstance)
        {
            if (invocation.Arguments.Count < 2 || invocation.Arguments[1].CodeNodeType != CodeNodeType.TypeOfExpression)
            {
                return null;
            }

            Expression castTarget = (Expression)Visit(invocation.Arguments[0]);
            TypeReference reference = (invocation.Arguments[1] as TypeOfExpression).Type;

            return createInstance(castTarget, reference);
        }

        private ICodeNode ConvertProperty(MethodInvocationExpression invocation)
        {
            MethodInvocationExpression methodInvocation = ConvertCall(invocation) as MethodInvocationExpression;
            if(methodInvocation != null)
            {
                PropertyReferenceExpression result = new PropertyReferenceExpression(methodInvocation, null);
                return result.Property != null ? result : null;
            }
            return null;
        }

        private ICodeNode ConvertField(MethodInvocationExpression invocation)
        {
            FieldReference fieldRef;
            if (invocation.Arguments.Count != 2 || !TryGetFieldReference((Expression)Visit(invocation.Arguments[1]), out fieldRef))
            {
                return null;
            }

            Expression target = GetTarget(invocation.Arguments[0]);
            return new FieldReferenceExpression(target, fieldRef, null);
        }

        private bool TryGetFieldReference(Expression expression, out FieldReference fieldRef)
        {
            fieldRef = null;

            MethodInvocationExpression methodInvoke = expression as MethodInvocationExpression;
            if (methodInvoke == null || methodInvoke.Arguments.Count > 2 || methodInvoke.Arguments.Count < 1 || methodInvoke.Arguments[0].CodeNodeType != CodeNodeType.MemberHandleExpression ||
                methodInvoke.Arguments.Count == 2 && methodInvoke.Arguments[1].CodeNodeType != CodeNodeType.MemberHandleExpression || methodInvoke.MethodExpression.Method == null ||
                methodInvoke.MethodExpression.Method.Name != "GetFieldFromHandle" || methodInvoke.MethodExpression.Method.DeclaringType == null ||
                methodInvoke.MethodExpression.Method.DeclaringType.FullName != "System.Reflection.FieldInfo")
            {
                return false;
            }

            fieldRef = (methodInvoke.Arguments[0] as MemberHandleExpression).MemberReference as FieldReference;
            return fieldRef != null;
        }

        private Expression GetTarget(Expression unprocessedTarget)
        {
            Expression target = (Expression)Visit(unprocessedTarget);
            return target.CodeNodeType != CodeNodeType.LiteralExpression || (target as LiteralExpression).Value != null ? target : null;
        }

        private UnaryExpression ConvertUnaryOperator(MethodInvocationExpression node, UnaryOperator unaryOperator)
        {
            if (node.Arguments.Count < 1)
            {
                return null;
            }
            return new UnaryExpression(unaryOperator, (Expression)Visit(node.Arguments[0]), null);
        }

        private ICodeNode ConvertUnaryOperatorChecked(MethodInvocationExpression node, UnaryOperator unaryOperator)
        {
            UnaryExpression unary = ConvertUnaryOperator(node, unaryOperator);
            return unary != null ? new CheckedExpression(unary, null) : null;
        }

        private ICodeNode ConvertCast(MethodInvocationExpression node)
        {
            return this.ConvertCast(node, (expression, type) => new ExplicitCastExpression(expression, type, null));
        }

        private ICodeNode ConvertCastChecked(MethodInvocationExpression node)
        {
            ExplicitCastExpression cast = ConvertCast(node) as ExplicitCastExpression;
            return cast != null ? new CheckedExpression(cast, null) : null;
        }

        private ICodeNode ConvertCondition(MethodInvocationExpression node)
        {
            if (node.Arguments.Count < 3)
            {
                return null;
            }

            Expression condition = (Expression)Visit(node.Arguments[0]);
            Expression then = (Expression)Visit(node.Arguments[1]);
            Expression @else = (Expression)Visit(node.Arguments[2]);

            return new ConditionExpression(condition, then, @else, null);
        }

        private ICodeNode ConvertCall(MethodInvocationExpression node)
        {
            MethodReference methodRef;
            if (node.Arguments.Count < 2 || !TryGetMethodReference((Expression)Visit(node.Arguments[1]), "System.Reflection.MethodInfo", out methodRef))
            {
                return null;
            }

            Expression target = GetTarget(node.Arguments[0]);
            MethodInvocationExpression methodInvoke = new MethodInvocationExpression(new MethodReferenceExpression(target, methodRef, null), null);

            if (node.Arguments.Count == 3)
            {
                ArrayCreationExpression arguments = Visit(node.Arguments[2]) as ArrayCreationExpression;
                if (arguments == null || arguments.Dimensions.Count != 1 || arguments.Initializer == null || arguments.Initializer.Expressions == null ||
                    arguments.Initializer.Expressions.Count != methodRef.Parameters.Count)
                {
                    return null;
                }

                foreach (Expression argument in arguments.Initializer.Expressions)
                {
                    methodInvoke.Arguments.Add(argument);
                }
            }


            return methodInvoke;
        }

        private bool TryGetMethodReference(Expression expression, string castTargetTypeName, out MethodReference methodRef)
        {
            methodRef = null;

            ExplicitCastExpression cast = expression as ExplicitCastExpression;
            if (cast == null || cast.Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression ||
                cast.TargetType == null || cast.TargetType.FullName != castTargetTypeName)
            {
                
                return false;
            }

            MethodInvocationExpression methodInvoke = cast.Expression as MethodInvocationExpression;
            if (methodInvoke == null || methodInvoke.Arguments.Count > 2 || methodInvoke.Arguments.Count < 1 || methodInvoke.Arguments[0].CodeNodeType != CodeNodeType.MemberHandleExpression ||
                methodInvoke.Arguments.Count == 2 && methodInvoke.Arguments[1].CodeNodeType != CodeNodeType.MemberHandleExpression || methodInvoke.MethodExpression.Method == null ||
                methodInvoke.MethodExpression.Method.Name != "GetMethodFromHandle" || methodInvoke.MethodExpression.Method.DeclaringType == null ||
                methodInvoke.MethodExpression.Method.DeclaringType.FullName != "System.Reflection.MethodBase")
            {
                return false;
            }

            methodRef = (methodInvoke.Arguments[0] as MemberHandleExpression).MemberReference as MethodReference;
            return methodRef != null;
        }

        private ICodeNode ConvertArrayLength(MethodInvocationExpression node)
        {
            if (node.Arguments.Count != 1)
            {
                return null;
            }

            return new ArrayLengthExpression((Expression)Visit(node.Arguments[0]), this.typeSystem, null);
        }

        private ICodeNode ConvertArrayIndex(MethodInvocationExpression node)
        {
            if(node.Arguments.Count != 2)
            {
                return null;
            }

            ArrayCreationExpression indexesArray = Visit(node.Arguments[1]) as ArrayCreationExpression;
            if (indexesArray == null || indexesArray.Dimensions.Count != 1 || indexesArray.Initializer == null || indexesArray.Initializer.Expressions.Count == 0)
            {
                return null;
            }

            Expression target = (Expression)Visit(node.Arguments[0]);
            return new ArrayIndexerExpression(target, null) { Indices = indexesArray.Initializer.Expressions };
        }

        private BinaryExpression ConvertBinaryOperator(MethodInvocationExpression node, BinaryOperator binaryOperator, bool isChecked)
        {
            if (node.Arguments.Count < 2)
            {
                return null;
            }

            try
            {
                return new BinaryExpression(binaryOperator, (Expression)Visit(node.Arguments[0]), (Expression)Visit(node.Arguments[1]),
                    typeSystem, isChecked, null, node.Arguments.Count > 2);
            }
            catch
            {
                return null;
            }
        }

        private BinaryExpression ConvertBinaryOperator(MethodInvocationExpression node, BinaryOperator binaryOperator)
        {
            return ConvertBinaryOperator(node, binaryOperator, false);
        }

        private ArgumentReferenceExpression ConvertParameter(MethodInvocationExpression node)
        {
            if (node.Arguments.Count < 1 || node.Arguments.Count > 2 || node.Arguments[0].CodeNodeType != CodeNodeType.TypeOfExpression)
            {
                return null;
            }

            TypeReference parameterType = (node.Arguments[0] as TypeOfExpression).Type;

            string parameterName = null;
            if (node.Arguments.Count == 2 && node.Arguments[1].CodeNodeType == CodeNodeType.LiteralExpression)
            {
                parameterName = (node.Arguments[1] as LiteralExpression).Value as string;
            }

            ParameterDefinition paramDef = new ParameterDefinition(parameterName ?? "expressionParameter" + this.paramterNameIndex++, ParameterAttributes.None, parameterType);
            return new ArgumentReferenceExpression(paramDef, null);
        }

        private class VariableUsageFinder : BaseCodeVisitor
        {
            private readonly VariableReference variable;
            private readonly HashSet<ExpressionStatement> assignments;

            public VariableUsageFinder(VariableReference variable, HashSet<ExpressionStatement> assignments)
            {
                this.variable = variable;
                this.assignments = assignments;
            }

            bool isUsed;
            public bool IsUsed(BlockStatement body)
            {
                this.isUsed = false;
                Visit(body);
                return this.isUsed;
            }

            public override void Visit(ICodeNode node)
            {
                if (!this.isUsed)
                {
                    base.Visit(node);
                }
            }

            public override void VisitExpressionStatement(ExpressionStatement node)
            {
                if (!assignments.Contains(node))
                {
                    base.VisitExpressionStatement(node);
                }
            }

            public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
            {
                if (node.Variable == variable)
                {
                    this.isUsed = true;
                }
            }
        }

        class ClosureVariablesRemover : BaseCodeTransformer
        {
            private readonly MethodSpecificContext methodContext;

            public ClosureVariablesRemover(MethodSpecificContext methodContext)
            {
                this.methodContext = methodContext;
            }

            public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
            {
                if (node.Target != null && node.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    VariableReference variable = (node.Target as VariableReferenceExpression).Variable;
                    Dictionary<FieldDefinition, Expression> fieldToExpressionMap;
                    if (this.methodContext.ClosureVariableToFieldValue.TryGetValue(variable, out fieldToExpressionMap))
                    {
                        FieldDefinition fieldDef = node.Field.Resolve();
                        Expression result;
                        if (fieldDef != null && fieldToExpressionMap.TryGetValue(fieldDef, out result))
                        {
                            return result.CloneExpressionOnly();
                        }
                    }
                }

                return base.VisitFieldReferenceExpression(node);
            }
        }

        class ExpressionTreesFinder : BaseCodeVisitor
        {
            private bool containsExpressionTree;

            public bool ContainsExpressionTree(BlockStatement body)
            {
                this.containsExpressionTree = false;
                Visit(body);
                return this.containsExpressionTree;
            }

            public override void Visit(ICodeNode node)
            {
                if (!this.containsExpressionTree)
                {
                    base.Visit(node);
                }
            }

            public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
            {
                if (node.MethodExpression != null && node.MethodExpression.Method != null && !node.MethodExpression.Method.HasThis &&
                    node.MethodExpression.Method.Name == "Lambda" && node.MethodExpression.Method.DeclaringType != null &&
                    node.MethodExpression.Method.DeclaringType.FullName == "System.Linq.Expressions.Expression")
                {
                    this.containsExpressionTree = true;
                }
                else
                {
                    base.VisitMethodInvocationExpression(node);
                }
            }
        }
    }
}
