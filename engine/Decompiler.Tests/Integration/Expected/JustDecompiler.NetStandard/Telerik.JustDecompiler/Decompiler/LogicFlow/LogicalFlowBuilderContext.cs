using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.DTree;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	public class LogicalFlowBuilderContext
	{
		public ControlFlowGraph CFG
		{
			get;
			private set;
		}

		public Dictionary<InstructionBlock, CFGBlockLogicalConstruct[]> CFGBlockToLogicalConstructMap
		{
			get;
			private set;
		}

		public Dictionary<int, InstructionBlock> InstructionToCFGBlockMapping
		{
			get;
			private set;
		}

		public Dictionary<ILogicalConstruct, DominatorTree> LogicalConstructToDominatorTreeMap
		{
			get;
			private set;
		}

		public int MaxBlockIndex
		{
			get;
			set;
		}

		public LogicalFlowBuilderContext(ControlFlowGraph cfg)
		{
			this.CFGBlockToLogicalConstructMap = new Dictionary<InstructionBlock, CFGBlockLogicalConstruct[]>();
			this.CFG = cfg;
			this.InstructionToCFGBlockMapping = cfg.InstructionToBlockMapping;
			this.LogicalConstructToDominatorTreeMap = new Dictionary<ILogicalConstruct, DominatorTree>();
		}
	}
}