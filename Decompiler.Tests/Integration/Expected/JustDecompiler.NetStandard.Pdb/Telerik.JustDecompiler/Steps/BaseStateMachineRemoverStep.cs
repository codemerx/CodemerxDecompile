using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal abstract class BaseStateMachineRemoverStep : IDecompilationStep
	{
		protected readonly HashSet<InstructionBlock> toBeRemoved;

		protected MethodSpecificContext moveNextMethodContext;

		protected ControlFlowGraph theCFG;

		protected BaseStateMachineRemoverStep()
		{
			this.toBeRemoved = new HashSet<InstructionBlock>();
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.moveNextMethodContext = context.get_MethodContext();
			this.theCFG = this.moveNextMethodContext.get_ControlFlowGraph();
			this.moveNextMethodContext.set_IsMethodBodyChanged(true);
			StateMachineUtilities.FixInstructionConnections(this.theCFG.get_Blocks());
			if (!this.ProcessCFG())
			{
				context.StopPipeline();
			}
			return body;
		}

		protected abstract bool ProcessCFG();
	}
}