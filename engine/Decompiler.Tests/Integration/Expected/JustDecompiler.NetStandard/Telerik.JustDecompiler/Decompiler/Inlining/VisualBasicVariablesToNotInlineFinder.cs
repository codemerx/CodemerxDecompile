using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class VisualBasicVariablesToNotInlineFinder : BaseCodeVisitor, IVariablesToNotInlineFinder
	{
		private Dictionary<VariableReference, CodeNodeType> variableToAssignedCodeNodeTypeMap;

		private HashSet<VariableDefinition> variablesToNotInline;

		private ILanguage language;

		public VisualBasicVariablesToNotInlineFinder(ILanguage language)
		{
			this.language = language;
		}

		public HashSet<VariableDefinition> Find(Dictionary<int, IList<Expression>> blockExpressions)
		{
			this.ResetInternalState();
			foreach (IList<Expression> value in blockExpressions.Values)
			{
				foreach (Expression expression in (List<Expression>)value)
				{
					this.ProcessExpression(expression);
				}
			}
			return this.variablesToNotInline;
		}

		public HashSet<VariableDefinition> Find(StatementCollection statements)
		{
			this.ResetInternalState();
			this.Visit(statements);
			return this.variablesToNotInline;
		}

		private void ProcessBinaryExpression(BinaryExpression binaryExpression)
		{
			CodeNodeType codeNodeType;
			if (!binaryExpression.IsAssignmentExpression)
			{
				return;
			}
			if (binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return;
			}
			codeNodeType = (!binaryExpression.Right.IsArgumentReferenceToRefParameter() ? binaryExpression.Right.CodeNodeType : CodeNodeType.ArgumentReferenceExpression);
			VariableReference variable = (binaryExpression.Left as VariableReferenceExpression).Variable;
			if (this.variableToAssignedCodeNodeTypeMap.ContainsKey(variable))
			{
				this.variableToAssignedCodeNodeTypeMap[variable] = codeNodeType;
				return;
			}
			this.variableToAssignedCodeNodeTypeMap.Add(variable, codeNodeType);
		}

		private void ProcessDelegateInvokeExpression(DelegateInvokeExpression delegateInvokeExpression)
		{
			if (delegateInvokeExpression.Target == null || delegateInvokeExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return;
			}
			this.ProcessVariableReferenceExpression(delegateInvokeExpression.Target as VariableReferenceExpression);
		}

		private void ProcessExpression(Expression expression)
		{
			if (expression.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				this.ProcessBinaryExpression(expression as BinaryExpression);
				return;
			}
			if (expression.CodeNodeType == CodeNodeType.MethodInvocationExpression)
			{
				this.ProcessMethodInvocation(expression as MethodInvocationExpression);
				return;
			}
			if (expression.CodeNodeType == CodeNodeType.DelegateInvokeExpression)
			{
				this.ProcessDelegateInvokeExpression(expression as DelegateInvokeExpression);
			}
		}

		private void ProcessMethodInvocation(MethodInvocationExpression methodInvocationExpression)
		{
			VariableReferenceExpression operand;
			Expression target = methodInvocationExpression.GetTarget();
			if (target == null)
			{
				return;
			}
			if (target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				if (target.CodeNodeType != CodeNodeType.UnaryExpression)
				{
					return;
				}
				UnaryExpression unaryExpression = target as UnaryExpression;
				if (unaryExpression.Operator != UnaryOperator.AddressDereference || unaryExpression.Operand.CodeNodeType != CodeNodeType.UnaryExpression)
				{
					return;
				}
				UnaryExpression operand1 = unaryExpression.Operand as UnaryExpression;
				if (operand1.Operator != UnaryOperator.AddressReference || operand1.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					return;
				}
				operand = operand1.Operand as VariableReferenceExpression;
			}
			else
			{
				operand = target as VariableReferenceExpression;
			}
			this.ProcessVariableReferenceExpression(operand);
		}

		private void ProcessVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			VariableReference variable = variableReferenceExpression.Variable;
			if (this.variableToAssignedCodeNodeTypeMap.ContainsKey(variable))
			{
				CodeNodeType item = this.variableToAssignedCodeNodeTypeMap[variable];
				VariableDefinition variableDefinition = variable.Resolve();
				if (!this.variablesToNotInline.Contains(variableDefinition) && !this.language.IsValidLineStarter(item))
				{
					this.variablesToNotInline.Add(variableDefinition);
				}
			}
		}

		private void ResetInternalState()
		{
			this.variableToAssignedCodeNodeTypeMap = new Dictionary<VariableReference, CodeNodeType>();
			this.variablesToNotInline = new HashSet<VariableDefinition>();
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			this.ProcessExpression(node.Expression);
		}
	}
}