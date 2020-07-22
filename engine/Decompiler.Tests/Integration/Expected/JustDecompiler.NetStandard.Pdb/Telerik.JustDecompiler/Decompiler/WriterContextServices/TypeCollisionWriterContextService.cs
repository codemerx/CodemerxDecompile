using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class TypeCollisionWriterContextService : BaseWriterContextService
	{
		private readonly static object assemblyContextsLocker;

		private readonly static object moduleContextsLocker;

		static TypeCollisionWriterContextService()
		{
			TypeCollisionWriterContextService.assemblyContextsLocker = new Object();
			TypeCollisionWriterContextService.moduleContextsLocker = new Object();
			return;
		}

		public TypeCollisionWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
		{
			base(cacheService, renameInvalidMembers);
			return;
		}

		private string EscapeNamespace(string[] namespaceParts, ILanguage langugage)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(langugage.ReplaceInvalidCharactersInIdentifier(namespaceParts[0]));
			V_1 = 1;
			while (V_1 < (int)namespaceParts.Length)
			{
				dummyVar1 = V_0.AppendFormat(".{0}", langugage.ReplaceInvalidCharactersInIdentifier(namespaceParts[V_1]));
				V_1 = V_1 + 1;
			}
			return V_0.ToString();
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			if (this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetAssemblyContextFromCache(assembly, language, this.renameInvalidMembers);
			}
			V_0 = new AssemblySpecificContext(this.GetAssemblyNamespaceUsings(assembly));
			V_1 = TypeCollisionWriterContextService.assemblyContextsLocker;
			V_2 = false;
			try
			{
				Monitor.Enter(V_1, ref V_2);
				if (!this.cacheService.IsAssemblyContextInCache(assembly, language, this.renameInvalidMembers))
				{
					this.cacheService.AddAssemblyContextToCache(assembly, language, this.renameInvalidMembers, V_0);
				}
			}
			finally
			{
				if (V_2)
				{
					Monitor.Exit(V_1);
				}
			}
			return V_0;
		}

		private Dictionary<string, List<string>> GetModuleCollisionTypesData(ModuleDefinition module, ILanguage language)
		{
			V_0 = new Dictionary<string, List<string>>(language.get_IdentifierComparer());
			V_1 = new Dictionary<string, string>(language.get_IdentifierComparer());
			this.UpdateCollisionTypesDataWithTypes(V_0, V_1, module.get_Types());
			V_2 = this.GetModuleDependsOnAnalysis(module);
			if (Mono.Cecil.AssemblyResolver.Extensions.IsReferenceAssembly(module))
			{
				stackVariable16 = 1;
			}
			else
			{
				stackVariable16 = 0;
			}
			V_3 = stackVariable16;
			V_4 = V_2.get_Keys().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					V_6 = module.get_AssemblyResolver().Resolve(V_5, "", ModuleDefinitionExtensions.GetModuleArchitecture(module), V_3, true);
					if (V_6 == null)
					{
						this.UpdateCollisionTypesDataWithTypes(V_0, V_1, V_2.get_Item(V_5));
					}
					else
					{
						V_7 = V_6.get_Modules().GetEnumerator();
						try
						{
							while (V_7.MoveNext())
							{
								V_8 = V_7.get_Current();
								this.UpdateCollisionTypesDataWithTypes(V_0, V_1, V_8.get_Types());
							}
						}
						finally
						{
							V_7.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_0;
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			V_0 = TypeCollisionWriterContextService.moduleContextsLocker;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				if (!this.cacheService.IsModuleContextInCache(module, language, this.renameInvalidMembers))
				{
					V_2 = this.GetModuleNamespaceUsings(module);
					V_3 = this.GetModuleCollisionTypesData(module, language);
					V_4 = this.GetModuleNamespaceHierarchy(module, language);
					V_5 = this.GetRenamedNamespacesMap(module, language);
					V_6 = this.GetMemberRenamingData(module, language);
					V_7 = new ModuleSpecificContext(module, V_2, V_3, V_4, V_5, V_6.get_RenamedMembers(), V_6.get_RenamedMembersMap());
					this.cacheService.AddModuleContextToCache(module, language, this.renameInvalidMembers, V_7);
					V_8 = V_7;
				}
				else
				{
					V_8 = this.cacheService.GetModuleContextFromCache(module, language, this.renameInvalidMembers);
				}
			}
			finally
			{
				if (V_1)
				{
					Monitor.Exit(V_0);
				}
			}
			return V_8;
		}

		private Dictionary<AssemblyNameReference, List<TypeReference>> GetModuleDependsOnAnalysis(ModuleDefinition module)
		{
			stackVariable1 = module.get_Types();
			V_0 = new HashSet<TypeReference>();
			V_3 = stackVariable1.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					if (V_0.Contains(V_4))
					{
						continue;
					}
					dummyVar0 = V_0.Add(V_4);
				}
			}
			finally
			{
				V_3.Dispose();
			}
			V_5 = module.GetTypeReferences().GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					if (V_0.Contains(V_6))
					{
						continue;
					}
					dummyVar1 = V_0.Add(V_6);
				}
			}
			finally
			{
				if (V_5 != null)
				{
					V_5.Dispose();
				}
			}
			V_2 = Utilities.GetAssembliesDependingOnToUsedTypesMap(module, Utilities.GetExpandedTypeDependanceList(V_0));
			V_7 = module.get_AssemblyReferences().GetEnumerator();
			try
			{
				while (V_7.MoveNext())
				{
					V_8 = V_7.get_Current();
					if (V_2.ContainsKey(V_8))
					{
						continue;
					}
					V_2.Add(V_8, new List<TypeReference>());
				}
			}
			finally
			{
				V_7.Dispose();
			}
			return V_2;
		}

		private Dictionary<string, HashSet<string>> GetModuleNamespaceHierarchy(ModuleDefinition module, ILanguage language)
		{
			V_0 = new Dictionary<string, HashSet<string>>(language.get_IdentifierComparer());
			this.UpdateNamespaceHiearchyDataWithTypes(V_0, module.get_Types());
			V_1 = this.GetModuleDependsOnAnalysis(module);
			if (Mono.Cecil.AssemblyResolver.Extensions.IsReferenceAssembly(module))
			{
				stackVariable12 = 1;
			}
			else
			{
				stackVariable12 = 0;
			}
			V_2 = stackVariable12;
			V_3 = V_1.get_Keys().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					V_5 = module.get_AssemblyResolver().Resolve(V_4, "", ModuleDefinitionExtensions.GetModuleArchitecture(module), V_2, true);
					if (V_5 == null)
					{
						this.UpdateNamespaceHiearchyDataWithTypes(V_0, V_1.get_Item(V_4));
					}
					else
					{
						this.UpdateNamespaceHiearchyDataWithTypes(V_0, V_5.get_MainModule().get_Types());
					}
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			return V_0;
		}

		private Dictionary<string, string> GetRenamedNamespacesMap(ModuleDefinition module, ILanguage langugage)
		{
			V_0 = new Dictionary<string, string>();
			if (!this.renameInvalidMembers)
			{
				return V_0;
			}
			V_1 = new HashSet<string>();
			V_2 = module.get_Types().GetEnumerator();
			try
			{
			Label0:
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current().GetNamespace();
					if (!String.op_Equality(V_3, String.Empty))
					{
						if (V_1.Contains(V_3))
						{
							continue;
						}
						dummyVar1 = V_1.Add(V_3);
						stackVariable23 = new Char[1];
						stackVariable23[0] = '.';
						V_4 = V_3.Split(stackVariable23);
						V_5 = V_4;
						V_6 = 0;
						while (V_6 < (int)V_5.Length)
						{
							if (langugage.IsValidIdentifier(V_5[V_6]))
							{
								V_6 = V_6 + 1;
							}
							else
							{
								V_0.Add(V_3, this.EscapeNamespace(V_4, langugage));
								goto Label0;
							}
						}
					}
					else
					{
						dummyVar0 = V_1.Add(V_3);
					}
				}
			}
			finally
			{
				V_2.Dispose();
			}
			return V_0;
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			return this.GetWriterContextForType(Utilities.GetDeclaringTypeOrSelf(member), language);
		}

		private WriterContext GetWriterContextForType(TypeDefinition type, ILanguage language)
		{
			V_0 = Utilities.GetOuterMostDeclaringType(type);
			if (!this.cacheService.AreNestedDecompiledTypesInCache(V_0, language, this.renameInvalidMembers))
			{
				V_1 = this.GetNestedDecompiledTypes(V_0, language);
				this.cacheService.AddNestedDecompiledTypesToCache(V_0, language, this.renameInvalidMembers, V_1);
			}
			else
			{
				V_1 = this.cacheService.GetNestedDecompiledTypesFromCache(V_0, language, this.renameInvalidMembers);
			}
			V_2 = this.GetTypeContext(V_0, language, V_1);
			this.AddTypeContextsToCache(V_1, V_0, language);
			V_3 = new Dictionary<string, MethodSpecificContext>();
			V_4 = new Dictionary<string, Statement>();
			if (!V_1.TryGetValue(type.get_FullName(), out V_5))
			{
				throw new Exception("Decompiled type missing from DecompiledTypes cache.");
			}
			if (V_2.get_GeneratedFilterMethods().get_Count() > 0)
			{
				this.AddGeneratedFilterMethodsToDecompiledType(V_5, V_2, language);
			}
			V_7 = V_5.get_DecompiledMembers().get_Values().GetEnumerator();
			try
			{
				while (V_7.MoveNext())
				{
					V_8 = V_7.get_Current();
					V_3.Add(V_8.get_MemberFullName(), V_8.get_Context());
					V_4.Add(V_8.get_MemberFullName(), V_8.get_Statement());
				}
			}
			finally
			{
				((IDisposable)V_7).Dispose();
			}
			stackVariable63 = this.GetAssemblyContext(V_0.get_Module().get_Assembly(), language);
			V_6 = this.GetModuleContext(V_0.get_Module(), language);
			return new WriterContext(stackVariable63, V_6, V_2, V_3, V_4);
		}

		private void UpdateCollisionTypesDataWithTypes(Dictionary<string, List<string>> collisionTypesData, Dictionary<string, string> typeNamesFirstOccurrence, IEnumerable<TypeReference> types)
		{
			V_0 = types.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Name();
					if (!typeNamesFirstOccurrence.ContainsKey(V_2))
					{
						typeNamesFirstOccurrence.Add(V_2, V_1.get_Namespace());
					}
					else
					{
						if (collisionTypesData.ContainsKey(V_2))
						{
							if (!collisionTypesData.TryGetValue(V_2, out V_5))
							{
								throw new Exception("Collision type namespaces collection not found in collision types cache.");
							}
							V_5.Add(V_1.get_Namespace());
						}
						else
						{
							if (!typeNamesFirstOccurrence.TryGetValue(V_2, out V_3))
							{
								throw new Exception("Namespace not found in type first time occurence cache.");
							}
							V_4 = new List<string>();
							V_4.Add(V_3);
							V_4.Add(V_1.get_Namespace());
							collisionTypesData.Add(V_2, V_4);
						}
					}
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		private void UpdateNamespaceHiearchyDataWithTypes(Dictionary<string, HashSet<string>> namespaceHierarchy, IEnumerable<TypeReference> types)
		{
			V_0 = types.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current().get_Namespace();
					if (!Utilities.HasNamespaceParentNamespace(V_1))
					{
						continue;
					}
					V_2 = Utilities.GetNamesapceParentNamesapce(V_1);
					V_3 = Utilities.GetNamespaceChildNamesapce(V_1);
					if (!namespaceHierarchy.TryGetValue(V_2, out V_4))
					{
						V_4 = new HashSet<string>();
						dummyVar1 = V_4.Add(V_3);
						namespaceHierarchy.Add(V_2, V_4);
					}
					else
					{
						if (V_4.Contains(V_3))
						{
							continue;
						}
						dummyVar0 = V_4.Add(V_3);
					}
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}
	}
}