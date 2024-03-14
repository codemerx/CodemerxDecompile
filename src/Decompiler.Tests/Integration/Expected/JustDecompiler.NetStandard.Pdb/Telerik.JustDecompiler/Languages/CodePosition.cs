using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.Languages
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public struct CodePosition
	{
		public int Line;

		public int Column;

		public CodePosition(int line, int column)
		{
			this.Line = line;
			this.Column = column;
		}
	}
}