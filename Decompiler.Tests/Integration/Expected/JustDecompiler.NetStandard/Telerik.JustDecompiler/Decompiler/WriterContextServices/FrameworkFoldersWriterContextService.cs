using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class FrameworkFoldersWriterContextService : BaseWriterContextService
	{
		public FrameworkFoldersWriterContextService(IDecompilationCacheService cacheService) : base(cacheService, false)
		{
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			return new AssemblySpecificContext();
		}

		private DecompiledType GetDecompiledType(IMemberDefinition member, ILanguage language)
		{
			DecompiledType decompiledType = new DecompiledType((member is TypeDefinition ? member as TypeDefinition : member.DeclaringType));
			if (!(member is MethodDefinition))
			{
				if (!(member is PropertyDefinition))
				{
					throw new NotSupportedException("FrameworkFolderWriterContext service supports only methods and properties.");
				}
				PropertyDefinition propertyDefinition = member as PropertyDefinition;
				if (propertyDefinition.GetMethod != null)
				{
					DecompiledMember decompiledMember = Utilities.TryGetDecompiledMember(propertyDefinition.GetMethod, decompiledType.TypeContext, language);
					decompiledType.DecompiledMembers.Add(propertyDefinition.GetMethod.FullName, decompiledMember);
				}
				if (propertyDefinition.SetMethod != null)
				{
					DecompiledMember decompiledMember1 = Utilities.TryGetDecompiledMember(propertyDefinition.SetMethod, decompiledType.TypeContext, language);
					decompiledType.DecompiledMembers.Add(propertyDefinition.SetMethod.FullName, decompiledMember1);
				}
			}
			else
			{
				MethodDefinition methodDefinition = member as MethodDefinition;
				DecompiledMember decompiledMember2 = Utilities.TryGetDecompiledMember(methodDefinition, decompiledType.TypeContext, language);
				decompiledType.DecompiledMembers.Add(methodDefinition.FullName, decompiledMember2);
			}
			base.AddGeneratedFilterMethodsToDecompiledType(decompiledType, decompiledType.TypeContext, language);
			return decompiledType;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			return new ModuleSpecificContext();
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			AssemblySpecificContext assemblySpecificContext = new AssemblySpecificContext();
			ModuleSpecificContext moduleSpecificContext = new ModuleSpecificContext();
			TypeSpecificContext typeSpecificContext = new TypeSpecificContext(Utilities.GetDeclaringTypeOrSelf(member));
			DecompiledType decompiledType = this.GetDecompiledType(member, language);
			Dictionary<string, MethodSpecificContext> strs = new Dictionary<string, MethodSpecificContext>();
			Dictionary<string, Statement> strs1 = new Dictionary<string, Statement>();
			foreach (KeyValuePair<string, DecompiledMember> decompiledMember in decompiledType.DecompiledMembers)
			{
				strs.Add(decompiledMember.Key, decompiledMember.Value.Context);
				strs1.Add(decompiledMember.Key, decompiledMember.Value.Statement);
			}
			return new WriterContext(assemblySpecificContext, moduleSpecificContext, typeSpecificContext, strs, strs1);
		}
	}
}