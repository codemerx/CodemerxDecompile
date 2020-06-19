using System;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class CollectionInitializationPattern : BaseInitialisationPattern
	{
		public CollectionInitializationPattern(CodePatternsContext patternsContext, TypeSystem typeSystem)
			: base(patternsContext, typeSystem)
		{
		}

		// MyCollection list = new MyCollection() { 1, { 2, 2 } , 3 };
		// 
		// ==
		// 
		// MyCollection temp = new MyCollection();
		// temp.Add(1);
		// temp.Add(2, 2);
		// temp.Add(3);
		// MyCollection list = temp;

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

			if (objectCreation.Initializer != null && objectCreation.Initializer.InitializerType != InitializerType.CollectionInitializer)
			{
				return false;
			}

			if (!ImplementsInterface(objectCreation.Type, "System.Collections.IEnumerable"))
			{
				return false;
			}

			ExpressionCollection addedExpressions = new ExpressionCollection();

			for (int i = startIndex + 1; i < statements.Count; i++)
			{
				Expression expression;
				if (!TryGetNextExpression(statements[i], out expression))
				{
					break;
				}

				if (expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
				{
					break;
				}
				MethodInvocationExpression methodInvocation = (expression as MethodInvocationExpression);
				MethodDefinition methodDefinition = methodInvocation.MethodExpression.MethodDefinition;

				if (!CompareTargets(assignee, methodInvocation.MethodExpression.Target))
				{
					break;
				}

				if (methodDefinition.Name != "Add")
				{
					break;
				}

				if (methodInvocation.Arguments.Count == 0)
				{
					break;
				}
				else if (methodInvocation.Arguments.Count == 1)
				{
					addedExpressions.Add(methodInvocation.Arguments[0].Clone());
				}
				else
				{
					ExpressionCollection currentArguments = new ExpressionCollection(
						methodInvocation.Arguments.Select(x => x.Clone()));

					BlockExpression blockExpression = new BlockExpression(currentArguments, null);
					addedExpressions.Add(blockExpression);
				}
			}

			if (addedExpressions.Count == 0)
			{
				return false;
			}

			if (objectCreation.Initializer == null)
			{
				var initializer = new InitializerExpression(addedExpressions, InitializerType.CollectionInitializer);
				initializer.IsMultiLine = true;
				objectCreation.Initializer = initializer;
			}
			else
			{
				foreach (var item in addedExpressions)
				{
					objectCreation.Initializer.Expressions.Add(item);
				}
			}

			result = statements[startIndex];
			replacedStatementsCount = addedExpressions.Count + 1;
			return true;
		}
	}
}
