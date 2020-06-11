using System;
//using JustDecompile.EngineInfrastructure;
using Mono.Cecil;
using System.IO;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages;
using System.Collections.Generic;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    class ProjectItemWriterFactory
    {
        private readonly IProjectManager projectFileManager;
        private readonly string sourceExtension;
		private readonly string targetDirectory;
        private readonly AssemblyDefinition assembly;
		private object fileNamesCollisionsLock = new object();
		private readonly IFilePathsService filePathsService;
		private readonly Dictionary<TypeDefinition, string> typeToPathMap;

        public ProjectItemWriterFactory(AssemblyDefinition thisAssembly, Mono.Collections.Generic.Collection<TypeDefinition> userDefinedTypes, 
			IProjectManager projectFileManager, IFilePathsService filePathsService, string sourceExtension, string targetDirectory)
        {
            this.assembly = thisAssembly;
            this.projectFileManager = projectFileManager;
			this.filePathsService = filePathsService;
			this.typeToPathMap = filePathsService.GetTypesToFilePathsMap();
            this.sourceExtension = sourceExtension;
			this.targetDirectory = targetDirectory;
		}

        public IProjectItemFileWriter GetProjectItemWriter(TypeDefinition type)
        {
			IMsBuildProjectManager msBuildProjectManager = this.projectFileManager as IMsBuildProjectManager;

			string relativeResourcePath;
            if(this.projectFileManager.ResourceDesignerMap.TryGetValue(type.FullName, out relativeResourcePath) ||
                IsVBResourceType(type, out relativeResourcePath))
            {
                if (type.BaseType != null && type.BaseType.Namespace == "System.Windows.Forms" && type.BaseType.Name == "Form")
                {
                    return new WinFormsItemWriter(this.targetDirectory,
                                                  relativeResourcePath,
                                                  sourceExtension,
												  msBuildProjectManager);
                }
                else
                {
                    return new ResXDesignerWriter(this.targetDirectory,
                                                  relativeResourcePath,
												  this.projectFileManager,
												  filePathsService);
                }
            }

            string relativeXamlPath;
            if(this.projectFileManager.XamlFullNameToRelativePathMap.TryGetValue(type.FullName, out relativeXamlPath))
            {
                if(assembly.EntryPoint != null && assembly.EntryPoint.DeclaringType == type)
                {
                    return new AppDefinitionItemWriter(this.targetDirectory,
                                                relativeXamlPath,
                                                sourceExtension,
												msBuildProjectManager);
                }
                else
                {
                    return new XamlPageItemWriter(this.targetDirectory,
                                                relativeXamlPath,
                                                sourceExtension,
												msBuildProjectManager);
                }
            }

			string friendlyFilePath;

			if (type.FullName == "Properties.AssemblyInfo")
			{
				friendlyFilePath = filePathsService.GetAssemblyInfoRelativePath();
			}
			else
			{
				lock (this.fileNamesCollisionsLock)
				{
					friendlyFilePath = this.typeToPathMap[type];
				}
			}

            return new RegularProjectItemWriter(this.targetDirectory,
											   friendlyFilePath,
                                               this.projectFileManager);
        }

        private bool IsVBResourceType(TypeDefinition type, out string relativeResourcePath)
        {
            relativeResourcePath = null;
            string resourceName;
            return Utilities.IsVBResourceType(type, out resourceName) && this.projectFileManager.ResourceDesignerMap.TryGetValue(resourceName, out relativeResourcePath);
        }
    }
}
