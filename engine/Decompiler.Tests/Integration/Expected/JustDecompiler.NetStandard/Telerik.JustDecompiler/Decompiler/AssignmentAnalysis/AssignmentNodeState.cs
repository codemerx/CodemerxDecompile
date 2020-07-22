using System;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal enum AssignmentNodeState
	{
		Unknown,
		NotAssigned,
		ContainsAssignment,
		ContainsUsage
	}
}