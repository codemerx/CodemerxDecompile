using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class AssignmentFlowNode
	{
		public InstructionBlock CFGBlock
		{
			get;
			private set;
		}

		public AssignmentNodeState NodeState
		{
			get;
			set;
		}

		public IList<AssignmentFlowNode> Successors
		{
			get;
			private set;
		}

		public AssignmentFlowNode(InstructionBlock theBlock)
		{
			base();
			this.set_CFGBlock(theBlock);
			this.set_NodeState(0);
			this.set_Successors(new AssignmentFlowNode[(int)theBlock.get_Successors().Length]);
			return;
		}
	}
}