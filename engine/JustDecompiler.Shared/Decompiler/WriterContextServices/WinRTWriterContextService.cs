using System;
using System.Linq;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
    public class WinRTWriterContextService : TypeCollisionWriterContextService
    {
        public WinRTWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
            : base(cacheService, renameInvalidMembers)
        {
        }

        protected override MemberRenamingServices.MemberRenamingData GetMemberRenamingData(Mono.Cecil.ModuleDefinition module, Languages.ILanguage language)
        {
            return new WinRTRenamingService(language, this.renameInvalidMembers).GetMemberRenamingData(module);
        }

        protected override TypeSpecificContext GetTypeContext(Mono.Cecil.TypeDefinition type, Languages.ILanguage language, System.Collections.Generic.Dictionary<string, DecompiledType> decompiledTypes)
        {
            TypeSpecificContext typeContext = base.GetTypeContext(type, language, decompiledTypes);
            if (!typeContext.IsWinRTImplementation && typeContext.CurrentType.IsNotPublic && typeContext.CurrentType.IsSealed && typeContext.CurrentType.Name.StartsWith("<CLR>"))
            {
                typeContext.IsWinRTImplementation = true;
            }

            return typeContext;
        }
    }
}
