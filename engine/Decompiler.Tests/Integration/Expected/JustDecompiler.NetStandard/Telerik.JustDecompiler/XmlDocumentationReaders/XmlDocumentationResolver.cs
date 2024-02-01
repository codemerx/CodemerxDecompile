using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;
using System.IO;

namespace Telerik.JustDecompiler.XmlDocumentationReaders
{
	internal class XmlDocumentationResolver
	{
		private readonly static string referenceAssembliesPath;

		private readonly static string frameworkPath;

		static XmlDocumentationResolver()
		{
			XmlDocumentationResolver.referenceAssembliesPath = Path.Combine(SystemInformation.get_ProgramFilesX86(), "Reference Assemblies\\Microsoft\\Framework");
			XmlDocumentationResolver.frameworkPath = SystemInformation.CLR_Default_32;
		}

		public XmlDocumentationResolver()
		{
		}

		private static string FindXmlDocumentation(string moduleFileName, TargetRuntime runtime)
		{
			string str;
			switch (runtime)
			{
				case 0:
				{
					str = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v1.0.3705", moduleFileName));
					break;
				}
				case 1:
				{
					str = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v1.1.4322", moduleFileName));
					break;
				}
				case 2:
				{
					str = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v2.0.50727", moduleFileName)) ?? (XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, "v3.5", moduleFileName)) ?? (XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, "v3.0\\en", moduleFileName)) ?? XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, ".NETFramework\\v3.5\\Profile\\Client", moduleFileName))));
					break;
				}
				case 3:
				{
					str = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, ".NETFramework\\v4.0", moduleFileName)) ?? XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v4.0.30319", moduleFileName));
					break;
				}
				default:
				{
					str = null;
					break;
				}
			}
			return str;
		}

		private static string LookupXmlDoc(string fileName)
		{
			string str = Path.ChangeExtension(fileName, ".xml");
			if (File.Exists(str))
			{
				return str;
			}
			return null;
		}

		public static bool TryResolveDocumentationLocation(ModuleDefinition module, out string location)
		{
			string filePath = module.get_FilePath();
			string str = filePath.Replace(Path.GetExtension(filePath), ".xml");
			if (File.Exists(str))
			{
				location = str;
				return true;
			}
			location = XmlDocumentationResolver.FindXmlDocumentation(Path.GetFileName(filePath), module.get_Runtime());
			if (location != null)
			{
				return true;
			}
			location = String.Empty;
			return false;
		}
	}
}