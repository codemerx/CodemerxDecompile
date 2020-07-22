using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    abstract class BaseAssignmentAnalysisStep : IDecompilationStep
    {
        protected DecompilationContext context;
        private AssignmentFlowNode[] mappedNodes;

        public Ast.Statements.BlockStatement Process(DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            this.context = context;
            if (ShouldExecuteStep())
            {
                ProcessTheCFG();
                AnalyzeAssignments();
            }
            return body;
        }

        protected virtual bool ShouldExecuteStep()
        {
            return true;
        }

        private void ProcessTheCFG()
        {
            this.mappedNodes = new AssignmentFlowNode[this.context.MethodContext.ControlFlowGraph.Blocks.Length];
            for(int i = 0; i < mappedNodes.Length; i++)
            {
                this.mappedNodes[i] = new AssignmentFlowNode(this.context.MethodContext.ControlFlowGraph.Blocks[i]);
            }

            foreach (AssignmentFlowNode node in mappedNodes)
            {
                for (int i = 0; i < node.Successors.Count; i++)
                {
                    node.Successors[i] = mappedNodes[node.CFGBlock.Successors[i].Index];
                }
            }
        }

        protected abstract void AnalyzeAssignments();

        private void PrepareNodes()
        {
            foreach (AssignmentFlowNode node in mappedNodes)
            {
                node.NodeState = AssignmentNodeState.Unknown;
            }
        }

        protected AssignmentType AnalyzeAssignmentType(BaseUsageFinder usageFinder)
        {
            PrepareNodes();

            AssignmentAnalyzer analyzer = new AssignmentAnalyzer(usageFinder, this.context.MethodContext.Expressions);

            AssignmentType regularControlFlowResult = analyzer.CheckAssignmentType(mappedNodes[0]);
            if (regularControlFlowResult == AssignmentType.NotAssigned)
            {
                return AssignmentType.NotAssigned;
            }

            AssignmentType result = regularControlFlowResult;

            List<ExceptionHandler> exceptionHandlers = new List<ExceptionHandler>(this.context.MethodContext.ControlFlowGraph.RawExceptionHandlers);
            int initialCount;
            do
            {
                initialCount = exceptionHandlers.Count;
                for (int i = 0; i < exceptionHandlers.Count; i++)
                {
                    ExceptionHandler handler = exceptionHandlers[i];

                    AssignmentFlowNode tryEntryNode = GetNodeFromBlockOffset(handler.TryStart.Offset);
                    if (tryEntryNode.NodeState == AssignmentNodeState.Unknown)
                    {
                        continue;
                    }

                    exceptionHandlers.RemoveAt(i--);

                    CheckHandler(GetNodeFromBlockOffset(handler.HandlerStart.Offset), analyzer, ref result);
                    if (result == AssignmentType.NotAssigned)
                    {
                        return AssignmentType.NotAssigned;
                    }

                    if (handler.HandlerType != ExceptionHandlerType.Filter)
                    {
                        continue;
                    }

                    CheckHandler(GetNodeFromBlockOffset(handler.FilterStart.Offset), analyzer, ref result);
                    if (result == AssignmentType.NotAssigned)
                    {
                        return AssignmentType.NotAssigned;
                    }
                }
            } while (initialCount != exceptionHandlers.Count);

            return result;
        }

        private void CheckHandler(AssignmentFlowNode handlerEntry, AssignmentAnalyzer analyzer, ref AssignmentType result)
        {
            AssignmentType handlerResult = analyzer.CheckAssignmentType(handlerEntry);
            switch (handlerResult)
            {
                case AssignmentType.SingleAssignment:
                    if (result != AssignmentType.MultipleAssignments)
                    {
                        result = handlerResult;
                    }
                    break;
                case AssignmentType.NotAssigned:
                case AssignmentType.MultipleAssignments:
                    result = handlerResult;
                    break;
            }
        }

        private AssignmentFlowNode GetNodeFromBlockOffset(int offset)
        {
            return mappedNodes[this.context.MethodContext.ControlFlowGraph.InstructionToBlockMapping[offset].Index];
        }
    }
}
