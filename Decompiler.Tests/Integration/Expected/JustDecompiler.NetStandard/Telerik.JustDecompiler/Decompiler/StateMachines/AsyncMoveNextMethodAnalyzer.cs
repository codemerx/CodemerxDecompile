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
			if (first.OpCode.Code != Code.Ldc_I4_1)
			{
				return false;
			}
			return StateMachineUtilities.TryGetVariableFromInstruction(first.Next, this.methodVariables, out this.doFinallyVariable);
		}

		private void GetStateVariable()
		{
			VariableReference variableReference;
			Instruction first = this.theCFG.Blocks[0].First;
			if (first.OpCode.Code != Code.Ldarg_0)
			{
				return;
			}
			first = first.Next;
			if (first.OpCode.Code != Code.Ldfld)
			{
				return;
			}
			FieldReference operand = first.Operand as FieldReference;
			if (operand == null || operand.Resolve() != this.stateField)
			{
				return;
			}
			first = first.Next;
			StateMachineUtilities.TryGetVariableFromInstruction(first, this.methodVariables, out this.stateVariable);
			if (first == this.theCFG.Blocks[0].Last)
			{
				return;
			}
			first = first.Next;
			bool flag = true;
			while (flag)
			{
				if (first.OpCode.Code == Code.Ldarg_0)
				{
					first = first.Next;
					if (first.OpCode.Code == Code.Ldfld)
					{
						FieldReference fieldReference = first.Operand as FieldReference;
						if (fieldReference != null)
						{
							first = first.Next;
							if (StateMachineUtilities.TryGetVariableFromInstruction(first, this.methodVariables, out variableReference))
							{
								this.variableToFieldMap.Add(variableReference, fieldReference);
								if (first == this.theCFG.Blocks[0].Last)
								{
									break;
								}
								first = first.Next;
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