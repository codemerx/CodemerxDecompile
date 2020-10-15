//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;

using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.External.Interfaces;
using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using JustDecompile.EngineInfrastructure;
using JustDecompile.Tools.MSBuildProjectBuilder.NetCore;
using CodemerxDecompile.Service.Extensions;
using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;
using CodemerxDecompile.Service.Services.Search.Models;

namespace CodemerxDecompile.Service
{
    public class RpcDecompilerService : RpcDecompiler.RpcDecompilerBase
    {
        private readonly IDecompilationContextService decompilationContext;
        private readonly IPathService pathService;
        private readonly ISearchService searchService;

        public RpcDecompilerService(IDecompilationContextService decompilationContext, IPathService pathService, ISearchService searchService)
        {
            this.decompilationContext = decompilationContext;
            this.pathService = pathService;
            this.searchService = searchService;
        }

        public override Task<Empty> RestoreDecompilationContext(Empty request, ServerCallContext context)
        {
            foreach (string assemblyFilePath in this.decompilationContext.GetOpenedAssemliesPaths())
            {
                AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(assemblyFilePath);
                Dictionary<string, TypeDefinition> filePathToTypeDefinition = this.GetAllTypeFilePathsForAssembly(assembly, assemblyFilePath);

                this.decompilationContext.DecompilationContext.FilePathToType.AddRange(filePathToTypeDefinition);
            }

            return Task.FromResult(new Empty());
        }

        public override Task<ShouldDecompileFileResponse> ShouldDecompileFile(ShouldDecompileFileRequest request, ServerCallContext context)
        {
            string normalizedFilePath = this.NormalizeFilePath(request.FilePath);
            if (!this.decompilationContext.DecompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition) ||
                this.decompilationContext.TryGetTypeMetadataFromCache(typeDefinition, out _))
            {
                return Task.FromResult(new ShouldDecompileFileResponse() { ShouldDecompileFile = false });
            }

            return Task.FromResult(new ShouldDecompileFileResponse() { ShouldDecompileFile = true });
        }

