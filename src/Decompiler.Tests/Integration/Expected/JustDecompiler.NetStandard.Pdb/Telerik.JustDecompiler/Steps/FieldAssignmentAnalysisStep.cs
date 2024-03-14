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
			this.dataSelector = dataSelector;
		}

		protected override void AnalyzeAssignments()
		{
			Dictionary<FieldDefinition, AssignmentType> fieldAssignmentData = this.dataSelector(this.context).FieldAssignmentData;
			foreach (FieldDefinition field in this.context.TypeContext.CurrentType.get_Fields())
			{
				fieldAssignmentData.Add(field, base.AnalyzeAssignmentType(new FieldUsageFinder(field)));
			}
		}
	}
}