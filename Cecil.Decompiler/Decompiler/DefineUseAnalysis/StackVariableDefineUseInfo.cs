using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.DefineUseAnalysis
{
    class StackVariableDefineUseInfo
    {
        public HashSet<int> DefinedAt { get; private set; }
        public List<int> UsedAt { get; private set; }

        public StackVariableDefineUseInfo()
        {
            DefinedAt = new HashSet<int>();
            UsedAt = new List<int>();
        }
    }
}
