using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class WinRTWriterContextService : TypeCollisionWriterContextService
	{
		public WinRTWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers) : base(cacheService, renameInvalidMembers)
		{
		}

		protected override MemberRenamingData GetMemberRenamingData(ModuleDefinition module, ILanguage language)
		{
			return (new WinRTRenamingService(language, this.renameInvalidMembers)).GetMemberRenamingData(module);
		}

		protected override TypeSpecificContext GetTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			TypeSpecificContext typeContext = base.GetTypeContext(type, language, decompiledTypes);
			if (!typeContext.IsWinRTImplementation && typeContext.CurrentType.get_IsNotPublic() && typeContext.CurrentType.get_IsSealed() && typeContext.CurrentType.get_Name().StartsWith("<CLR>"))
			{
				typeContext.IsWinRTImplementation = true;
			}
			return typeContext;
		}
	}
}