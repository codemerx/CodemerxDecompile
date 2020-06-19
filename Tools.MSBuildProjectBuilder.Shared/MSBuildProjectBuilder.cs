using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;

#if !NET35
using System.Threading.Tasks;
#endif

using JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Cecil.Extensions;
#if !ENGINEONLYBUILD && !JUSTASSEMBLY
using Telerik.Baml;
#endif
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Common.NamespaceHierarchy;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Decompiler.Caching;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using JustDecompile.SmartAssembly.Attributes;
using System.Collections;
using JustDecompile.EngineInfrastructure;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.VisualBasic;
using JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
	[DoNotPrune]
    [DoNotObfuscateType]
    public class MSBuildProjectBuilder : BaseProjectBuilder
	{
        public MSBuildProjectBuilder(string assemblyPath, AssemblyDefinition assembly,
            Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes,
 			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources,
			string targetPath, ILanguage language, IFrameworkResolver frameworkResolver,
			IDecompilationPreferences preferences, IAssemblyInfoService assemblyInfoService,
			VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2010, ProjectGenerationSettings projectGenerationSettings = null,
			IProjectGenerationNotifier projectNotifier = null)
			: base(assemblyPath, assembly, userDefinedTypes, resources, targetPath, language, frameworkResolver, preferences, assemblyInfoService, visualStudioVersion, projectGenerationSettings, projectNotifier)
        {
			this.projectFileManager = new MsBuildProjectFileManager(this.assembly, this.assemblyInfo, this.visualStudioVersion, this.modulesProjectsGuids, this.language, this.namespaceHierarchyTree);
        }

        public MSBuildProjectBuilder(string assemblyPath, string targetPath, ILanguage language,
            IFrameworkResolver frameworkResolver, IDecompilationPreferences preferences, IFileGenerationNotifier notifier,
			IAssemblyInfoService assemblyInfoService, VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2010,
			ProjectGenerationSettings projectGenerationSettings = null, IProjectGenerationNotifier projectNotifier = null)
			:base(assemblyPath, targetPath, language, frameworkResolver, preferences, notifier, assemblyInfoService, visualStudioVersion, projectGenerationSettings, projectNotifier)
        {
			this.projectFileManager = new MsBuildProjectFileManager(this.assembly, this.assemblyInfo, this.visualStudioVersion, this.modulesProjectsGuids, this.language, this.namespaceHierarchyTree);
		}

		protected IMsBuildProjectManager ProjectFileManager
		{
			get
			{
				return this.projectFileManager as IMsBuildProjectManager;
			}
		}

#if !NET35
		public override void BuildProjectCancellable(CancellationToken cancellationToken)
		{
			BuildProjectInternal(() => { return cancellationToken.IsCancellationRequested; });
		}
#endif

		public override void BuildProject()
		{
			BuildProjectInternal(() => { return false; });
		}

		public override void BuildProjectComCancellable(Func<bool> shouldCancel)
		{
			BuildProjectInternal(shouldCancel);
		}

		private bool BuildProjectInternal(Func<bool> shouldCancel)
        {
			if (this.fileGeneratedNotifier != null)
			{
				this.fileGeneratedNotifier.TotalFileCount = this.NumberOfFilesToGenerate;
			}

            try
            {
				foreach (ModuleDefinition module in assembly.Modules)
				{
					ModuleDefinition currentModule = module;

					CreateResources(module);
#if !NET35
					Task.Factory.StartNew(
						() => WriteUserDefinedTypes(currentModule, shouldCancel))
						.Wait();
#else
				    WriteUserDefinedTypes(currentModule, shouldCancel);
#endif

					if (shouldCancel())
					{
						return true;
					}

					bool isMainModule = Utilities.IsMainModule(module);

					if (isMainModule)
					{
						bool createdFile;
						string assemblyInfoRelativePath = WriteAssemblyInfo(assembly, out createdFile);
						if (createdFile)
						{
							this.ProjectFileManager.IncludeAssemblyInfo(assemblyInfoRelativePath);

							IFileGeneratedInfo assemblyInfoArgs = new FileGeneratedInfo(Path.Combine(this.targetDir, assemblyInfoRelativePath), false);
							this.OnProjectFileCreated(assemblyInfoArgs);
						}
					}

                    CopyAppConfig(module);

                    if (this.projectGenerationSettings != null && !this.projectGenerationSettings.JustDecompileSupportedProjectType && module.IsMain)
                    {
                        StreamWriter writer;
                        if (this.TargetPath.EndsWith(language.VSProjectFileExtension + ErrorFileExtension))
                        {
                            writer = new StreamWriter(this.TargetPath);
                        }
                        else
                        {
                            writer = new StreamWriter(this.TargetPath + language.VSProjectFileExtension + ErrorFileExtension);
                        }

                        using (writer)
                        {
                            writer.Write("JustDecompile: " + this.projectGenerationSettings.ErrorMessage);
                        }

                        InformProjectFileCreated(module, language.VSProjectFileExtension + ErrorFileExtension, false);
                    }
                    else
                    {
                        bool exceptionsWhenCreatingProjectFile = false;
						bool projectFileCreated = false;
						try
                        {
                            if (isMainModule)
                            {
                                projectFileCreated = this.WriteMainModuleProjectFile(module);
                            }
                            else
                            {
								projectFileCreated = this.WriteNetModuleProjectFile(module);
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptionsWhenCreatingProjectFile = true;

                            OnExceptionThrown(ex);
                        }

						if (projectFileCreated)
						{
							InformProjectFileCreated(module, this.language.VSProjectFileExtension, exceptionsWhenCreatingProjectFile);
						}
                    }

                    WriteModuleAdditionalFiles(module);
				}

                if (this.projectGenerationSettings == null || this.projectGenerationSettings.JustDecompileSupportedProjectType)
                {
                    // Write the solution file
                    bool exceptionWhileWritingSolutionFile = false;
					bool solutionFileCreated = false;
                    try
                    {
						solutionFileCreated = WriteSolutionFile();
                    }
                    catch (Exception ex)
                    {
                        exceptionWhileWritingSolutionFile = true;

                        OnExceptionThrown(ex);
                    }

					if (solutionFileCreated)
					{
						string solutionFilePath = Path.Combine(this.targetDir, this.filePathsService.GetSolutionRelativePath());
						IFileGeneratedInfo solutionArgs = new FileGeneratedInfo(solutionFilePath, exceptionWhileWritingSolutionFile);
						this.OnProjectFileCreated(solutionArgs);
					}
                }
            }
            catch (Exception ex)
            {
				base.OnProjectGenerationFailure(ex);

				if (this.projectNotifier != null)
				{
					this.projectNotifier.OnProjectGenerationFailure(ex);
				}
            }
            finally
            {
                OnProjectGenerationFinished();
            }
            if (decompilationPreferences.WriteDocumentation)
            {
                /// Clear the cached documentation
                Telerik.JustDecompiler.XmlDocumentationReaders.DocumentationManager.ClearCache();
            }

			ClearCaches();

            return false;
        }

		protected virtual bool WriteNetModuleProjectFile(ModuleDefinition module)
		{
			string moduleProjFilePath;
			if (!modulesToProjectsFilePathsMap.TryGetValue(module, out moduleProjFilePath))
			{
				throw new Exception("Module project file path not found in modules projects filepaths map.");
			}

			FileStream projectFile = new FileStream(Path.Combine(this.targetDir, moduleProjFilePath), FileMode.OpenOrCreate);

			try
			{
				this.ProjectFileManager.ConstructProject(module, (m) => this.CreateProjectReferences(m));

				this.ProjectFileManager.SerializeProject(projectFile);
			}
			catch (Exception e)
			{
				StreamWriter fileWriter = new StreamWriter(projectFile);
				string exceptionWithStackTrance = string.Format("{0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace);

				fileWriter.Write(exceptionWithStackTrance);
				throw;
			}
			finally
			{
				projectFile.Flush();
				projectFile.Close();
				projectFile.Dispose();
			}

			return true;
		}

		protected virtual void WriteModuleAdditionalFiles(ModuleDefinition module)
        {
        }

        private void CopyAppConfig(ModuleDefinition module)
        {
            string originalAppConfigFilePath = module.FilePath + ".config";
            string targetAppConfigFilePath = Path.Combine(this.targetDir, "App.config");
            if (File.Exists(originalAppConfigFilePath))
            {
                bool hasErrors = false;
                if (targetAppConfigFilePath.Length <= MaxPathLength && this.assembly.Modules.Count == 1)
                {
                    try
                    {
                        File.Copy(originalAppConfigFilePath, targetAppConfigFilePath, true);
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;

                        OnExceptionThrown(ex);
                    }

                    if (!hasErrors)
                    {
						this.ProjectFileManager.WriteAppConfigFileEntryProjectItem();
                    }
                }
                else
                {
                    hasErrors = true;
                }

                IFileGeneratedInfo appConfigArgs = new FileGeneratedInfo(targetAppConfigFilePath, hasErrors);
                this.OnProjectFileCreated(appConfigArgs);
            }
        }

        private string WriteAssemblyInfo(AssemblyDefinition assembly, out bool createdNewFile)
        {
            string fileContent;
            IAssemblyAttributeWriter writer = null;
            using (StringWriter stringWriter = new StringWriter())
            {
                IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true);
                writer = language.GetAssemblyAttributeWriter(new PlainTextFormatter(stringWriter), this.exceptionFormater, settings);
                IWriterContextService writerContextService = this.GetWriterContextService();
                writer.ExceptionThrown += OnExceptionThrown;
                writerContextService.ExceptionThrown += OnExceptionThrown;

                // "Duplicate 'TargetFramework' attribute" when having it written in AssemblyInfo
                writer.WriteAssemblyInfo(assembly, writerContextService, true,
                    new string[1] { "System.Runtime.Versioning.TargetFrameworkAttribute" }, new string[1] { "System.Security.UnverifiableCodeAttribute" });

                fileContent = stringWriter.ToString();

                writer.ExceptionThrown -= OnExceptionThrown;
                writerContextService.ExceptionThrown -= OnExceptionThrown;
            }

            string parentDirectory = Path.GetDirectoryName(this.TargetPath);
            string relativePath = filePathsService.GetAssemblyInfoRelativePath();
            string fullPath = Path.Combine(parentDirectory, relativePath);

            string dirPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (File.Exists(fullPath) && assembly.MainModule.Types.FirstOrDefault(x => x.FullName == "Properties.AssemblyInfo") != null)
            {
                createdNewFile = false;
                PushAssemblyAttributes(fullPath, fileContent, writer);
            }
            else
            {
                createdNewFile = true;
                using (StreamWriter fileWriter = new StreamWriter(fullPath, false, Encoding.UTF8))
                {
                    fileWriter.Write(fileContent);
                }
            }

            return relativePath;
        }

        private void PushAssemblyAttributes(string fileName, string fileContent, IAssemblyAttributeWriter writer)
        {
            string usingsText, content;
            HashSet<string> usings = new HashSet<string>();
            using (TextReader reader = new StreamReader(fileName))
            {
                StringBuilder usingsBuilder = new StringBuilder();
                while (true)
                {
                    string line = reader.ReadLine();
                    char[] chars = { ' ', ';' };
                    string[] words = line.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 2)
                    {
                        if (words[0] == "using")// make this language independant!
                        {
                            if (!(writer as BaseAssemblyAttributeWriter).AssemblyInfoNamespacesUsed.Contains(words[1]))
                            {
                                usingsBuilder.AppendLine(line);
                            }
                            usings.Add(words[1]);
                        }
                        else
                        {
                            content = line + Environment.NewLine;
                            break;
                        }
                    }
                    else
                    {
                        usingsBuilder.Append(line);
                    }
                }

                usingsText = usingsBuilder.ToString();
                content += reader.ReadToEnd();
            }

            File.WriteAllText(fileName, usingsText + fileContent + content);
        }

		protected override void CreateProjectReferences(ModuleDefinition module)
		{
			base.CreateProjectReferences(module);

			ICollection<ModuleReference> dependingOnModules = GetModulesDependingOn(module);
			this.ProjectFileManager.CreateAddModulesProjectItem(dependingOnModules.Count * 2);

			int moduleReferenceIndex = 0;
			foreach (ModuleReference moduleRef in dependingOnModules)
			{
				this.ProjectFileManager.AddModuleProjectItem(
					moduleReferenceIndex,
					@"bin\Debug\" + Utilities.GetNetmoduleName(moduleRef) + ".netmodule",
					" '$(Configuration)' == 'Debug' "
					);

				moduleReferenceIndex++;

				this.ProjectFileManager.AddModuleProjectItem(
					moduleReferenceIndex,
					@"bin\Release\" + Utilities.GetNetmoduleName(moduleRef) + ".netmodule",
					" '$(Configuration)' == 'Release' "
					);

				moduleReferenceIndex++;
			}
		}

		private ICollection<ModuleReference> GetModulesDependingOn(ModuleDefinition module)
		{
			ICollection<ModuleReference> result;

			ICollection<TypeReference> expadendTypeDependanceList = GetExpandedTypeDependanceList(module);
			result = Telerik.JustDecompiler.Decompiler.Utilities.GetModulesDependingOn(expadendTypeDependanceList);

			return result;
		}
	}
}
