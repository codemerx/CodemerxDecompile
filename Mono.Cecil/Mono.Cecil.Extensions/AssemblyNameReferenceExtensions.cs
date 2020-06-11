using System;

namespace Mono.Cecil.Extensions
{
    /*Telerik Authorship*/
    public static class AssemblyNameReferenceExtensions
    {
        public static bool IsFakeMscorlibReference(this AssemblyNameReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }

            return reference.Name == "mscorlib" &&
                reference.Version.Major == 255 &&
                reference.Version.Minor == 255 &&
                reference.Version.Revision == 255 &&
                reference.Version.Build == 255;
        }

		public static bool IsFakeCorlibReference(this AssemblyNameReference reference)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}

			return reference.Name == "mscorlib" &&
				reference.Version.Major == 0 &&
				reference.Version.Minor == 0 &&
				reference.Version.Revision == 0 &&
				reference.Version.Build == 0;
		}
    }
}
