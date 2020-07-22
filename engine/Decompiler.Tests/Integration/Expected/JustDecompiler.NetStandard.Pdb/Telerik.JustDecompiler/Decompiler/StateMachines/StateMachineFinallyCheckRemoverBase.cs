using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal abstract class StateMachineFinallyCheckRemoverBase
	{
		private readonly HashSet<InstructionBlock> toBeRemoved;

		protected readonly ControlFlowGraph theCFG;

		protected readonly Collection<VariableDefinition> methodVariables;

		public HashSet<InstructionBlock> BlocksMarkedForRemoval
		{
			get
			{
				return this.toBeRemoved;
			}
		}

		public StateMachineFinallyCheckRemoverBase(MethodSpecificContext methodContext)
		{
			this.toBeRemoved = new HashSet<InstructionBlock>();
			base();
			this.methodVariables = methodContext.get_Body().get_Variables();
			this.theCFG = methodContext.get_ControlFlowGraph();
			return;
		}

		protected abstract bool IsFinallyCheckBlock(InstructionBlock finallyEntry);

		public abstract void MarkFinallyConditionsForRemoval(VariableReference checkVariable);

		protected void MarkFinallyConditionsForRemovalInternal()
		{
			V_0 = this.theCFG.get_RawExceptionHandlers().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_HandlerType() != 2 || !this.theCFG.get_InstructionToBlockMapping().TryGetValue(V_1.get_HandlerStart().get_Offset(), out V_2) || !this.IsFinallyCheckBlock(V_2))
					{
						continue;
					}
					dummyVar0 = this.toBeRemoved.Add(V_2);
					V_2.set_Successors(new InstructionBlock[0]);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}
	}
}