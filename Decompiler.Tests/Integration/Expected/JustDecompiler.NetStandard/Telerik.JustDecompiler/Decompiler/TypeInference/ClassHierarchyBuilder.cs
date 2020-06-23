using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class ClassHierarchyBuilder
	{
		protected readonly Dictionary<string, ClassHierarchyNode> typeNameToNode;

		protected readonly Dictionary<int, Expression> offsetToExpression;

		protected readonly Dictionary<string, ClassHierarchyNode> variableNameToNode;

		protected readonly Dictionary<int, Instruction> offsetToInstruction;

		protected readonly HashSet<ClassHierarchyNode> resultingGraph;

		protected readonly MethodSpecificContext methodContext;

		protected readonly TypeSystem typeSystem;

		public ClassHierarchyBuilder(Dictionary<int, Expression> offsetToExpression, Dictionary<int, Instruction> offsetToInstruction, DecompilationContext context)
		{
			this.typeNameToNode = new Dictionary<string, ClassHierarchyNode>();
			this.variableNameToNode = new Dictionary<string, ClassHierarchyNode>();
			this.resultingGraph = new HashSet<ClassHierarchyNode>();
			this.offsetToExpression = offsetToExpression;
			this.offsetToInstruction = offsetToInstruction;
			this.methodContext = context.MethodContext;
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
		}

		private void AddObjectClassNodeIfMIssing()
		{
			ClassHierarchyNode typeNode = this.GetTypeNode(this.typeSystem.Object);
			this.resultingGraph.Add(typeNode);
		}

		internal ICollection<ClassHierarchyNode> BuildHierarchy(HashSet<VariableReference> resolvedVariables)
		{
			foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> variableToDefineUseInfo in this.methodContext.StackData.VariableToDefineUseInfo)
			{
				VariableDefinition key = variableToDefineUseInfo.Key;
				if (resolvedVariables.Contains(key) || !this.ShouldConsiderVariable(key))
				{
					continue;
				}
				ClassHierarchyNode variableNode = this.GetVariableNode(key);
				this.resultingGraph.Add(variableNode);
				foreach (int definedAt in variableToDefineUseInfo.Value.DefinedAt)
				{
					this.OnPhiVariableAssigned(definedAt, variableNode);
				}
				foreach (int usedAt in variableToDefineUseInfo.Value.UsedAt)
				{
					this.OnPhiVariableUsed(usedAt, variableNode);
				}
			}
			this.RemoveImpossibleEdges();
			HashSet<ClassHierarchyNode> classHierarchyNodes = new HashSet<ClassHierarchyNode>();
			foreach (ClassHierarchyNode classHierarchyNode in this.resultingGraph)
			{
				if (!classHierarchyNode.IsHardNode)
				{
					continue;
				}
				classHierarchyNodes.Add(classHierarchyNode);
			}
			this.BuildUpHardNodesHierarchy(classHierarchyNodes);
			return this.resultingGraph;
		}

		protected virtual void BuildUpHardNodesHierarchy(IEnumerable<ClassHierarchyNode> hardNodes)
		{
			Queue<ClassHierarchyNode> classHierarchyNodes = new Queue<ClassHierarchyNode>(hardNodes);
			HashSet<ClassHierarchyNode> classHierarchyNodes1 = new HashSet<ClassHierarchyNode>();
			while (classHierarchyNodes.Count > 0)
			{
				ClassHierarchyNode classHierarchyNode = classHierarchyNodes.Dequeue();
				classHierarchyNodes1.Add(classHierarchyNode);
				this.resultingGraph.Add(classHierarchyNode);
				TypeDefinition typeDefinition = classHierarchyNode.NodeType.Resolve();
				TypeReference baseType = null;
				if (typeDefinition == null)
				{
					continue;
				}
				baseType = typeDefinition.BaseType;
				if (baseType != null)
				{
					ClassHierarchyNode typeNode = this.GetTypeNode(baseType);
					classHierarchyNode.AddSupertype(typeNode);
					if (!classHierarchyNodes1.Contains(typeNode))
					{
						classHierarchyNodes.Enqueue(typeNode);
					}
				}
				if (typeDefinition.IsInterface)
				{
					classHierarchyNode.AddSupertype(this.GetTypeNode(this.typeSystem.Object));
				}
				foreach (TypeReference @interface in (IEnumerable<TypeReference>)classHierarchyNode.NodeType.Resolve().Interfaces)
				{
					ClassHierarchyNode typeNode1 = this.GetTypeNode(@interface);
					classHierarchyNode.AddSupertype(typeNode1);
					if (classHierarchyNodes1.Contains(typeNode1))
					{
						continue;
					}
					classHierarchyNodes.Enqueue(typeNode1);
				}
			}
			this.AddObjectClassNodeIfMIssing();
		}

		protected virtual ClassHierarchyNode GetTypeNode(TypeReference type)
		{
			string fullName = type.FullName;
			if (fullName == "System.Byte" || fullName == "System.SByte" || fullName == "System.Char" || fullName == "System.Int16" || fullName == "System.UInt16" || fullName == "System.Boolean")
			{
				fullName = "System.Int32";
			}
			if (!this.typeNameToNode.ContainsKey(fullName))
			{
				ClassHierarchyNode classHierarchyNode = new ClassHierarchyNode(type);
				this.typeNameToNode.Add(fullName, classHierarchyNode);
			}
			return this.typeNameToNode[fullName];
		}

		protected ClassHierarchyNode GetUseExpressionTypeNode(Instruction instruction, VariableReference variable)
		{
			Code code = instruction.OpCode.Code;
			if (code == Code.Ldobj)
			{
				return this.GetTypeNode(instruction.Operand as TypeReference);
			}
			if (ClassHierarchyBuilder.IsConditionalBranch(code))
			{
				return this.GetTypeNode(this.typeSystem.Boolean);
			}
			if (code == Code.Switch)
			{
				return this.GetTypeNode(this.typeSystem.UInt32);
			}
			Expression item = this.offsetToExpression[instruction.Offset];
			return this.GetUseExpressionTypeNode(item, variable);
		}

		private ClassHierarchyNode GetUseExpressionTypeNode(Expression expression, VariableReference variable)
		{
			CodeNodeType codeNodeType = expression.CodeNodeType;
			if (codeNodeType <= CodeNodeType.SafeCastExpression)
			{
				switch (codeNodeType)
				{
					case CodeNodeType.MethodInvocationExpression:
					{
						return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
					}
					case CodeNodeType.MethodReferenceExpression:
					case CodeNodeType.DelegateCreationExpression:
					case CodeNodeType.LiteralExpression:
					case CodeNodeType.ArgumentReferenceExpression:
					{
						break;
					}
					case CodeNodeType.UnaryExpression:
					{
						return this.GetUseExpressionTypeNode((expression as UnaryExpression).Operand, variable);
					}
					case CodeNodeType.BinaryExpression:
					{
						return this.GetUseInBinaryExpression(expression as BinaryExpression, variable);
					}
					case CodeNodeType.VariableReferenceExpression:
					{
						return this.GetTypeNode((expression as VariableReferenceExpression).Variable.VariableType);
					}
					case CodeNodeType.VariableDeclarationExpression:
					{
						return this.GetTypeNode((expression as VariableDeclarationExpression).Variable.VariableType);
					}
					default:
					{
						if (codeNodeType == CodeNodeType.ExplicitCastExpression || codeNodeType == CodeNodeType.SafeCastExpression)
						{
							return this.GetTypeNode(this.typeSystem.Object);
						}
						break;
					}
				}
			}
			else if (codeNodeType > CodeNodeType.ThisCtorExpression)
			{
				if (codeNodeType == CodeNodeType.ReturnExpression)
				{
					return this.GetTypeNode(this.methodContext.Method.FixedReturnType);
				}
				if (codeNodeType == CodeNodeType.BoxExpression)
				{
					return this.GetTypeNode((expression as BoxExpression).BoxedAs);
				}
			}
			else
			{
				switch (codeNodeType)
				{
					case CodeNodeType.ArrayCreationExpression:
					{
						return this.GetUseInArrayCreation(expression as ArrayCreationExpression, variable);
					}
					case CodeNodeType.ArrayIndexerExpression:
					{
						return this.GetUseInArrayIndexer(expression as ArrayIndexerExpression, variable);
					}
					case CodeNodeType.ObjectCreationExpression:
					{
						return this.GetUseInObjectCreation(expression as ObjectCreationExpression, variable);
					}
					case CodeNodeType.DefaultObjectExpression:
					case CodeNodeType.TypeReferenceExpression:
					case CodeNodeType.UsingStatement:
					{
						break;
					}
					case CodeNodeType.PropertyReferenceExpression:
					{
						return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
					}
					case CodeNodeType.StackAllocExpression:
					{
						return this.GetTypeNode((expression as StackAllocExpression).ExpressionType);
					}
					default:
					{
						if ((int)codeNodeType - (int)CodeNodeType.BaseCtorExpression <= (int)CodeNodeType.UnsafeBlock)
						{
							return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
						}
						break;
					}
				}
			}
			throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
		}

		private ClassHierarchyNode GetUseInArrayCreation(ArrayCreationExpression arrayCreationExpression, VariableReference variable)
		{
			ClassHierarchyNode typeNode;
			foreach (Expression dimension in arrayCreationExpression.Dimensions)
			{
				if (!(dimension is VariableReferenceExpression) || (dimension as VariableReferenceExpression).Variable != variable)
				{
					continue;
				}
				typeNode = this.GetTypeNode(this.typeSystem.Int32);
				return typeNode;
			}
			using (IEnumerator<Expression> enumerator = arrayCreationExpression.Initializer.Expressions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Expression current = enumerator.Current;
					if (!(current is VariableReferenceExpression) || (current as VariableReferenceExpression).Variable != variable)
					{
						continue;
					}
					typeNode = this.GetTypeNode(arrayCreationExpression.ElementType);
					return typeNode;
				}
				throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
			}
			return typeNode;
		}

		private ClassHierarchyNode GetUseInArrayIndexer(ArrayIndexerExpression arrayIndexerExpression, VariableReference variable)
		{
			ClassHierarchyNode typeNode;
			using (IEnumerator<Expression> enumerator = arrayIndexerExpression.Indices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Expression current = enumerator.Current;
					if (!(current is VariableReferenceExpression) || (current as VariableReferenceExpression).Variable != variable)
					{
						continue;
					}
					typeNode = this.GetTypeNode(this.typeSystem.Int32);
					return typeNode;
				}
				TypeReference typeReference = new TypeReference("System", "Array", this.typeSystem.Object.Module, this.typeSystem.Object.Scope);
				return this.GetTypeNode(typeReference);
			}
			return typeNode;
		}

		private ClassHierarchyNode GetUseInBinaryExpression(BinaryExpression binaryExpression, VariableReference variable)
		{
			if (binaryExpression.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression && (binaryExpression.Right as VariableReferenceExpression).Variable == variable)
			{
				if (!(binaryExpression.Left is VariableReferenceExpression) || binaryExpression.Left.HasType)
				{
					return this.GetTypeNode(binaryExpression.Left.ExpressionType);
				}
				return this.GetVariableNode((binaryExpression.Left as VariableReferenceExpression).Variable);
			}
			if (!(binaryExpression.Left is VariableReferenceExpression) || (binaryExpression.Left as VariableReferenceExpression).Variable != variable)
			{
				return this.GetUseExpressionTypeNode(binaryExpression.Left, variable);
			}
			if (!(binaryExpression.Right is VariableReferenceExpression) || binaryExpression.Right.HasType)
			{
				return this.GetTypeNode(binaryExpression.Right.ExpressionType);
			}
			return this.GetVariableNode((binaryExpression.Right as VariableReferenceExpression).Variable);
		}

		private ClassHierarchyNode GetUseInMethodInvocation(MethodInvocationExpression methodInvocationExpression, VariableReference variable)
		{
			Expression expression = null;
			foreach (Expression argument in methodInvocationExpression.Arguments)
			{
				if (!(argument is VariableReferenceExpression) || (argument as VariableReferenceExpression).Variable != variable)
				{
					continue;
				}
				expression = argument;
			}
			if (expression != null)
			{
				int num = methodInvocationExpression.Arguments.IndexOf(expression);
				return this.GetTypeNode(methodInvocationExpression.MethodExpression.Method.Parameters[num].ParameterType);
			}
			if ((methodInvocationExpression.MethodExpression.Target as VariableReferenceExpression).Variable != variable)
			{
				return null;
			}
			return this.GetTypeNode(methodInvocationExpression.MethodExpression.Member.DeclaringType);
		}

		private ClassHierarchyNode GetUseInObjectCreation(ObjectCreationExpression objectCreationExpression, VariableReference variable)
		{
			Expression expression = null;
			foreach (Expression argument in objectCreationExpression.Arguments)
			{
				if (!(argument is VariableReferenceExpression) || (argument as VariableReferenceExpression).Variable != variable)
				{
					continue;
				}
				expression = argument;
			}
			return this.GetTypeNode(objectCreationExpression.Constructor.Parameters[objectCreationExpression.Arguments.IndexOf(expression)].ParameterType);
		}

		protected ClassHierarchyNode GetVariableNode(VariableReference variable)
		{
			string name = variable.Name;
			if (!this.variableNameToNode.ContainsKey(name))
			{
				ClassHierarchyNode classHierarchyNode = this.MergeWithVariableTypeIfNeeded(variable, new ClassHierarchyNode(variable));
				this.variableNameToNode.Add(name, classHierarchyNode);
			}
			return this.variableNameToNode[name];
		}

		private IEnumerable<VariableReference> GetVariables(int offset)
		{
			List<VariableDefinition> variableDefinitions;
			if (!this.methodContext.StackData.InstructionOffsetToUsedStackVariablesMap.TryGetValue(offset, out variableDefinitions))
			{
				return new VariableReference[0];
			}
			List<VariableReference> variableReferences = new List<VariableReference>();
			foreach (VariableDefinition variableDefinition in variableDefinitions)
			{
				if (!this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variableDefinition))
				{
					continue;
				}
				variableReferences.Add(variableDefinition);
			}
			return variableReferences;
		}

		private static bool IsConditionalBranch(Code instructionOpCode)
		{
			if (instructionOpCode == Code.Brtrue || instructionOpCode == Code.Brtrue_S || instructionOpCode == Code.Brfalse)
			{
				return true;
			}
			return instructionOpCode == Code.Brfalse_S;
		}

		protected virtual ClassHierarchyNode MergeWithVariableTypeIfNeeded(VariableReference variable, ClassHierarchyNode variableNode)
		{
			if (variable.VariableType != null)
			{
				ClassHierarchyNode typeNode = this.GetTypeNode(variable.VariableType);
				variableNode = new ClassHierarchyNode(new ClassHierarchyNode[] { variableNode, typeNode });
			}
			return variableNode;
		}

		protected virtual void OnPhiVariableAssigned(int instructionOffset, ClassHierarchyNode variableNode)
		{
			ClassHierarchyNode typeNode;
			TypeReference expressionType = this.offsetToExpression[instructionOffset].ExpressionType;
			if (expressionType != null)
			{
				typeNode = this.GetTypeNode(expressionType);
				typeNode.AddSupertype(variableNode);
				this.resultingGraph.Add(typeNode);
				return;
			}
			foreach (VariableReference variable in this.GetVariables(instructionOffset))
			{
				typeNode = this.GetVariableNode(variable);
				typeNode.AddSupertype(variableNode);
				this.resultingGraph.Add(typeNode);
			}
		}

		protected virtual void OnPhiVariableUsed(int instructionOffset, ClassHierarchyNode variableNode)
		{
		}

		protected virtual void RemoveImpossibleEdges()
		{
		}

		protected virtual bool ShouldConsiderVariable(VariableReference variableReference)
		{
			return variableReference.VariableType == null;
		}
	}
}