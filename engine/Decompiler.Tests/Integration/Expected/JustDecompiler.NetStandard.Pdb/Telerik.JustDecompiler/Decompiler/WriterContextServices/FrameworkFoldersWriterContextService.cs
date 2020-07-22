using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class FrameworkFoldersWriterContextService : BaseWriterContextService
	{
		public FrameworkFoldersWriterContextService(IDecompilationCacheService cacheService)
		{
			base(cacheService, false);
			return;
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			return new AssemblySpecificContext();
		}

		private DecompiledType GetDecompiledType(IMemberDefinition member, ILanguage language)
		{
			if (member as TypeDefinition != null)
			{
				stackVariable3 = member as TypeDefinition;
			}
			else
			{
				stackVariable3 = member.get_DeclaringType();
			}
			V_0 = new DecompiledType(stackVariable3);
			if (member as MethodDefinition == null)
			{
				if (member as PropertyDefinition == null)
				{
					throw new NotSupportedException("FrameworkFolderWriterContext service supports only methods and properties.");
				}
				V_3 = member as PropertyDefinition;
				if (V_3.get_GetMethod() != null)
				{
					V_4 = Utilities.TryGetDecompiledMember(V_3.get_GetMethod(), V_0.get_TypeContext(), language);
					V_0.get_DecompiledMembers().Add(V_3.get_GetMethod().get_FullName(), V_4);
				}
				if (V_3.get_SetMethod() != null)
				{
					V_5 = Utilities.TryGetDecompiledMember(V_3.get_SetMethod(), V_0.get_TypeContext(), language);
					V_0.get_DecompiledMembers().Add(V_3.get_SetMethod().get_FullName(), V_5);
				}
			}
			else
			{
				V_1 = member as MethodDefinition;
				V_2 = Utilities.TryGetDecompiledMember(V_1, V_0.get_TypeContext(), language);
				V_0.get_DecompiledMembers().Add(V_1.get_FullName(), V_2);
			}
			this.AddGeneratedFilterMethodsToDecompiledType(V_0, V_0.get_TypeContext(), language);
			return V_0;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			return new ModuleSpecificContext();
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			V_0 = new AssemblySpecificContext();
			V_1 = new ModuleSpecificContext();
			V_2 = new TypeSpecificContext(Utilities.GetDeclaringTypeOrSelf(member));
			stackVariable8 = this.GetDecompiledType(member, language);
			V_3 = new Dictionary<string, MethodSpecificContext>();
			V_4 = new Dictionary<string, Statement>();
			V_5 = stackVariable8.get_DecompiledMembers().GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					V_3.Add(V_6.get_Key(), V_6.get_Value().get_Context());
					V_4.Add(V_6.get_Key(), V_6.get_Value().get_Statement());
				}
			}
			finally
			{
				((IDisposable)V_5).Dispose();
			}
			return new WriterContext(V_0, V_1, V_2, V_3, V_4);
		}
	}
}