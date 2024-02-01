using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class SentinelStep : BaseCodeVisitor, IDecompilationStep
	{
		private readonly HashSet<ICodeNode> visitedNodes = new HashSet<ICodeNode>();

		private readonly string previousStep;

		public SentinelStep(string previousStep)
		{
			this.previousStep = previousStep;
		}

		private List<Instruction> GetNonUniqueInstructions(IList<Instruction> instructions)
		{
			List<Instruction> instructions1 = new List<Instruction>();
			for (int i = 1; i < instructions.Count; i++)
			{
				if ((object)instructions[i] == (object)instructions[i - 1])
				{
					instructions1.Add(instructions[i]);
				}
			}
			return instructions1;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			if (this.GetNonUniqueInstructions(new List<Instruction>(body.UnderlyingSameMethodInstructions)).Count > 0)
			{
				throw new Exception(String.Concat("Instruction duplication detected after: ", this.previousStep));
			}
			return body;
		}

		public override void Visit(ICodeNode node)
		{
			IPdbCodeNode pdbCodeNode = node as IPdbCodeNode;
			if (pdbCodeNode != null)
			{
				foreach (Instruction underlyingSameMethodInstruction in pdbCodeNode.UnderlyingSameMethodInstructions)
				{
					if ((object)underlyingSameMethodInstruction.get_ContainingMethod() == (object)pdbCodeNode.UnderlyingInstructionsMember)
					{
						continue;
					}
					throw new Exception(String.Concat("IPdbCodeNode contains instructions from different methods. After: ", this.previousStep));
				}
			}
			if (node != null && !this.visitedNodes.Add(node))
			{
				throw new Exception(String.Concat("Node duplication detected after: ", this.previousStep));
			}
			base.Visit(node);
		}
	}
}