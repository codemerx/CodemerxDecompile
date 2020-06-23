using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler
{
	public class CompilerOptimizedSwitchByStringData : ICloneable
	{
		public List<int> SwitchBlocksStartInstructions
		{
			get;
			private set;
		}

		public Dictionary<int, List<int>> SwitchBlocksToCasesMap
		{
			get;
			private set;
		}

		public CompilerOptimizedSwitchByStringData()
		{
			this.SwitchBlocksStartInstructions = new List<int>();
			this.SwitchBlocksToCasesMap = new Dictionary<int, List<int>>();
		}

		public object Clone()
		{
			CompilerOptimizedSwitchByStringData compilerOptimizedSwitchByStringDatum = new CompilerOptimizedSwitchByStringData()
			{
				SwitchBlocksStartInstructions = new List<int>(this.SwitchBlocksStartInstructions),
				SwitchBlocksToCasesMap = new Dictionary<int, List<int>>()
			};
			foreach (KeyValuePair<int, List<int>> switchBlocksToCasesMap in this.SwitchBlocksToCasesMap)
			{
				compilerOptimizedSwitchByStringDatum.SwitchBlocksToCasesMap.Add(switchBlocksToCasesMap.Key, new List<int>(switchBlocksToCasesMap.Value));
			}
			return compilerOptimizedSwitchByStringDatum;
		}
	}
}