using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Cil
{
	public class SwitchData
	{
		public InstructionBlock DefaultCase
		{
			get;
			internal set;
		}

		public InstructionBlock[] OrderedCasesArray
		{
			get;
			private set;
		}

		public InstructionBlock SwitchBlock
		{
			get;
			private set;
		}

		public SwitchData(InstructionBlock switchBlock, InstructionBlock defaultCase, IList<InstructionBlock> orderedCases)
		{
			base();
			this.set_SwitchBlock(switchBlock);
			this.set_DefaultCase(defaultCase);
			this.set_OrderedCasesArray(new InstructionBlock[orderedCases.get_Count()]);
			V_0 = 0;
			while (V_0 < orderedCases.get_Count())
			{
				this.get_OrderedCasesArray()[V_0] = orderedCases.get_Item(V_0);
				V_0 = V_0 + 1;
			}
			return;
		}
	}
}