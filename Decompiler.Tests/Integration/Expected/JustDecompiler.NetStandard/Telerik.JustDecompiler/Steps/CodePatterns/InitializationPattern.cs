using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class InitializationPattern : CommonPatterns, ICodePattern
	{
		private readonly TypeSpecificContext typeContext;

		private readonly MethodDefinition method;

		public InitializationPattern(CodePatternsContext patternsContext, DecompilationContext context) : base(patternsContext, context.MethodContext.Method.Module.TypeSystem)
		{
			this.typeContext = context.TypeContext;
			this.method = context.MethodContext.Method;
		}

		private bool IsAutoPropertyAssignment(ExpressionStatement propertyAssignmentStatement, out Expression assignedValue, out string propertyFullName)
		{
			propertyFullName = null;
			assignedValue = null;
			BinaryExpression expression = propertyAssignmentStatement.Expression as BinaryExpression;
			if (expression == null || !expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.AutoPropertyConstructorInitializerExpression)
			{
				return false;
			}
			PropertyDefinition property = (expression.Left as AutoPropertyConstructorInitializerExpression).Property;
			if (property == null)
			{
				return false;
			}
			assignedValue = expression.Right;
			propertyFullName = property.FullName;
			return true;
		}

		private bool IsFieldAssignment(ExpressionStatement fieldAssignmentStatement, out Expression assignedValue, out string fieldFullName)
		{
			fieldFullName = null;
			assignedValue = null;
			BinaryExpression expression = fieldAssignmentStatement.Expression as BinaryExpression;
			if (expression == null || !expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return false;
			}
			FieldReference field = (expression.Left as FieldReferenceExpression).Field;
			if (field == null)
			{
				return false;
			}
			assignedValue = expression.Right;
			fieldFullName = field.FullName;
			return true;
		}

		private bool MapAssignmentIntoContext(string memberFullName, Expression assignedValue)
		{
			this.typeContext.AssignmentData.Add(memberFullName, new InitializationAssignment(this.method, assignedValue));
			return true;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			startIndex = 0;
			if (this.TryMatchArrayAssignmentInternal(statements))
			{
				replacedStatementsCount = 2;
				return true;
			}
			replacedStatementsCount = 1;
			return this.TryMatchDirectAssignmentInternal(statements);
		}

		private bool TryMatchArrayAssignmentInternal(StatementCollection statements)
		{
			VariableReference variableReference;
			Expression expression;
			string str;
			if (statements.Count < 2)
			{
				return false;
			}
			ExpressionStatement item = statements[0] as ExpressionStatement;
			if (item == null)
			{
				return false;
			}
			BinaryExpression binaryExpression = item.Expression as BinaryExpression;
			if (binaryExpression == null || !base.IsAssignToVariableExpression(binaryExpression, out variableReference))
			{
				return false;
			}
			Expression right = binaryExpression.Right;
			ExpressionStatement expressionStatement = statements[1] as ExpressionStatement;
			if (expressionStatement == null)
			{
				return false;
			}
			if (!this.IsFieldAssignment(expressionStatement, out expression, out str) && !this.IsAutoPropertyAssignment(expressionStatement, out expression, out str))
			{
				return false;
			}
			if (expression.CodeNodeType != CodeNodeType.VariableReferenceExpression || (expression as VariableReferenceExpression).Variable != variableReference)
			{
				return false;
			}
			return this.MapAssignmentIntoContext(str, right);
		}

		private bool TryMatchDirectAssignmentInternal(StatementCollection statements)
		{
			string str;
			Expression expression;
			ExpressionStatement item = statements[0] as ExpressionStatement;
			if (item == null || !String.IsNullOrEmpty(statements[0].Label))
			{
				return false;
			}
			if (!this.IsFieldAssignment(item, out expression, out str) && !this.IsAutoPropertyAssignment(item, out expression, out str))
			{
				return false;
			}
			return this.MapAssignmentIntoContext(str, expression);
		}
	}
}