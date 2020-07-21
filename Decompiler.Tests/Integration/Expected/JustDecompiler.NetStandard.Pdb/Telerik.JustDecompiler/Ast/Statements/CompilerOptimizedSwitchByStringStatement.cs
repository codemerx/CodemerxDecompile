using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CompilerOptimizedSwitchByStringStatement : SwitchStatement
	{
		public IEnumerable<int> LoadSwitchValueInstructionOffsets
		{
			get;
			private set;
		}

		public CompilerOptimizedSwitchByStringStatement(Expression condition, IEnumerable<int> loadSwitchValueInstructionOffsets)
		{
			base(condition, null);
			this.set_LoadSwitchValueInstructionOffsets(loadSwitchValueInstructionOffsets);
			return;
		}
	}
}