using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.IL;

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
				return JustDecompileGenerated_get_ExceptionsWhileDecompiling();
			}
			set
			{
				JustDecompileGenerated_set_ExceptionsWhileDecompiling(value);
			}
		}

		private ICollection<MethodDefinition> JustDecompileGenerated_ExceptionsWhileDecompiling_k__BackingField;

		public ICollection<MethodDefinition> JustDecompileGenerated_get_ExceptionsWhileDecompiling()
		{
			return this.JustDecompileGenerated_ExceptionsWhileDecompiling_k__BackingField;
		}

		private void JustDecompileGenerated_set_ExceptionsWhileDecompiling(ICollection<MethodDefinition> value)
		{
			this.JustDecompileGenerated_ExceptionsWhileDecompiling_k__BackingField = value;
		}

		public BaseWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
		{
			this.cacheService = cacheService;
			this.renameInvalidMembers = renameInvalidMembers;
			this.ExceptionsWhileDecompiling = new List<MethodDefinition>();
		}

		private static void AddAssignmentDataToDecompiledType(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			if (!decompiledType.TypeContext.FieldInitializationFailed)
			{
				foreach (KeyValuePair<string, InitializationAssignment> fieldAssignmentDatum in decompiledMember.FieldAssignmentData)
				{
					if (decompiledType.TypeContext.AssignmentData.ContainsKey(fieldAssignmentDatum.Key))
					{
						if (decompiledType.TypeContext.AssignmentData[fieldAssignmentDatum.Key].AssignmentExpression.Equals(fieldAssignmentDatum.Value.AssignmentExpression))
						{
							continue;
						}
						decompiledType.TypeContext.FieldInitializationFailed = true;
						decompiledType.TypeContext.AssignmentData = new Dictionary<string, InitializationAssignment>();
						return;
					}
					else
					{
						decompiledType.TypeContext.AssignmentData.Add(fieldAssignmentDatum.Key, fieldAssignmentDatum.Value);
					}
				}
			}
		}

		protected CachedDecompiledMember AddDecompiledMemberToCache(IMemberDefinition member, DecompiledMember decompiledMember, TypeSpecificContext typeContext, ILanguage language)
		{
			CachedDecompiledMember cachedDecompiledMember = new CachedDecompiledMember(decompiledMember, typeContext);
			if (!this.cacheService.IsDecompiledMemberInCache(member, language, this.renameInvalidMembers))
			{
				this.cacheService.AddDecompiledMemberToCache(member, language, this.renameInvalidMembers, cachedDecompiledMember);
			}
			return cachedDecompiledMember;
		}

		protected void AddDecompiledMemberToDecompiledType(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			if (!decompiledType.DecompiledMembers.ContainsKey(decompiledMember.Member.MemberFullName))
			{
				decompiledType.DecompiledMembers.Add(decompiledMember.Member.MemberFullName, decompiledMember.Member);
			}
		}

		private void AddExplicitlyImplementedMembers(ICollection<ImplementedMember> explicitlyImplementedMembers, ExplicitlyImplementedMembersCollection collection)
		{
			foreach (ImplementedMember explicitlyImplementedMember in explicitlyImplementedMembers)
			{
				if (collection.Contains(explicitlyImplementedMember.DeclaringType, explicitlyImplementedMember.Member.get_FullName()))
				{
					continue;
				}
				collection.Add(explicitlyImplementedMember.DeclaringType, explicitlyImplementedMember.Member.get_FullName());
			}
		}

		protected void AddGeneratedFilterMethodsToDecompiledType(DecompiledType decompiledType, TypeSpecificContext context, ILanguage language)
		{
			foreach (GeneratedMethod generatedFilterMethod in context.GeneratedFilterMethods)
			{
				CachedDecompiledMember cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(generatedFilterMethod.Method), generatedFilterMethod.Body, generatedFilterMethod.Context));
				this.AddDecompiledMemberToDecompiledType(cachedDecompiledMember, decompiledType);
			}
		}

		protected void AddTypeContextsToCache(Dictionary<string, DecompiledType> decompiledTypes, TypeDefinition outerMostDeclaringType, ILanguage language)
		{
			TypeSpecificContext typeSpecificContext;
			foreach (KeyValuePair<string, DecompiledType> decompiledType in decompiledTypes)
			{
				if (this.cacheService.IsTypeContextInCache(decompiledType.Value.Type, language, this.renameInvalidMembers))
				{
					continue;
				}
				typeSpecificContext = ((object)decompiledType.Value.Type != (object)outerMostDeclaringType ? decompiledType.Value.TypeContext : this.CreateTypeContext(decompiledType.Value.Type, language, decompiledTypes, decompiledType.Value));
				this.cacheService.AddTypeContextToCache(decompiledType.Value.Type, language, this.renameInvalidMembers, typeSpecificContext);
			}
		}

		private TypeSpecificContext CreateTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes, DecompiledType decompiledCurrentType)
		{
			ICollection<TypeReference> typesDependingOn = this.GetTypesDependingOn(decompiledTypes, language);
			ICollection<string> usedNamespaces = this.GetUsedNamespaces(typesDependingOn, type.get_Namespace());
			ICollection<string> typeVisibleMembersNames = this.GetTypeVisibleMembersNames(type, language, decompiledTypes);
			ExplicitlyImplementedMembersCollection explicitlyImplementedInterfaceMethods = this.GetExplicitlyImplementedInterfaceMethods(type, language);
			return new TypeSpecificContext(type, decompiledCurrentType.TypeContext.MethodDefinitionToNameMap, decompiledCurrentType.TypeContext.BackingFieldToNameMap, usedNamespaces, typeVisibleMembersNames, decompiledCurrentType.TypeContext.AssignmentData, this.GetAutoImplementedProperties(decompiledTypes), this.GetAutoImplementedEvents(decompiledTypes), explicitlyImplementedInterfaceMethods, this.ExceptionsWhileDecompiling, decompiledCurrentType.TypeContext.GeneratedFilterMethods, decompiledCurrentType.TypeContext.GeneratedMethodDefinitionToNameMap);
		}

		protected void DecompileConstructorChain(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (this.cacheService.IsDecompiledMemberInCache(method, language, this.renameInvalidMembers))
			{
				CachedDecompiledMember decompiledMemberFromCache = this.cacheService.GetDecompiledMemberFromCache(method, language, this.renameInvalidMembers);
				this.AddDecompiledMemberToDecompiledType(decompiledMemberFromCache, decompiledType);
				BaseWriterContextService.AddAssignmentDataToDecompiledType(decompiledMemberFromCache, decompiledType);
				return;
			}
			if (method.get_Body() == null)
			{
				CachedDecompiledMember cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
				this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, cachedDecompiledMember);
				return;
			}
			CachedDecompiledMember cachedDecompiledMember1 = this.DecompileMethod(language, method, decompiledType.TypeContext.ShallowPartialClone());
			List<CachedDecompiledMember> cachedDecompiledMembers = new List<CachedDecompiledMember>();
			TypeDefinition declaringType = method.get_DeclaringType();
			if (!method.get_IsStatic())
			{
				foreach (MethodDefinition methodDefinition in declaringType.get_Methods())
				{
					if (!methodDefinition.get_IsConstructor() || methodDefinition.get_FullName() == cachedDecompiledMember1.Member.MemberFullName || methodDefinition.get_IsStatic())
					{
						continue;
					}
					if (methodDefinition.get_Body() != null)
					{
						cachedDecompiledMembers.Add(this.DecompileMethod(language, methodDefinition, decompiledType.TypeContext.ShallowPartialClone()));
					}
					else
					{
						CachedDecompiledMember cachedDecompiledMember2 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(methodDefinition), null, null));
						cachedDecompiledMembers.Add(cachedDecompiledMember2);
					}
				}
				cachedDecompiledMembers.Add(cachedDecompiledMember1);
				this.MergeConstructorsTypeContexts(cachedDecompiledMembers, decompiledType);
				foreach (CachedDecompiledMember cachedDecompiledMember3 in cachedDecompiledMembers)
				{
					if (!(language is IntermediateLanguage))
					{
						this.RemoveBaseCtorInvocationStatements(cachedDecompiledMember3, decompiledType);
					}
					if (cachedDecompiledMember3.Member.Context == null)
					{
						MethodDefinition methodDefinition1 = decompiledType.Type.get_Methods().First<MethodDefinition>((MethodDefinition x) => x.get_FullName() == cachedDecompiledMember3.Member.MemberFullName);
						if (!this.cacheService.IsDecompiledMemberInCache(methodDefinition1, language, this.renameInvalidMembers))
						{
							this.cacheService.AddDecompiledMemberToCache(methodDefinition1, language, this.renameInvalidMembers, cachedDecompiledMember3);
						}
					}
					else if (!this.cacheService.IsDecompiledMemberInCache(cachedDecompiledMember3.Member.Context.Method, language, this.renameInvalidMembers))
					{
						this.cacheService.AddDecompiledMemberToCache(cachedDecompiledMember3.Member.Context.Method, language, this.renameInvalidMembers, cachedDecompiledMember3);
					}
					this.AddDecompiledMemberToDecompiledType(cachedDecompiledMember3, decompiledType);
				}
			}
		}

		protected void DecompileMember(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (method.get_IsConstructor() && !method.get_IsStatic() && method.get_HasBody())
			{
				this.DecompileConstructorChain(method, language, decompiledType);
				return;
			}
			this.AddDecompiledMemberToDecompiledType(this.GetDecompiledMember(method, language, decompiledType), decompiledType);
		}

		private CachedDecompiledMember DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext)
		{
			CachedDecompiledMember cachedDecompiledMember;
			try
			{
				DecompilationContext decompilationContext = null;
				Statement statement = method.get_Body().Decompile(language, out decompilationContext, typeContext);
				cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), statement, decompilationContext.MethodContext), decompilationContext.TypeContext);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.ExceptionsWhileDecompiling.Add(method);
				BlockStatement blockStatement = new BlockStatement();
				blockStatement.AddStatement(new ExceptionStatement(exception, method));
				cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, new MethodSpecificContext(method.get_Body())));
				base.OnExceptionThrown(exception);
			}
			return cachedDecompiledMember;
		}

		public abstract AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language);

		protected ICollection<string> GetAssemblyNamespaceUsings(AssemblyDefinition assembly)
		{
			return this.GetUsedNamespaces(AttributesUtilities.GetAssemblyAttributesUsedTypes(assembly), "");
		}

		protected HashSet<EventDefinition> GetAutoImplementedEvents(Dictionary<string, DecompiledType> decompiledTypes)
		{
			HashSet<EventDefinition> eventDefinitions = new HashSet<EventDefinition>();
			foreach (DecompiledType value in decompiledTypes.Values)
			{
				eventDefinitions.UnionWith(value.TypeContext.AutoImplementedEvents);
			}
			return eventDefinitions;
		}

		protected HashSet<PropertyDefinition> GetAutoImplementedProperties(Dictionary<string, DecompiledType> decompiledTypes)
		{
			HashSet<PropertyDefinition> propertyDefinitions = new HashSet<PropertyDefinition>();
			foreach (DecompiledType value in decompiledTypes.Values)
			{
				propertyDefinitions.UnionWith(value.TypeContext.AutoImplementedProperties);
			}
			return propertyDefinitions;
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
			CachedDecompiledMember cachedDecompiledMember = this.DecompileMethod(language, method, decompiledType.TypeContext);
			this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, cachedDecompiledMember);
			return cachedDecompiledMember;
		}

		private ICollection<TypeReference> GetEventTypesDependingOn(EventDefinition @event, ILanguage language)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(@event.get_EventType()));
			typeReferences.UnionWith(AttributesUtilities.GetEventAttributesUsedTypes(@event, language));
			if (@event.get_AddMethod() != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(@event.get_AddMethod(), language, false));
			}
			if (@event.get_RemoveMethod() != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(@event.get_RemoveMethod(), language, false));
			}
			if (@event.get_InvokeMethod() != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(@event.get_InvokeMethod(), language, false));
			}
			return typeReferences;
		}

		protected ExplicitlyImplementedMembersCollection GetExplicitlyImplementedInterfaceMethods(TypeDefinition type, ILanguage language)
		{
			ExplicitlyImplementedMembersCollection explicitlyImplementedMembersCollection = new ExplicitlyImplementedMembersCollection();
			foreach (IMemberDefinition membersUnordered in TypeDefinitionExtensions.GetMembersUnordered(type, true))
			{
				if (membersUnordered is MethodDefinition)
				{
					this.AddExplicitlyImplementedMembers((membersUnordered as MethodDefinition).GetExplicitlyImplementedMethods(), explicitlyImplementedMembersCollection);
				}
				if (membersUnordered is PropertyDefinition)
				{
					this.AddExplicitlyImplementedMembers((membersUnordered as PropertyDefinition).GetExplicitlyImplementedProperties(), explicitlyImplementedMembersCollection);
				}
				if (!(membersUnordered is EventDefinition))
				{
					continue;
				}
				this.AddExplicitlyImplementedMembers((membersUnordered as EventDefinition).GetExplicitlyImplementedEvents(), explicitlyImplementedMembersCollection);
			}
			return explicitlyImplementedMembersCollection;
		}

		private ICollection<TypeReference> GetFieldTypesDependingOn(FieldDefinition field)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(field.get_FieldType()));
			typeReferences.UnionWith(AttributesUtilities.GetFieldAttributesUsedTypes(field));
			return typeReferences;
		}

		protected virtual MemberRenamingData GetMemberRenamingData(ModuleDefinition module, ILanguage language)
		{
			return (new DefaultMemberRenamingService(language, this.renameInvalidMembers)).GetMemberRenamingData(module);
		}

		private ICollection<TypeReference> GetMethodTypesDependingOn(MethodDefinition method, ILanguage language, bool considerAttributes = true)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			if (method.get_ReturnType() != null)
			{
				typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(method.get_ReturnType()));
			}
			if (considerAttributes)
			{
				typeReferences.UnionWith(AttributesUtilities.GetMethodAttributesUsedTypes(method, language));
			}
			foreach (ParameterDefinition parameter in method.get_Parameters())
			{
				typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(parameter.get_ParameterType()));
			}
			return typeReferences;
		}

		public abstract ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language);

		protected ICollection<string> GetModuleNamespaceUsings(ModuleDefinition module)
		{
			return this.GetUsedNamespaces(AttributesUtilities.GetModuleAttributesUsedTypes(module), "");
		}

		protected Dictionary<string, DecompiledType> GetNestedDecompiledTypes(TypeDefinition type, ILanguage language)
		{
			DecompiledType decompiledType;
			CachedDecompiledMember cachedDecompiledMember;
			CachedDecompiledMember cachedDecompiledMember1;
			bool flag;
			Dictionary<string, DecompiledType> strs = new Dictionary<string, DecompiledType>();
			Queue<IMemberDefinition> memberDefinitions = new Queue<IMemberDefinition>();
			memberDefinitions.Enqueue(type);
			while (memberDefinitions.Count > 0)
			{
				IMemberDefinition memberDefinition = memberDefinitions.Dequeue();
				if (!(memberDefinition is TypeDefinition))
				{
					if (!strs.TryGetValue(memberDefinition.get_DeclaringType().get_FullName(), out decompiledType))
					{
						throw new Exception("Type missing from nested types decompilation cache.");
					}
					if (memberDefinition is MethodDefinition)
					{
						this.DecompileMember(memberDefinition as MethodDefinition, language, decompiledType);
					}
					if (memberDefinition is EventDefinition)
					{
						EventDefinition eventDefinition = memberDefinition as EventDefinition;
						if ((new AutoImplementedEventMatcher(eventDefinition, language)).IsAutoImplemented())
						{
							decompiledType.TypeContext.AutoImplementedEvents.Add(eventDefinition);
						}
						if (eventDefinition.get_AddMethod() != null)
						{
							this.DecompileMember(eventDefinition.get_AddMethod(), language, decompiledType);
						}
						if (eventDefinition.get_RemoveMethod() != null)
						{
							this.DecompileMember(eventDefinition.get_RemoveMethod(), language, decompiledType);
						}
						if (eventDefinition.get_InvokeMethod() != null)
						{
							this.DecompileMember(eventDefinition.get_InvokeMethod(), language, decompiledType);
						}
					}
					if (!(memberDefinition is PropertyDefinition))
					{
						continue;
					}
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
						this.AddDecompiledMemberToDecompiledType(cachedDecompiledMember, decompiledType);
					}
					if (cachedDecompiledMember1 != null)
					{
						this.AddDecompiledMemberToDecompiledType(cachedDecompiledMember1, decompiledType);
					}
					foreach (MethodDefinition exceptionsWhileDecompiling in propertyDecompiler.ExceptionsWhileDecompiling)
					{
						this.ExceptionsWhileDecompiling.Add(exceptionsWhileDecompiling);
					}
				}
				else
				{
					TypeDefinition typeDefinition = memberDefinition as TypeDefinition;
					strs.Add(typeDefinition.get_FullName(), new DecompiledType(typeDefinition));
					foreach (IMemberDefinition typeMembersToDecompile in Utilities.GetTypeMembersToDecompile(typeDefinition))
					{
						memberDefinitions.Enqueue(typeMembersToDecompile);
					}
				}
			}
			foreach (DecompiledType value in strs.Values)
			{
				this.AddGeneratedFilterMethodsToDecompiledType(value, value.TypeContext, language);
			}
			return strs;
		}

		private ICollection<TypeReference> GetPropertyTypesDependingOn(PropertyDefinition property, ILanguage language)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(property.get_PropertyType()));
			typeReferences.UnionWith(AttributesUtilities.GetPropertyAttributesUsedTypes(property, language));
			if (property.get_GetMethod() != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(property.get_GetMethod(), language, false));
			}
			if (property.get_SetMethod() != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(property.get_SetMethod(), language, false));
			}
			return typeReferences;
		}

		protected virtual TypeSpecificContext GetTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			DecompiledType decompiledType;
			TypeSpecificContext typeContextFromCache;
			if (!decompiledTypes.TryGetValue(type.get_FullName(), out decompiledType))
			{
				throw new Exception("Decompiled type not found in decompiled types cache.");
			}
			if (!this.cacheService.IsTypeContextInCache(type, language, this.renameInvalidMembers))
			{
				typeContextFromCache = this.CreateTypeContext(type, language, decompiledTypes, decompiledType);
				this.cacheService.AddTypeContextToCache(type, language, this.renameInvalidMembers, typeContextFromCache);
			}
			else
			{
				if (decompiledType.TypeContext.GeneratedFilterMethods.Count > 0)
				{
					this.cacheService.ReplaceCachedTypeContext(type, language, this.renameInvalidMembers, decompiledType.TypeContext);
				}
				typeContextFromCache = this.cacheService.GetTypeContextFromCache(type, language, this.renameInvalidMembers);
			}
			return typeContextFromCache;
		}

		private ICollection<TypeReference> GetTypesDependingOn(Dictionary<string, DecompiledType> decompiledTypes, ILanguage language)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			foreach (DecompiledType value in decompiledTypes.Values)
			{
				foreach (KeyValuePair<string, DecompiledMember> decompiledMember in value.DecompiledMembers)
				{
					if (decompiledMember.Value.Context == null)
					{
						continue;
					}
					typeReferences.UnionWith(decompiledMember.Value.Context.AnalysisResults.TypesDependingOn);
					if (decompiledMember.Value.Context.Method == null || decompiledMember.Value.Context.Method.get_Body() == null || !decompiledMember.Value.Context.Method.get_Body().get_HasVariables())
					{
						continue;
					}
					foreach (VariableDefinition variable in decompiledMember.Value.Context.Method.get_Body().get_Variables())
					{
						if (typeReferences.Contains(variable.get_VariableType()))
						{
							continue;
						}
						typeReferences.Add(variable.get_VariableType());
					}
				}
				if (value.Type.get_BaseType() != null)
				{
					typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(value.Type.get_BaseType()));
				}
				if (value.Type.get_HasGenericParameters())
				{
					foreach (GenericParameter genericParameter in value.Type.get_GenericParameters())
					{
						if (!genericParameter.get_HasConstraints())
						{
							continue;
						}
						foreach (TypeReference constraint in genericParameter.get_Constraints())
						{
							typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(constraint));
						}
					}
				}
				foreach (TypeReference @interface in value.Type.get_Interfaces())
				{
					typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(@interface));
				}
				typeReferences.UnionWith(AttributesUtilities.GetTypeAttributesUsedTypes(value.Type));
				foreach (IMemberDefinition typeMember in Utilities.GetTypeMembers(value.Type, language, true, null, null, null, value.TypeContext.GetFieldToPropertyMap(language).Keys))
				{
					if (typeMember is MethodDefinition)
					{
						typeReferences.UnionWith(this.GetMethodTypesDependingOn(typeMember as MethodDefinition, language, true));
					}
					if (typeMember is PropertyDefinition)
					{
						typeReferences.UnionWith(this.GetPropertyTypesDependingOn(typeMember as PropertyDefinition, language));
					}
					if (typeMember is EventDefinition)
					{
						typeReferences.UnionWith(this.GetEventTypesDependingOn(typeMember as EventDefinition, language));
					}
					if (!(typeMember is FieldDefinition))
					{
						continue;
					}
					typeReferences.UnionWith(this.GetFieldTypesDependingOn(typeMember as FieldDefinition));
				}
			}
			return typeReferences;
		}

		private ICollection<string> GetTypeVisibleMembersNames(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			DecompiledType decompiledType;
			TypeSpecificContext typeSpecificContext;
			HashSet<string> strs = new HashSet<string>(language.IdentifierComparer);
			Queue<TypeDefinition> typeDefinitions = new Queue<TypeDefinition>();
			typeDefinitions.Enqueue(type);
			while (typeDefinitions.Count > 0)
			{
				TypeDefinition typeDefinition = typeDefinitions.Dequeue();
				if (typeDefinition.get_BaseType() != null)
				{
					TypeDefinition typeDefinition1 = typeDefinition.get_BaseType().Resolve();
					if (typeDefinition1 != null && (typeDefinition1.get_IsPublic() || (object)typeDefinition1.get_Module().get_Assembly() == (object)type.get_Module().get_Assembly()))
					{
						typeDefinitions.Enqueue(typeDefinition1);
					}
				}
				if (typeDefinition.get_HasInterfaces())
				{
					foreach (TypeReference @interface in typeDefinition.get_Interfaces())
					{
						TypeDefinition typeDefinition2 = @interface.Resolve();
						if (typeDefinition2 == null || !typeDefinition2.get_IsPublic() && (object)typeDefinition2.get_Module().get_Assembly() != (object)type.get_Module().get_Assembly())
						{
							continue;
						}
						typeDefinitions.Enqueue(typeDefinition2);
					}
				}
				List<FieldDefinition> fieldDefinitions = null;
				if (decompiledTypes.TryGetValue(typeDefinition.get_FullName(), out decompiledType))
				{
					fieldDefinitions = new List<FieldDefinition>(decompiledType.TypeContext.GetFieldToPropertyMap(language).Keys);
				}
				else if (this.TryGetTypeContextFromCache(typeDefinition, language, out typeSpecificContext))
				{
					fieldDefinitions = new List<FieldDefinition>(typeSpecificContext.GetFieldToPropertyMap(language).Keys);
				}
				foreach (IMemberDefinition typeMember in Utilities.GetTypeMembers(typeDefinition, language, true, null, null, null, fieldDefinitions))
				{
					if (typeMember is PropertyDefinition)
					{
						PropertyDefinition propertyDefinition = typeMember as PropertyDefinition;
						if (propertyDefinition.get_GetMethod() != null && (!propertyDefinition.get_GetMethod().get_IsPrivate() || (object)propertyDefinition.get_DeclaringType() == (object)type) && !strs.Contains(propertyDefinition.get_Name()))
						{
							strs.Add(propertyDefinition.get_Name());
						}
						if (propertyDefinition.get_SetMethod() != null && (!propertyDefinition.get_SetMethod().get_IsPrivate() || (object)propertyDefinition.get_DeclaringType() == (object)type) && !strs.Contains(propertyDefinition.get_Name()))
						{
							strs.Add(propertyDefinition.get_Name());
						}
					}
					if (typeMember is FieldDefinition)
					{
						FieldDefinition fieldDefinition = typeMember as FieldDefinition;
						if ((!fieldDefinition.get_IsPrivate() || (object)fieldDefinition.get_DeclaringType() == (object)type) && !strs.Contains(fieldDefinition.get_Name()))
						{
							strs.Add(fieldDefinition.get_Name());
						}
					}
					if (typeMember is EventDefinition && !strs.Contains(typeMember.get_Name()))
					{
						strs.Add(typeMember.get_Name());
					}
					if (!(typeMember is TypeDefinition) || strs.Contains(typeMember.get_Name()))
					{
						continue;
					}
					strs.Add(typeMember.get_Name());
				}
			}
			return strs;
		}

		protected ICollection<string> GetUsedNamespaces(ICollection<TypeReference> typesDependingOn, string currentNamespace = "")
		{
			ICollection<string> strs = new HashSet<string>();
			foreach (TypeReference declaringType in typesDependingOn)
			{
				while (declaringType.get_IsNested())
				{
					declaringType = declaringType.get_DeclaringType();
				}
				string @namespace = declaringType.get_Namespace();
				if (!(@namespace != String.Empty) || !(@namespace != currentNamespace) || strs.Contains(@namespace))
				{
					continue;
				}
				strs.Add(@namespace);
			}
			return strs;
		}

		public abstract WriterContext GetWriterContext(IMemberDefinition member, ILanguage language);

		private void MergeConstructorsTypeContexts(List<CachedDecompiledMember> allConstructors, DecompiledType decompiledType)
		{
			if (allConstructors == null || allConstructors.Count == 0)
			{
				return;
			}
			for (int i = 0; i < allConstructors.Count; i++)
			{
				if (allConstructors[i].Member.Context != null && allConstructors[i].Member.Context.IsBaseConstructorInvokingConstructor)
				{
					decompiledType.TypeContext.BaseCtorInvocators.Add(allConstructors[i].Member.Context.Method);
				}
			}
			int num = 0;
			Dictionary<string, InitializationAssignment> strs = new Dictionary<string, InitializationAssignment>();
			while (true)
			{
				if (num >= allConstructors.Count)
				{
					break;
				}
				else if (allConstructors[num].Member.Context == null || !allConstructors[num].Member.Context.IsBaseConstructorInvokingConstructor)
				{
					num++;
				}
				else
				{
					strs = new Dictionary<string, InitializationAssignment>(allConstructors[num].FieldAssignmentData);
					break;
				}
			}
			while (true)
			{
				if (num < allConstructors.Count)
				{
					CachedDecompiledMember item = allConstructors[num];
					if (item.Member.Context != null && item.Member.Context.IsBaseConstructorInvokingConstructor)
					{
						if (item.FieldAssignmentData.Count != strs.Count)
						{
							decompiledType.TypeContext.FieldInitializationFailed = true;
							decompiledType.TypeContext.AssignmentData = new Dictionary<string, InitializationAssignment>();
							return;
						}
						foreach (KeyValuePair<string, InitializationAssignment> fieldAssignmentDatum in item.FieldAssignmentData)
						{
							if (strs.ContainsKey(fieldAssignmentDatum.Key) && strs[fieldAssignmentDatum.Key].AssignmentExpression.Equals(fieldAssignmentDatum.Value.AssignmentExpression))
							{
								continue;
							}
							decompiledType.TypeContext.FieldInitializationFailed = true;
							decompiledType.TypeContext.AssignmentData = new Dictionary<string, InitializationAssignment>();
							return;
						}
					}
					num++;
				}
				else
				{
					decompiledType.TypeContext.AssignmentData = strs;
					break;
				}
			}
		}

		private void RemoveBaseCtorInvocationStatements(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			MethodSpecificContext context = decompiledMember.Member.Context;
			if (context == null || !context.Method.get_IsConstructor() || context.Method.get_IsStatic() || context.CtorInvokeExpression == null)
			{
				return;
			}
			BlockStatement statement = decompiledMember.Member.Statement as BlockStatement;
			if (statement == null)
			{
				return;
			}
			if (statement.Statements.Count == 1 && statement.Statements[0] is UnsafeBlockStatement)
			{
				statement = statement.Statements[0] as UnsafeBlockStatement;
			}
			TypeSpecificContext typeContext = decompiledType.TypeContext;
			if (typeContext.FieldInitializationFailed && typeContext.BaseCtorInvocators.Contains(context.Method))
			{
				context.CtorInvokeExpression = null;
				return;
			}
			for (int i = 0; i < statement.Statements.Count; i++)
			{
				if (statement.Statements[i].CodeNodeType == CodeNodeType.ExpressionStatement && ((statement.Statements[i] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ThisCtorExpression || (statement.Statements[i] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.BaseCtorExpression))
				{
					this.RemoveFirstStatements(statement.Statements, i + 1);
					return;
				}
			}
			throw new Exception("Constructor invocation not found.");
		}

		private void RemoveFirstStatements(StatementCollection statements, int count)
		{
			for (int i = 0; i + count < statements.Count; i++)
			{
				statements[i] = statements[i + count];
			}
			while (true)
			{
				int num = count;
				count = num - 1;
				if (num <= 0)
				{
					break;
				}
				statements.RemoveAt(statements.Count - 1);
			}
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