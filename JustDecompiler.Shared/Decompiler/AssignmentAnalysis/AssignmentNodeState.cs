using System;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    enum AssignmentNodeState
    {
        Unknown,
        NotAssigned,
        ContainsAssignment,
        ContainsUsage
    }
}
