using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustDecompiler.Languages
{
    public struct OffsetSpan
    {
        public int StartOffset;
        public int EndOffset;

        public OffsetSpan(int start, int end)
        {
            StartOffset = start;
            EndOffset = end;
        }
    }
}
