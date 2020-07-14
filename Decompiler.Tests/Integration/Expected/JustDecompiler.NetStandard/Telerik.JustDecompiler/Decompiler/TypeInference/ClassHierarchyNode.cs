using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
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
				if (!this.IsHardNode)
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
				return this.ContainedNodes.Count > 0;
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
			this.CanAssignTo = new HashSet<ClassHierarchyNode>();
			this.ContainedNodes = new HashSet<ClassHierarchyNode>();
			this.inferedType = null;
			this.variable = null;
			this.SubTypes = new HashSet<ClassHierarchyNode>();
		}

		public ClassHierarchyNode(VariableReference variable) : this()
		{
			this.variable = variable;
		}

		public ClassHierarchyNode(TypeReference type) : this()
		{
			this.inferedType = type;
		}

		public ClassHierarchyNode(IEnumerable<ClassHierarchyNode> nodesToMerge) : this()
		{
			TypeReference typeReference = null;
			foreach (ClassHierarchyNode classHierarchyNode in nodesToMerge)
			{
				if (!classHierarchyNode.IsHardNode)
				{
					continue;
				}
				if (typeReference != null)
				{
					throw new InvalidCastException("Cannot infer types.");
				}
				typeReference = classHierarchyNode.inferedType;
			}
			this.inferedType = typeReference;
			foreach (ClassHierarchyNode classHierarchyNode1 in nodesToMerge)
			{
				classHierarchyNode1.inferedType = this.inferedType;
				this.ContainedNodes.Add(classHierarchyNode1);
				classHierarchyNode1.UpdateVariablesType();
			}
			this.RedirectConstraints();
		}

		public void AddSupertype(ClassHierarchyNode supertype)
		{
			if (!this.CanAssignTo.Contains(supertype))
			{
				this.CanAssignTo.Add(supertype);
				supertype.SubTypes.Add(this);
			}
		}

		private bool IsAssignable(TypeReference toAssign, TypeReference recipient)
		{
			if (!toAssign.get_IsPrimitive() || !recipient.get_IsPrimitive())
			{
				return true;
			}
			return ExpressionTypeInferer.GetTypeIndex(toAssign) + 1 == ExpressionTypeInferer.GetTypeIndex(recipient);
		}

		private void RedirectConstraints()
		{
			List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>> keyValuePairs = new List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>>();
			foreach (ClassHierarchyNode containedNode in this.ContainedNodes)
			{
				foreach (ClassHierarchyNode canAssignTo in containedNode.CanAssignTo)
				{
					if (this.ContainedNodes.Contains(canAssignTo))
					{
						continue;
					}
					if (!canAssignTo.IsHardNode || !canAssignTo.NodeType.get_IsPrimitive() || !containedNode.IsHardNode || this.IsAssignable(containedNode.NodeType, canAssignTo.NodeType))
					{
						keyValuePairs.Add(new KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>(containedNode, canAssignTo));
					}
					else
					{
						canAssignTo.SubTypes.Remove(containedNode);
					}
				}
			}
			foreach (KeyValuePair<ClassHierarchyNode, ClassHierarchyNode> keyValuePair in keyValuePairs)
			{
				ClassHierarchyNode key = keyValuePair.Key;
				ClassHierarchyNode value = keyValuePair.Value;
				this.CanAssignTo.Add(value);
				key.CanAssignTo.Remove(value);
				value.SubTypes.Remove(key);
				if (value.SubTypes.Contains(this))
				{
					continue;
				}
				value.SubTypes.Add(this);
			}
			keyValuePairs = new List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>>();
			foreach (ClassHierarchyNode classHierarchyNode in this.ContainedNodes)
			{
				foreach (ClassHierarchyNode subType in classHierarchyNode.SubTypes)
				{
					if (this.ContainedNodes.Contains(subType))
					{
						continue;
					}
					if (!subType.IsHardNode || !subType.NodeType.get_IsPrimitive() || !classHierarchyNode.IsHardNode || this.IsAssignable(subType.NodeType, classHierarchyNode.NodeType))
					{
						keyValuePairs.Add(new KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>(classHierarchyNode, subType));
					}
					else
					{
						subType.CanAssignTo.Remove(classHierarchyNode);
					}
				}
			}
			foreach (KeyValuePair<ClassHierarchyNode, ClassHierarchyNode> keyValuePair1 in keyValuePairs)
			{
				ClassHierarchyNode key1 = keyValuePair1.Key;
				ClassHierarchyNode value1 = keyValuePair1.Value;
				this.SubTypes.Add(value1);
				key1.SubTypes.Remove(value1);
				value1.CanAssignTo.Remove(key1);
				if (value1.CanAssignTo.Contains(this))
				{
					continue;
				}
				value1.CanAssignTo.Add(this);
			}
		}

		public override string ToString()
		{
			if (this.IsHardNode)
			{
				return this.inferedType.GetFriendlyFullName(null);
			}
			if (this.ContainedNodes.Count <= 0)
			{
				return this.variable.get_Name();
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ClassHierarchyNode containedNode in this.ContainedNodes)
			{
				stringBuilder.Append(containedNode.ToString());
			}
			return stringBuilder.ToString();
		}

		private void UpdateVariablesType()
		{
			foreach (ClassHierarchyNode containedNode in this.ContainedNodes)
			{
				containedNode.inferedType = this.inferedType;
				containedNode.UpdateVariablesType();
			}
			if (this.variable != null)
			{
				this.variable.set_VariableType(this.inferedType);
			}
		}
	}
}