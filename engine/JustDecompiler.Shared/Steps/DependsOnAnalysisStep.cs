using System;
using System.Linq;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{
    /// <summary>
    /// This step finds all dependencies of the method. Also it finds all CastExpressions, that have unresolved references up the inheritance chain that we need to find out whether == or != is overloaded.
    /// </summary>
	public class DependsOnAnalysisStep : IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			DependsOnAnalysisVisitor visitor = new DependsOnAnalysisVisitor(context.MethodContext.AnalysisResults.TypesDependingOn, context.MethodContext.AnalysisResults.AmbiguousCastsToObject);
			visitor.Visit(body);
			if(context.TypeContext.AssignmentData != null)
			{
				foreach (InitializationAssignment fieldAsssignment in context.TypeContext.AssignmentData.Values)
				{
					visitor.Visit(fieldAsssignment.AssignmentExpression);
				}
			}
			return body;
		}
	}
}
