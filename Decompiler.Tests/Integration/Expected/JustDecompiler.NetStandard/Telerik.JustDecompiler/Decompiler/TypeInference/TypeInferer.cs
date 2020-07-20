using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class TypeInferer
	{
		protected ICollection<ClassHierarchyNode> inferenceGraph;

		protected readonly DecompilationContext context;

		protected readonly Dictionary<int, Expression> offsetToExpression;

		public TypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression)
		{
			base();
			this.context = context;
			this.offsetToExpression = offsetToExpression;
			return;
		}

		private Expression AddAssignmentCastIfNeeded(Expression expr)
		{
			V_0 = this.context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			if (expr as BinaryExpression != null && (expr as BinaryExpression).get_IsAssignmentExpression())
			{
				V_1 = expr as BinaryExpression;
				if ((object)V_1.get_Left().get_ExpressionType() != (object)V_1.get_Right().get_ExpressionType() && V_1.get_Left().get_ExpressionType().get_IsPrimitive() && V_1.get_Right().get_ExpressionType().get_IsPrimitive() && !this.IsSubtype(V_1.get_Right().get_ExpressionType(), V_1.get_Left().get_ExpressionType()) && V_1.get_Right().get_CodeNodeType() != 22)
				{
					if (V_1.get_Right().get_CodeNodeType() == 31 && String.op_Equality(V_1.get_Right().get_ExpressionType().get_FullName(), V_0.get_UInt16().get_FullName()) && String.op_Equality(V_1.get_Left().get_ExpressionType().get_FullName(), V_0.get_Char().get_FullName()))
					{
						((ExplicitCastExpression)V_1.get_Right()).set_TargetType(V_0.get_Char());
						return expr;
					}
					V_1.set_Right(new ExplicitCastExpression(V_1.get_Right(), V_1.get_Left().get_ExpressionType(), null));
				}
			}
			return expr;
		}

		private void AddCastIfNeeded(Expression useExpression, VariableReference variable)
		{
			V_0 = new TypeInferer.u003cu003ec__DisplayClass8_0();
			V_0.variable = variable;
			V_4 = useExpression.get_CodeNodeType();
			if (V_4 == 19)
			{
				V_1 = useExpression as MethodInvocationExpression;
				V_2 = V_1.get_Arguments().FirstOrDefault<Expression>(new Func<Expression, bool>(V_0.u003cAddCastIfNeededu003eb__0));
				if (V_2 == null)
				{
					V_7 = V_1.get_MethodExpression().get_Target();
					if (V_7.get_CodeNodeType() != 26 || (object)(V_7 as VariableReferenceExpression).get_Variable() != (object)V_0.variable)
					{
						this.AddCastIfNeeded(V_7, V_0.variable);
						return;
					}
					V_8 = V_1.get_MethodExpression().get_Method().get_DeclaringType();
					if (!this.IsSubtype(V_8, V_0.variable.get_VariableType()))
					{
						V_1.get_MethodExpression().set_Target(new ExplicitCastExpression(V_7, V_8, null));
						return;
					}
				}
				else
				{
					V_5 = V_1.get_Arguments().IndexOf(V_2);
					V_6 = V_1.get_MethodExpression().get_Method().get_Parameters().get_Item(V_5).ResolveParameterType(V_1.get_MethodExpression().get_Method());
					if (!this.IsSubtype(V_6, V_0.variable.get_VariableType()))
					{
						if (V_6.get_IsPrimitive() && V_0.variable.get_VariableType().get_IsPrimitive() && String.op_Equality(ExpressionTypeInferer.GetContainingType(V_6.Resolve(), V_0.variable.get_VariableType().Resolve()).get_FullName(), V_6.get_FullName()))
						{
							return;
						}
						V_1.get_Arguments().set_Item(V_5, new ExplicitCastExpression(V_2, V_6, null));
						return;
					}
				}
			}
			else
			{
				if (V_4 != 24)
				{
					return;
				}
				V_3 = useExpression as BinaryExpression;
				if (V_3.get_Operator() == 26 && V_3.get_Right().get_CodeNodeType() == 26 && (object)(V_3.get_Right() as VariableReferenceExpression).get_Variable() == (object)V_0.variable)
				{
					V_9 = V_3.get_Left().get_ExpressionType();
					if (!this.IsSubtype(V_9, V_0.variable.get_VariableType()))
					{
						V_3.set_Right(new ExplicitCastExpression(V_3.get_Right(), V_9, null));
					}
				}
			}
			return;
		}

		private void AddCasts()
		{
			V_0 = this.context.get_MethodContext().get_StackData().get_VariableToDefineUseInfo().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Value().get_UsedAt().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							if (!this.offsetToExpression.TryGetValue(V_3, out V_4))
							{
								continue;
							}
							this.AddCastIfNeeded(V_4, V_1.get_Key());
						}
					}
					finally
					{
						((IDisposable)V_2).Dispose();
					}
					V_5 = V_1.get_Value().get_DefinedAt().GetEnumerator();
					try
					{
						while (V_5.MoveNext())
						{
							V_6 = V_5.get_Current();
							V_7 = this.offsetToExpression.get_Item(V_6);
							V_8 = this.GetInstructionBlock(V_6);
							V_9 = this.context.get_MethodContext().get_Expressions().get_BlockExpressions().get_Item(V_8.get_First().get_Offset());
							this.FixAssignmentInList(V_9, V_7);
						}
					}
					finally
					{
						((IDisposable)V_5).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		protected virtual ClassHierarchyNode FindLowestCommonAncestor(ICollection<ClassHierarchyNode> typeNodes)
		{
			V_0 = 0;
			V_1 = null;
			V_4 = typeNodes.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_0 = V_0 + 1;
					V_1 = V_4.get_Current();
				}
			}
			finally
			{
				if (V_4 != null)
				{
					V_4.Dispose();
				}
			}
			if (V_0 == 1)
			{
				return V_1;
			}
			V_2 = new Queue<ClassHierarchyNode>();
			V_3 = new HashSet<ClassHierarchyNode>();
			V_4 = typeNodes.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (V_2.get_Count() != 0)
					{
						V_10 = V_5;
						while (!V_3.Contains(V_10))
						{
							stackVariable28 = V_5.get_CanAssignTo();
							stackVariable29 = TypeInferer.u003cu003ec.u003cu003e9__18_0;
							if (stackVariable29 == null)
							{
								dummyVar1 = stackVariable29;
								stackVariable29 = new Func<ClassHierarchyNode, bool>(TypeInferer.u003cu003ec.u003cu003e9.u003cFindLowestCommonAncestoru003eb__18_0);
								TypeInferer.u003cu003ec.u003cu003e9__18_0 = stackVariable29;
							}
							if (stackVariable28.Count<ClassHierarchyNode>(stackVariable29) <= 1)
							{
								stackVariable33 = V_10.get_CanAssignTo();
								stackVariable34 = TypeInferer.u003cu003ec.u003cu003e9__18_1;
								if (stackVariable34 == null)
								{
									dummyVar2 = stackVariable34;
									stackVariable34 = new Func<ClassHierarchyNode, bool>(TypeInferer.u003cu003ec.u003cu003e9.u003cFindLowestCommonAncestoru003eb__18_1);
									TypeInferer.u003cu003ec.u003cu003e9__18_1 = stackVariable34;
								}
								V_10 = stackVariable33.FirstOrDefault<ClassHierarchyNode>(stackVariable34);
							}
							else
							{
								V_11 = null;
								goto Label1;
							}
						}
						while (V_2.Peek() != V_10)
						{
							dummyVar3 = V_3.Remove(V_2.Dequeue());
						}
					}
					else
					{
						V_6 = V_5;
						while (V_6 != null)
						{
							V_2.Enqueue(V_6);
							dummyVar0 = V_3.Add(V_6);
							V_7 = null;
							V_8 = V_6.get_CanAssignTo().GetEnumerator();
							try
							{
								while (V_8.MoveNext())
								{
									V_9 = V_8.get_Current();
									if (!V_9.get_IsHardNode())
									{
										continue;
									}
									V_7 = V_9;
									goto Label2;
								}
							}
							finally
							{
								if (V_8 != null)
								{
									V_8.Dispose();
								}
							}
						Label2:
							V_6 = V_7;
						}
					}
				}
				goto Label0;
			}
			finally
			{
				if (V_4 != null)
				{
					V_4.Dispose();
				}
			}
		Label1:
			return V_11;
		Label0:
			return V_2.Peek();
		}

		private void FixAssignmentInList(IList<Expression> expressionList, Expression value)
		{
			V_0 = expressionList.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_CodeNodeType() != 24)
					{
						continue;
					}
					V_2 = V_1 as BinaryExpression;
					if (!V_2.get_IsAssignmentExpression() || V_2.get_Right() != value)
					{
						continue;
					}
					dummyVar0 = this.AddAssignmentCastIfNeeded(V_2);
					goto Label0;
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label0:
			return;
		}

		private bool[,] GeenrateAdjacencyMatrix(Dictionary<ClassHierarchyNode, int> nodeToIndex)
		{
			stackVariable2 = this.inferenceGraph.get_Count();
			V_0 = new bool[stackVariable2, stackVariable2];
			V_1 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = nodeToIndex.get_Item(V_2);
					V_4 = V_2.get_CanAssignTo().GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							V_0[V_3, nodeToIndex.get_Item(V_5)] = true;
						}
					}
					finally
					{
						if (V_4 != null)
						{
							V_4.Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private Dictionary<ClassHierarchyNode, int> GenerateNodeToIndex()
		{
			V_0 = 0;
			V_1 = new Dictionary<ClassHierarchyNode, int>();
			V_2 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_1.Add(V_3, V_0);
					V_0 = V_0 + 1;
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

		private InstructionBlock GetInstructionBlock(int instructionOffset)
		{
			V_0 = this.context.get_MethodContext().get_ControlFlowGraph().get_OffsetToInstruction().get_Item(instructionOffset);
			while (!this.context.get_MethodContext().get_ControlFlowGraph().get_InstructionToBlockMapping().TryGetValue(V_0.get_Offset(), out V_1))
			{
				V_0 = V_0.get_Previous();
			}
			return V_1;
		}

		private void InferIntegerTypes(HashSet<VariableReference> resolvedVariables)
		{
			(new IntegerTypeInferer(this.context, this.offsetToExpression)).InferIntegerTypes(resolvedVariables);
			return;
		}

		public void InferTypes()
		{
			V_0 = (new GreedyTypeInferer(this.context, this.offsetToExpression)).InferTypes();
			V_1 = new ClassHierarchyBuilder(this.offsetToExpression, this.context.get_MethodContext().get_ControlFlowGraph().get_OffsetToInstruction(), this.context);
			this.inferenceGraph = V_1.BuildHierarchy(V_0);
			this.MergeConnectedComponents();
			this.RemoveTransitiveEdges();
			this.ProcessSingleConstraints();
			V_2 = null;
			V_4 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (!V_5.get_IsHardNode() || !String.op_Equality(V_5.get_NodeType().get_FullName(), "System.Object"))
					{
						continue;
					}
					V_2 = V_5;
					goto Label0;
				}
			}
			finally
			{
				if (V_4 != null)
				{
					V_4.Dispose();
				}
			}
		Label0:
			V_3 = new List<ClassHierarchyNode>();
			V_3.Add(V_2);
			V_4 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_6 = V_4.get_Current();
					if (V_6.get_IsHardNode())
					{
						continue;
					}
					V_3.Add(V_6);
				}
			}
			finally
			{
				if (V_4 != null)
				{
					V_4.Dispose();
				}
			}
			this.MergeNodes(V_3);
			this.InferIntegerTypes(V_0);
			this.AddCasts();
			return;
		}

		private bool IsArrayAssignable(TypeReference type, TypeReference supposedSubType)
		{
			if (type as ArrayType == null || supposedSubType as ArrayType == null)
			{
				return false;
			}
			V_0 = type as ArrayType;
			V_1 = supposedSubType as ArrayType;
			if (!this.IsSubtype(V_0.get_ElementType(), V_1.get_ElementType()))
			{
				return false;
			}
			return V_0.get_Dimensions().get_Count() == V_1.get_Dimensions().get_Count();
		}

		private bool IsSubtype(TypeReference type, TypeReference supposedSubType)
		{
			type = this.RemoveModifiers(type);
			supposedSubType = this.RemoveModifiers(supposedSubType);
			if (String.op_Equality(supposedSubType.GetFriendlyFullName(null), type.GetFriendlyFullName(null)) || String.op_Equality(type.get_FullName(), "System.Object"))
			{
				return true;
			}
			if (this.IsArrayAssignable(type, supposedSubType))
			{
				return true;
			}
			V_0 = supposedSubType.Resolve();
			if (V_0 == null)
			{
				return true;
			}
			if (type as GenericInstanceType != null)
			{
				type = type.GetElementType();
			}
		Label2:
			while (V_0 != null)
			{
				if (TypeNamesComparer.AreEqual(type, V_0))
				{
					return true;
				}
				V_1 = V_0.get_Interfaces().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!TypeNamesComparer.AreEqual(type, V_2))
						{
							continue;
						}
						V_3 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			Label1:
				return V_3;
			}
			return false;
		Label0:
			if (V_0.get_BaseType() == null)
			{
				return false;
			}
			V_0 = V_0.get_BaseType().Resolve();
			goto Label2;
		}

		private bool MergeAnySingleParent()
		{
			stackVariable1 = TypeInferer.u003cu003ec.u003cu003e9__19_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Func<ClassHierarchyNode, bool>(TypeInferer.u003cu003ec.u003cu003e9.u003cMergeAnySingleParentu003eb__19_0);
				TypeInferer.u003cu003ec.u003cu003e9__19_0 = stackVariable1;
			}
			return this.MergeSingleParent(stackVariable1);
		}

		protected void MergeConnectedComponents()
		{
			V_0 = (new ConnectedComponentsFinder(this.inferenceGraph)).GetConnectedComponents().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.MergeNodes(V_1);
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

		protected void MergeNodes(ICollection<ClassHierarchyNode> nodeCollection)
		{
			if (nodeCollection.get_Count() <= 1)
			{
				return;
			}
			V_0 = new ClassHierarchyNode(nodeCollection);
			V_1 = V_0.get_ContainedNodes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					dummyVar0 = this.inferenceGraph.Remove(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.inferenceGraph.Add(V_0);
			return;
		}

		private void MergeSingleChildConstraints()
		{
			V_0 = true;
			while (V_0)
			{
				V_1 = null;
				V_0 = false;
				V_2 = this.inferenceGraph.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_3.get_SubTypes().get_Count() != 1 || V_3.get_IsHardNode())
						{
							continue;
						}
						V_4 = V_3.get_SubTypes().First<ClassHierarchyNode>();
						stackVariable21 = new ClassHierarchyNode[2];
						stackVariable21[0] = V_3;
						stackVariable21[1] = V_4;
						V_1 = stackVariable21;
						V_0 = true;
						goto Label0;
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			Label0:
				if (!V_0)
				{
					continue;
				}
				this.MergeNodes(V_1);
			}
			return;
		}

		private bool MergeSingleParent(Func<ClassHierarchyNode, bool> chooseParentPred)
		{
			V_0 = false;
			V_1 = null;
			V_2 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (V_3.get_IsHardNode() || V_3.get_SubTypes().get_Count() != 1)
					{
						continue;
					}
					V_4 = V_3.get_SubTypes().First<ClassHierarchyNode>();
					if (!chooseParentPred.Invoke(V_4))
					{
						continue;
					}
					V_0 = true;
					stackVariable23 = new ClassHierarchyNode[2];
					stackVariable23[0] = V_3;
					stackVariable23[1] = V_4;
					V_1 = stackVariable23;
					goto Label0;
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
		Label0:
			if (V_0)
			{
				this.MergeNodes(V_1);
			}
			return V_0;
		}

		private bool MergeSingleSoftParent()
		{
			stackVariable1 = TypeInferer.u003cu003ec.u003cu003e9__15_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Func<ClassHierarchyNode, bool>(TypeInferer.u003cu003ec.u003cu003e9.u003cMergeSingleSoftParentu003eb__15_0);
				TypeInferer.u003cu003ec.u003cu003e9__15_0 = stackVariable1;
			}
			return this.MergeSingleParent(stackVariable1);
		}

		private bool MergeWithLowestCommonAncestor()
		{
			V_0 = false;
			V_1 = null;
			V_2 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (V_3.get_IsHardNode())
					{
						continue;
					}
					V_0 = true;
					V_4 = new HashSet<ClassHierarchyNode>();
					V_5 = V_3.get_SubTypes().GetEnumerator();
					try
					{
						while (V_5.MoveNext())
						{
							V_6 = V_5.get_Current();
							if (V_6.get_IsClassNode())
							{
								dummyVar0 = V_4.Add(V_6);
							}
							else
							{
								V_0 = false;
								goto Label1;
							}
						}
					}
					finally
					{
						if (V_5 != null)
						{
							V_5.Dispose();
						}
					}
				Label1:
					if (!V_0)
					{
						continue;
					}
					V_7 = this.FindLowestCommonAncestor(V_4);
					if (V_7 == null || V_7 == V_3)
					{
						V_0 = false;
					}
					else
					{
						stackVariable35 = new ClassHierarchyNode[2];
						stackVariable35[0] = V_7;
						stackVariable35[1] = V_3;
						V_1 = stackVariable35;
						goto Label0;
					}
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
		Label0:
			if (V_0)
			{
				this.MergeNodes(V_1);
			}
			return V_0;
		}

		private void ProcessSingleConstraints()
		{
			V_0 = true;
			while (V_0)
			{
				V_0 = false;
				this.MergeSingleChildConstraints();
				if (!this.MergeWithLowestCommonAncestor())
				{
					if (!this.MergeSingleSoftParent())
					{
						if (!this.MergeAnySingleParent())
						{
							continue;
						}
						V_0 = true;
					}
					else
					{
						V_0 = true;
					}
				}
				else
				{
					V_0 = true;
				}
			}
			return;
		}

		private TypeReference RemoveModifiers(TypeReference type)
		{
			if (type as OptionalModifierType != null)
			{
				return (type as OptionalModifierType).get_ElementType();
			}
			if (type as RequiredModifierType == null)
			{
				return type;
			}
			return (type as RequiredModifierType).get_ElementType();
		}

		private void RemoveSubtype(ClassHierarchyNode superType, ClassHierarchyNode subType)
		{
			if (!superType.get_SubTypes().Contains(subType))
			{
				throw new ArgumentOutOfRangeException(String.Format("No such relation between {0} and {1}.", this, subType));
			}
			dummyVar0 = superType.get_SubTypes().Remove(subType);
			dummyVar1 = subType.get_CanAssignTo().Remove(superType);
			return;
		}

		private void RemoveTransitiveEdges()
		{
			V_0 = this.GenerateNodeToIndex();
			V_1 = this.GeenrateAdjacencyMatrix(V_0);
			this.WarsawTransitiveClosure(V_1);
			V_2 = this.inferenceGraph.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_4 = this.inferenceGraph.GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							V_6 = this.inferenceGraph.GetEnumerator();
							try
							{
								while (V_6.MoveNext())
								{
									V_7 = V_6.get_Current();
									V_8 = V_0.get_Item(V_3);
									V_9 = V_0.get_Item(V_5);
									V_10 = V_0.get_Item(V_7);
									if (!V_1[V_8, V_9] || !V_1[V_9, V_10] || V_3.get_IsHardNode() && V_7.get_IsHardNode() || !V_3.get_SubTypes().Contains(V_7))
									{
										continue;
									}
									this.RemoveSubtype(V_3, V_7);
								}
							}
							finally
							{
								if (V_6 != null)
								{
									V_6.Dispose();
								}
							}
						}
					}
					finally
					{
						if (V_4 != null)
						{
							V_4.Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return;
		}

		private void WarsawTransitiveClosure(bool[,] matrix)
		{
			V_0 = matrix.GetLength(0);
			V_1 = 0;
			while (V_1 < V_0)
			{
				V_2 = 0;
				while (V_2 < V_0)
				{
					V_3 = 0;
					while (V_3 < V_0)
					{
						stackVariable12 = matrix;
						stackVariable13 = V_2;
						stackVariable14 = V_3;
						if (matrix[V_2, V_3])
						{
							stackVariable19 = true;
						}
						else
						{
							if (!matrix[V_2, V_1])
							{
								stackVariable19 = false;
							}
							else
							{
								stackVariable19 = matrix[V_1, V_3];
							}
						}
						stackVariable12[stackVariable13, stackVariable14] = stackVariable19;
						V_3 = V_3 + 1;
					}
					V_2 = V_2 + 1;
				}
				V_1 = V_1 + 1;
			}
			return;
		}
	}
}