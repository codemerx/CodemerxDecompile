using System;
using System.Linq;
using JustDecompile.SmartAssembly.Attributes;

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
            Start = start;
            End = end;
        }
    }
}
