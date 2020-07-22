using System;
using System.Linq;
using JustDecompile.SmartAssembly.Attributes;

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
            Line = line;
            Column = column;
        }
    }
}
