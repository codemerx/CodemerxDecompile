using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    public class LogicalFlowBuilderContext
    {
        public Dictionary<InstructionBlock, CFGBlockLogicalConstruct[]> CFGBlockToLogicalConstructMap { get; private set; }
		public ControlFlowGraph CFG { get; private set; }
		public Dictionary<int, InstructionBlock> InstructionToCFGBlockMapping { get; private set; }
        public int MaxBlockIndex { get; set; }
        public Dictionary<ILogicalConstruct, DominatorTree> LogicalConstructToDominatorTreeMap { get; private set; }

		public LogicalFlowBuilderContext(ControlFlowGraph cfg)
        {
            CFGBlockToLogicalConstructMap = new Dictionary<InstructionBlock, CFGBlockLogicalConstruct[]>();
			CFG = cfg;
			InstructionToCFGBlockMapping = cfg.InstructionToBlockMapping;
            LogicalConstructToDominatorTreeMap = new Dictionary<ILogicalConstruct, DominatorTree>();
        }
    }
}
