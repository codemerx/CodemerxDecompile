using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class IntegerTypesHierarchyBuilder : ClassHierarchyBuilder
	{
		private HashSet<ClassHierarchyNode> notPossibleBooleanNodes;

		public IntegerTypesHierarchyBuilder(Dictionary<int, Expression> offsetToExpression, DecompilationContext context) : base(offsetToExpression, context.MethodContext.ControlFlowGraph.OffsetToInstruction, context)
		{
			this.notPossibleBooleanNodes = new HashSet<ClassHierarchyNode>();
		}

		private void AddEdge(ClassHierarchyNode biggerTypeNode, ClassHierarchyNode smallerTypeNode)
		{
			smallerTypeNode.AddSupertype(biggerTypeNode);
			this.resultingGraph.Add(smallerTypeNode);
			this.resultingGraph.Add(biggerTypeNode);
		}

		protected override void BuildUpHardNodesHierarchy(IEnumerable<ClassHierarchyNode> hardNodes)
		{
			ClassHierarchyNode typeNode = this.GetTypeNode(this.typeSystem.Boolean);
			ClassHierarchyNode classHierarchyNode = this.GetTypeNode(this.typeSystem.Byte);
			this.AddEdge(classHierarchyNode, typeNode);
			typeNode = this.GetTypeNode(this.typeSystem.Byte);
			classHierarchyNode = this.GetTypeNode(this.typeSystem.Char);
			this.AddEdge(classHierarchyNode, typeNode);
			typeNode = this.GetTypeNode(this.typeSystem.Byte);
			classHierarchyNode = this.GetTypeNode(this.typeSystem.Int16);
			this.AddEdge(classHierarchyNode, typeNode);
			typeNode = this.GetTypeNode(this.typeSystem.Char);
			classHierarchyNode = this.GetTypeNode(this.typeSystem.Int32);
			this.AddEdge(classHierarchyNode, typeNode);
			typeNode = this.GetTypeNode(this.typeSystem.Int16);
			classHierarchyNode = this.GetTypeNode(this.typeSystem.Int32);
			this.AddEdge(classHierarchyNode, typeNode);
		}

		protected override ClassHierarchyNode GetTypeNode(TypeReference assignedType)
		{
			string fullName = assignedType.FullName;
			if (!this.typeNameToNode.ContainsKey(fullName))
			{
				ClassHierarchyNode classHierarchyNode = new ClassHierarchyNode(assignedType);
				this.typeNameToNode.Add(fullName, classHierarchyNode);
			}
			return this.typeNameToNode[fullName];
		}

		private List<ClassHierarchyNode> GetUsedPhiVariableNodes(int offset)
		{
			VariableDefinition variableDefinition;
			List<VariableDefinition> variableDefinitions;
			List<ClassHierarchyNode> classHierarchyNodes = new List<ClassHierarchyNode>();
			if (this.methodContext.StackData.InstructionOffsetToAssignedVariableMap.TryGetValue(offset, out variableDefinition) && this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variableDefinition))
			{
				classHierarchyNodes.Add(base.GetVariableNode(variableDefinition));
			}
			if (this.methodContext.StackData.InstructionOffsetToUsedStackVariablesMap.TryGetValue(offset, out variableDefinitions))
			{
				classHierarchyNodes.AddRange(
					from variable in variableDefinitions
					where this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variable)
					select base.GetVariableNode(variable));
			}
			return classHierarchyNodes;
		}

		private bool IsArithmeticOperation(Code code)
		{
			if (code == Code.Add || code == Code.Sub || code == Code.Mul || code == Code.Div || code == Code.Rem || code == Code.Add_Ovf || code == Code.Add_Ovf_Un || code == Code.Sub_Ovf || code == Code.Sub_Ovf_Un || code == Code.Rem_Un || code == Code.Mul_Ovf || code == Code.Mul_Ovf_Un)
			{
				return true;
			}
			return code == Code.Div_Un;
		}

		private bool IsStackVariable(VariableReference varRef)
		{
			return this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(varRef.Resolve());
		}

		protected override ClassHierarchyNode MergeWithVariableTypeIfNeeded(VariableReference variable, ClassHierarchyNode variableNode)
		{
			return variableNode;
		}

		private bool OnlyPhiVariablesUsed(Expression expression)
		{
			switch (expression.CodeNodeType)
			{
				case CodeNodeType.LiteralExpression:
				{
					return true;
				}
				case CodeNodeType.UnaryExpression:
				case CodeNodeType.ArgumentReferenceExpression:
				{
					return false;
				}
				case CodeNodeType.BinaryExpression:
				{
					BinaryExpression binaryExpression = expression as BinaryExpression;
					if (binaryExpression.Left is VariableReferenceExpression)
					{
						if (this.IsStackVariable((binaryExpression.Left as VariableReferenceExpression).Variable))
						{
							return this.OnlyPhiVariablesUsed(binaryExpression.Right);
						}
					}
					else if (binaryExpression.Left is VariableReferenceExpression && this.IsStackVariable((binaryExpression.Right as VariableReferenceExpression).Variable))
					{
						return this.OnlyPhiVariablesUsed(binaryExpression.Left);
					}
					return false;
				}
				case CodeNodeType.VariableReferenceExpression:
				{
					return this.IsStackVariable((expression as VariableReferenceExpression).Variable);
				}
				case CodeNodeType.VariableDeclarationExpression:
				{
					return this.IsStackVariable((expression as VariableDeclarationExpression).Variable);
				}
				default:
				{
					return false;
				}
			}
		}

		protected override void OnPhiVariableAssigned(int instructionOffset, ClassHierarchyNode variableNode)
		{
			Expression item = this.offsetToExpression[instructionOffset];
			TypeReference expressionType = item.ExpressionType;
			if (item is LiteralExpression)
			{
				int value = (Int32)(item as LiteralExpression).Value;
				if (value == 0 || value == 1)
				{
					expressionType = this.typeSystem.Boolean;
				}
				else if (value <= 0xff && value >= 0)
				{
					expressionType = this.typeSystem.Byte;
				}
				else if (value < 0xffff && value >= 0)
				{
					expressionType = this.typeSystem.Char;
				}
			}
			ClassHierarchyNode typeNode = this.GetTypeNode(expressionType);
			typeNode.AddSupertype(variableNode);
			this.resultingGraph.Add(typeNode);
		}

		protected override void OnPhiVariableUsed(int instructionOffset, ClassHierarchyNode variableNode)
		{
			Instruction item = this.offsetToInstruction[instructionOffset];
			if (item.OpCode.Code == Code.Dup || item.OpCode.Code == Code.Pop)
			{
				return;
			}
			ClassHierarchyNode useExpressionTypeNode = base.GetUseExpressionTypeNode(item, variableNode.Variable);
			if (item.OpCode.Code == Code.Switch)
			{
				variableNode.AddSupertype(useExpressionTypeNode);
				this.resultingGraph.Add(useExpressionTypeNode);
				return;
			}
			if (!(useExpressionTypeNode.NodeType.FullName == "System.Int32") || !this.OnlyPhiVariablesUsed(this.offsetToExpression[item.Offset]))
			{
				variableNode.AddSupertype(useExpressionTypeNode);
				this.resultingGraph.Add(useExpressionTypeNode);
				return;
			}
			List<ClassHierarchyNode> usedPhiVariableNodes = this.GetUsedPhiVariableNodes(item.Offset);
			for (int i = 0; i < usedPhiVariableNodes.Count; i++)
			{
				for (int j = i + 1; j < usedPhiVariableNodes.Count; j++)
				{
					usedPhiVariableNodes[i].AddSupertype(usedPhiVariableNodes[j]);
					usedPhiVariableNodes[j].AddSupertype(usedPhiVariableNodes[i]);
				}
			}
			if (this.IsArithmeticOperation(item.OpCode.Code))
			{
				for (int k = 0; k < usedPhiVariableNodes.Count; k++)
				{
					this.notPossibleBooleanNodes.Add(usedPhiVariableNodes[k]);
					ClassHierarchyNode typeNode = this.GetTypeNode(this.typeSystem.Int32);
					if (!typeNode.CanAssignTo.Contains(usedPhiVariableNodes[k]))
					{
						typeNode.CanAssignTo.Add(usedPhiVariableNodes[k]);
						usedPhiVariableNodes[k].SubTypes.Add(typeNode);
					}
				}
			}
		}

		private void RemoveBooleanAsASubtype(ClassHierarchyNode variableNode)
		{
			ClassHierarchyNode typeNode = this.GetTypeNode(this.typeSystem.Boolean);
			if (variableNode.SubTypes.Contains(typeNode))
			{
				variableNode.SubTypes.Remove(typeNode);
				typeNode.CanAssignTo.Remove(variableNode);
			}
		}

		protected override void RemoveImpossibleEdges()
		{
			foreach (ClassHierarchyNode notPossibleBooleanNode in this.notPossibleBooleanNodes)
			{
				this.RemoveBooleanAsASubtype(notPossibleBooleanNode);
			}
		}

		protected override bool ShouldConsiderVariable(VariableReference variableReference)
		{
			return variableReference.VariableType.FullName == "System.Int32";
		}
	}
}