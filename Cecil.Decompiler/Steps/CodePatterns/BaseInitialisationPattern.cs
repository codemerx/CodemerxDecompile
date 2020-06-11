using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	abstract class BaseInitialisationPattern : CommonPatterns, ICodePattern
	{
		public BaseInitialisationPattern(CodePatternsContext patternsContext, TypeSystem ts)
			: base(patternsContext, ts)
		{
		}

		public virtual bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			startIndex = -1;
			result = null;
			replacedStatementsCount = -1;

			if (statements == null || statements.Count - startIndex < 2)
			{
				return false;
			}

			for (int i = statements.Count - 1; i >= 0; i--)
			{
				if (TryMatchInternal(statements, i, out result, out replacedStatementsCount))
				{
					Expression expression = ((result as ExpressionStatement).Expression as BinaryExpression).Left;
					if (expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
					{
						FixContext((expression as VariableReferenceExpression).Variable.Resolve(), 0, replacedStatementsCount - 1, null);
					}
					startIndex = i;
					return true;
				}
			}

			return false;
		}

		protected abstract bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount);

		private bool TryGetAssignment(StatementCollection statements, int startIndex, out BinaryExpression binaryExpression)
		{
			binaryExpression = null;

			var statement = statements[startIndex];
			if (statement.CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			Expression firstExpression = (statement as ExpressionStatement).Expression;

			if (firstExpression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			var result = firstExpression as BinaryExpression;

			if (!result.IsAssignmentExpression)
			{
				return false;
			}

			if (!(result.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ||
				  result.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression ||
				  result.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression ||
				  result.Left.CodeNodeType == CodeNodeType.ThisReferenceExpression ||
				  result.Left.CodeNodeType == CodeNodeType.PropertyReferenceExpression ||
				  result.Left.CodeNodeType == CodeNodeType.ArgumentReferenceExpression ||
				  result.Left.CodeNodeType == CodeNodeType.UnaryExpression && (result.Left as UnaryExpression).Operand.CodeNodeType == CodeNodeType.ArgumentReferenceExpression))
			{
				return false;
			}

			binaryExpression = result;
			return true;
		}

		protected bool TryGetNextExpression(Statement statement, out Expression expression)
		{
			expression = null;

			if (statement.CodeNodeType != CodeNodeType.ExpressionStatement ||
				!string.IsNullOrEmpty(statement.Label))
			{
				return false;
			}

			expression = (statement as ExpressionStatement).Expression;
			return true;
		}

		protected bool TryGetObjectCreation(StatementCollection statements, int startIndex, out ObjectCreationExpression creation, out Expression assignee)
		{
			assignee = null;
			creation = null;

			BinaryExpression binaryExpression;
			if (!TryGetAssignment(statements, startIndex, out binaryExpression))
			{
				return false;
			}

			if (binaryExpression.Right.CodeNodeType != CodeNodeType.ObjectCreationExpression)
			{
				return false;
			}

			assignee = binaryExpression.Left;
			creation = binaryExpression.Right as ObjectCreationExpression;
			return true;
		}

		protected bool TryGetArrayCreation(StatementCollection statements, int startIndex, out ArrayCreationExpression creation, out Expression assignee)
		{
			assignee = null;
			creation = null;

			BinaryExpression binaryExpression;
			if (!TryGetAssignment(statements, startIndex, out binaryExpression))
			{
				return false;
			}

			if (binaryExpression.Right.CodeNodeType != CodeNodeType.ArrayCreationExpression)
			{
				return false;
			}

			var arrayCreation = binaryExpression.Right as ArrayCreationExpression;

			if (!(binaryExpression.Right.HasType && binaryExpression.Right.ExpressionType.IsArray))
			{
				return false;
			}

			/// Implemented for 1-dimentional arrays only.
			/// This covers most of the cases, and is far easier to implement than support
			/// for n-dimentional arrays.
			if (arrayCreation.Dimensions.Count != 1)
			{
				return false;
			}

			foreach (Expression dimention in arrayCreation.Dimensions)
			{
				if (dimention.CodeNodeType != CodeNodeType.LiteralExpression)
				{
					return false;
				}
			}

			creation = binaryExpression.Right as ArrayCreationExpression;
			assignee = binaryExpression.Left;
			return true;
		}

		protected bool CompareTargets(Expression assignee, Expression target)
		{
			if (target == null || assignee == null || target.CodeNodeType != assignee.CodeNodeType)
			{
				return false;
			}

			switch (target.CodeNodeType)
			{
				case CodeNodeType.PropertyReferenceExpression:
					var propertyReference = target as PropertyReferenceExpression;
					var assigneeProperty = assignee as PropertyReferenceExpression;

					if (!propertyReference.Property.Equals(assigneeProperty.Property))
					{
						return false;
					}

					return CompareTargets(assigneeProperty.Target, propertyReference.Target);

				default:
					return target.Equals(assignee);
			}
		}

		protected bool ImplementsInterface(TypeReference type, string interfaceName)
		{
			while (true)
			{
				if (type == null)
				{
					return false;
				}
				TypeDefinition typeDefinition = type.Resolve();

				if (typeDefinition == null)
				{
					return false;
				}

				if (typeDefinition.Interfaces.Any(x => x.FullName == interfaceName))
				{
					return true;
				}

				type = typeDefinition.BaseType;
			}
		}
	}
}
