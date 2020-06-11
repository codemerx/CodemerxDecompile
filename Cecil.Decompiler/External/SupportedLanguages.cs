using System;
using System.Linq;
using JustDecompile.SmartAssembly.Attributes;


namespace Telerik.JustDecompiler
{
    [DoNotPrune]
    [DoNotObfuscateType]
    public enum SupportedLanguages
    {
        CSharp,
        VB,
        MSIL
    }
}
