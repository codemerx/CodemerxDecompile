using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler.Inlining;

namespace Telerik.JustDecompiler.Steps
{
	class RemoveUnusedVariablesStep : BaseCodeVisitor, IDecompilationStep
	{

		protected DecompilationContext context;
		private readonly Dictionary<VariableReference, ExpressionStatement> referenceToDeclarationStatementMap = new Dictionary<VariableReference, ExpressionStatement>();
		private BlockStatement theBody;
		private readonly HashSet<VariableReference> bannedVariables = new HashSet<VariableReference>();

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.theBody = body;
			Visit((ICodeNode)body);
			CleanUpUnusedDeclarations();
			return body;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			VariableReference theVariableReference = null;
			if (node.Expression.CodeNodeType == CodeNodeType.BinaryExpression &&
				(node.Expression as BinaryExpression).IsAssignmentExpression)
			{
				Expression left = (node.Expression as BinaryExpression).Left;
				if (left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					theVariableReference = (left as VariableReferenceExpression).Variable;
				}
				else if (left.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
				{
					theVariableReference = (left as VariableDeclarationExpression).Variable;
				}
			}
			if (theVariableReference != null && node.Parent.CodeNodeType == CodeNodeType.BlockStatement && !bannedVariables.Contains(theVariableReference))
			{
				if (!referenceToDeclarationStatementMap.Remove(theVariableReference))
				{
					referenceToDeclarationStatementMap[theVariableReference] = node;
				}
				else
				{
					bannedVariables.Add(theVariableReference);
				}
				base.Visit((node.Expression as BinaryExpression).Right);
				return;
			}

			base.Visit(node.Expression);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			referenceToDeclarationStatementMap.Remove(node.Variable);
			bannedVariables.Add(node.Variable);
		}

		public bool IsOptimisableAssignment(ExpressionStatement statement)
		{
			BinaryExpression expr = statement.Expression as BinaryExpression;
			if (expr == null)
			{
				return false;
			}
			if (!expr.IsAssignmentExpression)
			{
				return false;
			}
			if (expr.Right.CodeNodeType == CodeNodeType.BinaryExpression &&
				((expr.Right as BinaryExpression).IsAssignmentExpression || (expr.Right as BinaryExpression).IsSelfAssign))
			{
				return false;
			}
			if (expr.Left is VariableReferenceExpression || expr.Left is VariableDeclarationExpression)
			{
				return !new SideEffectsFinder().HasSideEffectsRecursive(expr.Right);
			}
			return false;
		}

		public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			if (node.MethodExpression.CodeNodeType == CodeNodeType.LambdaExpression)
			{
				VisitLambdaExpression((LambdaExpression)node.MethodExpression);
				return;
			}
			base.VisitDelegateCreationExpression(node);
		}

		public void CleanUpUnusedDeclarations()
		{
			foreach (KeyValuePair<VariableReference, ExpressionStatement> referenceToStatementPair in referenceToDeclarationStatementMap)
			{
				VariableReference variableReference = referenceToStatementPair.Key;
				context.MethodContext.RemoveVariable(variableReference);

				ExpressionStatement declarationStatement = referenceToStatementPair.Value;
				BlockStatement parentBlock = declarationStatement.Parent as BlockStatement;

				if (IsOptimisableAssignment(declarationStatement))
				{
					TransferLabel(declarationStatement);
					parentBlock.Statements.Remove(declarationStatement);
				}
				else
				{
					/// check if the right side can exist on its own
					Expression rightSide = (declarationStatement.Expression as BinaryExpression).Right;
					if (CanExistInStatement(rightSide))
					{
						if (rightSide.CodeNodeType == CodeNodeType.ParenthesesExpression)
						{
							rightSide = (rightSide as ParenthesesExpression).Expression;
						}
						ExpressionStatement newStatement = new ExpressionStatement(rightSide);
						int index = parentBlock.Statements.IndexOf(declarationStatement);
						parentBlock.AddStatementAt(index + 1, newStatement);
						TransferLabel(declarationStatement);
						parentBlock.Statements.RemoveAt(index);
					}
				}
			}

			HashSet<VariableDefinition> unusedVariables = new HashSet<VariableDefinition>();
			foreach (VariableDefinition variable in context.MethodContext.Variables)
			{
				if (!bannedVariables.Contains(variable))
				{
					unusedVariables.Add(variable);
				}
			}

			foreach (VariableDefinition variable in unusedVariables)
			{
				context.MethodContext.RemoveVariable(variable);
			}
		}

		protected virtual bool CanExistInStatement(Expression expression)
		{
			if (expression.CodeNodeType == CodeNodeType.MethodInvocationExpression ||
				expression.CodeNodeType == CodeNodeType.DelegateInvokeExpression ||
				expression.CodeNodeType == CodeNodeType.AwaitExpression)
			{
				return true;
			}
			BinaryExpression binEx = null;
			if (expression.CodeNodeType == CodeNodeType.ParenthesesExpression)
			{
				ParenthesesExpression parenthesesExpression = expression as ParenthesesExpression;
				if (parenthesesExpression.Expression.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					binEx = parenthesesExpression.Expression as BinaryExpression;
				}
			}
			else if (expression.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				binEx = expression as BinaryExpression;
			}
			if (binEx != null && (binEx.IsAssignmentExpression || binEx.IsSelfAssign))
			{
				return true;
			}
			return false;
		}

		private void TransferLabel(ExpressionStatement expressionStatement)
		{
			if (expressionStatement.Label == string.Empty)
			{
				return;
			}

			string theLabel = expressionStatement.Label;
			Statement currentStatement = expressionStatement;
			while (currentStatement.Parent != null)
			{
				BlockStatement parent = currentStatement.Parent as BlockStatement;
				int statementIndex = parent.Statements.IndexOf(currentStatement);
				if (statementIndex != parent.Statements.Count - 1)
				{
					Statement nextStatement = parent.Statements[statementIndex + 1];
					MoveLabel(nextStatement, theLabel);
					return;
				}
				else if (IsLoopBody(parent))
				{
					Statement firstStatement = parent.Statements[0];
					MoveLabel(firstStatement, theLabel);
					return;
				}

				do
				{
					currentStatement = currentStatement.Parent;
				}
				while (currentStatement.Parent != null && currentStatement.Parent.CodeNodeType != CodeNodeType.BlockStatement);
			}

			EmptyStatement emptyStatement = new EmptyStatement();
			this.theBody.Statements.Add(emptyStatement);

			MoveLabel(emptyStatement, theLabel);
		}

		private void MoveLabel(Statement destination, string theLabel)
		{
			if (destination.Label == string.Empty)
			{
				destination.Label = theLabel;
				this.context.MethodContext.GotoLabels[theLabel] = destination;
			}
			else
			{
				string newLabel = destination.Label;
				foreach (GotoStatement gotoStatement in this.context.MethodContext.GotoStatements)
				{
					if (gotoStatement.TargetLabel == theLabel)
					{
						gotoStatement.TargetLabel = newLabel;
					}
				}

				this.context.MethodContext.GotoLabels.Remove(theLabel);
			}
		}

		private bool IsLoopBody(BlockStatement blockStatement)
		{
			Statement parent = blockStatement.Parent;
			return parent != null && (parent.CodeNodeType == CodeNodeType.DoWhileStatement || parent.CodeNodeType == CodeNodeType.WhileStatement ||
				parent.CodeNodeType == CodeNodeType.ForEachStatement || parent.CodeNodeType == CodeNodeType.ForStatement);
		}
	}
}
