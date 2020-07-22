using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class DependsOnAnalysisStep : IDecompilationStep
	{
		public DependsOnAnalysisStep()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = new DependsOnAnalysisVisitor(context.get_MethodContext().get_AnalysisResults().get_TypesDependingOn(), context.get_MethodContext().get_AnalysisResults().get_AmbiguousCastsToObject());
			V_0.Visit(body);
			if (context.get_TypeContext().get_AssignmentData() != null)
			{
				V_1 = context.get_TypeContext().get_AssignmentData().get_Values().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						V_0.Visit(V_2.get_AssignmentExpression());
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			}
			return body;
		}
	}
}