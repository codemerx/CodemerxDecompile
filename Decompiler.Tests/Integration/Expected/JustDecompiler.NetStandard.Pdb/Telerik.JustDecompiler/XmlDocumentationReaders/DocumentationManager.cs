using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.XmlDocumentationReaders
{
	public static class DocumentationManager
	{
		private static string cachedXmlFile;

		private static DocumentationCache cachedDocumentation;

		private static string cachedModuleLocation;

		private readonly static object locker;

		static DocumentationManager()
		{
			DocumentationManager.locker = new Object();
			return;
		}

		private static void CacheModule(ModuleDefinition module)
		{
			DocumentationManager.cachedModuleLocation = module.get_FilePath();
			return;
		}

		public static void ClearCache()
		{
			DocumentationManager.cachedModuleLocation = String.Empty;
			DocumentationManager.cachedXmlFile = String.Empty;
			if (DocumentationManager.cachedDocumentation != null)
			{
				DocumentationManager.cachedDocumentation.ClearCache();
			}
			return;
		}

		private static DocumentationCache GetDocumentationCache(ModuleDefinition module)
		{
			if (DocumentationManager.IsCachedModule(module))
			{
				return DocumentationManager.cachedDocumentation;
			}
			V_0 = DocumentationManager.locker;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				if (!DocumentationManager.IsCachedModule(module))
				{
					if (!XmlDocumentationResolver.TryResolveDocumentationLocation(module, out V_2))
					{
						DocumentationManager.cachedDocumentation = null;
						DocumentationManager.cachedXmlFile = null;
					}
					else
					{
						if (!String.op_Inequality(DocumentationManager.cachedXmlFile, V_2))
						{
							V_3 = DocumentationManager.cachedDocumentation;
							goto Label1;
						}
						else
						{
							DocumentationManager.cachedDocumentation = DocumentationManager.ReadDocumentation(V_2);
							DocumentationManager.cachedXmlFile = V_2;
						}
					}
					DocumentationManager.CacheModule(module);
					goto Label0;
				}
				else
				{
					V_3 = DocumentationManager.cachedDocumentation;
				}
			}
			finally
			{
				if (V_1)
				{
					Monitor.Exit(V_0);
				}
			}
		Label1:
			return V_3;
		Label0:
			return DocumentationManager.cachedDocumentation;
		}

		private static DocumentationCache GetDocumentationCache(IMemberDefinition member)
		{
			return DocumentationManager.GetDocumentationCache(DocumentationManager.GetModuleForMember(member));
		}

		public static string GetDocumentationForMember(IMemberDefinition member)
		{
			V_0 = DocumentationManager.GetDocumentationCache(member);
			if (V_0 == null)
			{
				return null;
			}
			return V_0.GetDocumentationForMember(member);
		}

		private static ModuleDefinition GetModuleForMember(IMemberDefinition member)
		{
			if (member as TypeDefinition == null)
			{
				return member.get_DeclaringType().get_Module();
			}
			return (member as TypeDefinition).get_Module();
		}

		private static bool IsCachedModule(ModuleDefinition module)
		{
			return String.op_Equality(module.get_FilePath(), DocumentationManager.cachedModuleLocation);
		}

		private static DocumentationCache ReadDocumentation(string documentationLocation)
		{
			return new DocumentationCache((new XmlDocumentationReader()).ReadDocumentation(documentationLocation), documentationLocation);
		}
	}
}