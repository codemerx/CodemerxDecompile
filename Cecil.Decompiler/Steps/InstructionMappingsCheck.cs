using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class InstructionMappingsCheck : IDecompilationStep
    {
        public Ast.Statements.BlockStatement Process(Decompiler.DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            HashSet<Instruction> mappedInstructions = new HashSet<Instruction>(body.UnderlyingSameMethodInstructions);
            if (context.MethodContext.IsMethodBodyChanged)
            {
                context.MethodContext.Method.RefreshBody();
                context.MethodContext.IsMethodBodyChanged = false;
            }

            List<Instruction> unmappedInstructions = new List<Instruction>();
            foreach (Instruction instruction in context.MethodContext.Method.Body.Instructions)
            {
                if (!mappedInstructions.Contains(instruction))
                {
                    unmappedInstructions.Add(instruction);
                }
            }

            if (unmappedInstructions.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder("Found unmapped instructions.\n");
                foreach (Instruction unmappedInstruction in unmappedInstructions)
                {
                    stringBuilder.AppendLine(unmappedInstruction.ToString());
                }
                throw new Exception(stringBuilder.ToString());
            }

            return body;
        }
    }
}
