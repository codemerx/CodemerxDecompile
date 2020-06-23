using Mono.Cecil;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class DetermineDestructorStep : IDecompilationStep
	{
		public DetermineDestructorStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			MethodDefinition method = context.MethodContext.Method;
			if (method.Name == "Finalize" && method.IsVirtual && body.Statements.Count == 1 && body.Statements[0] is TryStatement)
			{
				TryStatement item = body.Statements[0] as TryStatement;
				if (item.Finally != null && item.Finally.Body.Statements.Count == 1 && item.Finally.Body.Statements[0] is ExpressionStatement)
				{
					ExpressionStatement expressionStatement = item.Finally.Body.Statements[0] as ExpressionStatement;
					if (expressionStatement.Expression is MethodInvocationExpression)
					{
						MethodDefinition methodDefinition = (expressionStatement.Expression as MethodInvocationExpression).MethodExpression.MethodDefinition;
						if (methodDefinition != null && methodDefinition.Name == "Finalize" && methodDefinition.DeclaringType.FullName == method.DeclaringType.BaseType.FullName)
						{
							context.MethodContext.IsDestructor = true;
							context.MethodContext.DestructorStatements = new BlockStatement()
							{
								Statements = item.Try.Statements
							};
						}
					}
				}
			}
			return body;
		}
	}
}