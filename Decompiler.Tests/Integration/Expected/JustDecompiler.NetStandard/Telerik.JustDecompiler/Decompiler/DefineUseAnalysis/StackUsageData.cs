using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler.DefineUseAnalysis
{
	internal class StackUsageData
	{
		public Dictionary<int, VariableDefinition> ExceptionHandlerStartToExceptionVariableMap
		{
			get;
			private set;
		}

		public Dictionary<int, VariableDefinition> InstructionOffsetToAssignedVariableMap
		{
			get;
			private set;
		}

		public Dictionary<int, List<VariableDefinition>> InstructionOffsetToUsedStackVariablesMap
		{
			get;
			private set;
		}

		public Dictionary<VariableDefinition, StackVariableDefineUseInfo> VariableToDefineUseInfo
		{
			get;
			private set;
		}

		public StackUsageData()
		{
			this.InstructionOffsetToAssignedVariableMap = new Dictionary<int, VariableDefinition>();
			this.InstructionOffsetToUsedStackVariablesMap = new Dictionary<int, List<VariableDefinition>>();
			this.ExceptionHandlerStartToExceptionVariableMap = new Dictionary<int, VariableDefinition>();
			this.VariableToDefineUseInfo = new Dictionary<VariableDefinition, StackVariableDefineUseInfo>();
		}
	}
}