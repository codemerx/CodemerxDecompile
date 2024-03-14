using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Decompiler.DefineUseAnalysis
{
    class StackUsageData
    {
        public Dictionary<int, VariableDefinition> InstructionOffsetToAssignedVariableMap { get; private set; }
        public Dictionary<int, List<VariableDefinition>> InstructionOffsetToUsedStackVariablesMap { get; private set; }
        public Dictionary<int, VariableDefinition> ExceptionHandlerStartToExceptionVariableMap { get; private set; }
        public Dictionary<VariableDefinition, StackVariableDefineUseInfo> VariableToDefineUseInfo { get; private set; }

        public StackUsageData()
        {
            InstructionOffsetToAssignedVariableMap = new Dictionary<int, VariableDefinition>();
            InstructionOffsetToUsedStackVariablesMap = new Dictionary<int, List<VariableDefinition>>();
            ExceptionHandlerStartToExceptionVariableMap = new Dictionary<int, VariableDefinition>();
            VariableToDefineUseInfo = new Dictionary<VariableDefinition, StackVariableDefineUseInfo>();
        }
    }
}
