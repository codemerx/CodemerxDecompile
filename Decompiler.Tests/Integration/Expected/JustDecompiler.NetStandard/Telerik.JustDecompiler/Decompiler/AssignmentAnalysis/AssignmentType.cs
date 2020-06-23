using System;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal enum AssignmentType
	{
		NotUsed,
		NotAssigned,
		SingleAssignment,
		MultipleAssignments
	}
}