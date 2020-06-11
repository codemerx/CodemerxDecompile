using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildLinqQueriesStep : IDecompilationStep
    {
        public Ast.Statements.BlockStatement Process(Decompiler.DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            body = (BlockStatement)new LinqQueriesRebuilder(context.MethodContext).Visit(body);
            return body;
        }

        class LinqQueriesRebuilder : BaseCodeTransformer
        {
            private VariableReference currentIdentifier;
            private readonly List<QueryClause> clauses = new List<QueryClause>();
            private readonly MethodSpecificContext methodContext;

            public LinqQueriesRebuilder(MethodSpecificContext methodContext)
            {
                this.methodContext = methodContext;
            }

            public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
            {
                LinqQueryExpression result;
                if (TryMatchLinqQuery(node, out result))
                {
                    return RemoveTransparentIdentifiers(result);
                }
                return base.VisitMethodInvocationExpression(node);
            }

            private bool TryMatchLinqQuery(MethodInvocationExpression methodInvocation, out LinqQueryExpression linqQuery)
            {
                Stack<MethodInvocationExpression> queryStack = new Stack<MethodInvocationExpression>();
                MethodInvocationExpression current = methodInvocation;
                bool isQueryableMethod = current.MethodExpression.MethodDefinition.IsQueryableMethod();
                while (current != null && current.MethodExpression.MethodDefinition.IsQueryMethod() && isQueryableMethod == current.MethodExpression.MethodDefinition.IsQueryableMethod())
                {
                    queryStack.Push(current);
                    current = current.Arguments[0] as MethodInvocationExpression;
                }

                if (queryStack.Count == 0)
                {
                    linqQuery = null;
                    return false;
                }

                MethodInvocationExpression top = queryStack.Peek();
                top.Arguments[0] = (Expression)Visit(top.Arguments[0]);
                linqQuery = ProcessExtensionMethodChain(queryStack, methodInvocation, isQueryableMethod);
                return linqQuery != null;
            }

            public override ICodeNode VisitLinqQueryExpression(LinqQueryExpression node)
            {
                return node;
            }

            private LinqQueryExpression ProcessExtensionMethodChain(IEnumerable<MethodInvocationExpression> methodInvocations, MethodInvocationExpression topInvoke, bool queryable)
            {
                this.currentIdentifier = null;
                this.clauses.Clear();

                foreach (MethodInvocationExpression methodInvoke in methodInvocations)
                {
                    bool success = true;
                    switch (methodInvoke.MethodExpression.MethodDefinition.Name)
                    {
                        case "Select":
                            success = TryProcessSelectMethod(methodInvoke, queryable);
                            break;
                        case "SelectMany":
                            success = TryProcessSelectManyMethod(methodInvoke, queryable);
                            break;
                        case "Where":
                            success = TryProcessWhereMethod(methodInvoke, queryable);
                            break;
                        case "OrderBy":
                            success = TryProcessOrderByMethod(methodInvoke, true, queryable);
                            break;
                        case "OrderByDescending":
                            success = TryProcessOrderByMethod(methodInvoke, false, queryable);
                            break;
                        case "ThenBy":
                            success = TryProcessThenByMethod(methodInvoke, true, queryable);
                            break;
                        case "ThenByDescending":
                            success = TryProcessThenByMethod(methodInvoke, false, queryable);
                            break;
                        case "Join":
                            success = TryProcessJoinMethod(methodInvoke, false, queryable);
                            break;
                        case "GroupJoin":
                            success = TryProcessJoinMethod(methodInvoke, true, queryable);
                            break;
                        case "GroupBy":
                            success = TryProcessGroupByMethod(methodInvoke, queryable);
                            break;
                        default:
                            return null;
                    }

                    if (!success)
                    {
                        return null;
                    }
                }

                if (this.clauses.Count == 0)
                {
                    return null;
                }

                if (this.currentIdentifier != null)
                {
                    clauses.Add(new SelectClause(new VariableReferenceExpression(this.currentIdentifier, null), null));
                }

                return new LinqQueryExpression(new List<QueryClause>(this.clauses), topInvoke.ExpressionType, null);
            }

            private bool TryProcessSingleParameterQuery(MethodInvocationExpression methodInvoke, bool canChangeIdentifier, bool queryable, out Expression result)
            {
                result = null;

                MethodDefinition methodDef = methodInvoke.MethodExpression.MethodDefinition;
                if (methodDef.Parameters.Count != 2)
                {
                    return false;
                }

                GenericInstanceType funcType = GetFuncGenericInstance(methodDef.Parameters[1].ParameterType as GenericInstanceType, queryable);
                if (funcType == null || funcType.GenericArguments.Count != 2)
                {
                    return false;
                }

                LambdaExpression func = GetLambdaExpression(methodInvoke.Arguments[1], queryable);
                if (func == null || func.Arguments.Count != 1)
                {
                    return false;
                }

                ParameterDefinition parameter = func.Parameters[0].Resolve();

                if (canChangeIdentifier)
                {
                    ProcessCurrentIdentifier(methodInvoke.Arguments[0], parameter);
                }
                else if(this.currentIdentifier == null)
                {
                    return false;
                }

                result = ProcessReturnExpression(func, new[] { this.currentIdentifier });
                return result != null;
            }

            private bool TryProcessSelectMethod(MethodInvocationExpression methodInvoke, bool queryable)
            {
                Expression selectExpression;
                if (!TryProcessSingleParameterQuery(methodInvoke, true, queryable, out selectExpression))
                {
                    return false;
                }

                this.currentIdentifier = null;
                clauses.Add(new SelectClause(selectExpression, methodInvoke.InvocationInstructions));
                return true;
            }

            private bool TryProcessWhereMethod(MethodInvocationExpression methodInvoke, bool queryable)
            {
                Expression condition;
                if (!TryProcessSingleParameterQuery(methodInvoke, true, queryable, out condition))
                {
                    return false;
                }

                clauses.Add(new WhereClause(condition, methodInvoke.InvocationInstructions));
                return true;
            }

            private bool TryProcessOrderByMethod(MethodInvocationExpression methodInvoke, bool ascending, bool queryable)
            {
                Expression key;
                if (!TryProcessSingleParameterQuery(methodInvoke, true, queryable, out key))
                {
                    return false;
                }

                OrderByClause orderBy = new OrderByClause(new PairList<Expression, OrderDirection>(), methodInvoke.InvocationInstructions);
                orderBy.ExpressionToOrderDirectionMap.Add(key, ascending ? OrderDirection.Ascending : OrderDirection.Descending);
                clauses.Add(orderBy);

                return true;
            }

            private bool TryProcessThenByMethod(MethodInvocationExpression methodInvoke, bool ascending, bool queryable)
            {
                Expression key;
                if (!TryProcessSingleParameterQuery(methodInvoke, false, queryable, out key))
                {
                    return false;
                }

                OrderByClause orderBy = clauses[clauses.Count - 1] as OrderByClause;
                if (orderBy == null)
                {
                    return false;
                }
                orderBy.ExpressionToOrderDirectionMap.Add(key, ascending ? OrderDirection.Ascending : OrderDirection.Descending);
                clauses[clauses.Count - 1] = orderBy.CloneAndAttachInstructions(methodInvoke.InvocationInstructions) as QueryClause;
                return true;
            }

            private bool TryProcessSelectManyMethod(MethodInvocationExpression methodInvoke, bool queryable)
            {
                MethodDefinition methodDef = methodInvoke.MethodExpression.MethodDefinition;
                if (methodDef.Parameters.Count != 3)
                {
                    return false;
                }

                GenericInstanceType funcType = GetFuncGenericInstance(methodDef.Parameters[1].ParameterType as GenericInstanceType, queryable);
                if (funcType == null || funcType.GenericArguments.Count != 2)
                {
                    return false;
                }

                LambdaExpression collectionSelector = GetLambdaExpression(methodInvoke.Arguments[1], queryable);
                if (collectionSelector == null || collectionSelector.Arguments.Count != 1)
                {
                    return false;
                }

                ProcessCurrentIdentifier(methodInvoke.Arguments[0], collectionSelector.Parameters[0].Resolve());

                Expression collection = ProcessReturnExpression(collectionSelector, new[] { this.currentIdentifier });
                if (collection == null)
                {
                    return false;
                }

                LambdaExpression resultSelector = GetLambdaExpression(methodInvoke.Arguments[2], queryable);
                if (resultSelector == null || resultSelector.Arguments.Count != 2)
                {
                    return false;
                }

                VariableReference secondFromIdentifier = CreateNewIdentifier(resultSelector.Parameters[1].Name, resultSelector.Parameters[1].ParameterType);

                Expression result = ProcessReturnExpression(resultSelector, new[] { this.currentIdentifier, secondFromIdentifier });
                if (result == null)
                {
                    return false;
                }

                Expression secondIdentifierReference = GetIdentifierReference(secondFromIdentifier, ref collection);
                clauses.Add(new FromClause(secondIdentifierReference, collection, null));
                clauses.Add(new SelectClause(result, methodInvoke.InvocationInstructions));
                this.currentIdentifier = null;
                return true;
            }

            private GenericInstanceType GetFuncGenericInstance(GenericInstanceType type, bool queryable)
            {
                if (!queryable)
                {
                    return type;
                }

                if (type == null || type.GenericArguments.Count != 1)
                {
                    return null;
                }

                return type.GenericArguments[0] as GenericInstanceType;
            }

            private bool TryProcessJoinMethod(MethodInvocationExpression methodInvoke, bool isGroupJoin, bool queryable)
            {
                MethodDefinition methodDef = methodInvoke.MethodExpression.MethodDefinition;
                if (methodDef.Parameters.Count != 5)
                {
                    return false;
                }

                Expression innerCollection = (Expression)new LinqQueriesRebuilder(this.methodContext).Visit(methodInvoke.Arguments[1].Clone());

                LambdaExpression outerKeySelector = GetLambdaExpression(methodInvoke.Arguments[2], queryable);
                if (outerKeySelector == null || outerKeySelector.Arguments.Count != 1)
                {
                    return false;
                }

                ProcessCurrentIdentifier(methodInvoke.Arguments[0], outerKeySelector.Parameters[0].Resolve());

                Expression outerKey = ProcessReturnExpression(outerKeySelector, new[] { this.currentIdentifier });
                if(outerKey == null)
                {
                    return false;
                }

                LambdaExpression innerKeySelector = GetLambdaExpression(methodInvoke.Arguments[3], queryable);
                if (innerKeySelector == null || innerKeySelector.Arguments.Count != 1)
                {
                    return false;
                }

                VariableReference innerIdentifier = CreateNewIdentifier(innerKeySelector.Parameters[0].Name, innerKeySelector.Parameters[0].ParameterType);

                Expression innerKey = ProcessReturnExpression(innerKeySelector, new[] { innerIdentifier });
                if(innerKey == null)
                {
                    return false;
                }

                LambdaExpression resultSelector = GetLambdaExpression(methodInvoke.Arguments[4], queryable);
                if (resultSelector == null || resultSelector.Arguments.Count != 2)
                {
                    return false;
                }

                ParameterReference[] resultSelectorParameters = resultSelector.Parameters;
                VariableReference resultSelectorSecondIdentifier =
                    isGroupJoin ? CreateNewIdentifier(resultSelectorParameters[1].Name, resultSelectorParameters[1].ParameterType) : innerIdentifier;

                Expression result = ProcessReturnExpression(resultSelector, new[] { this.currentIdentifier, resultSelectorSecondIdentifier });
                if (result == null)
                {
                    return false;
                }

                clauses.Add(new JoinClause(GetIdentifierReference(innerIdentifier, ref innerCollection), innerCollection, outerKey, innerKey, methodInvoke.InvocationInstructions));

                if(isGroupJoin)
                {
                    clauses.Add(new IntoClause(new VariableReferenceExpression(resultSelectorSecondIdentifier, null), null));
                }

                clauses.Add(new SelectClause(result, null));
                this.currentIdentifier = null;

                return true;
            }

            private bool TryProcessGroupByMethod(MethodInvocationExpression methodInvoke, bool queryable)
            {
                MethodDefinition methodDef = methodInvoke.MethodExpression.MethodDefinition;
                if (methodDef.Parameters.Count > 3 || methodDef.Parameters.Count != methodDef.GenericParameters.Count ||
                    methodDef.GenericParameters.Count == 3 && methodDef.GenericParameters[2].Name != "TElement")
                {
                    return false;
                }

                LambdaExpression keySelector = GetLambdaExpression(methodInvoke.Arguments[1], queryable);
                if (keySelector == null || keySelector.Arguments.Count != 1)
                {
                    return false;
                }

                ProcessCurrentIdentifier(methodInvoke.Arguments[0], keySelector.Parameters[0].Resolve());

                Expression keyExpression = ProcessReturnExpression(keySelector, new[] { this.currentIdentifier });
                if(keyExpression == null)
                {
                    return false;
                }

                Expression elementExpression;
                if (methodInvoke.Arguments.Count == 3)
                {
                    LambdaExpression elementSelector = GetLambdaExpression(methodInvoke.Arguments[2], queryable);
                    if (elementSelector == null || elementSelector.Arguments.Count != 1)
                    {
                        return false;
                    }

                    elementExpression = ProcessReturnExpression(elementSelector, new[] { this.currentIdentifier });
                    if (elementExpression == null)
                    {
                        return false;
                    }
                }
                else
                {
                    elementExpression = new VariableReferenceExpression(this.currentIdentifier, null);
                }

                clauses.Add(new GroupClause(elementExpression, keyExpression, methodInvoke.InvocationInstructions));
                this.currentIdentifier = null;
                return true;
            }

            private void ProcessCurrentIdentifier(Expression target, ParameterDefinition identifierParameter)
            {
                if (clauses.Count == 0)
                {
                    AddInitialFromClause(target, identifierParameter.Name, identifierParameter.ParameterType);
                }
                else if (this.currentIdentifier == null || this.currentIdentifier.Name != identifierParameter.Name/* || this.currentIdentifier.VariableType != identifierParameter.ParameterType*/)
                {
                    AddNewIdentifier(identifierParameter);
                }
            }

            private Expression ProcessReturnExpression(LambdaExpression lambdaExpression, VariableReference[] identifiers)
            {
                if (lambdaExpression.Arguments.Count != identifiers.Length || lambdaExpression.Body.Statements.Count != 1 ||
                    lambdaExpression.Body.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
                {
                    return null;
                }
                
                ShortFormReturnExpression returnExpression = (lambdaExpression.Body.Statements[0] as ExpressionStatement).Expression as ShortFormReturnExpression;
                if (returnExpression == null)
                {
                    return null;
                }

                Dictionary<ParameterDefinition, VariableReference> parameterToIdentifierMap = new Dictionary<ParameterDefinition, VariableReference>();
                for (int i = 0; i < identifiers.Length; i++)
                {
                    parameterToIdentifierMap[lambdaExpression.Parameters[i].Resolve()] = identifiers[i];
                }

                Expression result = returnExpression.Value.CloneAndAttachInstructions(returnExpression.MappedInstructions);
                result = (Expression)new IdentifierReplacer(parameterToIdentifierMap).Visit(result);

                return result;
            }

            private void AddNewIdentifier(ParameterDefinition parameter)
            {
                VariableReference previousIdentifier = this.currentIdentifier;
                this.currentIdentifier = CreateNewIdentifier(parameter.Name, parameter.ParameterType);
                QueryClause previousClause = clauses[clauses.Count - 1];

                if (previousClause.CodeNodeType != CodeNodeType.SelectClause && previousClause.CodeNodeType != CodeNodeType.GroupClause)
                {
                    if (previousIdentifier == null)
                    {
                        throw new NullReferenceException("previousIdentifier");
                    }
                    clauses.Add(new SelectClause(new VariableReferenceExpression(previousIdentifier, null), null));
                }

                clauses.Add(new IntoClause(new VariableReferenceExpression(this.currentIdentifier, null), null));
            }

            private VariableDefinition CreateNewIdentifier(string name, TypeReference type)
            {
                VariableDefinition variable = new VariableDefinition(name, type, this.methodContext.Method);
                //methodContext.VariablesToRename.Add(variable);
                methodContext.UndeclaredLinqVariables.Add(variable);
                return variable;
            }

            private Expression GetIdentifierReference(VariableReference identifierReference, ref Expression target)
            {
                MethodInvocationExpression targetInvocation = target as MethodInvocationExpression;
                if (targetInvocation != null && targetInvocation.MethodExpression.MethodDefinition != null &&
                    targetInvocation.MethodExpression.MethodDefinition.IsStatic &&
                    (targetInvocation.MethodExpression.MethodDefinition.DeclaringType.FullName == "System.Linq.Enumerable" ||
                    targetInvocation.MethodExpression.MethodDefinition.DeclaringType.FullName == "System.Linq.Queryable") &&
                    targetInvocation.MethodExpression.Method.Name == "Cast")
                {
                    target = targetInvocation.Arguments[0];
                    methodContext.UndeclaredLinqVariables.Remove(identifierReference.Resolve());
                    return new VariableDeclarationExpression(identifierReference.Resolve(), null);
                }
                return new VariableReferenceExpression(identifierReference, null);
            }

            private void AddInitialFromClause(Expression target, string identifierName, TypeReference identifierType)
            {
                VariableDefinition variableDef;
                Expression identifierExpression;
                
                variableDef = CreateNewIdentifier(identifierName, identifierType);
                identifierExpression = GetIdentifierReference(variableDef, ref target);

                clauses.Add(new FromClause(identifierExpression, target, null));
                this.currentIdentifier = variableDef;
            }

            private LambdaExpression GetLambdaExpression(Expression expression, bool queryable)
            {
                if (queryable)
                {
                    LambdaExpression lambdaExpression = expression as LambdaExpression;
                    return lambdaExpression != null && lambdaExpression.IsExpressionTreeLambda ? lambdaExpression : null;
                }

                DelegateCreationExpression delegateCreation = expression as DelegateCreationExpression;
                if (delegateCreation != null && delegateCreation.MethodExpression.CodeNodeType == CodeNodeType.LambdaExpression)
                {
                    return delegateCreation.MethodExpression as LambdaExpression;
                }
                return null;
            }

            private LinqQueryExpression RemoveTransparentIdentifiers(LinqQueryExpression originalQuery)
            {
                LinqQueryExpression linqQuery = (LinqQueryExpression)originalQuery.Clone();
                List<VariableReference> identifiers = new List<VariableReference>();
                TransparentIdentifierCleaner cleaner = new TransparentIdentifierCleaner();
                HashSet<VariableReference> transparentIdentifiers = new HashSet<VariableReference>();

                for (int i = 0; i < linqQuery.Clauses.Count; i++)
                {
                    linqQuery.Clauses[i] = (QueryClause)cleaner.Visit(linqQuery.Clauses[i]);
                    QueryClause currentClause = linqQuery.Clauses[i];

                    if (currentClause.CodeNodeType == CodeNodeType.FromClause)
                    {
                        identifiers.Add(GetVariableReference((currentClause as FromClause).Identifier));
                    }
                    else if (currentClause.CodeNodeType == CodeNodeType.JoinClause)
                    {
                        if (linqQuery.Clauses[i + 1].CodeNodeType != CodeNodeType.IntoClause)
                        {
                            identifiers.Add(GetVariableReference((currentClause as JoinClause).InnerIdentifier));
                        }
                        else
                        {
                            identifiers.Add(((IntoClause)linqQuery.Clauses[i + 1]).Identifier.Variable);
                            i++;
                        }
                    }
                    else if (currentClause.CodeNodeType == CodeNodeType.SelectClause && i != linqQuery.Clauses.Count - 1)
                    {
                        VariableReference intoIdentifier = ((IntoClause)linqQuery.Clauses[i + 1]).Identifier.Variable;
                        if (IsTransparentIdentifier(intoIdentifier))
                        {
                            Dictionary<PropertyDefinition, Expression> propertyToValueMap = GetPropertyToValueMap((currentClause as SelectClause).Expression);
                            if (propertyToValueMap == null)
                            {
                                return originalQuery;
                            }

                            if (identifiers.Count == 2)
                            {
                                if (!RemoveIdentifier(propertyToValueMap, identifiers))
                                {
                                    return originalQuery;
                                }
                                linqQuery.Clauses.RemoveAt(i);
                                linqQuery.Clauses.RemoveAt(i);
                                i--;
                            }
                            else if (identifiers.Count == 1)
                            {
                                LetClause letClause = GenerateLetClause(propertyToValueMap, identifiers[0]);
                                if (letClause == null)
                                {
                                    return originalQuery;
                                }
                                linqQuery.Clauses[i] = letClause;
                                linqQuery.Clauses.RemoveAt(i + 1);
                            }
                            else
                            {
                                return originalQuery;
                            }

                            this.methodContext.VariablesToRename.Add(intoIdentifier.Resolve());
                            transparentIdentifiers.Add(intoIdentifier);
                            identifiers.Clear();
                            identifiers.Add(intoIdentifier);
                            UpdateCleaner(cleaner, intoIdentifier, propertyToValueMap);
                        }
                    }
                    else if (currentClause.CodeNodeType == CodeNodeType.IntoClause)
                    {
                        identifiers.Clear();
                        identifiers.Add(((IntoClause)currentClause).Identifier.Variable);
                    }
                }

                TransparentIdentifierFinder finder = new TransparentIdentifierFinder(transparentIdentifiers);
                if (finder.ContainsTransparentIdentifiers(linqQuery))
                {
                    return originalQuery;
                }

                return linqQuery;
            }

            private bool IsTransparentIdentifier(VariableReference identifier)
            {
                return identifier.Name.StartsWith("<>h__TransparentIdentifier") || identifier.Name.StartsWith("$VB$");
            }

            private bool RemoveIdentifier(Dictionary<PropertyDefinition, Expression> propertyToValueMap, List<VariableReference> oldIdentifiers)
            {
                HashSet<VariableReference> identifiersSet = new HashSet<VariableReference>(oldIdentifiers);

                foreach (VariableReferenceExpression value in propertyToValueMap.Values)
                {
                    if (!identifiersSet.Remove(value.Variable))
                    {
                        return false;
                    }
                }

                return true;
            }

            private LetClause GenerateLetClause(Dictionary<PropertyDefinition, Expression> propertyToValueMap, VariableReference oldIdentifier)
            {
                PropertyDefinition oldIdentifierProperty = null;
                PropertyDefinition newIdentifierProperty = null;
                foreach (KeyValuePair<PropertyDefinition, Expression> pair in propertyToValueMap)
                {
                    if (pair.Key.Name == oldIdentifier.Name && pair.Value.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                        (pair.Value as VariableReferenceExpression).Variable == oldIdentifier)
                    {
                        oldIdentifierProperty = pair.Key;
                    }
                    else
                    {
                        newIdentifierProperty = pair.Key;
                    }
                }

                if (oldIdentifierProperty == null || newIdentifierProperty == null)
                {
                    return null;
                }

                Expression newIdentifierValue = propertyToValueMap[newIdentifierProperty];
                
                VariableDefinition newIdentifier = CreateNewIdentifier(newIdentifierProperty.Name, newIdentifierValue.ExpressionType);
                
                LetClause result = new LetClause(new VariableReferenceExpression(newIdentifier, null), newIdentifierValue, null);
                propertyToValueMap[newIdentifierProperty] = result.Identifier;

                return result;
            }

            private void UpdateCleaner(TransparentIdentifierCleaner cleaner, VariableReference identifier, Dictionary<PropertyDefinition, Expression> propertyToValueMap)
            {
                Dictionary<MethodDefinition, VariableReference> methodDefToValue = new Dictionary<MethodDefinition,VariableReference>();
                foreach (KeyValuePair<PropertyDefinition, Expression> pair in propertyToValueMap)
                {
                    methodDefToValue.Add(pair.Key.GetMethod, ((VariableReferenceExpression)pair.Value).Variable);
                }
                cleaner.TransparentIdentifierToPropertyValueMap.Add(identifier, methodDefToValue);
            }

            private Dictionary<PropertyDefinition, Expression> GetPropertyToValueMap(Expression expression)
            {
                AnonymousObjectCreationExpression creationExpression = expression as AnonymousObjectCreationExpression;
                if (creationExpression == null || creationExpression.Initializer.Expressions.Count != 2)
                {
                    return null;
                }

                Dictionary<PropertyDefinition, Expression> result = new Dictionary<PropertyDefinition, Expression>();
                foreach (BinaryExpression initializerToValueAssignment in creationExpression.Initializer.Expressions)
                {
                    result.Add(((AnonymousPropertyInitializerExpression)initializerToValueAssignment.Left).Property, initializerToValueAssignment.Right);
                }
                return result;
            }

            private VariableReference GetVariableReference(Expression identifierReference)
            {
                if (identifierReference.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    return (identifierReference as VariableReferenceExpression).Variable;
                }
                if (identifierReference.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
                {
                    return (identifierReference as VariableDeclarationExpression).Variable;
                }
                throw new Exception("Invalid expression");
            }

            private class IdentifierReplacer : BaseCodeTransformer
            {
                private readonly Dictionary<ParameterDefinition, VariableReference> parameterToVariableMap;

                public IdentifierReplacer(Dictionary<ParameterDefinition, VariableReference> parameterToVariableMap)
                {
                    this.parameterToVariableMap = parameterToVariableMap;
                }

                public override ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
                {
                    ParameterReference parameterReference = node.Parameter;
                    VariableReference variableReference;
                    if (parameterToVariableMap.TryGetValue(parameterReference.Resolve(), out variableReference))
                    {
                        return new VariableReferenceExpression(variableReference, node.UnderlyingSameMethodInstructions);
                    }

                    return node;
                }
            }

            private class TransparentIdentifierCleaner : BaseCodeTransformer
            {
                public Dictionary<VariableReference, Dictionary<MethodDefinition, VariableReference>> TransparentIdentifierToPropertyValueMap { get; private set; }

                public TransparentIdentifierCleaner()
                {
                    this.TransparentIdentifierToPropertyValueMap = new Dictionary<VariableReference, Dictionary<MethodDefinition, VariableReference>>();
                }

                public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
                {
                    base.VisitPropertyReferenceExpression(node);

                    Dictionary<MethodDefinition, VariableReference> methodDefToIdentifierReference;
                    if (node.Target != null && node.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                        this.TransparentIdentifierToPropertyValueMap.TryGetValue((node.Target as VariableReferenceExpression).Variable, out methodDefToIdentifierReference))
                    {
                        return new VariableReferenceExpression(methodDefToIdentifierReference[node.MethodExpression.MethodDefinition], node.UnderlyingSameMethodInstructions);
                    }
                    return node;
                }
            }

            private class TransparentIdentifierFinder : BaseCodeVisitor
            {
                private readonly HashSet<VariableReference> transparentIdentifiers;
                private bool contains;

                public TransparentIdentifierFinder(HashSet<VariableReference> transparentIdentifiers)
                {
                    this.transparentIdentifiers = transparentIdentifiers;
                }

                public bool ContainsTransparentIdentifiers(ICodeNode node)
                {
                    contains = false;
                    Visit(node);
                    return contains;
                }

                public override void Visit(ICodeNode node)
                {
                    if (contains)
                    {
                        return;
                    }
                    base.Visit(node);
                }

                public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
                {
                    if (this.transparentIdentifiers.Contains(node.Variable))
                    {
                        contains = true;
                    }
                }
            }
        }
    }
}
