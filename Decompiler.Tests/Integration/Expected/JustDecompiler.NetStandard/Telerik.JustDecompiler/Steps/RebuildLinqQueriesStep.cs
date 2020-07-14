using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildLinqQueriesStep : IDecompilationStep
	{
		public RebuildLinqQueriesStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			body = (BlockStatement)(new RebuildLinqQueriesStep.LinqQueriesRebuilder(context.MethodContext)).Visit(body);
			return body;
		}

		private class LinqQueriesRebuilder : BaseCodeTransformer
		{
			private VariableReference currentIdentifier;

			private readonly List<QueryClause> clauses;

			private readonly MethodSpecificContext methodContext;

			public LinqQueriesRebuilder(MethodSpecificContext methodContext)
			{
				this.methodContext = methodContext;
			}

			private void AddInitialFromClause(Expression target, string identifierName, TypeReference identifierType)
			{
				VariableDefinition variableDefinition = this.CreateNewIdentifier(identifierName, identifierType);
				Expression identifierReference = this.GetIdentifierReference(variableDefinition, ref target);
				this.clauses.Add(new FromClause(identifierReference, target, null));
				this.currentIdentifier = variableDefinition;
			}

			private void AddNewIdentifier(ParameterDefinition parameter)
			{
				VariableReference variableReference = this.currentIdentifier;
				this.currentIdentifier = this.CreateNewIdentifier(parameter.get_Name(), parameter.get_ParameterType());
				QueryClause item = this.clauses[this.clauses.Count - 1];
				if (item.CodeNodeType != CodeNodeType.SelectClause && item.CodeNodeType != CodeNodeType.GroupClause)
				{
					if (variableReference == null)
					{
						throw new NullReferenceException("previousIdentifier");
					}
					this.clauses.Add(new SelectClause(new VariableReferenceExpression(variableReference, null), null));
				}
				this.clauses.Add(new IntoClause(new VariableReferenceExpression(this.currentIdentifier, null), null));
			}

			private VariableDefinition CreateNewIdentifier(string name, TypeReference type)
			{
				VariableDefinition variableDefinition = new VariableDefinition(name, type, this.methodContext.Method);
				this.methodContext.UndeclaredLinqVariables.Add(variableDefinition);
				return variableDefinition;
			}

			private LetClause GenerateLetClause(Dictionary<PropertyDefinition, Expression> propertyToValueMap, VariableReference oldIdentifier)
			{
				PropertyDefinition key = null;
				PropertyDefinition identifier = null;
				foreach (KeyValuePair<PropertyDefinition, Expression> keyValuePair in propertyToValueMap)
				{
					if (!(keyValuePair.Key.get_Name() == oldIdentifier.get_Name()) || keyValuePair.Value.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(keyValuePair.Value as VariableReferenceExpression).Variable != (object)oldIdentifier)
					{
						identifier = keyValuePair.Key;
					}
					else
					{
						key = keyValuePair.Key;
					}
				}
				if (key == null || identifier == null)
				{
					return null;
				}
				Expression item = propertyToValueMap[identifier];
				LetClause letClause = new LetClause(new VariableReferenceExpression(this.CreateNewIdentifier(identifier.get_Name(), item.ExpressionType), null), item, null);
				propertyToValueMap[identifier] = letClause.Identifier;
				return letClause;
			}

			private GenericInstanceType GetFuncGenericInstance(GenericInstanceType type, bool queryable)
			{
				if (!queryable)
				{
					return type;
				}
				if (type == null || type.get_GenericArguments().get_Count() != 1)
				{
					return null;
				}
				return type.get_GenericArguments().get_Item(0) as GenericInstanceType;
			}

			private Expression GetIdentifierReference(VariableReference identifierReference, ref Expression target)
			{
				MethodInvocationExpression methodInvocationExpression = target as MethodInvocationExpression;
				if (methodInvocationExpression == null || methodInvocationExpression.MethodExpression.MethodDefinition == null || !methodInvocationExpression.MethodExpression.MethodDefinition.get_IsStatic() || !(methodInvocationExpression.MethodExpression.MethodDefinition.get_DeclaringType().get_FullName() == "System.Linq.Enumerable") && !(methodInvocationExpression.MethodExpression.MethodDefinition.get_DeclaringType().get_FullName() == "System.Linq.Queryable") || !(methodInvocationExpression.MethodExpression.Method.get_Name() == "Cast"))
				{
					return new VariableReferenceExpression(identifierReference, null);
				}
				target = methodInvocationExpression.Arguments[0];
				this.methodContext.UndeclaredLinqVariables.Remove(identifierReference.Resolve());
				return new VariableDeclarationExpression(identifierReference.Resolve(), null);
			}

			private LambdaExpression GetLambdaExpression(Expression expression, bool queryable)
			{
				if (queryable)
				{
					LambdaExpression lambdaExpression = expression as LambdaExpression;
					if (lambdaExpression != null && lambdaExpression.IsExpressionTreeLambda)
					{
						return lambdaExpression;
					}
					return null;
				}
				DelegateCreationExpression delegateCreationExpression = expression as DelegateCreationExpression;
				if (delegateCreationExpression == null || delegateCreationExpression.MethodExpression.CodeNodeType != CodeNodeType.LambdaExpression)
				{
					return null;
				}
				return delegateCreationExpression.MethodExpression as LambdaExpression;
			}

			private Dictionary<PropertyDefinition, Expression> GetPropertyToValueMap(Expression expression)
			{
				AnonymousObjectCreationExpression anonymousObjectCreationExpression = expression as AnonymousObjectCreationExpression;
				if (anonymousObjectCreationExpression == null || anonymousObjectCreationExpression.Initializer.Expressions.Count != 2)
				{
					return null;
				}
				Dictionary<PropertyDefinition, Expression> propertyDefinitions = new Dictionary<PropertyDefinition, Expression>();
				foreach (BinaryExpression binaryExpression in anonymousObjectCreationExpression.Initializer.Expressions)
				{
					propertyDefinitions.Add(((AnonymousPropertyInitializerExpression)binaryExpression.Left).Property, binaryExpression.Right);
				}
				return propertyDefinitions;
			}

			private VariableReference GetVariableReference(Expression identifierReference)
			{
				if (identifierReference.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					return (identifierReference as VariableReferenceExpression).Variable;
				}
				if (identifierReference.CodeNodeType != CodeNodeType.VariableDeclarationExpression)
				{
					throw new Exception("Invalid expression");
				}
				return (identifierReference as VariableDeclarationExpression).Variable;
			}

			private bool IsTransparentIdentifier(VariableReference identifier)
			{
				if (identifier.get_Name().StartsWith("<>h__TransparentIdentifier"))
				{
					return true;
				}
				return identifier.get_Name().StartsWith("$VB$");
			}

			private void ProcessCurrentIdentifier(Expression target, ParameterDefinition identifierParameter)
			{
				if (this.clauses.Count == 0)
				{
					this.AddInitialFromClause(target, identifierParameter.get_Name(), identifierParameter.get_ParameterType());
					return;
				}
				if (this.currentIdentifier == null || this.currentIdentifier.get_Name() != identifierParameter.get_Name())
				{
					this.AddNewIdentifier(identifierParameter);
				}
			}

			private LinqQueryExpression ProcessExtensionMethodChain(IEnumerable<MethodInvocationExpression> methodInvocations, MethodInvocationExpression topInvoke, bool queryable)
			{
				LinqQueryExpression linqQueryExpression;
				this.currentIdentifier = null;
				this.clauses.Clear();
				using (IEnumerator<MethodInvocationExpression> enumerator = methodInvocations.GetEnumerator())
				{
					while (true)
					{
						if (enumerator.MoveNext())
						{
							MethodInvocationExpression current = enumerator.Current;
							bool flag = true;
							string name = current.MethodExpression.MethodDefinition.get_Name();
							if (name == null)
							{
								break;
							}
							switch (name)
							{
								case "Select":
								{
									flag = this.TryProcessSelectMethod(current, queryable);
									break;
								}
								case "SelectMany":
								{
									flag = this.TryProcessSelectManyMethod(current, queryable);
									break;
								}
								case "Where":
								{
									flag = this.TryProcessWhereMethod(current, queryable);
									break;
								}
								case "OrderBy":
								{
									flag = this.TryProcessOrderByMethod(current, true, queryable);
									break;
								}
								case "OrderByDescending":
								{
									flag = this.TryProcessOrderByMethod(current, false, queryable);
									break;
								}
								case "ThenBy":
								{
									flag = this.TryProcessThenByMethod(current, true, queryable);
									break;
								}
								case "ThenByDescending":
								{
									flag = this.TryProcessThenByMethod(current, false, queryable);
									break;
								}
								case "Join":
								{
									flag = this.TryProcessJoinMethod(current, false, queryable);
									break;
								}
								case "GroupJoin":
								{
									flag = this.TryProcessJoinMethod(current, true, queryable);
									break;
								}
								default:
								{
									if (name != "GroupBy")
									{
										break;
									}
									flag = this.TryProcessGroupByMethod(current, queryable);
									break;
								}
							}
							if (!flag)
							{
								linqQueryExpression = null;
								return linqQueryExpression;
							}
						}
						else
						{
							if (this.clauses.Count == 0)
							{
								return null;
							}
							if (this.currentIdentifier != null)
							{
								this.clauses.Add(new SelectClause(new VariableReferenceExpression(this.currentIdentifier, null), null));
							}
							return new LinqQueryExpression(new List<QueryClause>(this.clauses), topInvoke.ExpressionType, null);
						}
					}
					linqQueryExpression = null;
				}
				return linqQueryExpression;
			}

			private Expression ProcessReturnExpression(LambdaExpression lambdaExpression, VariableReference[] identifiers)
			{
				if (lambdaExpression.Arguments.Count != (int)identifiers.Length || lambdaExpression.Body.Statements.Count != 1 || lambdaExpression.Body.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
				{
					return null;
				}
				ShortFormReturnExpression expression = (lambdaExpression.Body.Statements[0] as ExpressionStatement).Expression as ShortFormReturnExpression;
				if (expression == null)
				{
					return null;
				}
				Dictionary<ParameterDefinition, VariableReference> parameterDefinitions = new Dictionary<ParameterDefinition, VariableReference>();
				for (int i = 0; i < (int)identifiers.Length; i++)
				{
					parameterDefinitions[lambdaExpression.Parameters[i].Resolve()] = identifiers[i];
				}
				Expression expression1 = expression.Value.CloneAndAttachInstructions(expression.MappedInstructions);
				return (Expression)(new RebuildLinqQueriesStep.LinqQueriesRebuilder.IdentifierReplacer(parameterDefinitions)).Visit(expression1);
			}

			private bool RemoveIdentifier(Dictionary<PropertyDefinition, Expression> propertyToValueMap, List<VariableReference> oldIdentifiers)
			{
				bool flag;
				HashSet<VariableReference> variableReferences = new HashSet<VariableReference>(oldIdentifiers);
				Dictionary<PropertyDefinition, Expression>.ValueCollection.Enumerator enumerator = propertyToValueMap.Values.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (variableReferences.Remove(((VariableReferenceExpression)enumerator.Current).Variable))
						{
							continue;
						}
						flag = false;
						return flag;
					}
					return true;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}

			private LinqQueryExpression RemoveTransparentIdentifiers(LinqQueryExpression originalQuery)
			{
				LinqQueryExpression linqQueryExpression = (LinqQueryExpression)originalQuery.Clone();
				List<VariableReference> variableReferences = new List<VariableReference>();
				RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierCleaner transparentIdentifierCleaner = new RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierCleaner();
				HashSet<VariableReference> variableReferences1 = new HashSet<VariableReference>();
				for (int i = 0; i < linqQueryExpression.Clauses.Count; i++)
				{
					linqQueryExpression.Clauses[i] = (QueryClause)transparentIdentifierCleaner.Visit(linqQueryExpression.Clauses[i]);
					QueryClause item = linqQueryExpression.Clauses[i];
					if (item.CodeNodeType == CodeNodeType.FromClause)
					{
						variableReferences.Add(this.GetVariableReference((item as FromClause).Identifier));
					}
					else if (item.CodeNodeType == CodeNodeType.JoinClause)
					{
						if (linqQueryExpression.Clauses[i + 1].CodeNodeType == CodeNodeType.IntoClause)
						{
							variableReferences.Add(((IntoClause)linqQueryExpression.Clauses[i + 1]).Identifier.Variable);
							i++;
						}
						else
						{
							variableReferences.Add(this.GetVariableReference((item as JoinClause).InnerIdentifier));
						}
					}
					else if (item.CodeNodeType == CodeNodeType.SelectClause && i != linqQueryExpression.Clauses.Count - 1)
					{
						VariableReference variable = ((IntoClause)linqQueryExpression.Clauses[i + 1]).Identifier.Variable;
						if (this.IsTransparentIdentifier(variable))
						{
							Dictionary<PropertyDefinition, Expression> propertyToValueMap = this.GetPropertyToValueMap((item as SelectClause).Expression);
							if (propertyToValueMap == null)
							{
								return originalQuery;
							}
							if (variableReferences.Count != 2)
							{
								if (variableReferences.Count != 1)
								{
									return originalQuery;
								}
								LetClause letClause = this.GenerateLetClause(propertyToValueMap, variableReferences[0]);
								if (letClause == null)
								{
									return originalQuery;
								}
								linqQueryExpression.Clauses[i] = letClause;
								linqQueryExpression.Clauses.RemoveAt(i + 1);
							}
							else
							{
								if (!this.RemoveIdentifier(propertyToValueMap, variableReferences))
								{
									return originalQuery;
								}
								linqQueryExpression.Clauses.RemoveAt(i);
								linqQueryExpression.Clauses.RemoveAt(i);
								i--;
							}
							this.methodContext.VariablesToRename.Add(variable.Resolve());
							variableReferences1.Add(variable);
							variableReferences.Clear();
							variableReferences.Add(variable);
							this.UpdateCleaner(transparentIdentifierCleaner, variable, propertyToValueMap);
						}
					}
					else if (item.CodeNodeType == CodeNodeType.IntoClause)
					{
						variableReferences.Clear();
						variableReferences.Add(((IntoClause)item).Identifier.Variable);
					}
				}
				if ((new RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierFinder(variableReferences1)).ContainsTransparentIdentifiers(linqQueryExpression))
				{
					return originalQuery;
				}
				return linqQueryExpression;
			}

			private bool TryMatchLinqQuery(MethodInvocationExpression methodInvocation, out LinqQueryExpression linqQuery)
			{
				Stack<MethodInvocationExpression> methodInvocationExpressions = new Stack<MethodInvocationExpression>();
				MethodInvocationExpression item = methodInvocation;
				bool flag = item.MethodExpression.MethodDefinition.IsQueryableMethod();
				while (item != null && item.MethodExpression.MethodDefinition.IsQueryMethod() && flag == item.MethodExpression.MethodDefinition.IsQueryableMethod())
				{
					methodInvocationExpressions.Push(item);
					item = item.Arguments[0] as MethodInvocationExpression;
				}
				if (methodInvocationExpressions.Count == 0)
				{
					linqQuery = null;
					return false;
				}
				MethodInvocationExpression methodInvocationExpression = methodInvocationExpressions.Peek();
				methodInvocationExpression.Arguments[0] = (Expression)this.Visit(methodInvocationExpression.Arguments[0]);
				linqQuery = this.ProcessExtensionMethodChain(methodInvocationExpressions, methodInvocation, flag);
				return linqQuery != null;
			}

			private bool TryProcessGroupByMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				Expression variableReferenceExpression;
				MethodDefinition methodDefinition = methodInvoke.MethodExpression.MethodDefinition;
				if (methodDefinition.get_Parameters().get_Count() > 3 || methodDefinition.get_Parameters().get_Count() != methodDefinition.get_GenericParameters().get_Count() || methodDefinition.get_GenericParameters().get_Count() == 3 && methodDefinition.get_GenericParameters().get_Item(2).get_Name() != "TElement")
				{
					return false;
				}
				LambdaExpression lambdaExpression = this.GetLambdaExpression(methodInvoke.Arguments[1], queryable);
				if (lambdaExpression == null || lambdaExpression.Arguments.Count != 1)
				{
					return false;
				}
				this.ProcessCurrentIdentifier(methodInvoke.Arguments[0], lambdaExpression.Parameters[0].Resolve());
				Expression expression = this.ProcessReturnExpression(lambdaExpression, new VariableReference[] { this.currentIdentifier });
				if (expression == null)
				{
					return false;
				}
				if (methodInvoke.Arguments.Count != 3)
				{
					variableReferenceExpression = new VariableReferenceExpression(this.currentIdentifier, null);
				}
				else
				{
					LambdaExpression lambdaExpression1 = this.GetLambdaExpression(methodInvoke.Arguments[2], queryable);
					if (lambdaExpression1 == null || lambdaExpression1.Arguments.Count != 1)
					{
						return false;
					}
					variableReferenceExpression = this.ProcessReturnExpression(lambdaExpression1, new VariableReference[] { this.currentIdentifier });
					if (variableReferenceExpression == null)
					{
						return false;
					}
				}
				this.clauses.Add(new GroupClause(variableReferenceExpression, expression, methodInvoke.InvocationInstructions));
				this.currentIdentifier = null;
				return true;
			}

			private bool TryProcessJoinMethod(MethodInvocationExpression methodInvoke, bool isGroupJoin, bool queryable)
			{
				VariableReference variableReference;
				if (methodInvoke.MethodExpression.MethodDefinition.get_Parameters().get_Count() != 5)
				{
					return false;
				}
				Expression expression = (Expression)(new RebuildLinqQueriesStep.LinqQueriesRebuilder(this.methodContext)).Visit(methodInvoke.Arguments[1].Clone());
				LambdaExpression lambdaExpression = this.GetLambdaExpression(methodInvoke.Arguments[2], queryable);
				if (lambdaExpression == null || lambdaExpression.Arguments.Count != 1)
				{
					return false;
				}
				this.ProcessCurrentIdentifier(methodInvoke.Arguments[0], lambdaExpression.Parameters[0].Resolve());
				Expression expression1 = this.ProcessReturnExpression(lambdaExpression, new VariableReference[] { this.currentIdentifier });
				if (expression1 == null)
				{
					return false;
				}
				LambdaExpression lambdaExpression1 = this.GetLambdaExpression(methodInvoke.Arguments[3], queryable);
				if (lambdaExpression1 == null || lambdaExpression1.Arguments.Count != 1)
				{
					return false;
				}
				VariableReference variableReference1 = this.CreateNewIdentifier(lambdaExpression1.Parameters[0].get_Name(), lambdaExpression1.Parameters[0].get_ParameterType());
				Expression expression2 = this.ProcessReturnExpression(lambdaExpression1, new VariableReference[] { variableReference1 });
				if (expression2 == null)
				{
					return false;
				}
				LambdaExpression lambdaExpression2 = this.GetLambdaExpression(methodInvoke.Arguments[4], queryable);
				if (lambdaExpression2 == null || lambdaExpression2.Arguments.Count != 2)
				{
					return false;
				}
				ParameterReference[] parameters = lambdaExpression2.Parameters;
				if (isGroupJoin)
				{
					variableReference = this.CreateNewIdentifier(parameters[1].get_Name(), parameters[1].get_ParameterType());
				}
				else
				{
					variableReference = variableReference1;
				}
				VariableReference variableReference2 = variableReference;
				Expression expression3 = this.ProcessReturnExpression(lambdaExpression2, new VariableReference[] { this.currentIdentifier, variableReference2 });
				if (expression3 == null)
				{
					return false;
				}
				this.clauses.Add(new JoinClause(this.GetIdentifierReference(variableReference1, ref expression), expression, expression1, expression2, methodInvoke.InvocationInstructions));
				if (isGroupJoin)
				{
					this.clauses.Add(new IntoClause(new VariableReferenceExpression(variableReference2, null), null));
				}
				this.clauses.Add(new SelectClause(expression3, null));
				this.currentIdentifier = null;
				return true;
			}

			private bool TryProcessOrderByMethod(MethodInvocationExpression methodInvoke, bool ascending, bool queryable)
			{
				Expression expression;
				if (!this.TryProcessSingleParameterQuery(methodInvoke, true, queryable, out expression))
				{
					return false;
				}
				OrderByClause orderByClause = new OrderByClause(new PairList<Expression, OrderDirection>(), methodInvoke.InvocationInstructions);
				orderByClause.ExpressionToOrderDirectionMap.Add(expression, (ascending ? OrderDirection.Ascending : OrderDirection.Descending));
				this.clauses.Add(orderByClause);
				return true;
			}

			private bool TryProcessSelectManyMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				MethodDefinition methodDefinition = methodInvoke.MethodExpression.MethodDefinition;
				if (methodDefinition.get_Parameters().get_Count() != 3)
				{
					return false;
				}
				GenericInstanceType funcGenericInstance = this.GetFuncGenericInstance(methodDefinition.get_Parameters().get_Item(1).get_ParameterType() as GenericInstanceType, queryable);
				if (funcGenericInstance == null || funcGenericInstance.get_GenericArguments().get_Count() != 2)
				{
					return false;
				}
				LambdaExpression lambdaExpression = this.GetLambdaExpression(methodInvoke.Arguments[1], queryable);
				if (lambdaExpression == null || lambdaExpression.Arguments.Count != 1)
				{
					return false;
				}
				this.ProcessCurrentIdentifier(methodInvoke.Arguments[0], lambdaExpression.Parameters[0].Resolve());
				Expression expression = this.ProcessReturnExpression(lambdaExpression, new VariableReference[] { this.currentIdentifier });
				if (expression == null)
				{
					return false;
				}
				LambdaExpression lambdaExpression1 = this.GetLambdaExpression(methodInvoke.Arguments[2], queryable);
				if (lambdaExpression1 == null || lambdaExpression1.Arguments.Count != 2)
				{
					return false;
				}
				VariableReference variableReference = this.CreateNewIdentifier(lambdaExpression1.Parameters[1].get_Name(), lambdaExpression1.Parameters[1].get_ParameterType());
				Expression expression1 = this.ProcessReturnExpression(lambdaExpression1, new VariableReference[] { this.currentIdentifier, variableReference });
				if (expression1 == null)
				{
					return false;
				}
				Expression identifierReference = this.GetIdentifierReference(variableReference, ref expression);
				this.clauses.Add(new FromClause(identifierReference, expression, null));
				this.clauses.Add(new SelectClause(expression1, methodInvoke.InvocationInstructions));
				this.currentIdentifier = null;
				return true;
			}

			private bool TryProcessSelectMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				Expression expression;
				if (!this.TryProcessSingleParameterQuery(methodInvoke, true, queryable, out expression))
				{
					return false;
				}
				this.currentIdentifier = null;
				this.clauses.Add(new SelectClause(expression, methodInvoke.InvocationInstructions));
				return true;
			}

			private bool TryProcessSingleParameterQuery(MethodInvocationExpression methodInvoke, bool canChangeIdentifier, bool queryable, out Expression result)
			{
				result = null;
				MethodDefinition methodDefinition = methodInvoke.MethodExpression.MethodDefinition;
				if (methodDefinition.get_Parameters().get_Count() != 2)
				{
					return false;
				}
				GenericInstanceType funcGenericInstance = this.GetFuncGenericInstance(methodDefinition.get_Parameters().get_Item(1).get_ParameterType() as GenericInstanceType, queryable);
				if (funcGenericInstance == null || funcGenericInstance.get_GenericArguments().get_Count() != 2)
				{
					return false;
				}
				LambdaExpression lambdaExpression = this.GetLambdaExpression(methodInvoke.Arguments[1], queryable);
				if (lambdaExpression == null || lambdaExpression.Arguments.Count != 1)
				{
					return false;
				}
				ParameterDefinition parameterDefinition = lambdaExpression.Parameters[0].Resolve();
				if (canChangeIdentifier)
				{
					this.ProcessCurrentIdentifier(methodInvoke.Arguments[0], parameterDefinition);
				}
				else if (this.currentIdentifier == null)
				{
					return false;
				}
				result = this.ProcessReturnExpression(lambdaExpression, new VariableReference[] { this.currentIdentifier });
				return result != null;
			}

			private bool TryProcessThenByMethod(MethodInvocationExpression methodInvoke, bool ascending, bool queryable)
			{
				Expression expression;
				if (!this.TryProcessSingleParameterQuery(methodInvoke, false, queryable, out expression))
				{
					return false;
				}
				OrderByClause item = this.clauses[this.clauses.Count - 1] as OrderByClause;
				if (item == null)
				{
					return false;
				}
				item.ExpressionToOrderDirectionMap.Add(expression, (ascending ? OrderDirection.Ascending : OrderDirection.Descending));
				this.clauses[this.clauses.Count - 1] = item.CloneAndAttachInstructions(methodInvoke.InvocationInstructions) as QueryClause;
				return true;
			}

			private bool TryProcessWhereMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				Expression expression;
				if (!this.TryProcessSingleParameterQuery(methodInvoke, true, queryable, out expression))
				{
					return false;
				}
				this.clauses.Add(new WhereClause(expression, methodInvoke.InvocationInstructions));
				return true;
			}

			private void UpdateCleaner(RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierCleaner cleaner, VariableReference identifier, Dictionary<PropertyDefinition, Expression> propertyToValueMap)
			{
				Dictionary<MethodDefinition, VariableReference> methodDefinitions = new Dictionary<MethodDefinition, VariableReference>();
				foreach (KeyValuePair<PropertyDefinition, Expression> keyValuePair in propertyToValueMap)
				{
					methodDefinitions.Add(keyValuePair.Key.get_GetMethod(), ((VariableReferenceExpression)keyValuePair.Value).Variable);
				}
				cleaner.TransparentIdentifierToPropertyValueMap.Add(identifier, methodDefinitions);
			}

			public override ICodeNode VisitLinqQueryExpression(LinqQueryExpression node)
			{
				return node;
			}

			public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				LinqQueryExpression linqQueryExpression;
				if (this.TryMatchLinqQuery(node, out linqQueryExpression))
				{
					return this.RemoveTransparentIdentifiers(linqQueryExpression);
				}
				return base.VisitMethodInvocationExpression(node);
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
					VariableReference variableReference;
					ParameterReference parameter = node.Parameter;
					if (!this.parameterToVariableMap.TryGetValue(parameter.Resolve(), out variableReference))
					{
						return node;
					}
					return new VariableReferenceExpression(variableReference, node.UnderlyingSameMethodInstructions);
				}
			}

			private class TransparentIdentifierCleaner : BaseCodeTransformer
			{
				public Dictionary<VariableReference, Dictionary<MethodDefinition, VariableReference>> TransparentIdentifierToPropertyValueMap
				{
					get;
					private set;
				}

				public TransparentIdentifierCleaner()
				{
					this.TransparentIdentifierToPropertyValueMap = new Dictionary<VariableReference, Dictionary<MethodDefinition, VariableReference>>();
				}

				public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
				{
					Dictionary<MethodDefinition, VariableReference> methodDefinitions;
					base.VisitPropertyReferenceExpression(node);
					if (node.Target == null || node.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.TransparentIdentifierToPropertyValueMap.TryGetValue((node.Target as VariableReferenceExpression).Variable, out methodDefinitions))
					{
						return node;
					}
					return new VariableReferenceExpression(methodDefinitions[node.MethodExpression.MethodDefinition], node.UnderlyingSameMethodInstructions);
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
					this.contains = false;
					this.Visit(node);
					return this.contains;
				}

				public override void Visit(ICodeNode node)
				{
					if (this.contains)
					{
						return;
					}
					base.Visit(node);
				}

				public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
				{
					if (this.transparentIdentifiers.Contains(node.Variable))
					{
						this.contains = true;
					}
				}
			}
		}
	}
}