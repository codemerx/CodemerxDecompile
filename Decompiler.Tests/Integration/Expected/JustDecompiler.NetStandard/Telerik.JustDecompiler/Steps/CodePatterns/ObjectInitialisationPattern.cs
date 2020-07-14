using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class ObjectInitialisationPattern : BaseInitialisationPattern
	{
		public ObjectInitialisationPattern(CodePatternsContext patternsContext, TypeSystem typeSystem) : base(patternsContext, typeSystem)
		{
		}

		private string GetName(Expression initializer)
		{
			CodeNodeType codeNodeType = initializer.CodeNodeType;
			if (codeNodeType == CodeNodeType.PropertyInitializerExpression)
			{
				return (initializer as PropertyInitializerExpression).Property.get_Name();
			}
			if (codeNodeType != CodeNodeType.FieldInitializerExpression)
			{
				throw new ArgumentException("Expected field or property");
			}
			return (initializer as FieldInitializerExpression).Field.get_Name();
		}

		private bool IsObjectPropertyOrFieldAssignment(BinaryExpression assignment, Expression assignee)
		{
			if (assignment == null || !assignment.IsAssignmentExpression)
			{
				return false;
			}
			CodeNodeType codeNodeType = assignment.Left.CodeNodeType;
			if (codeNodeType == CodeNodeType.FieldReferenceExpression)
			{
				return base.CompareTargets(assignee, (assignment.Left as FieldReferenceExpression).Target);
			}
			if (codeNodeType != CodeNodeType.PropertyReferenceExpression)
			{
				return false;
			}
			PropertyReferenceExpression left = assignment.Left as PropertyReferenceExpression;
			if (!base.CompareTargets(assignee, left.Target))
			{
				return false;
			}
			if (left.IsSetter && !left.IsIndexer)
			{
				return true;
			}
			return false;
		}

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			ObjectCreationExpression objectCreationExpression;
			Expression expression;
			Expression expression1;
			result = null;
			replacedStatementsCount = 0;
			if (!base.TryGetObjectCreation(statements, startIndex, out objectCreationExpression, out expression))
			{
				return false;
			}
			ExpressionCollection expressionCollection = new ExpressionCollection();
			HashSet<string> strs = new HashSet<string>();
			if (objectCreationExpression.Initializer != null)
			{
				if (objectCreationExpression.Initializer.InitializerType != InitializerType.ObjectInitializer)
				{
					return false;
				}
				foreach (Expression expression2 in objectCreationExpression.Initializer.Expressions)
				{
					string name = this.GetName((expression2 as BinaryExpression).Left);
					strs.Add(name);
				}
			}
			for (int i = startIndex + 1; i < statements.Count && base.TryGetNextExpression(statements[i], out expression1); i++)
			{
				BinaryExpression binaryExpression = expression1 as BinaryExpression;
				if (!this.IsObjectPropertyOrFieldAssignment(binaryExpression, expression))
				{
					break;
				}
				Expression propertyInitializerExpression = null;
				if (binaryExpression.Left.CodeNodeType == CodeNodeType.PropertyReferenceExpression)
				{
					PropertyDefinition property = (binaryExpression.Left as PropertyReferenceExpression).Property;
					if (!this.Visit(property.get_Name(), strs))
					{
						break;
					}
					propertyInitializerExpression = new PropertyInitializerExpression(property, property.get_PropertyType(), binaryExpression.Left.UnderlyingSameMethodInstructions);
				}
				else if (binaryExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldDefinition fieldDefinition = (binaryExpression.Left as FieldReferenceExpression).Field.Resolve();
					if (!this.Visit(fieldDefinition.get_Name(), strs))
					{
						break;
					}
					propertyInitializerExpression = new FieldInitializerExpression(fieldDefinition, fieldDefinition.get_FieldType(), binaryExpression.Left.UnderlyingSameMethodInstructions);
				}
				BinaryExpression binaryExpression1 = new BinaryExpression(BinaryOperator.Assign, propertyInitializerExpression, binaryExpression.Right.Clone(), this.typeSystem, null, false);
				expressionCollection.Add(binaryExpression1);
			}
			if (expressionCollection.Count == 0)
			{
				return false;
			}
			if (objectCreationExpression.Initializer != null)
			{
				foreach (Expression expression3 in expressionCollection)
				{
					objectCreationExpression.Initializer.Expressions.Add(expression3);
				}
			}
			else
			{
				InitializerExpression initializerExpression = new InitializerExpression(expressionCollection, InitializerType.ObjectInitializer)
				{
					IsMultiLine = true
				};
				objectCreationExpression.Initializer = initializerExpression;
			}
			result = statements[startIndex];
			replacedStatementsCount = expressionCollection.Count + 1;
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
	}
}