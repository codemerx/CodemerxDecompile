using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	public class DeclareTopLevelVariables : BaseCodeVisitor, IDecompilationStep
	{
		private readonly Stack<CodeNodeType> codeNodeTypes = new Stack<CodeNodeType>();

		private readonly Dictionary<VariableReference, bool> variableReferences = new Dictionary<VariableReference, bool>();

		private TypeSystem typeSystem;

		private Mono.Collections.Generic.Collection<VariableDefinition> methodVariables;

		private int inLambdaCount;

		private DecompilationContext context;

		public DeclareTopLevelVariables()
		{
		}

		private int GetIndexOfCtorCall(BlockStatement block)
		{
			for (int i = 0; i < block.Statements.Count; i++)
			{
				if (block.Statements[i].CodeNodeType == CodeNodeType.ExpressionStatement)
				{
					Expression expression = (block.Statements[i] as ExpressionStatement).Expression;
					if (expression.CodeNodeType == CodeNodeType.BaseCtorExpression || expression.CodeNodeType == CodeNodeType.ThisCtorExpression)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private void InsertTopLevelDeclarations(BlockStatement block)
		{
			AssignmentType assignmentType;
			bool flag;
			int indexOfCtorCall = 0;
			if (this.context.MethodContext.Method.IsConstructor)
			{
				indexOfCtorCall = this.GetIndexOfCtorCall(block) + 1;
			}
			int num = 0;
			while (num < this.methodVariables.Count)
			{
				bool flag1 = this.context.MethodContext.VariableAssignmentData.TryGetValue(this.methodVariables[num], out assignmentType);
				if (flag1 && assignmentType == AssignmentType.NotUsed)
				{
					indexOfCtorCall--;
				}
				else if (!this.variableReferences.TryGetValue(this.methodVariables[num], out flag) || flag && (!flag1 || assignmentType != AssignmentType.NotAssigned))
				{
					this.InsertVariableDeclaration(block, indexOfCtorCall, num);
				}
				else
				{
					this.InsertVariableDeclarationAndAssignment(block, indexOfCtorCall, num);
				}
				num++;
				indexOfCtorCall++;
			}
		}

		private void InsertVariableDeclaration(BlockStatement block, int insertIndex, int variableIndex)
		{
			block.AddStatementAt(insertIndex, new ExpressionStatement(new VariableDeclarationExpression(this.methodVariables[variableIndex], null)));
		}

		private void InsertVariableDeclarationAndAssignment(BlockStatement block, int insertIndex, int variableIndex)
		{
			Expression defaultValueExpression = this.methodVariables[variableIndex].VariableType.GetDefaultValueExpression(this.typeSystem);
			if (defaultValueExpression == null)
			{
				this.InsertVariableDeclaration(block, insertIndex, variableIndex);
				return;
			}
			BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, new VariableDeclarationExpression(this.methodVariables[variableIndex], null), defaultValueExpression, this.typeSystem, null, false);
			block.AddStatementAt(insertIndex, new ExpressionStatement(binaryExpression));
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.methodVariables = new Mono.Collections.Generic.Collection<VariableDefinition>();
			foreach (VariableDefinition variable in context.MethodContext.Variables)
			{
				if (context.MethodContext.VariablesToNotDeclare.Contains(variable))
				{
					continue;
				}
				this.methodVariables.Add(variable);
			}
			this.codeNodeTypes.Push(CodeNodeType.BlockStatement);
			this.Visit(block);
			this.codeNodeTypes.Pop();
			this.InsertTopLevelDeclarations(block);
			return block;
		}

		private void VisitAssignExpression(BinaryExpression node)
		{
			bool flag;
			flag = (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ? true : node.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression);
			if (flag)
			{
				this.codeNodeTypes.Push(CodeNodeType.BinaryExpression);
			}
			this.Visit(node.Left);
			if (flag)
			{
				this.codeNodeTypes.Pop();
			}
			this.Visit(node.Right);
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression)
			{
				this.VisitAssignExpression(node);
				return;
			}
			base.VisitBinaryExpression(node);
		}

		public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			if (node.MethodExpression.CodeNodeType != CodeNodeType.LambdaExpression)
			{
				base.VisitDelegateCreationExpression(node);
				return;
			}
			this.VisitLambdaExpression((LambdaExpression)node.MethodExpression);
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			this.inLambdaCount++;
			base.VisitLambdaExpression(node);
			this.inLambdaCount--;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			MethodDefinition methodDefinition = node.MethodExpression.MethodDefinition;
			if (methodDefinition == null)
			{
				base.VisitMethodInvocationExpression(node);
				return;
			}
			this.Visit(node.MethodExpression);
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				UnaryExpression item = node.Arguments[i] as UnaryExpression;
				if (item == null || item.Operator != UnaryOperator.AddressReference || item.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression || !methodDefinition.Parameters[i].IsOutParameter())
				{
					this.Visit(node.Arguments[i]);
				}
				else
				{
					VariableReference variable = (item.Operand as VariableReferenceExpression).Variable;
					if (!this.variableReferences.ContainsKey(variable))
					{
						this.variableReferences.Add(variable, true);
					}
				}
			}
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (!this.variableReferences.ContainsKey(node.Variable))
			{
				this.variableReferences.Add(node.Variable, (this.codeNodeTypes.Peek() != CodeNodeType.BinaryExpression ? false : this.inLambdaCount == 0));
			}
			base.VisitVariableReferenceExpression(node);
		}
	}
}