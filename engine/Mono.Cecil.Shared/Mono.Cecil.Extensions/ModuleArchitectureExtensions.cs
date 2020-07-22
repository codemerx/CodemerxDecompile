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
                case TargetArchitecture.I386:
                    return "x86";

                case TargetArchitecture.AMD64:
                    return "x64";

                case TargetArchitecture.IA64:
                    return "Itanium";

                case TargetArchitecture.AnyCPU: 
                    return "Any CPU";

                default: return targetArchitecture.ToString();
            }
        }
    }
}
