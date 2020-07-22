using Mono.Cecil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Decompiler
{
	internal interface IStateMachineData
	{
		Dictionary<FieldDefinition, AssignmentType> FieldAssignmentData
		{
			get;
		}
	}
}