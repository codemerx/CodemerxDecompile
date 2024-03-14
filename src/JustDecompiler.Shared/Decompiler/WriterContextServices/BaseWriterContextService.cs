using System;
using System.Linq;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Ast.Statements;
using System.Collections.Generic;
using Mono.Collections.Generic;
using System.Collections;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Languages.IL;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.External;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public abstract class BaseWriterContextService : ExceptionThrownNotifier, IWriterContextService
	{
		protected readonly IDecompilationCacheService cacheService;
		protected readonly bool renameInvalidMembers;

		public BaseWriterContextService(IDecompilationCacheService cacheService, bool renameInvalidMembers)
		{
			this.cacheService = cacheService;
			this.renameInvalidMembers = renameInvalidMembers;
			this.ExceptionsWhileDecompiling = new List<MethodDefinition>();
		}

		public ICollection<MethodDefinition> ExceptionsWhileDecompiling { get; private set; }

		protected virtual MemberRenamingData GetMemberRenamingData(ModuleDefinition module, ILanguage language)
		{
			DefaultMemberRenamingService renamingService = new DefaultMemberRenamingService(language, this.renameInvalidMembers);
			return renamingService.GetMemberRenamingData(module);
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

			CachedDecompiledMember decompiledMember = DecompileMethod(language, method, decompiledType.TypeContext);

			this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, decompiledMember);
			return decompiledMember;
		}
  
		private void RemoveBaseCtorInvocationStatements(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			MethodSpecificContext methodContext = decompiledMember.Member.Context;
			//value types chained constructors?
			if (methodContext== null || !methodContext.Method.IsConstructor || methodContext.Method.IsStatic || methodContext.CtorInvokeExpression == null)
			{
				return;
			}
			BlockStatement methodBody = decompiledMember.Member.Statement as BlockStatement;
			if (methodBody == null) // it might have been an exception
			{
				return;
			}
            else if (methodBody.Statements.Count == 1 && methodBody.Statements[0] is UnsafeBlockStatement)
            {
                methodBody = methodBody.Statements[0] as UnsafeBlockStatement;
            }

			TypeSpecificContext typeContext = decompiledType.TypeContext;
			if (typeContext.FieldInitializationFailed && typeContext.BaseCtorInvocators.Contains(methodContext.Method))
			{
				methodContext.CtorInvokeExpression = null;
			}
			else
			{
				/// all statements before the constructor invocation should be removed
                for (int i = 0; i < methodBody.Statements.Count; i++)
                {
                    if (methodBody.Statements[i].CodeNodeType == Ast.CodeNodeType.ExpressionStatement &&
                        ((methodBody.Statements[i] as ExpressionStatement).Expression.CodeNodeType == Ast.CodeNodeType.ThisCtorExpression ||
                        (methodBody.Statements[i] as ExpressionStatement).Expression.CodeNodeType == Ast.CodeNodeType.BaseCtorExpression))
                    {
                        RemoveFirstStatements(methodBody.Statements, i + 1);
                        return;
                    }
                }
                throw new Exception("Constructor invocation not found.");
			}
		}

        private void RemoveFirstStatements(StatementCollection statements, int count)
        {
            for (int i = 0; i + count < statements.Count; i++)
            {
                statements[i] = statements[i + count];
            }

            while (count-- > 0)
            {
                statements.RemoveAt(statements.Count - 1);
            }
        }

		private CachedDecompiledMember DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext)
		{
			CachedDecompiledMember decompiledMember;
			Statement statement;
			try
			{
				DecompilationContext innerContext = null;
				statement = method.Body.Decompile(language, out innerContext, typeContext);
				decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), statement, innerContext.MethodContext), innerContext.TypeContext);
			}
			catch (Exception ex)
			{
				this.ExceptionsWhileDecompiling.Add(method);

				BlockStatement blockStatement = new BlockStatement();
				statement = new ExceptionStatement(ex, method);
				blockStatement.AddStatement(statement);

				decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, new MethodSpecificContext(method.Body)));

                OnExceptionThrown(ex);
            }
			return decompiledMember;
		}

		protected void AddDecompiledMemberToDecompiledType(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
		{
			if (!decompiledType.DecompiledMembers.ContainsKey(decompiledMember.Member.MemberFullName))
			{
				decompiledType.DecompiledMembers.Add(decompiledMember.Member.MemberFullName, decompiledMember.Member);
			}
		}

        protected void AddGeneratedFilterMethodsToDecompiledType(DecompiledType decompiledType, TypeSpecificContext context, ILanguage language)
        {
            foreach (GeneratedMethod generatedMethod in context.GeneratedFilterMethods)
            {
                CachedDecompiledMember member = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(generatedMethod.Method), generatedMethod.Body, generatedMethod.Context));
                AddDecompiledMemberToDecompiledType(member, decompiledType);
            }
        }

		protected void DecompileMember(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (method.IsConstructor && !method.IsStatic && method.HasBody)
			{
				DecompileConstructorChain(method, language, decompiledType);
				return;
			}
			CachedDecompiledMember decompiledMember = GetDecompiledMember(method, language, decompiledType);
			AddDecompiledMemberToDecompiledType(decompiledMember, decompiledType);
			//UpdateTypeContext(decompiledType.TypeContext, decompiledMember);
		}
  
		protected void DecompileConstructorChain(MethodDefinition method, ILanguage language, DecompiledType decompiledType)
		{
			if (this.cacheService.IsDecompiledMemberInCache(method, language, this.renameInvalidMembers))
			{
				///all constructors have already been decompiled.

				CachedDecompiledMember decompiledMember = this.cacheService.GetDecompiledMemberFromCache(method, language, this.renameInvalidMembers);
				AddDecompiledMemberToDecompiledType(decompiledMember, decompiledType);
                AddAssignmentDataToDecompiledType(decompiledMember, decompiledType);

				return;
			}
			if (method.Body == null)
			{
				CachedDecompiledMember methodCache = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
				this.cacheService.AddDecompiledMemberToCache(method, language, this.renameInvalidMembers, methodCache);
				return;
			}
            CachedDecompiledMember originalConstructor = DecompileMethod(language, method, decompiledType.TypeContext.ShallowPartialClone());
			List<CachedDecompiledMember> allConstructors = new List<CachedDecompiledMember>();
			TypeDefinition declaringType = method.DeclaringType;
			if (!method.IsStatic)
			{
				foreach (MethodDefinition constructor in declaringType.Methods)
				{
					if (!constructor.IsConstructor || constructor.FullName == originalConstructor.Member.MemberFullName || constructor.IsStatic)
					{
						continue;
					}
					if (constructor.Body == null)
					{
						CachedDecompiledMember methodCache = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(constructor), null, null));
						allConstructors.Add(methodCache);
					}
					else
					{
						allConstructors.Add(DecompileMethod(language, constructor, decompiledType.TypeContext.ShallowPartialClone()));
					}
				}
				allConstructors.Add(originalConstructor);

				MergeConstructorsTypeContexts(allConstructors, decompiledType);

				foreach (CachedDecompiledMember constructor in allConstructors)
				{
					if (!(language is IntermediateLanguage))
					{
						// there are no such statements in IL
						RemoveBaseCtorInvocationStatements(constructor, decompiledType);
					}
					if (constructor.Member.Context == null)
					{
						var methodDefinition = decompiledType.Type.Methods.First(x => x.FullName == constructor.Member.MemberFullName);

						if (!this.cacheService.IsDecompiledMemberInCache(methodDefinition, language, this.renameInvalidMembers))
						{
							this.cacheService.AddDecompiledMemberToCache(methodDefinition, language, this.renameInvalidMembers, constructor);
						}
					}
					else
					{
						if (!this.cacheService.IsDecompiledMemberInCache(constructor.Member.Context.Method, language, this.renameInvalidMembers))
						{
							this.cacheService.AddDecompiledMemberToCache(constructor.Member.Context.Method, language, this.renameInvalidMembers, constructor);
						}
					}
					AddDecompiledMemberToDecompiledType(constructor, decompiledType);
                    //UpdateTypeContext(decompiledType.TypeContext, constructor);
                }
			}
		}

        private static void AddAssignmentDataToDecompiledType(CachedDecompiledMember decompiledMember, DecompiledType decompiledType)
        {
            if (!decompiledType.TypeContext.FieldInitializationFailed)
            {
                foreach (KeyValuePair<string, InitializationAssignment> pair in decompiledMember.FieldAssignmentData)
                {
                    if (!decompiledType.TypeContext.AssignmentData.ContainsKey(pair.Key))
                    {
                        decompiledType.TypeContext.AssignmentData.Add(pair.Key, pair.Value);
                    }
                    else if (!decompiledType.TypeContext.AssignmentData[pair.Key].AssignmentExpression.Equals(pair.Value.AssignmentExpression))
                    {
                        decompiledType.TypeContext.FieldInitializationFailed = true;
                        decompiledType.TypeContext.AssignmentData = new Dictionary<string, InitializationAssignment>();
                        return;
                    }
                }
            }
        }
  
		private void MergeConstructorsTypeContexts(List<CachedDecompiledMember> allConstructors, DecompiledType decompiledType)
		{
			if (allConstructors == null || allConstructors.Count == 0)
			{
				return;
			}

			for (int j = 0; j < allConstructors.Count; j++)
			{
				// context can be null, if the constructor has no body
				if (allConstructors[j].Member.Context != null && allConstructors[j].Member.Context.IsBaseConstructorInvokingConstructor)
				{
					decompiledType.TypeContext.BaseCtorInvocators.Add(allConstructors[j].Member.Context.Method);
				}
			}

			int i = 0;
			Dictionary<string, InitializationAssignment> combinedDictionary = new Dictionary<string, InitializationAssignment>();
			for (; i < allConstructors.Count; i++)
			{
				if (allConstructors[i].Member.Context != null &&  allConstructors[i].Member.Context.IsBaseConstructorInvokingConstructor)
				{
					/// at least one constructor will call the base's ctor
					combinedDictionary = new Dictionary<string, InitializationAssignment>(allConstructors[i].FieldAssignmentData);
					break;
				}
			}

			for (; i < allConstructors.Count; i++)
			{
				CachedDecompiledMember constructor = allConstructors[i];
				if (constructor.Member.Context == null || !constructor.Member.Context.IsBaseConstructorInvokingConstructor)
				{
					continue;
				}
				if (constructor.FieldAssignmentData.Count != combinedDictionary.Count)
				{
					// merge should fail
					decompiledType.TypeContext.FieldInitializationFailed = true;
					decompiledType.TypeContext.AssignmentData = new Dictionary<string, InitializationAssignment>();
					return;				
				}
				foreach (KeyValuePair<string, InitializationAssignment> pair in constructor.FieldAssignmentData)
				{
					if (combinedDictionary.ContainsKey(pair.Key) && combinedDictionary[pair.Key].AssignmentExpression.Equals(pair.Value.AssignmentExpression))
					{
						continue;
					}
					// merge should fail otherwise
					decompiledType.TypeContext.FieldInitializationFailed = true;
					decompiledType.TypeContext.AssignmentData = new Dictionary<string, InitializationAssignment>();
					return;
				}
			}
			decompiledType.TypeContext.AssignmentData = combinedDictionary;
		}

		protected Dictionary<string, DecompiledType> GetNestedDecompiledTypes(TypeDefinition type, ILanguage language)
		{
			Dictionary<string, DecompiledType> decompiledTypes = new Dictionary<string, DecompiledType>();

			Queue<IMemberDefinition> decompilationQueue = new Queue<IMemberDefinition>();

			decompilationQueue.Enqueue(type);
			while (decompilationQueue.Count > 0)
			{
				IMemberDefinition currentMember = decompilationQueue.Dequeue();

				if (currentMember is TypeDefinition)
				{
					TypeDefinition currentType = (currentMember as TypeDefinition);
					decompiledTypes.Add(currentType.FullName, new DecompiledType(currentType));

					//List<IMemberDefinition> members = Utilities.GetTypeMembers(currentType);
					List<IMemberDefinition> members = Utilities.GetTypeMembersToDecompile(currentType);
					foreach (IMemberDefinition member in members)
					{
						decompilationQueue.Enqueue(member);
					}
				}
				else
				{
					TypeDefinition currentType = currentMember.DeclaringType;

					DecompiledType decompiledType;
					if (!decompiledTypes.TryGetValue(currentType.FullName, out decompiledType))
					{
						throw new Exception("Type missing from nested types decompilation cache.");
					}

					if (currentMember is MethodDefinition)
					{
						MethodDefinition method = currentMember as MethodDefinition;
						DecompileMember(method, language, decompiledType);
						//Utilities.AddDecompiledMethodToDecompiledType(method, language, decompiledType);
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
							//Utilities.AddDecompiledMethodToDecompiledType(eventDefinition.AddMethod, language, decompiledType);
							DecompileMember(eventDefinition.AddMethod, language, decompiledType);
						}

						if (eventDefinition.RemoveMethod != null)
						{
							//Utilities.AddDecompiledMethodToDecompiledType(eventDefinition.RemoveMethod, language, decompiledType);
							DecompileMember(eventDefinition.RemoveMethod, language, decompiledType);
						}

						if (eventDefinition.InvokeMethod != null)
						{
							//Utilities.AddDecompiledMethodToDecompiledType(eventDefinition.InvokeMethod, language, decompiledType);
							DecompileMember(eventDefinition.InvokeMethod, language, decompiledType);
						}
					}
					if (currentMember is PropertyDefinition)
					{
						PropertyDefinition propertyDefinition = (currentMember as PropertyDefinition);

                        CachedDecompiledMember getMethod;
                        CachedDecompiledMember setMethod;
                        bool isAutoImplemented;

                        PropertyDecompiler propertyDecompiler = new PropertyDecompiler(propertyDefinition, language, renameInvalidMembers, this.cacheService, decompiledType.TypeContext);
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
				}
			}

            foreach (DecompiledType decompiledType in decompiledTypes.Values)
            {
                AddGeneratedFilterMethodsToDecompiledType(decompiledType, decompiledType.TypeContext, language);
            }

			return decompiledTypes;
		}

        protected CachedDecompiledMember AddDecompiledMemberToCache(IMemberDefinition member, DecompiledMember decompiledMember, TypeSpecificContext typeContext, ILanguage language)
        {
            CachedDecompiledMember cachedMember = new CachedDecompiledMember(decompiledMember, typeContext);
            if (!this.cacheService.IsDecompiledMemberInCache(member, language, this.renameInvalidMembers))
            {
                this.cacheService.AddDecompiledMemberToCache(member, language, this.renameInvalidMembers, cachedMember);
            }

            return cachedMember;
        }

		private void AddExplicitlyImplementedMembers(ICollection<ImplementedMember> explicitlyImplementedMembers, ExplicitlyImplementedMembersCollection collection)
		{
			foreach (ImplementedMember explicitlyImplementedMember in explicitlyImplementedMembers)
			{
				if (!collection.Contains(explicitlyImplementedMember.DeclaringType, explicitlyImplementedMember.Member.FullName))
				{
					collection.Add(explicitlyImplementedMember.DeclaringType, explicitlyImplementedMember.Member.FullName);
				}
			}
		}

		protected ExplicitlyImplementedMembersCollection GetExplicitlyImplementedInterfaceMethods(TypeDefinition type, ILanguage language)
		{
			ExplicitlyImplementedMembersCollection result = new ExplicitlyImplementedMembersCollection();

			IEnumerable<IMemberDefinition> members = type.GetMembersUnordered(true);
			foreach (IMemberDefinition member in members)
			{
				if (member is MethodDefinition)
				{
					MethodDefinition method = member as MethodDefinition;
					ICollection<ImplementedMember> explicitlyImplementedMethods = method.GetExplicitlyImplementedMethods();
					AddExplicitlyImplementedMembers(explicitlyImplementedMethods, result);
				}

				if (member is PropertyDefinition)
				{
					PropertyDefinition property = member as PropertyDefinition;
					ICollection<ImplementedMember> explicitlyImplementedProperties = property.GetExplicitlyImplementedProperties();
					AddExplicitlyImplementedMembers(explicitlyImplementedProperties, result);
				}

				if (member is EventDefinition)
				{
					EventDefinition @event = member as EventDefinition;
					ICollection<ImplementedMember> explicitlyImplementedEvents = @event.GetExplicitlyImplementedEvents();
					AddExplicitlyImplementedMembers(explicitlyImplementedEvents, result);
				}
			}

			return result;
		}

		protected virtual TypeSpecificContext GetTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			DecompiledType decompiledCurrentType;
			if (!decompiledTypes.TryGetValue(type.FullName, out decompiledCurrentType))
			{
				throw new Exception("Decompiled type not found in decompiled types cache.");
			}

			TypeSpecificContext typeContext;
			if (this.cacheService.IsTypeContextInCache(type, language, this.renameInvalidMembers))
			{
                if (decompiledCurrentType.TypeContext.GeneratedFilterMethods.Count > 0)
                {
                    this.cacheService.ReplaceCachedTypeContext(type, language, this.renameInvalidMembers, decompiledCurrentType.TypeContext);
                }

				typeContext = this.cacheService.GetTypeContextFromCache(type, language, this.renameInvalidMembers);
			}
			else
			{
                typeContext = CreateTypeContext(type, language, decompiledTypes, decompiledCurrentType);

				this.cacheService.AddTypeContextToCache(type, language, this.renameInvalidMembers, typeContext);
			}

			return typeContext;
		}

        private TypeSpecificContext CreateTypeContext(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes, DecompiledType decompiledCurrentType)
        {
            ICollection<TypeReference> typesDependingOn = GetTypesDependingOn(decompiledTypes, language);
            ICollection<string> usedNamespaces = GetUsedNamespaces(typesDependingOn, type.Namespace);
            ICollection<string> visibleMembersNames = GetTypeVisibleMembersNames(type, language, decompiledTypes);
            ExplicitlyImplementedMembersCollection explicitlyImplementedInterfaceMethods = GetExplicitlyImplementedInterfaceMethods(type, language);

            return new TypeSpecificContext(
                type,
                decompiledCurrentType.TypeContext.MethodDefinitionToNameMap,
                decompiledCurrentType.TypeContext.BackingFieldToNameMap,
                usedNamespaces,
                visibleMembersNames,
                decompiledCurrentType.TypeContext.AssignmentData,
                GetAutoImplementedProperties(decompiledTypes),
                GetAutoImplementedEvents(decompiledTypes),
                explicitlyImplementedInterfaceMethods,
                this.ExceptionsWhileDecompiling,
                decompiledCurrentType.TypeContext.GeneratedFilterMethods,
                decompiledCurrentType.TypeContext.GeneratedMethodDefinitionToNameMap
            );
        }

        protected void AddTypeContextsToCache(Dictionary<string, DecompiledType> decompiledTypes, TypeDefinition outerMostDeclaringType, ILanguage language)
        {
            foreach (KeyValuePair<string, DecompiledType> pair in decompiledTypes)
            {
                if (!this.cacheService.IsTypeContextInCache(pair.Value.Type, language, this.renameInvalidMembers))
                {
                    TypeSpecificContext context;
                    if (pair.Value.Type == outerMostDeclaringType)
                    {
                        context = CreateTypeContext(pair.Value.Type, language, decompiledTypes, pair.Value);
                    }
                    else
                    {
                        context = pair.Value.TypeContext;
                    }

                    this.cacheService.AddTypeContextToCache(pair.Value.Type, language, this.renameInvalidMembers, context);
                }
            }
        }

		private ICollection<string> GetTypeVisibleMembersNames(TypeDefinition type, ILanguage language, Dictionary<string, DecompiledType> decompiledTypes)
		{
			HashSet<string> membersNames = new HashSet<string>(language.IdentifierComparer);

			Queue<TypeDefinition> typesQueue = new Queue<TypeDefinition>();
			typesQueue.Enqueue(type);
			while (typesQueue.Count > 0)
			{
				TypeDefinition currentType = typesQueue.Dequeue();
				if (currentType.BaseType != null)
				{
					TypeDefinition baseType = currentType.BaseType.Resolve();
					if (baseType != null && (baseType.IsPublic || baseType.Module.Assembly == type.Module.Assembly))
					{
						typesQueue.Enqueue(baseType);
					}
				}

				if (currentType.HasInterfaces)
				{
					foreach (TypeReference typeRef in currentType.Interfaces)
					{
						TypeDefinition resolvedInterface = typeRef.Resolve();
						if (resolvedInterface != null && (resolvedInterface.IsPublic || resolvedInterface.Module.Assembly == type.Module.Assembly))
						{
							typesQueue.Enqueue(resolvedInterface);
						}
					}
				}

                DecompiledType currentDecompiledType;
                TypeSpecificContext currentTypeContext;
                List<FieldDefinition> propertyFields = null;
                if (decompiledTypes.TryGetValue(currentType.FullName, out currentDecompiledType))
                {
                    propertyFields = new List<FieldDefinition>(currentDecompiledType.TypeContext.GetFieldToPropertyMap(language).Keys);
                }
                else if (TryGetTypeContextFromCache(currentType, language, out currentTypeContext))
                {
                    propertyFields = new List<FieldDefinition>(currentTypeContext.GetFieldToPropertyMap(language).Keys);
                }

                List<IMemberDefinition> members = Utilities.GetTypeMembers(currentType, language, true, propertyFields: propertyFields);
				foreach (IMemberDefinition member in members)
				{
					if (member is PropertyDefinition)
					{
						PropertyDefinition property = member as PropertyDefinition;

						if (property.GetMethod != null)
						{
							if (!property.GetMethod.IsPrivate || property.DeclaringType == type)
							{
								if (!membersNames.Contains(property.Name))
								{
									membersNames.Add(property.Name);
								}
							}
						}

						if (property.SetMethod != null)
						{
							if (!property.SetMethod.IsPrivate || property.DeclaringType == type)
							{
								if (!membersNames.Contains(property.Name))
								{
									membersNames.Add(property.Name);
								}
							}
						}
					}

					if (member is FieldDefinition)
					{
						FieldDefinition field = member as FieldDefinition;
						if (!field.IsPrivate || field.DeclaringType == type)
						{
							if (!membersNames.Contains(field.Name))
							{
								membersNames.Add(field.Name);
							}
						}
					}

					if (member is EventDefinition)
					{
						if (!membersNames.Contains(member.Name))
						{
							membersNames.Add(member.Name);
						}
					}

					if (member is TypeDefinition)
					{
						if (!membersNames.Contains(member.Name))
						{
							membersNames.Add(member.Name);
						}
					}
				}
			}

			return membersNames;
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

		protected ICollection<string> GetUsedNamespaces(ICollection<TypeReference> typesDependingOn, string currentNamespace = "")
		{
			ICollection<string> neededNamesapceUsings = new HashSet<string>();

			foreach (TypeReference typeReference in typesDependingOn)
			{
				TypeReference outerMostTypeReference = typeReference;
				while (outerMostTypeReference.IsNested)
				{
					outerMostTypeReference = outerMostTypeReference.DeclaringType;
				}

				string namespaceName = outerMostTypeReference.Namespace;
				if ((namespaceName != String.Empty) && (namespaceName != currentNamespace) && (!neededNamesapceUsings.Contains(namespaceName)))
				{
					neededNamesapceUsings.Add(namespaceName);
				}
			}

			return neededNamesapceUsings;
		}

		private ICollection<TypeReference> GetTypesDependingOn(Dictionary<string, DecompiledType> decompiledTypes, ILanguage language)
		{
			HashSet<TypeReference> typesDependingOn = new HashSet<TypeReference>();

			foreach (DecompiledType decompiledType in decompiledTypes.Values)
			{
				foreach (KeyValuePair<string, DecompiledMember> pair in decompiledType.DecompiledMembers)
				{
					if (pair.Value.Context != null)
					{
						typesDependingOn.UnionWith(pair.Value.Context.AnalysisResults.TypesDependingOn);

						if ((pair.Value.Context.Method != null) && (pair.Value.Context.Method.Body != null) && (pair.Value.Context.Method.Body.HasVariables))
						{
							foreach (VariableDefinition localVariable in pair.Value.Context.Method.Body.Variables)
							{
								if (!typesDependingOn.Contains(localVariable.VariableType))
								{
									typesDependingOn.Add(localVariable.VariableType);
								}
							}
						}
					}
				}

				if (decompiledType.Type.BaseType != null)
				{
					typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(decompiledType.Type.BaseType));
				}

				if (decompiledType.Type.HasGenericParameters)
				{
					foreach (GenericParameter genericParameter in decompiledType.Type.GenericParameters)
					{
						if (genericParameter.HasConstraints)
						{
							foreach (TypeReference constraint in genericParameter.Constraints)
							{
								typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(constraint));
							}
						}
					}
				}

				foreach (TypeReference reference in decompiledType.Type.Interfaces)
				{
					typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(reference));
				}

				typesDependingOn.UnionWith(AttributesUtilities.GetTypeAttributesUsedTypes(decompiledType.Type));

				List<IMemberDefinition> members = Utilities.GetTypeMembers(decompiledType.Type, language, propertyFields: decompiledType.TypeContext.GetFieldToPropertyMap(language).Keys);
				foreach (IMemberDefinition member in members)
				{
					if (member is MethodDefinition)
					{
						MethodDefinition method = member as MethodDefinition;

						typesDependingOn.UnionWith(GetMethodTypesDependingOn(method, language));

                        //if (method.IsConstructor)
                        //{
                        //    DecompiledMember decompiledMember;
                        //    if (decompiledType.DecompiledMembers.TryGetValue(Utilities.GetMemberUniqueName(method), out decompiledMember))
                        //    {
                        //        if (decompiledMember.Context != null && decompiledMember.Context.CtorInvokeExpression != null)
                        //        {
                        //            DependsOnAnalysisVisitor visitor = new DependsOnAnalysisVisitor(typesDependingOn);
                        //            visitor.Visit(decompiledMember.Context.CtorInvokeExpression);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        throw new Exception("Decompiled member not found in decompiled members cache.");
                        //    }
                        //}
					}

					if (member is PropertyDefinition)
					{
						typesDependingOn.UnionWith(GetPropertyTypesDependingOn(member as PropertyDefinition, language));
					}

					if (member is EventDefinition)
					{
						typesDependingOn.UnionWith(GetEventTypesDependingOn(member as EventDefinition, language));
					}

					if (member is FieldDefinition)
					{
						typesDependingOn.UnionWith(GetFieldTypesDependingOn(member as FieldDefinition));
					}
				}
			}

			return typesDependingOn;
		}

		private ICollection<TypeReference> GetMethodTypesDependingOn(MethodDefinition method, ILanguage language, bool considerAttributes = true)
		{
			HashSet<TypeReference> typesDependingOn = new HashSet<TypeReference>();

			if (method.ReturnType != null)
			{
				typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(method.ReturnType));
			}

			if (considerAttributes)
			{
				typesDependingOn.UnionWith(AttributesUtilities.GetMethodAttributesUsedTypes(method, language));
			}

			foreach (ParameterDefinition parameter in method.Parameters)
			{
				typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(parameter.ParameterType));
			}

			return typesDependingOn;
		}

		private ICollection<TypeReference> GetPropertyTypesDependingOn(PropertyDefinition property, ILanguage language)
		{
			HashSet<TypeReference> typesDependingOn = new HashSet<TypeReference>();
			typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(property.PropertyType));

			typesDependingOn.UnionWith(AttributesUtilities.GetPropertyAttributesUsedTypes(property, language));

			if (property.GetMethod != null)
			{
				typesDependingOn.UnionWith(GetMethodTypesDependingOn(property.GetMethod, language, false));
			}

			if (property.SetMethod != null)
			{
				typesDependingOn.UnionWith(GetMethodTypesDependingOn(property.SetMethod, language, false));
			}

			return typesDependingOn;
		}

		private ICollection<TypeReference> GetEventTypesDependingOn(EventDefinition @event, ILanguage language)
		{
			HashSet<TypeReference> typesDependingOn = new HashSet<TypeReference>();
			typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(@event.EventType));

			typesDependingOn.UnionWith(AttributesUtilities.GetEventAttributesUsedTypes(@event, language));

			if (@event.AddMethod != null)
			{
				typesDependingOn.UnionWith(GetMethodTypesDependingOn(@event.AddMethod, language, false));
			}

			if (@event.RemoveMethod != null)
			{
				typesDependingOn.UnionWith(GetMethodTypesDependingOn(@event.RemoveMethod, language, false));
			}

			if (@event.InvokeMethod != null)
			{
				typesDependingOn.UnionWith(GetMethodTypesDependingOn(@event.InvokeMethod, language, false));
			}

			return typesDependingOn;
		}

		private ICollection<TypeReference> GetFieldTypesDependingOn(FieldDefinition field)
		{
			HashSet<TypeReference> typesDependingOn = new HashSet<TypeReference>();
			
			typesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(field.FieldType));
			
			typesDependingOn.UnionWith(AttributesUtilities.GetFieldAttributesUsedTypes(field));

			return typesDependingOn;
		}

		protected ICollection<string> GetAssemblyNamespaceUsings(AssemblyDefinition assembly)
		{
			ICollection<TypeReference> typesDependingOn = AttributesUtilities.GetAssemblyAttributesUsedTypes(assembly);
			return GetUsedNamespaces(typesDependingOn);
		}

		protected ICollection<string> GetModuleNamespaceUsings(ModuleDefinition module)
		{
			ICollection<TypeReference> typesDependingOn = AttributesUtilities.GetModuleAttributesUsedTypes(module);
			return GetUsedNamespaces(typesDependingOn);
		}

		public abstract WriterContext GetWriterContext(IMemberDefinition member, ILanguage language);

		public abstract AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language);

		public abstract ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language);

		protected HashSet<PropertyDefinition> GetAutoImplementedProperties(Dictionary<string, DecompiledType> decompiledTypes)
		{
			HashSet<PropertyDefinition> result = new HashSet<PropertyDefinition>();

			foreach (DecompiledType decompiledType in decompiledTypes.Values)
			{
				result.UnionWith(decompiledType.TypeContext.AutoImplementedProperties);
			}

			return result;
		}

		protected HashSet<EventDefinition> GetAutoImplementedEvents(Dictionary<string, DecompiledType> decompiledTypes)
		{
			HashSet<EventDefinition> result = new HashSet<EventDefinition>();

			foreach (DecompiledType decompiledType in decompiledTypes.Values)
			{
				result.UnionWith(decompiledType.TypeContext.AutoImplementedEvents);
			}

			return result;
		}
	}
}
