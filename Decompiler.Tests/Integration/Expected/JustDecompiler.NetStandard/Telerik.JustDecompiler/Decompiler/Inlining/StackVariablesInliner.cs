using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
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

		private readonly HashSet<VariableDefinition> inlinedOnSecondPass = new HashSet<VariableDefinition>();

		public StackVariablesInliner(MethodSpecificContext methodContext, Dictionary<int, Expression> offsetToExpression, IVariablesToNotInlineFinder finder) : base(methodContext, new SimpleVariableInliner(methodContext.Method.Module.TypeSystem), finder)
		{
			this.offsetToExpression = offsetToExpression;
		}

		protected override void FindSingleDefineSingleUseVariables()
		{
			foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> variableToDefineUseInfo in this.methodContext.StackData.VariableToDefineUseInfo)
			{
				if (variableToDefineUseInfo.Value.DefinedAt.Count != 1 || variableToDefineUseInfo.Value.UsedAt.Count != 1 || this.variablesToNotInline.Contains(variableToDefineUseInfo.Key))
				{
					continue;
				}
				this.variablesToInline.Add(variableToDefineUseInfo.Key);
			}
		}

		private void FixBlockExpressions(Dictionary<Expression, Expression> oldToNewValueMap)
		{
			Expression expression;
			IList<Expression> value;
			int i;
			foreach (KeyValuePair<int, IList<Expression>> blockExpression in this.methodContext.Expressions.BlockExpressions)
			{
				value = blockExpression.Value;
				for (i = 0; i < value.Count; i++)
				{
					Expression item = value[i];
					if (item.CodeNodeType == CodeNodeType.BinaryExpression)
					{
						BinaryExpression binaryExpression = item as BinaryExpression;
						if (binaryExpression.IsAssignmentExpression && binaryExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression && this.inlinedOnSecondPass.Contains((binaryExpression.Left as VariableReferenceExpression).Variable.Resolve()))
						{
							goto Label1;
						}
						if (oldToNewValueMap.TryGetValue(binaryExpression.Right, out expression))
						{
							binaryExpression.Right = expression;
						}
					}
					if (oldToNewValueMap.TryGetValue(value[i], out expression))
					{
						value[i] = expression;
					}
				Label0:
				}
			}
			return;
		Label1:
			int num = i;
			i = num - 1;
			value.RemoveAt(num);
			goto Label0;
		}

		private void FixContextAfterInlining(VariableDefinition varDef)
		{
			this.methodContext.StackData.VariableToDefineUseInfo.Remove(varDef);
		}

		private void InlineAssignmentInNextExpression()
		{
			ICodeNode codeNode;
			foreach (KeyValuePair<int, IList<Expression>> blockExpression in this.methodContext.Expressions.BlockExpressions)
			{
				IList<Expression> value = blockExpression.Value;
				bool[] flagArray = new Boolean[value.Count];
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
							if (!this.inliner.TryInlineVariable(variableDefinition, right, value[num], true, out codeNode))
							{
								num = count;
							}
							else
							{
								this.FixContextAfterInlining(variableDefinition);
								this.variablesToInline.Remove(variableDefinition);
								value[num] = (Expression)codeNode;
								flagArray[count] = true;
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

		private void InlineAssignmentInSameBlock()
		{
			ICodeNode codeNode;
			foreach (KeyValuePair<int, IList<Expression>> blockExpression in this.methodContext.Expressions.BlockExpressions)
			{
				IList<Expression> value = blockExpression.Value;
				for (int i = 0; i < value.Count - 1; i++)
				{
					BinaryExpression item = value[i] as BinaryExpression;
					if (item != null && item.IsAssignmentExpression && item.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
					{
						VariableDefinition variableDefinition = (item.Left as VariableReferenceExpression).Variable.Resolve();
						if (this.variablesToInline.Contains(variableDefinition))
						{
							Expression right = item.Right;
							SideEffectsFinder sideEffectsFinder = new SideEffectsFinder();
							bool flag = sideEffectsFinder.HasSideEffectsRecursive(right);
							StackVariablesInliner.VariablesArgumentsAndFieldsFinder variablesArgumentsAndFieldsFinder = new StackVariablesInliner.VariablesArgumentsAndFieldsFinder();
							variablesArgumentsAndFieldsFinder.Visit(right);
							StackVariablesInliner.VariableReferenceFinder variableReferenceFinder = new StackVariablesInliner.VariableReferenceFinder(variablesArgumentsAndFieldsFinder.Variables, variablesArgumentsAndFieldsFinder.Parameters);
							int num = i + 1;
							while (num < value.Count)
							{
								if (!this.inliner.TryInlineVariable(variableDefinition, right, value[num], true, out codeNode))
								{
									if (flag && sideEffectsFinder.HasSideEffectsRecursive(value[num]) || variableReferenceFinder.ContainsReference(value[num]))
									{
										break;
									}
									if (value[num].CodeNodeType == CodeNodeType.BinaryExpression && (value[num] as BinaryExpression).IsAssignmentExpression)
									{
										Expression left = (value[num] as BinaryExpression).Left;
										if (left.CodeNodeType == CodeNodeType.ArgumentReferenceExpression && variablesArgumentsAndFieldsFinder.Parameters.Contains((left as ArgumentReferenceExpression).Parameter.Resolve()) || left.CodeNodeType == CodeNodeType.VariableReferenceExpression && variablesArgumentsAndFieldsFinder.Variables.Contains((left as VariableReferenceExpression).Variable.Resolve()) || left.CodeNodeType == CodeNodeType.FieldReferenceExpression && variablesArgumentsAndFieldsFinder.Fields.Contains((left as FieldReferenceExpression).Field.Resolve()))
										{
											break;
										}
									}
									num++;
								}
								else
								{
									this.FixContextAfterInlining(variableDefinition);
									this.variablesToInline.Remove(variableDefinition);
									value[num] = (Expression)codeNode;
									value.RemoveAt(i);
									i = i - (i > 0 ? 2 : 1);
									break;
								}
							}
						}
					}
				}
			}
		}

		private void InlineConstantVariables()
		{
			Expression expression;
			ICodeNode codeNode;
			Dictionary<Expression, Expression> expressions = new Dictionary<Expression, Expression>();
			foreach (VariableDefinition variableDefinition in this.variablesToInline)
			{
				StackVariableDefineUseInfo item = this.methodContext.StackData.VariableToDefineUseInfo[variableDefinition];
				int num = item.UsedAt.First<int>();
				if (!this.offsetToExpression.TryGetValue(num, out expression))
				{
					continue;
				}
				Expression item1 = this.offsetToExpression[item.DefinedAt.First<int>()];
				if (!StackVariablesInliner.ConstantDeterminator.IsConstantExpression(item1) || !this.inliner.TryInlineVariable(variableDefinition, item1, expression, true, out codeNode))
				{
					continue;
				}
				this.inlinedOnSecondPass.Add(variableDefinition);
				this.FixContextAfterInlining(variableDefinition);
				if (expression == codeNode)
				{
					continue;
				}
				expressions.Add(expression, (Expression)codeNode);
			}
			this.FixBlockExpressions(expressions);
		}

		protected override void InlineInBlocks()
		{
			this.InlineAssignmentInNextExpression();
			this.InlineAssignmentInSameBlock();
			this.InlineConstantVariables();
		}

		private class ConstantDeterminator : BaseCodeVisitor
		{
			private bool isConstant;

			private ConstantDeterminator()
			{
			}

			public static bool IsConstantExpression(Expression expression)
			{
				StackVariablesInliner.ConstantDeterminator constantDeterminator = new StackVariablesInliner.ConstantDeterminator()
				{
					isConstant = true
				};
				constantDeterminator.Visit(expression);
				return constantDeterminator.isConstant;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.isConstant)
				{
					return;
				}
				CodeNodeType codeNodeType = node.CodeNodeType;
				switch (codeNodeType)
				{
					case CodeNodeType.MethodReferenceExpression:
					case CodeNodeType.LiteralExpression:
					case CodeNodeType.BaseReferenceExpression:
					{
						return;
					}
					case CodeNodeType.DelegateCreationExpression:
					case CodeNodeType.VariableReferenceExpression:
					case CodeNodeType.VariableDeclarationExpression:
					case CodeNodeType.FieldReferenceExpression:
					case CodeNodeType.ExplicitCastExpression:
					case CodeNodeType.ImplicitCastExpression:
					case CodeNodeType.CanCastExpression:
					{
						this.isConstant = false;
						return;
					}
					case CodeNodeType.UnaryExpression:
					{
						switch ((node as UnaryExpression).Operator)
						{
							case UnaryOperator.Negate:
							case UnaryOperator.LogicalNot:
							case UnaryOperator.BitwiseNot:
							case UnaryOperator.AddressDereference:
							case UnaryOperator.UnaryPlus:
							case UnaryOperator.None:
							{
								base.Visit(node);
								return;
							}
							case UnaryOperator.PostDecrement:
							case UnaryOperator.PostIncrement:
							case UnaryOperator.PreDecrement:
							case UnaryOperator.PreIncrement:
							{
								this.isConstant = false;
								return;
							}
							case UnaryOperator.AddressReference:
							case UnaryOperator.AddressOf:
							{
								return;
							}
							default:
							{
								this.isConstant = false;
								return;
							}
						}
						break;
					}
					case CodeNodeType.BinaryExpression:
					{
						BinaryExpression binaryExpression = node as BinaryExpression;
						this.isConstant = (binaryExpression.IsChecked || binaryExpression.Operator == BinaryOperator.Divide ? false : binaryExpression.Operator != BinaryOperator.Modulo);
						if (this.isConstant)
						{
							base.Visit(node);
							return;
						}
						return;
					}
					case CodeNodeType.ArgumentReferenceExpression:
					{
						this.isConstant = false;
						return;
					}
					case CodeNodeType.ThisReferenceExpression:
					{
						return;
					}
					case CodeNodeType.SafeCastExpression:
					case CodeNodeType.TypeOfExpression:
					{
						base.Visit(node);
						return;
					}
					default:
					{
						if (codeNodeType == CodeNodeType.TypeReferenceExpression)
						{
							return;
						}
						this.isConstant = false;
						return;
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
				this.variables = variables;
				this.parameters = parameters;
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
				base.Visit(node);
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (node.Operator != UnaryOperator.AddressOf && node.Operator != UnaryOperator.AddressReference || (node.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.variables.Contains((node.Operand as VariableReferenceExpression).Variable.Resolve())) && (node.Operand.CodeNodeType != CodeNodeType.ArgumentReferenceExpression || !this.parameters.Contains((node.Operand as ArgumentReferenceExpression).Parameter.Resolve())))
				{
					base.VisitUnaryExpression(node);
					return;
				}
				this.containsReference = true;
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
				this.Variables = new HashSet<VariableDefinition>();
				this.Parameters = new HashSet<ParameterDefinition>();
				this.Fields = new HashSet<FieldDefinition>();
			}

			public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
			{
				this.Parameters.Add(node.Parameter.Resolve());
			}

			public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				this.Fields.Add(node.Field.Resolve());
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				this.Variables.Add(node.Variable.Resolve());
			}
		}
	}
}