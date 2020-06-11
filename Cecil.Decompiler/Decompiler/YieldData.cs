using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Decompiler
{
    internal class YieldData : IStateMachineData
    {
        /// <summary>
        /// Gets a set of the instruction blocks that contain yield breaks.
        /// </summary>
        public HashSet<InstructionBlock> YieldBreaks { get; private set; }

        /// <summary>
        /// Gets a set of the instruction blocks that contain yield returns.
        /// </summary>
        public HashSet<InstructionBlock> YieldReturns { get; private set; }

        /// <summary>
        /// Gets the structure that holds information about the fields used by the yield state machine.
        /// </summary>
        public YieldFieldsInformation FieldsInfo { get; private set; }

        /// <summary>
        /// Holds information about the exception handlers that are removed because of the state machine.
        /// </summary>
        public YieldExceptionHandlerInfo[] ExceptionHandlers { get; private set; }

        public YieldStateMachineVersion StateMachineVersion { get; private set; }

        public Dictionary<FieldDefinition, AssignmentType> FieldAssignmentData { get; set; }

        public YieldData(YieldStateMachineVersion stateMachineVersion, HashSet<InstructionBlock> yieldReturns, HashSet<InstructionBlock> yieldBreaks,
            YieldFieldsInformation fieldsInfo,
            List<YieldExceptionHandlerInfo> exceptionHandlers)
        {
            StateMachineVersion = stateMachineVersion;
            YieldReturns = yieldReturns;
            YieldBreaks = yieldBreaks;
            FieldsInfo = fieldsInfo;
            ExceptionHandlers = exceptionHandlers.ToArray();
            FieldAssignmentData = new Dictionary<FieldDefinition, AssignmentType>();
        }
    }
}
