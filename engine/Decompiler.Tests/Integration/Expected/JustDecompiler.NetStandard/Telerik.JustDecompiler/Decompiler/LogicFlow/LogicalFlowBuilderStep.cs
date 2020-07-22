using Mono.Cecil;
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
			base();
			return;
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
			V_0 = this.logicalBuilderContext.get_CFG().get_Blocks()[0].get_Index();
			V_1 = this.logicalBuilderContext.get_CFG().get_Blocks()[(int)this.logicalBuilderContext.get_CFG().get_Blocks().Length - 1].get_Index();
			stackVariable22 = this.logicalBuilderContext;
			if (V_0 > V_1)
			{
				stackVariable25 = V_0;
			}
			else
			{
				stackVariable25 = V_1;
			}
			stackVariable22.set_MaxBlockIndex(stackVariable25);
			return;
		}

		private void InitializeTheBlock()
		{
			this.MapBlocks();
			V_0 = this.logicalBuilderContext.get_CFGBlockToLogicalConstructMap().get_Item(this.logicalBuilderContext.get_CFG().get_Blocks()[0])[0];
			V_1 = new HashSet<ILogicalConstruct>();
			V_2 = this.logicalBuilderContext.get_CFGBlockToLogicalConstructMap().get_Values().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_1.UnionWith(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			this.theBlockLogicalConstruct = new BlockLogicalConstruct(V_0, V_1);
			return;
		}

		private void MapBlocks()
		{
			V_0 = this.logicalBuilderContext.get_CFG().get_Blocks();
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_2 = V_0[V_1];
				V_3 = V_2.get_First().get_Offset();
				stackVariable17 = this.logicalBuilderContext.get_CFGBlockToLogicalConstructMap();
				stackVariable20 = new CFGBlockLogicalConstruct[1];
				stackVariable20[0] = new CFGBlockLogicalConstruct(V_2, this.methodContext.get_Expressions().get_BlockExpressions().get_Item(V_3));
				stackVariable17.Add(V_2, stackVariable20);
				V_1 = V_1 + 1;
			}
			V_0 = this.logicalBuilderContext.get_CFG().get_Blocks();
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				V_4 = V_0[V_1];
				V_5 = this.logicalBuilderContext.get_CFGBlockToLogicalConstructMap().get_Item(V_4)[0];
				V_6 = V_4.get_Successors();
				V_7 = 0;
				while (V_7 < (int)V_6.Length)
				{
					V_8 = V_6[V_7];
					V_9 = this.logicalBuilderContext.get_CFGBlockToLogicalConstructMap().get_Item(V_8)[0];
					V_5.AddToSuccessors(V_9);
					V_9.AddToPredecessors(V_5);
					V_7 = V_7 + 1;
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			V_0 = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.logicalBuilderContext = new LogicalFlowBuilderContext(context.get_MethodContext().get_ControlFlowGraph());
			this.cfgBlockSplitter = new CFGBlockSplitter(this.logicalBuilderContext);
			this.conditionBuilder = new ConditionBuilder(this.logicalBuilderContext, V_0);
			this.loopBuilder = new LoopBuilder(this.logicalBuilderContext, V_0);
			this.switchBuilder = new SwitchBuilder(this.logicalBuilderContext);
			this.ifBuilder = new IfBuilder(this.logicalBuilderContext, this.methodContext.get_Method().get_Module().get_TypeSystem());
			this.followNodeDeterminator = new FollowNodeDeterminator(V_0);
			this.yieldGuardedBlocksBuilder = new YieldGuardedBlocksBuilder(this.logicalBuilderContext, context);
			this.GetMaxIndexOfBlock();
			this.InitializeTheBlock();
			this.guardedBlocksBuilder = new GuardedBlocksBuilder(this.logicalBuilderContext);
			context.get_MethodContext().set_LogicalConstructsTree(this.BuildLogicalConstructTree());
			context.get_MethodContext().set_LogicalConstructsContext(this.logicalBuilderContext);
			return body;
		}
	}
}