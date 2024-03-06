using System.Reflection;

namespace CodemerxDecompile;

public static class AssemblyProvider
{
    public static Assembly Assembly => typeof(AssemblyProvider).Assembly;
}
