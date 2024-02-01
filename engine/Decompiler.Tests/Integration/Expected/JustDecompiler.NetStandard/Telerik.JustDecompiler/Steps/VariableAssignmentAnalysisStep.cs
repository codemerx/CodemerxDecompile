using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class VariableAssignmentAnalysisStep : BaseAssignmentAnalysisStep
	{
		public VariableAssignmentAnalysisStep()
		{
		}

		protected override void AnalyzeAssignments()
		{
			foreach (VariableDefinition variable in this.context.MethodContext.Variables)
			{
				AssignmentType assignmentType = base.AnalyzeAssignmentType(new VariableUsageFinder(variable));
				this.context.MethodContext.VariableAssignmentData.Add(variable, assignmentType);
			}
		}
	}
}