using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class InstructionMappingsCheck : IDecompilationStep
	{
		public InstructionMappingsCheck()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			HashSet<Instruction> instructions = new HashSet<Instruction>(body.UnderlyingSameMethodInstructions);
			if (context.MethodContext.IsMethodBodyChanged)
			{
				context.MethodContext.Method.RefreshBody();
				context.MethodContext.IsMethodBodyChanged = false;
			}
			List<Instruction> instructions1 = new List<Instruction>();
			foreach (Instruction instruction in context.MethodContext.Method.Body.Instructions)
			{
				if (instructions.Contains(instruction))
				{
					continue;
				}
				instructions1.Add(instruction);
			}
			if (instructions1.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder("Found unmapped instructions.\n");
				foreach (Instruction instruction1 in instructions1)
				{
					stringBuilder.AppendLine(instruction1.ToString());
				}
				throw new Exception(stringBuilder.ToString());
			}
			return body;
		}
	}
}