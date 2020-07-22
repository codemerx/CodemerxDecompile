using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
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
				if (this.cachedInstructionsContainer == null)
				{
					V_0 = this.get_UnderlyingSameMethodInstructions().FirstOrDefault<Instruction>();
					if (V_0 != null)
					{
						stackVariable10 = V_0.get_ContainingMethod();
					}
					else
					{
						stackVariable10 = null;
					}
					this.cachedInstructionsContainer = stackVariable10;
				}
				return this.cachedInstructionsContainer;
			}
		}

		protected BasePdbStatement()
		{
			base();
			return;
		}
	}
}