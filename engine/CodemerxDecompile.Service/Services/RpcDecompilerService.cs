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

        public override Task<GetMemberDefinitionResponse> GetMemberDefinition(GetMemberDefinitionRequest request, ServerCallContext context)
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

            KeyValuePair<CodeSpan, MemberReference> codeSpanToMemberReference = typeMetadata.CodeSpanToMemberReference
                .FirstOrDefault(kvp => kvp.Key.Start.Line <= request.LineNumber && kvp.Key.End.Line >= request.LineNumber &&
                    kvp.Key.Start.Column < request.ColumnIndex && kvp.Key.End.Column > request.ColumnIndex);

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

            GetMemberDefinitionResponse response = new GetMemberDefinitionResponse();

            if (!this.decompilationContext.TryGetTypeFilePathFromCache(typeReference, out string typeFilePath))
            {
                if (typeReference.Scope != null && typeReference.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference)
                {
                    response.IsCrossAssemblyReference = true;

                    if (this.TryResolveTypeAssemblyFilePath(typeReference, out string referencedAssemblyPath))
                    {
                        response.AssemblyReferenceFilePath = referencedAssemblyPath;
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
                response.NavigationFilePath = typeFilePath;
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
                        StartColumnIndex = memberDeclarationToCodeSpan.Value.Start.Column + 1,
                        EndLineNumber = memberDeclarationToCodeSpan.Value.End.Line + 1,
                        EndColumnIndex = memberDeclarationToCodeSpan.Value.End.Column + 1
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

        private bool TryResolveTypeAssemblyFilePath(TypeReference typeReference, out string assemblyFilePath)
        {
            ModuleDefinition moduleDefinition = typeReference.Module;
            AssemblyNameReference assemblyNameReference = typeReference.Scope as AssemblyNameReference;

            if (assemblyNameReference == null)
            {
                assemblyFilePath = null;
                return false;
            }

            SpecialTypeAssembly special = moduleDefinition.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;

            AssemblyName assemblyName = new AssemblyName(assemblyNameReference.Name,
                                                                assemblyNameReference.FullName,
                                                                assemblyNameReference.Version,
                                                                assemblyNameReference.PublicKeyToken);
            AssemblyStrongNameExtended assemblyKey = new AssemblyStrongNameExtended(assemblyName.FullName, moduleDefinition.Architecture, special);

            assemblyFilePath = moduleDefinition.AssemblyResolver.FindAssemblyPath(assemblyName, null, assemblyKey);

            return File.Exists(assemblyFilePath);
        }

        private string NormalizeFilePath(string filePath)
        {
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            if (isWindows && Path.IsPathFullyQualified(filePath))
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
