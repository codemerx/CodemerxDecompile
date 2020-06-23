using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public abstract class BasePdbStatement : Statement, IPdbCodeNode, ICodeNode
	{
		private MethodDefinition cachedInstructionsContainer;

		public MethodDefinition UnderlyingInstructionsMember
		{
			get
			{
				MethodDefinition containingMethod;
				if (this.cachedInstructionsContainer == null)
				{
					Instruction instruction = base.UnderlyingSameMethodInstructions.FirstOrDefault<Instruction>();
					if (instruction != null)
					{
						containingMethod = instruction.ContainingMethod;
					}
					else
					{
						containingMethod = null;
					}
					this.cachedInstructionsContainer = containingMethod;
				}
				return this.cachedInstructionsContainer;
			}
		}

		protected BasePdbStatement()
		{
		}
	}
}