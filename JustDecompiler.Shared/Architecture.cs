using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustDecompile.SmartAssembly.Attributes;

namespace Telerik.JustDecompiler.External
{
    [DoNotPrune]
    [DoNotObfuscateType]
    public enum Architecture
    {
        I386,
        AMD64,
        IA64,
        AnyCPU,
        ARMv7
    }
}
