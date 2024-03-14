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
			this.DefinedAt = new HashSet<int>();
			this.UsedAt = new List<int>();
		}
	}
}