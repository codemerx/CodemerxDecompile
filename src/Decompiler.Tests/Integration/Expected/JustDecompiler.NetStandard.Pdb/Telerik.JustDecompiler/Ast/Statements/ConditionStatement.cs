using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public abstract class ConditionStatement : BasePdbStatement
	{
		public Expression Condition
		{
			get;
			set;
		}

		public InstructionBlock ConditionBlock
		{
			get;
			set;
		}

		public ConditionStatement(Expression condition)
		{
			this.Condition = condition;
		}
	}
}