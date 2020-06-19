using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using System.Threading;
using System.IO;
using Telerik.JustDecompiler.External;
using Mono.Cecil.Extensions;
using Mono.Cecil.AssemblyResolver;
using JustDecompile.SmartAssembly.Attributes;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts.FileManagers;
using JustDecompile.EngineInfrastructure;
using JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers;

namespace JustDecompile.Tools.MSBuildProjectBuilder.NetCore
{
	[DoNotPrune]
	[DoNotObfuscateType]
	public class NetCoreProjectBuilder : BaseProjectBuilder
	{
		public NetCoreProjectBuilder(string assemblyPath, AssemblyDefinition assembly,
			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes,
 			Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources,
			string targetPath, ILanguage language, IDecompilationPreferences preferences,
			IAssemblyInfoService assemblyInfoService, VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2017, ProjectGenerationSettings projectGenerationSettings = null, IProjectGenerationNotifier projectNotifier = null)
			: base(assemblyPath, assembly, userDefinedTypes, resources, targetPath, language, null, preferences, assemblyInfoService, visualStudioVersion, projectGenerationSettings, projectNotifier)
		{
			this.projectFileManager = new NetCoreProjectFileManager(this.assembly, this.assemblyInfo, this.modulesProjectsGuids);
		}

		public NetCoreProjectBuilder(string assemblyPath, string targetPath, ILanguage language, IDecompilationPreferences preferences, IFileGenerationNotifier notifier,
			IAssemblyInfoService assemblyInfoService, VisualStudioVersion visualStudioVersion = VisualStudioVersion.VS2017, ProjectGenerationSettings projectGenerationSettings = null, IProjectGenerationNotifier projectNotifier = null)
			: base(assemblyPath, targetPath, language, null, preferences, notifier, assemblyInfoService, visualStudioVersion, projectGenerationSettings, projectNotifier)
		{
			this.projectFileManager = new NetCoreProjectFileManager(this.assembly, this.assemblyInfo, this.modulesProjectsGuids);
		}

		private INetCoreProjectManager ProjectFileManager
		{
			get
			{
				return this.projectFileManager as INetCoreProjectManager;
			}
		}

		public override void BuildProjectCancellable(CancellationToken cancellationToken)
		{
			BuildProjectInternal(() => { return cancellationToken.IsCancellationRequested; });
		}

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
							projectFileCreated = WriteMainModuleProjectFile(module);
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

		protected override bool WriteMainModuleProjectFile(ModuleDefinition module)
		{
			FileStream projectFile = null;
			if (this.TargetPath.EndsWith(language.VSProjectFileExtension))
			{
				projectFile = new FileStream(this.TargetPath, FileMode.OpenOrCreate);
			}
			else
			{
				projectFile = new FileStream(this.TargetPath + language.VSProjectFileExtension, FileMode.OpenOrCreate);
			}
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

		protected override void CreateProjectReferenceInternal(ModuleDefinition module, AssemblyNameReference reference, ref int assemblyReferenceIndex, SpecialTypeAssembly special, string referencesPath, string copiedReferencesSubfolder)
		{
			AssemblyName assemblyName = new AssemblyName(reference.Name,
																			reference.FullName,
																			reference.Version,
																			reference.PublicKeyToken);
			AssemblyStrongNameExtended assemblyKey = new AssemblyStrongNameExtended(assemblyName.FullName, module.Architecture, special);

			string currentReferenceInitialLocation = this.currentAssemblyResolver.FindAssemblyPath(assemblyName, null, assemblyKey);
			AssemblyDefinition referencedAssembly = this.currentAssemblyResolver.Resolve(reference, "", assembly.MainModule.GetModuleArchitecture(), special);

#if NET35
				if (!currentReferenceInitialLocation.IsNullOrWhiteSpace())
#else
			if (!string.IsNullOrWhiteSpace(currentReferenceInitialLocation))
#endif
			{
				if (!this.IsInDotNetAssemblies(referencedAssembly))
				{
					if (!Directory.Exists(referencesPath))
					{
						Directory.CreateDirectory(referencesPath);
					}

					string currentReferenceFileName = Path.GetFileName(currentReferenceInitialLocation);
					string currentReferenceFinalLocation = Path.Combine(referencesPath, currentReferenceFileName);
					File.Copy(currentReferenceInitialLocation, currentReferenceFinalLocation, true);

					//set to normal for testing purposes-to allow the test project to delete the coppied file between test runs
					File.SetAttributes(currentReferenceFinalLocation, FileAttributes.Normal);

					string relativePath = Path.Combine(".", copiedReferencesSubfolder);
					relativePath = Path.Combine(relativePath, currentReferenceFileName);

					this.ProjectFileManager.AddReferenceProjectItem(
						assemblyReferenceIndex,
						Path.GetFileNameWithoutExtension(currentReferenceFinalLocation),
						relativePath);
				}
			}
			else
			{
				if (this.IsAspNetAssembly(reference))
				{
					this.ProjectFileManager.AddPackageReferenceProjectItem(reference.Name, reference.Version.ToString());
				}
				else
				{
					this.ProjectFileManager.AddReferenceProjectItem(assemblyReferenceIndex, reference.FullName);
				}
			}
		}

		private bool IsAspNetAssembly(AssemblyNameReference reference)
		{
			return reference.Name.StartsWith("Microsoft.AspNetCore");
		}
	}
}
