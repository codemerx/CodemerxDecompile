using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class ObjectInitialisationPattern : BaseInitialisationPattern
	{
		public ObjectInitialisationPattern(CodePatternsContext patternsContext, TypeSystem typeSystem)
			: base(patternsContext, typeSystem)
		{
		}

		// Person person = new Person { Name = "John", Age = 20 };
		// 
		// ==
		// 
		// Person person = new Person();
		// person.Name = "John";
		// person.Age = 20;

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 0;

			ObjectCreationExpression objectCreation;
			Expression assignee;
			if (!TryGetObjectCreation(statements, startIndex, out objectCreation, out assignee))
			{
				return false;
			}

			ExpressionCollection inlinedExpressions = new ExpressionCollection();
			HashSet<string> visitedPropertyNames = new HashSet<string>();

			if (objectCreation.Initializer != null)
			{
				if (objectCreation.Initializer.InitializerType != InitializerType.ObjectInitializer)
				{
					return false;
				}

				foreach (var item in objectCreation.Initializer.Expressions)
				{
					string name = GetName((item as BinaryExpression).Left);
					visitedPropertyNames.Add(name);
				}
			}

			for (int i = startIndex + 1; i < statements.Count; i++)
			{
				Expression expression;
				if (!TryGetNextExpression(statements[i], out expression))
				{
					break;
				}

				BinaryExpression assignment = expression as BinaryExpression;
				if (!IsObjectPropertyOrFieldAssignment(assignment, assignee))
				{
					break;
				}

				Expression initializer = null;

				if (assignment.Left.CodeNodeType == CodeNodeType.PropertyReferenceExpression)
				{
					PropertyDefinition property = (assignment.Left as PropertyReferenceExpression).Property;
					if (!Visit(property.Name, visitedPropertyNames))
					{
						break;
					}
					initializer = new PropertyInitializerExpression(property, property.PropertyType,
						assignment.Left.UnderlyingSameMethodInstructions);
				}
				else if (assignment.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldDefinition field = (assignment.Left as FieldReferenceExpression).Field.Resolve();
					if (!Visit(field.Name, visitedPropertyNames))
					{
						break;
					}
					initializer = new FieldInitializerExpression(field, field.FieldType,
						assignment.Left.UnderlyingSameMethodInstructions);
				}

				var inlinedAssignment = new BinaryExpression(BinaryOperator.Assign,
					initializer, assignment.Right.Clone(), this.typeSystem, null);
				inlinedExpressions.Add(inlinedAssignment);
			}

			if (inlinedExpressions.Count == 0)
			{
				return false;
			}

			if (objectCreation.Initializer == null)
			{
				var initializer = new InitializerExpression(inlinedExpressions, InitializerType.ObjectInitializer);
				initializer.IsMultiLine = true;
				objectCreation.Initializer = initializer;
			}
			else
			{
				foreach (var item in inlinedExpressions)
				{
					objectCreation.Initializer.Expressions.Add(item);
				}
			}

			result = statements[startIndex];
			replacedStatementsCount = inlinedExpressions.Count + 1;
			return true;
		}

		private bool Visit(string name, ICollection<string> visitedPropertyNames)
		{
			if (visitedPropertyNames.Contains(name))
			{
				return false;
			}

			visitedPropertyNames.Add(name);
			return true;
		}

		private string GetName(Expression initializer)
		{
			switch (initializer.CodeNodeType)
			{
				case CodeNodeType.PropertyInitializerExpression:
					return (initializer as PropertyInitializerExpression).Property.Name;

				case CodeNodeType.FieldInitializerExpression:
					return (initializer as FieldInitializerExpression).Field.Name;

				default:
					throw new ArgumentException("Expected field or property");
			}
		}

		private bool IsObjectPropertyOrFieldAssignment(BinaryExpression assignment, Expression assignee)
		{
			if (assignment == null || !assignment.IsAssignmentExpression)
			{
				return false;
			}

			switch (assignment.Left.CodeNodeType)
			{
				case CodeNodeType.PropertyReferenceExpression:
					PropertyReferenceExpression propertyReference = (assignment.Left as PropertyReferenceExpression);

					if (!CompareTargets(assignee, propertyReference.Target))
					{
						return false;
					}

					if (!propertyReference.IsSetter || propertyReference.IsIndexer)
					{
						return false;
					}

					return true;

				case CodeNodeType.FieldReferenceExpression:
					FieldReferenceExpression fieldReference = (assignment.Left as FieldReferenceExpression);
					return CompareTargets(assignee, fieldReference.Target);

				default:
					return false;
			}
		}
	}
}
