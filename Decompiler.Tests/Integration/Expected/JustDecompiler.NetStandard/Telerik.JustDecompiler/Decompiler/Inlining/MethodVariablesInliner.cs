using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class MethodVariablesInliner : BaseVariablesInliner
	{
		public MethodVariablesInliner(MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder) : base(methodContext, new RestrictedVariableInliner(methodContext.Method.Module.TypeSystem), finder)
		{
		}

		protected override void FindSingleDefineSingleUseVariables()
		{
			MethodVariablesInliner.SingleDefineSingleUseFinder singleDefineSingleUseFinder = new MethodVariablesInliner.SingleDefineSingleUseFinder(this.variablesToNotInline);
			foreach (IList<Expression> value in this.methodContext.Expressions.BlockExpressions.Values)
			{
				singleDefineSingleUseFinder.VisitExpressionsInBlock(value);
			}
			this.variablesToInline.UnionWith(singleDefineSingleUseFinder.SingleDefineSingleUsageVariables);
		}

		protected override void InlineInBlocks()
		{
			ICodeNode codeNode;
			foreach (KeyValuePair<int, IList<Expression>> blockExpression in this.methodContext.Expressions.BlockExpressions)
			{
				IList<Expression> value = blockExpression.Value;
				bool[] flagArray = new Boolean[value.Count];
				bool length = (int)this.methodContext.ControlFlowGraph.InstructionToBlockMapping[blockExpression.Key].Successors.Length > 1;
				int count = value.Count - 2;
				int num = count + 1;
				while (count >= 0)
				{
					BinaryExpression item = value[count] as BinaryExpression;
					if (item == null || !item.IsAssignmentExpression || item.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
					{
						num = count;
					}
					else
					{
						VariableDefinition variableDefinition = (item.Left as VariableReferenceExpression).Variable.Resolve();
						if (this.variablesToInline.Contains(variableDefinition))
						{
							Expression right = item.Right;
							if (this.IsEnumeratorGetCurrent(right) || this.IsQueryInvocation(right) || variableDefinition.VariableType != null && variableDefinition.VariableType.IsPinned)
							{
								num = count;
							}
							else
							{
								List<Instruction> instructions = new List<Instruction>(item.MappedInstructions);
								instructions.AddRange(item.Left.UnderlyingSameMethodInstructions);
								if (!this.inliner.TryInlineVariable(variableDefinition, right.CloneAndAttachInstructions(instructions), value[num], (!length ? false : num + 1 == value.Count), out codeNode))
								{
									num = count;
								}
								else
								{
									value[num] = (Expression)codeNode;
									flagArray[count] = true;
								}
							}
						}
						else
						{
							num = count;
						}
					}
					count--;
				}
				base.FastRemoveExpressions(value, flagArray);
			}
		}

		private bool IsEnumeratorGetCurrent(Expression expression)
		{
			if (expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{
				expression = (expression as ExplicitCastExpression).Expression;
			}
			if (expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
			{
				return false;
			}
			return (expression as MethodInvocationExpression).MethodExpression.Method.Name == "get_Current";
		}

		private bool IsQueryInvocation(Expression expression)
		{
			MethodInvocationExpression methodInvocationExpression = expression as MethodInvocationExpression;
			if (methodInvocationExpression == null || methodInvocationExpression.MethodExpression == null)
			{
				return false;
			}
			if (methodInvocationExpression.MethodExpression.Method.DeclaringType.FullName != "System.Linq.Enumerable")
			{
				return false;
			}
			return methodInvocationExpression.MethodExpression.MethodDefinition.IsQueryMethod();
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
				this.bannedVariables.UnionWith(variablesToNotInline);
			}

			private void AddDefinition(VariableDefinition variable)
			{
				if (!this.bannedVariables.Contains(variable))
				{
					this.singleDefinitionVariables.Add(variable);
				}
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (!node.IsAssignmentExpression || node.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					base.VisitBinaryExpression(node);
					return;
				}
				this.Visit(node.Right);
				this.AddDefinition((node.Left as VariableReferenceExpression).Variable.Resolve());
			}

			public void VisitExpressionsInBlock(IList<Expression> expressions)
			{
				this.singleDefinitionVariables.Clear();
				foreach (Expression expression in expressions)
				{
					this.Visit(expression);
				}
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if ((node.Operator == UnaryOperator.AddressOf || node.Operator == UnaryOperator.AddressReference) && node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableDefinition variableDefinition = (node.Operand as VariableReferenceExpression).Variable.Resolve();
					if (this.bannedVariables.Add(variableDefinition))
					{
						this.singleDefinitionVariables.Remove(variableDefinition);
						this.singleUsageVariables.Remove(variableDefinition);
					}
					return;
				}
				if (node.Operator == UnaryOperator.AddressDereference && node.Operand.CodeNodeType == CodeNodeType.UnaryExpression)
				{
					UnaryExpression operand = node.Operand as UnaryExpression;
					if (operand.Operator == UnaryOperator.AddressOf || operand.Operator == UnaryOperator.AddressReference)
					{
						base.Visit(operand.Operand);
						return;
					}
				}
				base.VisitUnaryExpression(node);
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				VariableDefinition variableDefinition = node.Variable.Resolve();
				if (!this.bannedVariables.Contains(variableDefinition))
				{
					if (this.singleDefinitionVariables.Remove(variableDefinition))
					{
						this.singleUsageVariables.Add(variableDefinition);
						return;
					}
					this.singleUsageVariables.Remove(variableDefinition);
					this.bannedVariables.Add(variableDefinition);
				}
			}
		}
	}
}