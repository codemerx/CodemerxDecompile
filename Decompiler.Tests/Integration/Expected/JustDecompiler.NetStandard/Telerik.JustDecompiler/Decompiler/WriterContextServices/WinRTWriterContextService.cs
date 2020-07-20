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
		public WinRTWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
		{
			base(cacheService, renameInvalidMembers);
			return;
		}

		protected override MemberRenamingData GetMemberRenamingData(ModuleDefinition module, ILanguage language)
		{
			return (new WinRTRenamingService(language, this.renameInvalidMembers)).GetMemberRenamingData(module);
		}

		protected override TypeSpecificContext GetTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			V_0 = this.GetTypeContext(type, language, decompiledTypes);
			if (!V_0.get_IsWinRTImplementation() && V_0.get_CurrentType().get_IsNotPublic() && V_0.get_CurrentType().get_IsSealed() && V_0.get_CurrentType().get_Name().StartsWith("<CLR>"))
			{
				V_0.set_IsWinRTImplementation(true);
			}
			return V_0;
		}
	}
}