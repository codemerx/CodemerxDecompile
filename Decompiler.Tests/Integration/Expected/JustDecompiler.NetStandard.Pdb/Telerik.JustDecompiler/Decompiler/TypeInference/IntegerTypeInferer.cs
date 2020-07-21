using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class IntegerTypeInferer : TypeInferer
	{
		public IntegerTypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression)
		{
			base(context, offsetToExpression);
			return;
		}

		private ClassHierarchyNode FindGreatestCommonDescendant(ICollection<ClassHierarchyNode> typeNodes)
		{
			V_0 = null;
			V_1 = 0x7fffffff;
			V_2 = typeNodes.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_4 = ExpressionTypeInferer.GetTypeIndex(V_3.get_NodeType());
					if (V_4 >= V_1)
					{
						continue;
					}
					V_1 = V_4;
					V_0 = V_3;
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return V_0;
		}

		protected override ClassHierarchyNode FindLowestCommonAncestor(ICollection<ClassHierarchyNode> typeNodes)
		{
			V_0 = null;
			V_1 = -2147483648;
			V_2 = typeNodes.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_4 = ExpressionTypeInferer.GetTypeIndex(V_3.get_NodeType());
					if (V_4 <= V_1)
					{
						continue;
					}
					V_1 = V_4;
					V_0 = V_3;
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return V_0;
		}

		internal void InferIntegerTypes(HashSet<VariableReference> resolved)
		{
			V_0 = new IntegerTypesHierarchyBuilder(this.offsetToExpression, this.context);
			this.inferenceGraph = V_0.BuildHierarchy(resolved);
			try
			{
				this.MergeConnectedComponents();
				this.ReplaceMultipleParentDependencies();
				this.ReplaceMultipleChildConstraints();
				this.MergeWithSingleParent();
				this.MergeWithSingleChild();
			}
			catch (InvalidCastException exception_0)
			{
				if (String.op_Inequality(exception_0.get_Message(), "Cannot infer types."))
				{
					throw;
				}
			}
			return;
		}

		private void MergeWithSingleChild()
		{
			V_0 = false;
			do
			{
				V_1 = null;
				V_2 = this.inferenceGraph.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_3.get_IsHardNode() || V_3.get_CanAssignTo().get_Count() != 1 || V_3.get_SubTypes().get_Count() != 0)
						{
							continue;
						}
						V_4 = null;
						V_5 = V_3.get_CanAssignTo().GetEnumerator();
						try
						{
							if (V_5.MoveNext())
							{
								V_4 = V_5.get_Current();
							}
						}
						finally
						{
							if (V_5 != null)
							{
								V_5.Dispose();
							}
						}
						stackVariable27 = new ClassHierarchyNode[2];
						stackVariable27[0] = V_3;
						stackVariable27[1] = V_4;
						V_1 = (ICollection<ClassHierarchyNode>)stackVariable27;
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
			while (V_0);
			return;
		}

		private void MergeWithSingleParent()
		{
			V_0 = false;
			do
			{
				V_1 = null;
				V_2 = this.inferenceGraph.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						if (V_3.get_IsHardNode() || V_3.get_SubTypes().get_Count() != 1 || V_3.get_CanAssignTo().get_Count() != 0)
						{
							continue;
						}
						V_4 = null;
						V_5 = V_3.get_SubTypes().GetEnumerator();
						try
						{
							if (V_5.MoveNext())
							{
								V_4 = V_5.get_Current();
							}
						}
						finally
						{
							if (V_5 != null)
							{
								V_5.Dispose();
							}
						}
						stackVariable27 = new ClassHierarchyNode[2];
						stackVariable27[0] = V_3;
						stackVariable27[1] = V_4;
						V_1 = (ICollection<ClassHierarchyNode>)stackVariable27;
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
			while (V_0);
			return;
		}

		private void ReplaceMultipleChildConstraints()
		{
			V_0 = false;
			do
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
						V_4 = true;
						V_6 = V_3.get_CanAssignTo().GetEnumerator();
						try
						{
							while (V_6.MoveNext())
							{
								if (V_6.get_Current().get_IsHardNode())
								{
									continue;
								}
								V_4 = false;
								goto Label1;
							}
						}
						finally
						{
							if (V_6 != null)
							{
								V_6.Dispose();
							}
						}
					Label1:
						if (!V_4 || V_3.get_CanAssignTo().get_Count() == 0)
						{
							continue;
						}
						V_0 = true;
						V_5 = this.FindGreatestCommonDescendant(V_3.get_CanAssignTo());
						stackVariable32 = new ClassHierarchyNode[2];
						stackVariable32[0] = V_3;
						stackVariable32[1] = V_5;
						V_1 = (ICollection<ClassHierarchyNode>)stackVariable32;
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
			while (V_0);
			return;
		}

		private void ReplaceMultipleParentDependencies()
		{
			V_0 = false;
			do
			{
				V_0 = false;
				V_1 = null;
				V_2 = this.inferenceGraph.GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						V_4 = true;
						if (V_3.get_IsHardNode())
						{
							continue;
						}
						V_6 = V_3.get_SubTypes().GetEnumerator();
						try
						{
							while (V_6.MoveNext())
							{
								if (V_6.get_Current().get_IsHardNode())
								{
									continue;
								}
								V_4 = false;
								goto Label1;
							}
						}
						finally
						{
							if (V_6 != null)
							{
								V_6.Dispose();
							}
						}
					Label1:
						if (!V_4 || V_3.get_SubTypes().get_Count() == 0)
						{
							continue;
						}
						V_0 = true;
						V_5 = this.FindLowestCommonAncestor(V_3.get_SubTypes());
						stackVariable32 = new ClassHierarchyNode[2];
						stackVariable32[0] = V_3;
						stackVariable32[1] = V_5;
						V_1 = (ICollection<ClassHierarchyNode>)stackVariable32;
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
			while (V_0);
			return;
		}
	}
}