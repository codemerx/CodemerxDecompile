using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustDecompile.Tools.MSBuildProjectBuilder;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.External;
using System.IO;
using System.Threading;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Decompiler.Caching;
using JustDecompile.EngineInfrastructure;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.External.JustAssembly
{
	class JustAssemblyProjectBuilder : MSBuildProjectBuilder
	{
		private Dictionary<uint, Dictionary<uint, IDecompilationResults>> decompilationResults;

        public JustAssemblyProjectBuilder(string assemblyPath, string targetPath, ILanguage language, Telerik.JustDecompiler.External.IFileGenerationNotifier notifier)
            : base(assemblyPath, targetPath, language, new JustAssemblyProjectBuilderFrameworkVersionResolver(), new DecompilationPreferences(), notifier, NoCacheAssemblyInfoService.Instance)
        {
            this.decompilationResults = new Dictionary<uint, Dictionary<uint, IDecompilationResults>>();
        }

        public JustAssemblyProjectBuilder(string assemblyPath, AssemblyDefinition assembly,
            Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<TypeDefinition>> userDefinedTypes,
             Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> resources,
            string targetPath, ILanguage language, IDecompilationPreferences preferences, Telerik.JustDecompiler.External.IFileGenerationNotifier notifier)
            : base(assemblyPath, assembly, userDefinedTypes, resources, targetPath, language, new JustAssemblyProjectBuilderFrameworkVersionResolver(), preferences, NoCacheAssemblyInfoService.Instance)
        {
            this.decompilationResults = new Dictionary<uint, Dictionary<uint, IDecompilationResults>>();
            this.fileGeneratedNotifier = notifier;
        }

        public IAssemblyDecompilationResults GenerateFiles(CancellationToken cancellationToken)
		{
			this.decompilationResults = new Dictionary<uint, Dictionary<uint, IDecompilationResults>>();
			
			BuildProjectCancellable(cancellationToken);

			ICollection<IModuleDecompilationResults> moduleDecompilationResults = new List<IModuleDecompilationResults>();

			foreach (KeyValuePair<uint, Dictionary<uint, IDecompilationResults>> pair in this.decompilationResults)
			{
				uint moduleToken = pair.Key;
				moduleDecompilationResults.Add(new ModuleDecompilationResults(moduleToken, GetModuleFilePathByToken(moduleToken), pair.Value));
			}

			string assemblyAttributesFilePath = GetAssemblyAttributesFilePath();
			IDecompilationResults assemblyAttributesDecompilationResults = GetAssemblyAttributesDecompilationResults(assemblyAttributesFilePath);

			ProjectGenerationDecompilationCacheService.ClearAssemblyContextsCache();
			this.currentAssemblyResolver.ClearCache();
			this.currentAssemblyResolver.ClearAssemblyFailedResolverCache();

			ICollection<string> resourcesFilePaths = new List<string>();

			foreach (string resourceFilePath in this.resourcesToPathsMap.Values)
			{
				resourcesFilePaths.Add(Path.Combine(this.targetDir, resourceFilePath));
			}

			foreach (string xamlResourceFilePath in this.xamlResourcesToPathsMap.Values)
			{
				resourcesFilePaths.Add(Path.Combine(this.targetDir, xamlResourceFilePath));
			}

			return new AssemblyDecompilationResults(this.assemblyPath, assemblyAttributesDecompilationResults, moduleDecompilationResults, resourcesFilePaths);
		}

		private IDecompilationResults GetAssemblyAttributesDecompilationResults(string assemblyAttributesFilePath)
		{
			AvalonEditCodeFormatter formatter = new AvalonEditCodeFormatter(new StringWriter());

            IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true);
			IAssemblyAttributeWriter writer = language.GetAssemblyAttributeWriter(formatter, this.exceptionFormater, settings);
			IWriterContextService writerContextService = new TypeCollisionWriterContextService(new ProjectGenerationDecompilationCacheService(), decompilationPreferences.RenameInvalidMembers);

			string fileContent;
			try
			{
				writer.WriteAssemblyInfo(assembly, writerContextService, true,
					new string[1] { "System.Runtime.Versioning.TargetFrameworkAttribute" }, new string[1] { "System.Security.UnverifiableCodeAttribute" });

				fileContent = formatter.GetSourceCode().GetSourceCode();
			}
			catch (Exception e)
			{

				string[] exceptionMessageLines = exceptionFormater.Format(e, null, assemblyAttributesFilePath);
				fileContent = string.Join(Environment.NewLine, exceptionMessageLines);
			}

			using (StreamWriter outfile = new StreamWriter(assemblyAttributesFilePath))
			{
				outfile.Write(fileContent);
			}

			JustDecompile.EngineInfrastructure.ICodeViewerResults originalcodeViewerResults = formatter.GetSourceCode();

			if (!(originalcodeViewerResults is JustDecompile.EngineInfrastructure.DecompiledSourceCode))
			{
				throw new Exception("Unexpected code viewer results type.");
			}

			JustDecompile.EngineInfrastructure.DecompiledSourceCode decompiledSourceCode = originalcodeViewerResults as JustDecompile.EngineInfrastructure.DecompiledSourceCode;

			ICodeViewerResults codeViewerResults = new CodeViewerResults(decompiledSourceCode);

			return new DecompilationResults(assemblyAttributesFilePath, codeViewerResults, 
				new Dictionary<uint, IOffsetSpan>(), new Dictionary<uint, IOffsetSpan>(), new Dictionary<uint, IOffsetSpan>(), new Dictionary<uint, IOffsetSpan>(), new HashSet<uint>());
		}

		private string GetAssemblyAttributesFilePath()
		{
			string targetDir = Path.GetDirectoryName(this.TargetPath);
			string decompilerDirectory = Path.Combine(targetDir, "JustAssemblyDecompilerResults");

			if (!Directory.Exists(decompilerDirectory))
			{
				Directory.CreateDirectory(decompilerDirectory);
			}

			string assemblyAttributesFileName = "AssemblyAttributes" + this.language.VSCodeFileExtension;
			string assemblyAttributesFilePath = Path.Combine(decompilerDirectory, assemblyAttributesFileName);

			int collisionsCount = 0;
			while (File.Exists(assemblyAttributesFilePath))
			{
				collisionsCount++;
				assemblyAttributesFileName = "AssemblyAttributes" + collisionsCount.ToString() + this.language.VSCodeFileExtension;
				assemblyAttributesFilePath = Path.Combine(decompilerDirectory, assemblyAttributesFileName);
			}

			return assemblyAttributesFilePath;
		}

		private string GetModuleFilePathByToken(uint moduleToken)
		{
			ModuleDefinition module = this.assembly.Modules.Where(x => x.MetadataToken.ToUInt32() == moduleToken).FirstOrDefault();
			return module.FilePath;
		}

		private object recordGeneratedFileDataLock = new object();

		protected override void RecordGeneratedFileData(TypeDefinition type, string sourceFilePath, StringWriter theWriter, IFormatter formatter, IWriterContextService writerContextService, List<WritingInfo> writingInfos)
		{
			lock (recordGeneratedFileDataLock)
			{
				if (!(formatter is AvalonEditCodeFormatter))
				{
					throw new Exception("Unexpected formatter type.");
				}

				AvalonEditCodeFormatter languageCodeFormatter = formatter as AvalonEditCodeFormatter;

				JustDecompile.EngineInfrastructure.ICodeViewerResults originalcodeViewerResults = languageCodeFormatter.GetSourceCode();

				if (!(originalcodeViewerResults is JustDecompile.EngineInfrastructure.DecompiledSourceCode))
				{
					throw new Exception("Unexpected code viewer results type.");
				}

				JustDecompile.EngineInfrastructure.DecompiledSourceCode decompiledSourceCode = originalcodeViewerResults as JustDecompile.EngineInfrastructure.DecompiledSourceCode;

				ICodeViewerResults codeViewerResults = new CodeViewerResults(decompiledSourceCode);
				Dictionary<uint, IOffsetSpan> memberDeclarationToCodePostionMap = GetMemberDeclarationToCodePostionMap(writingInfos);
				Dictionary<uint, IOffsetSpan> memberTokenToDocumentationMap = GetMemberTokenToDocumentationMap(writingInfos);
				Dictionary<uint, IOffsetSpan> memberTokenToAttributesMap = GetMemberTokenToAttributesMap(writingInfos);
				Dictionary<uint, IOffsetSpan> memberTokenToDecompiledCodeMap = GetMemberTokenToDecompiledCodeMap(writingInfos);
				ICollection<uint> membersWithExceptions = GetMembersWithExceptions(writingInfos);

				IDecompilationResults typeDecompilationResults = new DecompilationResults(sourceFilePath, codeViewerResults, memberDeclarationToCodePostionMap,
					memberTokenToDocumentationMap, memberTokenToAttributesMap, memberTokenToDecompiledCodeMap, membersWithExceptions);

				uint moduleToken = type.Module.MetadataToken.ToUInt32();
				if (!this.decompilationResults.ContainsKey(moduleToken))
				{
					this.decompilationResults.Add(moduleToken, new Dictionary<uint, IDecompilationResults>());
				}

				Dictionary<uint, IDecompilationResults> moduleTypes = this.decompilationResults[moduleToken];
				moduleTypes.Add(type.MetadataToken.ToUInt32(), typeDecompilationResults);
			}
		}

		private Dictionary<uint, IOffsetSpan> GetMemberDeclarationToCodePostionMap(List<WritingInfo> writingInfos)
		{
			Dictionary<uint, IOffsetSpan> result = new Dictionary<uint, IOffsetSpan>();

			foreach (WritingInfo writingInfo in writingInfos)
			{
				foreach (KeyValuePair<IMemberDefinition, Telerik.JustDecompiler.Languages.OffsetSpan> pair in writingInfo.MemberDeclarationToCodePostionMap)
				{
					result[pair.Key.MetadataToken.ToUInt32()] = new OffsetSpan(pair.Value.StartOffset, pair.Value.EndOffset);
				}
			}

			return result;
		}

		private Dictionary<uint, IOffsetSpan> GetMemberTokenToDocumentationMap(List<WritingInfo> writingInfos)
		{
			Dictionary<uint, IOffsetSpan> result = new Dictionary<uint, IOffsetSpan>();

			foreach (WritingInfo writingInfo in writingInfos)
			{
				foreach (KeyValuePair<uint, Telerik.JustDecompiler.Languages.OffsetSpan> pair in writingInfo.MemberTokenToDocumentationMap)
				{
					result[pair.Key] = new OffsetSpan(pair.Value.StartOffset, pair.Value.EndOffset);
				}
			}

			return result;
		}

		private Dictionary<uint, IOffsetSpan> GetMemberTokenToAttributesMap(List<WritingInfo> writingInfos)
		{
			Dictionary<uint, IOffsetSpan> result = new Dictionary<uint, IOffsetSpan>();

			foreach (WritingInfo writingInfo in writingInfos)
			{
				foreach (KeyValuePair<uint, Telerik.JustDecompiler.Languages.OffsetSpan> pair in writingInfo.MemberTokenToAttributesMap)
				{
					result[pair.Key] = new OffsetSpan(pair.Value.StartOffset, pair.Value.EndOffset);
				}
			}

			return result;
		}

		private Dictionary<uint, IOffsetSpan> GetMemberTokenToDecompiledCodeMap(List<WritingInfo> writingInfos)
		{
			Dictionary<uint, IOffsetSpan> result = new Dictionary<uint, IOffsetSpan>();

			foreach (WritingInfo writingInfo in writingInfos)
			{
				foreach (KeyValuePair<uint, Telerik.JustDecompiler.Languages.OffsetSpan> pair in writingInfo.MemberTokenToDecompiledCodeMap)
				{
					result[pair.Key] = new OffsetSpan(pair.Value.StartOffset, pair.Value.EndOffset);
				}
			}

			return result;
		}

		public ICollection<uint> GetMembersWithExceptions(List<WritingInfo> writingInfos)
		{
			HashSet<uint> result = new HashSet<uint>();

			foreach (WritingInfo writingInfo in writingInfos)
			{
				result.UnionWith(writingInfo.MembersWithExceptions);
			}

			return result;
		}

		protected override IFormatter GetFormatter(System.IO.StringWriter writer)
		{
			return new AvalonEditCodeFormatter(writer);
		}

		protected override bool WriteMainModuleProjectFile(ModuleDefinition module)
		{
			// do not write project file
			return false;
		}

		protected override bool WriteNetModuleProjectFile(ModuleDefinition module)
		{
			// do not write project file
			return false;
		}

		protected override bool WriteSolutionFile()
		{
			// do not write solution file
			return false;
		}

		protected override void ClearCaches()
		{
			// caches are cleared after the assembly attributes have been written, so that
			// assembly & module contexts aren't being calculated twice.
		}

		class JustAssemblyProjectBuilderFrameworkVersionResolver : IFrameworkResolver
		{
			public FrameworkVersion GetDefaultFallbackFramework4Version()
			{
				return FrameworkVersion.v4_0;
			}
		}
	}
}
