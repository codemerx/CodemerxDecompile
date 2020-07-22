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
	internal class RebuildExpressionTreesStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		private TypeSystem typeSystem;

		private readonly Dictionary<VariableReference, Expression> variableToValueMap;

		private readonly Dictionary<VariableReference, HashSet<ExpressionStatement>> variableToAssigingStatementsMap;

		private readonly HashSet<VariableReference> usedVariables;

		private readonly Dictionary<VariableReference, int> variableToLastUninitializedIndex;

		private int conversionDepth;

		private int paramterNameIndex;

		private bool failure;

		public RebuildExpressionTreesStep()
		{
			this.variableToValueMap = new Dictionary<VariableReference, Expression>();
			this.variableToAssigingStatementsMap = new Dictionary<VariableReference, HashSet<ExpressionStatement>>();
			this.usedVariables = new HashSet<VariableReference>();
			this.variableToLastUninitializedIndex = new Dictionary<VariableReference, int>();
			base();
			return;
		}

		private ICodeNode ConvertArrayIndex(MethodInvocationExpression node)
		{
			if (node.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.Visit(node.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_0 == null || V_0.get_Dimensions().get_Count() != 1 || V_0.get_Initializer() == null || V_0.get_Initializer().get_Expressions().get_Count() == 0)
			{
				return null;
			}
			stackVariable31 = new ArrayIndexerExpression((Expression)this.Visit(node.get_Arguments().get_Item(0)), null);
			stackVariable31.set_Indices(V_0.get_Initializer().get_Expressions());
			return stackVariable31;
		}

		private ICodeNode ConvertArrayLength(MethodInvocationExpression node)
		{
			if (node.get_Arguments().get_Count() != 1)
			{
				return null;
			}
			return new ArrayLengthExpression((Expression)this.Visit(node.get_Arguments().get_Item(0)), this.typeSystem, null);
		}

		private BinaryExpression ConvertBinaryOperator(MethodInvocationExpression node, BinaryOperator binaryOperator, bool isChecked)
		{
			if (node.get_Arguments().get_Count() < 2)
			{
				return null;
			}
			try
			{
				V_0 = new BinaryExpression(binaryOperator, (Expression)this.Visit(node.get_Arguments().get_Item(0)), (Expression)this.Visit(node.get_Arguments().get_Item(1)), this.typeSystem, isChecked, null, node.get_Arguments().get_Count() > 2);
			}
			catch
			{
				dummyVar0 = exception_0;
				V_0 = null;
			}
			return V_0;
		}

		private BinaryExpression ConvertBinaryOperator(MethodInvocationExpression node, BinaryOperator binaryOperator)
		{
			return this.ConvertBinaryOperator(node, binaryOperator, false);
		}

		private ICodeNode ConvertBind(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2 || !this.TryGetMethodReference((Expression)this.Visit(invocation.get_Arguments().get_Item(0)), "System.Reflection.MethodInfo", out V_0))
			{
				return null;
			}
			V_1 = this.ResolveProperty(V_0);
			if (V_1 == null)
			{
				return null;
			}
			return new BinaryExpression(26, new PropertyInitializerExpression(V_1, V_1.get_PropertyType()), (Expression)this.Visit(invocation.get_Arguments().get_Item(1)), this.typeSystem, null, false);
		}

		private ICodeNode ConvertCall(MethodInvocationExpression node)
		{
			if (node.get_Arguments().get_Count() < 2 || !this.TryGetMethodReference((Expression)this.Visit(node.get_Arguments().get_Item(1)), "System.Reflection.MethodInfo", out V_0))
			{
				return null;
			}
			V_1 = new MethodInvocationExpression(new MethodReferenceExpression(this.GetTarget(node.get_Arguments().get_Item(0)), V_0, null), null);
			if (node.get_Arguments().get_Count() == 3)
			{
				V_2 = this.Visit(node.get_Arguments().get_Item(2)) as ArrayCreationExpression;
				if (V_2 == null || V_2.get_Dimensions().get_Count() != 1 || V_2.get_Initializer() == null || V_2.get_Initializer().get_Expressions() == null || V_2.get_Initializer().get_Expressions().get_Count() != V_0.get_Parameters().get_Count())
				{
					return null;
				}
				V_3 = V_2.get_Initializer().get_Expressions().GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = V_3.get_Current();
						V_1.get_Arguments().Add(V_4);
					}
				}
				finally
				{
					if (V_3 != null)
					{
						V_3.Dispose();
					}
				}
			}
			return V_1;
		}

		private ICodeNode ConvertCast(MethodInvocationExpression invocation, Func<Expression, TypeReference, Expression> createInstance)
		{
			if (invocation.get_Arguments().get_Count() < 2 || invocation.get_Arguments().get_Item(1).get_CodeNodeType() != 35)
			{
				return null;
			}
			V_0 = (Expression)this.Visit(invocation.get_Arguments().get_Item(0));
			V_1 = (invocation.get_Arguments().get_Item(1) as TypeOfExpression).get_Type();
			return createInstance.Invoke(V_0, V_1);
		}

		private ICodeNode ConvertCast(MethodInvocationExpression node)
		{
			stackVariable1 = node;
			stackVariable2 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__45_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<Expression, TypeReference, Expression>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertCastu003eb__45_0);
				RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__45_0 = stackVariable2;
			}
			return this.ConvertCast(stackVariable1, stackVariable2);
		}

		private ICodeNode ConvertCastChecked(MethodInvocationExpression node)
		{
			V_0 = this.ConvertCast(node) as ExplicitCastExpression;
			if (V_0 == null)
			{
				return null;
			}
			return new CheckedExpression(V_0, null);
		}

		private ICodeNode ConvertCondition(MethodInvocationExpression node)
		{
			if (node.get_Arguments().get_Count() < 3)
			{
				return null;
			}
			stackVariable10 = (Expression)this.Visit(node.get_Arguments().get_Item(0));
			V_0 = (Expression)this.Visit(node.get_Arguments().get_Item(1));
			V_1 = (Expression)this.Visit(node.get_Arguments().get_Item(2));
			return new ConditionExpression(stackVariable10, V_0, V_1, null);
		}

		private ICodeNode ConvertConstant(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() < 1 || invocation.get_Arguments().get_Count() > 2)
			{
				return null;
			}
			V_0 = (Expression)this.Visit(invocation.get_Arguments().get_Item(0));
			if (invocation.get_Arguments().get_Count() == 2)
			{
				dummyVar0 = this.Visit(invocation.get_Arguments().get_Item(1));
			}
			V_1 = V_0 as BoxExpression;
			if (V_1 == null || !V_1.get_IsAutoBox())
			{
				return V_0;
			}
			return V_1.get_BoxedExpression();
		}

		private ICodeNode ConvertElementInit(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_0 == null || V_0.get_Dimensions() == null || V_0.get_Dimensions().get_Count() != 1 || V_0.get_Initializer() == null || V_0.get_Initializer().get_Expressions() == null || V_0.get_Initializer().get_Expressions().get_Count() == 0)
			{
				return null;
			}
			dummyVar0 = this.Visit(invocation.get_Arguments().get_Item(0));
			if (V_0.get_Initializer().get_Expressions().get_Count() != 1)
			{
				return V_0.get_Initializer().get_Expression();
			}
			return V_0.get_Initializer().get_Expressions().get_Item(0);
		}

		private ICodeNode ConvertField(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2 || !this.TryGetFieldReference((Expression)this.Visit(invocation.get_Arguments().get_Item(1)), out V_0))
			{
				return null;
			}
			return new FieldReferenceExpression(this.GetTarget(invocation.get_Arguments().get_Item(0)), V_0, null);
		}

		private ICodeNode ConvertInvocation(MethodInvocationExpression invocation)
		{
			if (invocation.get_MethodExpression() == null || invocation.get_MethodExpression().get_Method() == null || invocation.get_MethodExpression().get_Method().get_HasThis() || invocation.get_MethodExpression().get_Method().get_DeclaringType() == null || String.op_Inequality(invocation.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Linq.Expressions.Expression"))
			{
				return null;
			}
			if (this.conversionDepth == 0 && String.op_Inequality(invocation.get_MethodExpression().get_Method().get_Name(), "Lambda"))
			{
				return null;
			}
			V_0 = null;
			V_1 = invocation.get_MethodExpression().get_Method().get_Name();
			if (V_1 != null)
			{
				if (String.op_Equality(V_1, "Add"))
				{
					V_0 = this.ConvertBinaryOperator(invocation, 1, false);
				}
				else
				{
					if (String.op_Equality(V_1, "AddChecked"))
					{
						V_0 = this.ConvertBinaryOperator(invocation, 1, true);
					}
					else
					{
						if (String.op_Equality(V_1, "And"))
						{
							V_0 = this.ConvertBinaryOperator(invocation, 22);
						}
						else
						{
							if (String.op_Equality(V_1, "AndAlso"))
							{
								V_0 = this.ConvertBinaryOperator(invocation, 12);
							}
							else
							{
								if (String.op_Equality(V_1, "ArrayAccess") || String.op_Equality(V_1, "ArrayIndex"))
								{
									V_0 = this.ConvertArrayIndex(invocation);
								}
								else
								{
									if (String.op_Equality(V_1, "ArrayLength"))
									{
										V_0 = this.ConvertArrayLength(invocation);
									}
									else
									{
										if (String.op_Equality(V_1, "Bind"))
										{
											V_0 = this.ConvertBind(invocation);
										}
										else
										{
											if (String.op_Equality(V_1, "Call"))
											{
												V_0 = this.ConvertCall(invocation);
											}
											else
											{
												if (String.op_Equality(V_1, "Coalesce"))
												{
													V_0 = this.ConvertBinaryOperator(invocation, 27);
												}
												else
												{
													if (String.op_Equality(V_1, "Condition"))
													{
														V_0 = this.ConvertCondition(invocation);
													}
													else
													{
														if (String.op_Equality(V_1, "Constant"))
														{
															V_0 = this.ConvertConstant(invocation);
														}
														else
														{
															if (String.op_Equality(V_1, "Convert"))
															{
																V_0 = this.ConvertCast(invocation);
															}
															else
															{
																if (String.op_Equality(V_1, "ConvertChecked"))
																{
																	V_0 = this.ConvertCastChecked(invocation);
																}
																else
																{
																	if (String.op_Equality(V_1, "Divide"))
																	{
																		V_0 = this.ConvertBinaryOperator(invocation, 7);
																	}
																	else
																	{
																		if (String.op_Equality(V_1, "ElementInit"))
																		{
																			V_0 = this.ConvertElementInit(invocation);
																		}
																		else
																		{
																			if (String.op_Equality(V_1, "Equal"))
																			{
																				V_0 = this.ConvertBinaryOperator(invocation, 9);
																			}
																			else
																			{
																				if (String.op_Equality(V_1, "ExclusiveOr"))
																				{
																					V_0 = this.ConvertBinaryOperator(invocation, 23);
																				}
																				else
																				{
																					if (String.op_Equality(V_1, "Field"))
																					{
																						V_0 = this.ConvertField(invocation);
																					}
																					else
																					{
																						if (String.op_Equality(V_1, "GreaterThan"))
																						{
																							V_0 = this.ConvertBinaryOperator(invocation, 15);
																						}
																						else
																						{
																							if (String.op_Equality(V_1, "GreaterThanOrEqual"))
																							{
																								V_0 = this.ConvertBinaryOperator(invocation, 16);
																							}
																							else
																							{
																								if (String.op_Equality(V_1, "Invoke"))
																								{
																									V_0 = this.ConvertInvoke(invocation);
																								}
																								else
																								{
																									if (String.op_Equality(V_1, "Lambda"))
																									{
																										V_0 = this.ConvertLambda(invocation);
																									}
																									else
																									{
																										if (String.op_Equality(V_1, "LeftShift"))
																										{
																											V_0 = this.ConvertBinaryOperator(invocation, 17);
																										}
																										else
																										{
																											if (String.op_Equality(V_1, "LessThan"))
																											{
																												V_0 = this.ConvertBinaryOperator(invocation, 13);
																											}
																											else
																											{
																												if (String.op_Equality(V_1, "LessThanOrEqual"))
																												{
																													V_0 = this.ConvertBinaryOperator(invocation, 14);
																												}
																												else
																												{
																													if (String.op_Equality(V_1, "ListBind"))
																													{
																														V_0 = this.ConvertListBind(invocation);
																													}
																													else
																													{
																														if (String.op_Equality(V_1, "ListInit"))
																														{
																															V_0 = this.ConvertListInit(invocation);
																														}
																														else
																														{
																															if (String.op_Equality(V_1, "MemberInit"))
																															{
																																V_0 = this.ConvertMemberInit(invocation);
																															}
																															else
																															{
																																if (String.op_Equality(V_1, "Modulo"))
																																{
																																	V_0 = this.ConvertBinaryOperator(invocation, 24);
																																}
																																else
																																{
																																	if (String.op_Equality(V_1, "Multiply"))
																																	{
																																		V_0 = this.ConvertBinaryOperator(invocation, 5, false);
																																	}
																																	else
																																	{
																																		if (String.op_Equality(V_1, "MultiplyChecked"))
																																		{
																																			V_0 = this.ConvertBinaryOperator(invocation, 5, true);
																																		}
																																		else
																																		{
																																			if (String.op_Equality(V_1, "Negate"))
																																			{
																																				V_0 = this.ConvertUnaryOperator(invocation, 0);
																																			}
																																			else
																																			{
																																				if (String.op_Equality(V_1, "NegateChecked"))
																																				{
																																					V_0 = this.ConvertUnaryOperatorChecked(invocation, 0);
																																				}
																																				else
																																				{
																																					if (String.op_Equality(V_1, "New"))
																																					{
																																						V_0 = this.ConvertNewObject(invocation);
																																					}
																																					else
																																					{
																																						if (String.op_Equality(V_1, "NewArrayBounds"))
																																						{
																																							V_0 = this.ConvertNewArrayBounds(invocation);
																																						}
																																						else
																																						{
																																							if (String.op_Equality(V_1, "NewArrayInit"))
																																							{
																																								V_0 = this.ConvertNewArrayInit(invocation);
																																							}
																																							else
																																							{
																																								if (String.op_Equality(V_1, "Not"))
																																								{
																																									V_0 = this.ConvertUnaryOperator(invocation, 1);
																																								}
																																								else
																																								{
																																									if (String.op_Equality(V_1, "NotEqual"))
																																									{
																																										V_0 = this.ConvertBinaryOperator(invocation, 10);
																																									}
																																									else
																																									{
																																										if (String.op_Equality(V_1, "OnesComplement"))
																																										{
																																											V_0 = this.ConvertUnaryOperator(invocation, 2);
																																										}
																																										else
																																										{
																																											if (String.op_Equality(V_1, "Or"))
																																											{
																																												V_0 = this.ConvertBinaryOperator(invocation, 21);
																																											}
																																											else
																																											{
																																												if (String.op_Equality(V_1, "OrElse"))
																																												{
																																													V_0 = this.ConvertBinaryOperator(invocation, 11);
																																												}
																																												else
																																												{
																																													if (String.op_Equality(V_1, "Parameter"))
																																													{
																																														V_0 = this.ConvertParameter(invocation);
																																													}
																																													else
																																													{
																																														if (String.op_Equality(V_1, "Property"))
																																														{
																																															V_0 = this.ConvertProperty(invocation);
																																														}
																																														else
																																														{
																																															if (String.op_Equality(V_1, "Quote"))
																																															{
																																																if (invocation.get_Arguments().get_Count() == 1)
																																																{
																																																	V_0 = this.Visit(invocation.get_Arguments().get_Item(0));
																																																}
																																															}
																																															else
																																															{
																																																if (String.op_Equality(V_1, "RightShift"))
																																																{
																																																	V_0 = this.ConvertBinaryOperator(invocation, 19);
																																																}
																																																else
																																																{
																																																	if (String.op_Equality(V_1, "Subtract"))
																																																	{
																																																		V_0 = this.ConvertBinaryOperator(invocation, 3, false);
																																																	}
																																																	else
																																																	{
																																																		if (String.op_Equality(V_1, "SubtractChecked"))
																																																		{
																																																			V_0 = this.ConvertBinaryOperator(invocation, 3, true);
																																																		}
																																																		else
																																																		{
																																																			if (String.op_Equality(V_1, "TypeAs"))
																																																			{
																																																				V_0 = this.ConvertTypeAs(invocation);
																																																			}
																																																			else
																																																			{
																																																				if (!String.op_Equality(V_1, "TypeIs"))
																																																				{
																																																					goto Label0;
																																																				}
																																																				V_0 = this.ConvertTypeIs(invocation);
																																																			}
																																																		}
																																																	}
																																																}
																																															}
																																														}
																																													}
																																												}
																																											}
																																										}
																																									}
																																								}
																																							}
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				this.failure = this.failure | V_0 == null;
				return V_0;
			}
		Label0:
			return null;
		}

		private ICodeNode ConvertInvoke(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_0 == null || V_0.get_Dimensions().get_Count() != 1 || V_0.get_Initializer() == null || V_0.get_Initializer().get_Expressions() == null)
			{
				return null;
			}
			V_1 = (Expression)this.Visit(invocation.get_Arguments().get_Item(0));
			V_2 = this.GetInvokeMethodReference(V_1);
			if (V_2 == null)
			{
				return null;
			}
			return new DelegateInvokeExpression(V_1, V_0.get_Initializer().get_Expressions(), V_2, null);
		}

		private ICodeNode ConvertLambda(MethodInvocationExpression invocation)
		{
			V_0 = new RebuildExpressionTreesStep.u003cu003ec__DisplayClass29_0();
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_1 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_1 != null && V_1.get_Initializer() != null && V_1.get_Initializer().get_Expressions() != null)
			{
				stackVariable21 = V_1.get_Initializer().get_Expressions();
				stackVariable22 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__29_0;
				if (stackVariable22 == null)
				{
					dummyVar0 = stackVariable22;
					stackVariable22 = new Func<Expression, bool>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertLambdau003eb__29_0);
					RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__29_0 = stackVariable22;
				}
				if (!stackVariable21.Any<Expression>(stackVariable22))
				{
					V_2 = V_1.get_Initializer().get_Expressions().Cast<ArgumentReferenceExpression>().ToList<ArgumentReferenceExpression>();
					stackVariable29 = V_0;
					stackVariable30 = V_2;
					stackVariable31 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__29_1;
					if (stackVariable31 == null)
					{
						dummyVar1 = stackVariable31;
						stackVariable31 = new Func<ArgumentReferenceExpression, bool>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertLambdau003eb__29_1);
						RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__29_1 = stackVariable31;
					}
					stackVariable29.hasAnonymousParameter = stackVariable30.Any<ArgumentReferenceExpression>(stackVariable31);
					V_3 = new BlockStatement();
					V_3.AddStatement(new ExpressionStatement(new ShortFormReturnExpression((Expression)this.Visit(invocation.get_Arguments().get_Item(0)), null)));
					stackVariable50 = new ExpressionCollection(V_2.Select<ArgumentReferenceExpression, LambdaParameterExpression>(new Func<ArgumentReferenceExpression, LambdaParameterExpression>(V_0.u003cConvertLambdau003eb__2)));
					stackVariable51 = V_3;
					stackVariable54 = V_2;
					stackVariable55 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__29_3;
					if (stackVariable55 == null)
					{
						dummyVar2 = stackVariable55;
						stackVariable55 = new Func<ArgumentReferenceExpression, ParameterReference>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertLambdau003eb__29_3);
						RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__29_3 = stackVariable55;
					}
					return new LambdaExpression(stackVariable50, stackVariable51, false, false, stackVariable54.Select<ArgumentReferenceExpression, ParameterReference>(stackVariable55), true, null);
				}
			}
			return null;
		}

		private ICodeNode ConvertListBind(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2 || !this.TryGetMethodReference((Expression)this.Visit(invocation.get_Arguments().get_Item(0)), "System.Reflection.MethodInfo", out V_0))
			{
				return null;
			}
			V_1 = this.ResolveProperty(V_0);
			if (V_1 == null)
			{
				return null;
			}
			V_2 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_2 == null || V_2.get_Dimensions() == null || V_2.get_Dimensions().get_Count() != 1 || V_2.get_Initializer() == null || V_2.get_Initializer().get_Expressions() == null)
			{
				return null;
			}
			V_2.get_Initializer().set_InitializerType(0);
			V_2.get_Initializer().set_IsMultiLine(true);
			return new BinaryExpression(26, new PropertyInitializerExpression(V_1, V_1.get_PropertyType()), V_2.get_Initializer(), this.typeSystem, null, false);
		}

		private ICodeNode ConvertListInit(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.Visit(invocation.get_Arguments().get_Item(0)) as ObjectCreationExpression;
			if (V_0 == null || V_0.get_Initializer() != null)
			{
				return null;
			}
			V_1 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_1 == null || V_1.get_Dimensions() == null || V_1.get_Dimensions().get_Count() != 1 || V_1.get_Initializer() == null || V_1.get_Initializer().get_Expressions() == null)
			{
				return null;
			}
			V_0.set_Initializer(new InitializerExpression(V_1.get_Initializer().get_Expression(), 0));
			V_0.get_Initializer().set_IsMultiLine(true);
			return V_0;
		}

		private ICodeNode ConvertMemberInit(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.Visit(invocation.get_Arguments().get_Item(0)) as ObjectCreationExpression;
			if (V_0 == null || V_0.get_Initializer() != null)
			{
				return null;
			}
			V_1 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_1 != null && V_1.get_Dimensions() != null && V_1.get_Dimensions().get_Count() == 1 && V_1.get_Initializer() != null && V_1.get_Initializer().get_Expressions() != null)
			{
				stackVariable37 = V_1.get_Initializer().get_Expressions();
				stackVariable38 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__27_0;
				if (stackVariable38 == null)
				{
					dummyVar0 = stackVariable38;
					stackVariable38 = new Func<Expression, bool>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertMemberInitu003eb__27_0);
					RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__27_0 = stackVariable38;
				}
				if (!stackVariable37.Any<Expression>(stackVariable38))
				{
					if (V_1.get_Initializer().get_Expressions().get_Count() > 0)
					{
						V_0.set_Initializer(new InitializerExpression(V_1.get_Initializer().get_Expression(), 1));
						V_0.get_Initializer().set_IsMultiLine(true);
					}
					return V_0;
				}
			}
			return null;
		}

		private ICodeNode ConvertNewArrayBounds(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.GenerateArrayCreationExpression(invocation.get_Arguments().get_Item(0));
			if (V_0 == null)
			{
				return null;
			}
			V_1 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_1 == null || V_1.get_Dimensions().get_Count() != 1 || V_1.get_Initializer() == null || V_1.get_Initializer().get_Expressions() == null || V_1.get_Initializer().get_Expressions().get_Count() == 0)
			{
				return null;
			}
			V_0.set_Dimensions(V_1.get_Initializer().get_Expressions());
			return V_0;
		}

		private ICodeNode ConvertNewArrayInit(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			V_0 = this.GenerateArrayCreationExpression(invocation.get_Arguments().get_Item(0));
			if (V_0 == null)
			{
				return null;
			}
			V_1 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_1 == null || V_1.get_Dimensions() == null || V_1.get_Initializer() == null || V_1.get_Initializer().get_Expressions() == null || V_1.get_Initializer().get_Expressions().get_Count() == 0)
			{
				return null;
			}
			V_0.set_Dimensions(V_1.get_Dimensions());
			V_0.set_Initializer(V_1.get_Initializer());
			return V_0;
		}

		private ICodeNode ConvertNewObject(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() < 1 || invocation.get_Arguments().get_Count() > 3)
			{
				return null;
			}
			V_0 = (Expression)this.Visit(invocation.get_Arguments().get_Item(0));
			if (!this.TryGetMethodReference(V_0, "System.Reflection.ConstructorInfo", out V_1))
			{
				if (invocation.get_Arguments().get_Count() != 1 || V_0.get_CodeNodeType() != 35)
				{
					return null;
				}
				return new ObjectCreationExpression(null, (V_0 as TypeOfExpression).get_Type(), null, null);
			}
			V_2 = new ObjectCreationExpression(V_1, V_1.get_DeclaringType(), null, null);
			if (invocation.get_Arguments().get_Count() == 1)
			{
				return V_2;
			}
			V_3 = this.Visit(invocation.get_Arguments().get_Item(1)) as ArrayCreationExpression;
			if (V_3 == null || V_3.get_Dimensions().get_Count() != 1 || V_3.get_Initializer() == null || V_3.get_Initializer().get_Expressions() == null || V_3.get_Initializer().get_Expressions().get_Count() != V_1.get_Parameters().get_Count())
			{
				return null;
			}
			V_4 = V_3.get_Initializer().get_Expressions().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					V_2.get_Arguments().Add(V_5);
				}
			}
			finally
			{
				if (V_4 != null)
				{
					V_4.Dispose();
				}
			}
			if (invocation.get_Arguments().get_Count() == 2)
			{
				return V_2;
			}
			dummyVar0 = this.Visit(invocation.get_Arguments().get_Item(2));
			return V_2;
		}

		private ArgumentReferenceExpression ConvertParameter(MethodInvocationExpression node)
		{
			if (node.get_Arguments().get_Count() < 1 || node.get_Arguments().get_Count() > 2 || node.get_Arguments().get_Item(0).get_CodeNodeType() != 35)
			{
				return null;
			}
			V_0 = (node.get_Arguments().get_Item(0) as TypeOfExpression).get_Type();
			V_1 = null;
			if (node.get_Arguments().get_Count() == 2 && node.get_Arguments().get_Item(1).get_CodeNodeType() == 22)
			{
				V_1 = (node.get_Arguments().get_Item(1) as LiteralExpression).get_Value() as String;
			}
			stackVariable26 = V_1;
			if (stackVariable26 == null)
			{
				dummyVar0 = stackVariable26;
				V_2 = this.paramterNameIndex;
				this.paramterNameIndex = V_2 + 1;
				stackVariable26 = String.Concat("expressionParameter", V_2.ToString());
			}
			return new ArgumentReferenceExpression(new ParameterDefinition(stackVariable26, 0, V_0), null);
		}

		private ICodeNode ConvertProperty(MethodInvocationExpression invocation)
		{
			V_0 = this.ConvertCall(invocation) as MethodInvocationExpression;
			if (V_0 == null)
			{
				return null;
			}
			V_1 = new PropertyReferenceExpression(V_0, null);
			if (V_1.get_Property() == null)
			{
				return null;
			}
			return V_1;
		}

		private ICodeNode ConvertTypeAs(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			stackVariable5 = invocation;
			stackVariable6 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__37_0;
			if (stackVariable6 == null)
			{
				dummyVar0 = stackVariable6;
				stackVariable6 = new Func<Expression, TypeReference, Expression>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertTypeAsu003eb__37_0);
				RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__37_0 = stackVariable6;
			}
			return this.ConvertCast(stackVariable5, stackVariable6);
		}

		private ICodeNode ConvertTypeIs(MethodInvocationExpression invocation)
		{
			if (invocation.get_Arguments().get_Count() != 2)
			{
				return null;
			}
			stackVariable5 = invocation;
			stackVariable6 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__36_0;
			if (stackVariable6 == null)
			{
				dummyVar0 = stackVariable6;
				stackVariable6 = new Func<Expression, TypeReference, Expression>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cConvertTypeIsu003eb__36_0);
				RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__36_0 = stackVariable6;
			}
			return this.ConvertCast(stackVariable5, stackVariable6);
		}

		private UnaryExpression ConvertUnaryOperator(MethodInvocationExpression node, UnaryOperator unaryOperator)
		{
			if (node.get_Arguments().get_Count() < 1)
			{
				return null;
			}
			return new UnaryExpression(unaryOperator, (Expression)this.Visit(node.get_Arguments().get_Item(0)), null);
		}

		private ICodeNode ConvertUnaryOperatorChecked(MethodInvocationExpression node, UnaryOperator unaryOperator)
		{
			V_0 = this.ConvertUnaryOperator(node, unaryOperator);
			if (V_0 == null)
			{
				return null;
			}
			return new CheckedExpression(V_0, null);
		}

		private ArrayCreationExpression GenerateArrayCreationExpression(Expression unprocessedExpression)
		{
			V_0 = this.Visit(unprocessedExpression) as TypeOfExpression;
			if (V_0 == null)
			{
				return null;
			}
			return new ArrayCreationExpression(V_0.get_Type(), null, null);
		}

		private int GetIntegerValue(LiteralExpression size)
		{
			if (size == null)
			{
				return -1;
			}
			try
			{
				V_0 = Convert.ToInt32(size.get_Value());
			}
			catch (InvalidCastException exception_0)
			{
				dummyVar0 = exception_0;
				V_0 = -1;
			}
			return V_0;
		}

		private MethodReference GetInvokeMethodReference(Expression target)
		{
			if (!target.get_HasType() || target.get_ExpressionType() == null)
			{
				return null;
			}
			V_0 = target.get_ExpressionType();
			V_1 = V_0.Resolve();
			if (V_1 == null || !V_1.IsDelegate())
			{
				return null;
			}
			stackVariable14 = V_1.get_Methods();
			stackVariable15 = RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__31_0;
			if (stackVariable15 == null)
			{
				dummyVar0 = stackVariable15;
				stackVariable15 = new Func<MethodDefinition, bool>(RebuildExpressionTreesStep.u003cu003ec.u003cu003e9.u003cGetInvokeMethodReferenceu003eb__31_0);
				RebuildExpressionTreesStep.u003cu003ec.u003cu003e9__31_0 = stackVariable15;
			}
			V_2 = stackVariable14.FirstOrDefault<MethodDefinition>(stackVariable15);
			if (V_2 == null)
			{
				return null;
			}
			stackVariable23 = new MethodReference(V_2.get_Name(), V_2.get_ReturnType(), V_0);
			stackVariable23.get_Parameters().AddRange(V_2.get_Parameters());
			return stackVariable23;
		}

		private Expression GetTarget(Expression unprocessedTarget)
		{
			V_0 = (Expression)this.Visit(unprocessedTarget);
			if (V_0.get_CodeNodeType() == 22 && (V_0 as LiteralExpression).get_Value() == null)
			{
				return null;
			}
			return V_0;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			if (!(new RebuildExpressionTreesStep.ExpressionTreesFinder()).ContainsExpressionTree(body))
			{
				return body;
			}
			this.context = context;
			this.typeSystem = context.get_TypeContext().get_CurrentType().get_Module().get_TypeSystem();
			this.failure = false;
			V_0 = (BlockStatement)this.Visit(body.Clone());
			if (this.failure || this.usedVariables.get_Count() == 0 || !this.TryRemoveUnusedVariableAssignments(V_0))
			{
				return body;
			}
			V_0 = (BlockStatement)(new RebuildExpressionTreesStep.ClosureVariablesRemover(context.get_MethodContext())).Visit(V_0);
			V_0 = (new CombinedTransformerStep()).Process(context, V_0);
			return V_0;
		}

		private void RecordVariableAssignment(ExpressionStatement node)
		{
			V_0 = node.get_Expression() as BinaryExpression;
			V_1 = (V_0.get_Left() as VariableReferenceExpression).get_Variable();
			V_0.set_Right((Expression)this.Visit(V_0.get_Right()));
			V_2 = V_0.get_Right().CloneExpressionOnly();
			this.variableToValueMap.set_Item(V_1, V_2);
			if (!this.variableToAssigingStatementsMap.TryGetValue(V_1, out V_3))
			{
				V_3 = new HashSet<ExpressionStatement>();
				this.variableToAssigingStatementsMap.set_Item(V_1, V_3);
			}
			dummyVar0 = V_3.Add(node);
			V_4 = V_2 as ArrayCreationExpression;
			if (V_4 == null || V_4.get_Dimensions() == null || V_4.get_Dimensions().get_Count() != 1 || V_4.get_Initializer() != null && V_4.get_Initializer().get_Expressions() != null && V_4.get_Initializer().get_Expressions().get_Count() > 0)
			{
				return;
			}
			V_4.set_Initializer(new InitializerExpression(new BlockExpression(null), 2));
			this.variableToLastUninitializedIndex.set_Item(V_1, 0);
			return;
		}

		private PropertyDefinition ResolveProperty(MethodReference methodRef)
		{
			V_0 = new RebuildExpressionTreesStep.u003cu003ec__DisplayClass25_0();
			V_0.methodDef = methodRef.Resolve();
			if (V_0.methodDef == null || V_0.methodDef.get_DeclaringType() == null)
			{
				return null;
			}
			return V_0.methodDef.get_DeclaringType().get_Properties().FirstOrDefault<PropertyDefinition>(new Func<PropertyDefinition, bool>(V_0.u003cResolvePropertyu003eb__0));
		}

		private bool TryGetFieldReference(Expression expression, out FieldReference fieldRef)
		{
			fieldRef = null;
			V_0 = expression as MethodInvocationExpression;
			if (V_0 == null || V_0.get_Arguments().get_Count() > 2 || V_0.get_Arguments().get_Count() < 1 || V_0.get_Arguments().get_Item(0).get_CodeNodeType() != 90 || V_0.get_Arguments().get_Count() == 2 && V_0.get_Arguments().get_Item(1).get_CodeNodeType() != 90 || V_0.get_MethodExpression().get_Method() == null || String.op_Inequality(V_0.get_MethodExpression().get_Method().get_Name(), "GetFieldFromHandle") || V_0.get_MethodExpression().get_Method().get_DeclaringType() == null || String.op_Inequality(V_0.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Reflection.FieldInfo"))
			{
				return false;
			}
			fieldRef = (V_0.get_Arguments().get_Item(0) as MemberHandleExpression).get_MemberReference() as FieldReference;
			return (object)fieldRef != (object)null;
		}

		private bool TryGetMethodReference(Expression expression, string castTargetTypeName, out MethodReference methodRef)
		{
			methodRef = null;
			V_0 = expression as ExplicitCastExpression;
			if (V_0 == null || V_0.get_Expression().get_CodeNodeType() != 19 || V_0.get_TargetType() == null || String.op_Inequality(V_0.get_TargetType().get_FullName(), castTargetTypeName))
			{
				return false;
			}
			V_1 = V_0.get_Expression() as MethodInvocationExpression;
			if (V_1 == null || V_1.get_Arguments().get_Count() > 2 || V_1.get_Arguments().get_Count() < 1 || V_1.get_Arguments().get_Item(0).get_CodeNodeType() != 90 || V_1.get_Arguments().get_Count() == 2 && V_1.get_Arguments().get_Item(1).get_CodeNodeType() != 90 || V_1.get_MethodExpression().get_Method() == null || String.op_Inequality(V_1.get_MethodExpression().get_Method().get_Name(), "GetMethodFromHandle") || V_1.get_MethodExpression().get_Method().get_DeclaringType() == null || String.op_Inequality(V_1.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Reflection.MethodBase"))
			{
				return false;
			}
			methodRef = (V_1.get_Arguments().get_Item(0) as MemberHandleExpression).get_MemberReference() as MethodReference;
			return (object)methodRef != (object)null;
		}

		private bool TryRemoveExpressionStatment(ExpressionStatement statement)
		{
			V_0 = statement.get_Parent() as BlockStatement;
			if (V_0 == null)
			{
				return false;
			}
			dummyVar0 = V_0.get_Statements().Remove(statement);
			return true;
		}

		private bool TryRemoveUnusedVariableAssignments(BlockStatement body)
		{
			V_0 = new HashSet<VariableReference>();
			do
			{
				V_1 = false;
				V_2 = this.usedVariables.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_0.Contains(V_3))
						{
							continue;
						}
						V_4 = this.variableToAssigingStatementsMap.get_Item(V_3);
						if ((new RebuildExpressionTreesStep.VariableUsageFinder(V_3, V_4)).IsUsed(body))
						{
							continue;
						}
						V_5 = V_4.GetEnumerator();
						try
						{
							while (V_5.MoveNext())
							{
								V_6 = V_5.get_Current();
								if (this.TryRemoveExpressionStatment(V_6))
								{
									continue;
								}
								V_7 = false;
								goto Label0;
							}
						}
						finally
						{
							((IDisposable)V_5).Dispose();
						}
						V_1 = true;
						dummyVar0 = V_0.Add(V_3);
					}
					continue;
				}
				finally
				{
					((IDisposable)V_2).Dispose();
				}
			Label0:
				return V_7;
			}
			while (V_1);
			return this.usedVariables.get_Count() == V_0.get_Count();
		}

		private bool TryUpdateInitializer(ExpressionStatement node)
		{
			V_0 = node.get_Expression() as BinaryExpression;
			V_1 = V_0.get_Left() as ArrayIndexerExpression;
			if (V_1.get_Target() == null || V_1.get_Target().get_CodeNodeType() != 26)
			{
				return false;
			}
			V_2 = (V_1.get_Target() as VariableReferenceExpression).get_Variable();
			if (!this.variableToLastUninitializedIndex.TryGetValue(V_2, out V_3))
			{
				return false;
			}
			if (V_1.get_Indices() == null || V_1.get_Indices().get_Count() != 1)
			{
				return false;
			}
			V_4 = this.GetIntegerValue(V_1.get_Indices().get_Item(0) as LiteralExpression);
			if (V_4 == V_3)
			{
				(this.variableToValueMap.get_Item(V_2) as ArrayCreationExpression).get_Initializer().get_Expressions().Add((Expression)this.Visit(V_0.get_Right().CloneExpressionOnly()));
				dummyVar3 = this.variableToAssigingStatementsMap.get_Item(V_2).Add(node);
				this.variableToLastUninitializedIndex.set_Item(V_2, V_4 + 1);
				return true;
			}
			dummyVar0 = this.variableToLastUninitializedIndex.Remove(V_2);
			dummyVar1 = this.variableToValueMap.Remove(V_2);
			dummyVar2 = this.variableToAssigingStatementsMap.Remove(V_2);
			if (this.usedVariables.Contains(V_2))
			{
				this.failure = true;
			}
			return false;
		}

		public override ICodeNode Visit(ICodeNode node)
		{
			if (this.failure)
			{
				return node;
			}
			return this.Visit(node);
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			this.variableToLastUninitializedIndex.Clear();
			this.variableToValueMap.Clear();
			return this.VisitBlockStatement(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			if (node.get_Expression().get_CodeNodeType() == 24)
			{
				V_0 = node.get_Expression() as BinaryExpression;
				if (V_0.get_IsAssignmentExpression())
				{
					if (V_0.get_Left().get_CodeNodeType() == 26)
					{
						this.RecordVariableAssignment(node);
						return node;
					}
					if (V_0.get_Left().get_CodeNodeType() == 39 && this.TryUpdateInitializer(node))
					{
						return node;
					}
				}
			}
			return this.VisitExpressionStatement(node);
		}

		public override ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			if (!node.get_IsExpressionTreeLambda())
			{
				return this.VisitLambdaExpression(node);
			}
			return node;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.conversionDepth = this.conversionDepth + 1;
			this.conversionDepth = this.conversionDepth - 1;
			stackVariable13 = this.ConvertInvocation(node);
			if (stackVariable13 == null)
			{
				dummyVar0 = stackVariable13;
				stackVariable13 = this.VisitMethodInvocationExpression(node);
			}
			return stackVariable13;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.conversionDepth > 0)
			{
				if (this.variableToValueMap.TryGetValue(node.get_Variable(), out V_0))
				{
					dummyVar0 = this.usedVariables.Add(node.get_Variable());
					return this.Visit(V_0.CloneExpressionOnly());
				}
				this.failure = !this.context.get_MethodContext().get_ClosureVariableToFieldValue().ContainsKey(node.get_Variable());
			}
			return this.VisitVariableReferenceExpression(node);
		}

		private class ClosureVariablesRemover : BaseCodeTransformer
		{
			private readonly MethodSpecificContext methodContext;

			public ClosureVariablesRemover(MethodSpecificContext methodContext)
			{
				base();
				this.methodContext = methodContext;
				return;
			}

			public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				if (node.get_Target() != null && node.get_Target().get_CodeNodeType() == 26)
				{
					V_0 = (node.get_Target() as VariableReferenceExpression).get_Variable();
					if (this.methodContext.get_ClosureVariableToFieldValue().TryGetValue(V_0, out V_1))
					{
						V_2 = node.get_Field().Resolve();
						if (V_2 != null && V_1.TryGetValue(V_2, out V_3))
						{
							return V_3.CloneExpressionOnly();
						}
					}
				}
				return this.VisitFieldReferenceExpression(node);
			}
		}

		private class ExpressionTreesFinder : BaseCodeVisitor
		{
			private bool containsExpressionTree;

			public ExpressionTreesFinder()
			{
				base();
				return;
			}

			public bool ContainsExpressionTree(BlockStatement body)
			{
				this.containsExpressionTree = false;
				this.Visit(body);
				return this.containsExpressionTree;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.containsExpressionTree)
				{
					this.Visit(node);
				}
				return;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (node.get_MethodExpression() != null && node.get_MethodExpression().get_Method() != null && !node.get_MethodExpression().get_Method().get_HasThis() && String.op_Equality(node.get_MethodExpression().get_Method().get_Name(), "Lambda") && node.get_MethodExpression().get_Method().get_DeclaringType() != null && String.op_Equality(node.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Linq.Expressions.Expression"))
				{
					this.containsExpressionTree = true;
					return;
				}
				this.VisitMethodInvocationExpression(node);
				return;
			}
		}

		private class VariableUsageFinder : BaseCodeVisitor
		{
			private readonly VariableReference variable;

			private readonly HashSet<ExpressionStatement> assignments;

			private bool isUsed;

			public VariableUsageFinder(VariableReference variable, HashSet<ExpressionStatement> assignments)
			{
				base();
				this.variable = variable;
				this.assignments = assignments;
				return;
			}

			public bool IsUsed(BlockStatement body)
			{
				this.isUsed = false;
				this.Visit(body);
				return this.isUsed;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.isUsed)
				{
					this.Visit(node);
				}
				return;
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				if (!this.assignments.Contains(node))
				{
					this.VisitExpressionStatement(node);
				}
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if ((object)node.get_Variable() == (object)this.variable)
				{
					this.isUsed = true;
				}
				return;
			}
		}
	}
}