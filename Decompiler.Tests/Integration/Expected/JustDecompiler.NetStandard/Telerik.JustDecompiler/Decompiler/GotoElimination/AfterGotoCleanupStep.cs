using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
	[Obsolete]
	internal class AfterGotoCleanupStep : BaseCodeVisitor, IDecompilationStep
	{
		private readonly List<ExpressionStatement> statementsToRemove;

		private List<IfStatement> emptyThenIfs;

		private TypeSystem typeSystem;

		public AfterGotoCleanupStep()
		{
			this.statementsToRemove = new List<ExpressionStatement>();
			this.emptyThenIfs = new List<IfStatement>();
		}

		private void CleanupEmptyIfs(BlockStatement body)
		{
			do
			{
				foreach (IfStatement emptyThenIf in this.emptyThenIfs)
				{
					if (emptyThenIf.Else == null || emptyThenIf.Else.Statements.Count == 0)
					{
						(emptyThenIf.Parent as BlockStatement).Statements.Remove(emptyThenIf);
					}
					else
					{
						emptyThenIf.Then = emptyThenIf.Else;
						emptyThenIf.Else = null;
						Negator.Negate(emptyThenIf.Condition, this.typeSystem);
					}
				}
				this.emptyThenIfs = new List<IfStatement>();
				this.Visit(body);
			}
			while (this.emptyThenIfs.Count != 0);
		}

		private void CleanupRedundantAssignments()
		{
			foreach (ExpressionStatement expressionStatement in this.statementsToRemove)
			{
				(expressionStatement.Parent as BlockStatement).Statements.Remove(expressionStatement);
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.Visit(body);
			this.CleanupRedundantAssignments();
			this.CleanupEmptyIfs(body);
			return body;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			if (node.Expression is BinaryExpression)
			{
				BinaryExpression expression = node.Expression as BinaryExpression;
				if (expression.Operator == BinaryOperator.Assign && expression.Left is VariableReferenceExpression && expression.Right is VariableReferenceExpression && (expression.Left as VariableReferenceExpression).Variable == (expression.Right as VariableReferenceExpression).Variable)
				{
					this.statementsToRemove.Add(node);
				}
			}
		}

		public override void VisitIfStatement(IfStatement node)
		{
			if (node.Then.Statements.Count == 0)
			{
				this.emptyThenIfs.Add(node);
			}
			base.VisitIfStatement(node);
		}
	}
}