using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Cil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class RemoveUnreachableBlocksStep : IDecompilationStep
    {
        private DecompilationContext decompilationContext;
        private ControlFlowGraph theCFG;
        private readonly Dictionary<InstructionBlock, ICollection<InstructionBlock>> guardedBlockToExceptionHandler =
            new Dictionary<InstructionBlock, ICollection<InstructionBlock>>();
        private readonly HashSet<InstructionBlock> reachableBlocks = new HashSet<InstructionBlock>();

        public Ast.Statements.BlockStatement Process(DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            this.decompilationContext = context;
            this.theCFG = context.MethodContext.ControlFlowGraph;
            ProcessTheControlFlowGraph();
            return body;
        }

        private void ProcessTheControlFlowGraph()
        {
            FindReachableBlocks();
            RemoveUnreachableBlocks();
        }

        private void FindReachableBlocks()
        {
            GetGuardedBlockToExceptionHandlersMap();

            GetReachableBlocks(new InstructionBlock[] { theCFG.Blocks[0] });


            while(true)
            {
                List<InstructionBlock> reachableGuardedBlocks = new List<InstructionBlock>();
                List<InstructionBlock> reachableExceptionHandlers = new List<InstructionBlock>();
                foreach (KeyValuePair<InstructionBlock, ICollection<InstructionBlock>> pair in guardedBlockToExceptionHandler)
                {
                    if(reachableBlocks.Contains(pair.Key))
                    {
                        reachableGuardedBlocks.Add(pair.Key);
                        reachableExceptionHandlers.AddRange(pair.Value);
                    }
                }

                if(reachableGuardedBlocks.Count == 0)
                {
                    return;
                }

                foreach (InstructionBlock guardedBlockEntry in reachableGuardedBlocks)
                {
                    guardedBlockToExceptionHandler.Remove(guardedBlockEntry);
                }

                GetReachableBlocks(reachableExceptionHandlers);
            }
        }

        void GetReachableBlocks(IEnumerable<InstructionBlock> startBlocks)
        {
            reachableBlocks.UnionWith(startBlocks);
            Queue<InstructionBlock> traversalQueue = new Queue<InstructionBlock>(startBlocks);

            while(traversalQueue.Count > 0)
            {
                InstructionBlock currentBlock = traversalQueue.Dequeue();

                foreach (InstructionBlock successor in currentBlock.Successors)
                {
                    if (reachableBlocks.Add(successor))
                    {
                        traversalQueue.Enqueue(successor);
                    }
                }
            }
        }

        void GetGuardedBlockToExceptionHandlersMap()
        {
            foreach (ExceptionHandler handler in theCFG.RawExceptionHandlers)
            {
                InstructionBlock guardedBlockEntry = theCFG.InstructionToBlockMapping[handler.TryStart.Offset];

                ICollection<InstructionBlock> exceptionHandlers;
                if(!guardedBlockToExceptionHandler.TryGetValue(guardedBlockEntry, out exceptionHandlers))
                {
                    exceptionHandlers = new List<InstructionBlock>();
                    guardedBlockToExceptionHandler[guardedBlockEntry] = exceptionHandlers;
                }

                InstructionBlock exceptionHandlerEntry = theCFG.InstructionToBlockMapping[handler.HandlerStart.Offset];
                exceptionHandlers.Add(exceptionHandlerEntry);

                if(handler.HandlerType == ExceptionHandlerType.Filter)
                {
                    InstructionBlock filterEntry = theCFG.InstructionToBlockMapping[handler.FilterStart.Offset];
                    exceptionHandlers.Add(filterEntry);
                }
            }
        }

        void RemoveUnreachableBlocks()
        {
            HashSet<InstructionBlock> unreachableBlocks = new HashSet<InstructionBlock>();
            foreach (InstructionBlock block in theCFG.Blocks)
            {
                if(!reachableBlocks.Contains(block))
                {
                    block.Successors = new InstructionBlock[0];
                    unreachableBlocks.Add(block);
                }
            }

            if(unreachableBlocks.Count > 0)
            {
                decompilationContext.MethodContext.IsMethodBodyChanged = true;
            }

            for(int i = 0; i < theCFG.RawExceptionHandlers.Count; i++)
            {
                ExceptionHandler handler = theCFG.RawExceptionHandlers[i];
                if(unreachableBlocks.Contains(theCFG.InstructionToBlockMapping[handler.TryStart.Offset]))
                {
                    theCFG.RawExceptionHandlers.RemoveAt(i--);
                }
            }

            foreach (InstructionBlock unreachableBlock in unreachableBlocks)
            {
                theCFG.RemoveBlockAt(unreachableBlock.Index);
            }
        }
    }
}
