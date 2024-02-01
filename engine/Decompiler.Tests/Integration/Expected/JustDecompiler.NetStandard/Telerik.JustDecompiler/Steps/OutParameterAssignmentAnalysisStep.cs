using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class OutParameterAssignmentAnalysisStep : BaseAssignmentAnalysisStep
	{
		private readonly List<ParameterDefinition> outParameters = new List<ParameterDefinition>();

		public OutParameterAssignmentAnalysisStep()
		{
		}

		protected override void AnalyzeAssignments()
		{
			foreach (ParameterDefinition outParameter in this.outParameters)
			{
				AssignmentType assignmentType = base.AnalyzeAssignmentType(new OutParameterUsageFinder(outParameter));
				if (assignmentType != AssignmentType.NotAssigned && assignmentType != AssignmentType.NotUsed)
				{
					continue;
				}
				this.context.MethodContext.OutParametersToAssign.Add(outParameter);
			}
		}

		protected override bool ShouldExecuteStep()
		{
			foreach (ParameterDefinition parameter in this.context.MethodContext.Method.get_Parameters())
			{
				if (!parameter.IsOutParameter())
				{
					continue;
				}
				this.outParameters.Add(parameter);
			}
			return this.outParameters.Count > 0;
		}
	}
}