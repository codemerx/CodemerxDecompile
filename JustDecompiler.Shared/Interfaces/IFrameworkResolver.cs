using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil.AssemblyResolver;

namespace Telerik.JustDecompiler.External.Interfaces
{
    [DoNotPrune]
    [DoNotObfuscateType]
    public interface IFrameworkResolver
    {
        /// <summary>
        /// It is often impossible to distinguish whether a given assembly was built against .NET 4.0 or .NET 4.5 (mscorlib has the same strong name). Unless JD finds a .NET 4.5 framework specific
        /// entity in the assembly JD does not know which framework version the decompiled source code should target. Should this occur the JD engine uses the user provided implementation of this interface
        /// to determine the target framework.
        /// </summary>
        /// <returns>Possible values are "v4.0" and "v4.5"</returns>
        FrameworkVersion GetDefaultFallbackFramework4Version();
    }
}
