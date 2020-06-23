using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class DependsOnAnalysisStep : IDecompilationStep
	{
		public DependsOnAnalysisStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			DependsOnAnalysisVisitor dependsOnAnalysisVisitor = new DependsOnAnalysisVisitor(context.MethodContext.AnalysisResults.TypesDependingOn, context.MethodContext.AnalysisResults.AmbiguousCastsToObject);
			dependsOnAnalysisVisitor.Visit(body);
			if (context.TypeContext.AssignmentData != null)
			{
				foreach (InitializationAssignment value in context.TypeContext.AssignmentData.Values)
				{
					dependsOnAnalysisVisitor.Visit(value.AssignmentExpression);
				}
			}
			return body;
		}
	}
}