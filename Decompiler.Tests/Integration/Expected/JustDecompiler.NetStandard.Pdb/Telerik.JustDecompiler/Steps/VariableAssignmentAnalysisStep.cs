using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class VariableAssignmentAnalysisStep : BaseAssignmentAnalysisStep
	{
		public VariableAssignmentAnalysisStep()
		{
			base();
			return;
		}

		protected override void AnalyzeAssignments()
		{
			V_0 = this.context.get_MethodContext().get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.AnalyzeAssignmentType(new VariableUsageFinder(V_1));
					this.context.get_MethodContext().get_VariableAssignmentData().Add(V_1, V_2);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}
	}
}