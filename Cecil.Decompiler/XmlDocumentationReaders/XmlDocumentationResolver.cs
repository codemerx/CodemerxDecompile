using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;

namespace Telerik.JustDecompiler.XmlDocumentationReaders
{
	class XmlDocumentationResolver
	{
		public static bool TryResolveDocumentationLocation(ModuleDefinition module, out string location)
		{
			string moduleLocation = module.FilePath;
			string moduleExtension = Path.GetExtension(moduleLocation);
			string expectedDocumentationLocation = moduleLocation.Replace(moduleExtension,".xml");
			if (File.Exists(expectedDocumentationLocation))
			{
				location = expectedDocumentationLocation;
				return true;
			}

            location = FindXmlDocumentation(Path.GetFileName(moduleLocation), module.Runtime);
            if (location != null)
            {
                return true;
            }

			// todo : Check for online documentation here (msdn for instance)

            location = string.Empty;
            return false;
		}

        private static readonly string referenceAssembliesPath = Path.Combine(SystemInformation.ProgramFilesX86, @"Reference Assemblies\Microsoft\Framework");
        private static readonly string frameworkPath = SystemInformation.CLR_Default_32;

        private static string FindXmlDocumentation(string moduleFileName, TargetRuntime runtime)
        {
            string fileName;
            switch (runtime)
            {
                case TargetRuntime.Net_1_0:
#if !NET35
                    fileName = LookupXmlDoc(Path.Combine(frameworkPath, "v1.0.3705", moduleFileName));
#else
                     fileName = LookupXmlDoc(Path.Combine(Path.Combine(frameworkPath, "v1.0.3705"), moduleFileName));
#endif
                    break;
                case TargetRuntime.Net_1_1:
#if !NET35
                    fileName = LookupXmlDoc(Path.Combine(frameworkPath, "v1.1.4322", moduleFileName));
#else
                    fileName = LookupXmlDoc(Path.Combine(Path.Combine(frameworkPath, "v1.1.4322"), moduleFileName));
#endif
                    break;
                case TargetRuntime.Net_2_0:
#if !NET35
                    fileName = LookupXmlDoc(Path.Combine(frameworkPath, "v2.0.50727", moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(referenceAssembliesPath, "v3.5", moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(referenceAssembliesPath, @"v3.0\en", moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v3.5\Profile\Client", moduleFileName));
#else
                    fileName = LookupXmlDoc(Path.Combine(Path.Combine(frameworkPath, "v2.0.50727"), moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(Path.Combine(referenceAssembliesPath, "v3.5"), moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(Path.Combine(referenceAssembliesPath, @"v3.0\en"), moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(Path.Combine(referenceAssembliesPath, @".NETFramework\v3.5\Profile\Client"), moduleFileName));
#endif
                    break;
                case TargetRuntime.Net_4_0:
#if !NET35
                    fileName = LookupXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.0", moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(frameworkPath, "v4.0.30319", moduleFileName));
#else
                    fileName = LookupXmlDoc(Path.Combine(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.0"), moduleFileName))
                        ?? LookupXmlDoc(Path.Combine(Path.Combine(frameworkPath, "v4.0.30319"), moduleFileName));
#endif
                    break;
                default:
                    fileName = null;
                    break;
            }
            return fileName;
        }

        private static string LookupXmlDoc(string fileName)
        {
            string xmlFileName = Path.ChangeExtension(fileName, ".xml");

            if (File.Exists(xmlFileName))
            {
                return xmlFileName;
            }
            return null;
        }
	}
}
