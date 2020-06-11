using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices
{
	public class DefaultFilePathsAnalyzer
	{
		private readonly Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes;
		private readonly Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources;
		private readonly string sourceExtension;
        private readonly bool decompileDangerousResources;

        public DefaultFilePathsAnalyzer(Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes, 
			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources, ILanguage language, bool decompileDangerousResources)
		{
			this.userDefinedTypes = userDefinedTypes;
			this.resources = resources;
			this.sourceExtension = language.VSCodeFileExtension;
            this.decompileDangerousResources = decompileDangerousResources;
		}

		public int GetMinimumNeededRelativeFilePathLength(string projFileName)
		{
			int minimumTypesAndResourcesRelativePathLength = Math.Max(
				GetTypesMaxFileLength(),
				GetResourcesMaxFileLength()
			);

			return Math.Max(GetProjFileMaximumLength(projFileName), minimumTypesAndResourcesRelativePathLength);
		}

		protected virtual int GetTypesMaxFileLength()
		{
			return Math.Max(
				Convert.ToString(userDefinedTypes.Count + 1).Length + ".cs".Length,
				DefaultFilePathsService.AssemblyInfoShortFileName.Length + sourceExtension.Length);
		}

		protected virtual int GetResourcesMaxFileLength()
		{
			int resourcesCount = Utilities.GetResourcesCount(resources, this.decompileDangerousResources);

			return Math.Max(
				DefaultFilePathsService.XamlResourcesShortNameStartSymbol.Length + Convert.ToString(resourcesCount).Length +
				DefaultFilePathsService.XamlExtension.Length + sourceExtension.Length,

				DefaultFilePathsService.ResourcesShortNameStartSymbol.Length + Convert.ToString(resourcesCount).Length +
				DefaultFilePathsService.ResourceDesignerShortNameExtension.Length + sourceExtension.Length
			);
		}

		protected virtual int GetProjFileMaximumLength(string projFileName)
		{
			return projFileName.Length;
		}

	}
}
