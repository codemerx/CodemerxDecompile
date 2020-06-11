using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Ast.Expressions;
namespace Telerik.JustDecompiler.Ast.Statements
{
	public abstract class ConditionStatement : BasePdbStatement
	{
		public ConditionStatement(Expression condition)
		{
			this.Condition = condition;
		}

		public InstructionBlock ConditionBlock { get; set; }

		public Expression Condition { get; set; }
	}
}