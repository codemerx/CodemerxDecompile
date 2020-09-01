using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;

using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace CodemerxDecompile.Service
{
    public class RpcDecompilerService : RpcDecompiler.RpcDecompilerBase
    {
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

            Dictionary<TypeDefinition, string> typeToFilePathMap = filePathsService.GetTypesToFilePathsMap();

            GetAllTypeFilePathsResponse response = new GetAllTypeFilePathsResponse();
            response.TypeFilePaths.AddRange(typeToFilePathMap.Select(pair =>
            {
                TypeFilePath result = new TypeFilePath();
                result.TypeFullName = pair.Key.FullName;
                result.RelativeFilePath = pair.Value;

                return result;
            }));

            return Task.FromResult(response);
        }

        public override Task<DecompileTypeResponse> DecompileType(DecompileTypeRequest request, ServerCallContext context)
        {
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(request.AssemblyPath);
            TypeDefinition type = assembly.MainModule.GetType(request.TypeFullName);
            IExceptionFormatter exceptionFormatter = SimpleExceptionFormatter.Instance;
            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            StringWriter theWriter = new StringWriter();
            IFormatter formatter = new PlainTextFormatter(theWriter);
            IWriterSettings settings = new WriterSettings(writeExceptionsAsComments: true,
                                                          writeFullyQualifiedNames: false,
                                                          writeDocumentation: true,
                                                          showCompilerGeneratedMembers: false,
                                                          writeLargeNumbersInHex: false);
            ILanguageWriter writer = language.GetWriter(formatter, exceptionFormatter, settings);

            IWriterContextService writerContextService = new TypeCollisionWriterContextService(new ProjectGenerationDecompilationCacheService(), true);

            try
            {
                (writer as INamespaceLanguageWriter).WriteTypeAndNamespaces(type, writerContextService);

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
    }
}
