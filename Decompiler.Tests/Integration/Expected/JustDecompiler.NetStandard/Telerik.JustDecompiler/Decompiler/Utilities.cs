using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public class Utilities
	{
		public Utilities()
		{
		}

		private static bool ArgumentsMatch(Mono.Collections.Generic.Collection<ParameterDefinition> parameters, IList<Type> arguments)
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

		internal static string Escape(string name, ILanguage language)
		{
			return language.EscapeWord(name);
		}

		public static string EscapeNameIfNeeded(string name, ILanguage language)
		{
			string str = name;
			if (language.IsLanguageKeyword(str))
			{
				str = Utilities.Escape(name, language);
			}
			return str;
		}

		public static string EscapeNamespaceIfNeeded(string @namespace, ILanguage language)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			string[] strArray = @namespace.Split(new Char[] { '.' });
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (!language.IsValidIdentifier(str))
				{
					str = language.ReplaceInvalidCharactersInIdentifier(str);
					flag = true;
				}
				if (language.IsGlobalKeyword(str))
				{
					str = language.EscapeWord(str);
					flag = true;
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(".");
				}
				stringBuilder.Append(str);
			}
			if (!flag)
			{
				return @namespace;
			}
			return stringBuilder.ToString();
		}

		public static string EscapeTypeNameIfNeeded(string typeName, ILanguage language)
		{
			string str = typeName;
			if (language.IsGlobalKeyword(str))
			{
				str = Utilities.Escape(typeName, language);
			}
			return str;
		}

		public static MemberReference FindMemberArgumentRefersTo(IList fieldsAndProperties, Mono.Cecil.CustomAttributeNamedArgument argument)
		{
			MemberReference memberReference = null;
			foreach (object fieldsAndProperty in fieldsAndProperties)
			{
				if ((fieldsAndProperty as MemberReference).Name != argument.Name)
				{
					continue;
				}
				memberReference = fieldsAndProperty as MemberReference;
			}
			return memberReference;
		}

		public static ICollection<AssemblyNameReference> GetAssembliesDependingOn(ModuleDefinition module, ICollection<TypeReference> typesDependingOn)
		{
			return new HashSet<AssemblyNameReference>(Utilities.GetAssembliesDependingOnToUsedTypesMap(module, typesDependingOn).Keys);
		}

		public static Dictionary<AssemblyNameReference, List<TypeReference>> GetAssembliesDependingOnToUsedTypesMap(ModuleDefinition module, ICollection<TypeReference> typesDependingOn)
		{
			Dictionary<AssemblyNameReference, List<TypeReference>> assemblyNameReferences = new Dictionary<AssemblyNameReference, List<TypeReference>>(new Utilities.AssemblyNameReferenceEqualityComparer());
			foreach (TypeReference typeReference in typesDependingOn)
			{
				AssemblyNameReference name = null;
				ModuleDefinition scope = typeReference.Scope as ModuleDefinition;
				if (scope != null && scope.Kind != ModuleKind.NetModule)
				{
					name = scope.Assembly.Name;
				}
				else if (typeReference.Scope is AssemblyNameReference)
				{
					name = typeReference.Scope as AssemblyNameReference;
				}
				if (name == null || module != module.Assembly.MainModule || name == module.Assembly.Name)
				{
					continue;
				}
				if (!assemblyNameReferences.ContainsKey(name))
				{
					assemblyNameReferences.Add(name, new List<TypeReference>());
				}
				assemblyNameReferences[name].Add(typeReference);
			}
			return assemblyNameReferences;
		}

		public static AssemblyDefinition GetAssembly(string assemblyPath)
		{
			WeakAssemblyResolver weakAssemblyResolver = new WeakAssemblyResolver(GlobalAssemblyResolver.CurrentAssemblyPathCache);
			return weakAssemblyResolver.LoadAssemblyDefinition(assemblyPath, new ReaderParameters(weakAssemblyResolver), true);
		}

		public static ICollection<string> GetAssemblyAndModuleNamespaceUsings(AssemblySpecificContext assemblyContext, ModuleSpecificContext moduleContext)
		{
			ICollection<string> strs = new HashSet<string>();
			foreach (string assemblyNamespaceUsing in assemblyContext.AssemblyNamespaceUsings)
			{
				strs.Add(assemblyNamespaceUsing);
			}
			foreach (string moduleNamespaceUsing in moduleContext.ModuleNamespaceUsings)
			{
				if (strs.Contains(moduleNamespaceUsing))
				{
					continue;
				}
				strs.Add(moduleNamespaceUsing);
			}
			return strs;
		}

		public static FieldDefinition GetCompileGeneratedBackingField(PropertyDefinition property)
		{
			FieldDefinition fieldDefinition;
			TypeDefinition declaringType = property.DeclaringType;
			if (!declaringType.HasFields)
			{
				return null;
			}
			Mono.Collections.Generic.Collection<FieldDefinition>.Enumerator enumerator = declaringType.Fields.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.Current;
					if (!current.Name.Equals(String.Concat("<", property.Name, ">k__BackingField"), StringComparison.Ordinal) || !current.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					fieldDefinition = current;
					return fieldDefinition;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return fieldDefinition;
		}

		public static TypeReference GetCorlibTypeReference(Type type, ModuleDefinition currentModule)
		{
			AssemblyNameReference assemblyNameReference = currentModule.ReferencedMscorlibRef() ?? (currentModule.GetReferencedCoreLibraryRef("System.Runtime") ?? (currentModule.GetReferencedCoreLibraryRef("System.Private.CoreLib") ?? currentModule.GetReferencedCoreLibraryRef("netstandard")));
			return new TypeReference(type.Namespace, type.Name, currentModule, assemblyNameReference);
		}

		public static TypeDefinition GetDeclaringTypeOrSelf(IMemberDefinition member)
		{
			if (!(member is TypeDefinition))
			{
				return member.DeclaringType;
			}
			return member as TypeDefinition;
		}

		public static MethodReference GetEmptyConstructor(Type type, ModuleDefinition currentModule, IList<Type> mscorlibArgumentTypes = null)
		{
			MethodReference methodReference;
			if (mscorlibArgumentTypes == null)
			{
				mscorlibArgumentTypes = new List<Type>();
			}
			TypeReference corlibTypeReference = Utilities.GetCorlibTypeReference(type, currentModule);
			TypeDefinition typeDefinition = corlibTypeReference.Resolve();
			if (typeDefinition == null)
			{
				MethodReference methodReference1 = new MethodReference(".ctor", Utilities.GetCorlibTypeReference(typeof(Void), currentModule), corlibTypeReference);
				methodReference1.Parameters.AddRange(Utilities.GetMatchingArguments(mscorlibArgumentTypes, currentModule));
				return methodReference1;
			}
			Mono.Collections.Generic.Collection<MethodDefinition>.Enumerator enumerator = typeDefinition.Methods.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MethodDefinition current = enumerator.Current;
					if (!current.IsConstructor || !Utilities.ArgumentsMatch(current.Parameters, mscorlibArgumentTypes))
					{
						continue;
					}
					methodReference = current;
					return methodReference;
				}
				throw new ArgumentOutOfRangeException(String.Format("Type {0} doesnt provide matching constructor.", type.FullName));
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return methodReference;
		}

		public static ICollection<TypeReference> GetExpandedTypeDependanceList(HashSet<TypeReference> firstLevelDependanceTypes)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			Queue<TypeReference> typeReferences1 = new Queue<TypeReference>();
			foreach (TypeReference firstLevelDependanceType in firstLevelDependanceTypes)
			{
				typeReferences1.Enqueue(firstLevelDependanceType);
			}
			while (typeReferences1.Count > 0)
			{
				TypeReference typeReference = typeReferences1.Dequeue();
				if (typeReference.Scope.Name == "mscorlib" || typeReferences.Contains(typeReference))
				{
					continue;
				}
				typeReferences.Add(typeReference);
				if (typeReference.DeclaringType != null)
				{
					typeReferences1.Enqueue(typeReference.DeclaringType);
				}
				TypeDefinition typeDefinition = typeReference.Resolve();
				if (typeDefinition == null)
				{
					continue;
				}
				if (typeDefinition.BaseType != null && typeDefinition.BaseType.Scope.Name != "mscorlib" && !typeReferences.Contains(typeDefinition.BaseType))
				{
					typeReferences1.Enqueue(typeDefinition.BaseType);
				}
				if (!typeDefinition.HasInterfaces)
				{
					continue;
				}
				foreach (TypeReference @interface in typeDefinition.Interfaces)
				{
					if (@interface.Scope.Name == "mscorlib" || typeReferences.Contains(@interface))
					{
						continue;
					}
					typeReferences1.Enqueue(@interface);
				}
			}
			return typeReferences;
		}

		private static IEnumerable<ParameterDefinition> GetMatchingArguments(IList<Type> mscorlibArgumentTypes, ModuleDefinition currentModule)
		{
			List<ParameterDefinition> parameterDefinitions = new List<ParameterDefinition>(mscorlibArgumentTypes.Count);
			foreach (Type mscorlibArgumentType in mscorlibArgumentTypes)
			{
				parameterDefinitions.Add(new ParameterDefinition(Utilities.GetCorlibTypeReference(mscorlibArgumentType, currentModule)));
			}
			return parameterDefinitions;
		}

		public static string GetMemberUniqueName(IMemberDefinition member)
		{
			if (!(member is MethodDefinition))
			{
				return member.FullName;
			}
			MethodDefinition methodDefinition = member as MethodDefinition;
			string fullName = methodDefinition.FullName;
			if (methodDefinition.HasGenericParameters)
			{
				foreach (GenericParameter genericParameter in methodDefinition.GenericParameters)
				{
					fullName = String.Concat(fullName, genericParameter.Name);
					if (!genericParameter.HasConstraints && !genericParameter.HasDefaultConstructorConstraint && !genericParameter.HasReferenceTypeConstraint && !genericParameter.HasNotNullableValueTypeConstraint)
					{
						continue;
					}
					bool flag = false;
					if (genericParameter.HasNotNullableValueTypeConstraint)
					{
						if (flag)
						{
							fullName = String.Concat(fullName, ", ");
						}
						flag = true;
						fullName = String.Concat(fullName, "struct");
					}
					foreach (TypeReference constraint in genericParameter.Constraints)
					{
						if (genericParameter.HasNotNullableValueTypeConstraint && constraint.FullName == "System.ValueType")
						{
							continue;
						}
						if (flag)
						{
							fullName = String.Concat(fullName, ", ");
						}
						fullName = String.Concat(fullName, constraint.FullName);
						flag = true;
					}
					if (genericParameter.HasReferenceTypeConstraint)
					{
						if (flag)
						{
							fullName = String.Concat(fullName, ", ");
						}
						flag = true;
						fullName = String.Concat(fullName, "class");
					}
					if (!genericParameter.HasDefaultConstructorConstraint || genericParameter.HasNotNullableValueTypeConstraint)
					{
						continue;
					}
					if (flag)
					{
						fullName = String.Concat(fullName, ", ");
					}
					flag = true;
					fullName = String.Concat(fullName, "new()");
				}
			}
			return fullName;
		}

		public static ICollection<ModuleReference> GetModulesDependingOn(ICollection<TypeReference> typesDependingOn)
		{
			HashSet<ModuleReference> moduleReferences = new HashSet<ModuleReference>();
			foreach (TypeReference typeReference in typesDependingOn)
			{
				if (!(typeReference.Scope is ModuleReference) || typeReference.Scope is AssemblyNameReference || typeReference.Scope is ModuleDefinition)
				{
					continue;
				}
				ModuleReference scope = typeReference.Scope as ModuleReference;
				if (moduleReferences.Contains(scope))
				{
					continue;
				}
				moduleReferences.Add(scope);
			}
			return moduleReferences;
		}

		public static string GetNamesapceParentNamesapce(string @namespace)
		{
			if (!Utilities.HasNamespaceParentNamespace(@namespace))
			{
				throw new Exception("Namespace does not have a parent namesapce.");
			}
			string[] strArray = @namespace.Split(new Char[] { '.' });
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < (int)strArray.Length - 1; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(".");
				}
				stringBuilder.Append(strArray[i]);
			}
			return stringBuilder.ToString();
		}

		public static string GetNamespaceChildNamesapce(string @namespace)
		{
			if (!Utilities.HasNamespaceParentNamespace(@namespace))
			{
				throw new Exception("Namespace does not have a parent namesapce.");
			}
			string[] strArray = @namespace.Split(new Char[] { '.' });
			return strArray[(int)strArray.Length - 1];
		}

		public static TypeDefinition GetOuterMostDeclaringType(IMemberDefinition member)
		{
			TypeDefinition declaringType;
			declaringType = (!(member is TypeDefinition) ? member.DeclaringType : member as TypeDefinition);
			while (declaringType.DeclaringType != null)
			{
				declaringType = declaringType.DeclaringType;
			}
			return declaringType;
		}

		public static List<IMemberDefinition> GetTypeMembers(TypeDefinition type, ILanguage language, bool showCompilerGeneratedMembers = true, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null, IEnumerable<MethodDefinition> generatedFilterMethods = null, IEnumerable<FieldReference> propertyFields = null)
		{
			return TypeDefinitionExtensions.GetMembersSorted(type, showCompilerGeneratedMembers, language, attributesToSkip, fieldsToSkip, new HashSet<FieldReference>(type.GetFieldToEventMap(language).Keys), generatedFilterMethods, propertyFields).ToList<IMemberDefinition>();
		}

		public static List<IMemberDefinition> GetTypeMembersToDecompile(TypeDefinition type)
		{
			return TypeDefinitionExtensions.GetMembersToDecompile(type, true).ToList<IMemberDefinition>();
		}

		public static ICollection<TypeReference> GetTypeReferenceTypesDepedningOn(TypeReference reference)
		{
			HashSet<TypeReference> typeReferences = new HashSet<TypeReference>();
			for (TypeReference i = reference; i != null; i = i.DeclaringType)
			{
				if (!typeReferences.Contains(i))
				{
					typeReferences.Add(i);
				}
				if (i.IsGenericInstance)
				{
					foreach (TypeReference genericArgument in (i as GenericInstanceType).GenericArguments)
					{
						typeReferences.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(genericArgument));
					}
				}
			}
			return typeReferences;
		}

		public static bool HasNamespaceParentNamespace(string @namespace)
		{
			return @namespace.Contains(".");
		}

		public static bool IsComputeStringHashMethod(MethodReference method)
		{
			if (method.FullName == "System.UInt32 <PrivateImplementationDetails>::ComputeStringHash(System.String)")
			{
				return true;
			}
			if (method.FullName == "System.UInt32 <PrivateImplementationDetails>::$$method0x6000001-ComputeStringHash(System.String)")
			{
				return true;
			}
			return false;
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
			if (!(theDefinition is EventDefinition))
			{
				return false;
			}
			return ((EventDefinition)theDefinition).IsExplicitImplementation();
		}

		public static bool IsInitializerPresent(InitializerExpression initializer)
		{
			if (initializer == null || initializer.Expression == null)
			{
				return false;
			}
			return initializer.Expression.Expressions.Count > 0;
		}

		public static bool IsTypeNameInCollisionOnAssemblyLevel(string typeName, AssemblySpecificContext assemblyContext, ModuleSpecificContext mainModuleContext)
		{
			List<string> strs;
			HashSet<string> strs1 = new HashSet<string>();
			foreach (string assemblyNamespaceUsing in assemblyContext.AssemblyNamespaceUsings)
			{
				strs1.Add(assemblyNamespaceUsing);
			}
			strs1.UnionWith(mainModuleContext.ModuleNamespaceUsings);
			if (mainModuleContext.CollisionTypesData.TryGetValue(typeName, out strs) && strs.Intersect<string>(strs1).Count<string>() > 1)
			{
				return true;
			}
			return false;
		}

		public static DecompiledMember TryGetDecompiledMember(MethodDefinition method, TypeSpecificContext typeContext, ILanguage language)
		{
			if (method.Body == null)
			{
				return new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null);
			}
			DecompilationContext decompilationContext = null;
			BlockStatement blockStatement = method.Body.Decompile(language, out decompilationContext, typeContext);
			return new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, decompilationContext.MethodContext);
		}

		private class AssemblyNameReferenceEqualityComparer : IEqualityComparer<AssemblyNameReference>
		{
			public AssemblyNameReferenceEqualityComparer()
			{
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

			public int GetHashCode(AssemblyNameReference obj)
			{
				return obj.FullName.GetHashCode();
			}
		}
	}
}