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
			base();
			this.set_StateField(stateField);
			this.set_AwaiterVariables(awaiterVariables);
			this.set_VariableToFieldMap(variableToFieldMap);
			this.set_FieldAssignmentData(new Dictionary<FieldDefinition, AssignmentType>());
			return;
		}
	}
}