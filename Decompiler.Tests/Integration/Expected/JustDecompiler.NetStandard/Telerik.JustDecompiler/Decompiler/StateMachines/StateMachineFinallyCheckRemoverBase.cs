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
		private readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

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
			this.methodVariables = methodContext.Body.Variables;
			this.theCFG = methodContext.ControlFlowGraph;
		}

		protected abstract bool IsFinallyCheckBlock(InstructionBlock finallyEntry);

		public abstract void MarkFinallyConditionsForRemoval(VariableReference checkVariable);

		protected void MarkFinallyConditionsForRemovalInternal()
		{
			InstructionBlock instructionBlocks;
			foreach (ExceptionHandler rawExceptionHandler in this.theCFG.RawExceptionHandlers)
			{
				if (rawExceptionHandler.HandlerType != ExceptionHandlerType.Finally || !this.theCFG.InstructionToBlockMapping.TryGetValue(rawExceptionHandler.HandlerStart.Offset, out instructionBlocks) || !this.IsFinallyCheckBlock(instructionBlocks))
				{
					continue;
				}
				this.toBeRemoved.Add(instructionBlocks);
				instructionBlocks.Successors = new InstructionBlock[0];
			}
		}
	}
}