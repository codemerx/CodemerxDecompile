using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler
{
    interface IStateMachineData
    {
        Dictionary<FieldDefinition, AssignmentType> FieldAssignmentData { get; }
    }
}
