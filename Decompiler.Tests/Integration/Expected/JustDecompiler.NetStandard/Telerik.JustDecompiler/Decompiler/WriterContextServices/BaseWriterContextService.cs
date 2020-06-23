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
				if (collection.Contains(explicitlyImplementedMember.DeclaringType, explicitlyImplementedMember.Member.FullName))
				{
					continue;
				}
				collection.Add(explicitlyImplementedMember.DeclaringType, explicitlyImplementedMember.Member.FullName);
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
				typeSpecificContext = (decompiledType.Value.Type != outerMostDeclaringType ? decompiledType.Value.TypeContext : this.CreateTypeContext(decompiledType.Value.Type, language, decompiledTypes, decompiledType.Value));
				this.cacheService.AddTypeContextToCache(decompiledType.Value.Type, language, this.renameInvalidMembers, typeSpecificContext);
			}
		}

		private TypeSpecificContext CreateTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes, DecompiledType decompiledCurrentType)
		{
			ICollection<TypeReference> typesDependingOn = this.GetTypesDependingOn(decompiledTypes, language);
			ICollection<string> usedNamespaces = this.GetUsedNamespaces(typesDependingOn, type.Namespace);
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
			if (method.Body == null)
			{
				CachedDecompiledMember cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
				this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, cachedDecompiledMember);
				return;
			}
			CachedDecompiledMember cachedDecompiledMember1 = this.DecompileMethod(language, method, decompiledType.TypeContext.ShallowPartialClone());
			List<CachedDecompiledMember> cachedDecompiledMembers = new List<CachedDecompiledMember>();
			TypeDefinition declaringType = method.DeclaringType;
			if (!method.IsStatic)
			{
				foreach (MethodDefinition methodDefinition in declaringType.Methods)
				{
					if (!methodDefinition.IsConstructor || methodDefinition.FullName == cachedDecompiledMember1.Member.MemberFullName || methodDefinition.IsStatic)
					{
						continue;
					}
					if (methodDefinition.Body != null)
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
						MethodDefinition methodDefinition1 = decompiledType.Type.Methods.First<MethodDefinition>((MethodDefinition x) => x.FullName == cachedDecompiledMember3.Member.MemberFullName);
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
			if (method.IsConstructor && !method.IsStatic && method.HasBody)
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
				Statement statement = method.Body.Decompile(language, out decompilationContext, typeContext);
				cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), statement, decompilationContext.MethodContext), decompilationContext.TypeContext);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.ExceptionsWhileDecompiling.Add(method);
				BlockStatement blockStatement = new BlockStatement();
				blockStatement.AddStatement(new ExceptionStatement(exception, method));
				cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, new MethodSpecificContext(method.Body)));
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
			if (method.Body == null)
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
			typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(@event.EventType));
			typeReferences.UnionWith(AttributesUtilities.GetEventAttributesUsedTypes(@event, language));
			if (@event.AddMethod != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(@event.AddMethod, language, false));
			}
			if (@event.RemoveMethod != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(@event.RemoveMethod, language, false));
			}
			if (@event.InvokeMethod != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(@event.InvokeMethod, language, false));
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
			typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(field.FieldType));
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
			if (method.ReturnType != null)
			{
				typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(method.ReturnType));
			}
			if (considerAttributes)
			{
				typeReferences.UnionWith(AttributesUtilities.GetMethodAttributesUsedTypes(method, language));
			}
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(parameter.ParameterType));
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
					if (!strs.TryGetValue(memberDefinition.DeclaringType.FullName, out decompiledType))
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
						if (eventDefinition.AddMethod != null)
						{
							this.DecompileMember(eventDefinition.AddMethod, language, decompiledType);
						}
						if (eventDefinition.RemoveMethod != null)
						{
							this.DecompileMember(eventDefinition.RemoveMethod, language, decompiledType);
						}
						if (eventDefinition.InvokeMethod != null)
						{
							this.DecompileMember(eventDefinition.InvokeMethod, language, decompiledType);
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
					strs.Add(typeDefinition.FullName, new DecompiledType(typeDefinition));
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
			typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(property.PropertyType));
			typeReferences.UnionWith(AttributesUtilities.GetPropertyAttributesUsedTypes(property, language));
			if (property.GetMethod != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(property.GetMethod, language, false));
			}
			if (property.SetMethod != null)
			{
				typeReferences.UnionWith(this.GetMethodTypesDependingOn(property.SetMethod, language, false));
			}
			return typeReferences;
		}

		protected virtual TypeSpecificContext GetTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			DecompiledType decompiledType;
			TypeSpecificContext typeContextFromCache;
			if (!decompiledTypes.TryGetValue(type.FullName, out decompiledType))
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
					if (decompiledMember.Value.Context.Method == null || decompiledMember.Value.Context.Method.Body == null || !decompiledMember.Value.Context.Method.Body.HasVariables)
					{
						continue;
					}
					foreach (VariableDefinition variable in decompiledMember.Value.Context.Method.Body.Variables)
					{
						if (typeReferences.Contains(variable.VariableType))
						{
							continue;
						}
						typeReferences.Add(variable.VariableType);
					}
				}
				if (value.Type.BaseType != null)
				{
					typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(value.Type.BaseType));
				}
				if (value.Type.HasGenericParameters)
				{
					foreach (GenericParameter genericParameter in value.Type.GenericParameters)
					{
						if (!genericParameter.HasConstraints)
						{
							continue;
						}
						foreach (TypeReference constraint in genericParameter.Constraints)
						{
							typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(constraint));
						}
					}
				}
				foreach (TypeReference @interface in value.Type.Interfaces)
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
				if (typeDefinition.BaseType != null)
				{
					TypeDefinition typeDefinition1 = typeDefinition.BaseType.Resolve();
					if (typeDefinition1 != null && (typeDefinition1.IsPublic || typeDefinition1.Module.Assembly == type.Module.Assembly))
					{
						typeDefinitions.Enqueue(typeDefinition1);
					}
				}
				if (typeDefinition.HasInterfaces)
				{
					foreach (TypeReference @interface in typeDefinition.Interfaces)
					{
						TypeDefinition typeDefinition2 = @interface.Resolve();
						if (typeDefinition2 == null || !typeDefinition2.IsPublic && typeDefinition2.Module.Assembly != type.Module.Assembly)
						{
							continue;
						}
						typeDefinitions.Enqueue(typeDefinition2);
					}
				}
				List<FieldDefinition> fieldDefinitions = null;
				if (decompiledTypes.TryGetValue(typeDefinition.FullName, out decompiledType))
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
						if (propertyDefinition.GetMethod != null && (!propertyDefinition.GetMethod.IsPrivate || propertyDefinition.DeclaringType == type) && !strs.Contains(propertyDefinition.Name))
						{
							strs.Add(propertyDefinition.Name);
						}
						if (propertyDefinition.SetMethod != null && (!propertyDefinition.SetMethod.IsPrivate || propertyDefinition.DeclaringType == type) && !strs.Contains(propertyDefinition.Name))
						{
							strs.Add(propertyDefinition.Name);
						}
					}
					if (typeMember is FieldDefinition)
					{
						FieldDefinition fieldDefinition = typeMember as FieldDefinition;
						if ((!fieldDefinition.IsPrivate || fieldDefinition.DeclaringType == type) && !strs.Contains(fieldDefinition.Name))
						{
							strs.Add(fieldDefinition.Name);
						}
					}
					if (typeMember is EventDefinition && !strs.Contains(typeMember.Name))
					{
						strs.Add(typeMember.Name);
					}
					if (!(typeMember is TypeDefinition) || strs.Contains(typeMember.Name))
					{
						continue;
					}
					strs.Add(typeMember.Name);
				}
			}
			return strs;
		}

		protected ICollection<string> GetUsedNamespaces(ICollection<TypeReference> typesDependingOn, string currentNamespace = "")
		{
			ICollection<string> strs = new HashSet<string>();
			foreach (TypeReference declaringType in typesDependingOn)
			{
				while (declaringType.IsNested)
				{
					declaringType = declaringType.DeclaringType;
				}
				string @namespace = declaringType.Namespace;
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
			if (context == null || !context.Method.IsConstructor || context.Method.IsStatic || context.CtorInvokeExpression == null)
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