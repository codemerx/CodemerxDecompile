using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class SimpleWriterContextService : BaseWriterContextService
	{
		public SimpleWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
		{
			base(cacheService, renameInvalidMembers);
			return;
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			if (this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetAssemblyContextFromCache(assembly, language, this.renameInvalidMembers);
			}
			V_0 = new AssemblySpecificContext(this.GetAssemblyNamespaceUsings(assembly));
			this.cacheService.AddAssemblyContextToCache(assembly, language, this.renameInvalidMembers, V_0);
			return V_0;
		}

		private DecompiledType GetDecompiledType(IMemberDefinition member, ILanguage language)
		{
			V_0 = Utilities.GetDeclaringTypeOrSelf(member);
			V_1 = new DecompiledType(V_0);
			V_2 = new Queue<IMemberDefinition>();
			V_2.Enqueue(member);
		Label0:
			while (V_2.get_Count() > 0)
			{
				V_3 = V_2.Dequeue();
				if (V_3 as TypeDefinition != null && (object)V_3 == (object)member)
				{
					V_4 = Utilities.GetTypeMembers(V_3 as TypeDefinition, language, true, null, null, null, V_1.get_TypeContext().GetFieldToPropertyMap(language).get_Keys()).GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							V_2.Enqueue(V_5);
						}
					}
					finally
					{
						((IDisposable)V_4).Dispose();
					}
				}
				if (V_3 as MethodDefinition != null)
				{
					this.DecompileMember(V_3 as MethodDefinition, language, V_1);
				}
				if (V_3 as EventDefinition != null)
				{
					V_6 = V_3 as EventDefinition;
					if ((new AutoImplementedEventMatcher(V_6, language)).IsAutoImplemented())
					{
						dummyVar0 = V_1.get_TypeContext().get_AutoImplementedEvents().Add(V_6);
					}
					if (V_6.get_AddMethod() != null)
					{
						this.DecompileMember(V_6.get_AddMethod(), language, V_1);
					}
					if (V_6.get_RemoveMethod() != null)
					{
						this.DecompileMember(V_6.get_RemoveMethod(), language, V_1);
					}
					if (V_6.get_InvokeMethod() != null)
					{
						this.DecompileMember(V_6.get_InvokeMethod(), language, V_1);
					}
				}
				if (V_3 as PropertyDefinition != null)
				{
					V_7 = V_3 as PropertyDefinition;
					stackVariable52 = new PropertyDecompiler(V_7, language, this.renameInvalidMembers, this.cacheService, V_1.get_TypeContext());
					stackVariable52.add_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
					stackVariable52.Decompile(out V_8, out V_9, out V_10);
					stackVariable52.remove_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
					if (V_10)
					{
						dummyVar1 = V_1.get_TypeContext().get_AutoImplementedProperties().Add(V_7);
					}
					if (V_8 != null)
					{
						this.AddDecompiledMemberToDecompiledType(V_8, V_1);
					}
					if (V_9 != null)
					{
						this.AddDecompiledMemberToDecompiledType(V_9, V_1);
					}
					V_11 = stackVariable52.get_ExceptionsWhileDecompiling().GetEnumerator();
					try
					{
						while (V_11.MoveNext())
						{
							V_12 = V_11.get_Current();
							this.get_ExceptionsWhileDecompiling().Add(V_12);
						}
					}
					finally
					{
						if (V_11 != null)
						{
							V_11.Dispose();
						}
					}
				}
				if (V_3 as FieldDefinition == null)
				{
					continue;
				}
				V_13 = V_3 as FieldDefinition;
				V_14 = V_3.get_DeclaringType().get_Methods().GetEnumerator();
				try
				{
					while (V_14.MoveNext())
					{
						V_15 = V_14.get_Current();
						if (!V_15.get_IsConstructor() || V_13.get_IsStatic() != V_15.get_IsStatic())
						{
							continue;
						}
						this.DecompileConstructorChain(V_15, language, V_1);
						goto Label0;
					}
				}
				finally
				{
					V_14.Dispose();
				}
			}
			V_1.get_TypeContext().set_ExplicitlyImplementedMembers(this.GetExplicitlyImplementedInterfaceMethods(V_0, language));
			this.AddGeneratedFilterMethodsToDecompiledType(V_1, V_1.get_TypeContext(), language);
			return V_1;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			if (this.cacheService.IsModuleContextInCache(module, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetModuleContextFromCache(module, language, this.renameInvalidMembers);
			}
			V_0 = this.GetModuleNamespaceUsings(module);
			V_1 = new Dictionary<string, List<string>>();
			V_2 = new Dictionary<string, HashSet<string>>();
			V_3 = new Dictionary<string, string>();
			V_4 = this.GetMemberRenamingData(module, language);
			V_5 = new ModuleSpecificContext(module, V_0, V_1, V_2, V_3, V_4.get_RenamedMembers(), V_4.get_RenamedMembersMap());
			this.cacheService.AddModuleContextToCache(module, language, this.renameInvalidMembers, V_5);
			return V_5;
		}

		private TypeSpecificContext GetTypeContext(DecompiledType decompiledType, ILanguage language)
		{
			V_1 = decompiledType.get_Type();
			if (!this.cacheService.IsTypeContextInCache(V_1, language, this.renameInvalidMembers))
			{
				V_0 = decompiledType.get_TypeContext();
				this.cacheService.AddTypeContextToCache(V_1, language, this.renameInvalidMembers, V_0);
			}
			else
			{
				if (decompiledType.get_TypeContext().get_GeneratedFilterMethods().get_Count() > 0)
				{
					this.cacheService.ReplaceCachedTypeContext(V_1, language, this.renameInvalidMembers, decompiledType.get_TypeContext());
				}
				V_0 = this.cacheService.GetTypeContextFromCache(V_1, language, this.renameInvalidMembers);
			}
			return V_0;
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			if (member as TypeDefinition == null || member != Utilities.GetOuterMostDeclaringType(member))
			{
				V_1 = this.GetDecompiledType(member, language);
				V_0 = this.GetTypeContext(V_1, language);
			}
			else
			{
				V_7 = member as TypeDefinition;
				V_8 = this.GetNestedDecompiledTypes(V_7, language);
				V_0 = this.GetTypeContext(V_7, language, V_8);
				this.AddTypeContextsToCache(V_8, V_7, language);
				if (!V_8.TryGetValue(V_7.get_FullName(), out V_1))
				{
					throw new Exception("Decompiled type not found in decompiled types cache.");
				}
			}
			V_2 = new TypeSpecificContext(V_0.get_CurrentType(), V_0.get_MethodDefinitionToNameMap(), V_0.get_BackingFieldToNameMap(), V_0.get_UsedNamespaces(), new HashSet<string>(), V_0.get_AssignmentData(), V_0.get_AutoImplementedProperties(), V_0.get_AutoImplementedEvents(), V_0.get_ExplicitlyImplementedMembers(), V_0.get_ExceptionWhileDecompiling(), V_0.get_GeneratedFilterMethods(), V_0.get_GeneratedMethodDefinitionToNameMap());
			if (V_2.get_GeneratedFilterMethods().get_Count() > 0)
			{
				this.AddGeneratedFilterMethodsToDecompiledType(V_1, V_2, language);
			}
			V_3 = new Dictionary<string, MethodSpecificContext>();
			V_4 = new Dictionary<string, Statement>();
			V_9 = V_1.get_DecompiledMembers().GetEnumerator();
			try
			{
				while (V_9.MoveNext())
				{
					V_10 = V_9.get_Current();
					V_3.Add(V_10.get_Key(), V_10.get_Value().get_Context());
					V_4.Add(V_10.get_Key(), V_10.get_Value().get_Statement());
				}
			}
			finally
			{
				((IDisposable)V_9).Dispose();
			}
			V_5 = Utilities.GetDeclaringTypeOrSelf(member);
			stackVariable66 = this.GetAssemblyContext(V_5.get_Module().get_Assembly(), language);
			V_6 = this.GetModuleContext(V_5.get_Module(), language);
			return new WriterContext(stackVariable66, V_6, V_2, V_3, V_4);
		}
	}
}