using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.StateMachines;

namespace Telerik.JustDecompiler.Steps
{
	internal abstract class BaseStateMachineRemoverStep : IDecompilationStep
	{
		protected readonly HashSet<InstructionBlock> toBeRemoved = new HashSet<InstructionBlock>();

		protected MethodSpecificContext moveNextMethodContext;

		protected ControlFlowGraph theCFG;

		protected BaseStateMachineRemoverStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.moveNextMethodContext = context.MethodContext;
			this.theCFG = this.moveNextMethodContext.ControlFlowGraph;
			this.moveNextMethodContext.IsMethodBodyChanged = true;
			StateMachineUtilities.FixInstructionConnections(this.theCFG.Blocks);
			if (!this.ProcessCFG())
			{
				context.StopPipeline();
			}
			return body;
		}

		protected abstract bool ProcessCFG();
	}
}