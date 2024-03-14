using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Inlining;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveUnusedVariablesStep : BaseCodeVisitor, IDecompilationStep
	{
		protected DecompilationContext context;

		private readonly Dictionary<VariableReference, ExpressionStatement> referenceToDeclarationStatementMap = new Dictionary<VariableReference, ExpressionStatement>();

		private BlockStatement theBody;

		private readonly HashSet<VariableReference> bannedVariables = new HashSet<VariableReference>();

		public RemoveUnusedVariablesStep()
		{
		}

		protected virtual bool CanExistInStatement(Expression expression)
		{
			if (expression.CodeNodeType == CodeNodeType.MethodInvocationExpression || expression.CodeNodeType == CodeNodeType.DelegateInvokeExpression || expression.CodeNodeType == CodeNodeType.AwaitExpression)
			{
				return true;
			}
			BinaryExpression binaryExpression = null;
			if (expression.CodeNodeType == CodeNodeType.ParenthesesExpression)
			{
				ParenthesesExpression parenthesesExpression = expression as ParenthesesExpression;
				if (parenthesesExpression.Expression.CodeNodeType == CodeNodeType.BinaryExpression)
				{
					binaryExpression = parenthesesExpression.Expression as BinaryExpression;
				}
			}
			else if (expression.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				binaryExpression = expression as BinaryExpression;
			}
			if (binaryExpression != null && (binaryExpression.IsAssignmentExpression || binaryExpression.IsSelfAssign))
			{
				return true;
			}
			return false;
		}

		public void CleanUpUnusedDeclarations()
		{
			foreach (KeyValuePair<VariableReference, ExpressionStatement> keyValuePair in this.referenceToDeclarationStatementMap)
			{
				VariableReference key = keyValuePair.Key;
				this.context.MethodContext.RemoveVariable(key);
				ExpressionStatement value = keyValuePair.Value;
				BlockStatement parent = value.Parent as BlockStatement;
				if (!this.IsOptimisableAssignment(value))
				{
					Expression right = (value.Expression as BinaryExpression).Right;
					if (!this.CanExistInStatement(right))
					{
						continue;
					}
					if (right.CodeNodeType == CodeNodeType.ParenthesesExpression)
					{
						right = (right as ParenthesesExpression).Expression;
					}
					ExpressionStatement expressionStatement = new ExpressionStatement(right);
					int num = parent.Statements.IndexOf(value);
					parent.AddStatementAt(num + 1, expressionStatement);
					this.TransferLabel(value);
					parent.Statements.RemoveAt(num);
				}
				else
				{
					this.TransferLabel(value);
					parent.Statements.Remove(value);
				}
			}
			HashSet<VariableDefinition> variableDefinitions = new HashSet<VariableDefinition>();
			foreach (VariableDefinition variable in this.context.MethodContext.Variables)
			{
				if (this.bannedVariables.Contains(variable))
				{
					continue;
				}
				variableDefinitions.Add(variable);
			}
			foreach (VariableDefinition variableDefinition in variableDefinitions)
			{
				this.context.MethodContext.RemoveVariable(variableDefinition);
			}
		}

		private bool IsLoopBody(BlockStatement blockStatement)
		{
			Statement parent = blockStatement.Parent;
			if (parent == null)
			{
				return false;
			}
			if (parent.CodeNodeType == CodeNodeType.DoWhileStatement || parent.CodeNodeType == CodeNodeType.WhileStatement || parent.CodeNodeType == CodeNodeType.ForEachStatement)
			{
				return true;
			}
			return parent.CodeNodeType == CodeNodeType.ForStatement;
		}

		public bool IsOptimisableAssignment(ExpressionStatement statement)
		{
			BinaryExpression expression = statement.Expression as BinaryExpression;
			if (expression == null)
			{
				return false;
			}
			if (!expression.IsAssignmentExpression)
			{
				return false;
			}
			if (expression.Right.CodeNodeType == CodeNodeType.BinaryExpression && ((expression.Right as BinaryExpression).IsAssignmentExpression || (expression.Right as BinaryExpression).IsSelfAssign))
			{
				return false;
			}
			if (!(expression.Left is VariableReferenceExpression) && !(expression.Left is VariableDeclarationExpression))
			{
				return false;
			}
			return !(new SideEffectsFinder()).HasSideEffectsRecursive(expression.Right);
		}

		private void MoveLabel(Statement destination, string theLabel)
		{
			if (destination.Label == String.Empty)
			{
				destination.Label = theLabel;
				this.context.MethodContext.GotoLabels[theLabel] = destination;
				return;
			}
			string label = destination.Label;
			foreach (GotoStatement gotoStatement in this.context.MethodContext.GotoStatements)
			{
				if (gotoStatement.TargetLabel != theLabel)
				{
					continue;
				}
				gotoStatement.TargetLabel = label;
			}
			this.context.MethodContext.GotoLabels.Remove(theLabel);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.theBody = body;
			this.Visit(body);
			this.CleanUpUnusedDeclarations();
			return body;
		}

		private void TransferLabel(ExpressionStatement expressionStatement)
		{
			if (expressionStatement.Label == String.Empty)
			{
				return;
			}
			string label = expressionStatement.Label;
			Statement parent = expressionStatement;
			while (parent.Parent != null)
			{
				BlockStatement blockStatement = parent.Parent as BlockStatement;
				int num = blockStatement.Statements.IndexOf(parent);
				if (num != blockStatement.Statements.Count - 1)
				{
					Statement item = blockStatement.Statements[num + 1];
					this.MoveLabel(item, label);
					return;
				}
				if (this.IsLoopBody(blockStatement))
				{
					this.MoveLabel(blockStatement.Statements[0], label);
					return;
				}
				do
				{
					parent = parent.Parent;
				}
				while (parent.Parent != null && parent.Parent.CodeNodeType != CodeNodeType.BlockStatement);
			}
			EmptyStatement emptyStatement = new EmptyStatement();
			this.theBody.Statements.Add(emptyStatement);
			this.MoveLabel(emptyStatement, label);
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

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			VariableReference variable = null;
			if (node.Expression.CodeNodeType == CodeNodeType.BinaryExpression && (node.Expression as BinaryExpression).IsAssignmentExpression)
			{
				Expression left = (node.Expression as BinaryExpression).Left;
				if (left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					variable = (left as VariableReferenceExpression).Variable;
				}
				else if (left.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
				{
					variable = (left as VariableDeclarationExpression).Variable;
				}
			}
			if (variable == null || node.Parent.CodeNodeType != CodeNodeType.BlockStatement || this.bannedVariables.Contains(variable))
			{
				base.Visit(node.Expression);
				return;
			}
			if (this.referenceToDeclarationStatementMap.Remove(variable))
			{
				this.bannedVariables.Add(variable);
			}
			else
			{
				this.referenceToDeclarationStatementMap[variable] = node;
			}
			base.Visit((node.Expression as BinaryExpression).Right);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.referenceToDeclarationStatementMap.Remove(node.Variable);
			this.bannedVariables.Add(node.Variable);
		}
	}
}