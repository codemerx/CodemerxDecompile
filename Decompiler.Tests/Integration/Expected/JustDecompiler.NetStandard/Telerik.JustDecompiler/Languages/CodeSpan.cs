using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.Languages
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public struct CodeSpan
	{
		public CodePosition Start;

		public CodePosition End;

		public CodeSpan(CodePosition start, CodePosition end)
		{
			this.Start = start;
			this.End = end;
			return;
		}
	}
}