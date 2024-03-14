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
	internal class NullCoalescingPattern : CommonPatterns, ICodePattern
	{
		private readonly MethodSpecificContext methodContext;

		public NullCoalescingPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext) : base(patternsContext, methodContext.Method.get_Module().get_TypeSystem())
		{
			this.methodContext = methodContext;
		}

		private bool ContainsDummyAssignment(BlockStatement theThen, VariableReference xVariableReference)
		{
			StackVariableDefineUseInfo stackVariableDefineUseInfo;
			if (theThen.Statements.Count == 0 || theThen.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement || !String.IsNullOrEmpty(theThen.Statements[0].Label))
			{
				return false;
			}
			BinaryExpression expression = (theThen.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (expression == null || expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(expression.Right as VariableReferenceExpression).Variable != (object)xVariableReference)
			{
				return false;
			}
			if (!this.methodContext.StackData.VariableToDefineUseInfo.TryGetValue((expression.Left as VariableReferenceExpression).Variable.Resolve(), out stackVariableDefineUseInfo))
			{
				return false;
			}
			return stackVariableDefineUseInfo.UsedAt.Count == 0;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 2;
			startIndex = 0;
			while (startIndex + 1 < statements.Count)
			{
				if (this.TryMatchInternal(statements, startIndex, out result))
				{
					return true;
				}
				startIndex++;
			}
			return false;
		}

		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result)
		{
			VariableReference variableReference;
			VariableReference variableReference1;
			result = null;
			if (statements.Count < startIndex + 2)
			{
				return false;
			}
			if (statements[startIndex].CodeNodeType != CodeNodeType.ExpressionStatement || statements[startIndex + 1].CodeNodeType != CodeNodeType.IfStatement)
			{
				return false;
			}
			if (!String.IsNullOrEmpty(statements[startIndex + 1].Label))
			{
				return false;
			}
			BinaryExpression expression = (statements[startIndex] as ExpressionStatement).Expression as BinaryExpression;
			if (!base.IsAssignToVariableExpression(expression, out variableReference))
			{
				return false;
			}
			Expression right = expression.Right;
			IfStatement item = statements[startIndex + 1] as IfStatement;
			int num = (this.ContainsDummyAssignment(item.Then, variableReference) ? 1 : 0);
			if (item.Else != null || item.Then.Statements.Count != 1 + num || item.Then.Statements[num].CodeNodeType != CodeNodeType.ExpressionStatement || !String.IsNullOrEmpty(item.Then.Statements[num].Label))
			{
				return false;
			}
			BinaryExpression condition = item.Condition as BinaryExpression;
			if (condition == null || condition.Operator != BinaryOperator.ValueEquality || condition.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(condition.Left as VariableReferenceExpression).Variable != (object)variableReference || condition.Right.CodeNodeType != CodeNodeType.LiteralExpression || (condition.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			BinaryExpression binaryExpression = (item.Then.Statements[num] as ExpressionStatement).Expression as BinaryExpression;
			if (binaryExpression == null || !base.IsAssignToVariableExpression(binaryExpression, out variableReference1) || (object)variableReference1 != (object)variableReference)
			{
				return false;
			}
			Expression right1 = binaryExpression.Right;
			if (!right.HasType || !right1.HasType || right.ExpressionType.get_FullName() != right1.ExpressionType.get_FullName())
			{
				return false;
			}
			BinaryExpression binaryExpression1 = new BinaryExpression(BinaryOperator.NullCoalesce, right, right1, this.typeSystem, null, false);
			BinaryExpression binaryExpression2 = new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(variableReference, null), binaryExpression1, this.typeSystem, null, false);
			result = new ExpressionStatement(binaryExpression2)
			{
				Parent = statements[startIndex].Parent
			};
			base.FixContext(variableReference.Resolve(), 1, num + 1, result as ExpressionStatement);
			return true;
		}
	}
}