        public override Task<GetWorkspaceDirectoryResponse> GetWorkspaceDirectory(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new GetWorkspaceDirectoryResponse() { DirectoryPath = this.pathService.WorkingDirectory });
        }

        public override Task<GetAssemblyRelatedFilePathsResponse> GetAssemblyRelatedFilePaths(GetAssemblyRelatedFilePathsRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(request.AssemblyPath);

            GetAssemblyRelatedFilePathsResponse response = new GetAssemblyRelatedFilePathsResponse()
            {
                DecompiledAssemblyDirectory = this.pathService.WorkingDirectory,
                DecompiledAssemblyPath = Path.Join(this.pathService.WorkingDirectory, assembly.FullName)
            };

            return Task.FromResult(response);
        }

        public override Task<GetAllTypeFilePathsResponse> GetAllTypeFilePaths(GetAllTypeFilePathsRequest request, ServerCallContext context)
        {
            string normalizedAssemblyFilePath = this.NormalizeFilePath(request.AssemblyPath);
            AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(normalizedAssemblyFilePath);

            Dictionary<string, TypeDefinition> filePathToTypeDefinition = this.GetAllTypeFilePathsForAssembly(assembly, normalizedAssemblyFilePath);

            this.decompilationContext.SaveAssemblyToCache(assembly, normalizedAssemblyFilePath);
            this.decompilationContext.DecompilationContext.FilePathToType.AddRange(filePathToTypeDefinition);

            GetAllTypeFilePathsResponse response = new GetAllTypeFilePathsResponse();
            response.TypeFilePaths.AddRange(filePathToTypeDefinition.Keys);

            return Task.FromResult(response);
        }

        private Dictionary<string, TypeDefinition> GetAllTypeFilePathsForAssembly(AssemblyDefinition assembly, string assemblyFilePath)
        {
            Dictionary<ModuleDefinition, Collection<TypeDefinition>> userDefinedTypes = Utilities.GetUserDefinedTypes(assembly, true);
            Dictionary<ModuleDefinition, Collection<Resource>> resources = Utilities.GetResources(assembly);
            DefaultFilePathsService filePathsService = new DefaultFilePathsService(
                assembly,
                assemblyFilePath,
                null,
                userDefinedTypes,
                resources,
                assembly.BuildNamespaceHierarchyTree(),
                LanguageFactory.GetLanguage(CSharpVersion.V7),
                Utilities.GetMaxRelativePathLength(this.pathService.WorkingDirectory),
                true);

            return filePathsService.GetTypesToFilePathsMap()
                                   .ToDictionary(kvp => Path.Join(this.pathService.WorkingDirectory, assembly.FullName, assembly.MainModule.Name, kvp.Value), kvp => kvp.Key);
        }

        public override Task<Empty> AddResolvedAssembly(AddResolvedAssemblyRequest request, ServerCallContext context)
        {
            string normalizedFilePath = this.NormalizeFilePath(request.FilePath);

            if (!File.Exists(normalizedFilePath))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "The specified assembly path does not exist"));
            }

            AssemblyDefinition resolvedAssembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(normalizedFilePath);
            GlobalAssemblyResolver.Instance.RemoveFromFailedAssemblies(this.GetExtendedStrongName(resolvedAssembly.MainModule));

            return Task.FromResult(new Empty());
        }

        public override Task<GetMemberReferenceMetadataResponse> GetMemberReferenceMetadata(GetMemberReferenceMetadataRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.AbsoluteFilePath))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No file path provided in member definition retrieval request"));
            }

            string normalizedFilePath = this.NormalizeFilePath(request.AbsoluteFilePath);

            if (!this.decompilationContext.DecompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No type to corresponding file path"));
            }

            if (!this.decompilationContext.TryGetTypeMetadataFromCache(typeDefinition, out DecompiledTypeMetadata typeMetadata))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No metadata for the provided type was found"));
            }

            int lineIndex = request.LineNumber - 1;
            int columnIndex = request.Column - 1;
            KeyValuePair<CodeSpan, MemberReference> codeSpanToMemberReference = typeMetadata.CodeSpanToMemberReference
                .FirstOrDefault(kvp => kvp.Key.Start.Line <= lineIndex && kvp.Key.End.Line >= lineIndex &&
                    kvp.Key.Start.Column <= columnIndex && kvp.Key.End.Column >= columnIndex);

            if (codeSpanToMemberReference.Value == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No member reference found at the specified coordinates"));
            }

            MemberReference memberReference = codeSpanToMemberReference.Value;
            TypeReference typeReference = memberReference.DeclaringType;

            if (typeReference != null)
            {
                while (typeReference.DeclaringType != null)
                {
                    typeReference = typeReference.DeclaringType;
                }
            }
            else
            {
                typeReference = memberReference as TypeReference;
            }

            GetMemberReferenceMetadataResponse response = new GetMemberReferenceMetadataResponse();

            bool isCrossAssemblyReference = typeReference.Scope != null && typeReference.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference;
            if (isCrossAssemblyReference)
            {
                TypeDefinition resolvedTypeDefinition = typeReference.Resolve();
                string referencedAssemblyStrongName = resolvedTypeDefinition == null ? ((AssemblyNameReference)typeReference.Scope).FullName : resolvedTypeDefinition.Module.Assembly.FullName;
                response.ReferencedAssemblyFullName = referencedAssemblyStrongName;
                typeReference = resolvedTypeDefinition ?? throw new RpcException(new Status(StatusCode.NotFound, "Could not resolve type assembly"), new Metadata
                    {
                        { "unresolvedAssemblyName", referencedAssemblyStrongName }
                    });
            }

            if (!this.decompilationContext.TryGetTypeFilePathFromCache(typeReference, out string typeFilePath))
            {
                if (isCrossAssemblyReference)
                {
                    response.IsCrossAssemblyReference = true;

                    if (this.TryResolveTypeAssemblyFilePath(typeReference, out string referencedAssemblyPath))
                    {
                        response.ReferencedAssemblyFilePath = referencedAssemblyPath;
                    }
                    else
                    {
                        throw new RpcException(new Status(StatusCode.NotFound, "Could not resolve referenced assembly."));
                    }
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "No type to corresponding file path."));
                }
            }
            else
            {
                response.DefinitionFilePath = typeFilePath;
            }

            response.MemberName = memberReference.Name;

            if (memberReference.DeclaringType != null)
            {
                response.DeclaringTypeName = memberReference.DeclaringType.Name;
            }

            return Task.FromResult(response);
        }

        public override Task<Selection> GetMemberDefinitionPosition(GetMemberDefinitionPositionRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.AbsoluteFilePath))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No file path provided in member definition retrieval request"));
            }

            string normalizedFilePath = this.NormalizeFilePath(request.AbsoluteFilePath);

            if (!this.decompilationContext.DecompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No type to corresponding file path"));
            }

            if (!this.decompilationContext.TryGetTypeMetadataFromCache(typeDefinition, out DecompiledTypeMetadata typeMetadata))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No metadata for the provided type was found"));
            }
            else
            {
                Func<KeyValuePair<IMemberDefinition, CodeSpan>, bool> findMemberDeclaration = kvp => !string.IsNullOrEmpty(request.DeclaringTypeName) ?
                    kvp.Key.Name == request.MemberName && kvp.Key.DeclaringType?.Name == request.DeclaringTypeName :
                    kvp.Key.Name == request.MemberName;
                KeyValuePair <IMemberDefinition, CodeSpan> memberDeclarationToCodeSpan = typeMetadata.MemberDeclarationToCodeSpan.FirstOrDefault(findMemberDeclaration);
                if (memberDeclarationToCodeSpan.Key == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "No coordinates found for the supplied member reference"));
                }
                else
                {
                    Selection selection = new Selection()
                    {
                        StartLineNumber = memberDeclarationToCodeSpan.Value.Start.Line + 1,
                        StartColumn = memberDeclarationToCodeSpan.Value.Start.Column + 1,
                        EndLineNumber = memberDeclarationToCodeSpan.Value.End.Line + 1,
                        EndColumn = memberDeclarationToCodeSpan.Value.End.Column + 1
                    };

                    return Task.FromResult(selection);
                }
            }
        }

        public override Task<DecompileTypeResponse> DecompileType(DecompileTypeRequest request, ServerCallContext context)
        {
            string normalizedFilePath = this.NormalizeFilePath(request.FilePath);

            if (!this.decompilationContext.DecompilationContext.FilePathToType.ContainsKey(normalizedFilePath))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No type to corresponding file path"));
            }

            TypeDefinition type = this.decompilationContext.DecompilationContext.FilePathToType[normalizedFilePath];
            IExceptionFormatter exceptionFormatter = SimpleExceptionFormatter.Instance;
            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            StringWriter theWriter = new StringWriter();
            CodeFormatter formatter = new CodeFormatter(theWriter);
            IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true,
                                                          writeFullyQualifiedNames: false,
                                                          writeDocumentation: true,
                                                          showCompilerGeneratedMembers: false,
                                                          writeLargeNumbersInHex: false);
            ILanguageWriter writer = language.GetWriter(formatter, exceptionFormatter, settings);

            IWriterContextService writerContextService = new TypeCollisionWriterContextService(new ProjectGenerationDecompilationCacheService(), true);

            try
            {
                List<WritingInfo> infos = (writer as INamespaceLanguageWriter).WriteTypeAndNamespaces(type, writerContextService);

                DecompiledTypeMetadata decompiledTypeMetadata = new DecompiledTypeMetadata();

                decompiledTypeMetadata.CodeSpanToMemberReference.AddRange(formatter.CodeSpanToMemberReference);

                foreach (WritingInfo info in infos)
                {
                    decompiledTypeMetadata.MemberDeclarationToCodeSpan.AddRange(info.MemberDeclarationToCodeSpan);

                    decompiledTypeMetadata.CodeMappingInfo.NodeToCodeMap.AddRange(info.CodeMappingInfo.NodeToCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.InstructionToCodeMap.AddRange(info.CodeMappingInfo.InstructionToCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.FieldConstantValueToCodeMap.AddRange(info.CodeMappingInfo.FieldConstantValueToCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.VariableToCodeMap.AddRange(info.CodeMappingInfo.VariableToCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.ParameterToCodeMap.AddRange(info.CodeMappingInfo.ParameterToCodeMap);

                    decompiledTypeMetadata.CodeMappingInfo.MethodDefinitionToMethodReturnTypeCodeMap.AddRange(info.CodeMappingInfo.MethodDefinitionToMethodReturnTypeCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.FieldDefinitionToFieldTypeCodeMap.AddRange(info.CodeMappingInfo.FieldDefinitionToFieldTypeCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.PropertyDefinitionToPropertyTypeCodeMap.AddRange(info.CodeMappingInfo.PropertyDefinitionToPropertyTypeCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.EventDefinitionToEventTypeCodeMap.AddRange(info.CodeMappingInfo.EventDefinitionToEventTypeCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap.AddRange(info.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap);
                    decompiledTypeMetadata.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap.AddRange(info.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap);
                }

                this.decompilationContext.AddTypeMetadataToCache(type, decompiledTypeMetadata);

                return Task.FromResult(new DecompileTypeResponse() { SourceCode = theWriter.ToString() });
            }
            catch (Exception e)
            {
                string[] exceptionMessageLines = exceptionFormatter.Format(e, type.FullName, null);
                string exceptionMessage = string.Join(Environment.NewLine, exceptionMessageLines);
                string commentedExceptionMessage = language.CommentLines(exceptionMessage);

                return Task.FromResult(new DecompileTypeResponse() { SourceCode = commentedExceptionMessage });
            }
        }

        public override Task<GetContextAssemblyResponse> GetContextAssembly(GetContextAssemblyRequest request, ServerCallContext context)
        {
            string normalizedFilePath = this.NormalizeFilePath(request.ContextUri);

            if (!this.decompilationContext.DecompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition))
            {
                typeDefinition = this.decompilationContext.DecompilationContext.FilePathToType.FirstOrDefault(p => p.Key.StartsWith(normalizedFilePath)).Value;
            }

            if (typeDefinition == null || !this.TryResolveTypeAssemblyFilePath(typeDefinition, out string assemblyPath))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to resolve assembly from path"));
            }

            return Task.FromResult(new GetContextAssemblyResponse()
            {
                AssemblyName = typeDefinition.Module.Assembly.FullName,
                AssemblyFilePath = assemblyPath
            });
        }

        public override async Task Search(SearchRequest request, IServerStreamWriter<SearchResultResponse> responseStream, ServerCallContext context)
        {
            foreach (SearchResult searchResult in this.searchService.Search(request.Query, request.MatchCasing, request.MatchWholeWord))
            {
                int highlightStartIndex = searchResult.MatchedString.IndexOf(request.Query, StringComparison.InvariantCultureIgnoreCase);

                await responseStream.WriteAsync(new SearchResultResponse()
                {
                    Id = searchResult.Id,
                    FilePath = searchResult.DeclaringTypeFilePath,
                    Preview = searchResult.MatchedString,
                    HighlightRange = new PreviewHighlightRange()
                    {
                        StartIndex = highlightStartIndex,
                        EndIndex = highlightStartIndex + request.Query.Length
                    }
                });
            }
        }

        public override Task<Empty> CancelSearch(Empty request, ServerCallContext context)
        {
            this.searchService.CancelSearch();

            return Task.FromResult(new Empty());
        }

        public override Task<Selection> GetSearchResultPosition(GetSearchResultPositionRequest request, ServerCallContext context)
        {
            if (request.SearchResultId <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid search result id"));
            }

            CodeSpan? searchResultCodeSpan = this.searchService.GetSearchResultPosition(request.SearchResultId);

            if (!searchResultCodeSpan.HasValue)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Failed to find search result code position"));
            }

            return Task.FromResult(new Selection()
            {
                StartLineNumber = searchResultCodeSpan.Value.Start.Line + 1,
                StartColumn = searchResultCodeSpan.Value.Start.Column + 1,
                EndLineNumber = searchResultCodeSpan.Value.End.Line + 1,
                EndColumn = searchResultCodeSpan.Value.End.Column + 1
            });
        }

        public override Task<GetProjectCreationMetadataResponse> GetProjectCreationMetadata(GetProjectCreationMetadataRequest request, ServerCallContext context)
        {
            string normalizedFilePath = this.NormalizeFilePath(request.AssemblyFilePath);
            VisualStudioVersion visualStudioVersion = this.GetProjectCreationVSVersion(request.ProjectVisualStudioVersion);
            AssemblyDefinition assemblyDefinition = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(normalizedFilePath);
            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            ProjectGenerationSettings settings = ProjectGenerationSettingsProvider.GetProjectGenerationSettings(normalizedFilePath, NoCacheAssemblyInfoService.Instance,
                EmptyResolver.Instance, visualStudioVersion, language, TargetPlatformResolver.Instance);
            bool containsDangerousResources = assemblyDefinition.Modules.SelectMany(m => m.Resources).Any(r => DangerousResourceIdentifier.IsDangerousResource(r));
            string normalizedVSProjectFileExtension = language.VSProjectFileExtension.TrimStart('.');
            string generatedProjectExtension = normalizedVSProjectFileExtension + (settings.JustDecompileSupportedProjectType ? string.Empty : MSBuildProjectBuilder.ErrorFileExtension);

            return Task.FromResult(new GetProjectCreationMetadataResponse()
            {
                ContainsDangerousResources = containsDangerousResources,
                ProjectFileMetadata = new ProjectFileMetadata()
                {
                    IsDecompilerSupportedProjectType = settings.JustDecompileSupportedProjectType,
                    IsVSSupportedProjectType = settings.VisualStudioSupportedProjectType,
                    ProjectTypeNotSupportedErrorMessage = settings.ErrorMessage ?? string.Empty,
                    ProjectFileName = assemblyDefinition.Name.Name,
                    ProjectFileExtension = generatedProjectExtension
                }
            });
        }

        public override Task<CreateProjectResponse> CreateProject(CreateProjectRequest request, ServerCallContext context)
        {
            string outputPath = this.NormalizeFilePath(request.OutputPath);
            string targetPath = this.NormalizeFilePath(request.AssemblyFilePath);

            if (string.IsNullOrEmpty(outputPath) || string.IsNullOrEmpty(targetPath))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid argument"));
            }

            bool decompileDangerousResources = request.DecompileDangerousResources;
            VisualStudioVersion visualStudioVersion = this.GetProjectCreationVSVersion(request.ProjectVisualStudioVersion);
            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.V7);

            AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(targetPath);
            ProjectGenerationSettings settings = ProjectGenerationSettingsProvider.GetProjectGenerationSettings(targetPath, NoCacheAssemblyInfoService.Instance,
                EmptyResolver.Instance, visualStudioVersion, language, TargetPlatformResolver.Instance);

            DecompilationPreferences preferences = new DecompilationPreferences()
            {
                WriteFullNames = false,
                WriteDocumentation = true,
                RenameInvalidMembers = true,
                WriteLargeNumbersInHex = false,
                DecompileDangerousResources = decompileDangerousResources
            };

            BaseProjectBuilder projectBuilder = this.GetProjectBuilder(assembly, targetPath, visualStudioVersion, settings, language, outputPath, preferences, EmptyResolver.Instance, TargetPlatformResolver.Instance);
            string generationErrorMessage = this.CreateProject(projectBuilder);

            return Task.FromResult(new CreateProjectResponse() { ErrorMessage = generationErrorMessage });
        }

        private string CreateProject(BaseProjectBuilder projectBuilder)
        {
            StringBuilder builder = new StringBuilder();

            BaseProjectBuilder.ProjectGenerationFailureEventHandler OnProjectGenerationFailure = (s, e) =>
            {
                builder.AppendLine(e.Message);
            };
            projectBuilder.ProjectGenerationFailure += OnProjectGenerationFailure;

            projectBuilder.BuildProject();

            projectBuilder.ProjectGenerationFailure -= OnProjectGenerationFailure;

            return builder.ToString();
        }

        private VisualStudioVersion GetProjectCreationVSVersion(string version)
        {
            switch (version)
            {
                case "2010":
                    return VisualStudioVersion.VS2010;
                case "2012":
                    return VisualStudioVersion.VS2012;
                case "2013":
                    return VisualStudioVersion.VS2013;
                case "2015":
                    return VisualStudioVersion.VS2015;
                case "2017":
                    return VisualStudioVersion.VS2017;
                default:
                    return VisualStudioVersion.VS2019;
            }
        }

        public override Task<Empty> ClearAssemblyList(Empty request, ServerCallContext context)
        {
            GlobalAssemblyResolver.Instance.ClearCache();
            this.decompilationContext.ClearMetadataCache();

            return Task.FromResult(new Empty());
        }

        private bool TryResolveTypeAssemblyFilePath(TypeReference typeReference, out string assemblyFilePath)
        {
            ModuleDefinition moduleDefinition = typeReference.Module;
            AssemblyNameReference assemblyNameReference = typeReference.Module.Assembly.Name;
            AssemblyStrongNameExtended assemblyKey = this.GetExtendedStrongName(moduleDefinition);
            AssemblyName assemblyName = new AssemblyName(assemblyNameReference.Name,
                                                                assemblyNameReference.FullName,
                                                                assemblyNameReference.Version,
                                                                assemblyNameReference.PublicKeyToken)
            {
                TargetArchitecture = moduleDefinition.GetModuleArchitecture()
            };

            assemblyFilePath = moduleDefinition.AssemblyResolver.FindAssemblyPath(assemblyName, null, assemblyKey);

            return !string.IsNullOrEmpty(assemblyFilePath);
        }

        private AssemblyStrongNameExtended GetExtendedStrongName(ModuleDefinition moduleDefinition)
        {
            AssemblyNameReference assemblyNameReference = moduleDefinition.Assembly.Name;
            SpecialTypeAssembly special = moduleDefinition.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;

            return new AssemblyStrongNameExtended(assemblyNameReference.FullName, moduleDefinition.GetModuleArchitecture(), special);
        }

        private string NormalizeFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            if (SystemInformation.IsWindows && Path.IsPathFullyQualified(filePath))
            {
                return char.ToUpper(filePath[0]) + filePath.Substring(1);
            }

            return filePath;
        }

        private BaseProjectBuilder GetProjectBuilder(AssemblyDefinition assembly, string targetPath, VisualStudioVersion visualStudioVersion, ProjectGenerationSettings settings, ILanguage language, string projFilePath, DecompilationPreferences preferences, IFrameworkResolver frameworkResolver, ITargetPlatformResolver targetPlatformResolver)
        {
            TargetPlatform targetPlatform = targetPlatformResolver.GetTargetPlatform(assembly.MainModule.FilePath, assembly.MainModule);
            BaseProjectBuilder projectBuilder;

            if (targetPlatform == TargetPlatform.NetCore)
            {
                projectBuilder = new NetCoreProjectBuilder(targetPath, projFilePath, language, preferences, null, NoCacheAssemblyInfoService.Instance, visualStudioVersion, settings);
            }
            else if (targetPlatform == TargetPlatform.WinRT)
            {
                projectBuilder = new WinRTProjectBuilder(targetPath, projFilePath, language, preferences, null, NoCacheAssemblyInfoService.Instance, visualStudioVersion, settings);
            }
            else
            {
                projectBuilder = new MSBuildProjectBuilder(targetPath, projFilePath, language, frameworkResolver, preferences, null, NoCacheAssemblyInfoService.Instance, visualStudioVersion, settings);
            }

            return projectBuilder;
        }
    }
}
