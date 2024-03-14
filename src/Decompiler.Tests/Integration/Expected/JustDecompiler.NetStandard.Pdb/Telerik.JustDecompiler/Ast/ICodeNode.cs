using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast
{
	public interface ICodeNode
	{
		IEnumerable<ICodeNode> Children
		{
			get;
		}

		Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get;
		}

		ICollection<int> SearchableUnderlyingSameMethodInstructionOffsets
		{
			get;
		}

		IEnumerable<Instruction> UnderlyingSameMethodInstructions
		{
			get;
		}
	}
}