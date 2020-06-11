using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler.Caching;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class FrameworkFoldersWriterContextService : BaseWriterContextService
	{
		public FrameworkFoldersWriterContextService(IDecompilationCacheService cacheService) : base(cacheService, false) { }

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			AssemblySpecificContext assemblyContext = new AssemblySpecificContext();
			ModuleSpecificContext moduleContext = new ModuleSpecificContext();
			TypeSpecificContext typeContext = new TypeSpecificContext(Utilities.GetDeclaringTypeOrSelf(member));

			DecompiledType decompiledType = GetDecompiledType(member, language);

			Dictionary<string, MethodSpecificContext> methodContexts = new Dictionary<string, MethodSpecificContext>();
			Dictionary<string, Statement> decompiledStatements = new Dictionary<string, Statement>();

			foreach (KeyValuePair<string, DecompiledMember> decompiledPair in decompiledType.DecompiledMembers)
			{
				methodContexts.Add(decompiledPair.Key, decompiledPair.Value.Context);
				decompiledStatements.Add(decompiledPair.Key, decompiledPair.Value.Statement);
			}

			WriterContext writerContext = new WriterContext(assemblyContext, moduleContext, typeContext, methodContexts, decompiledStatements);

			return writerContext;
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			return new AssemblySpecificContext();
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			return new ModuleSpecificContext();
		}

		private DecompiledType GetDecompiledType(IMemberDefinition member, ILanguage language)
		{
			TypeDefinition declaringType = member is TypeDefinition ? member as TypeDefinition : member.DeclaringType;
			DecompiledType decompiledType = new DecompiledType(declaringType);

			if (member is MethodDefinition)
			{
				MethodDefinition method = member as MethodDefinition;
					
				DecompiledMember decompiledMember = Utilities.TryGetDecompiledMember(method, decompiledType.TypeContext, language);
				decompiledType.DecompiledMembers.Add(method.FullName, decompiledMember);
			}
			else if (member is PropertyDefinition)
			{
				PropertyDefinition propertyDefinition = (member as PropertyDefinition);

				if (propertyDefinition.GetMethod != null)
				{
					DecompiledMember decompiledMember = Utilities.TryGetDecompiledMember(propertyDefinition.GetMethod, decompiledType.TypeContext, language);
					decompiledType.DecompiledMembers.Add(propertyDefinition.GetMethod.FullName, decompiledMember);
				}

				if (propertyDefinition.SetMethod != null)
				{
					DecompiledMember decompiledMember = Utilities.TryGetDecompiledMember(propertyDefinition.SetMethod, decompiledType.TypeContext, language);
					decompiledType.DecompiledMembers.Add(propertyDefinition.SetMethod.FullName, decompiledMember);
				}
			}
			else
			{
				throw new NotSupportedException("FrameworkFolderWriterContext service supports only methods and properties.");
			}

            AddGeneratedFilterMethodsToDecompiledType(decompiledType, decompiledType.TypeContext, language);

			return decompiledType;
		}

	}
}
