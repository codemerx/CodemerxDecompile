using System;
using Telerik.JustDecompiler.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    class AssignmentFlowNode
    {
        public InstructionBlock CFGBlock { get; private set; }
        public IList<AssignmentFlowNode> Successors { get; private set; }
        public AssignmentNodeState NodeState { get; set; }

        public AssignmentFlowNode(InstructionBlock theBlock)
        {
            this.CFGBlock = theBlock;
            this.NodeState = AssignmentNodeState.Unknown;
            this.Successors = new AssignmentFlowNode[theBlock.Successors.Length];
        }
    }
}
