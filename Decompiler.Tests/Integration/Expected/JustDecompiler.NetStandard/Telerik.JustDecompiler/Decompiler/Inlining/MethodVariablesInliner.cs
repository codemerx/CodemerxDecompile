using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class MethodVariablesInliner : BaseVariablesInliner
	{
		public MethodVariablesInliner(MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder)
		{
			base(methodContext, new RestrictedVariableInliner(methodContext.get_Method().get_Module().get_TypeSystem()), finder);
			return;
		}

		protected override void FindSingleDefineSingleUseVariables()
		{
			V_0 = new MethodVariablesInliner.SingleDefineSingleUseFinder(this.variablesToNotInline);
			V_1 = this.methodContext.get_Expressions().get_BlockExpressions().get_Values().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.VisitExpressionsInBlock(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			this.variablesToInline.UnionWith(V_0.get_SingleDefineSingleUsageVariables());
			return;
		}

		protected override void InlineInBlocks()
		{
			V_0 = this.methodContext.get_Expressions().get_BlockExpressions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Value();
					V_3 = new Boolean[V_2.get_Count()];
					V_4 = (int)this.methodContext.get_ControlFlowGraph().get_InstructionToBlockMapping().get_Item(V_1.get_Key()).get_Successors().Length > 1;
					V_5 = V_2.get_Count() - 2;
					V_6 = V_5 + 1;
					while (V_5 >= 0)
					{
						V_7 = V_2.get_Item(V_5) as BinaryExpression;
						if (V_7 == null || !V_7.get_IsAssignmentExpression() || V_7.get_Left().get_CodeNodeType() != 26)
						{
							V_6 = V_5;
						}
						else
						{
							V_8 = (V_7.get_Left() as VariableReferenceExpression).get_Variable().Resolve();
							if (this.variablesToInline.Contains(V_8))
							{
								V_9 = V_7.get_Right();
								if (this.IsEnumeratorGetCurrent(V_9) || this.IsQueryInvocation(V_9) || V_8.get_VariableType() != null && V_8.get_VariableType().get_IsPinned())
								{
									V_6 = V_5;
								}
								else
								{
									V_10 = new List<Instruction>(V_7.get_MappedInstructions());
									V_10.AddRange(V_7.get_Left().get_UnderlyingSameMethodInstructions());
									stackVariable78 = this.inliner;
									stackVariable79 = V_8;
									stackVariable82 = V_9.CloneAndAttachInstructions(V_10);
									stackVariable85 = V_2.get_Item(V_6);
									if (!V_4)
									{
										stackVariable87 = false;
									}
									else
									{
										stackVariable87 = V_6 + 1 == V_2.get_Count();
									}
									if (!stackVariable78.TryInlineVariable(stackVariable79, stackVariable82, stackVariable85, stackVariable87, out V_11))
									{
										V_6 = V_5;
									}
									else
									{
										V_2.set_Item(V_6, (Expression)V_11);
										V_3[V_5] = true;
									}
								}
							}
							else
							{
								V_6 = V_5;
							}
						}
						V_5 = V_5 - 1;
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

		private bool IsEnumeratorGetCurrent(Expression expression)
		{
			if (expression.get_CodeNodeType() == 31)
			{
				expression = (expression as ExplicitCastExpression).get_Expression();
			}
			if (expression.get_CodeNodeType() != 19)
			{
				return false;
			}
			return String.op_Equality((expression as MethodInvocationExpression).get_MethodExpression().get_Method().get_Name(), "get_Current");
		}

		private bool IsQueryInvocation(Expression expression)
		{
			V_0 = expression as MethodInvocationExpression;
			if (V_0 == null || V_0.get_MethodExpression() == null)
			{
				return false;
			}
			if (!String.op_Equality(V_0.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Linq.Enumerable"))
			{
				return false;
			}
			return V_0.get_MethodExpression().get_MethodDefinition().IsQueryMethod();
		}

		private class SingleDefineSingleUseFinder : BaseCodeVisitor
		{
			private readonly HashSet<VariableDefinition> singleDefinitionVariables;

			private readonly HashSet<VariableDefinition> singleUsageVariables;

			private readonly HashSet<VariableDefinition> bannedVariables;

			public HashSet<VariableDefinition> SingleDefineSingleUsageVariables
			{
				get
				{
					return this.singleUsageVariables;
				}
			}

			public SingleDefineSingleUseFinder(HashSet<VariableDefinition> variablesToNotInline)
			{
				this.singleDefinitionVariables = new HashSet<VariableDefinition>();
				this.singleUsageVariables = new HashSet<VariableDefinition>();
				this.bannedVariables = new HashSet<VariableDefinition>();
				base();
				this.bannedVariables.UnionWith(variablesToNotInline);
				return;
			}

			private void AddDefinition(VariableDefinition variable)
			{
				if (!this.bannedVariables.Contains(variable))
				{
					dummyVar0 = this.singleDefinitionVariables.Add(variable);
				}
				return;
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (!node.get_IsAssignmentExpression() || node.get_Left().get_CodeNodeType() != 26)
				{
					this.VisitBinaryExpression(node);
					return;
				}
				this.Visit(node.get_Right());
				this.AddDefinition((node.get_Left() as VariableReferenceExpression).get_Variable().Resolve());
				return;
			}

			public void VisitExpressionsInBlock(IList<Expression> expressions)
			{
				this.singleDefinitionVariables.Clear();
				V_0 = expressions.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.Visit(V_1);
					}
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
				return;
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (node.get_Operator() == 9 || node.get_Operator() == 7 && node.get_Operand().get_CodeNodeType() == 26)
				{
					V_0 = (node.get_Operand() as VariableReferenceExpression).get_Variable().Resolve();
					if (this.bannedVariables.Add(V_0))
					{
						dummyVar0 = this.singleDefinitionVariables.Remove(V_0);
						dummyVar1 = this.singleUsageVariables.Remove(V_0);
					}
					return;
				}
				if (node.get_Operator() == 8 && node.get_Operand().get_CodeNodeType() == 23)
				{
					V_1 = node.get_Operand() as UnaryExpression;
					if (V_1.get_Operator() == 9 || V_1.get_Operator() == 7)
					{
						this.Visit(V_1.get_Operand());
						return;
					}
				}
				this.VisitUnaryExpression(node);
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				V_0 = node.get_Variable().Resolve();
				if (!this.bannedVariables.Contains(V_0))
				{
					if (this.singleDefinitionVariables.Remove(V_0))
					{
						dummyVar0 = this.singleUsageVariables.Add(V_0);
						return;
					}
					dummyVar1 = this.singleUsageVariables.Remove(V_0);
					dummyVar2 = this.bannedVariables.Add(V_0);
				}
				return;
			}
		}
	}
}