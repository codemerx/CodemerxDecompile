using System;

using Mono.Cecil;

namespace JustDecompile.EngineInfrastructure
{
    public static class DangerousResourceIdentifier
    {
        public static bool IsDangerousResource(Resource resource)
        {
            return resource != null &&
                   resource.ResourceType == ResourceType.Embedded &&
                   resource.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase);
        }
    }
}
