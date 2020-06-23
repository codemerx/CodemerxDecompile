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
			this.CFGBlock = theBlock;
			this.NodeState = AssignmentNodeState.Unknown;
			this.Successors = new AssignmentFlowNode[(int)theBlock.Successors.Length];
		}
	}
}