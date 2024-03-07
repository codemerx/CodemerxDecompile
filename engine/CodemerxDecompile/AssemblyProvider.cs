using System;
using System.Reflection;

namespace CodemerxDecompile;

public static class AssemblyProvider
{
    private static readonly Lazy<Assembly> AssemblyHolder = new(() => typeof(AssemblyProvider).Assembly);

    public static Assembly Assembly => AssemblyHolder.Value;
}
