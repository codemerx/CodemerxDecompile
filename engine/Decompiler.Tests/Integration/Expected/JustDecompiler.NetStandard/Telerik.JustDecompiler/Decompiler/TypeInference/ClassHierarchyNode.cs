using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class ClassHierarchyNode
	{
		private TypeReference inferedType;

		private readonly VariableReference variable;

		public ICollection<ClassHierarchyNode> CanAssignTo
		{
			get;
			private set;
		}

		public ICollection<ClassHierarchyNode> ContainedNodes
		{
			get;
			private set;
		}

		public bool IsClassNode
		{
			get
			{
				if (!this.get_IsHardNode())
				{
					return false;
				}
				if (this.inferedType.get_MetadataType() == 18)
				{
					return true;
				}
				return this.inferedType.get_MetadataType() == 28;
			}
		}

		public bool IsHardNode
		{
			get
			{
				return (object)this.inferedType != (object)null;
			}
		}

		public bool IsMergedNode
		{
			get
			{
				return this.get_ContainedNodes().get_Count() > 0;
			}
		}

		public TypeReference NodeType
		{
			get
			{
				return this.inferedType;
			}
		}

		public ICollection<ClassHierarchyNode> SubTypes
		{
			get;
			private set;
		}

		public VariableReference Variable
		{
			get
			{
				return this.variable;
			}
		}

		public ClassHierarchyNode()
		{
			base();
			this.set_CanAssignTo(new HashSet<ClassHierarchyNode>());
			this.set_ContainedNodes(new HashSet<ClassHierarchyNode>());
			this.inferedType = null;
			this.variable = null;
			this.set_SubTypes(new HashSet<ClassHierarchyNode>());
			return;
		}

		public ClassHierarchyNode(VariableReference variable)
		{
			this();
			this.variable = variable;
			return;
		}

		public ClassHierarchyNode(TypeReference type)
		{
			this();
			this.inferedType = type;
			return;
		}

		public ClassHierarchyNode(IEnumerable<ClassHierarchyNode> nodesToMerge)
		{
			this();
			V_0 = null;
			V_1 = nodesToMerge.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!V_2.get_IsHardNode())
					{
						continue;
					}
					if (V_0 != null)
					{
						throw new InvalidCastException("Cannot infer types.");
					}
					V_0 = V_2.inferedType;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.inferedType = V_0;
			V_1 = nodesToMerge.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_3 = V_1.get_Current();
					V_3.inferedType = this.inferedType;
					this.get_ContainedNodes().Add(V_3);
					V_3.UpdateVariablesType();
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.RedirectConstraints();
			return;
		}

		public void AddSupertype(ClassHierarchyNode supertype)
		{
			if (!this.get_CanAssignTo().Contains(supertype))
			{
				this.get_CanAssignTo().Add(supertype);
				supertype.get_SubTypes().Add(this);
			}
			return;
		}

		private bool IsAssignable(TypeReference toAssign, TypeReference recipient)
		{
			if (!toAssign.get_IsPrimitive() || !recipient.get_IsPrimitive())
			{
				return true;
			}
			stackVariable6 = ExpressionTypeInferer.GetTypeIndex(toAssign);
			return stackVariable6 + 1 == ExpressionTypeInferer.GetTypeIndex(recipient);
		}

		private void RedirectConstraints()
		{
			V_0 = new List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>>();
			V_1 = this.get_ContainedNodes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_CanAssignTo().GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							if (this.get_ContainedNodes().Contains(V_4))
							{
								continue;
							}
							if (!V_4.get_IsHardNode() || !V_4.get_NodeType().get_IsPrimitive() || !V_2.get_IsHardNode() || this.IsAssignable(V_2.get_NodeType(), V_4.get_NodeType()))
							{
								V_0.Add(new KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>(V_2, V_4));
							}
							else
							{
								dummyVar0 = V_4.get_SubTypes().Remove(V_2);
							}
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
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			V_5 = V_0.GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					V_7 = V_6.get_Key();
					V_8 = V_6.get_Value();
					this.get_CanAssignTo().Add(V_8);
					dummyVar1 = V_7.get_CanAssignTo().Remove(V_8);
					dummyVar2 = V_8.get_SubTypes().Remove(V_7);
					if (V_8.get_SubTypes().Contains(this))
					{
						continue;
					}
					V_8.get_SubTypes().Add(this);
				}
			}
			finally
			{
				((IDisposable)V_5).Dispose();
			}
			V_0 = new List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>>();
			V_1 = this.get_ContainedNodes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_9 = V_1.get_Current();
					V_3 = V_9.get_SubTypes().GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_10 = V_3.get_Current();
							if (this.get_ContainedNodes().Contains(V_10))
							{
								continue;
							}
							if (!V_10.get_IsHardNode() || !V_10.get_NodeType().get_IsPrimitive() || !V_9.get_IsHardNode() || this.IsAssignable(V_10.get_NodeType(), V_9.get_NodeType()))
							{
								V_0.Add(new KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>(V_9, V_10));
							}
							else
							{
								dummyVar3 = V_10.get_CanAssignTo().Remove(V_9);
							}
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
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			V_5 = V_0.GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_11 = V_5.get_Current();
					V_12 = V_11.get_Key();
					V_13 = V_11.get_Value();
					this.get_SubTypes().Add(V_13);
					dummyVar4 = V_12.get_SubTypes().Remove(V_13);
					dummyVar5 = V_13.get_CanAssignTo().Remove(V_12);
					if (V_13.get_CanAssignTo().Contains(this))
					{
						continue;
					}
					V_13.get_CanAssignTo().Add(this);
				}
			}
			finally
			{
				((IDisposable)V_5).Dispose();
			}
			return;
		}

		public override string ToString()
		{
			if (this.get_IsHardNode())
			{
				return this.inferedType.GetFriendlyFullName(null);
			}
			if (this.get_ContainedNodes().get_Count() <= 0)
			{
				return this.variable.get_Name();
			}
			V_0 = new StringBuilder();
			V_1 = this.get_ContainedNodes().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					dummyVar0 = V_0.Append(V_2.ToString());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0.ToString();
		}

		private void UpdateVariablesType()
		{
			V_0 = this.get_ContainedNodes().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					stackVariable6 = V_0.get_Current();
					stackVariable6.inferedType = this.inferedType;
					stackVariable6.UpdateVariablesType();
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			if (this.variable != null)
			{
				this.variable.set_VariableType(this.inferedType);
			}
			return;
		}
	}
}