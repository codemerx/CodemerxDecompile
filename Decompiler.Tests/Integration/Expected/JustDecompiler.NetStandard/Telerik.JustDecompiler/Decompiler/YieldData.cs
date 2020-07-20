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
				return get_FieldAssignmentData();
			}
			set
			{
				set_FieldAssignmentData(value);
			}
		}

		// <FieldAssignmentData>k__BackingField
		private Dictionary<FieldDefinition, AssignmentType> u003cFieldAssignmentDatau003ek__BackingField;

		public Dictionary<FieldDefinition, AssignmentType> get_FieldAssignmentData()
		{
			return this.u003cFieldAssignmentDatau003ek__BackingField;
		}

		public void set_FieldAssignmentData(Dictionary<FieldDefinition, AssignmentType> value)
		{
			this.u003cFieldAssignmentDatau003ek__BackingField = value;
			return;
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
			base();
			this.set_StateMachineVersion(stateMachineVersion);
			this.set_YieldReturns(yieldReturns);
			this.set_YieldBreaks(yieldBreaks);
			this.set_FieldsInfo(fieldsInfo);
			this.set_ExceptionHandlers(exceptionHandlers.ToArray());
			this.set_FieldAssignmentData(new Dictionary<FieldDefinition, AssignmentType>());
			return;
		}
	}
}