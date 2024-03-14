using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class VisualBasicRemoveDelegateCachingStep : RemoveDelegateCachingStep
	{
		private HashSet<VariableReference> variablesToNotInline;

		private Dictionary<VariableReference, Statement> initializationsToFix;

		public VisualBasicRemoveDelegateCachingStep()
		{
			this.variablesToNotInline = new HashSet<VariableReference>();
			this.initializationsToFix = new Dictionary<VariableReference, Statement>();
		}

		protected override ICodeNode GetIfSubstitution(IfStatement node)
		{
			BinaryExpression condition = node.Condition as BinaryExpression;
			if (condition.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return null;
			}
			FieldDefinition fieldDefinition = (condition.Left as FieldReferenceExpression).Field.Resolve();
			if (!this.fieldToReplacingExpressionMap.ContainsKey(fieldDefinition))
			{
				throw new Exception("Caching field not found.");
			}
			VariableDefinition variableDefinition = new VariableDefinition(fieldDefinition.get_FieldType(), this.context.MethodContext.Method);
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(variableDefinition, null);
			ExpressionStatement expressionStatement = new ExpressionStatement(new BinaryExpression(BinaryOperator.Assign, variableReferenceExpression, this.fieldToReplacingExpressionMap[fieldDefinition], this.context.MethodContext.Method.get_Module().get_TypeSystem(), null, false));
			this.initializationsToRemove.Add(variableDefinition, expressionStatement);
			this.variableToReplacingExpressionMap.Add(variableDefinition, this.fieldToReplacingExpressionMap[fieldDefinition]);
			this.fieldToReplacingExpressionMap[fieldDefinition] = variableReferenceExpression;
			this.context.MethodContext.Variables.Add(variableDefinition);
			this.context.MethodContext.VariablesToRename.Add(variableDefinition);
			return expressionStatement;
		}

		protected override void ProcessInitializations()
		{
			foreach (KeyValuePair<VariableReference, Statement> item in this.initializationsToFix)
			{
				if (!this.variableToReplacingExpressionMap.ContainsKey(item.Key))
				{
					continue;
				}
				((item.Value as ExpressionStatement).Expression as BinaryExpression).Right = this.variableToReplacingExpressionMap[item.Key];
			}
			base.RemoveInitializations();
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			ICodeNode codeNode = base.VisitFieldReferenceExpression(node);
			if (codeNode.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return codeNode;
			}
			return this.Visit(codeNode);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			Expression target = node.GetTarget();
			if (target != null)
			{
				if (target.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableReference variable = (target as VariableReferenceExpression).Variable;
					if (this.variableToReplacingExpressionMap.ContainsKey(variable))
					{
						this.variablesToNotInline.Add(variable);
					}
				}
				else if (target.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldDefinition fieldDefinition = (target as FieldReferenceExpression).Field.Resolve();
					if (this.fieldToReplacingExpressionMap.ContainsKey(fieldDefinition))
					{
						VariableReference variableReference = (this.fieldToReplacingExpressionMap[fieldDefinition] as VariableReferenceExpression).Variable;
						this.variablesToNotInline.Add(variableReference);
					}
				}
			}
			return base.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (!this.variableToReplacingExpressionMap.ContainsKey(node.Variable) || !this.variablesToNotInline.Contains(node.Variable))
			{
				return base.VisitVariableReferenceExpression(node);
			}
			this.initializationsToFix.Add(node.Variable, this.initializationsToRemove[node.Variable]);
			this.initializationsToRemove.Remove(node.Variable);
			return node;
		}
	}
}