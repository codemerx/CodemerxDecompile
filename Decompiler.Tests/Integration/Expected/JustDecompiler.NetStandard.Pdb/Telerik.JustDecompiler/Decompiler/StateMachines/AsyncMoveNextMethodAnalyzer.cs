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

		public Dictionary<VariableReference, FieldReference> variableToFieldMap;

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
			this.variableToFieldMap = new Dictionary<VariableReference, FieldReference>();
			base();
			this.theCFG = moveNextMethodContext.get_ControlFlowGraph();
			this.methodVariables = moveNextMethodContext.get_Variables();
			this.stateField = stateField;
			if (this.GetDoFinallyVariable())
			{
				this.set_StateMachineVersion(0);
				return;
			}
			this.GetStateVariable();
			this.set_StateMachineVersion(1);
			return;
		}

		private bool GetDoFinallyVariable()
		{
			V_0 = this.theCFG.get_Blocks()[0].get_First();
			if (V_0.get_OpCode().get_Code() != 23)
			{
				return false;
			}
			return StateMachineUtilities.TryGetVariableFromInstruction(V_0.get_Next(), this.methodVariables, out this.doFinallyVariable);
		}

		private void GetStateVariable()
		{
			V_0 = this.theCFG.get_Blocks()[0].get_First();
			if (V_0.get_OpCode().get_Code() != 2)
			{
				return;
			}
			V_0 = V_0.get_Next();
			if (V_0.get_OpCode().get_Code() != 120)
			{
				return;
			}
			V_1 = V_0.get_Operand() as FieldReference;
			if (V_1 == null || (object)V_1.Resolve() != (object)this.stateField)
			{
				return;
			}
			V_0 = V_0.get_Next();
			dummyVar0 = StateMachineUtilities.TryGetVariableFromInstruction(V_0, this.methodVariables, out this.stateVariable);
			if ((object)V_0 == (object)this.theCFG.get_Blocks()[0].get_Last())
			{
				return;
			}
			V_0 = V_0.get_Next();
			V_2 = true;
			while (V_2)
			{
				if (V_0.get_OpCode().get_Code() == 2)
				{
					V_0 = V_0.get_Next();
					if (V_0.get_OpCode().get_Code() == 120)
					{
						V_4 = V_0.get_Operand() as FieldReference;
						if (V_4 != null)
						{
							V_0 = V_0.get_Next();
							if (StateMachineUtilities.TryGetVariableFromInstruction(V_0, this.methodVariables, out V_5))
							{
								this.variableToFieldMap.Add(V_5, V_4);
								if ((object)V_0 == (object)this.theCFG.get_Blocks()[0].get_Last())
								{
									break;
								}
								V_0 = V_0.get_Next();
								continue;
							}
						}
					}
				}
				V_2 = false;
			}
			return;
		}
	}
}