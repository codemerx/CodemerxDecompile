using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class MultiAssignPattern : CommonPatterns, ICodePattern
	{
		private readonly HashSet<VariableDefinition> variablesToRemove = new HashSet<VariableDefinition>();

		private readonly MethodSpecificContext methodContext;

		public MultiAssignPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext) : base(patternsContext, methodContext.Method.Module.TypeSystem)
		{
			this.methodContext = methodContext;
		}

		private void RemoveFromContext()
		{
			foreach (VariableDefinition variableDefinition in this.variablesToRemove)
			{
				if (!this.patternsContext.VariableToDefineUseCountContext.Remove(variableDefinition))
				{
					continue;
				}
				this.patternsContext.VariableToSingleAssignmentMap.Remove(variableDefinition);
			}
			this.variablesToRemove.Clear();
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			VariableDefinition variableDefinition;
			result = null;
			replacedStatementsCount = 0;
			startIndex = 0;
			startIndex = 0;
			while (startIndex < statements.Count)
			{
				if (this.TryMatchInternal(statements, startIndex, out result, out replacedStatementsCount, out variableDefinition))
				{
					base.FixContext(variableDefinition, 0, replacedStatementsCount - 1, (ExpressionStatement)result);
					this.RemoveFromContext();
					return true;
				}
				startIndex++;
			}
			this.variablesToRemove.Clear();
			return false;
		}

		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount, out VariableDefinition xVariableDef)
		{
			VariableReference variableReference;
			int i;
			result = null;
			replacedStatementsCount = 0;
			xVariableDef = null;
			if (statements.Count < 1 || statements[startIndex].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			BinaryExpression expression = (statements[startIndex] as ExpressionStatement).Expression as BinaryExpression;
			if (!base.IsAssignToVariableExpression(expression, out variableReference) || !this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variableReference.Resolve()))
			{
				return false;
			}
			Expression right = expression.Right;
			for (i = startIndex + 1; i < statements.Count; i++)
			{
				Statement item = statements[i];
				if (item.CodeNodeType != CodeNodeType.ExpressionStatement || !String.IsNullOrEmpty(item.Label))
				{
					break;
				}
				BinaryExpression binaryExpression = (item as ExpressionStatement).Expression as BinaryExpression;
				if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression || (binaryExpression.Right as VariableReferenceExpression).Variable != variableReference)
				{
					break;
				}
				if (binaryExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					return false;
				}
				if (binaryExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableDefinition variableDefinition = (binaryExpression.Left as VariableReferenceExpression).Variable.Resolve();
					if (variableDefinition == variableReference)
					{
						return false;
					}
					this.variablesToRemove.Add(variableDefinition);
				}
				right = new BinaryExpression(BinaryOperator.Assign, binaryExpression.Left, right, this.typeSystem, null, false);
			}
			replacedStatementsCount = i - startIndex;
			if (replacedStatementsCount == 1)
			{
				return false;
			}
			BinaryExpression binaryExpression1 = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(variableReference, null), right, this.typeSystem, null, false);
			result = new ExpressionStatement(binaryExpression1)
			{
				Parent = statements[startIndex].Parent
			};
			xVariableDef = variableReference.Resolve();
			return true;
		}
	}
}