using Mono.Cecil;
using System;

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
			return;
		}

		public XmlDocumentationResolver()
		{
			base();
			return;
		}

		private static string FindXmlDocumentation(string moduleFileName, TargetRuntime runtime)
		{
			switch (runtime)
			{
				case 0:
				{
					V_0 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v1.0.3705", moduleFileName));
					break;
				}
				case 1:
				{
					V_0 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v1.1.4322", moduleFileName));
					break;
				}
				case 2:
				{
					stackVariable16 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v2.0.50727", moduleFileName));
					if (stackVariable16 == null)
					{
						dummyVar0 = stackVariable16;
						stackVariable16 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, "v3.5", moduleFileName));
						if (stackVariable16 == null)
						{
							dummyVar1 = stackVariable16;
							stackVariable16 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, "v3.0\\en", moduleFileName));
							if (stackVariable16 == null)
							{
								dummyVar2 = stackVariable16;
								stackVariable16 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, ".NETFramework\\v3.5\\Profile\\Client", moduleFileName));
							}
						}
					}
					V_0 = stackVariable16;
					break;
				}
				case 3:
				{
					stackVariable33 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.referenceAssembliesPath, ".NETFramework\\v4.0", moduleFileName));
					if (stackVariable33 == null)
					{
						dummyVar3 = stackVariable33;
						stackVariable33 = XmlDocumentationResolver.LookupXmlDoc(Path.Combine(XmlDocumentationResolver.frameworkPath, "v4.0.30319", moduleFileName));
					}
					V_0 = stackVariable33;
					break;
				}
				default:
				{
					V_0 = null;
					break;
				}
			}
			return V_0;
		}

		private static string LookupXmlDoc(string fileName)
		{
			V_0 = Path.ChangeExtension(fileName, ".xml");
			if (File.Exists(V_0))
			{
				return V_0;
			}
			return null;
		}

		public static bool TryResolveDocumentationLocation(ModuleDefinition module, out string location)
		{
			V_0 = module.get_FilePath();
			V_2 = V_0.Replace(Path.GetExtension(V_0), ".xml");
			if (File.Exists(V_2))
			{
				location = V_2;
				return true;
			}
			location = XmlDocumentationResolver.FindXmlDocumentation(Path.GetFileName(V_0), module.get_Runtime());
			if (location != null)
			{
				return true;
			}
			location = String.Empty;
			return false;
		}
	}
}