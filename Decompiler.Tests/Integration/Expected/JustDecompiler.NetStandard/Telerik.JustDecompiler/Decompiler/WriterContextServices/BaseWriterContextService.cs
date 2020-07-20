using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public abstract class BaseWriterContextService : ExceptionThrownNotifier, IWriterContextService, IExceptionThrownNotifier
	{
		protected readonly IDecompilationCacheService cacheService;

		protected readonly bool renameInvalidMembers;

		public ICollection<MethodDefinition> ExceptionsWhileDecompiling
		{
			get
			{
				return get_ExceptionsWhileDecompiling();
			}
			set
			{
				set_ExceptionsWhileDecompiling(value);
			}
		}

		// <ExceptionsWhileDecompiling>k__BackingField
		private ICollection<MethodDefinition> u003cExceptionsWhileDecompilingu003ek__BackingField;

		public ICollection<MethodDefinition> get_ExceptionsWhileDecompiling()
		{
			return this.u003cExceptionsWhileDecompilingu003ek__BackingField;
		}

		private void set_ExceptionsWhileDecompiling(ICollection<MethodDefinition> value)
		{
			this.u003cExceptionsWhileDecompilingu003ek__BackingField = value;
			return;
		}

		public BaseWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
		{
			base();
			this.cacheService = cacheService;
			this.renameInvalidMembers = renameInvalidMembers;
			this.set_ExceptionsWhileDecompiling(new List<MethodDefinition>());
			return;
		}

		private static void AddAssignmentDataToDecompiledType(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			if (!decompiledType.get_TypeContext().get_FieldInitializationFailed())
			{
				V_0 = decompiledMember.get_FieldAssignmentData().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						if (decompiledType.get_TypeContext().get_AssignmentData().ContainsKey(V_1.get_Key()))
						{
							if (decompiledType.get_TypeContext().get_AssignmentData().get_Item(V_1.get_Key()).get_AssignmentExpression().Equals(V_1.get_Value().get_AssignmentExpression()))
							{
								continue;
							}
							decompiledType.get_TypeContext().set_FieldInitializationFailed(true);
							decompiledType.get_TypeContext().set_AssignmentData(new Dictionary<string, InitializationAssignment>());
							goto Label0;
						}
						else
						{
							decompiledType.get_TypeContext().get_AssignmentData().Add(V_1.get_Key(), V_1.get_Value());
						}
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
			}
		Label0:
			return;
		}

		protected CachedDecompiledMember AddDecompiledMemberToCache(IMemberDefinition member, DecompiledMember decompiledMember, TypeSpecificContext typeContext, ILanguage language)
		{
			V_0 = new CachedDecompiledMember(decompiledMember, typeContext);
			if (!this.cacheService.IsDecompiledMemberInCache(member, language, this.renameInvalidMembers))
			{
				this.cacheService.AddDecompiledMemberToCache(member, language, this.renameInvalidMembers, V_0);
			}
			return V_0;
		}

		protected void AddDecompiledMemberToDecompiledType(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			if (!decompiledType.get_DecompiledMembers().ContainsKey(decompiledMember.get_Member().get_MemberFullName()))
			{
				decompiledType.get_DecompiledMembers().Add(decompiledMember.get_Member().get_MemberFullName(), decompiledMember.get_Member());
			}
			return;
		}

		private void AddExplicitlyImplementedMembers(ICollection<ImplementedMember> explicitlyImplementedMembers, ExplicitlyImplementedMembersCollection collection)
		{
			V_0 = explicitlyImplementedMembers.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (collection.Contains(V_1.get_DeclaringType(), V_1.get_Member().get_FullName()))
					{
						continue;
					}
					collection.Add(V_1.get_DeclaringType(), V_1.get_Member().get_FullName());
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

		protected void AddGeneratedFilterMethodsToDecompiledType(DecompiledType decompiledType, TypeSpecificContext context, ILanguage language)
		{
			V_0 = context.get_GeneratedFilterMethods().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(V_1.get_Method()), V_1.get_Body(), V_1.get_Context()));
					this.AddDecompiledMemberToDecompiledType(V_2, decompiledType);
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

		protected void AddTypeContextsToCache(Dictionary<string, DecompiledType> decompiledTypes, TypeDefinition outerMostDeclaringType, ILanguage language)
		{
			V_0 = decompiledTypes.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.cacheService.IsTypeContextInCache(V_1.get_Value().get_Type(), language, this.renameInvalidMembers))
					{
						continue;
					}
					if ((object)V_1.get_Value().get_Type() != (object)outerMostDeclaringType)
					{
						V_2 = V_1.get_Value().get_TypeContext();
					}
					else
					{
						V_2 = this.CreateTypeContext(V_1.get_Value().get_Type(), language, decompiledTypes, V_1.get_Value());
					}
					this.cacheService.AddTypeContextToCache(V_1.get_Value().get_Type(), language, this.renameInvalidMembers, V_2);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private TypeSpecificContext CreateTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes, DecompiledType decompiledCurrentType)
		{
			V_0 = this.GetTypesDependingOn(decompiledTypes, language);
			V_1 = this.GetUsedNamespaces(V_0, type.get_Namespace());
			V_2 = this.GetTypeVisibleMembersNames(type, language, decompiledTypes);
			V_3 = this.GetExplicitlyImplementedInterfaceMethods(type, language);
			return new TypeSpecificContext(type, decompiledCurrentType.get_TypeContext().get_MethodDefinitionToNameMap(), decompiledCurrentType.get_TypeContext().get_BackingFieldToNameMap(), V_1, V_2, decompiledCurrentType.get_TypeContext().get_AssignmentData(), this.GetAutoImplementedProperties(decompiledTypes), this.GetAutoImplementedEvents(decompiledTypes), V_3, this.get_ExceptionsWhileDecompiling(), decompiledCurrentType.get_TypeContext().get_GeneratedFilterMethods(), decompiledCurrentType.get_TypeContext().get_GeneratedMethodDefinitionToNameMap());
		}

		protected void DecompileConstructorChain(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (this.cacheService.IsDecompiledMemberInCache(method, language, this.renameInvalidMembers))
			{
				V_3 = this.cacheService.GetDecompiledMemberFromCache(method, language, this.renameInvalidMembers);
				this.AddDecompiledMemberToDecompiledType(V_3, decompiledType);
				BaseWriterContextService.AddAssignmentDataToDecompiledType(V_3, decompiledType);
				return;
			}
			if (method.get_Body() == null)
			{
				V_4 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
				this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, V_4);
				return;
			}
			V_0 = this.DecompileMethod(language, method, decompiledType.get_TypeContext().ShallowPartialClone());
			V_1 = new List<CachedDecompiledMember>();
			V_2 = method.get_DeclaringType();
			if (!method.get_IsStatic())
			{
				V_5 = V_2.get_Methods().GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_6 = V_5.get_Current();
						if (!V_6.get_IsConstructor() || String.op_Equality(V_6.get_FullName(), V_0.get_Member().get_MemberFullName()) || V_6.get_IsStatic())
						{
							continue;
						}
						if (V_6.get_Body() != null)
						{
							V_1.Add(this.DecompileMethod(language, V_6, decompiledType.get_TypeContext().ShallowPartialClone()));
						}
						else
						{
							V_7 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(V_6), null, null));
							V_1.Add(V_7);
						}
					}
				}
				finally
				{
					V_5.Dispose();
				}
				V_1.Add(V_0);
				this.MergeConstructorsTypeContexts(V_1, decompiledType);
				V_8 = V_1.GetEnumerator();
				try
				{
					while (V_8.MoveNext())
					{
						V_9 = new BaseWriterContextService.u003cu003ec__DisplayClass15_0();
						V_9.constructor = V_8.get_Current();
						if (language as IntermediateLanguage == null)
						{
							this.RemoveBaseCtorInvocationStatements(V_9.constructor, decompiledType);
						}
						if (V_9.constructor.get_Member().get_Context() != null)
						{
							if (!this.cacheService.IsDecompiledMemberInCache(V_9.constructor.get_Member().get_Context().get_Method(), language, this.renameInvalidMembers))
							{
								this.cacheService.AddDecompiledMemberToCache(V_9.constructor.get_Member().get_Context().get_Method(), language, this.renameInvalidMembers, V_9.constructor);
							}
						}
						else
						{
							V_10 = decompiledType.get_Type().get_Methods().First<MethodDefinition>(new Func<MethodDefinition, bool>(V_9.u003cDecompileConstructorChainu003eb__0));
							if (!this.cacheService.IsDecompiledMemberInCache(V_10, language, this.renameInvalidMembers))
							{
								this.cacheService.AddDecompiledMemberToCache(V_10, language, this.renameInvalidMembers, V_9.constructor);
							}
						}
						this.AddDecompiledMemberToDecompiledType(V_9.constructor, decompiledType);
					}
				}
				finally
				{
					((IDisposable)V_8).Dispose();
				}
			}
			return;
		}

		protected void DecompileMember(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (method.get_IsConstructor() && !method.get_IsStatic() && method.get_HasBody())
			{
				this.DecompileConstructorChain(method, language, decompiledType);
				return;
			}
			this.AddDecompiledMemberToDecompiledType(this.GetDecompiledMember(method, language, decompiledType), decompiledType);
			return;
		}

		private CachedDecompiledMember DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext)
		{
			try
			{
				V_2 = null;
				V_1 = method.get_Body().Decompile(language, out V_2, typeContext);
				V_0 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), V_1, V_2.get_MethodContext()), V_2.get_TypeContext());
			}
			catch (Exception exception_0)
			{
				V_3 = exception_0;
				this.get_ExceptionsWhileDecompiling().Add(method);
				V_4 = new BlockStatement();
				V_4.AddStatement(new ExceptionStatement(V_3, method));
				V_0 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), V_4, new MethodSpecificContext(method.get_Body())));
				this.OnExceptionThrown(V_3);
			}
			return V_0;
		}

		public abstract AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language);

		protected ICollection<string> GetAssemblyNamespaceUsings(AssemblyDefinition assembly)
		{
			return this.GetUsedNamespaces(AttributesUtilities.GetAssemblyAttributesUsedTypes(assembly), "");
		}

		protected HashSet<EventDefinition> GetAutoImplementedEvents(Dictionary<string, DecompiledType> decompiledTypes)
		{
			V_0 = new HashSet<EventDefinition>();
			V_1 = decompiledTypes.get_Values().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.UnionWith(V_2.get_TypeContext().get_AutoImplementedEvents());
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		protected HashSet<PropertyDefinition> GetAutoImplementedProperties(Dictionary<string, DecompiledType> decompiledTypes)
		{
			V_0 = new HashSet<PropertyDefinition>();
			V_1 = decompiledTypes.get_Values().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.UnionWith(V_2.get_TypeContext().get_AutoImplementedProperties());
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		protected CachedDecompiledMember GetDecompiledMember(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (method.get_Body() == null)
			{
				return new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
			}
			if (this.cacheService.IsDecompiledMemberInCache(method, language, this.renameInvalidMembers))
			{
				return this.cacheService.GetDecompiledMemberFromCache(method, language, this.renameInvalidMembers);
			}
			V_0 = this.DecompileMethod(language, method, decompiledType.get_TypeContext());
			this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, V_0);
			return V_0;
		}

		private ICollection<TypeReference> GetEventTypesDependingOn(EventDefinition event, ILanguage language)
		{
			V_0 = new HashSet<TypeReference>();
			V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(event.get_EventType()));
			V_0.UnionWith(AttributesUtilities.GetEventAttributesUsedTypes(event, language));
			if (event.get_AddMethod() != null)
			{
				V_0.UnionWith(this.GetMethodTypesDependingOn(event.get_AddMethod(), language, false));
			}
			if (event.get_RemoveMethod() != null)
			{
				V_0.UnionWith(this.GetMethodTypesDependingOn(event.get_RemoveMethod(), language, false));
			}
			if (event.get_InvokeMethod() != null)
			{
				V_0.UnionWith(this.GetMethodTypesDependingOn(event.get_InvokeMethod(), language, false));
			}
			return V_0;
		}

		protected ExplicitlyImplementedMembersCollection GetExplicitlyImplementedInterfaceMethods(TypeDefinition type, ILanguage language)
		{
			V_0 = new ExplicitlyImplementedMembersCollection();
			V_1 = TypeDefinitionExtensions.GetMembersUnordered(type, true).GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as MethodDefinition != null)
					{
						this.AddExplicitlyImplementedMembers((V_2 as MethodDefinition).GetExplicitlyImplementedMethods(), V_0);
					}
					if (V_2 as PropertyDefinition != null)
					{
						this.AddExplicitlyImplementedMembers((V_2 as PropertyDefinition).GetExplicitlyImplementedProperties(), V_0);
					}
					if (V_2 as EventDefinition == null)
					{
						continue;
					}
					this.AddExplicitlyImplementedMembers((V_2 as EventDefinition).GetExplicitlyImplementedEvents(), V_0);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private ICollection<TypeReference> GetFieldTypesDependingOn(FieldDefinition field)
		{
			stackVariable0 = new HashSet<TypeReference>();
			stackVariable0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(field.get_FieldType()));
			stackVariable0.UnionWith(AttributesUtilities.GetFieldAttributesUsedTypes(field));
			return stackVariable0;
		}

		protected virtual MemberRenamingData GetMemberRenamingData(ModuleDefinition module, ILanguage language)
		{
			return (new DefaultMemberRenamingService(language, this.renameInvalidMembers)).GetMemberRenamingData(module);
		}

		private ICollection<TypeReference> GetMethodTypesDependingOn(MethodDefinition method, ILanguage language, bool considerAttributes = true)
		{
			V_0 = new HashSet<TypeReference>();
			if (method.get_ReturnType() != null)
			{
				V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(method.get_ReturnType()));
			}
			if (considerAttributes)
			{
				V_0.UnionWith(AttributesUtilities.GetMethodAttributesUsedTypes(method, language));
			}
			V_1 = method.get_Parameters().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(V_2.get_ParameterType()));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		public abstract ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language);

		protected ICollection<string> GetModuleNamespaceUsings(ModuleDefinition module)
		{
			return this.GetUsedNamespaces(AttributesUtilities.GetModuleAttributesUsedTypes(module), "");
		}

		protected Dictionary<string, DecompiledType> GetNestedDecompiledTypes(TypeDefinition type, ILanguage language)
		{
			V_0 = new Dictionary<string, DecompiledType>();
			V_1 = new Queue<IMemberDefinition>();
			V_1.Enqueue(type);
			while (V_1.get_Count() > 0)
			{
				V_2 = V_1.Dequeue();
				if (V_2 as TypeDefinition == null)
				{
					if (!V_0.TryGetValue(V_2.get_DeclaringType().get_FullName(), out V_7))
					{
						throw new Exception("Type missing from nested types decompilation cache.");
					}
					if (V_2 as MethodDefinition != null)
					{
						this.DecompileMember(V_2 as MethodDefinition, language, V_7);
					}
					if (V_2 as EventDefinition != null)
					{
						V_9 = V_2 as EventDefinition;
						if ((new AutoImplementedEventMatcher(V_9, language)).IsAutoImplemented())
						{
							dummyVar0 = V_7.get_TypeContext().get_AutoImplementedEvents().Add(V_9);
						}
						if (V_9.get_AddMethod() != null)
						{
							this.DecompileMember(V_9.get_AddMethod(), language, V_7);
						}
						if (V_9.get_RemoveMethod() != null)
						{
							this.DecompileMember(V_9.get_RemoveMethod(), language, V_7);
						}
						if (V_9.get_InvokeMethod() != null)
						{
							this.DecompileMember(V_9.get_InvokeMethod(), language, V_7);
						}
					}
					if (V_2 as PropertyDefinition == null)
					{
						continue;
					}
					V_10 = V_2 as PropertyDefinition;
					stackVariable34 = new PropertyDecompiler(V_10, language, this.renameInvalidMembers, this.cacheService, V_7.get_TypeContext());
					stackVariable34.add_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
					stackVariable34.Decompile(out V_11, out V_12, out V_13);
					stackVariable34.remove_ExceptionThrown(new EventHandler<Exception>(this.OnExceptionThrown));
					if (V_13)
					{
						dummyVar1 = V_7.get_TypeContext().get_AutoImplementedProperties().Add(V_10);
					}
					if (V_11 != null)
					{
						this.AddDecompiledMemberToDecompiledType(V_11, V_7);
					}
					if (V_12 != null)
					{
						this.AddDecompiledMemberToDecompiledType(V_12, V_7);
					}
					V_14 = stackVariable34.get_ExceptionsWhileDecompiling().GetEnumerator();
					try
					{
						while (V_14.MoveNext())
						{
							V_15 = V_14.get_Current();
							this.get_ExceptionsWhileDecompiling().Add(V_15);
						}
					}
					finally
					{
						if (V_14 != null)
						{
							V_14.Dispose();
						}
					}
				}
				else
				{
					V_3 = V_2 as TypeDefinition;
					V_0.Add(V_3.get_FullName(), new DecompiledType(V_3));
					V_4 = Utilities.GetTypeMembersToDecompile(V_3).GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							V_1.Enqueue(V_5);
						}
					}
					finally
					{
						((IDisposable)V_4).Dispose();
					}
				}
			}
			V_16 = V_0.get_Values().GetEnumerator();
			try
			{
				while (V_16.MoveNext())
				{
					V_17 = V_16.get_Current();
					this.AddGeneratedFilterMethodsToDecompiledType(V_17, V_17.get_TypeContext(), language);
				}
			}
			finally
			{
				((IDisposable)V_16).Dispose();
			}
			return V_0;
		}

		private ICollection<TypeReference> GetPropertyTypesDependingOn(PropertyDefinition property, ILanguage language)
		{
			V_0 = new HashSet<TypeReference>();
			V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(property.get_PropertyType()));
			V_0.UnionWith(AttributesUtilities.GetPropertyAttributesUsedTypes(property, language));
			if (property.get_GetMethod() != null)
			{
				V_0.UnionWith(this.GetMethodTypesDependingOn(property.get_GetMethod(), language, false));
			}
			if (property.get_SetMethod() != null)
			{
				V_0.UnionWith(this.GetMethodTypesDependingOn(property.get_SetMethod(), language, false));
			}
			return V_0;
		}

		protected virtual TypeSpecificContext GetTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			if (!decompiledTypes.TryGetValue(type.get_FullName(), out V_0))
			{
				throw new Exception("Decompiled type not found in decompiled types cache.");
			}
			if (!this.cacheService.IsTypeContextInCache(type, language, this.renameInvalidMembers))
			{
				V_1 = this.CreateTypeContext(type, language, decompiledTypes, V_0);
				this.cacheService.AddTypeContextToCache(type, language, this.renameInvalidMembers, V_1);
			}
			else
			{
				if (V_0.get_TypeContext().get_GeneratedFilterMethods().get_Count() > 0)
				{
					this.cacheService.ReplaceCachedTypeContext(type, language, this.renameInvalidMembers, V_0.get_TypeContext());
				}
				V_1 = this.cacheService.GetTypeContextFromCache(type, language, this.renameInvalidMembers);
			}
			return V_1;
		}

		private ICollection<TypeReference> GetTypesDependingOn(Dictionary<string, DecompiledType> decompiledTypes, ILanguage language)
		{
			V_0 = new HashSet<TypeReference>();
			V_1 = decompiledTypes.get_Values().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_DecompiledMembers().GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							if (V_4.get_Value().get_Context() == null)
							{
								continue;
							}
							V_0.UnionWith(V_4.get_Value().get_Context().get_AnalysisResults().get_TypesDependingOn());
							if (V_4.get_Value().get_Context().get_Method() == null || V_4.get_Value().get_Context().get_Method().get_Body() == null || !V_4.get_Value().get_Context().get_Method().get_Body().get_HasVariables())
							{
								continue;
							}
							V_5 = V_4.get_Value().get_Context().get_Method().get_Body().get_Variables().GetEnumerator();
							try
							{
								while (V_5.MoveNext())
								{
									V_6 = V_5.get_Current();
									if (V_0.Contains(V_6.get_VariableType()))
									{
										continue;
									}
									dummyVar0 = V_0.Add(V_6.get_VariableType());
								}
							}
							finally
							{
								V_5.Dispose();
							}
						}
					}
					finally
					{
						((IDisposable)V_3).Dispose();
					}
					if (V_2.get_Type().get_BaseType() != null)
					{
						V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(V_2.get_Type().get_BaseType()));
					}
					if (V_2.get_Type().get_HasGenericParameters())
					{
						V_7 = V_2.get_Type().get_GenericParameters().GetEnumerator();
						try
						{
							while (V_7.MoveNext())
							{
								V_8 = V_7.get_Current();
								if (!V_8.get_HasConstraints())
								{
									continue;
								}
								V_9 = V_8.get_Constraints().GetEnumerator();
								try
								{
									while (V_9.MoveNext())
									{
										V_10 = V_9.get_Current();
										V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(V_10));
									}
								}
								finally
								{
									V_9.Dispose();
								}
							}
						}
						finally
						{
							V_7.Dispose();
						}
					}
					V_9 = V_2.get_Type().get_Interfaces().GetEnumerator();
					try
					{
						while (V_9.MoveNext())
						{
							V_11 = V_9.get_Current();
							V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(V_11));
						}
					}
					finally
					{
						V_9.Dispose();
					}
					V_0.UnionWith(AttributesUtilities.GetTypeAttributesUsedTypes(V_2.get_Type()));
					V_12 = Utilities.GetTypeMembers(V_2.get_Type(), language, true, null, null, null, V_2.get_TypeContext().GetFieldToPropertyMap(language).get_Keys()).GetEnumerator();
					try
					{
						while (V_12.MoveNext())
						{
							V_13 = V_12.get_Current();
							if (V_13 as MethodDefinition != null)
							{
								V_0.UnionWith(this.GetMethodTypesDependingOn(V_13 as MethodDefinition, language, true));
							}
							if (V_13 as PropertyDefinition != null)
							{
								V_0.UnionWith(this.GetPropertyTypesDependingOn(V_13 as PropertyDefinition, language));
							}
							if (V_13 as EventDefinition != null)
							{
								V_0.UnionWith(this.GetEventTypesDependingOn(V_13 as EventDefinition, language));
							}
							if (V_13 as FieldDefinition == null)
							{
								continue;
							}
							V_0.UnionWith(this.GetFieldTypesDependingOn(V_13 as FieldDefinition));
						}
					}
					finally
					{
						((IDisposable)V_12).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private ICollection<string> GetTypeVisibleMembersNames(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			V_0 = new HashSet<string>(language.get_IdentifierComparer());
			V_1 = new Queue<TypeDefinition>();
			V_1.Enqueue(type);
			while (V_1.get_Count() > 0)
			{
				V_2 = V_1.Dequeue();
				if (V_2.get_BaseType() != null)
				{
					V_6 = V_2.get_BaseType().Resolve();
					if (V_6 != null && V_6.get_IsPublic() || (object)V_6.get_Module().get_Assembly() == (object)type.get_Module().get_Assembly())
					{
						V_1.Enqueue(V_6);
					}
				}
				if (V_2.get_HasInterfaces())
				{
					V_7 = V_2.get_Interfaces().GetEnumerator();
					try
					{
						while (V_7.MoveNext())
						{
							V_8 = V_7.get_Current().Resolve();
							if (V_8 == null || !V_8.get_IsPublic() && (object)V_8.get_Module().get_Assembly() != (object)type.get_Module().get_Assembly())
							{
								continue;
							}
							V_1.Enqueue(V_8);
						}
					}
					finally
					{
						V_7.Dispose();
					}
				}
				V_5 = null;
				if (!decompiledTypes.TryGetValue(V_2.get_FullName(), out V_3))
				{
					if (this.TryGetTypeContextFromCache(V_2, language, out V_4))
					{
						V_5 = new List<FieldDefinition>(V_4.GetFieldToPropertyMap(language).get_Keys());
					}
				}
				else
				{
					V_5 = new List<FieldDefinition>(V_3.get_TypeContext().GetFieldToPropertyMap(language).get_Keys());
				}
				V_9 = Utilities.GetTypeMembers(V_2, language, true, null, null, null, V_5).GetEnumerator();
				try
				{
					while (V_9.MoveNext())
					{
						V_10 = V_9.get_Current();
						if (V_10 as PropertyDefinition != null)
						{
							V_11 = V_10 as PropertyDefinition;
							if (V_11.get_GetMethod() != null && !V_11.get_GetMethod().get_IsPrivate() || (object)V_11.get_DeclaringType() == (object)type && !V_0.Contains(V_11.get_Name()))
							{
								dummyVar0 = V_0.Add(V_11.get_Name());
							}
							if (V_11.get_SetMethod() != null && !V_11.get_SetMethod().get_IsPrivate() || (object)V_11.get_DeclaringType() == (object)type && !V_0.Contains(V_11.get_Name()))
							{
								dummyVar1 = V_0.Add(V_11.get_Name());
							}
						}
						if (V_10 as FieldDefinition != null)
						{
							V_12 = V_10 as FieldDefinition;
							if (!V_12.get_IsPrivate() || (object)V_12.get_DeclaringType() == (object)type && !V_0.Contains(V_12.get_Name()))
							{
								dummyVar2 = V_0.Add(V_12.get_Name());
							}
						}
						if (V_10 as EventDefinition != null && !V_0.Contains(V_10.get_Name()))
						{
							dummyVar3 = V_0.Add(V_10.get_Name());
						}
						if (V_10 as TypeDefinition == null || V_0.Contains(V_10.get_Name()))
						{
							continue;
						}
						dummyVar4 = V_0.Add(V_10.get_Name());
					}
				}
				finally
				{
					((IDisposable)V_9).Dispose();
				}
			}
			return V_0;
		}

		protected ICollection<string> GetUsedNamespaces(ICollection<TypeReference> typesDependingOn, string currentNamespace = "")
		{
			V_0 = new HashSet<string>();
			V_1 = typesDependingOn.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					while (V_2.get_IsNested())
					{
						V_2 = V_2.get_DeclaringType();
					}
					V_3 = V_2.get_Namespace();
					if (!String.op_Inequality(V_3, String.Empty) || !String.op_Inequality(V_3, currentNamespace) || V_0.Contains(V_3))
					{
						continue;
					}
					V_0.Add(V_3);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public abstract WriterContext GetWriterContext(IMemberDefinition member, ILanguage language);

		private void MergeConstructorsTypeContexts(List<CachedDecompiledMember> allConstructors, DecompiledType decompiledType)
		{
			if (allConstructors == null || allConstructors.get_Count() == 0)
			{
				return;
			}
			V_2 = 0;
			while (V_2 < allConstructors.get_Count())
			{
				if (allConstructors.get_Item(V_2).get_Member().get_Context() != null && allConstructors.get_Item(V_2).get_Member().get_Context().get_IsBaseConstructorInvokingConstructor())
				{
					decompiledType.get_TypeContext().get_BaseCtorInvocators().Add(allConstructors.get_Item(V_2).get_Member().get_Context().get_Method());
				}
				V_2 = V_2 + 1;
			}
			V_0 = 0;
			V_1 = new Dictionary<string, InitializationAssignment>();
			while (true)
			{
				if (V_0 < allConstructors.get_Count())
				{
					if (allConstructors.get_Item(V_0).get_Member().get_Context() == null || !allConstructors.get_Item(V_0).get_Member().get_Context().get_IsBaseConstructorInvokingConstructor())
					{
						V_0 = V_0 + 1;
					}
					else
					{
						V_1 = new Dictionary<string, InitializationAssignment>(allConstructors.get_Item(V_0).get_FieldAssignmentData());
						break;
					}
				}
				else
				{
					break;
				}
			}
			while (true)
			{
				if (V_0 < allConstructors.get_Count())
				{
					V_3 = allConstructors.get_Item(V_0);
					if (V_3.get_Member().get_Context() != null && V_3.get_Member().get_Context().get_IsBaseConstructorInvokingConstructor())
					{
						if (V_3.get_FieldAssignmentData().get_Count() != V_1.get_Count())
						{
							decompiledType.get_TypeContext().set_FieldInitializationFailed(true);
							decompiledType.get_TypeContext().set_AssignmentData(new Dictionary<string, InitializationAssignment>());
							return;
						}
						V_4 = V_3.get_FieldAssignmentData().GetEnumerator();
						try
						{
							while (V_4.MoveNext())
							{
								V_5 = V_4.get_Current();
								if (V_1.ContainsKey(V_5.get_Key()) && V_1.get_Item(V_5.get_Key()).get_AssignmentExpression().Equals(V_5.get_Value().get_AssignmentExpression()))
								{
									continue;
								}
								decompiledType.get_TypeContext().set_FieldInitializationFailed(true);
								decompiledType.get_TypeContext().set_AssignmentData(new Dictionary<string, InitializationAssignment>());
								goto Label0;
							}
						}
						finally
						{
							((IDisposable)V_4).Dispose();
						}
					}
					V_0 = V_0 + 1;
				}
				else
				{
					decompiledType.get_TypeContext().set_AssignmentData(V_1);
					break;
				}
			}
		Label0:
			return;
		}

		private void RemoveBaseCtorInvocationStatements(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			V_0 = decompiledMember.get_Member().get_Context();
			if (V_0 == null || !V_0.get_Method().get_IsConstructor() || V_0.get_Method().get_IsStatic() || V_0.get_CtorInvokeExpression() == null)
			{
				return;
			}
			V_1 = decompiledMember.get_Member().get_Statement() as BlockStatement;
			if (V_1 == null)
			{
				return;
			}
			if (V_1.get_Statements().get_Count() == 1 && V_1.get_Statements().get_Item(0) as UnsafeBlockStatement != null)
			{
				V_1 = V_1.get_Statements().get_Item(0) as UnsafeBlockStatement;
			}
			V_2 = decompiledType.get_TypeContext();
			if (V_2.get_FieldInitializationFailed() && V_2.get_BaseCtorInvocators().Contains(V_0.get_Method()))
			{
				V_0.set_CtorInvokeExpression(null);
				return;
			}
			V_3 = 0;
			while (V_3 < V_1.get_Statements().get_Count())
			{
				if (V_1.get_Statements().get_Item(V_3).get_CodeNodeType() == 5 && (V_1.get_Statements().get_Item(V_3) as ExpressionStatement).get_Expression().get_CodeNodeType() == 53 || (V_1.get_Statements().get_Item(V_3) as ExpressionStatement).get_Expression().get_CodeNodeType() == 52)
				{
					this.RemoveFirstStatements(V_1.get_Statements(), V_3 + 1);
					return;
				}
				V_3 = V_3 + 1;
			}
			throw new Exception("Constructor invocation not found.");
		}

		private void RemoveFirstStatements(StatementCollection statements, int count)
		{
			V_0 = 0;
			while (V_0 + count < statements.get_Count())
			{
				statements.set_Item(V_0, statements.get_Item(V_0 + count));
				V_0 = V_0 + 1;
			}
			while (true)
			{
				stackVariable16 = count;
				count = stackVariable16 - 1;
				if (stackVariable16 <= 0)
				{
					break;
				}
				statements.RemoveAt(statements.get_Count() - 1);
			}
			return;
		}

		private bool TryGetTypeContextFromCache(TypeDefinition type, ILanguage language, out TypeSpecificContext typeContext)
		{
			typeContext = null;
			if (this.cacheService.IsTypeContextInCache(type, language, this.renameInvalidMembers))
			{
				typeContext = this.cacheService.GetTypeContextFromCache(type, language, this.renameInvalidMembers);
			}
			return typeContext != null;
		}
	}
}