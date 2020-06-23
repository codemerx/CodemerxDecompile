using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class YieldData : IStateMachineData
	{
		public YieldExceptionHandlerInfo[] ExceptionHandlers
		{
			get;
			private set;
		}

		public Dictionary<FieldDefinition, AssignmentType> FieldAssignmentData
		{
			get
			{
				return JustDecompileGenerated_get_FieldAssignmentData();
			}
			set
			{
				JustDecompileGenerated_set_FieldAssignmentData(value);
			}
		}

		private Dictionary<FieldDefinition, AssignmentType> JustDecompileGenerated_FieldAssignmentData_k__BackingField;

		public Dictionary<FieldDefinition, AssignmentType> JustDecompileGenerated_get_FieldAssignmentData()
		{
			return this.JustDecompileGenerated_FieldAssignmentData_k__BackingField;
		}

		public void JustDecompileGenerated_set_FieldAssignmentData(Dictionary<FieldDefinition, AssignmentType> value)
		{
			this.JustDecompileGenerated_FieldAssignmentData_k__BackingField = value;
		}

		public YieldFieldsInformation FieldsInfo
		{
			get;
			private set;
		}

		public YieldStateMachineVersion StateMachineVersion
		{
			get;
			private set;
		}

		public HashSet<InstructionBlock> YieldBreaks
		{
			get;
			private set;
		}

		public HashSet<InstructionBlock> YieldReturns
		{
			get;
			private set;
		}

		public YieldData(YieldStateMachineVersion stateMachineVersion, HashSet<InstructionBlock> yieldReturns, HashSet<InstructionBlock> yieldBreaks, YieldFieldsInformation fieldsInfo, List<YieldExceptionHandlerInfo> exceptionHandlers)
		{
			this.StateMachineVersion = stateMachineVersion;
			this.YieldReturns = yieldReturns;
			this.YieldBreaks = yieldBreaks;
			this.FieldsInfo = fieldsInfo;
			this.ExceptionHandlers = exceptionHandlers.ToArray();
			this.FieldAssignmentData = new Dictionary<FieldDefinition, AssignmentType>();
		}
	}
}