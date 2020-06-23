using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class SimpleWriterContextService : BaseWriterContextService
	{
		public SimpleWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers) : base(cacheService, renameInvalidMembers)
		{
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			if (this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetAssemblyContextFromCache(assembly, language, this.renameInvalidMembers);
			}
			AssemblySpecificContext assemblySpecificContext = new AssemblySpecificContext(base.GetAssemblyNamespaceUsings(assembly));
			this.cacheService.AddAssemblyContextToCache(assembly, language, this.renameInvalidMembers, assemblySpecificContext);
			return assemblySpecificContext;
		}

		private DecompiledType GetDecompiledType(IMemberDefinition member, ILanguage language)
		{
			CachedDecompiledMember cachedDecompiledMember;
			CachedDecompiledMember cachedDecompiledMember1;
			bool flag;
			TypeDefinition declaringTypeOrSelf = Utilities.GetDeclaringTypeOrSelf(member);
			DecompiledType decompiledType = new DecompiledType(declaringTypeOrSelf);
			Queue<IMemberDefinition> memberDefinitions = new Queue<IMemberDefinition>();
			memberDefinitions.Enqueue(member);
		Label0:
			while (memberDefinitions.Count > 0)
			{
				IMemberDefinition memberDefinition = memberDefinitions.Dequeue();
				if (memberDefinition is TypeDefinition && memberDefinition == member)
				{
					foreach (IMemberDefinition typeMember in Utilities.GetTypeMembers(memberDefinition as TypeDefinition, language, true, null, null, null, decompiledType.TypeContext.GetFieldToPropertyMap(language).Keys))
					{
						memberDefinitions.Enqueue(typeMember);
					}
				}
				if (memberDefinition is MethodDefinition)
				{
					base.DecompileMember(memberDefinition as MethodDefinition, language, decompiledType);
				}
				if (memberDefinition is EventDefinition)
				{
					EventDefinition eventDefinition = memberDefinition as EventDefinition;
					if ((new AutoImplementedEventMatcher(eventDefinition, language)).IsAutoImplemented())
					{
						decompiledType.TypeContext.AutoImplementedEvents.Add(eventDefinition);
					}
					if (eventDefinition.AddMethod != null)
					{
						base.DecompileMember(eventDefinition.AddMethod, language, decompiledType);
					}
					if (eventDefinition.RemoveMethod != null)
					{
						base.DecompileMember(eventDefinition.RemoveMethod, language, decompiledType);
					}
					if (eventDefinition.InvokeMethod != null)
					{
						base.DecompileMember(eventDefinition.InvokeMethod, language, decompiledType);
					}
				}
				if (memberDefinition is PropertyDefinition)
				{
					PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
					PropertyDecompiler propertyDecompiler = new PropertyDecompiler(propertyDefinition, language, this.renameInvalidMembers, this.cacheService, decompiledType.TypeContext);
					propertyDecompiler.ExceptionThrown += new EventHandler<Exception>(this.OnExceptionThrown);
					propertyDecompiler.Decompile(out cachedDecompiledMember, out cachedDecompiledMember1, out flag);
					propertyDecompiler.ExceptionThrown -= new EventHandler<Exception>(this.OnExceptionThrown);
					if (flag)
					{
						decompiledType.TypeContext.AutoImplementedProperties.Add(propertyDefinition);
					}
					if (cachedDecompiledMember != null)
					{
						base.AddDecompiledMemberToDecompiledType(cachedDecompiledMember, decompiledType);
					}
					if (cachedDecompiledMember1 != null)
					{
						base.AddDecompiledMemberToDecompiledType(cachedDecompiledMember1, decompiledType);
					}
					foreach (MethodDefinition exceptionsWhileDecompiling in propertyDecompiler.ExceptionsWhileDecompiling)
					{
						base.ExceptionsWhileDecompiling.Add(exceptionsWhileDecompiling);
					}
				}
				if (!(memberDefinition is FieldDefinition))
				{
					continue;
				}
				FieldDefinition fieldDefinition = memberDefinition as FieldDefinition;
				foreach (MethodDefinition method in memberDefinition.DeclaringType.Methods)
				{
					if (!method.IsConstructor || fieldDefinition.IsStatic != method.IsStatic)
					{
						continue;
					}
					base.DecompileConstructorChain(method, language, decompiledType);
					goto Label0;
				}
			}
			decompiledType.TypeContext.ExplicitlyImplementedMembers = base.GetExplicitlyImplementedInterfaceMethods(declaringTypeOrSelf, language);
			base.AddGeneratedFilterMethodsToDecompiledType(decompiledType, decompiledType.TypeContext, language);
			return decompiledType;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			if (this.cacheService.IsModuleContextInCache(module, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetModuleContextFromCache(module, language, this.renameInvalidMembers);
			}
			ICollection<string> moduleNamespaceUsings = base.GetModuleNamespaceUsings(module);
			Dictionary<string, List<string>> strs = new Dictionary<string, List<string>>();
			Dictionary<string, HashSet<string>> strs1 = new Dictionary<string, HashSet<string>>();
			Dictionary<string, string> strs2 = new Dictionary<string, string>();
			MemberRenamingData memberRenamingData = this.GetMemberRenamingData(module, language);
			ModuleSpecificContext moduleSpecificContext = new ModuleSpecificContext(module, moduleNamespaceUsings, strs, strs1, strs2, memberRenamingData.RenamedMembers, memberRenamingData.RenamedMembersMap);
			this.cacheService.AddModuleContextToCache(module, language, this.renameInvalidMembers, moduleSpecificContext);
			return moduleSpecificContext;
		}

		private TypeSpecificContext GetTypeContext(DecompiledType decompiledType, ILanguage language)
		{
			TypeSpecificContext typeContext;
			TypeDefinition type = decompiledType.Type;
			if (!this.cacheService.IsTypeContextInCache(type, language, this.renameInvalidMembers))
			{
				typeContext = decompiledType.TypeContext;
				this.cacheService.AddTypeContextToCache(type, language, this.renameInvalidMembers, typeContext);
			}
			else
			{
				if (decompiledType.TypeContext.GeneratedFilterMethods.Count > 0)
				{
					this.cacheService.ReplaceCachedTypeContext(type, language, this.renameInvalidMembers, decompiledType.TypeContext);
				}
				typeContext = this.cacheService.GetTypeContextFromCache(type, language, this.renameInvalidMembers);
			}
			return typeContext;
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			TypeSpecificContext typeContext;
			DecompiledType decompiledType;
			if (!(member is TypeDefinition) || member != Utilities.GetOuterMostDeclaringType(member))
			{
				decompiledType = this.GetDecompiledType(member, language);
				typeContext = this.GetTypeContext(decompiledType, language);
			}
			else
			{
				TypeDefinition typeDefinition = member as TypeDefinition;
				Dictionary<string, DecompiledType> nestedDecompiledTypes = base.GetNestedDecompiledTypes(typeDefinition, language);
				typeContext = this.GetTypeContext(typeDefinition, language, nestedDecompiledTypes);
				base.AddTypeContextsToCache(nestedDecompiledTypes, typeDefinition, language);
				if (!nestedDecompiledTypes.TryGetValue(typeDefinition.FullName, out decompiledType))
				{
					throw new Exception("Decompiled type not found in decompiled types cache.");
				}
			}
			TypeSpecificContext typeSpecificContext = new TypeSpecificContext(typeContext.CurrentType, typeContext.MethodDefinitionToNameMap, typeContext.BackingFieldToNameMap, typeContext.UsedNamespaces, new HashSet<string>(), typeContext.AssignmentData, typeContext.AutoImplementedProperties, typeContext.AutoImplementedEvents, typeContext.ExplicitlyImplementedMembers, typeContext.ExceptionWhileDecompiling, typeContext.GeneratedFilterMethods, typeContext.GeneratedMethodDefinitionToNameMap);
			if (typeSpecificContext.GeneratedFilterMethods.Count > 0)
			{
				base.AddGeneratedFilterMethodsToDecompiledType(decompiledType, typeSpecificContext, language);
			}
			Dictionary<string, MethodSpecificContext> strs = new Dictionary<string, MethodSpecificContext>();
			Dictionary<string, Statement> strs1 = new Dictionary<string, Statement>();
			foreach (KeyValuePair<string, DecompiledMember> decompiledMember in decompiledType.DecompiledMembers)
			{
				strs.Add(decompiledMember.Key, decompiledMember.Value.Context);
				strs1.Add(decompiledMember.Key, decompiledMember.Value.Statement);
			}
			TypeDefinition declaringTypeOrSelf = Utilities.GetDeclaringTypeOrSelf(member);
			AssemblySpecificContext assemblyContext = this.GetAssemblyContext(declaringTypeOrSelf.Module.Assembly, language);
			ModuleSpecificContext moduleContext = this.GetModuleContext(declaringTypeOrSelf.Module, language);
			return new WriterContext(assemblyContext, moduleContext, typeSpecificContext, strs, strs1);
		}
	}
}