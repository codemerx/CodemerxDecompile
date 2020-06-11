using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    /// <summary>
    /// Represents the nodes in the class heirarchy graphs built by ClassHierarchyBuilder and IntegerTypesHierarchyBuilder.
    /// </summary>
    class ClassHierarchyNode
    {
        private TypeReference inferedType;
        private readonly VariableReference variable;

        public ClassHierarchyNode()
        {
            this.CanAssignTo = new HashSet<ClassHierarchyNode>();
            this.ContainedNodes = new HashSet<ClassHierarchyNode>();
            this.inferedType = null;
            this.variable = null;
            this.SubTypes = new HashSet<ClassHierarchyNode>();
        }

        /// <summary>
        /// Create a simple node, corresponding to <paramref name="variable"/>.
        /// </summary>
        /// <param name="variable">The variable.</param>
        public ClassHierarchyNode(VariableReference variable)
            : this()
        {
            this.variable = variable;
        }

        /// <summary>
        /// Creates a simple node, corresponding to <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type.</param>
        public ClassHierarchyNode(TypeReference type)
            : this()
        {
            this.inferedType = type;
        }

        /// <summary>
        /// Creates a complex node, containing other nodes(simple and/or complex). Attaches the new node properly to the grap, 
        /// according to the nodes it combiens.
        /// </summary>
        /// <param name="nodesToMerge">The enumeration of nodes to be combined.</param>
        public ClassHierarchyNode(IEnumerable<ClassHierarchyNode> nodesToMerge)
            : this()
        {
            TypeReference type = null;
            foreach (ClassHierarchyNode node in nodesToMerge)
            {
                if (node.IsHardNode)
                {
                    if (type != null)
                    {
                        ///Merging collection of nodes with more than one infered type is forbiden.
                        throw new InvalidCastException("Cannot infer types.");
                    }
                    else
                    {
                        type = node.inferedType;
                    }
                }
            }

            inferedType = type;
            foreach (ClassHierarchyNode node in nodesToMerge)
            {
                node.inferedType = this.inferedType;
                this.ContainedNodes.Add(node);
                node.UpdateVariablesType();
            }

            RedirectConstraints();
        }

        /// <summary>
        /// Updates the types of all variables, contained in the current node.
        /// </summary>
        private void UpdateVariablesType()
        {
            foreach (ClassHierarchyNode node in this.ContainedNodes)
            {
                node.inferedType = this.inferedType;
                node.UpdateVariablesType();
            }
            if (this.variable != null)
            {
                this.variable.VariableType = inferedType;
            }
        }

        /// <summary>
        /// Redirects the constraints in child nodes, so that the graph is in valid state. All relations that were once from/to child node will be 
        /// redirected from/to the current node. This method has effect only on complex nodes.
        /// </summary>
        private void RedirectConstraints()
        {
            List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>> listPairs = new List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>>();
           
            #region RedirectSupertypes
            
            foreach (ClassHierarchyNode node in ContainedNodes)
            {
                foreach (ClassHierarchyNode superNode in node.CanAssignTo)
                {
                    if (!ContainedNodes.Contains(superNode))
                    {
                        ///Then the super node is not part of the merged node
                        if (superNode.IsHardNode && superNode.NodeType.IsPrimitive)
                        {
                            if (node.IsHardNode && !IsAssignable(node.NodeType, superNode.NodeType))
                            {
                                ///This checks ensure that the hierarchy tree won't be deformed when dealing with IntegerTypeInference.
                                ///For instance, when merging soft node A, that can assign to Byte and Int32, with the hard node Byte,
                                ///the new merged node will not be able to directly assign to Int32, but rather only to Int16 and Char.
                                superNode.SubTypes.Remove(node);
                                continue;
                            }
                        }
                        listPairs.Add(new KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>(node, superNode));
                    }
                }
            }

            foreach (KeyValuePair<ClassHierarchyNode, ClassHierarchyNode> pair in listPairs)
            {
                ClassHierarchyNode node = pair.Key;
                ClassHierarchyNode superNode = pair.Value;
                this.CanAssignTo.Add(superNode);
                node.CanAssignTo.Remove(superNode);
                superNode.SubTypes.Remove(node);
                if (!superNode.SubTypes.Contains(this))
                {
                    superNode.SubTypes.Add(this);
                }
            }
            #endregion

            #region RedirectSubtypes
            listPairs = new List<KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>>();

            foreach (ClassHierarchyNode node in ContainedNodes)
            {
                foreach (ClassHierarchyNode subTypeNode in node.SubTypes)
                {
                    if (!ContainedNodes.Contains(subTypeNode))
                    {
                        ///Then the subtype node is not part of the merged node
                        if (subTypeNode.IsHardNode && subTypeNode.NodeType.IsPrimitive)
                        {
                            if (node.IsHardNode && !IsAssignable(subTypeNode.NodeType, node.NodeType))
                            {
                                ///This checks ensure that the hierarchy tree won't be deformed when dealing with IntegerTypeInference.
                                ///For instance, when merging soft node A, that can be assigned from Byte and Int32, with the hard node Int32,
                                ///the new merged node will not be directly assignable from Byte, but rather only from Int16 and Char.
                                subTypeNode.CanAssignTo.Remove(node);
                                continue;
                            }
                        }
                        listPairs.Add(new KeyValuePair<ClassHierarchyNode, ClassHierarchyNode>(node, subTypeNode));
                    }
                }
            }
            foreach (KeyValuePair<ClassHierarchyNode, ClassHierarchyNode> pair in listPairs)
            {
                ClassHierarchyNode node = pair.Key;
                ClassHierarchyNode superNode = pair.Value;
                this.SubTypes.Add(superNode);
                node.SubTypes.Remove(superNode);
                superNode.CanAssignTo.Remove(node);
                if (!superNode.CanAssignTo.Contains(this))
                {
                    superNode.CanAssignTo.Add(this);
                }
            }
            #endregion
        }

        /// <summary>
        /// Checks if one primitive type is Directly smaller than the other.
        /// </summary>
        /// <param name="toAssign">The supposed smaller type.</param>
        /// <param name="recipient">The supposed bigger type.</param>
        /// <returns></returns>
        private bool IsAssignable(TypeReference toAssign, TypeReference recipient)
        {
            ///This method handles assignability between primitive types only.
            if (!toAssign.IsPrimitive || !recipient.IsPrimitive)
            {
                return true;
            }
            int index = ExpressionTypeInferer.GetTypeIndex(toAssign);
            int index2 = ExpressionTypeInferer.GetTypeIndex(recipient);
            return index + 1 == index2;
        }

        /// <summary>
        /// The variable corresponding to this node, if any.
        /// </summary>
        public VariableReference Variable
        {
            get
            {
                return variable;
            }
        }

        /// <summary>
        /// The collection of nodes, merged inside this one.
        /// </summary>
        public ICollection<ClassHierarchyNode> ContainedNodes { get; private set; }

        /// <summary>
        /// Flag if this node has type.
        /// </summary>
        public bool IsHardNode
        {
            get
            {
                return inferedType != null;
            }
        }

        /// <summary>
        /// The type infered for this node.
        /// </summary>
        public TypeReference NodeType
        {
            get
            {
                return inferedType;
            }
        }

        /// <summary>
        /// Flag if this node is complex node.
        /// </summary>
        public bool IsMergedNode
        {
            get
            {
                return ContainedNodes.Count > 0;
            }
        }

        /// <summary>
        /// Collection of all nodes, that represent types that can assign to this node.
        /// </summary>
        public ICollection<ClassHierarchyNode> SubTypes { get; private set; }

        /// <summary>
        /// Collection of all nodes, that represent type this node can assign to.
        /// </summary>
        public ICollection<ClassHierarchyNode> CanAssignTo { get; private set; }

        /// <summary>
        /// Checks if this node has a class as infered type.
        /// </summary>
        public bool IsClassNode 
        {
            get 
            {
                if (!IsHardNode)
                {
                    return false;
                }
                return inferedType.MetadataType == MetadataType.Class || inferedType.MetadataType == MetadataType.Object;
            }
        }

        /// <summary>
        /// Adds <paramref name="supertype"/> to the collection of nodes, this one can assign to.
        /// Also adds this node in <paramref name="supertype"/>'s collection of nodes, that can assign to it.
        /// </summary>
        /// <param name="supertype">The node, representing supertype of the current node.</param>
        public void AddSupertype(ClassHierarchyNode supertype)
        {
            if (!CanAssignTo.Contains(supertype))
            {
                CanAssignTo.Add(supertype);
                supertype.SubTypes.Add(this);
            }
        }

        /// <summary>
        /// Implements object's ToString() method. Used to ease debugging.
        /// </summary>
        /// <returns>Returns user-friendly textual representation of the node.</returns>
        public override string ToString()
        {
            if (this.IsHardNode)
            {
                return this.inferedType.GetFriendlyFullName(null);
            }
            else
            {
                if (this.ContainedNodes.Count > 0)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (ClassHierarchyNode containedNode in ContainedNodes)
                    {
                        sb.Append(containedNode.ToString());
                    }
                    return sb.ToString();
                }
                else
                {
                    return this.variable.Name;
                }
            }
        }
    }
}
