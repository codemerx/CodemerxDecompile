using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler.DefineUseAnalysis
{
	internal class StackVariableDefineUseInfo
	{
		public HashSet<int> DefinedAt
		{
			get;
			private set;
		}

		public List<int> UsedAt
		{
			get;
			private set;
		}

		public StackVariableDefineUseInfo()
		{
			base();
			this.set_DefinedAt(new HashSet<int>());
			this.set_UsedAt(new List<int>());
			return;
		}
	}
}