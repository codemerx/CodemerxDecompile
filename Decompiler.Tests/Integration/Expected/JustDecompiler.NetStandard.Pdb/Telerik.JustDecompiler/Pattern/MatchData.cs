using System;

namespace Telerik.JustDecompiler.Pattern
{
	public struct MatchData
	{
		public readonly string Name;

		public readonly object Value;

		public MatchData(string name, object value)
		{
			this.Name = name;
			this.Value = value;
			return;
		}
	}
}