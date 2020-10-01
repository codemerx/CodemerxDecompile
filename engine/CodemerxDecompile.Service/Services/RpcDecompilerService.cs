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
        private readonly IDecompilationContext decompilationContext;
        private readonly string AssembliesDirectory = Path.Join(Path.GetTempPath(), "CD");

        public RpcDecompilerService(IDecompilationContext decompilationContext)
        {
            this.decompilationContext = decompilationContext;
        }

        public override Task<GetAssemblyRelatedFilePathsResponse> GetAssemblyRelatedFilePaths(GetAssemblyRelatedFilePathsRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(request.AssemblyPath);

            GetAssemblyRelatedFilePathsResponse response = new GetAssemblyRelatedFilePathsResponse()
            {
                DecompiledAssemblyDirectory = AssembliesDirectory,
                DecompiledAssemblyPath = Path.Join(AssembliesDirectory, assembly.FullName)
            };

            return Task.FromResult(response);
        }

        public override Task<GetAllTypeFilePathsResponse> GetAllTypeFilePaths(GetAllTypeFilePathsRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(request.AssemblyPath);
            Dictionary<ModuleDefinition, Collection<TypeDefinition>> userDefinedTypes = Utilities.GetUserDefinedTypes(assembly, true);
            Dictionary<ModuleDefinition, Collection<Resource>> resources = Utilities.GetResources(assembly);
            DefaultFilePathsService filePathsService = new DefaultFilePathsService(
                assembly,
                request.AssemblyPath,
                null,
                userDefinedTypes,
                resources,
                assembly.BuildNamespaceHierarchyTree(),
                LanguageFactory.GetLanguage(CSharpVersion.V7),
                Utilities.GetMaxRelativePathLength(AssembliesDirectory),
                true);

            Dictionary<string, TypeDefinition> filePathToTypeDefinition = filePathsService.GetTypesToFilePathsMap()
                                                                                          .ToDictionary(kvp => Path.Join(AssembliesDirectory, assembly.FullName, assembly.MainModule.Name, kvp.Value), kvp => kvp.Key);

            this.decompilationContext.FilePathToType.AddRange(filePathToTypeDefinition);

            GetAllTypeFilePathsResponse response = new GetAllTypeFilePathsResponse();
            response.TypeFilePaths.AddRange(filePathToTypeDefinition.Keys);

            return Task.FromResult(response);
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

            if (!this.decompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition))
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

            response.MemberFullName = memberReference.FullName;

            return Task.FromResult(response);
        }

        public override Task<Selection> GetMemberDefinitionPosition(GetMemberDefinitionPositionRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.AbsoluteFilePath))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No file path provided in member definition retrieval request"));
            }

            string normalizedFilePath = this.NormalizeFilePath(request.AbsoluteFilePath);

            if (!this.decompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No type to corresponding file path"));
            }

            if (!this.decompilationContext.TryGetTypeMetadataFromCache(typeDefinition, out DecompiledTypeMetadata typeMetadata))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No metadata for the provided type was found"));
            }
            else
            {
                KeyValuePair<IMemberDefinition, CodeSpan> memberDeclarationToCodeSpan = typeMetadata.MemberDeclarationToCodeSpan.FirstOrDefault(kvp => kvp.Key.FullName == request.MemberFullName);
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

            if (!this.decompilationContext.FilePathToType.ContainsKey(normalizedFilePath))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No type to corresponding file path"));
            }

            TypeDefinition type = this.decompilationContext.FilePathToType[normalizedFilePath];
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

                Dictionary<IMemberDefinition, CodeSpan> memberDeclarationToCodeSpan = new Dictionary<IMemberDefinition, CodeSpan>();

                foreach (WritingInfo info in infos)
                {
                    memberDeclarationToCodeSpan.AddRange(info.MemberDeclarationToCodeSpan);
                }

                this.decompilationContext.AddTypeMetadataToCache(type, memberDeclarationToCodeSpan, formatter.CodeSpanToMemberReference);

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

        public override async Task Search(SearchRequest request, IServerStreamWriter<SearchResultResponse> responseStream, ServerCallContext context)
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(200);
                await responseStream.WriteAsync(new SearchResultResponse()
                {
                    FilePath = @"C:\Users\User\AppData\Local\Temp\CD\JustDecompiler, Version = 2019.1.118.0, Culture = neutral, PublicKeyToken = null\JustDecompiler.dll\Mono.Cecil\GenericHelper.cs",
                    Preview = $"Result #{i + 1}"
                });
            }
        }

        public override Task<GetProjectCreationMetadataFromTypeFilePathResponse> GetProjectCreationMetadataFromTypeFilePath(GetProjectCreationMetadataFromTypeFilePathRequest request, ServerCallContext context)
        {
            string normalizedFilePath = this.NormalizeFilePath(request.TypeFilePath);

            if (!this.decompilationContext.FilePathToType.TryGetValue(normalizedFilePath, out TypeDefinition typeDefinition))
            {
                typeDefinition = this.decompilationContext.FilePathToType.FirstOrDefault(p => p.Key.StartsWith(normalizedFilePath)).Value;
            }

            if (typeDefinition == null || !this.TryResolveTypeAssemblyFilePath(typeDefinition, out string assemblyPath))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Failed to resolve assembly from path"));
            }

            VisualStudioVersion visualStudioVersion = this.GetProjectCreationVSVersion(request.ProjectVisualStudioVersion);

            AssemblyDefinition assemblyDefinition = typeDefinition.Module.Assembly;
            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            ProjectGenerationSettings settings = ProjectGenerationSettingsProvider.GetProjectGenerationSettings(assemblyPath, NoCacheAssemblyInfoService.Instance,
                EmptyResolver.Instance, visualStudioVersion, language, TargetPlatformResolver.Instance);
            bool containsDangerousResources = assemblyDefinition.Modules.SelectMany(m => m.Resources).Any(r => DangerousResourceIdentifier.IsDangerousResource(r));
            string normalizedVSProjectFileExtension = language.VSProjectFileExtension.TrimStart('.');
            string generatedProjectExtension = normalizedVSProjectFileExtension + (settings.JustDecompileSupportedProjectType ? string.Empty : MSBuildProjectBuilder.ErrorFileExtension);

            return Task.FromResult(new GetProjectCreationMetadataFromTypeFilePathResponse()
            {
                AssemblyFilePath = assemblyPath,
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
                WriteLargeNumbersInHex = true,
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
                default:
                    return VisualStudioVersion.VS2017;
            }
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
