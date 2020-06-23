using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class AsyncData : IStateMachineData
	{
		public HashSet<VariableReference> AwaiterVariables
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

		public FieldDefinition StateField
		{
			get;
			set;
		}

		public Dictionary<VariableReference, FieldReference> VariableToFieldMap
		{
			get;
			set;
		}

		public AsyncData(FieldDefinition stateField, HashSet<VariableReference> awaiterVariables, Dictionary<VariableReference, FieldReference> variableToFieldMap)
		{
			this.StateField = stateField;
			this.AwaiterVariables = awaiterVariables;
			this.VariableToFieldMap = variableToFieldMap;
			this.FieldAssignmentData = new Dictionary<FieldDefinition, AssignmentType>();
		}
	}
}