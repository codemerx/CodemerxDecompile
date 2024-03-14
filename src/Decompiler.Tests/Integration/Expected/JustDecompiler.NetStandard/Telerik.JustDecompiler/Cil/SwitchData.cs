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
			this.SwitchBlock = switchBlock;
			this.DefaultCase = defaultCase;
			this.OrderedCasesArray = new InstructionBlock[orderedCases.Count];
			for (int i = 0; i < orderedCases.Count; i++)
			{
				this.OrderedCasesArray[i] = orderedCases[i];
			}
		}
	}
}