using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	public class DeclareVariablesOnFirstAssignment : BaseCodeTransformer, IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private readonly HashSet<VariableDefinition> not_assigned = new HashSet<VariableDefinition>();

		private readonly Dictionary<VariableDefinition, DeclareVariablesOnFirstAssignment.StatementDeclaration> variableDeclarations = new Dictionary<VariableDefinition, DeclareVariablesOnFirstAssignment.StatementDeclaration>();

		private readonly Stack<Statement> statements = new Stack<Statement>();

		private readonly Stack<CodeNodeType> codeNodeTypes = new Stack<CodeNodeType>();

		private DeclareVariablesOnFirstAssignment.State state;

		public DeclareVariablesOnFirstAssignment()
		{
		}

		private bool GetResultOnFirstOccurrence(VariableDefinition variable)
		{
			DeclareVariablesOnFirstAssignment.StatementDeclaration statementDeclaration;
			if (this.state == DeclareVariablesOnFirstAssignment.State.LocateDeclarations)
			{
				if (this.variableDeclarations.TryGetValue(variable, out statementDeclaration) && !statementDeclaration.UsedInOtherStatements)
				{
					statementDeclaration.UsedInOtherStatements = !this.IsChildOfCurrentStatement(statementDeclaration.Statement);
				}
				else if (!this.methodContext.Variables.Contains(variable) && this.codeNodeTypes.Peek() == CodeNodeType.BinaryExpression)
				{
					this.not_assigned.Add(variable);
					return true;
				}
			}
			return false;
		}

		private bool IsChildOfCurrentStatement(Statement statement)
		{
			bool flag;
			Stack<Statement>.Enumerator enumerator = this.statements.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != statement)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void PopulateNotAssigned()
		{
			this.not_assigned.Clear();
			foreach (VariableDefinition variable in this.methodContext.Variables)
			{
				if (this.methodContext.VariablesToNotDeclare.Contains(variable))
				{
					continue;
				}
				this.not_assigned.Add(variable);
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.methodContext = context.MethodContext;
			this.PopulateNotAssigned();
			this.codeNodeTypes.Push(CodeNodeType.BlockStatement);
			this.VisitBlockStatement(block);
			this.codeNodeTypes.Pop();
			return this.ReplaceDeclarations(block);
		}

		private bool RemoveVariable(VariableDefinition variable)
		{
			bool flag = this.ShouldRemoveVariable(variable);
			if (flag)
			{
				this.methodContext.RemoveVariable(variable);
			}
			this.not_assigned.Remove(variable);
			return flag;
		}

		private BlockStatement ReplaceDeclarations(BlockStatement block)
		{
			this.state = DeclareVariablesOnFirstAssignment.State.ReplaceDeclarations;
			this.PopulateNotAssigned();
			block = (BlockStatement)this.VisitBlockStatement(block);
			this.state = DeclareVariablesOnFirstAssignment.State.LocateDeclarations;
			return block;
		}

		private bool ShouldRemoveVariable(VariableDefinition variable)
		{
			DeclareVariablesOnFirstAssignment.StatementDeclaration statementDeclaration;
			if (this.state != DeclareVariablesOnFirstAssignment.State.ReplaceDeclarations)
			{
				return false;
			}
			if (this.variableDeclarations.TryGetValue(variable, out statementDeclaration))
			{
				return !statementDeclaration.UsedInOtherStatements;
			}
			return !this.methodContext.VariableAssignmentData.ContainsKey(variable);
		}

		private void TryAddNewVariableDeclaration(VariableDefinition variable)
		{
			DeclareVariablesOnFirstAssignment.StatementDeclaration statementDeclaration;
			AssignmentType assignmentType;
			if (this.state != DeclareVariablesOnFirstAssignment.State.LocateDeclarations)
			{
				return;
			}
			Statement statement = this.statements.Peek();
			if (this.variableDeclarations.TryGetValue(variable, out statementDeclaration))
			{
				if (!statementDeclaration.UsedInOtherStatements)
				{
					statementDeclaration.UsedInOtherStatements = !this.IsChildOfCurrentStatement(statement);
					return;
				}
			}
			else if (!this.methodContext.VariableAssignmentData.TryGetValue(variable, out assignmentType) || assignmentType == AssignmentType.SingleAssignment)
			{
				DeclareVariablesOnFirstAssignment.StatementDeclaration statementDeclaration1 = new DeclareVariablesOnFirstAssignment.StatementDeclaration(statement)
				{
					UsedInOtherStatements = this.codeNodeTypes.Peek() != CodeNodeType.BinaryExpression
				};
				this.variableDeclarations.Add(variable, statementDeclaration1);
			}
		}

		private bool TryDiscardVariable(VariableDefinition variable)
		{
			if (!this.not_assigned.Contains(variable))
			{
				return this.GetResultOnFirstOccurrence(variable);
			}
			if (!this.methodContext.Variables.Contains(variable))
			{
				return false;
			}
			this.TryAddNewVariableDeclaration(variable);
			return this.RemoveVariable(variable);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			bool flag;
			flag = (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ? true : node.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression);
			if (flag)
			{
				this.codeNodeTypes.Push(CodeNodeType.BinaryExpression);
			}
			node.Left = (Expression)this.Visit(node.Left);
			if (flag)
			{
				this.codeNodeTypes.Pop();
			}
			node.Right = (Expression)this.Visit(node.Right);
			return node;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression)
			{
				return this.VisitAssignExpression(node);
			}
			return base.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			this.statements.Push(node);
			ICodeNode codeNode = base.VisitBlockStatement(node);
			this.statements.Pop();
			return codeNode;
		}

		public override ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			this.codeNodeTypes.Push(CodeNodeType.BinaryExpression);
			node.Variable = (VariableDeclarationExpression)this.Visit(node.Variable);
			this.codeNodeTypes.Pop();
			node.Collection = (Expression)this.Visit(node.Collection);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public override ICodeNode VisitForStatement(ForStatement node)
		{
			this.statements.Push(node);
			base.VisitForStatement(node);
			this.statements.Pop();
			return node;
		}

		public override ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			if (this.state == DeclareVariablesOnFirstAssignment.State.LocateDeclarations)
			{
				this.Visit((node.CloneExpressionOnly() as LambdaExpression).Body);
			}
			return node;
		}

		public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.TryDiscardVariable(node.Variable);
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			VariableDefinition variable = (VariableDefinition)node.Variable;
			if (!this.TryDiscardVariable(variable))
			{
				return node;
			}
			if (variable.VariableType.IsByReference)
			{
				return new RefVariableDeclarationExpression(variable, node.UnderlyingSameMethodInstructions);
			}
			return new VariableDeclarationExpression(variable, node.UnderlyingSameMethodInstructions);
		}

		private enum State
		{
			LocateDeclarations,
			ReplaceDeclarations
		}

		private class StatementDeclaration
		{
			public Statement Statement
			{
				get;
				set;
			}

			public bool UsedInOtherStatements
			{
				get;
				set;
			}

			public StatementDeclaration(Statement statement)
			{
				this.Statement = statement;
			}
		}
	}
}