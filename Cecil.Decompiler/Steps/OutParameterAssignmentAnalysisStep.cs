using System;
using System.Collections.Generic;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{
    class OutParameterAssignmentAnalysisStep : BaseAssignmentAnalysisStep
    {
        private readonly List<ParameterDefinition> outParameters = new List<ParameterDefinition>();

        protected override bool ShouldExecuteStep()
        {
            foreach (ParameterDefinition parameter in this.context.MethodContext.Method.Parameters)
            {
                if (parameter.IsOutParameter())
                {
                    outParameters.Add(parameter);
                }
            }

            return outParameters.Count > 0;
        }

        protected override void AnalyzeAssignments()
        {
            foreach (ParameterDefinition parameter in outParameters)
            {
                OutParameterUsageFinder usageFinder = new OutParameterUsageFinder(parameter);
                AssignmentType result = AnalyzeAssignmentType(usageFinder);
                if (result == AssignmentType.NotAssigned || result == AssignmentType.NotUsed)
                {
                    this.context.MethodContext.OutParametersToAssign.Add(parameter);
                }
            }
        }
    }
}
