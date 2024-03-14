using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class CollectionInitializationPattern : BaseInitialisationPattern
	{
		public CollectionInitializationPattern(CodePatternsContext patternsContext, TypeSystem typeSystem) : base(patternsContext, typeSystem)
		{
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
			if (objectCreationExpression.Initializer != null && objectCreationExpression.Initializer.InitializerType != InitializerType.CollectionInitializer)
			{
				return false;
			}
			if (!base.ImplementsInterface(objectCreationExpression.Type, "System.Collections.IEnumerable"))
			{
				return false;
			}
			ExpressionCollection expressionCollection = new ExpressionCollection();
			for (int i = startIndex + 1; i < statements.Count && base.TryGetNextExpression(statements[i], out expression1) && expression1.CodeNodeType == CodeNodeType.MethodInvocationExpression; i++)
			{
				MethodInvocationExpression methodInvocationExpression = expression1 as MethodInvocationExpression;
				MethodDefinition methodDefinition = methodInvocationExpression.MethodExpression.MethodDefinition;
				if (!base.CompareTargets(expression, methodInvocationExpression.MethodExpression.Target) || methodDefinition.get_Name() != "Add" || methodInvocationExpression.Arguments.Count == 0)
				{
					break;
				}
				if (methodInvocationExpression.Arguments.Count != 1)
				{
					BlockExpression blockExpression = new BlockExpression(new ExpressionCollection(
						from x in methodInvocationExpression.Arguments
						select x.Clone()), null);
					expressionCollection.Add(blockExpression);
				}
				else
				{
					expressionCollection.Add(methodInvocationExpression.Arguments[0].Clone());
				}
			}
			if (expressionCollection.Count == 0)
			{
				return false;
			}
			if (objectCreationExpression.Initializer != null)
			{
				foreach (Expression expression2 in expressionCollection)
				{
					objectCreationExpression.Initializer.Expressions.Add(expression2);
				}
			}
			else
			{
				InitializerExpression initializerExpression = new InitializerExpression(expressionCollection, InitializerType.CollectionInitializer)
				{
					IsMultiLine = true
				};
				objectCreationExpression.Initializer = initializerExpression;
			}
			result = statements[startIndex];
			replacedStatementsCount = expressionCollection.Count + 1;
			return true;
		}
	}
}