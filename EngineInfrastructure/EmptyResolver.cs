using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.External.Interfaces;

namespace JustDecompile.EngineInfrastructure
{
    public class EmptyResolver : IFrameworkResolver
    {
        private static EmptyResolver instance;

        private EmptyResolver()
        {
        }

        public static EmptyResolver Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmptyResolver();
                }

                return instance;
            }
        }

        public FrameworkVersion GetDefaultFallbackFramework4Version()
        {
            return FrameworkVersion.Unknown;
        }
    }
}
