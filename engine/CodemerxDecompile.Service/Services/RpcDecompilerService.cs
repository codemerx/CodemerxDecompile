using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services;
using Grpc.Core;

using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace CodemerxDecompile.Service
{
    public class RpcDecompilerService : RpcDecompiler.RpcDecompilerBase
    {
        private readonly IDecompilationContext decompilationContext;

        public RpcDecompilerService(IDecompilationContext decompilationContext)
        {
            this.decompilationContext = decompilationContext;
        }

        public override Task<GetAssemblyMetadataResponse> GetAssemblyMetadata(GetAssemblyMetadataRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(request.AssemblyPath);

            GetAssemblyMetadataResponse response = new GetAssemblyMetadataResponse();
            response.StrongName = assembly.FullName;
            response.MainModuleName = assembly.MainModule.Name;

            return Task.FromResult(response);
        }

        public override Task<GetAllTypeFilePathsResponse> GetAllTypeFilePaths(GetAllTypeFilePathsRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(request.AssemblyPath);
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
                Utilities.GetMaxRelativePathLength(request.TargetPath),
                true);

            this.decompilationContext.TypeToFilePathMap = filePathsService.GetTypesToFilePathsMap()
                                                                          .ToDictionary(kvp => kvp.Key, kvp => Path.Join(assembly.FullName, assembly.MainModule.Name, kvp.Value));

            GetAllTypeFilePathsResponse response = new GetAllTypeFilePathsResponse();
            response.TypeFilePaths.AddRange(this.decompilationContext.TypeToFilePathMap.Select(pair =>
            {
                TypeFilePath result = new TypeFilePath();
                result.TypeFullName = pair.Key.FullName;
                result.RelativeFilePath = pair.Value;

                return result;
            }));

            return Task.FromResult(response);
        }

        public override Task<GetMemberDefinitionResponse> GetMemberDefinition(GetMemberDefinitionRequest request, ServerCallContext context)
        {
            GetMemberDefinitionResponse response = new GetMemberDefinitionResponse();

            if (!this.decompilationContext.CodeSpanToMemberReference.ContainsKey(request.FilePath))
            {
                return Task.FromResult(response);
            }

            KeyValuePair<CodeSpan, MemberReference> entry = this.decompilationContext.CodeSpanToMemberReference[request.FilePath]
                .FirstOrDefault(kvp => kvp.Key.Start.Line <= request.LineNumber && kvp.Key.End.Line >= request.LineNumber &&
                    kvp.Key.Start.Column < request.ColumnIndex && kvp.Key.End.Column > request.ColumnIndex);

            if (entry.Value == null)
            {
                return Task.FromResult(response);
            }

            MemberReference reference = entry.Value;
            TypeReference typeRef = reference.DeclaringType;
            string typeDefFullName = string.Empty;

            if (typeRef != null)
            {
                while (typeRef.DeclaringType != null)
                {
                    typeRef = typeRef.DeclaringType;
                }

                typeDefFullName = typeRef.FullName;
            }
            else
            {
                typeDefFullName = reference.FullName;
            }

            KeyValuePair<TypeDefinition, string> typeDefKvp = this.decompilationContext.TypeToFilePathMap.FirstOrDefault(kvp => kvp.Key.FullName == typeDefFullName);
            string typeDefinitionFilePath = typeDefKvp.Value;

            response.Filepath = typeDefinitionFilePath;
            response.MemberFullName = reference.FullName;

            return Task.FromResult(response);
        }

        public override Task<Selection> GetMemberDefinitionPosition(GetMemberDefinitionPositionRequest request, ServerCallContext context)
        {
            Selection selection = new Selection();

            if (!this.decompilationContext.MemberDeclarationToCodeSpan.ContainsKey(request.Filepath))
            {
                return Task.FromResult(selection);
            }

            KeyValuePair<IMemberDefinition, CodeSpan> entry = this.decompilationContext.MemberDeclarationToCodeSpan[request.Filepath].FirstOrDefault(kvp => kvp.Key.FullName == request.MemberFullName);
            if (entry.Key == null)
            {
                return Task.FromResult(selection);
            }
            else
            {
                selection.StartLineNumber = entry.Value.Start.Line + 1;
                selection.StartColumnIndex = entry.Value.Start.Column + 1;
                selection.EndLineNumber = entry.Value.End.Line + 1;
                selection.EndColumnIndex = entry.Value.End.Column + 1;

                return Task.FromResult(selection);
            }
        }

        public override Task<DecompileTypeResponse> DecompileType(DecompileTypeRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(request.AssemblyPath);
            TypeDefinition type = assembly.MainModule.GetType(request.TypeFullName);
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

                Dictionary<IMemberDefinition, CodeSpan> mapping = new Dictionary<IMemberDefinition, CodeSpan>();

                foreach (WritingInfo info in infos)
                {
                    this.AddRange(mapping, info.MemberDeclarationToCodeSpan);
                }

                KeyValuePair<TypeDefinition, string> kvp = this.decompilationContext.TypeToFilePathMap.Where(kvp => kvp.Key.FullName == type.FullName).FirstOrDefault();

                if (kvp.Value != null)
                {
                    this.decompilationContext.MemberDeclarationToCodeSpan.Add(kvp.Value, mapping);
                    this.decompilationContext.CodeSpanToMemberReference.Add(kvp.Value, formatter.CodeSpanToMemberReference);
                }

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

        private void AddRange(Dictionary<IMemberDefinition, CodeSpan> source, Dictionary<IMemberDefinition, CodeSpan> target)
        {
            foreach (var item in target)
            {
                source[item.Key] = item.Value;
            }
        }
    }
}
