using Mono.Cecil.Cil;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForStatements : BaseCodeVisitor, IDecompilationStep
	{
		public RebuildForStatements()
		{
		}

		private bool CheckTheInitializer(ExpressionStatement statement, out VariableReference forVariable)
		{
			forVariable = null;
			if (statement == null || !statement.IsAssignmentStatement())
			{
				return false;
			}
			return this.TryGetAssignedVariable(statement, out forVariable);
		}

		protected virtual bool CheckTheLoop(WhileStatement theWhile, VariableReference forVariable)
		{
			VariableReference variableReference;
			if (theWhile == null || theWhile.Body.Statements.Count < 2)
			{
				return false;
			}
			if (!(new VariableFinder(forVariable)).FindVariable(theWhile.Condition))
			{
				return false;
			}
			ExpressionStatement item = theWhile.Body.Statements[theWhile.Body.Statements.Count - 1] as ExpressionStatement;
			if (item == null || !this.TryGetAssignedVariable(item, out variableReference) || (object)forVariable != (object)variableReference)
			{
				return false;
			}
			return !(new RebuildForStatements.ContinueFinder()).FindContinue(theWhile.Body);
		}

		private ForStatement CreateForStatement(Statement initializer, WhileStatement theWhile)
		{
			int count = theWhile.Body.Statements.Count - 1;
			string label = theWhile.Body.Statements[count].Label;
			ForStatement forStatement = new ForStatement((initializer as ExpressionStatement).Expression, theWhile.Condition, (theWhile.Body.Statements[count] as ExpressionStatement).Expression, new BlockStatement());
			for (int i = 0; i < count; i++)
			{
				forStatement.Body.AddStatement(theWhile.Body.Statements[i]);
			}
			if (!String.IsNullOrEmpty(label))
			{
				EmptyStatement emptyStatement = new EmptyStatement()
				{
					Label = label
				};
				forStatement.Body.AddStatement(emptyStatement);
			}
			return forStatement;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		protected bool TryGetAssignedVariable(ExpressionStatement node, out VariableReference variable)
		{
			Expression left;
			variable = null;
			BinaryExpression expression = node.Expression as BinaryExpression;
			if (expression != null && !expression.IsAssignmentExpression)
			{
				expression = null;
			}
			if (expression != null)
			{
				left = expression.Left;
			}
			else
			{
				UnaryExpression unaryExpression = node.Expression as UnaryExpression;
				if (unaryExpression == null || unaryExpression.Operator != UnaryOperator.PostDecrement && unaryExpression.Operator != UnaryOperator.PostIncrement && unaryExpression.Operator != UnaryOperator.PreDecrement && unaryExpression.Operator != UnaryOperator.PreIncrement)
				{
					return false;
				}
				left = unaryExpression.Operand;
			}
			if (left.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
			{
				variable = ((VariableDeclarationExpression)left).Variable;
			}
			else if (left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				variable = ((VariableReferenceExpression)left).Variable;
			}
			return (object)variable != (object)null;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			VariableReference variableReference;
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				ExpressionStatement item = node.Statements[i] as ExpressionStatement;
				WhileStatement whileStatement = node.Statements[i + 1] as WhileStatement;
				if (this.CheckTheInitializer(item, out variableReference) && this.CheckTheLoop(whileStatement, variableReference))
				{
					ForStatement forStatement = this.CreateForStatement(item, whileStatement);
					forStatement.Parent = node;
					node.Statements[i] = forStatement;
					node.Statements.RemoveAt(i + 1);
				}
			}
			base.VisitBlockStatement(node);
		}

		private class ContinueFinder : BaseCodeVisitor
		{
			private bool found;

			public ContinueFinder()
			{
			}

			public bool FindContinue(ICodeNode node)
			{
				this.found = false;
				this.Visit(node);
				return this.found;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.found)
				{
					base.Visit(node);
				}
			}

			public override void VisitContinueStatement(ContinueStatement node)
			{
				this.found = true;
			}

			public override void VisitDoWhileStatement(DoWhileStatement node)
			{
			}

			public override void VisitForEachStatement(ForEachStatement node)
			{
			}

			public override void VisitForStatement(ForStatement node)
			{
			}

			public override void VisitWhileStatement(WhileStatement node)
			{
			}
		}
	}
}