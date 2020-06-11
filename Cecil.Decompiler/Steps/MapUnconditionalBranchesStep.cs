using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
    class MapUnconditionalBranchesStep : BaseCodeVisitor, IDecompilationStep
    {
        private readonly HashSet<Instruction> mappedInstructions = new HashSet<Instruction>();
        private MethodSpecificContext methodContext;

        public Ast.Statements.BlockStatement Process(Decompiler.DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            this.methodContext = context.MethodContext;
            mappedInstructions.UnionWith(body.UnderlyingSameMethodInstructions);
            Visit(body);
            return body;
        }

        public override void Visit(ICodeNode node)
        {
            Expression expression = node as Expression;
            if (expression != null)
            {
                MapBranches(expression);
            }
            else
            {
                base.Visit(node);
            }
        }

        private void MapBranches(Expression expression)
        {
            List<Instruction> orderedInstructions = new List<Instruction>(expression.UnderlyingSameMethodInstructions);
            List<Instruction> branchesToAdd = new List<Instruction>();

            int count = orderedInstructions.Count;
            for (int i = 0; i < count; i++)
            {
                Instruction current = orderedInstructions[i];
                switch (current.OpCode.FlowControl)
                {
                    case FlowControl.Branch:
                    case FlowControl.Cond_Branch:
                    case FlowControl.Return:
                    case FlowControl.Throw:
                        continue;
                }

                Instruction next = current.Next;
                if(next != null && !mappedInstructions.Contains(next) && (i == orderedInstructions.Count - 1 || next != orderedInstructions[i + 1])  &&
                    IsUnconditionalBranch(next) && !methodContext.ControlFlowGraph.InstructionToBlockMapping.ContainsKey(next.Offset))
                {
                    branchesToAdd.Add(next);
                }
            }

            mappedInstructions.UnionWith(branchesToAdd);
            expression.MapBranchInstructions(branchesToAdd);
        }

        private bool IsUnconditionalBranch(Instruction instruction)
        {
            Code code = instruction.OpCode.Code;
            return code == Code.Br || code == Code.Br_S;
        }
    }
}
