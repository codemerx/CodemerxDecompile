using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal interface IInstructionBlockContainer
	{
		List<Expression> LogicalConstructExpressions
		{
			get;
		}

		InstructionBlock TheBlock
		{
			get;
		}
	}
}