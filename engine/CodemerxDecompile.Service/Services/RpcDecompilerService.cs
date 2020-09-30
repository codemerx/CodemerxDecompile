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
using System.Threading.Tasks;
using CodemerxDecompile.Service.Extensions;
using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services;
using Grpc.Core;

using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using CodemerxDecompile.Service.Services.DecompilationContext.Models;

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
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(request.AssemblyPath);

            GetAssemblyRelatedFilePathsResponse response = new GetAssemblyRelatedFilePathsResponse()
            {
                DecompiledAssemblyDirectory = AssembliesDirectory,
                DecompiledAssemblyPath = Path.Join(AssembliesDirectory, assembly.FullName)
            };

            return Task.FromResult(response);
        }

        public override Task<GetAllTypeFilePathsResponse> GetAllTypeFilePaths(GetAllTypeFilePathsRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = ExternallyVisibleDecompilationUtilities.ResolveAssembly(new AssemblyIdentifier(request.AssemblyPath));
            //SpecialTypeAssembly special = assembly.MainModule.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;
            //IAssemblyResolver assemblyResolver = assembly.MainModule.AssemblyResolver;
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
            //ICollection<AssemblyNameReference> assemblyReferencesNames = this.GetAssembliesDependingOn(assembly.MainModule, userDefinedTypes);

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

        public override async Task Search(SearchRequest request, IServerStreamWriter<SearchResult> responseStream, ServerCallContext context)
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(200);
                await responseStream.WriteAsync(new SearchResult()
                {
                    FilePath = @"C:\Users\User\AppData\Local\Temp\CD\JustDecompiler, Version = 2019.1.118.0, Culture = neutral, PublicKeyToken = null\JustDecompiler.dll\Mono.Cecil\GenericHelper.cs",
                    Preview = $"Result #{i + 1}"
                });
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
                                                                assemblyNameReference.PublicKeyToken);

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

        private ICollection<TypeReference> GetExpandedTypeDependanceList(ModuleDefinition module, Dictionary<ModuleDefinition, Collection<TypeDefinition>> userDefinedTypes)
        {
            Mono.Collections.Generic.Collection<TypeDefinition> moduleTypes;
            if (!userDefinedTypes.TryGetValue(module, out moduleTypes))
            {
                throw new Exception("Module types not found.");
            }

            HashSet<TypeReference> firstLevelDependanceTypes = new HashSet<TypeReference>();
            foreach (TypeReference type in moduleTypes)
            {
                if (!firstLevelDependanceTypes.Contains(type))
                {
                    firstLevelDependanceTypes.Add(type);
                }
            }

            foreach (TypeReference type in module.GetTypeReferences())
            {
                if (!firstLevelDependanceTypes.Contains(type))
                {
                    firstLevelDependanceTypes.Add(type);
                }
            }

            return Telerik.JustDecompiler.Decompiler.Utilities.GetExpandedTypeDependanceList(firstLevelDependanceTypes);
        }

        private ICollection<AssemblyNameReference> GetAssembliesDependingOn(ModuleDefinition module, Dictionary<ModuleDefinition, Collection<TypeDefinition>> userDefinedTypes)
        {
            ICollection<AssemblyNameReference> result;

            ICollection<TypeReference> expadendTypeDependanceList = this.GetExpandedTypeDependanceList(module, userDefinedTypes);
            result = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembliesDependingOn(module, expadendTypeDependanceList);

            foreach (AssemblyNameReference assemblyReference in module.AssemblyReferences)
            {
                if (!result.Contains(assemblyReference))
                {
                    result.Add(assemblyReference);
                }
            }

            return result;
        }
    }
}
