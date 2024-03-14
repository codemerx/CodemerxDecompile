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
		}

		private static void CacheModule(ModuleDefinition module)
		{
			DocumentationManager.cachedModuleLocation = module.get_FilePath();
		}

		public static void ClearCache()
		{
			DocumentationManager.cachedModuleLocation = String.Empty;
			DocumentationManager.cachedXmlFile = String.Empty;
			if (DocumentationManager.cachedDocumentation != null)
			{
				DocumentationManager.cachedDocumentation.ClearCache();
			}
		}

		private static DocumentationCache GetDocumentationCache(ModuleDefinition module)
		{
			string str;
			DocumentationCache documentationCache;
			if (DocumentationManager.IsCachedModule(module))
			{
				return DocumentationManager.cachedDocumentation;
			}
			lock (DocumentationManager.locker)
			{
				if (!DocumentationManager.IsCachedModule(module))
				{
					if (!XmlDocumentationResolver.TryResolveDocumentationLocation(module, out str))
					{
						DocumentationManager.cachedDocumentation = null;
						DocumentationManager.cachedXmlFile = null;
					}
					else if (DocumentationManager.cachedXmlFile == str)
					{
						documentationCache = DocumentationManager.cachedDocumentation;
						return documentationCache;
					}
					else
					{
						DocumentationManager.cachedDocumentation = DocumentationManager.ReadDocumentation(str);
						DocumentationManager.cachedXmlFile = str;
					}
					DocumentationManager.CacheModule(module);
					return DocumentationManager.cachedDocumentation;
				}
				else
				{
					documentationCache = DocumentationManager.cachedDocumentation;
				}
			}
			return documentationCache;
		}

		private static DocumentationCache GetDocumentationCache(IMemberDefinition member)
		{
			return DocumentationManager.GetDocumentationCache(DocumentationManager.GetModuleForMember(member));
		}

		public static string GetDocumentationForMember(IMemberDefinition member)
		{
			DocumentationCache documentationCache = DocumentationManager.GetDocumentationCache(member);
			if (documentationCache == null)
			{
				return null;
			}
			return documentationCache.GetDocumentationForMember(member);
		}

		private static ModuleDefinition GetModuleForMember(IMemberDefinition member)
		{
			if (!(member is TypeDefinition))
			{
				return member.get_DeclaringType().get_Module();
			}
			return (member as TypeDefinition).get_Module();
		}

		private static bool IsCachedModule(ModuleDefinition module)
		{
			return module.get_FilePath() == DocumentationManager.cachedModuleLocation;
		}

		private static DocumentationCache ReadDocumentation(string documentationLocation)
		{
			return new DocumentationCache((new XmlDocumentationReader()).ReadDocumentation(documentationLocation), documentationLocation);
		}
	}
}