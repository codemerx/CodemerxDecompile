using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildFixedStatements : BaseCodeVisitor, IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private readonly IList<VariableReference> variables = new List<VariableReference>();

		public RebuildFixedStatements()
		{
		}

		private FixedStatement GetFixedStatement(Statement statement, List<VariableReference> variableReferences)
		{
			if (statement.CodeNodeType == CodeNodeType.ExpressionStatement)
			{
				ExpressionStatement expressionStatement = statement as ExpressionStatement;
				if (expressionStatement.Expression is BinaryExpression)
				{
					BinaryExpression expression = expressionStatement.Expression as BinaryExpression;
					if (expression.IsAssignmentExpression && expression.Left.ExpressionType.IsPinned && expression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
					{
						variableReferences.Add((expression.Left as VariableReferenceExpression).Variable);
						return new FixedStatement(expressionStatement.Expression, new BlockStatement());
					}
				}
			}
			return null;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.Visit(body);
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				FixedStatement fixedStatement = null;
				List<VariableReference> variableReferences = new List<VariableReference>();
				Statement item = node.Statements[i];
				fixedStatement = this.GetFixedStatement(item, variableReferences);
				if (fixedStatement != null)
				{
					foreach (VariableReference variableReference in variableReferences)
					{
						this.methodContext.RemoveVariable(variableReference);
					}
					node.Statements.RemoveAt(i);
					node.AddStatementAt(i, fixedStatement);
					ExpressionStatement expressionStatement = node.Statements[i + 1] as ExpressionStatement;
					int count = node.Statements.Count;
					VariableReference variable = null;
					RebuildFixedStatements.VariableReferenceVisitor variableReferenceVisitor = new RebuildFixedStatements.VariableReferenceVisitor();
					variableReferenceVisitor.Visit(fixedStatement.Expression);
					variable = variableReferenceVisitor.Variable;
					for (int j = i + 1; j < count; j++)
					{
						RebuildFixedStatements.VariableReferenceVisitor variableReferenceVisitor1 = new RebuildFixedStatements.VariableReferenceVisitor();
						variableReferenceVisitor1.Visit(node.Statements[j]);
						VariableReference variable1 = variableReferenceVisitor1.Variable;
						if (variable1 != null && !this.variables.Contains(variable1))
						{
							this.variables.Add(variable1);
						}
						if (node.Statements[j].CodeNodeType == CodeNodeType.ExpressionStatement)
						{
							expressionStatement = node.Statements[j] as ExpressionStatement;
							if (variable1 != null && variable1 == variable && expressionStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression && (expressionStatement.Expression as BinaryExpression).IsAssignmentExpression && (expressionStatement.Expression as BinaryExpression).Right.CodeNodeType == CodeNodeType.LiteralExpression && ((expressionStatement.Expression as BinaryExpression).Right as LiteralExpression).Value == null)
							{
								node.Statements.RemoveAt(j);
								j--;
								count--;
								break;
							}
						}
						fixedStatement.Body.AddStatement(node.Statements[j]);
						node.Statements.RemoveAt(j);
						j--;
						count--;
					}
					this.ProcessBlock(fixedStatement.Body);
					return;
				}
			}
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			this.ProcessBlock(node);
			foreach (Statement statement in node.Statements)
			{
				this.Visit(statement);
			}
		}

		internal class VariableReferenceVisitor : BaseCodeVisitor
		{
			public VariableReference Variable
			{
				get;
				private set;
			}

			public VariableReferenceVisitor()
			{
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (!node.IsAssignmentExpression)
				{
					base.VisitBinaryExpression(node);
					return;
				}
				this.Visit(node.Left);
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				this.Visit(node.Expression);
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				this.Variable = node.Variable;
			}
		}
	}
}