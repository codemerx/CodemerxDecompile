using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Telerik.JustDecompiler.Decompiler
{
	public struct YieldFieldsInformation
	{
		private readonly FieldDefinition stateHolderField;

		private readonly FieldDefinition currentItemField;

		private readonly VariableReference returnFlagVariable;

		public FieldReference CurrentItemField
		{
			get
			{
				return this.currentItemField;
			}
		}

		public VariableReference ReturnFlagVariable
		{
			get
			{
				return this.returnFlagVariable;
			}
		}

		public FieldReference StateHolderField
		{
			get
			{
				return this.stateHolderField;
			}
		}

		public YieldFieldsInformation(FieldDefinition stateHolderField, FieldDefinition currentItemField, VariableReference returnFlagVariable)
		{
			this.stateHolderField = stateHolderField;
			this.currentItemField = currentItemField;
			this.returnFlagVariable = returnFlagVariable;
			return;
		}
	}
}