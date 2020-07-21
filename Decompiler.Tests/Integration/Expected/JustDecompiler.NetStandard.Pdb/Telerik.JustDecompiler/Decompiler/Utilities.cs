using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public class Utilities
	{
		public Utilities()
		{
			base();
			return;
		}

		private static bool ArgumentsMatch(Collection<ParameterDefinition> parameters, IList<Type> arguments)
		{
			if (parameters == null && arguments.get_Count() != 0)
			{
				return false;
			}
			if (parameters.get_Count() != arguments.get_Count())
			{
				return false;
			}
			V_0 = 0;
			while (V_0 < parameters.get_Count())
			{
				if (String.op_Inequality(parameters.get_Item(V_0).get_ParameterType().get_FullName(), arguments.get_Item(V_0).get_FullName()))
				{
					return false;
				}
				V_0 = V_0 + 1;
			}
			return true;
		}

		internal static string Escape(string name, ILanguage language)
		{
			return language.EscapeWord(name);
		}

		public static string EscapeNameIfNeeded(string name, ILanguage language)
		{
			V_0 = name;
			if (language.IsLanguageKeyword(V_0))
			{
				V_0 = Utilities.Escape(name, language);
			}
			return V_0;
		}

		public static string EscapeNamespaceIfNeeded(string namespace, ILanguage language)
		{
			V_0 = new StringBuilder();
			V_1 = false;
			stackVariable4 = new Char[1];
			stackVariable4[0] = '.';
			V_2 = namespace.Split(stackVariable4);
			V_3 = 0;
			while (V_3 < (int)V_2.Length)
			{
				V_4 = V_2[V_3];
				if (!language.IsValidIdentifier(V_4))
				{
					V_4 = language.ReplaceInvalidCharactersInIdentifier(V_4);
					V_1 = true;
				}
				if (language.IsGlobalKeyword(V_4))
				{
					V_4 = language.EscapeWord(V_4);
					V_1 = true;
				}
				if (V_0.get_Length() > 0)
				{
					dummyVar0 = V_0.Append(".");
				}
				dummyVar1 = V_0.Append(V_4);
				V_3 = V_3 + 1;
			}
			if (!V_1)
			{
				return namespace;
			}
			return V_0.ToString();
		}

		public static string EscapeTypeNameIfNeeded(string typeName, ILanguage language)
		{
			V_0 = typeName;
			if (language.IsGlobalKeyword(V_0))
			{
				V_0 = Utilities.Escape(typeName, language);
			}
			return V_0;
		}

		public static MemberReference FindMemberArgumentRefersTo(IList fieldsAndProperties, CustomAttributeNamedArgument argument)
		{
			V_0 = null;
			V_1 = fieldsAndProperties.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!String.op_Equality((V_2 as MemberReference).get_Name(), argument.get_Name()))
					{
						continue;
					}
					V_0 = V_2 as MemberReference;
				}
			}
			finally
			{
				V_3 = V_1 as IDisposable;
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			return V_0;
		}

		public static ICollection<AssemblyNameReference> GetAssembliesDependingOn(ModuleDefinition module, ICollection<TypeReference> typesDependingOn)
		{
			return new HashSet<AssemblyNameReference>(Utilities.GetAssembliesDependingOnToUsedTypesMap(module, typesDependingOn).get_Keys());
		}

		public static Dictionary<AssemblyNameReference, List<TypeReference>> GetAssembliesDependingOnToUsedTypesMap(ModuleDefinition module, ICollection<TypeReference> typesDependingOn)
		{
			V_0 = new Dictionary<AssemblyNameReference, List<TypeReference>>(new Utilities.AssemblyNameReferenceEqualityComparer());
			V_1 = typesDependingOn.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = null;
					V_4 = V_2.get_Scope() as ModuleDefinition;
					if (V_4 == null || V_4.get_Kind() == 3)
					{
						if (V_2.get_Scope() as AssemblyNameReference != null)
						{
							V_3 = V_2.get_Scope() as AssemblyNameReference;
						}
					}
					else
					{
						V_3 = V_4.get_Assembly().get_Name();
					}
					if (V_3 == null || (object)module != (object)module.get_Assembly().get_MainModule() || V_3 == module.get_Assembly().get_Name())
					{
						continue;
					}
					if (!V_0.ContainsKey(V_3))
					{
						V_0.Add(V_3, new List<TypeReference>());
					}
					V_0.get_Item(V_3).Add(V_2);
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

		public static AssemblyDefinition GetAssembly(string assemblyPath)
		{
			stackVariable1 = new WeakAssemblyResolver(GlobalAssemblyResolver.CurrentAssemblyPathCache);
			V_0 = new ReaderParameters(stackVariable1);
			return ((BaseAssemblyResolver)stackVariable1).LoadAssemblyDefinition(assemblyPath, V_0, true);
		}

		public static ICollection<string> GetAssemblyAndModuleNamespaceUsings(AssemblySpecificContext assemblyContext, ModuleSpecificContext moduleContext)
		{
			V_0 = new HashSet<string>();
			V_1 = assemblyContext.get_AssemblyNamespaceUsings().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			V_1 = moduleContext.get_ModuleNamespaceUsings().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_3 = V_1.get_Current();
					if (V_0.Contains(V_3))
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

		public static FieldDefinition GetCompileGeneratedBackingField(PropertyDefinition property)
		{
			V_0 = property.get_DeclaringType();
			if (!V_0.get_HasFields())
			{
				return null;
			}
			V_1 = V_0.get_Fields().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!V_2.get_Name().Equals(String.Concat("<", property.get_Name(), ">k__BackingField"), 4) || !V_2.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					V_3 = V_2;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_1.Dispose();
			}
		Label1:
			return V_3;
		Label0:
			return null;
		}

		public static TypeReference GetCorlibTypeReference(Type type, ModuleDefinition currentModule)
		{
			V_0 = currentModule.ReferencedMscorlibRef();
			if (V_0 == null)
			{
				V_0 = currentModule.GetReferencedCoreLibraryRef("System.Runtime");
				if (V_0 == null)
				{
					V_0 = currentModule.GetReferencedCoreLibraryRef("System.Private.CoreLib");
					if (V_0 == null)
					{
						V_0 = currentModule.GetReferencedCoreLibraryRef("netstandard");
					}
				}
			}
			return new TypeReference(type.get_Namespace(), type.get_Name(), currentModule, V_0);
		}

		public static TypeDefinition GetDeclaringTypeOrSelf(IMemberDefinition member)
		{
			if (member as TypeDefinition == null)
			{
				return member.get_DeclaringType();
			}
			return member as TypeDefinition;
		}

		public static MethodReference GetEmptyConstructor(Type type, ModuleDefinition currentModule, IList<Type> mscorlibArgumentTypes = null)
		{
			if (mscorlibArgumentTypes == null)
			{
				mscorlibArgumentTypes = new List<Type>();
			}
			V_0 = Utilities.GetCorlibTypeReference(type, currentModule);
			V_1 = V_0.Resolve();
			if (V_1 == null)
			{
				stackVariable13 = new MethodReference(".ctor", Utilities.GetCorlibTypeReference(Type.GetTypeFromHandle(// 
				// Current member / type: Mono.Cecil.MethodReference Telerik.JustDecompiler.Decompiler.Utilities::GetEmptyConstructor(System.Type,Mono.Cecil.ModuleDefinition,System.Collections.Generic.IList`1<System.Type>)
				// Exception in: Mono.Cecil.MethodReference GetEmptyConstructor(System.Type,Mono.Cecil.ModuleDefinition,System.Collections.Generic.IList<System.Type>)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public static ICollection<TypeReference> GetExpandedTypeDependanceList(HashSet<TypeReference> firstLevelDependanceTypes)
		{
			V_0 = new HashSet<TypeReference>();
			V_1 = new Queue<TypeReference>();
			V_2 = firstLevelDependanceTypes.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_1.Enqueue(V_3);
				}
			}
			finally
			{
				((IDisposable)V_2).Dispose();
			}
			while (V_1.get_Count() > 0)
			{
				V_4 = V_1.Dequeue();
				if (String.op_Equality(V_4.get_Scope().get_Name(), "mscorlib") || V_0.Contains(V_4))
				{
					continue;
				}
				dummyVar0 = V_0.Add(V_4);
				if (V_4.get_DeclaringType() != null)
				{
					V_1.Enqueue(V_4.get_DeclaringType());
				}
				V_5 = V_4.Resolve();
				if (V_5 == null)
				{
					continue;
				}
				if (V_5.get_BaseType() != null && String.op_Inequality(V_5.get_BaseType().get_Scope().get_Name(), "mscorlib") && !V_0.Contains(V_5.get_BaseType()))
				{
					V_1.Enqueue(V_5.get_BaseType());
				}
				if (!V_5.get_HasInterfaces())
				{
					continue;
				}
				V_6 = V_5.get_Interfaces().GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = V_6.get_Current();
						if (String.op_Equality(V_7.get_Scope().get_Name(), "mscorlib") || V_0.Contains(V_7))
						{
							continue;
						}
						V_1.Enqueue(V_7);
					}
				}
				finally
				{
					V_6.Dispose();
				}
			}
			return V_0;
		}

		private static IEnumerable<ParameterDefinition> GetMatchingArguments(IList<Type> mscorlibArgumentTypes, ModuleDefinition currentModule)
		{
			V_0 = new List<ParameterDefinition>(mscorlibArgumentTypes.get_Count());
			V_1 = mscorlibArgumentTypes.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(new ParameterDefinition(Utilities.GetCorlibTypeReference(V_2, currentModule)));
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

		public static string GetMemberUniqueName(IMemberDefinition member)
		{
			if (member as MethodDefinition == null)
			{
				return member.get_FullName();
			}
			V_0 = member as MethodDefinition;
			V_1 = V_0.get_FullName();
			if (V_0.get_HasGenericParameters())
			{
				V_2 = V_0.get_GenericParameters().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						V_1 = String.Concat(V_1, V_3.get_Name());
						if (!V_3.get_HasConstraints() && !V_3.get_HasDefaultConstructorConstraint() && !V_3.get_HasReferenceTypeConstraint() && !V_3.get_HasNotNullableValueTypeConstraint())
						{
							continue;
						}
						V_4 = false;
						if (V_3.get_HasNotNullableValueTypeConstraint())
						{
							if (V_4)
							{
								V_1 = String.Concat(V_1, ", ");
							}
							V_4 = true;
							V_1 = String.Concat(V_1, "struct");
						}
						V_5 = V_3.get_Constraints().GetEnumerator();
						try
						{
							while (V_5.MoveNext())
							{
								V_6 = V_5.get_Current();
								if (V_3.get_HasNotNullableValueTypeConstraint() && String.op_Equality(V_6.get_FullName(), "System.ValueType"))
								{
									continue;
								}
								if (V_4)
								{
									V_1 = String.Concat(V_1, ", ");
								}
								V_1 = String.Concat(V_1, V_6.get_FullName());
								V_4 = true;
							}
						}
						finally
						{
							V_5.Dispose();
						}
						if (V_3.get_HasReferenceTypeConstraint())
						{
							if (V_4)
							{
								V_1 = String.Concat(V_1, ", ");
							}
							V_4 = true;
							V_1 = String.Concat(V_1, "class");
						}
						if (!V_3.get_HasDefaultConstructorConstraint() || V_3.get_HasNotNullableValueTypeConstraint())
						{
							continue;
						}
						if (V_4)
						{
							V_1 = String.Concat(V_1, ", ");
						}
						V_4 = true;
						V_1 = String.Concat(V_1, "new()");
					}
				}
				finally
				{
					V_2.Dispose();
				}
			}
			return V_1;
		}

		public static ICollection<ModuleReference> GetModulesDependingOn(ICollection<TypeReference> typesDependingOn)
		{
			V_0 = new HashSet<ModuleReference>();
			V_1 = typesDependingOn.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Scope() as ModuleReference == null || V_2.get_Scope() as AssemblyNameReference != null || V_2.get_Scope() as ModuleDefinition != null)
					{
						continue;
					}
					V_3 = V_2.get_Scope() as ModuleReference;
					if (V_0.Contains(V_3))
					{
						continue;
					}
					dummyVar0 = V_0.Add(V_3);
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

		public static string GetNamesapceParentNamesapce(string namespace)
		{
			if (!Utilities.HasNamespaceParentNamespace(namespace))
			{
				throw new Exception("Namespace does not have a parent namesapce.");
			}
			stackVariable4 = new Char[1];
			stackVariable4[0] = '.';
			V_0 = namespace.Split(stackVariable4);
			V_1 = new StringBuilder();
			V_2 = 0;
			while (V_2 < (int)V_0.Length - 1)
			{
				if (V_2 > 0)
				{
					dummyVar0 = V_1.Append(".");
				}
				dummyVar1 = V_1.Append(V_0[V_2]);
				V_2 = V_2 + 1;
			}
			return V_1.ToString();
		}

		public static string GetNamespaceChildNamesapce(string namespace)
		{
			if (!Utilities.HasNamespaceParentNamespace(namespace))
			{
				throw new Exception("Namespace does not have a parent namesapce.");
			}
			stackVariable4 = new Char[1];
			stackVariable4[0] = '.';
			stackVariable7 = namespace.Split(stackVariable4);
			return stackVariable7[(int)stackVariable7.Length - 1];
		}

		public static TypeDefinition GetOuterMostDeclaringType(IMemberDefinition member)
		{
			if (member as TypeDefinition == null)
			{
				V_0 = member.get_DeclaringType();
			}
			else
			{
				V_0 = member as TypeDefinition;
			}
			while (V_0.get_DeclaringType() != null)
			{
				V_0 = V_0.get_DeclaringType();
			}
			return V_0;
		}

		public static List<IMemberDefinition> GetTypeMembers(TypeDefinition type, ILanguage language, bool showCompilerGeneratedMembers = true, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null, IEnumerable<MethodDefinition> generatedFilterMethods = null, IEnumerable<FieldReference> propertyFields = null)
		{
			return TypeDefinitionExtensions.GetMembersSorted(type, showCompilerGeneratedMembers, language, attributesToSkip, fieldsToSkip, new HashSet<FieldReference>(type.GetFieldToEventMap(language).get_Keys()), generatedFilterMethods, propertyFields).ToList<IMemberDefinition>();
		}

		public static List<IMemberDefinition> GetTypeMembersToDecompile(TypeDefinition type)
		{
			return TypeDefinitionExtensions.GetMembersToDecompile(type, true).ToList<IMemberDefinition>();
		}

		public static ICollection<TypeReference> GetTypeReferenceTypesDepedningOn(TypeReference reference)
		{
			V_0 = new HashSet<TypeReference>();
			V_1 = reference;
			while (V_1 != null)
			{
				if (!V_0.Contains(V_1))
				{
					dummyVar0 = V_0.Add(V_1);
				}
				if (V_1.get_IsGenericInstance())
				{
					V_2 = (V_1 as GenericInstanceType).get_GenericArguments().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							V_0.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(V_3));
						}
					}
					finally
					{
						V_2.Dispose();
					}
				}
				V_1 = V_1.get_DeclaringType();
			}
			return V_0;
		}

		public static bool HasNamespaceParentNamespace(string namespace)
		{
			return namespace.Contains(".");
		}

		public static bool IsComputeStringHashMethod(MethodReference method)
		{
			if (String.op_Equality(method.get_FullName(), "System.UInt32 <PrivateImplementationDetails>::ComputeStringHash(System.String)"))
			{
				return true;
			}
			if (String.op_Equality(method.get_FullName(), "System.UInt32 <PrivateImplementationDetails>::$$method0x6000001-ComputeStringHash(System.String)"))
			{
				return true;
			}
			return false;
		}

		public static bool IsExplicitInterfaceImplementataion(IMemberDefinition theDefinition)
		{
			if (theDefinition as MethodDefinition != null)
			{
				return ((MethodDefinition)theDefinition).get_HasOverrides();
			}
			if (theDefinition as PropertyDefinition != null)
			{
				return ((PropertyDefinition)theDefinition).IsExplicitImplementation();
			}
			if (theDefinition as EventDefinition == null)
			{
				return false;
			}
			return ((EventDefinition)theDefinition).IsExplicitImplementation();
		}

		public static bool IsInitializerPresent(InitializerExpression initializer)
		{
			if (initializer == null || initializer.get_Expression() == null)
			{
				return false;
			}
			return initializer.get_Expression().get_Expressions().get_Count() > 0;
		}

		public static bool IsTypeNameInCollisionOnAssemblyLevel(string typeName, AssemblySpecificContext assemblyContext, ModuleSpecificContext mainModuleContext)
		{
			V_0 = new HashSet<string>();
			V_2 = assemblyContext.get_AssemblyNamespaceUsings().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					dummyVar0 = V_0.Add(V_3);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			V_0.UnionWith(mainModuleContext.get_ModuleNamespaceUsings());
			if (mainModuleContext.get_CollisionTypesData().TryGetValue(typeName, out V_1) && V_1.Intersect<string>(V_0).Count<string>() > 1)
			{
				return true;
			}
			return false;
		}

		public static DecompiledMember TryGetDecompiledMember(MethodDefinition method, TypeSpecificContext typeContext, ILanguage language)
		{
			if (method.get_Body() == null)
			{
				return new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null);
			}
			V_0 = null;
			V_1 = method.get_Body().Decompile(language, out V_0, typeContext);
			return new DecompiledMember(Utilities.GetMemberUniqueName(method), V_1, V_0.get_MethodContext());
		}

		private class AssemblyNameReferenceEqualityComparer : IEqualityComparer<AssemblyNameReference>
		{
			public AssemblyNameReferenceEqualityComparer()
			{
				base();
				return;
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
				return String.op_Equality(x.get_FullName(), y.get_FullName());
			}

			public int GetHashCode(AssemblyNameReference obj)
			{
				return obj.get_FullName().GetHashCode();
			}
		}
	}
}