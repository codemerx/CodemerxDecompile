using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class SimpleWriterContextService : BaseWriterContextService
	{
		public SimpleWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers) : base(cacheService, renameInvalidMembers) 
		{ }

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
            TypeSpecificContext cachedTypeContext;
			DecompiledType decompiledType;

			if (member is TypeDefinition && member == Utilities.GetOuterMostDeclaringType(member))
			{
				TypeDefinition type = member as TypeDefinition;
				Dictionary<string, DecompiledType> decompiledTypes = GetNestedDecompiledTypes(type, language);

                cachedTypeContext = GetTypeContext(type, language, decompiledTypes);

                AddTypeContextsToCache(decompiledTypes, type, language);

                if (!decompiledTypes.TryGetValue(type.FullName, out decompiledType))
                {
                    throw new Exception("Decompiled type not found in decompiled types cache.");
                }
			}
			else
			{
				decompiledType = GetDecompiledType(member, language);
                cachedTypeContext = GetTypeContext(decompiledType, language);
            }

            TypeSpecificContext typeContext = new TypeSpecificContext(
                cachedTypeContext.CurrentType,
                cachedTypeContext.MethodDefinitionToNameMap,
                cachedTypeContext.BackingFieldToNameMap,
                cachedTypeContext.UsedNamespaces,
                new HashSet<string>(),
                cachedTypeContext.AssignmentData,
                cachedTypeContext.AutoImplementedProperties,
                cachedTypeContext.AutoImplementedEvents,
                cachedTypeContext.ExplicitlyImplementedMembers,
                cachedTypeContext.ExceptionWhileDecompiling,
                cachedTypeContext.GeneratedFilterMethods,
                cachedTypeContext.GeneratedMethodDefinitionToNameMap
            );

            // If members were taken from the cache, generated filter methods must be added to decompiled type.
            if (typeContext.GeneratedFilterMethods.Count > 0)
            {
                AddGeneratedFilterMethodsToDecompiledType(decompiledType, typeContext, language);
            }

			Dictionary<string, MethodSpecificContext> methodContexts = new Dictionary<string, MethodSpecificContext>();
			Dictionary<string, Statement> decompiledStatements = new Dictionary<string, Statement>();

			foreach (KeyValuePair<string, DecompiledMember> decompiledPair in decompiledType.DecompiledMembers)
			{
				methodContexts.Add(decompiledPair.Key, decompiledPair.Value.Context);
				decompiledStatements.Add(decompiledPair.Key, decompiledPair.Value.Statement);
			}

			TypeDefinition declaringType = Utilities.GetDeclaringTypeOrSelf(member);

			AssemblySpecificContext assemblyContext = GetAssemblyContext(declaringType.Module.Assembly, language);
			ModuleSpecificContext moduleContext = GetModuleContext(declaringType.Module, language);

			WriterContext writerContext = new WriterContext(assemblyContext, moduleContext, typeContext, methodContexts, decompiledStatements);

			return writerContext;
		}

        private TypeSpecificContext GetTypeContext(DecompiledType decompiledType, ILanguage language)
        {
            TypeSpecificContext typeContext;
            TypeDefinition type = decompiledType.Type;
            if (this.cacheService.IsTypeContextInCache(type, language, this.renameInvalidMembers))
            {
                if (decompiledType.TypeContext.GeneratedFilterMethods.Count > 0)
                {
                    this.cacheService.ReplaceCachedTypeContext(type, language, this.renameInvalidMembers, decompiledType.TypeContext);
                }

                typeContext = this.cacheService.GetTypeContextFromCache(type, language, this.renameInvalidMembers);
            }
            else
            {
                typeContext = decompiledType.TypeContext;

                this.cacheService.AddTypeContextToCache(type, language, this.renameInvalidMembers, typeContext);
            }

            return typeContext;
        }

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			if (this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetAssemblyContextFromCache(assembly, language, this.renameInvalidMembers);
			}

			ICollection<string> assemblyNamespaceUsings = GetAssemblyNamespaceUsings(assembly);
			AssemblySpecificContext assemblyContext = new AssemblySpecificContext(assemblyNamespaceUsings);

			this.cacheService.AddAssemblyContextToCache(assembly, language, this.renameInvalidMembers, assemblyContext);

			return assemblyContext;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			if (this.cacheService.IsModuleContextInCache(module, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetModuleContextFromCache(module, language, this.renameInvalidMembers);
			}

			ICollection<string> moduleNamespaceUsings = GetModuleNamespaceUsings(module);
			Dictionary<string, List<string>> collisionTypesData = new Dictionary<string, List<string>>();
			Dictionary<string, HashSet<string>> namesapceHieararchy = new Dictionary<string, HashSet<string>>();
			Dictionary<string, string> renamedNamespacesMap = new Dictionary<string, string>();
			MemberRenamingData memberRenamingData = GetMemberRenamingData(module, language);

			ModuleSpecificContext moduleContext =
				new ModuleSpecificContext(module, moduleNamespaceUsings, collisionTypesData, namesapceHieararchy, renamedNamespacesMap, memberRenamingData.RenamedMembers, memberRenamingData.RenamedMembersMap);

			this.cacheService.AddModuleContextToCache(module, language, this.renameInvalidMembers, moduleContext);

			return moduleContext;
		}

		private DecompiledType GetDecompiledType(IMemberDefinition member, ILanguage language)
		{
			TypeDefinition declaringType = Utilities.GetDeclaringTypeOrSelf(member);
			DecompiledType decompiledType = new DecompiledType(declaringType);

			Queue<IMemberDefinition> decompilationQueue = new Queue<IMemberDefinition>();

			decompilationQueue.Enqueue(member);
			while (decompilationQueue.Count > 0)
			{
				IMemberDefinition currentMember = decompilationQueue.Dequeue();

				if (currentMember is TypeDefinition && currentMember == member)
				{
					TypeDefinition currentType = (currentMember as TypeDefinition);

                    List<IMemberDefinition> members = Utilities.GetTypeMembers(currentType, language,
                        propertyFields: decompiledType.TypeContext.GetFieldToPropertyMap(language).Keys);
					foreach (IMemberDefinition typeMember in members)
					{
						decompilationQueue.Enqueue(typeMember);
					}
				}

				if (currentMember is MethodDefinition)
				{
					DecompileMember(currentMember as MethodDefinition, language, decompiledType);
				}
				if (currentMember is EventDefinition)
				{
					EventDefinition eventDefinition = (currentMember as EventDefinition);

					AutoImplementedEventMatcher matcher = new AutoImplementedEventMatcher(eventDefinition, language);
					bool isAutoImplemented = matcher.IsAutoImplemented();

					if (isAutoImplemented)
					{
						decompiledType.TypeContext.AutoImplementedEvents.Add(eventDefinition);
					}

					if (eventDefinition.AddMethod != null)
					{
						DecompileMember(eventDefinition.AddMethod, language, decompiledType);
					}

					if (eventDefinition.RemoveMethod != null)
					{
						DecompileMember(eventDefinition.RemoveMethod, language, decompiledType);
					}

					if (eventDefinition.InvokeMethod != null)
					{
						DecompileMember(eventDefinition.InvokeMethod, language, decompiledType);
					}
				}
				if (currentMember is PropertyDefinition)
				{
					PropertyDefinition propertyDefinition = (currentMember as PropertyDefinition);

                    CachedDecompiledMember getMethod;
                    CachedDecompiledMember setMethod;
                    bool isAutoImplemented;

                    PropertyDecompiler propertyDecompiler = new PropertyDecompiler(propertyDefinition, language, this.renameInvalidMembers, this.cacheService, decompiledType.TypeContext);
                    propertyDecompiler.ExceptionThrown += OnExceptionThrown;
                    propertyDecompiler.Decompile(out getMethod, out setMethod, out isAutoImplemented);
                    propertyDecompiler.ExceptionThrown -= OnExceptionThrown;

                    if (isAutoImplemented)
					{
						decompiledType.TypeContext.AutoImplementedProperties.Add(propertyDefinition);
					}

                    if (getMethod != null)
                    {
                        AddDecompiledMemberToDecompiledType(getMethod, decompiledType);
                    }

                    if (setMethod != null)
                    {
                        AddDecompiledMemberToDecompiledType(setMethod, decompiledType);
                    }

                    foreach (MethodDefinition exceptionWhileDecompiling in propertyDecompiler.ExceptionsWhileDecompiling)
                    {
                        this.ExceptionsWhileDecompiling.Add(exceptionWhileDecompiling);
                    }
				}
				if (currentMember is FieldDefinition)
				{
					FieldDefinition currentField = currentMember as FieldDefinition;

					/// Decompile all the constructors, that can set default values to the field.
					/// For instance fields decompile only instance constructors.
					/// For static fields decompile only static constructors.
					foreach (MethodDefinition method in currentMember.DeclaringType.Methods)
					{
						if (method.IsConstructor && currentField.IsStatic == method.IsStatic)
						{
							DecompileConstructorChain(method, language, decompiledType);
							break;
						}
					}
				}

			}

			decompiledType.TypeContext.ExplicitlyImplementedMembers = GetExplicitlyImplementedInterfaceMethods(declaringType, language);
            AddGeneratedFilterMethodsToDecompiledType(decompiledType, decompiledType.TypeContext, language);

			return decompiledType;
		}

	}
}
