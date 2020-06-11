using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class SentinelStep : BaseCodeVisitor, IDecompilationStep
    {
        private readonly HashSet<ICodeNode> visitedNodes = new HashSet<ICodeNode>();
        private readonly string previousStep;

        public SentinelStep(string previousStep)
        {
            this.previousStep = previousStep;
        }

        public Ast.Statements.BlockStatement Process(Decompiler.DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            Visit(body);
            List<Instruction> instructions = new List<Instruction>(body.UnderlyingSameMethodInstructions);
            List<Instruction> nonUniques = GetNonUniqueInstructions(instructions);
            if (nonUniques.Count > 0)
            {
                throw new Exception("Instruction duplication detected after: " + previousStep);
            }
            return body;
        }

        private List<Instruction> GetNonUniqueInstructions(IList<Instruction> instructions)
        {
            List<Instruction> result = new List<Instruction>();
            for (int i = 1; i < instructions.Count; i++)
            {
                if (instructions[i] == instructions[i - 1])
                {
                    result.Add(instructions[i]);
                }
            }

            return result;
        }

        public override void Visit(ICodeNode node)
        {
            IPdbCodeNode pdbNode = node as IPdbCodeNode;
            if(pdbNode != null)
            {
                foreach (Instruction instruction in pdbNode.UnderlyingSameMethodInstructions)
                {
                    if (instruction.ContainingMethod != pdbNode.UnderlyingInstructionsMember)
                    {
                        throw new Exception("IPdbCodeNode contains instructions from different methods. After: " + previousStep);
                    }
                }
            }

            if (node != null && !visitedNodes.Add(node))
            {
                throw new Exception("Node duplication detected after: " + previousStep);
            }
            base.Visit(node);
        }
    }
}
