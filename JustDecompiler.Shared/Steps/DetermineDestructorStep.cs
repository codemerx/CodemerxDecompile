using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class DetermineDestructorStep : IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			MethodDefinition method = context.MethodContext.Method;
			if (method.Name == "Finalize" && method.IsVirtual)
			{
				if (body.Statements.Count == 1 && body.Statements[0] is TryStatement)
				{
					TryStatement tryFinally = body.Statements[0] as TryStatement;

					if (tryFinally.Finally != null && tryFinally.Finally.Body.Statements.Count == 1 && tryFinally.Finally.Body.Statements[0] is ExpressionStatement)
					{
						ExpressionStatement finallyStatementExpressionStatement = tryFinally.Finally.Body.Statements[0] as ExpressionStatement;

						if (finallyStatementExpressionStatement.Expression is MethodInvocationExpression)
						{
							MethodDefinition baseDestructor = (finallyStatementExpressionStatement.Expression as MethodInvocationExpression).MethodExpression.MethodDefinition;

							if (baseDestructor != null)
							{
								if (baseDestructor.Name == "Finalize" && baseDestructor.DeclaringType.FullName == method.DeclaringType.BaseType.FullName)
								{
									context.MethodContext.IsDestructor = true;
									context.MethodContext.DestructorStatements = new BlockStatement() { Statements = tryFinally.Try.Statements };
								}
							}
						}
					}

				}
			}

			return body;
		}
	}
}
