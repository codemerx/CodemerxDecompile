using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
    class FieldAssignmentAnalysisStep : BaseAssignmentAnalysisStep
    {
        private readonly Func<DecompilationContext, IStateMachineData> dataSelector;

        public FieldAssignmentAnalysisStep(Func<DecompilationContext, IStateMachineData> dataSelector)
        {
            this.dataSelector = dataSelector;
        }

        protected override void AnalyzeAssignments()
        {
            Dictionary<FieldDefinition, AssignmentType> fieldToAssignmentTypeMap = dataSelector(this.context).FieldAssignmentData;
            foreach (FieldDefinition field in this.context.TypeContext.CurrentType.Fields)
            {
                AssignmentType analysisResult = AnalyzeAssignmentType(new FieldUsageFinder(field));
                fieldToAssignmentTypeMap.Add(field, analysisResult);
            }
        }
    }
}
