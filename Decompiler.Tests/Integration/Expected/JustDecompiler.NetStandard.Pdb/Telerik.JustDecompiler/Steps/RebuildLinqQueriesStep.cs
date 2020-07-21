using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildLinqQueriesStep : IDecompilationStep
	{
		public RebuildLinqQueriesStep()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			body = (BlockStatement)(new RebuildLinqQueriesStep.LinqQueriesRebuilder(context.get_MethodContext())).Visit(body);
			return body;
		}

		private class LinqQueriesRebuilder : BaseCodeTransformer
		{
			private VariableReference currentIdentifier;

			private readonly List<QueryClause> clauses;

			private readonly MethodSpecificContext methodContext;

			public LinqQueriesRebuilder(MethodSpecificContext methodContext)
			{
				this.clauses = new List<QueryClause>();
				base();
				this.methodContext = methodContext;
				return;
			}

			private void AddInitialFromClause(Expression target, string identifierName, TypeReference identifierType)
			{
				V_0 = this.CreateNewIdentifier(identifierName, identifierType);
				V_1 = this.GetIdentifierReference(V_0, ref target);
				this.clauses.Add(new FromClause(V_1, target, null));
				this.currentIdentifier = V_0;
				return;
			}

			private void AddNewIdentifier(ParameterDefinition parameter)
			{
				V_0 = this.currentIdentifier;
				this.currentIdentifier = this.CreateNewIdentifier(parameter.get_Name(), parameter.get_ParameterType());
				V_1 = this.clauses.get_Item(this.clauses.get_Count() - 1);
				if (V_1.get_CodeNodeType() != 74 && V_1.get_CodeNodeType() != 76)
				{
					if (V_0 == null)
					{
						throw new NullReferenceException("previousIdentifier");
					}
					this.clauses.Add(new SelectClause(new VariableReferenceExpression(V_0, null), null));
				}
				this.clauses.Add(new IntoClause(new VariableReferenceExpression(this.currentIdentifier, null), null));
				return;
			}

			private VariableDefinition CreateNewIdentifier(string name, TypeReference type)
			{
				V_0 = new VariableDefinition(name, type, this.methodContext.get_Method());
				dummyVar0 = this.methodContext.get_UndeclaredLinqVariables().Add(V_0);
				return V_0;
			}

			private LetClause GenerateLetClause(Dictionary<PropertyDefinition, Expression> propertyToValueMap, VariableReference oldIdentifier)
			{
				V_0 = null;
				V_1 = null;
				V_4 = propertyToValueMap.GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (!String.op_Equality(V_5.get_Key().get_Name(), oldIdentifier.get_Name()) || V_5.get_Value().get_CodeNodeType() != 26 || (object)(V_5.get_Value() as VariableReferenceExpression).get_Variable() != (object)oldIdentifier)
						{
							V_1 = V_5.get_Key();
						}
						else
						{
							V_0 = V_5.get_Key();
						}
					}
				}
				finally
				{
					((IDisposable)V_4).Dispose();
				}
				if (V_0 == null || V_1 == null)
				{
					return null;
				}
				V_2 = propertyToValueMap.get_Item(V_1);
				V_3 = new LetClause(new VariableReferenceExpression(this.CreateNewIdentifier(V_1.get_Name(), V_2.get_ExpressionType()), null), V_2, null);
				propertyToValueMap.set_Item(V_1, V_3.get_Identifier());
				return V_3;
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
				V_0 = target as MethodInvocationExpression;
				if (V_0 == null || V_0.get_MethodExpression().get_MethodDefinition() == null || !V_0.get_MethodExpression().get_MethodDefinition().get_IsStatic() || !String.op_Equality(V_0.get_MethodExpression().get_MethodDefinition().get_DeclaringType().get_FullName(), "System.Linq.Enumerable") && !String.op_Equality(V_0.get_MethodExpression().get_MethodDefinition().get_DeclaringType().get_FullName(), "System.Linq.Queryable") || !String.op_Equality(V_0.get_MethodExpression().get_Method().get_Name(), "Cast"))
				{
					return new VariableReferenceExpression(identifierReference, null);
				}
				target = V_0.get_Arguments().get_Item(0);
				dummyVar0 = this.methodContext.get_UndeclaredLinqVariables().Remove(identifierReference.Resolve());
				return new VariableDeclarationExpression(identifierReference.Resolve(), null);
			}

			private LambdaExpression GetLambdaExpression(Expression expression, bool queryable)
			{
				if (queryable)
				{
					V_1 = expression as LambdaExpression;
					if (V_1 != null && V_1.get_IsExpressionTreeLambda())
					{
						return V_1;
					}
					return null;
				}
				V_0 = expression as DelegateCreationExpression;
				if (V_0 == null || V_0.get_MethodExpression().get_CodeNodeType() != 50)
				{
					return null;
				}
				return V_0.get_MethodExpression() as LambdaExpression;
			}

			private Dictionary<PropertyDefinition, Expression> GetPropertyToValueMap(Expression expression)
			{
				V_0 = expression as AnonymousObjectCreationExpression;
				if (V_0 == null || V_0.get_Initializer().get_Expressions().get_Count() != 2)
				{
					return null;
				}
				V_1 = new Dictionary<PropertyDefinition, Expression>();
				V_2 = V_0.get_Initializer().get_Expressions().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = (BinaryExpression)V_2.get_Current();
						V_1.Add(((AnonymousPropertyInitializerExpression)V_3.get_Left()).get_Property(), V_3.get_Right());
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
				return V_1;
			}

			private VariableReference GetVariableReference(Expression identifierReference)
			{
				if (identifierReference.get_CodeNodeType() == 26)
				{
					return (identifierReference as VariableReferenceExpression).get_Variable();
				}
				if (identifierReference.get_CodeNodeType() != 27)
				{
					throw new Exception("Invalid expression");
				}
				return (identifierReference as VariableDeclarationExpression).get_Variable();
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
				if (this.clauses.get_Count() == 0)
				{
					this.AddInitialFromClause(target, identifierParameter.get_Name(), identifierParameter.get_ParameterType());
					return;
				}
				if (this.currentIdentifier == null || String.op_Inequality(this.currentIdentifier.get_Name(), identifierParameter.get_Name()))
				{
					this.AddNewIdentifier(identifierParameter);
				}
				return;
			}

			private LinqQueryExpression ProcessExtensionMethodChain(IEnumerable<MethodInvocationExpression> methodInvocations, MethodInvocationExpression topInvoke, bool queryable)
			{
				this.currentIdentifier = null;
				this.clauses.Clear();
				V_0 = methodInvocations.GetEnumerator();
				try
				{
					while (true)
					{
						if (V_0.MoveNext())
						{
							V_1 = V_0.get_Current();
							V_2 = true;
							V_3 = V_1.get_MethodExpression().get_MethodDefinition().get_Name();
							if (V_3 == null)
							{
								break;
							}
							if (String.op_Equality(V_3, "Select"))
							{
								V_2 = this.TryProcessSelectMethod(V_1, queryable);
							}
							else
							{
								if (String.op_Equality(V_3, "SelectMany"))
								{
									V_2 = this.TryProcessSelectManyMethod(V_1, queryable);
								}
								else
								{
									if (String.op_Equality(V_3, "Where"))
									{
										V_2 = this.TryProcessWhereMethod(V_1, queryable);
									}
									else
									{
										if (String.op_Equality(V_3, "OrderBy"))
										{
											V_2 = this.TryProcessOrderByMethod(V_1, true, queryable);
										}
										else
										{
											if (String.op_Equality(V_3, "OrderByDescending"))
											{
												V_2 = this.TryProcessOrderByMethod(V_1, false, queryable);
											}
											else
											{
												if (String.op_Equality(V_3, "ThenBy"))
												{
													V_2 = this.TryProcessThenByMethod(V_1, true, queryable);
												}
												else
												{
													if (String.op_Equality(V_3, "ThenByDescending"))
													{
														V_2 = this.TryProcessThenByMethod(V_1, false, queryable);
													}
													else
													{
														if (String.op_Equality(V_3, "Join"))
														{
															V_2 = this.TryProcessJoinMethod(V_1, false, queryable);
														}
														else
														{
															if (String.op_Equality(V_3, "GroupJoin"))
															{
																V_2 = this.TryProcessJoinMethod(V_1, true, queryable);
															}
															else
															{
																if (!String.op_Equality(V_3, "GroupBy"))
																{
																	break;
																}
																V_2 = this.TryProcessGroupByMethod(V_1, queryable);
															}
														}
													}
												}
											}
										}
									}
								}
							}
							if (!V_2)
							{
								V_5 = null;
								goto Label1;
							}
						}
						else
						{
							goto Label0;
						}
					}
					V_5 = null;
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
			Label1:
				return V_5;
			Label0:
				if (this.clauses.get_Count() == 0)
				{
					return null;
				}
				if (this.currentIdentifier != null)
				{
					this.clauses.Add(new SelectClause(new VariableReferenceExpression(this.currentIdentifier, null), null));
				}
				return new LinqQueryExpression(new List<QueryClause>(this.clauses), topInvoke.get_ExpressionType(), null);
			}

			private Expression ProcessReturnExpression(LambdaExpression lambdaExpression, VariableReference[] identifiers)
			{
				if (lambdaExpression.get_Arguments().get_Count() != (int)identifiers.Length || lambdaExpression.get_Body().get_Statements().get_Count() != 1 || lambdaExpression.get_Body().get_Statements().get_Item(0).get_CodeNodeType() != 5)
				{
					return null;
				}
				V_0 = (lambdaExpression.get_Body().get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as ShortFormReturnExpression;
				if (V_0 == null)
				{
					return null;
				}
				V_1 = new Dictionary<ParameterDefinition, VariableReference>();
				V_3 = 0;
				while (V_3 < (int)identifiers.Length)
				{
					V_1.set_Item(lambdaExpression.get_Parameters()[V_3].Resolve(), identifiers[V_3]);
					V_3 = V_3 + 1;
				}
				V_2 = V_0.get_Value().CloneAndAttachInstructions(V_0.get_MappedInstructions());
				return (Expression)(new RebuildLinqQueriesStep.LinqQueriesRebuilder.IdentifierReplacer(V_1)).Visit(V_2);
			}

			private bool RemoveIdentifier(Dictionary<PropertyDefinition, Expression> propertyToValueMap, List<VariableReference> oldIdentifiers)
			{
				V_0 = new HashSet<VariableReference>(oldIdentifiers);
				V_1 = propertyToValueMap.get_Values().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = (VariableReferenceExpression)V_1.get_Current();
						if (V_0.Remove(V_2.get_Variable()))
						{
							continue;
						}
						V_3 = false;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			Label1:
				return V_3;
			Label0:
				return true;
			}

			private LinqQueryExpression RemoveTransparentIdentifiers(LinqQueryExpression originalQuery)
			{
				V_0 = (LinqQueryExpression)originalQuery.Clone();
				V_1 = new List<VariableReference>();
				V_2 = new RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierCleaner();
				V_3 = new HashSet<VariableReference>();
				V_4 = 0;
				while (V_4 < V_0.get_Clauses().get_Count())
				{
					V_0.get_Clauses().set_Item(V_4, (QueryClause)V_2.Visit(V_0.get_Clauses().get_Item(V_4)));
					V_5 = V_0.get_Clauses().get_Item(V_4);
					if (V_5.get_CodeNodeType() != 73)
					{
						if (V_5.get_CodeNodeType() != 78)
						{
							if (V_5.get_CodeNodeType() != 74 || V_4 == V_0.get_Clauses().get_Count() - 1)
							{
								if (V_5.get_CodeNodeType() == 80)
								{
									V_1.Clear();
									V_1.Add(((IntoClause)V_5).get_Identifier().get_Variable());
								}
							}
							else
							{
								V_6 = ((IntoClause)V_0.get_Clauses().get_Item(V_4 + 1)).get_Identifier().get_Variable();
								if (this.IsTransparentIdentifier(V_6))
								{
									V_7 = this.GetPropertyToValueMap((V_5 as SelectClause).get_Expression());
									if (V_7 == null)
									{
										return originalQuery;
									}
									if (V_1.get_Count() != 2)
									{
										if (V_1.get_Count() != 1)
										{
											return originalQuery;
										}
										V_8 = this.GenerateLetClause(V_7, V_1.get_Item(0));
										if (V_8 == null)
										{
											return originalQuery;
										}
										V_0.get_Clauses().set_Item(V_4, V_8);
										V_0.get_Clauses().RemoveAt(V_4 + 1);
									}
									else
									{
										if (!this.RemoveIdentifier(V_7, V_1))
										{
											return originalQuery;
										}
										V_0.get_Clauses().RemoveAt(V_4);
										V_0.get_Clauses().RemoveAt(V_4);
										V_4 = V_4 - 1;
									}
									dummyVar0 = this.methodContext.get_VariablesToRename().Add(V_6.Resolve());
									dummyVar1 = V_3.Add(V_6);
									V_1.Clear();
									V_1.Add(V_6);
									this.UpdateCleaner(V_2, V_6, V_7);
								}
							}
						}
						else
						{
							if (V_0.get_Clauses().get_Item(V_4 + 1).get_CodeNodeType() == 80)
							{
								V_1.Add(((IntoClause)V_0.get_Clauses().get_Item(V_4 + 1)).get_Identifier().get_Variable());
								V_4 = V_4 + 1;
							}
							else
							{
								V_1.Add(this.GetVariableReference((V_5 as JoinClause).get_InnerIdentifier()));
							}
						}
					}
					else
					{
						V_1.Add(this.GetVariableReference((V_5 as FromClause).get_Identifier()));
					}
					V_4 = V_4 + 1;
				}
				if ((new RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierFinder(V_3)).ContainsTransparentIdentifiers(V_0))
				{
					return originalQuery;
				}
				return V_0;
			}

			private bool TryMatchLinqQuery(MethodInvocationExpression methodInvocation, out LinqQueryExpression linqQuery)
			{
				V_0 = new Stack<MethodInvocationExpression>();
				V_1 = methodInvocation;
				V_2 = V_1.get_MethodExpression().get_MethodDefinition().IsQueryableMethod();
				while (V_1 != null && V_1.get_MethodExpression().get_MethodDefinition().IsQueryMethod() && V_2 == V_1.get_MethodExpression().get_MethodDefinition().IsQueryableMethod())
				{
					V_0.Push(V_1);
					V_1 = V_1.get_Arguments().get_Item(0) as MethodInvocationExpression;
				}
				if (V_0.get_Count() == 0)
				{
					linqQuery = null;
					return false;
				}
				V_3 = V_0.Peek();
				V_3.get_Arguments().set_Item(0, (Expression)this.Visit(V_3.get_Arguments().get_Item(0)));
				linqQuery = this.ProcessExtensionMethodChain(V_0, methodInvocation, V_2);
				return linqQuery != null;
			}

			private bool TryProcessGroupByMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				V_0 = methodInvoke.get_MethodExpression().get_MethodDefinition();
				if (V_0.get_Parameters().get_Count() > 3 || V_0.get_Parameters().get_Count() != V_0.get_GenericParameters().get_Count() || V_0.get_GenericParameters().get_Count() == 3 && String.op_Inequality(V_0.get_GenericParameters().get_Item(2).get_Name(), "TElement"))
				{
					return false;
				}
				V_1 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(1), queryable);
				if (V_1 == null || V_1.get_Arguments().get_Count() != 1)
				{
					return false;
				}
				this.ProcessCurrentIdentifier(methodInvoke.get_Arguments().get_Item(0), V_1.get_Parameters()[0].Resolve());
				stackVariable44 = new VariableReference[1];
				stackVariable44[0] = this.currentIdentifier;
				V_2 = this.ProcessReturnExpression(V_1, stackVariable44);
				if (V_2 == null)
				{
					return false;
				}
				if (methodInvoke.get_Arguments().get_Count() != 3)
				{
					V_3 = new VariableReferenceExpression(this.currentIdentifier, null);
				}
				else
				{
					V_4 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(2), queryable);
					if (V_4 == null || V_4.get_Arguments().get_Count() != 1)
					{
						return false;
					}
					stackVariable84 = new VariableReference[1];
					stackVariable84[0] = this.currentIdentifier;
					V_3 = this.ProcessReturnExpression(V_4, stackVariable84);
					if (V_3 == null)
					{
						return false;
					}
				}
				this.clauses.Add(new GroupClause(V_3, V_2, methodInvoke.get_InvocationInstructions()));
				this.currentIdentifier = null;
				return true;
			}

			private bool TryProcessJoinMethod(MethodInvocationExpression methodInvoke, bool isGroupJoin, bool queryable)
			{
				if (methodInvoke.get_MethodExpression().get_MethodDefinition().get_Parameters().get_Count() != 5)
				{
					return false;
				}
				V_0 = (Expression)(new RebuildLinqQueriesStep.LinqQueriesRebuilder(this.methodContext)).Visit(methodInvoke.get_Arguments().get_Item(1).Clone());
				V_1 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(2), queryable);
				if (V_1 == null || V_1.get_Arguments().get_Count() != 1)
				{
					return false;
				}
				this.ProcessCurrentIdentifier(methodInvoke.get_Arguments().get_Item(0), V_1.get_Parameters()[0].Resolve());
				stackVariable42 = new VariableReference[1];
				stackVariable42[0] = this.currentIdentifier;
				V_2 = this.ProcessReturnExpression(V_1, stackVariable42);
				if (V_2 == null)
				{
					return false;
				}
				V_3 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(3), queryable);
				if (V_3 == null || V_3.get_Arguments().get_Count() != 1)
				{
					return false;
				}
				V_4 = this.CreateNewIdentifier(V_3.get_Parameters()[0].get_Name(), V_3.get_Parameters()[0].get_ParameterType());
				stackVariable76 = new VariableReference[1];
				stackVariable76[0] = V_4;
				V_5 = this.ProcessReturnExpression(V_3, stackVariable76);
				if (V_5 == null)
				{
					return false;
				}
				V_6 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(4), queryable);
				if (V_6 == null || V_6.get_Arguments().get_Count() != 2)
				{
					return false;
				}
				V_7 = V_6.get_Parameters();
				if (isGroupJoin)
				{
					stackVariable106 = this.CreateNewIdentifier(V_7[1].get_Name(), V_7[1].get_ParameterType());
				}
				else
				{
					stackVariable106 = V_4;
				}
				V_8 = stackVariable106;
				stackVariable110 = new VariableReference[2];
				stackVariable110[0] = this.currentIdentifier;
				stackVariable110[1] = V_8;
				V_9 = this.ProcessReturnExpression(V_6, stackVariable110);
				if (V_9 == null)
				{
					return false;
				}
				this.clauses.Add(new JoinClause(this.GetIdentifierReference(V_4, ref V_0), V_0, V_2, V_5, methodInvoke.get_InvocationInstructions()));
				if (isGroupJoin)
				{
					this.clauses.Add(new IntoClause(new VariableReferenceExpression(V_8, null), null));
				}
				this.clauses.Add(new SelectClause(V_9, null));
				this.currentIdentifier = null;
				return true;
			}

			private bool TryProcessOrderByMethod(MethodInvocationExpression methodInvoke, bool ascending, bool queryable)
			{
				if (!this.TryProcessSingleParameterQuery(methodInvoke, true, queryable, out V_0))
				{
					return false;
				}
				V_1 = new OrderByClause(new PairList<Expression, OrderDirection>(), methodInvoke.get_InvocationInstructions());
				stackVariable11 = V_1.get_ExpressionToOrderDirectionMap();
				stackVariable12 = V_0;
				if (ascending)
				{
					stackVariable14 = 0;
				}
				else
				{
					stackVariable14 = 1;
				}
				stackVariable11.Add(stackVariable12, stackVariable14);
				this.clauses.Add(V_1);
				return true;
			}

			private bool TryProcessSelectManyMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				V_0 = methodInvoke.get_MethodExpression().get_MethodDefinition();
				if (V_0.get_Parameters().get_Count() != 3)
				{
					return false;
				}
				V_1 = this.GetFuncGenericInstance(V_0.get_Parameters().get_Item(1).get_ParameterType() as GenericInstanceType, queryable);
				if (V_1 == null || V_1.get_GenericArguments().get_Count() != 2)
				{
					return false;
				}
				V_2 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(1), queryable);
				if (V_2 == null || V_2.get_Arguments().get_Count() != 1)
				{
					return false;
				}
				this.ProcessCurrentIdentifier(methodInvoke.get_Arguments().get_Item(0), V_2.get_Parameters()[0].Resolve());
				stackVariable48 = new VariableReference[1];
				stackVariable48[0] = this.currentIdentifier;
				V_3 = this.ProcessReturnExpression(V_2, stackVariable48);
				if (V_3 == null)
				{
					return false;
				}
				V_4 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(2), queryable);
				if (V_4 == null || V_4.get_Arguments().get_Count() != 2)
				{
					return false;
				}
				V_5 = this.CreateNewIdentifier(V_4.get_Parameters()[1].get_Name(), V_4.get_Parameters()[1].get_ParameterType());
				stackVariable82 = new VariableReference[2];
				stackVariable82[0] = this.currentIdentifier;
				stackVariable82[1] = V_5;
				V_6 = this.ProcessReturnExpression(V_4, stackVariable82);
				if (V_6 == null)
				{
					return false;
				}
				V_7 = this.GetIdentifierReference(V_5, ref V_3);
				this.clauses.Add(new FromClause(V_7, V_3, null));
				this.clauses.Add(new SelectClause(V_6, methodInvoke.get_InvocationInstructions()));
				this.currentIdentifier = null;
				return true;
			}

			private bool TryProcessSelectMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				if (!this.TryProcessSingleParameterQuery(methodInvoke, true, queryable, out V_0))
				{
					return false;
				}
				this.currentIdentifier = null;
				this.clauses.Add(new SelectClause(V_0, methodInvoke.get_InvocationInstructions()));
				return true;
			}

			private bool TryProcessSingleParameterQuery(MethodInvocationExpression methodInvoke, bool canChangeIdentifier, bool queryable, out Expression result)
			{
				result = null;
				V_0 = methodInvoke.get_MethodExpression().get_MethodDefinition();
				if (V_0.get_Parameters().get_Count() != 2)
				{
					return false;
				}
				V_1 = this.GetFuncGenericInstance(V_0.get_Parameters().get_Item(1).get_ParameterType() as GenericInstanceType, queryable);
				if (V_1 == null || V_1.get_GenericArguments().get_Count() != 2)
				{
					return false;
				}
				V_2 = this.GetLambdaExpression(methodInvoke.get_Arguments().get_Item(1), queryable);
				if (V_2 == null || V_2.get_Arguments().get_Count() != 1)
				{
					return false;
				}
				V_3 = V_2.get_Parameters()[0].Resolve();
				if (!canChangeIdentifier)
				{
					if (this.currentIdentifier == null)
					{
						return false;
					}
				}
				else
				{
					this.ProcessCurrentIdentifier(methodInvoke.get_Arguments().get_Item(0), V_3);
				}
				stackVariable49 = new VariableReference[1];
				stackVariable49[0] = this.currentIdentifier;
				result = this.ProcessReturnExpression(V_2, stackVariable49);
				return result != null;
			}

			private bool TryProcessThenByMethod(MethodInvocationExpression methodInvoke, bool ascending, bool queryable)
			{
				if (!this.TryProcessSingleParameterQuery(methodInvoke, false, queryable, out V_0))
				{
					return false;
				}
				V_1 = this.clauses.get_Item(this.clauses.get_Count() - 1) as OrderByClause;
				if (V_1 == null)
				{
					return false;
				}
				stackVariable17 = V_1.get_ExpressionToOrderDirectionMap();
				stackVariable18 = V_0;
				if (ascending)
				{
					stackVariable20 = 0;
				}
				else
				{
					stackVariable20 = 1;
				}
				stackVariable17.Add(stackVariable18, stackVariable20);
				this.clauses.set_Item(this.clauses.get_Count() - 1, V_1.CloneAndAttachInstructions(methodInvoke.get_InvocationInstructions()) as QueryClause);
				return true;
			}

			private bool TryProcessWhereMethod(MethodInvocationExpression methodInvoke, bool queryable)
			{
				if (!this.TryProcessSingleParameterQuery(methodInvoke, true, queryable, out V_0))
				{
					return false;
				}
				this.clauses.Add(new WhereClause(V_0, methodInvoke.get_InvocationInstructions()));
				return true;
			}

			private void UpdateCleaner(RebuildLinqQueriesStep.LinqQueriesRebuilder.TransparentIdentifierCleaner cleaner, VariableReference identifier, Dictionary<PropertyDefinition, Expression> propertyToValueMap)
			{
				V_0 = new Dictionary<MethodDefinition, VariableReference>();
				V_1 = propertyToValueMap.GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_0.Add(V_2.get_Key().get_GetMethod(), ((VariableReferenceExpression)V_2.get_Value()).get_Variable());
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
				cleaner.get_TransparentIdentifierToPropertyValueMap().Add(identifier, V_0);
				return;
			}

			public override ICodeNode VisitLinqQueryExpression(LinqQueryExpression node)
			{
				return node;
			}

			public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (this.TryMatchLinqQuery(node, out V_0))
				{
					return this.RemoveTransparentIdentifiers(V_0);
				}
				return this.VisitMethodInvocationExpression(node);
			}

			private class IdentifierReplacer : BaseCodeTransformer
			{
				private readonly Dictionary<ParameterDefinition, VariableReference> parameterToVariableMap;

				public IdentifierReplacer(Dictionary<ParameterDefinition, VariableReference> parameterToVariableMap)
				{
					base();
					this.parameterToVariableMap = parameterToVariableMap;
					return;
				}

				public override ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
				{
					V_0 = node.get_Parameter();
					if (!this.parameterToVariableMap.TryGetValue(V_0.Resolve(), out V_1))
					{
						return node;
					}
					return new VariableReferenceExpression(V_1, node.get_UnderlyingSameMethodInstructions());
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
					base();
					this.set_TransparentIdentifierToPropertyValueMap(new Dictionary<VariableReference, Dictionary<MethodDefinition, VariableReference>>());
					return;
				}

				public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
				{
					dummyVar0 = this.VisitPropertyReferenceExpression(node);
					if (node.get_Target() == null || node.get_Target().get_CodeNodeType() != 26 || !this.get_TransparentIdentifierToPropertyValueMap().TryGetValue((node.get_Target() as VariableReferenceExpression).get_Variable(), out V_0))
					{
						return node;
					}
					return new VariableReferenceExpression(V_0.get_Item(node.get_MethodExpression().get_MethodDefinition()), node.get_UnderlyingSameMethodInstructions());
				}
			}

			private class TransparentIdentifierFinder : BaseCodeVisitor
			{
				private readonly HashSet<VariableReference> transparentIdentifiers;

				private bool contains;

				public TransparentIdentifierFinder(HashSet<VariableReference> transparentIdentifiers)
				{
					base();
					this.transparentIdentifiers = transparentIdentifiers;
					return;
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
					this.Visit(node);
					return;
				}

				public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
				{
					if (this.transparentIdentifiers.Contains(node.get_Variable()))
					{
						this.contains = true;
					}
					return;
				}
			}
		}
	}
}