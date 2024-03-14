using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal abstract class BaseInitialisationPattern : CommonPatterns, ICodePattern
	{
		public BaseInitialisationPattern(CodePatternsContext patternsContext, TypeSystem ts) : base(patternsContext, ts)
		{
		}

		protected bool CompareTargets(Expression assignee, Expression target)
		{
			if (target == null || assignee == null || target.CodeNodeType != assignee.CodeNodeType)
			{
				return false;
			}
			if (target.CodeNodeType != CodeNodeType.PropertyReferenceExpression)
			{
				return target.Equals(assignee);
			}
			PropertyReferenceExpression propertyReferenceExpression = target as PropertyReferenceExpression;
			PropertyReferenceExpression propertyReferenceExpression1 = assignee as PropertyReferenceExpression;
			if (!propertyReferenceExpression.Property.Equals(propertyReferenceExpression1.Property))
			{
				return false;
			}
			return this.CompareTargets(propertyReferenceExpression1.Target, propertyReferenceExpression.Target);
		}

		protected bool ImplementsInterface(TypeReference type, string interfaceName)
		{
			Func<TypeReference, bool> func = null;
			while (type != null)
			{
				TypeDefinition typeDefinition = type.Resolve();
				if (typeDefinition == null)
				{
					return false;
				}
				Mono.Collections.Generic.Collection<TypeReference> interfaces = typeDefinition.get_Interfaces();
				Func<TypeReference, bool> func1 = func;
				if (func1 == null)
				{
					Func<TypeReference, bool> fullName = (TypeReference x) => x.get_FullName() == interfaceName;
					Func<TypeReference, bool> func2 = fullName;
					func = fullName;
					func1 = func2;
				}
				if (interfaces.Any<TypeReference>(func1))
				{
					return true;
				}
				type = typeDefinition.get_BaseType();
			}
			return false;
		}

		protected bool TryGetArrayCreation(StatementCollection statements, int startIndex, out ArrayCreationExpression creation, out Expression assignee)
		{
			BinaryExpression binaryExpression;
			bool flag;
			assignee = null;
			creation = null;
			if (!this.TryGetAssignment(statements, startIndex, out binaryExpression))
			{
				return false;
			}
			if (binaryExpression.Right.CodeNodeType != CodeNodeType.ArrayCreationExpression)
			{
				return false;
			}
			ArrayCreationExpression right = binaryExpression.Right as ArrayCreationExpression;
			if (!binaryExpression.Right.HasType || !binaryExpression.Right.ExpressionType.get_IsArray())
			{
				return false;
			}
			if (right.Dimensions.Count != 1)
			{
				return false;
			}
			using (IEnumerator<Expression> enumerator = right.Dimensions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CodeNodeType == CodeNodeType.LiteralExpression)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				creation = binaryExpression.Right as ArrayCreationExpression;
				assignee = binaryExpression.Left;
				return true;
			}
			return flag;
		}

		private bool TryGetAssignment(StatementCollection statements, int startIndex, out BinaryExpression binaryExpression)
		{
			binaryExpression = null;
			Statement item = statements[startIndex];
			if (item.CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			Expression expression = (item as ExpressionStatement).Expression;
			if (expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression binaryExpression1 = expression as BinaryExpression;
			if (!binaryExpression1.IsAssignmentExpression)
			{
				return false;
			}
			if (binaryExpression1.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression && binaryExpression1.Left.CodeNodeType != CodeNodeType.VariableDeclarationExpression && binaryExpression1.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression && binaryExpression1.Left.CodeNodeType != CodeNodeType.ThisReferenceExpression && binaryExpression1.Left.CodeNodeType != CodeNodeType.PropertyReferenceExpression && binaryExpression1.Left.CodeNodeType != CodeNodeType.ArgumentReferenceExpression && (binaryExpression1.Left.CodeNodeType != CodeNodeType.UnaryExpression || (binaryExpression1.Left as UnaryExpression).Operand.CodeNodeType != CodeNodeType.ArgumentReferenceExpression))
			{
				return false;
			}
			binaryExpression = binaryExpression1;
			return true;
		}

		protected bool TryGetNextExpression(Statement statement, out Expression expression)
		{
			expression = null;
			if (statement.CodeNodeType != CodeNodeType.ExpressionStatement || !String.IsNullOrEmpty(statement.Label))
			{
				return false;
			}
			expression = (statement as ExpressionStatement).Expression;
			return true;
		}

		protected bool TryGetObjectCreation(StatementCollection statements, int startIndex, out ObjectCreationExpression creation, out Expression assignee)
		{
			BinaryExpression binaryExpression;
			assignee = null;
			creation = null;
			if (!this.TryGetAssignment(statements, startIndex, out binaryExpression))
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
				if (this.TryMatchInternal(statements, i, out result, out replacedStatementsCount))
				{
					Expression left = ((result as ExpressionStatement).Expression as BinaryExpression).Left;
					if (left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
					{
						base.FixContext((left as VariableReferenceExpression).Variable.Resolve(), 0, replacedStatementsCount - 1, null);
					}
					startIndex = i;
					return true;
				}
			}
			return false;
		}

		protected abstract bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount);
	}
}