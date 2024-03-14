using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.FollowNodes;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class LogicalFlowBuilderStep : IDecompilationStep
	{
		private BlockLogicalConstruct theBlockLogicalConstruct;

		private LogicalFlowBuilderContext logicalBuilderContext;

		private MethodSpecificContext methodContext;

		private YieldGuardedBlocksBuilder yieldGuardedBlocksBuilder;

		private GuardedBlocksBuilder guardedBlocksBuilder;

		private ConditionBuilder conditionBuilder;

		private LoopBuilder loopBuilder;

		private SwitchBuilder switchBuilder;

		private IfBuilder ifBuilder;

		private CFGBlockSplitter cfgBlockSplitter;

		private FollowNodeDeterminator followNodeDeterminator;

		public LogicalFlowBuilderStep()
		{
		}

		private BlockLogicalConstruct BuildLogicalConstructTree()
		{
			this.cfgBlockSplitter.SplitConditionalCFGBlocks();
			this.guardedBlocksBuilder.FindExceptionHandlingConstructs();
			this.yieldGuardedBlocksBuilder.BuildGuardedBlocks(this.theBlockLogicalConstruct);
			this.conditionBuilder.BuildConstructs(this.theBlockLogicalConstruct);
			this.loopBuilder.BuildLoops(this.theBlockLogicalConstruct);
			this.switchBuilder.BuildConstructs();
			this.ifBuilder.BuildConstructs(this.theBlockLogicalConstruct);
			this.followNodeDeterminator.ProcessConstruct(this.theBlockLogicalConstruct);
			return this.theBlockLogicalConstruct;
		}

		private void GetMaxIndexOfBlock()
		{
			int index = this.logicalBuilderContext.CFG.Blocks[0].Index;
			int num = this.logicalBuilderContext.CFG.Blocks[(int)this.logicalBuilderContext.CFG.Blocks.Length - 1].Index;
			this.logicalBuilderContext.MaxBlockIndex = (index > num ? index : num);
		}

		private void InitializeTheBlock()
		{
			this.MapBlocks();
			CFGBlockLogicalConstruct item = this.logicalBuilderContext.CFGBlockToLogicalConstructMap[this.logicalBuilderContext.CFG.Blocks[0]][0];
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			foreach (CFGBlockLogicalConstruct[] value in this.logicalBuilderContext.CFGBlockToLogicalConstructMap.Values)
			{
				logicalConstructs.UnionWith(value);
			}
			this.theBlockLogicalConstruct = new BlockLogicalConstruct(item, logicalConstructs);
		}

		private void MapBlocks()
		{
			int i;
			InstructionBlock[] blocks = this.logicalBuilderContext.CFG.Blocks;
			for (i = 0; i < (int)blocks.Length; i++)
			{
				InstructionBlock instructionBlocks = blocks[i];
				int offset = instructionBlocks.First.get_Offset();
				this.logicalBuilderContext.CFGBlockToLogicalConstructMap.Add(instructionBlocks, new CFGBlockLogicalConstruct[] { new CFGBlockLogicalConstruct(instructionBlocks, this.methodContext.Expressions.BlockExpressions[offset]) });
			}
			blocks = this.logicalBuilderContext.CFG.Blocks;
			for (i = 0; i < (int)blocks.Length; i++)
			{
				InstructionBlock instructionBlocks1 = blocks[i];
				CFGBlockLogicalConstruct item = this.logicalBuilderContext.CFGBlockToLogicalConstructMap[instructionBlocks1][0];
				InstructionBlock[] successors = instructionBlocks1.Successors;
				for (int j = 0; j < (int)successors.Length; j++)
				{
					InstructionBlock instructionBlocks2 = successors[j];
					CFGBlockLogicalConstruct cFGBlockLogicalConstruct = this.logicalBuilderContext.CFGBlockToLogicalConstructMap[instructionBlocks2][0];
					item.AddToSuccessors(cFGBlockLogicalConstruct);
					cFGBlockLogicalConstruct.AddToPredecessors(item);
				}
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			TypeSystem typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			this.logicalBuilderContext = new LogicalFlowBuilderContext(context.MethodContext.ControlFlowGraph);
			this.cfgBlockSplitter = new CFGBlockSplitter(this.logicalBuilderContext);
			this.conditionBuilder = new ConditionBuilder(this.logicalBuilderContext, typeSystem);
			this.loopBuilder = new LoopBuilder(this.logicalBuilderContext, typeSystem);
			this.switchBuilder = new SwitchBuilder(this.logicalBuilderContext);
			this.ifBuilder = new IfBuilder(this.logicalBuilderContext, this.methodContext.Method.get_Module().get_TypeSystem());
			this.followNodeDeterminator = new FollowNodeDeterminator(typeSystem);
			this.yieldGuardedBlocksBuilder = new YieldGuardedBlocksBuilder(this.logicalBuilderContext, context);
			this.GetMaxIndexOfBlock();
			this.InitializeTheBlock();
			this.guardedBlocksBuilder = new GuardedBlocksBuilder(this.logicalBuilderContext);
			context.MethodContext.LogicalConstructsTree = this.BuildLogicalConstructTree();
			context.MethodContext.LogicalConstructsContext = this.logicalBuilderContext;
			return body;
		}
	}
}