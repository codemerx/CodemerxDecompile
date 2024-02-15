using System;
using System.Linq;

namespace Mono.Cecil.Extensions
{
    public static class ModuleArchitectureExtensions
    {
        public static string GetFriendlyModuleArchitecture(this TargetArchitecture targetArchitecture)
        {
            switch (targetArchitecture)
            {
                case TargetArchitecture.I386Windows:
                    return "x86";

                case TargetArchitecture.AMD64Windows:
                    return "x64";

                case TargetArchitecture.IA64Windows:
                    return "Itanium";

                case TargetArchitecture.AnyCPU: 
                    return "Any CPU";

                default: return targetArchitecture.ToString();
            }
        }
    }
}
