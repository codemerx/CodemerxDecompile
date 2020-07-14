using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class TypeInferer
	{
		protected ICollection<ClassHierarchyNode> inferenceGraph;

		protected readonly DecompilationContext context;

		protected readonly Dictionary<int, Expression> offsetToExpression;

		public TypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression)
		{
			this.context = context;
			this.offsetToExpression = offsetToExpression;
		}

		private Expression AddAssignmentCastIfNeeded(Expression expr)
		{
			TypeSystem typeSystem = this.context.MethodContext.Method.get_Module().get_TypeSystem();
			if (expr is BinaryExpression && (expr as BinaryExpression).IsAssignmentExpression)
			{
				BinaryExpression chr = expr as BinaryExpression;
				if ((object)chr.Left.ExpressionType != (object)chr.Right.ExpressionType && chr.Left.ExpressionType.get_IsPrimitive() && chr.Right.ExpressionType.get_IsPrimitive() && !this.IsSubtype(chr.Right.ExpressionType, chr.Left.ExpressionType) && chr.Right.CodeNodeType != CodeNodeType.LiteralExpression)
				{
					if (chr.Right.CodeNodeType == CodeNodeType.ExplicitCastExpression && chr.Right.ExpressionType.get_FullName() == typeSystem.get_UInt16().get_FullName() && chr.Left.ExpressionType.get_FullName() == typeSystem.get_Char().get_FullName())
					{
						((ExplicitCastExpression)chr.Right).TargetType = typeSystem.get_Char();
						return expr;
					}
					chr.Right = new ExplicitCastExpression(chr.Right, chr.Left.ExpressionType, null);
				}
			}
			return expr;
		}

		private void AddCastIfNeeded(Expression useExpression, VariableReference variable)
		{
			CodeNodeType codeNodeType = useExpression.CodeNodeType;
			if (codeNodeType == CodeNodeType.MethodInvocationExpression)
			{
				MethodInvocationExpression explicitCastExpression = useExpression as MethodInvocationExpression;
				Expression expression = explicitCastExpression.Arguments.FirstOrDefault<Expression>((Expression x) => {
					if (x.CodeNodeType != CodeNodeType.VariableReferenceExpression)
					{
						return false;
					}
					return (object)(x as VariableReferenceExpression).Variable == (object)variable;
				});
				if (expression == null)
				{
					Expression target = explicitCastExpression.MethodExpression.Target;
					if (target.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(target as VariableReferenceExpression).Variable != (object)variable)
					{
						this.AddCastIfNeeded(target, variable);
						return;
					}
					TypeReference declaringType = explicitCastExpression.MethodExpression.Method.get_DeclaringType();
					if (!this.IsSubtype(declaringType, variable.get_VariableType()))
					{
						explicitCastExpression.MethodExpression.Target = new ExplicitCastExpression(target, declaringType, null);
						return;
					}
				}
				else
				{
					int num = explicitCastExpression.Arguments.IndexOf(expression);
					TypeReference typeReference = explicitCastExpression.MethodExpression.Method.get_Parameters().get_Item(num).ResolveParameterType(explicitCastExpression.MethodExpression.Method);
					if (!this.IsSubtype(typeReference, variable.get_VariableType()))
					{
						if (typeReference.get_IsPrimitive() && variable.get_VariableType().get_IsPrimitive() && ExpressionTypeInferer.GetContainingType(typeReference.Resolve(), variable.get_VariableType().Resolve()).get_FullName() == typeReference.get_FullName())
						{
							return;
						}
						explicitCastExpression.Arguments[num] = new ExplicitCastExpression(expression, typeReference, null);
						return;
					}
				}
			}
			else
			{
				if (codeNodeType != CodeNodeType.BinaryExpression)
				{
					return;
				}
				BinaryExpression binaryExpression = useExpression as BinaryExpression;
				if (binaryExpression.Operator == BinaryOperator.Assign && binaryExpression.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression && (object)(binaryExpression.Right as VariableReferenceExpression).Variable == (object)variable)
				{
					TypeReference expressionType = binaryExpression.Left.ExpressionType;
					if (!this.IsSubtype(expressionType, variable.get_VariableType()))
					{
						binaryExpression.Right = new ExplicitCastExpression(binaryExpression.Right, expressionType, null);
					}
				}
			}
		}

		private void AddCasts()
		{
			Expression expression;
			foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> variableToDefineUseInfo in this.context.MethodContext.StackData.VariableToDefineUseInfo)
			{
				foreach (int usedAt in variableToDefineUseInfo.Value.UsedAt)
				{
					if (!this.offsetToExpression.TryGetValue(usedAt, out expression))
					{
						continue;
					}
					this.AddCastIfNeeded(expression, variableToDefineUseInfo.Key);
				}
				foreach (int definedAt in variableToDefineUseInfo.Value.DefinedAt)
				{
					Expression item = this.offsetToExpression[definedAt];
					InstructionBlock instructionBlock = this.GetInstructionBlock(definedAt);
					IList<Expression> expressions = this.context.MethodContext.Expressions.BlockExpressions[instructionBlock.First.get_Offset()];
					this.FixAssignmentInList(expressions, item);
				}
			}
		}

		protected virtual ClassHierarchyNode FindLowestCommonAncestor(ICollection<ClassHierarchyNode> typeNodes)
		{
			ClassHierarchyNode classHierarchyNode = null;
			ClassHierarchyNode classHierarchyNode1;
			int num = 0;
			ClassHierarchyNode typeNode = null;
			foreach (ClassHierarchyNode typeNode in typeNodes)
			{
				num++;
			}
			if (num == 1)
			{
				return typeNode;
			}
			Queue<ClassHierarchyNode> classHierarchyNodes = new Queue<ClassHierarchyNode>();
			HashSet<ClassHierarchyNode> classHierarchyNodes1 = new HashSet<ClassHierarchyNode>();
			using (IEnumerator<ClassHierarchyNode> enumerator = typeNodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ClassHierarchyNode current = enumerator.Current;
					if (classHierarchyNodes.Count != 0)
					{
						ClassHierarchyNode classHierarchyNode2 = current;
						while (!classHierarchyNodes1.Contains(classHierarchyNode2))
						{
							if (current.CanAssignTo.Count<ClassHierarchyNode>((ClassHierarchyNode x) => x.IsHardNode) <= 1)
							{
								classHierarchyNode2 = classHierarchyNode2.CanAssignTo.FirstOrDefault<ClassHierarchyNode>((ClassHierarchyNode x) => x.IsHardNode);
							}
							else
							{
								classHierarchyNode1 = null;
								return classHierarchyNode1;
							}
						}
						while (classHierarchyNodes.Peek() != classHierarchyNode2)
						{
							classHierarchyNodes1.Remove(classHierarchyNodes.Dequeue());
						}
					}
					else
					{
						for (ClassHierarchyNode i = current; i != null; i = classHierarchyNode)
						{
							classHierarchyNodes.Enqueue(i);
							classHierarchyNodes1.Add(i);
							classHierarchyNode = null;
							using (IEnumerator<ClassHierarchyNode> enumerator1 = i.CanAssignTo.GetEnumerator())
							{
								while (enumerator1.MoveNext())
								{
									ClassHierarchyNode current1 = enumerator1.Current;
									if (!current1.IsHardNode)
									{
										continue;
									}
									classHierarchyNode = current1;
									goto Label2;
								}
							}
						Label2:
						}
					}
				}
				return classHierarchyNodes.Peek();
			}
			return classHierarchyNode1;
		}

		private void FixAssignmentInList(IList<Expression> expressionList, Expression value)
		{
			foreach (Expression expression in expressionList)
			{
				if (expression.CodeNodeType != CodeNodeType.BinaryExpression)
				{
					continue;
				}
				BinaryExpression binaryExpression = expression as BinaryExpression;
				if (!binaryExpression.IsAssignmentExpression || binaryExpression.Right != value)
				{
					continue;
				}
				this.AddAssignmentCastIfNeeded(binaryExpression);
				return;
			}
		}

		private bool[,] GeenrateAdjacencyMatrix(Dictionary<ClassHierarchyNode, int> nodeToIndex)
		{
			int count = this.inferenceGraph.Count;
			bool[,] flagArray = new bool[count, count];
			foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
			{
				int item = nodeToIndex[classHierarchyNode];
				foreach (ClassHierarchyNode canAssignTo in classHierarchyNode.CanAssignTo)
				{
					flagArray[item, nodeToIndex[canAssignTo]] = true;
				}
			}
			return flagArray;
		}

		private Dictionary<ClassHierarchyNode, int> GenerateNodeToIndex()
		{
			int num = 0;
			Dictionary<ClassHierarchyNode, int> classHierarchyNodes = new Dictionary<ClassHierarchyNode, int>();
			foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
			{
				classHierarchyNodes.Add(classHierarchyNode, num);
				num++;
			}
			return classHierarchyNodes;
		}

		private InstructionBlock GetInstructionBlock(int instructionOffset)
		{
			InstructionBlock instructionBlocks;
			Instruction item = this.context.MethodContext.ControlFlowGraph.OffsetToInstruction[instructionOffset];
			while (!this.context.MethodContext.ControlFlowGraph.InstructionToBlockMapping.TryGetValue(item.get_Offset(), out instructionBlocks))
			{
				item = item.get_Previous();
			}
			return instructionBlocks;
		}

		private void InferIntegerTypes(HashSet<VariableReference> resolvedVariables)
		{
			(new IntegerTypeInferer(this.context, this.offsetToExpression)).InferIntegerTypes(resolvedVariables);
		}

		public void InferTypes()
		{
			List<ClassHierarchyNode> classHierarchyNodes;
			ClassHierarchyNode classHierarchyNode;
			HashSet<VariableReference> variableReferences = (new GreedyTypeInferer(this.context, this.offsetToExpression)).InferTypes();
			ClassHierarchyBuilder classHierarchyBuilder = new ClassHierarchyBuilder(this.offsetToExpression, this.context.MethodContext.ControlFlowGraph.OffsetToInstruction, this.context);
			this.inferenceGraph = classHierarchyBuilder.BuildHierarchy(variableReferences);
			this.MergeConnectedComponents();
			this.RemoveTransitiveEdges();
			this.ProcessSingleConstraints();
			ClassHierarchyNode classHierarchyNode1 = null;
			using (IEnumerator<ClassHierarchyNode> enumerator = this.inferenceGraph.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ClassHierarchyNode current = enumerator.Current;
					if (!current.IsHardNode || !(current.NodeType.get_FullName() == "System.Object"))
					{
						continue;
					}
					classHierarchyNode1 = current;
					classHierarchyNodes = new List<ClassHierarchyNode>()
					{
						classHierarchyNode1
					};
					using (enumerator = this.inferenceGraph.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							classHierarchyNode = current;
							if (classHierarchyNode.IsHardNode)
							{
								continue;
							}
							classHierarchyNodes.Add(classHierarchyNode);
						}
					}
					this.MergeNodes(classHierarchyNodes);
					this.InferIntegerTypes(variableReferences);
					this.AddCasts();
					return;
				}
			}
			classHierarchyNodes = new List<ClassHierarchyNode>()
			{
				classHierarchyNode1
			};
			foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
			{
				if (classHierarchyNode.IsHardNode)
				{
					continue;
				}
				classHierarchyNodes.Add(classHierarchyNode);
			}
			this.MergeNodes(classHierarchyNodes);
			this.InferIntegerTypes(variableReferences);
			this.AddCasts();
		}

		private bool IsArrayAssignable(TypeReference type, TypeReference supposedSubType)
		{
			if (!(type is ArrayType) || !(supposedSubType is ArrayType))
			{
				return false;
			}
			ArrayType arrayType = type as ArrayType;
			ArrayType arrayType1 = supposedSubType as ArrayType;
			if (!this.IsSubtype(arrayType.get_ElementType(), arrayType1.get_ElementType()))
			{
				return false;
			}
			return arrayType.get_Dimensions().get_Count() == arrayType1.get_Dimensions().get_Count();
		}

		private bool IsSubtype(TypeReference type, TypeReference supposedSubType)
		{
			bool flag;
			type = this.RemoveModifiers(type);
			supposedSubType = this.RemoveModifiers(supposedSubType);
			if (supposedSubType.GetFriendlyFullName(null) == type.GetFriendlyFullName(null) || type.get_FullName() == "System.Object")
			{
				return true;
			}
			if (this.IsArrayAssignable(type, supposedSubType))
			{
				return true;
			}
			TypeDefinition typeDefinition = supposedSubType.Resolve();
			if (typeDefinition == null)
			{
				return true;
			}
			if (type is GenericInstanceType)
			{
				type = type.GetElementType();
			}
		Label2:
			while (typeDefinition != null)
			{
				if (TypeNamesComparer.AreEqual(type, typeDefinition))
				{
					return true;
				}
				Mono.Collections.Generic.Collection<TypeReference>.Enumerator enumerator = typeDefinition.get_Interfaces().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!TypeNamesComparer.AreEqual(type, enumerator.get_Current()))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					goto Label0;
				}
				finally
				{
					enumerator.Dispose();
				}
				return flag;
			}
			return false;
		Label0:
			if (typeDefinition.get_BaseType() == null)
			{
				return false;
			}
			typeDefinition = typeDefinition.get_BaseType().Resolve();
			goto Label2;
		}

		private bool MergeAnySingleParent()
		{
			return this.MergeSingleParent((ClassHierarchyNode x) => true);
		}

		protected void MergeConnectedComponents()
		{
			foreach (ICollection<ClassHierarchyNode> connectedComponent in (new ConnectedComponentsFinder(this.inferenceGraph)).GetConnectedComponents())
			{
				this.MergeNodes(connectedComponent);
			}
		}

		protected void MergeNodes(ICollection<ClassHierarchyNode> nodeCollection)
		{
			if (nodeCollection.Count <= 1)
			{
				return;
			}
			ClassHierarchyNode classHierarchyNode = new ClassHierarchyNode(nodeCollection);
			foreach (ClassHierarchyNode containedNode in classHierarchyNode.ContainedNodes)
			{
				this.inferenceGraph.Remove(containedNode);
			}
			this.inferenceGraph.Add(classHierarchyNode);
		}

		private void MergeSingleChildConstraints()
		{
			bool flag = true;
			while (flag)
			{
				ClassHierarchyNode[] classHierarchyNodeArray = null;
				flag = false;
				foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
				{
					if (classHierarchyNode.SubTypes.Count != 1 || classHierarchyNode.IsHardNode)
					{
						continue;
					}
					ClassHierarchyNode classHierarchyNode1 = classHierarchyNode.SubTypes.First<ClassHierarchyNode>();
					classHierarchyNodeArray = new ClassHierarchyNode[] { classHierarchyNode, classHierarchyNode1 };
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					continue;
				}
				this.MergeNodes(classHierarchyNodeArray);
			}
		}

		private bool MergeSingleParent(Func<ClassHierarchyNode, bool> chooseParentPred)
		{
			bool flag = false;
			ClassHierarchyNode[] classHierarchyNodeArray = null;
			foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
			{
				if (classHierarchyNode.IsHardNode || classHierarchyNode.SubTypes.Count != 1)
				{
					continue;
				}
				ClassHierarchyNode classHierarchyNode1 = classHierarchyNode.SubTypes.First<ClassHierarchyNode>();
				if (!chooseParentPred(classHierarchyNode1))
				{
					continue;
				}
				flag = true;
				classHierarchyNodeArray = new ClassHierarchyNode[] { classHierarchyNode, classHierarchyNode1 };
				if (flag)
				{
					this.MergeNodes(classHierarchyNodeArray);
				}
				return flag;
			}
			if (flag)
			{
				this.MergeNodes(classHierarchyNodeArray);
			}
			return flag;
		}

		private bool MergeSingleSoftParent()
		{
			return this.MergeSingleParent((ClassHierarchyNode x) => !x.IsHardNode);
		}

		private bool MergeWithLowestCommonAncestor()
		{
			bool flag = false;
			ClassHierarchyNode[] classHierarchyNodeArray = null;
			foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
			{
				if (classHierarchyNode.IsHardNode)
				{
					continue;
				}
				flag = true;
				HashSet<ClassHierarchyNode> classHierarchyNodes = new HashSet<ClassHierarchyNode>();
				foreach (ClassHierarchyNode subType in classHierarchyNode.SubTypes)
				{
					if (subType.IsClassNode)
					{
						classHierarchyNodes.Add(subType);
					}
					else
					{
						flag = false;
						goto Label1;
					}
				}
			Label1:
				if (!flag)
				{
					continue;
				}
				ClassHierarchyNode classHierarchyNode1 = this.FindLowestCommonAncestor(classHierarchyNodes);
				if (classHierarchyNode1 == null || classHierarchyNode1 == classHierarchyNode)
				{
					flag = false;
				}
				else
				{
					classHierarchyNodeArray = new ClassHierarchyNode[] { classHierarchyNode1, classHierarchyNode };
					if (flag)
					{
						this.MergeNodes(classHierarchyNodeArray);
					}
					return flag;
				}
			}
			if (flag)
			{
				this.MergeNodes(classHierarchyNodeArray);
			}
			return flag;
		}

		private void ProcessSingleConstraints()
		{
			bool flag = true;
			while (flag)
			{
				flag = false;
				this.MergeSingleChildConstraints();
				if (this.MergeWithLowestCommonAncestor())
				{
					flag = true;
				}
				else if (!this.MergeSingleSoftParent())
				{
					if (!this.MergeAnySingleParent())
					{
						continue;
					}
					flag = true;
				}
				else
				{
					flag = true;
				}
			}
		}

		private TypeReference RemoveModifiers(TypeReference type)
		{
			if (type is OptionalModifierType)
			{
				return (type as OptionalModifierType).get_ElementType();
			}
			if (!(type is RequiredModifierType))
			{
				return type;
			}
			return (type as RequiredModifierType).get_ElementType();
		}

		private void RemoveSubtype(ClassHierarchyNode superType, ClassHierarchyNode subType)
		{
			if (!superType.SubTypes.Contains(subType))
			{
				throw new ArgumentOutOfRangeException(String.Format("No such relation between {0} and {1}.", this, subType));
			}
			superType.SubTypes.Remove(subType);
			subType.CanAssignTo.Remove(superType);
		}

		private void RemoveTransitiveEdges()
		{
			Dictionary<ClassHierarchyNode, int> index = this.GenerateNodeToIndex();
			bool[,] flagArray = this.GeenrateAdjacencyMatrix(index);
			this.WarsawTransitiveClosure(flagArray);
			foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
			{
				foreach (ClassHierarchyNode classHierarchyNode1 in this.inferenceGraph)
				{
					foreach (ClassHierarchyNode classHierarchyNode2 in this.inferenceGraph)
					{
						int item = index[classHierarchyNode];
						int num = index[classHierarchyNode1];
						int item1 = index[classHierarchyNode2];
						if (!flagArray[item, num] || !flagArray[num, item1] || classHierarchyNode.IsHardNode && classHierarchyNode2.IsHardNode || !classHierarchyNode.SubTypes.Contains(classHierarchyNode2))
						{
							continue;
						}
						this.RemoveSubtype(classHierarchyNode, classHierarchyNode2);
					}
				}
			}
		}

		private void WarsawTransitiveClosure(bool[,] matrix)
		{
			bool flag;
			int length = matrix.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length; j++)
				{
					for (int k = 0; k < length; k++)
					{
						bool[,] flagArray = matrix;
						int num = j;
						int num1 = k;
						if (matrix[j, k])
						{
							flag = true;
						}
						else
						{
							flag = (!matrix[j, i] ? false : matrix[i, k]);
						}
						flagArray[num, num1] = flag;
					}
				}
			}
		}
	}
}