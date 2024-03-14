using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Ast.Statements;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	public class Utilities
	{
		/// <summary>
		/// Gets a reference to the <paramref name="type"/> from the core library for <paramref name="currentModule"/>.
		/// Core library is mscorlib.dll for .NET up to 4.5 or System.Runtime for WinRT apps.
		/// </summary>
		/// <param name="type">The type to be searched in the core library.</param>
		/// <param name="currentModule">The module which uses the type.</param>
		/// <returns></returns>
		public static TypeReference GetCorlibTypeReference(Type type, ModuleDefinition currentModule)
		{
            AssemblyNameReference scope = currentModule.ReferencedMscorlibRef();
			if (scope == null)
			{
				scope = currentModule.GetReferencedCoreLibraryRef("System.Runtime");

                if (scope == null)
                {
                    scope = currentModule.GetReferencedCoreLibraryRef("System.Private.CoreLib");

                    if (scope == null)
                    {
                        scope = currentModule.GetReferencedCoreLibraryRef("netstandard");
                    }
                }
            }
			return new TypeReference(type.Namespace, type.Name, currentModule, scope);
		}

		public static MethodReference GetEmptyConstructor(Type type, ModuleDefinition currentModule, IList<Type> mscorlibArgumentTypes = null)
		{
            if (mscorlibArgumentTypes == null)
            {
                mscorlibArgumentTypes = new List<Type>();
            }

            TypeReference typeRef = Utilities.GetCorlibTypeReference(type, currentModule);

			TypeDefinition attributeTypeDef = typeRef.Resolve();
            if (attributeTypeDef != null)
            {
                foreach (MethodDefinition constr in attributeTypeDef.Methods)
                {
                    if (constr.IsConstructor && ArgumentsMatch(constr.Parameters, mscorlibArgumentTypes))
                    {
                        return constr;
                    }
                }
                throw new ArgumentOutOfRangeException(string.Format("Type {0} doesnt provide matching constructor.", type.FullName));
            }
            else
            {
                MethodReference ctorRef = new MethodReference(".ctor", Utilities.GetCorlibTypeReference(typeof(void), currentModule), typeRef);
                ctorRef.Parameters.AddRange(GetMatchingArguments(mscorlibArgumentTypes, currentModule));
                return ctorRef;
            }
		}

		private static bool ArgumentsMatch(Collection<ParameterDefinition> parameters, IList<Type> arguments)
		{
			if (parameters == null && arguments.Count != 0)
			{
				return false;
			}
			if (parameters.Count != arguments.Count)
			{
				return false;
			}
			for (int i = 0; i < parameters.Count; i++)
			{
				if (parameters[i].ParameterType.FullName != arguments[i].FullName)
				{ 
					return false;
				}
			}
			return true;
		}

        private static IEnumerable<ParameterDefinition> GetMatchingArguments(IList<Type> mscorlibArgumentTypes, ModuleDefinition currentModule)
        {
            List<ParameterDefinition> result = new List<ParameterDefinition>(mscorlibArgumentTypes.Count);
            foreach (Type type in mscorlibArgumentTypes)
            {
                result.Add(new ParameterDefinition(Utilities.GetCorlibTypeReference(type, currentModule)));
            }
            return result;
        }

		public static MemberReference FindMemberArgumentRefersTo(IList fieldsAndProperties, CustomAttributeNamedArgument argument)
		{
			MemberReference result = null;
			foreach (object member in fieldsAndProperties)
			{
				if ((member as MemberReference).Name == argument.Name)
				{
					result = member as MemberReference;
				}
			}
			return result;
		}

		public static List<IMemberDefinition> GetTypeMembersToDecompile(TypeDefinition type)
		{
			List<IMemberDefinition> members = type.GetMembersToDecompile().ToList();
			return members;
		}

		public static List<IMemberDefinition> GetTypeMembers(TypeDefinition type, ILanguage language, bool showCompilerGeneratedMembers = true,
			IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null, IEnumerable<MethodDefinition> generatedFilterMethods = null,
            IEnumerable<FieldReference> propertyFields = null)
		{

#if !NET35
            List<IMemberDefinition> members = type.GetMembersSorted(showCompilerGeneratedMembers, language, attributesToSkip, fieldsToSkip, new HashSet<FieldReference>(type.GetFieldToEventMap(language).Keys), generatedFilterMethods, propertyFields).ToList();
#else
			IEnumerable<FieldDefinition> fields = type.GetFieldToEventMap().Keys;
			HashSet<FieldReference> temp = new HashSet<FieldReference>();

			foreach(FieldDefinition fdef in fields)
			{
				temp.Add(fdef);
			}

			List<IMemberDefinition> members = type.GetMembersSorted(showCompilerGeneratedMembers, language, attributesToSkip, fieldsToSkip, temp, generatedFilterMethods, propertyFields).ToList();
#endif

            return members;
		}

		public static bool IsTypeNameInCollisionOnAssemblyLevel(string typeName, AssemblySpecificContext assemblyContext, ModuleSpecificContext mainModuleContext)
		{
			HashSet<string> usedNamespaces = new HashSet<string>();

			foreach (string usedNamespace in assemblyContext.AssemblyNamespaceUsings)
			{
				usedNamespaces.Add(usedNamespace);
			}

			usedNamespaces.UnionWith(mainModuleContext.ModuleNamespaceUsings);

			List<string> typeCollisionNamespaces;
			if (mainModuleContext.CollisionTypesData.TryGetValue(typeName, out typeCollisionNamespaces))
			{
				IEnumerable<string> namespacesIntersection = typeCollisionNamespaces.Intersect(usedNamespaces);

				if (namespacesIntersection.Count() > 1)
				{
					return true;
				}
			}

			return false;
		}

		public static bool HasNamespaceParentNamespace(string @namespace)
		{
			return @namespace.Contains(".");
		}

		public static string GetNamesapceParentNamesapce(string @namespace)
		{
			if (!HasNamespaceParentNamespace(@namespace))
			{
				throw new Exception("Namespace does not have a parent namesapce.");
			}

			string[] namespaceParts = @namespace.Split('.');
			StringBuilder parentNamespace = new StringBuilder();
			for (int i = 0; i < namespaceParts.Length - 1; i++)
			{
				if (i > 0)
				{
					parentNamespace.Append(".");
				}

				parentNamespace.Append(namespaceParts[i]);
			}

			return parentNamespace.ToString();
		}

		public static string GetNamespaceChildNamesapce(string @namespace)
		{
			if (!HasNamespaceParentNamespace(@namespace))
			{
				throw new Exception("Namespace does not have a parent namesapce.");
			}

			string[] namespaceParts = @namespace.Split('.');

			return namespaceParts[namespaceParts.Length - 1];
		}

		public static ICollection<TypeReference> GetTypeReferenceTypesDepedningOn(TypeReference reference)
		{
			HashSet<TypeReference> typesDependingOn = new HashSet<TypeReference>();

			TypeReference currentType = reference;
			while (currentType != null)
			{
				if (!typesDependingOn.Contains(currentType))
				{
					typesDependingOn.Add(currentType);
				}

				if (currentType.IsGenericInstance)
				{
					GenericInstanceType referenceGeneric = currentType as GenericInstanceType;

					foreach (TypeReference genericArgument in referenceGeneric.GenericArguments)
					{
						typesDependingOn.UnionWith(GetTypeReferenceTypesDepedningOn(genericArgument));
					}
				}

				currentType = currentType.DeclaringType;
			}

			return typesDependingOn;
		}

		public static FieldDefinition GetCompileGeneratedBackingField(PropertyDefinition property)
		{
			TypeDefinition declaringType = property.DeclaringType;
			if (!declaringType.HasFields)
			{
				return null;
			}
			foreach (FieldDefinition field in declaringType.Fields)
			{
				string fieldName = field.Name;
				if (fieldName.Equals("<" + property.Name + ">" + Constants.BackingFieldSuffix, StringComparison.Ordinal) &&
					field.HasCompilerGeneratedAttribute())
				{
					return field;
				}
			}
			return null;
		}

		public static string GetMemberUniqueName(IMemberDefinition member)
		{
			if (member is MethodDefinition)
			{
				MethodDefinition method = member as MethodDefinition;

				string name = method.FullName;
				if (method.HasGenericParameters)
				{
					foreach (GenericParameter genericParameter in method.GenericParameters)
					{
						name = name + genericParameter.Name;

						if (genericParameter.HasConstraints || genericParameter.HasDefaultConstructorConstraint ||
							genericParameter.HasReferenceTypeConstraint || genericParameter.HasNotNullableValueTypeConstraint)
						{
							bool wroteConstraint = false;
							if (genericParameter.HasNotNullableValueTypeConstraint)
							{
								if (wroteConstraint)
								{
									name = name + ", ";
								}
								wroteConstraint = true;
								name = name + "struct";
							}
							foreach (TypeReference constraint in genericParameter.Constraints)
							{
								if (genericParameter.HasNotNullableValueTypeConstraint && constraint.FullName == "System.ValueType")
								{
									continue;
								}
								if (wroteConstraint)
								{
									name = name + ", ";
								}
								name = name + constraint.FullName;
								wroteConstraint = true;
							}
							if (genericParameter.HasReferenceTypeConstraint)
							{
								if (wroteConstraint)
								{
									name = name + ", ";
								}
								wroteConstraint = true;
								name = name + "class";
							}

							if (genericParameter.HasDefaultConstructorConstraint && !genericParameter.HasNotNullableValueTypeConstraint)
							{
								if (wroteConstraint)
								{
									name = name + ", ";
								}
								wroteConstraint = true;
								name = name + "new()";
							}
						}
					}
				}

				return name;
			}
			else
			{
				return member.FullName;
			}
		}

		public static TypeDefinition GetOuterMostDeclaringType(IMemberDefinition member)
		{
			TypeDefinition type;

			if (member is TypeDefinition)
			{
				type = (member as TypeDefinition);
			}
			else
			{
				type = member.DeclaringType;
			}

			while (type.DeclaringType != null)
			{
				type = type.DeclaringType;
			}

			return type;
		}

		public static TypeDefinition GetDeclaringTypeOrSelf(IMemberDefinition member)
		{
			return member is TypeDefinition ? member as TypeDefinition : member.DeclaringType;
		}

		public static DecompiledMember TryGetDecompiledMember(MethodDefinition method, TypeSpecificContext typeContext, ILanguage language)
		{
			if (method.Body == null)
			{
				return new DecompiledMember(GetMemberUniqueName(method), null, null);
			}

			DecompilationContext innerContext = null;
			BlockStatement statement = method.Body.Decompile(language, out innerContext, typeContext);
			return new DecompiledMember(GetMemberUniqueName(method), statement, innerContext.MethodContext);
		}

		/// <summary>
		/// Adds all the types/interfaces that are base for any of the types in <paramref name="firstLevelDependanceTypes"/>.
		/// </summary>
		/// <param name="firstLevelDependanceTypes">The starting collection of dependant types.</param>
		/// <returns>Returns an updated collection, containing all types needed to the compiler.</returns>
		public static ICollection<TypeReference> GetExpandedTypeDependanceList(HashSet<TypeReference> firstLevelDependanceTypes)
		{
			HashSet<TypeReference> passed = new HashSet<TypeReference>();
			Queue<TypeReference> typesQueue = new Queue<TypeReference>();

			foreach (TypeReference type in firstLevelDependanceTypes)
			{
				typesQueue.Enqueue(type);
			}

			while (typesQueue.Count > 0)
			{
				TypeReference t = typesQueue.Dequeue();

				if (t.Scope.Name == "mscorlib" || passed.Contains(t))
				{
					continue;
				}
				passed.Add(t);

				if (t.DeclaringType != null)
				{
					typesQueue.Enqueue(t.DeclaringType);
				}

				TypeDefinition tDefinition = t.Resolve();
				if (tDefinition != null)
				{
					if (tDefinition.BaseType != null)
					{
						if (tDefinition.BaseType.Scope.Name != "mscorlib")
						{
							if (!passed.Contains(tDefinition.BaseType))
							{
								typesQueue.Enqueue(tDefinition.BaseType);
							}
						}
					}

					if (tDefinition.HasInterfaces)
					{
						foreach (TypeReference @interface in tDefinition.Interfaces)
						{
							if (@interface.Scope.Name == "mscorlib")
							{
								continue;
							}
							if (!passed.Contains(@interface))
							{
								typesQueue.Enqueue(@interface);
							}
						}
					}
				}
				else
				{
					//Sample error message: Assembly references needed for <thisMember> Will not be added to the project file.
					//Prompt for assembly location
				}
			}

			return passed;
		}

		public static ICollection<AssemblyNameReference> GetAssembliesDependingOn(ModuleDefinition module, ICollection<TypeReference> typesDependingOn)
		{
            return new HashSet<AssemblyNameReference>(GetAssembliesDependingOnToUsedTypesMap(module, typesDependingOn).Keys);
		}

        public static Dictionary<AssemblyNameReference, List<TypeReference>> GetAssembliesDependingOnToUsedTypesMap(ModuleDefinition module, ICollection<TypeReference> typesDependingOn)
        {
            Dictionary<AssemblyNameReference, List<TypeReference>> result = new Dictionary<AssemblyNameReference, List<TypeReference>>(new AssemblyNameReferenceEqualityComparer());
            foreach (TypeReference type in typesDependingOn)
            {
                AssemblyNameReference assemblyToAdd = null;

                ModuleDefinition typeModule = type.Scope as ModuleDefinition;
                if (typeModule != null && typeModule.Kind != ModuleKind.NetModule)
                {
                    assemblyToAdd = typeModule.Assembly.Name;
                }
                else if (type.Scope is AssemblyNameReference)
                {
                    assemblyToAdd = type.Scope as AssemblyNameReference;
                }
                if (assemblyToAdd != null && (module == module.Assembly.MainModule && assemblyToAdd != module.Assembly.Name))
                {
                    if (!result.ContainsKey(assemblyToAdd))
                    {
                        result.Add(assemblyToAdd, new List<TypeReference>());
                    }

                    result[assemblyToAdd].Add(type);
                }
            }
            return result;
        }

		public static ICollection<ModuleReference> GetModulesDependingOn(ICollection<TypeReference> typesDependingOn)
		{
			HashSet<ModuleReference> result = new HashSet<ModuleReference>();

			foreach (TypeReference type in typesDependingOn)
			{
				if (type.Scope is ModuleReference && !(type.Scope is AssemblyNameReference || type.Scope is ModuleDefinition))
				{
					ModuleReference module = type.Scope as ModuleReference;
					if (!result.Contains(module))
					{
						result.Add(module);
					}
				}
			}

			return result;
		}

		public static ICollection<string> GetAssemblyAndModuleNamespaceUsings(AssemblySpecificContext assemblyContext, ModuleSpecificContext moduleContext)
		{
			ICollection<string> result = new HashSet<string>();

			foreach (string namespaceUsing in assemblyContext.AssemblyNamespaceUsings)
			{
				result.Add(namespaceUsing);
			}

			foreach (string namespaceUsing in moduleContext.ModuleNamespaceUsings)
			{
				if (!result.Contains(namespaceUsing))
				{
					result.Add(namespaceUsing);
				}
			}

			return result;
		}

		public static string EscapeNameIfNeeded(string name, ILanguage language)
		{
			string result = name;

			if (language.IsLanguageKeyword(result))
			{
                result = Escape(name, language);
			}

			return result;
		}

		public static string EscapeTypeNameIfNeeded(string typeName, ILanguage language)
		{
			string result = typeName;

			if (language.IsGlobalKeyword(result))
			{
                result = Escape(typeName, language);
			}

			return result;
		}

        internal static string Escape(string name, ILanguage language)
        {
            return language.EscapeWord(name);
        }

		public static string EscapeNamespaceIfNeeded(string @namespace, ILanguage language)
		{
			StringBuilder sb = new StringBuilder();
			bool changed = false;
			foreach (string part in @namespace.Split('.'))
			{
				string newPart = part;
				if (!language.IsValidIdentifier(newPart))
				{
					newPart = language.ReplaceInvalidCharactersInIdentifier(newPart);
					changed = true;
				}
				if (language.IsGlobalKeyword(newPart))
				{
					newPart = language.EscapeWord(newPart);
					changed = true;
				}
				if (sb.Length > 0)
					sb.Append(".");
				sb.Append(newPart);
			}
			return changed ? sb.ToString() : @namespace;
		}

		public static bool IsInitializerPresent(InitializerExpression initializer)
		{
			return initializer != null && initializer.Expression != null && initializer.Expression.Expressions.Count > 0;
		}

		public static bool IsExplicitInterfaceImplementataion(IMemberDefinition theDefinition)
		{
			if (theDefinition is MethodDefinition)
			{
				return ((MethodDefinition)theDefinition).HasOverrides;
			}
			if (theDefinition is PropertyDefinition)
			{
				return ((PropertyDefinition)theDefinition).IsExplicitImplementation();
			}
			if (theDefinition is EventDefinition)
			{
				return ((EventDefinition)theDefinition).IsExplicitImplementation();
			}
			return false;
		}

        public static AssemblyDefinition GetAssembly(string assemblyPath)
        {
            WeakAssemblyResolver assemblyResolver = new WeakAssemblyResolver(GlobalAssemblyResolver.CurrentAssemblyPathCache);
            ReaderParameters readerParameters = new ReaderParameters(assemblyResolver);
            return assemblyResolver.LoadAssemblyDefinition(assemblyPath, readerParameters, loadPdb: true);
        }

        /// <summary>
        /// Use this method to check if given <see cref="MethodReference"/> is ComputeStringHash method, which is used when
        /// switch by string is generated by the compiler (C# 6.0 and above).
        /// </summary>
        /// <param name="method">The method to be checked.</param>
        /// <returns>true if the method is a ComputeStringHash method, false otherwise.</returns>
        public static bool IsComputeStringHashMethod(MethodReference method)
        {
            if (method.FullName == "System.UInt32 <PrivateImplementationDetails>::ComputeStringHash(System.String)")
            {
                return true;
            }
            else if (method.FullName == "System.UInt32 <PrivateImplementationDetails>::$$method0x6000001-ComputeStringHash(System.String)")
            {
                return true;
            }

            return false;
        }

		private class AssemblyNameReferenceEqualityComparer : IEqualityComparer<AssemblyNameReference>
		{
			public int GetHashCode(AssemblyNameReference obj)
			{
				return obj.FullName.GetHashCode();
			}

			public bool Equals(AssemblyNameReference x, AssemblyNameReference y)
			{
				if (x == null && y == null)
				{
					return true;
				}
				if (x == null || y == null)
				{
					return false;
				}
				return x.FullName == y.FullName;
			}
		}
	}
}
