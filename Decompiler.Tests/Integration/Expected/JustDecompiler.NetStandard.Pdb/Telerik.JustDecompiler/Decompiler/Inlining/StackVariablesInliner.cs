using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class StackVariablesInliner : BaseVariablesInliner
	{
		private readonly Dictionary<int, Expression> offsetToExpression;

		private readonly HashSet<VariableDefinition> inlinedOnSecondPass;

		public StackVariablesInliner(MethodSpecificContext methodContext, Dictionary<int, Expression> offsetToExpression, IVariablesToNotInlineFinder finder)
		{
			this.inlinedOnSecondPass = new HashSet<VariableDefinition>();
			base(methodContext, new SimpleVariableInliner(methodContext.get_Method().get_Module().get_TypeSystem()), finder);
			this.offsetToExpression = offsetToExpression;
			return;
		}

		protected override void FindSingleDefineSingleUseVariables()
		{
			V_0 = this.methodContext.get_StackData().get_VariableToDefineUseInfo().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_Value().get_DefinedAt().get_Count() != 1 || V_1.get_Value().get_UsedAt().get_Count() != 1 || this.variablesToNotInline.Contains(V_1.get_Key()))
					{
						continue;
					}
					dummyVar0 = this.variablesToInline.Add(V_1.get_Key());
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void FixBlockExpressions(Dictionary<Expression, Expression> oldToNewValueMap)
		{
			V_1 = this.methodContext.get_Expressions().get_BlockExpressions().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_Value();
					V_4 = 0;
					while (V_4 < V_3.get_Count())
					{
						V_5 = V_3.get_Item(V_4);
						if (V_5.get_CodeNodeType() == 24)
						{
							V_6 = V_5 as BinaryExpression;
							if (V_6.get_IsAssignmentExpression() && V_6.get_Left().get_CodeNodeType() == 26 && this.inlinedOnSecondPass.Contains((V_6.get_Left() as VariableReferenceExpression).get_Variable().Resolve()))
							{
								goto Label1;
							}
							if (oldToNewValueMap.TryGetValue(V_6.get_Right(), out V_0))
							{
								V_6.set_Right(V_0);
							}
						}
						if (oldToNewValueMap.TryGetValue(V_3.get_Item(V_4), out V_0))
						{
							V_3.set_Item(V_4, V_0);
						}
					Label0:
						V_4 = V_4 + 1;
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return;
		Label1:
			stackVariable57 = V_4;
			V_4 = stackVariable57 - 1;
			V_3.RemoveAt(stackVariable57);
			goto Label0;
		}

		private void FixContextAfterInlining(VariableDefinition varDef)
		{
			dummyVar0 = this.methodContext.get_StackData().get_VariableToDefineUseInfo().Remove(varDef);
			return;
		}

		private void InlineAssignmentInNextExpression()
		{
			V_0 = this.methodContext.get_Expressions().get_BlockExpressions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Value();
					V_3 = new Boolean[V_2.get_Count()];
					V_4 = V_2.get_Count() - 2;
					V_5 = V_4 + 1;
					while (V_4 >= 0)
					{
						V_6 = V_2.get_Item(V_4) as BinaryExpression;
						if (V_6 == null || !V_6.get_IsAssignmentExpression() || V_6.get_Left().get_CodeNodeType() != 26)
						{
							V_5 = V_4;
						}
						else
						{
							V_7 = (V_6.get_Left() as VariableReferenceExpression).get_Variable().Resolve();
							if (this.variablesToInline.Contains(V_7))
							{
								V_8 = V_6.get_Right();
								if (!this.inliner.TryInlineVariable(V_7, V_8, V_2.get_Item(V_5), true, out V_9))
								{
									V_5 = V_4;
								}
								else
								{
									this.FixContextAfterInlining(V_7);
									dummyVar0 = this.variablesToInline.Remove(V_7);
									V_2.set_Item(V_5, (Expression)V_9);
									V_3[V_4] = true;
								}
							}
							else
							{
								V_5 = V_4;
							}
						}
						V_4 = V_4 - 1;
					}
					this.FastRemoveExpressions(V_2, V_3);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void InlineAssignmentInSameBlock()
		{
			V_0 = this.methodContext.get_Expressions().get_BlockExpressions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Value();
					V_3 = 0;
					while (V_3 < V_2.get_Count() - 1)
					{
						V_4 = V_2.get_Item(V_3) as BinaryExpression;
						if (V_4 != null && V_4.get_IsAssignmentExpression() && V_4.get_Left().get_CodeNodeType() == 26)
						{
							V_5 = (V_4.get_Left() as VariableReferenceExpression).get_Variable().Resolve();
							if (this.variablesToInline.Contains(V_5))
							{
								V_6 = V_4.get_Right();
								V_7 = new SideEffectsFinder();
								V_8 = V_7.HasSideEffectsRecursive(V_6);
								V_9 = new StackVariablesInliner.VariablesArgumentsAndFieldsFinder();
								V_9.Visit(V_6);
								V_10 = new StackVariablesInliner.VariableReferenceFinder(V_9.get_Variables(), V_9.get_Parameters());
								V_11 = V_3 + 1;
								while (V_11 < V_2.get_Count())
								{
									if (!this.inliner.TryInlineVariable(V_5, V_6, V_2.get_Item(V_11), true, out V_12))
									{
										if (V_8 && V_7.HasSideEffectsRecursive(V_2.get_Item(V_11)) || V_10.ContainsReference(V_2.get_Item(V_11)))
										{
											break;
										}
										if (V_2.get_Item(V_11).get_CodeNodeType() == 24 && (V_2.get_Item(V_11) as BinaryExpression).get_IsAssignmentExpression())
										{
											V_13 = (V_2.get_Item(V_11) as BinaryExpression).get_Left();
											if (V_13.get_CodeNodeType() == 25 && V_9.get_Parameters().Contains((V_13 as ArgumentReferenceExpression).get_Parameter().Resolve()) || V_13.get_CodeNodeType() == 26 && V_9.get_Variables().Contains((V_13 as VariableReferenceExpression).get_Variable().Resolve()) || V_13.get_CodeNodeType() == 30 && V_9.get_Fields().Contains((V_13 as FieldReferenceExpression).get_Field().Resolve()))
											{
												break;
											}
										}
										V_11 = V_11 + 1;
									}
									else
									{
										this.FixContextAfterInlining(V_5);
										dummyVar0 = this.variablesToInline.Remove(V_5);
										V_2.set_Item(V_11, (Expression)V_12);
										V_2.RemoveAt(V_3);
										stackVariable141 = V_3;
										if (V_3 > 0)
										{
											stackVariable144 = 2;
										}
										else
										{
											stackVariable144 = 1;
										}
										V_3 = stackVariable141 - stackVariable144;
										break;
									}
								}
							}
						}
						V_3 = V_3 + 1;
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void InlineConstantVariables()
		{
			V_0 = new Dictionary<Expression, Expression>();
			V_1 = this.variablesToInline.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = this.methodContext.get_StackData().get_VariableToDefineUseInfo().get_Item(V_2);
					V_4 = V_3.get_UsedAt().First<int>();
					if (!this.offsetToExpression.TryGetValue(V_4, out V_5))
					{
						continue;
					}
					V_6 = this.offsetToExpression.get_Item(V_3.get_DefinedAt().First<int>());
					if (!StackVariablesInliner.ConstantDeterminator.IsConstantExpression(V_6) || !this.inliner.TryInlineVariable(V_2, V_6, V_5, true, out V_7))
					{
						continue;
					}
					dummyVar0 = this.inlinedOnSecondPass.Add(V_2);
					this.FixContextAfterInlining(V_2);
					if (V_5 == V_7)
					{
						continue;
					}
					V_0.Add(V_5, (Expression)V_7);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			this.FixBlockExpressions(V_0);
			return;
		}

		protected override void InlineInBlocks()
		{
			this.InlineAssignmentInNextExpression();
			this.InlineAssignmentInSameBlock();
			this.InlineConstantVariables();
			return;
		}

		private class ConstantDeterminator : BaseCodeVisitor
		{
			private bool isConstant;

			private ConstantDeterminator()
			{
				base();
				return;
			}

			public static bool IsConstantExpression(Expression expression)
			{
				stackVariable0 = new StackVariablesInliner.ConstantDeterminator();
				stackVariable0.isConstant = true;
				stackVariable0.Visit(expression);
				return stackVariable0.isConstant;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.isConstant)
				{
					return;
				}
				V_1 = node.get_CodeNodeType();
				switch (V_1 - 20)
				{
					case 0:
					case 2:
					case 9:
					{
					Label2:
						return;
					}
					case 1:
					case 6:
					case 7:
					case 10:
					case 11:
					case 12:
					case 14:
					{
					Label3:
						this.isConstant = false;
						return;
					}
					case 3:
					{
						switch ((node as UnaryExpression).get_Operator())
						{
							case 0:
							case 1:
							case 2:
							case 8:
							case 10:
							case 11:
							{
								goto Label0;
							}
							case 3:
							case 4:
							case 5:
							case 6:
							{
							Label1:
								this.isConstant = false;
								return;
							}
							case 7:
							case 9:
							{
								return;
							}
							default:
							{
								goto Label1;
							}
						}
						break;
					}
					case 4:
					{
						V_0 = node as BinaryExpression;
						if (V_0.get_IsChecked() || V_0.get_Operator() == 7)
						{
							stackVariable22 = false;
						}
						else
						{
							stackVariable22 = V_0.get_Operator() != 24;
						}
						this.isConstant = stackVariable22;
						if (this.isConstant)
						{
							goto Label0;
						}
						return;
					}
					case 5:
					{
						this.isConstant = false;
						return;
					}
					case 8:
					{
						return;
					}
					case 13:
					case 15:
					{
					Label0:
						this.Visit(node);
						return;
					}
					default:
					{
						if (V_1 == 43)
						{
							goto Label2;
						}
						goto Label3;
					}
				}
			}
		}

		private class VariableReferenceFinder : BaseCodeVisitor
		{
			private readonly HashSet<VariableDefinition> variables;

			private readonly HashSet<ParameterDefinition> parameters;

			private bool containsReference;

			public VariableReferenceFinder(HashSet<VariableDefinition> variables, HashSet<ParameterDefinition> parameters)
			{
				base();
				this.variables = variables;
				this.parameters = parameters;
				return;
			}

			public bool ContainsReference(Expression expression)
			{
				this.containsReference = false;
				this.Visit(expression);
				return this.containsReference;
			}

			public override void Visit(ICodeNode node)
			{
				if (this.containsReference)
				{
					return;
				}
				this.Visit(node);
				return;
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (node.get_Operator() != 9 && node.get_Operator() != 7 || node.get_Operand().get_CodeNodeType() != 26 || !this.variables.Contains((node.get_Operand() as VariableReferenceExpression).get_Variable().Resolve()) && node.get_Operand().get_CodeNodeType() != 25 || !this.parameters.Contains((node.get_Operand() as ArgumentReferenceExpression).get_Parameter().Resolve()))
				{
					this.VisitUnaryExpression(node);
					return;
				}
				this.containsReference = true;
				return;
			}
		}

		private class VariablesArgumentsAndFieldsFinder : BaseCodeVisitor
		{
			public HashSet<FieldDefinition> Fields
			{
				get;
				private set;
			}

			public HashSet<ParameterDefinition> Parameters
			{
				get;
				private set;
			}

			public HashSet<VariableDefinition> Variables
			{
				get;
				private set;
			}

			public VariablesArgumentsAndFieldsFinder()
			{
				base();
				this.set_Variables(new HashSet<VariableDefinition>());
				this.set_Parameters(new HashSet<ParameterDefinition>());
				this.set_Fields(new HashSet<FieldDefinition>());
				return;
			}

			public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
			{
				dummyVar0 = this.get_Parameters().Add(node.get_Parameter().Resolve());
				return;
			}

			public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				dummyVar0 = this.get_Fields().Add(node.get_Field().Resolve());
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				dummyVar0 = this.get_Variables().Add(node.get_Variable().Resolve());
				return;
			}
		}
	}
}