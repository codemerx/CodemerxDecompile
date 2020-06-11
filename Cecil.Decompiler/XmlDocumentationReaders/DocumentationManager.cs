using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.XmlDocumentationReaders
{
	/// <summary>
	/// A singleton class, containing the cached documentation for a given assembly.
	/// </summary>
	public static class DocumentationManager
	{
		// At the moment the class contains the cache for only one xml file, so that it doesn't consume too much memory.

		/// <summary>
		/// The full file path of the cached file.
		/// </summary>
		private static string cachedXmlFile;

		/// <summary>
		/// The documentation cache created for the cached file, specified in <see cref="cachedXmlFile"/>.
		/// </summary>
		private static DocumentationCache cachedDocumentation;

		private static string cachedModuleLocation;

        private static readonly object locker = new object();

		private static DocumentationCache GetDocumentationCache(ModuleDefinition module)
		{
			if (IsCachedModule(module))
			{
                return cachedDocumentation;
            }


			lock (locker)
			{
                if (IsCachedModule(module))
                {
                    return cachedDocumentation;
                }

			    string filePath;
			    bool hasDocumentation = XmlDocumentationResolver.TryResolveDocumentationLocation(module, out filePath);
                if (hasDocumentation)
                {
                    if (cachedXmlFile != filePath)
                    {
                        cachedDocumentation = ReadDocumentation(filePath);
                        cachedXmlFile = filePath;
                    }
                    else 
                    {
                        // This can be the case if two different assemblies share the same documentation file and are decompiled in sequence.
                        // For instance PresentationFramework.dll form 32 and 64 bit .NET folders.
                        return cachedDocumentation;
                    }
                }
                else
                {
                    cachedDocumentation = null;
                    cachedXmlFile = null;
                }
				CacheModule(module);
			}
			return cachedDocumentation;
		}
  
		private static void CacheModule(ModuleDefinition module)
		{
			cachedModuleLocation = module.FilePath;
		}
  
		private static bool IsCachedModule(ModuleDefinition module)
		{
			return module.FilePath == cachedModuleLocation;
		}

		private static DocumentationCache GetDocumentationCache(IMemberDefinition member)
		{
			return GetDocumentationCache(GetModuleForMember(member));
		}

		private static DocumentationCache ReadDocumentation(string documentationLocation)
		{
			XmlDocumentationReader reader = new XmlDocumentationReader();
			Dictionary<string, string> map = reader.ReadDocumentation(documentationLocation);
			DocumentationCache documentation = new DocumentationCache(map, documentationLocation);
			return documentation;
		}

		/// <summary>
		/// Retrieves the documentation string for <paramref name="member"/>.
		/// </summary>
		/// <param name="member">The member that is documented.</param>
		/// <returns>Returns the string of the documentation. If no documentation was found for <paramref name="member"/> returns null.</returns>
		public static string GetDocumentationForMember(IMemberDefinition member)
		{
			DocumentationCache cache = GetDocumentationCache(member);
			if (cache == null)
			{
				return null;
			}
			return cache.GetDocumentationForMember(member);
		}
	
		private static ModuleDefinition GetModuleForMember(IMemberDefinition member)
		{
			return member is TypeDefinition ? (member as TypeDefinition).Module : member.DeclaringType.Module;
		}

		public static void ClearCache() 
		{
			cachedModuleLocation = string.Empty;
			cachedXmlFile = string.Empty;
			if (cachedDocumentation != null)
			{
				cachedDocumentation.ClearCache();
			}
		}
	}
}
