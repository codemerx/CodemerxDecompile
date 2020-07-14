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

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveDelegateCachingStep : BaseCodeTransformer, IDecompilationStep
	{
		protected DecompilationContext context;

		protected Dictionary<FieldDefinition, Expression> fieldToReplacingExpressionMap;

		protected Dictionary<VariableReference, Expression> variableToReplacingExpressionMap;

		protected Dictionary<VariableReference, Statement> initializationsToRemove;

		private RemoveDelegateCachingStep.DelegateCachingVersion cachingVersion;

		public RemoveDelegateCachingStep()
		{
		}

		private bool CheckFieldCaching(IfStatement theIf)
		{
			BinaryExpression condition = theIf.Condition as BinaryExpression;
			if (condition.Operator != BinaryOperator.ValueEquality || condition.Right.CodeNodeType != CodeNodeType.LiteralExpression || (condition.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			FieldDefinition right = (condition.Left as FieldReferenceExpression).Field.Resolve();
			if (right == null || !right.get_IsStatic() || !right.get_IsPrivate())
			{
				return false;
			}
			BinaryExpression expression = (theIf.Then.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (expression == null || !expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(expression.Left as FieldReferenceExpression).Field.Resolve() != (object)right)
			{
				return false;
			}
			if (this.fieldToReplacingExpressionMap.ContainsKey(right))
			{
				throw new Exception("A caching field cannot be assigned more than once.");
			}
			if (!right.IsCompilerGenerated(true))
			{
				return false;
			}
			TypeDefinition typeDefinition = right.get_FieldType().Resolve();
			if (typeDefinition == null || typeDefinition.get_BaseType() == null || typeDefinition.get_BaseType().get_FullName() != "System.MulticastDelegate")
			{
				return false;
			}
			this.fieldToReplacingExpressionMap[right] = expression.Right;
			return true;
		}

		private bool CheckIfStatement(IfStatement theIf)
		{
			if (!this.CheckIfStatementStructure(theIf))
			{
				return false;
			}
			BinaryExpression condition = theIf.Condition as BinaryExpression;
			if (condition.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
			{
				return this.CheckFieldCaching(theIf);
			}
			if (condition.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return false;
			}
			return this.CheckVariableCaching(theIf);
		}

		private bool CheckIfStatementStructure(IfStatement theIf)
		{
			if (theIf.Else == null && theIf.Condition.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				if (theIf.Then.Statements.Count == 1 && theIf.Then.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement)
				{
					this.cachingVersion = RemoveDelegateCachingStep.DelegateCachingVersion.V1;
					return true;
				}
				if (theIf.Then.Statements.Count == 3)
				{
					ExpressionStatement item = theIf.Then.Statements[0] as ExpressionStatement;
					if (item == null)
					{
						return false;
					}
					BinaryExpression expression = item.Expression as BinaryExpression;
					if (expression == null)
					{
						return false;
					}
					if (!expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.initializationsToRemove.ContainsKey((expression.Right as VariableReferenceExpression).Variable))
					{
						return false;
					}
					if (theIf.Then.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement)
					{
						return false;
					}
					ExpressionStatement expressionStatement = theIf.Then.Statements[2] as ExpressionStatement;
					if (expressionStatement == null)
					{
						return false;
					}
					BinaryExpression binaryExpression = expressionStatement.Expression as BinaryExpression;
					if (binaryExpression == null)
					{
						return false;
					}
					if (!binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || binaryExpression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.initializationsToRemove.ContainsKey((binaryExpression.Right as VariableReferenceExpression).Variable))
					{
						return false;
					}
					this.cachingVersion = RemoveDelegateCachingStep.DelegateCachingVersion.V2;
					return true;
				}
			}
			return false;
		}

		private bool CheckVariableCaching(IfStatement theIf)
		{
			BinaryExpression condition = theIf.Condition as BinaryExpression;
			if (condition.Operator != BinaryOperator.ValueEquality || condition.Right.CodeNodeType != CodeNodeType.LiteralExpression || (condition.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			VariableReference variable = (condition.Left as VariableReferenceExpression).Variable;
			if (!this.initializationsToRemove.ContainsKey(variable))
			{
				return false;
			}
			int num = (this.cachingVersion == RemoveDelegateCachingStep.DelegateCachingVersion.V1 ? 0 : 1);
			BinaryExpression expression = (theIf.Then.Statements[num] as ExpressionStatement).Expression as BinaryExpression;
			if (expression == null || !expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(expression.Left as VariableReferenceExpression).Variable != (object)variable)
			{
				return false;
			}
			if (this.variableToReplacingExpressionMap.ContainsKey(variable))
			{
				throw new Exception("A caching variable cannot be assigned more than once.");
			}
			TypeDefinition typeDefinition = variable.get_VariableType().Resolve();
			if (typeDefinition == null || typeDefinition.get_BaseType() == null || typeDefinition.get_BaseType().get_FullName() != "System.MulticastDelegate")
			{
				return false;
			}
			this.variableToReplacingExpressionMap[variable] = expression.Right;
			return true;
		}

		private bool CheckVariableInitialization(ExpressionStatement node)
		{
			if (!node.IsAssignmentStatement())
			{
				return false;
			}
			BinaryExpression expression = node.Expression as BinaryExpression;
			if (expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return false;
			}
			Expression right = expression.Right;
			if (right.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{
				right = (right as ExplicitCastExpression).Expression;
			}
			if ((right.CodeNodeType != CodeNodeType.LiteralExpression || (right as LiteralExpression).Value != null) && right.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return false;
			}
			if (right.CodeNodeType == CodeNodeType.FieldReferenceExpression)
			{
				FieldReferenceExpression fieldReferenceExpression = right as FieldReferenceExpression;
				TypeDefinition typeDefinition = fieldReferenceExpression.ExpressionType.Resolve();
				if (typeDefinition == null || typeDefinition.get_BaseType() == null || typeDefinition.get_BaseType().get_FullName() != "System.MulticastDelegate")
				{
					return false;
				}
				FieldDefinition fieldDefinition = fieldReferenceExpression.Field.Resolve();
				if ((object)fieldDefinition.get_DeclaringType() != (object)this.context.MethodContext.Method.get_DeclaringType() && !fieldDefinition.get_DeclaringType().IsNestedIn(this.context.MethodContext.Method.get_DeclaringType()) || !fieldDefinition.get_DeclaringType().IsCompilerGenerated())
				{
					return false;
				}
			}
			this.initializationsToRemove[(expression.Left as VariableReferenceExpression).Variable] = node;
			return true;
		}

		protected virtual ICodeNode GetIfSubstitution(IfStatement node)
		{
			return null;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.fieldToReplacingExpressionMap = new Dictionary<FieldDefinition, Expression>();
			this.variableToReplacingExpressionMap = new Dictionary<VariableReference, Expression>();
			this.initializationsToRemove = new Dictionary<VariableReference, Statement>();
			BlockStatement blockStatement = (BlockStatement)this.Visit(body);
			this.ProcessInitializations();
			return blockStatement;
		}

		protected virtual void ProcessInitializations()
		{
			this.RemoveInitializations();
		}

		protected void RemoveInitializations()
		{
			foreach (KeyValuePair<VariableReference, Statement> keyValuePair in this.initializationsToRemove)
			{
				if (!this.variableToReplacingExpressionMap.ContainsKey(keyValuePair.Key))
				{
					continue;
				}
				BlockStatement parent = keyValuePair.Value.Parent as BlockStatement;
				if (parent == null)
				{
					throw new Exception("Invalid parent statement.");
				}
				this.context.MethodContext.Variables.Remove(keyValuePair.Key.Resolve());
				parent.Statements.Remove(keyValuePair.Value);
			}
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression)
			{
				if (node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldDefinition fieldDefinition = (node.Left as FieldReferenceExpression).Field.Resolve();
					if (fieldDefinition != null && this.fieldToReplacingExpressionMap.ContainsKey(fieldDefinition))
					{
						throw new Exception("A caching field cannot be assigned more than once.");
					}
				}
				else if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression && this.variableToReplacingExpressionMap.ContainsKey((node.Left as VariableReferenceExpression).Variable))
				{
					throw new Exception("A caching variable cannot be assigned more than once.");
				}
			}
			return base.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			if (this.CheckVariableInitialization(node))
			{
				return node;
			}
			return base.VisitExpressionStatement(node);
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			Expression expression;
			FieldDefinition fieldDefinition = node.Field.Resolve();
			if (fieldDefinition == null || !this.fieldToReplacingExpressionMap.TryGetValue(fieldDefinition, out expression))
			{
				return base.VisitFieldReferenceExpression(node);
			}
			return expression.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			if (this.CheckIfStatement(node))
			{
				return this.GetIfSubstitution(node);
			}
			return base.VisitIfStatement(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			Expression expression;
			if (!this.variableToReplacingExpressionMap.TryGetValue(node.Variable, out expression))
			{
				return base.VisitVariableReferenceExpression(node);
			}
			return expression.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
		}

		private enum DelegateCachingVersion
		{
			V1,
			V2
		}
	}
}