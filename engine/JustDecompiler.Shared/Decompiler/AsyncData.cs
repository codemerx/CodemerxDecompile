using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Decompiler
{
    class AsyncData : IStateMachineData
    {
        public HashSet<VariableReference> AwaiterVariables { get; private set; }
        public FieldDefinition StateField { get; set; }
        public Dictionary<VariableReference, FieldReference> VariableToFieldMap { get; set; }

        public Dictionary<FieldDefinition, AssignmentType> FieldAssignmentData { get; set; }

        public AsyncData(FieldDefinition stateField, HashSet<VariableReference> awaiterVariables, Dictionary<VariableReference, FieldReference> variableToFieldMap)
        {
            this.StateField = stateField;
            this.AwaiterVariables = awaiterVariables;
            this.VariableToFieldMap = variableToFieldMap;
            this.FieldAssignmentData = new Dictionary<FieldDefinition, AssignmentType>();
        }
    }
}
