using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class IntegerTypeInferer : TypeInferer
	{
		public IntegerTypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression) : base(context, offsetToExpression)
		{
		}

		private ClassHierarchyNode FindGreatestCommonDescendant(ICollection<ClassHierarchyNode> typeNodes)
		{
			ClassHierarchyNode classHierarchyNode = null;
			int num = 0x7fffffff;
			foreach (ClassHierarchyNode typeNode in typeNodes)
			{
				int typeIndex = ExpressionTypeInferer.GetTypeIndex(typeNode.NodeType);
				if (typeIndex >= num)
				{
					continue;
				}
				num = typeIndex;
				classHierarchyNode = typeNode;
			}
			return classHierarchyNode;
		}

		protected override ClassHierarchyNode FindLowestCommonAncestor(ICollection<ClassHierarchyNode> typeNodes)
		{
			ClassHierarchyNode classHierarchyNode = null;
			int num = -2147483648;
			foreach (ClassHierarchyNode typeNode in typeNodes)
			{
				int typeIndex = ExpressionTypeInferer.GetTypeIndex(typeNode.NodeType);
				if (typeIndex <= num)
				{
					continue;
				}
				num = typeIndex;
				classHierarchyNode = typeNode;
			}
			return classHierarchyNode;
		}

		internal void InferIntegerTypes(HashSet<VariableReference> resolved)
		{
			IntegerTypesHierarchyBuilder integerTypesHierarchyBuilder = new IntegerTypesHierarchyBuilder(this.offsetToExpression, this.context);
			this.inferenceGraph = integerTypesHierarchyBuilder.BuildHierarchy(resolved);
			try
			{
				base.MergeConnectedComponents();
				this.ReplaceMultipleParentDependencies();
				this.ReplaceMultipleChildConstraints();
				this.MergeWithSingleParent();
				this.MergeWithSingleChild();
			}
			catch (InvalidCastException invalidCastException)
			{
				if (invalidCastException.Message != "Cannot infer types.")
				{
					throw;
				}
			}
		}

		private void MergeWithSingleChild()
		{
			bool flag = false;
			do
			{
				ICollection<ClassHierarchyNode> classHierarchyNodes = null;
				foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
				{
					if (classHierarchyNode.IsHardNode || classHierarchyNode.CanAssignTo.Count != 1 || classHierarchyNode.SubTypes.Count != 0)
					{
						continue;
					}
					ClassHierarchyNode current = null;
					using (IEnumerator<ClassHierarchyNode> enumerator = classHierarchyNode.CanAssignTo.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							current = enumerator.Current;
						}
					}
					classHierarchyNodes = (ICollection<ClassHierarchyNode>)(new ClassHierarchyNode[] { classHierarchyNode, current });
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					continue;
				}
				base.MergeNodes(classHierarchyNodes);
			}
			while (flag);
		}

		private void MergeWithSingleParent()
		{
			bool flag = false;
			do
			{
				ICollection<ClassHierarchyNode> classHierarchyNodes = null;
				foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
				{
					if (classHierarchyNode.IsHardNode || classHierarchyNode.SubTypes.Count != 1 || classHierarchyNode.CanAssignTo.Count != 0)
					{
						continue;
					}
					ClassHierarchyNode current = null;
					using (IEnumerator<ClassHierarchyNode> enumerator = classHierarchyNode.SubTypes.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							current = enumerator.Current;
						}
					}
					classHierarchyNodes = (ICollection<ClassHierarchyNode>)(new ClassHierarchyNode[] { classHierarchyNode, current });
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					continue;
				}
				base.MergeNodes(classHierarchyNodes);
			}
			while (flag);
		}

		private void ReplaceMultipleChildConstraints()
		{
			bool flag = false;
			do
			{
				flag = false;
				ICollection<ClassHierarchyNode> classHierarchyNodes = null;
				foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
				{
					if (classHierarchyNode.IsHardNode)
					{
						continue;
					}
					bool flag1 = true;
					foreach (ClassHierarchyNode canAssignTo in classHierarchyNode.CanAssignTo)
					{
						if (canAssignTo.IsHardNode)
						{
							continue;
						}
						flag1 = false;
						goto Label1;
					}
				Label1:
					if (!flag1 || classHierarchyNode.CanAssignTo.Count == 0)
					{
						continue;
					}
					flag = true;
					ClassHierarchyNode classHierarchyNode1 = this.FindGreatestCommonDescendant(classHierarchyNode.CanAssignTo);
					classHierarchyNodes = (ICollection<ClassHierarchyNode>)(new ClassHierarchyNode[] { classHierarchyNode, classHierarchyNode1 });
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					continue;
				}
				base.MergeNodes(classHierarchyNodes);
			}
			while (flag);
		}

		private void ReplaceMultipleParentDependencies()
		{
			bool flag = false;
			do
			{
				flag = false;
				ICollection<ClassHierarchyNode> classHierarchyNodes = null;
				foreach (ClassHierarchyNode classHierarchyNode in this.inferenceGraph)
				{
					bool flag1 = true;
					if (classHierarchyNode.IsHardNode)
					{
						continue;
					}
					foreach (ClassHierarchyNode subType in classHierarchyNode.SubTypes)
					{
						if (subType.IsHardNode)
						{
							continue;
						}
						flag1 = false;
						goto Label1;
					}
				Label1:
					if (!flag1 || classHierarchyNode.SubTypes.Count == 0)
					{
						continue;
					}
					flag = true;
					ClassHierarchyNode classHierarchyNode1 = this.FindLowestCommonAncestor(classHierarchyNode.SubTypes);
					classHierarchyNodes = (ICollection<ClassHierarchyNode>)(new ClassHierarchyNode[] { classHierarchyNode, classHierarchyNode1 });
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					continue;
				}
				base.MergeNodes(classHierarchyNodes);
			}
			while (flag);
		}
	}
}