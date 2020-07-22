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
			base();
			this.set_InstructionOffsetToAssignedVariableMap(new Dictionary<int, VariableDefinition>());
			this.set_InstructionOffsetToUsedStackVariablesMap(new Dictionary<int, List<VariableDefinition>>());
			this.set_ExceptionHandlerStartToExceptionVariableMap(new Dictionary<int, VariableDefinition>());
			this.set_VariableToDefineUseInfo(new Dictionary<VariableDefinition, StackVariableDefineUseInfo>());
			return;
		}
	}
}