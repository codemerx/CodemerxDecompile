using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class FieldAssignmentAnalysisStep : BaseAssignmentAnalysisStep
	{
		private readonly Func<DecompilationContext, IStateMachineData> dataSelector;

		public FieldAssignmentAnalysisStep(Func<DecompilationContext, IStateMachineData> dataSelector)
		{
			base();
			this.dataSelector = dataSelector;
			return;
		}

		protected override void AnalyzeAssignments()
		{
			V_0 = this.dataSelector.Invoke(this.context).get_FieldAssignmentData();
			V_1 = this.context.get_TypeContext().get_CurrentType().get_Fields().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(V_2, this.AnalyzeAssignmentType(new FieldUsageFinder(V_2)));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return;
		}
	}
}