using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class OutParameterAssignmentAnalysisStep : BaseAssignmentAnalysisStep
	{
		private readonly List<ParameterDefinition> outParameters;

		public OutParameterAssignmentAnalysisStep()
		{
			this.outParameters = new List<ParameterDefinition>();
			base();
			return;
		}

		protected override void AnalyzeAssignments()
		{
			V_0 = this.outParameters.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_3 = this.AnalyzeAssignmentType(new OutParameterUsageFinder(V_1));
					if (V_3 != 1 && V_3 != AssignmentType.NotUsed)
					{
						continue;
					}
					this.context.get_MethodContext().get_OutParametersToAssign().Add(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		protected override bool ShouldExecuteStep()
		{
			V_0 = this.context.get_MethodContext().get_Method().get_Parameters().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!V_1.IsOutParameter())
					{
						continue;
					}
					this.outParameters.Add(V_1);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return this.outParameters.get_Count() > 0;
		}
	}
}