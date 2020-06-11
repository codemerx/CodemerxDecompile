using System;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
    class VariableAssignmentAnalysisStep : BaseAssignmentAnalysisStep
    {
        protected override void AnalyzeAssignments()
        {
            foreach (VariableDefinition variable in this.context.MethodContext.Variables)
            {
                AssignmentType analysisResult = AnalyzeAssignmentType(new VariableUsageFinder(variable));
                this.context.MethodContext.VariableAssignmentData.Add(variable, analysisResult);
            }
        }
    }
}
