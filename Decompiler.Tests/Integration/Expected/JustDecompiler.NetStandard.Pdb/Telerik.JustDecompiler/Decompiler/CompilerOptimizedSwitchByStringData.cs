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
			base();
			this.set_SwitchBlocksStartInstructions(new List<int>());
			this.set_SwitchBlocksToCasesMap(new Dictionary<int, List<int>>());
			return;
		}

		public object Clone()
		{
			V_0 = new CompilerOptimizedSwitchByStringData();
			V_0.set_SwitchBlocksStartInstructions(new List<int>(this.get_SwitchBlocksStartInstructions()));
			V_0.set_SwitchBlocksToCasesMap(new Dictionary<int, List<int>>());
			V_1 = this.get_SwitchBlocksToCasesMap().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.get_SwitchBlocksToCasesMap().Add(V_2.get_Key(), new List<int>(V_2.get_Value()));
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}
	}
}