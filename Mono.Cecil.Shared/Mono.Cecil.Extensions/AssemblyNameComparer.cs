using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Extensions
{
    /*Telerik Authorship*/
    public static class AssemblyNameComparer
    {
        public static bool ArePublicKeyEquals(byte[] publicKeyToken1, byte[] publicKeyToken2)
        {
            if (publicKeyToken1 == null && publicKeyToken2 == null)
            {
                return true;
            }
            if (publicKeyToken1 != null && publicKeyToken2 != null)
            {
                return publicKeyToken1.SequenceEqual(publicKeyToken2);
            }
            return false;
        }

        public static bool AreVersionEquals(Version version1, Version version2)
        {
            if (IsZero(version1) && IsZero(version2))
            {
                return true;
            }
            else if (!IsZero(version1) && !IsZero(version2))
            {
                return Version.Equals(version1, version2);
            }
            return false;
        }

        private static bool IsZero(Version version)
        {
            return version == null || (version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0);
        }
    }
}
