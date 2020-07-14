using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal class AsyncMoveNextMethodAnalyzer
	{
		private ControlFlowGraph theCFG;

		private IList<VariableDefinition> methodVariables;

		private FieldDefinition stateField;

		private VariableReference stateVariable;

		private VariableReference doFinallyVariable;

		public Dictionary<VariableReference, FieldReference> variableToFieldMap = new Dictionary<VariableReference, FieldReference>();

		public VariableReference DoFinallyVariable
		{
			get
			{
				return this.doFinallyVariable;
			}
		}

		public AsyncStateMachineVersion StateMachineVersion
		{
			get;
			private set;
		}

		public VariableReference StateVariable
		{
			get
			{
				return this.stateVariable;
			}
		}

		public AsyncMoveNextMethodAnalyzer(MethodSpecificContext moveNextMethodContext, FieldDefinition stateField)
		{
			this.theCFG = moveNextMethodContext.ControlFlowGraph;
			this.methodVariables = moveNextMethodContext.Variables;
			this.stateField = stateField;
			if (this.GetDoFinallyVariable())
			{
				this.StateMachineVersion = AsyncStateMachineVersion.V1;
				return;
			}
			this.GetStateVariable();
			this.StateMachineVersion = AsyncStateMachineVersion.V2;
		}

		private bool GetDoFinallyVariable()
		{
			Instruction first = this.theCFG.Blocks[0].First;
			if (first.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			return StateMachineUtilities.TryGetVariableFromInstruction(first.get_Next(), this.methodVariables, out this.doFinallyVariable);
		}

		private void GetStateVariable()
		{
			VariableReference variableReference;
			Instruction first = this.theCFG.Blocks[0].First;
			if (first.get_OpCode().get_Code() != 2)
			{
				return;
			}
			first = first.get_Next();
			if (first.get_OpCode().get_Code() != 120)
			{
				return;
			}
			FieldReference operand = first.get_Operand() as FieldReference;
			if (operand == null || (object)operand.Resolve() != (object)this.stateField)
			{
				return;
			}
			first = first.get_Next();
			StateMachineUtilities.TryGetVariableFromInstruction(first, this.methodVariables, out this.stateVariable);
			if ((object)first == (object)this.theCFG.Blocks[0].Last)
			{
				return;
			}
			first = first.get_Next();
			bool flag = true;
			while (flag)
			{
				if (first.get_OpCode().get_Code() == 2)
				{
					first = first.get_Next();
					if (first.get_OpCode().get_Code() == 120)
					{
						FieldReference fieldReference = first.get_Operand() as FieldReference;
						if (fieldReference != null)
						{
							first = first.get_Next();
							if (StateMachineUtilities.TryGetVariableFromInstruction(first, this.methodVariables, out variableReference))
							{
								this.variableToFieldMap.Add(variableReference, fieldReference);
								if ((object)first == (object)this.theCFG.Blocks[0].Last)
								{
									break;
								}
								first = first.get_Next();
								continue;
							}
						}
					}
				}
				flag = false;
			}
		}
	}
}