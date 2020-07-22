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
		private readonly HashSet<ICodeNode> visitedNodes;

		private readonly string previousStep;

		public SentinelStep(string previousStep)
		{
			this.visitedNodes = new HashSet<ICodeNode>();
			base();
			this.previousStep = previousStep;
			return;
		}

		private List<Instruction> GetNonUniqueInstructions(IList<Instruction> instructions)
		{
			V_0 = new List<Instruction>();
			V_1 = 1;
			while (V_1 < instructions.get_Count())
			{
				if ((object)instructions.get_Item(V_1) == (object)instructions.get_Item(V_1 - 1))
				{
					V_0.Add(instructions.get_Item(V_1));
				}
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			if (this.GetNonUniqueInstructions(new List<Instruction>(body.get_UnderlyingSameMethodInstructions())).get_Count() > 0)
			{
				throw new Exception(String.Concat("Instruction duplication detected after: ", this.previousStep));
			}
			return body;
		}

		public override void Visit(ICodeNode node)
		{
			V_0 = node as IPdbCodeNode;
			if (V_0 != null)
			{
				V_1 = V_0.get_UnderlyingSameMethodInstructions().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						if ((object)V_1.get_Current().get_ContainingMethod() == (object)V_0.get_UnderlyingInstructionsMember())
						{
							continue;
						}
						throw new Exception(String.Concat("IPdbCodeNode contains instructions from different methods. After: ", this.previousStep));
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			if (node != null && !this.visitedNodes.Add(node))
			{
				throw new Exception(String.Concat("Node duplication detected after: ", this.previousStep));
			}
			this.Visit(node);
			return;
		}
	}
}