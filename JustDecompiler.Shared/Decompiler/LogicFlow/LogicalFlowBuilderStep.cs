using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.FollowNodes;
using Telerik.JustDecompiler.Steps;
using Mono.Cecil;


namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
    /// <summary>
    /// Creates the basic high-level control flow constructs, by analizing the control flow graph.
    /// </summary>
	class LogicalFlowBuilderStep : IDecompilationStep
	{
		BlockLogicalConstruct theBlockLogicalConstruct;
		LogicalFlowBuilderContext logicalBuilderContext;
		MethodSpecificContext methodContext;

        YieldGuardedBlocksBuilder yieldGuardedBlocksBuilder;
		GuardedBlocksBuilder guardedBlocksBuilder;
        ConditionBuilder conditionBuilder;
		LoopBuilder loopBuilder;
		SwitchBuilder switchBuilder;
        IfBuilder ifBuilder;
        CFGBlockSplitter cfgBlockSplitter;
        FollowNodeDeterminator followNodeDeterminator;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
            TypeSystem typeSystem = context.MethodContext.Method.Module.TypeSystem;

			logicalBuilderContext = new LogicalFlowBuilderContext(context.MethodContext.ControlFlowGraph);
            cfgBlockSplitter = new CFGBlockSplitter(logicalBuilderContext);
			conditionBuilder = new ConditionBuilder(logicalBuilderContext, typeSystem);
			loopBuilder = new LoopBuilder(logicalBuilderContext, typeSystem);
			switchBuilder = new SwitchBuilder(logicalBuilderContext);
			ifBuilder = new IfBuilder(logicalBuilderContext, methodContext.Method.Module.TypeSystem);
            followNodeDeterminator = new FollowNodeDeterminator(typeSystem);
            yieldGuardedBlocksBuilder = new YieldGuardedBlocksBuilder(logicalBuilderContext, context);

            GetMaxIndexOfBlock();
			InitializeTheBlock();
            
			guardedBlocksBuilder = new GuardedBlocksBuilder(logicalBuilderContext);

			context.MethodContext.LogicalConstructsTree = BuildLogicalConstructTree();
			context.MethodContext.LogicalConstructsContext = logicalBuilderContext;

			return body;
		}

        /// <summary>
        /// Finds the maximum index of a block in the control flow graph.
        /// </summary>
        private void GetMaxIndexOfBlock()
        {
            int entryBlockIndex = logicalBuilderContext.CFG.Blocks[0].Index;
            int lastBlockIndex = logicalBuilderContext.CFG.Blocks[logicalBuilderContext.CFG.Blocks.Length - 1].Index;
            //The RemoveYieldStateMachineStep might have changed the order of the blocks, so the entry might be the block with the max index.
            logicalBuilderContext.MaxBlockIndex = entryBlockIndex > lastBlockIndex ? entryBlockIndex : lastBlockIndex;
        }

        /// <summary>
        /// Initializes the block logical construct representing the method body.
        /// </summary>
		private void InitializeTheBlock()
		{
            MapBlocks();

            CFGBlockLogicalConstruct entry = logicalBuilderContext.CFGBlockToLogicalConstructMap[logicalBuilderContext.CFG.Blocks[0]][0];

            HashSet<ILogicalConstruct> blockContents = new HashSet<ILogicalConstruct>();
            foreach (CFGBlockLogicalConstruct[] cfgBlocks in logicalBuilderContext.CFGBlockToLogicalConstructMap.Values)
            {
                blockContents.UnionWith(cfgBlocks);
            }

			theBlockLogicalConstruct = new BlockLogicalConstruct(entry, blockContents);

		}


        /// <summary>
        /// Maps each CFG instruction blocks to a new CFGBlockLogicalConstruct, creating the relations between the new constructs and updating the logical builder
        /// context.
        /// </summary>
		private void MapBlocks()
		{
            //The new CFG logical constructs hold the block that they represent, and all the expressions that are for this block.
            foreach (InstructionBlock instructionBlock in logicalBuilderContext.CFG.Blocks)
            {
                int offset = instructionBlock.First.Offset;
                logicalBuilderContext.CFGBlockToLogicalConstructMap.Add(instructionBlock,
                    new CFGBlockLogicalConstruct[] { new CFGBlockLogicalConstruct(instructionBlock, methodContext.Expressions.BlockExpressions[offset]) });
            }

            //After creating all the blocks, we can add the relations between them.
            foreach (InstructionBlock instructionBlock in logicalBuilderContext.CFG.Blocks)
            {
                CFGBlockLogicalConstruct logicalConstruct = logicalBuilderContext.CFGBlockToLogicalConstructMap[instructionBlock][0];

                foreach (InstructionBlock successorBlock in instructionBlock.Successors)
                {
                    CFGBlockLogicalConstruct successorConstruct = logicalBuilderContext.CFGBlockToLogicalConstructMap[successorBlock][0];
                    logicalConstruct.AddToSuccessors(successorConstruct);
                    successorConstruct.AddToPredecessors(logicalConstruct);
                }
            }
		}

        /// <summary>
        /// Builds the various control flow constructs.
        /// </summary>
        /// <returns></returns>
		private BlockLogicalConstruct BuildLogicalConstructTree()
		{
            cfgBlockSplitter.SplitConditionalCFGBlocks();
            guardedBlocksBuilder.FindExceptionHandlingConstructs();
            yieldGuardedBlocksBuilder.BuildGuardedBlocks(theBlockLogicalConstruct);
            conditionBuilder.BuildConstructs(theBlockLogicalConstruct);
            loopBuilder.BuildLoops(theBlockLogicalConstruct);

			switchBuilder.BuildConstructs();
            ifBuilder.BuildConstructs(theBlockLogicalConstruct);

            followNodeDeterminator.ProcessConstruct(theBlockLogicalConstruct);

			return theBlockLogicalConstruct;
		}
	}
}
