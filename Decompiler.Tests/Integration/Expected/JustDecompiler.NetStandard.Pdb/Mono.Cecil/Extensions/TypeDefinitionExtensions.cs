using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil.Extensions
{
	public static class TypeDefinitionExtensions
	{
		public static IEnumerable<MethodDefinition> GetAllMethodsUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers, IEnumerable<string> attributesToSkip = null)
		{
			stackVariable1 = new TypeDefinitionExtensions.u003cGetAllMethodsUnorderedu003ed__2(-2);
			stackVariable1.u003cu003e3__typeDefinition = typeDefinition;
			stackVariable1.u003cu003e3__showCompilerGeneratedMembers = showCompilerGeneratedMembers;
			stackVariable1.u003cu003e3__attributesToSkip = attributesToSkip;
			return stackVariable1;
		}

		public static List<TypeDefinition> GetBaseTypes(this TypeDefinition targetType)
		{
			V_0 = new List<TypeDefinition>();
			if (targetType == null)
			{
				return V_0;
			}
			V_0.Add(targetType);
			V_1 = 0;
			while (V_1 < V_0.get_Count())
			{
				stackVariable10 = V_0.get_Item(V_1);
				V_2 = stackVariable10.get_BaseType();
				if (V_2 != null)
				{
					V_3 = V_2.Resolve();
					if (V_3 != null)
					{
						V_0.Add(V_3);
					}
				}
				V_4 = stackVariable10.get_Interfaces().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (V_5 == null)
						{
							continue;
						}
						V_6 = V_5.Resolve();
						if (V_6 == null)
						{
							continue;
						}
						V_0.Add(V_6);
					}
				}
				finally
				{
					V_4.Dispose();
				}
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		public static Dictionary<FieldDefinition, EventDefinition> GetFieldToEventMap(this TypeDefinition typeDefinition, ILanguage language)
		{
			V_0 = new Dictionary<FieldDefinition, EventDefinition>();
			V_1 = typeDefinition.get_Events().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!(new AutoImplementedEventMatcher(V_2, language)).IsAutoImplemented(out V_3))
					{
						continue;
					}
					V_0.set_Item(V_3, V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		public static Dictionary<FieldDefinition, PropertyDefinition> GetFieldToPropertyMap(this TypeDefinition typeDefinition, ILanguage language)
		{
			V_0 = new Dictionary<FieldDefinition, PropertyDefinition>();
			V_1 = typeDefinition.get_Properties().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!(new PropertyDecompiler(V_2, language, null)).IsAutoImplemented(out V_3))
					{
						continue;
					}
					V_0.set_Item(V_3, V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		public static IMemberDefinition GetMember(this TypeDefinition self, string fullName)
		{
			V_0 = new TypeDefinitionExtensions.u003cu003ec__DisplayClass13_0();
			V_0.fullName = fullName;
			V_1 = self.get_Methods().Where<MethodDefinition>(new Func<MethodDefinition, bool>(V_0.u003cGetMemberu003eb__0)).FirstOrDefault<MethodDefinition>();
			if (V_1 == null && self.get_HasProperties())
			{
				V_1 = self.get_Properties().Where<PropertyDefinition>(new Func<PropertyDefinition, bool>(V_0.u003cGetMemberu003eb__1)).FirstOrDefault<PropertyDefinition>();
			}
			if (V_1 == null && self.get_HasEvents())
			{
				V_1 = self.get_Events().Where<EventDefinition>(new Func<EventDefinition, bool>(V_0.u003cGetMemberu003eb__2)).FirstOrDefault<EventDefinition>();
			}
			if (V_1 == null && self.get_HasFields())
			{
				V_1 = self.get_Fields().Where<FieldDefinition>(new Func<FieldDefinition, bool>(V_0.u003cGetMemberu003eb__3)).FirstOrDefault<FieldDefinition>();
			}
			return V_1;
		}

		public static IEnumerable<IMemberDefinition> GetMembersSorted(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers, ILanguage language, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null, HashSet<FieldReference> eventFields = null, IEnumerable<MethodDefinition> generatedFilterMethods = null, IEnumerable<FieldReference> propertyFields = null)
		{
			stackVariable1 = new TypeDefinitionExtensions.u003cGetMembersSortedu003ed__7(-2);
			stackVariable1.u003cu003e3__typeDefinition = typeDefinition;
			stackVariable1.u003cu003e3__showCompilerGeneratedMembers = showCompilerGeneratedMembers;
			stackVariable1.u003cu003e3__language = language;
			stackVariable1.u003cu003e3__attributesToSkip = attributesToSkip;
			stackVariable1.u003cu003e3__fieldsToSkip = fieldsToSkip;
			stackVariable1.u003cu003e3__eventFields = eventFields;
			stackVariable1.u003cu003e3__generatedFilterMethods = generatedFilterMethods;
			stackVariable1.u003cu003e3__propertyFields = propertyFields;
			return stackVariable1;
		}

		public static IEnumerable<IMemberDefinition> GetMembersToDecompile(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers = true)
		{
			stackVariable1 = new TypeDefinitionExtensions.u003cGetMembersToDecompileu003ed__3(-2);
			stackVariable1.u003cu003e3__typeDefinition = typeDefinition;
			stackVariable1.u003cu003e3__showCompilerGeneratedMembers = showCompilerGeneratedMembers;
			return stackVariable1;
		}

		public static IEnumerable<IMemberDefinition> GetMembersUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			stackVariable1 = new TypeDefinitionExtensions.u003cGetMembersUnorderedu003ed__0(-2);
			stackVariable1.u003cu003e3__typeDefinition = typeDefinition;
			stackVariable1.u003cu003e3__showCompilerGeneratedMembers = showCompilerGeneratedMembers;
			return stackVariable1;
		}

		public static IEnumerable<IMemberDefinition> GetMethodsEventsPropertiesUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			stackVariable1 = new TypeDefinitionExtensions.u003cGetMethodsEventsPropertiesUnorderedu003ed__1(-2);
			stackVariable1.u003cu003e3__typeDefinition = typeDefinition;
			stackVariable1.u003cu003e3__showCompilerGeneratedMembers = showCompilerGeneratedMembers;
			return stackVariable1;
		}

		public static Dictionary<MethodDefinition, PropertyDefinition> GetMethodToPropertyMap(this TypeDefinition typeDefinition)
		{
			V_0 = new Dictionary<MethodDefinition, PropertyDefinition>();
			V_1 = typeDefinition.get_Properties().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_GetMethod() != null)
					{
						V_0.Add(V_2.get_GetMethod(), V_2);
					}
					if (V_2.get_SetMethod() == null)
					{
						continue;
					}
					V_0.Add(V_2.get_SetMethod(), V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		internal static bool IsAnonymous(this TypeDefinition self)
		{
			if (self == null || String.op_Inequality(self.get_Namespace(), String.Empty) || !self.get_IsSealed() || !self.get_IsNotPublic() || String.op_Inequality(self.get_BaseType().get_FullName(), "System.Object") || !self.get_HasGenericParameters() || !self.HasCompilerGeneratedAttribute())
			{
				return false;
			}
			V_0 = 0;
			if (self.get_Interfaces().get_Count() > 1)
			{
				return false;
			}
			if (self.get_Interfaces().get_Count() == 1)
			{
				if (!String.op_Equality(self.get_Interfaces().get_Item(0).get_Name(), "IEquatable`1"))
				{
					return false;
				}
				V_0 = 1;
			}
			V_1 = self.get_Properties().get_Count();
			if (V_1 != self.get_GenericParameters().get_Count() || V_1 != self.get_Fields().get_Count())
			{
				return false;
			}
			V_2 = 0;
			V_4 = self.get_Properties().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					if (V_5.get_GetMethod() != null)
					{
						stackVariable50 = V_2;
						if (V_5.get_SetMethod() != null)
						{
							stackVariable53 = 2;
						}
						else
						{
							stackVariable53 = 1;
						}
						V_2 = stackVariable50 + stackVariable53;
					}
					else
					{
						V_6 = false;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				V_4.Dispose();
			}
		Label1:
			return V_6;
		Label0:
			V_3 = self.get_Methods().get_Count();
			if (V_2 + V_0 >= V_3)
			{
				return false;
			}
			return V_3 <= V_2 + V_0 + 4;
		}

		public static bool IsAsyncStateMachine(this TypeDefinition self)
		{
			V_0 = self.get_Interfaces().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (!String.op_Equality(V_0.get_Current().get_FullName(), "System.Runtime.CompilerServices.IAsyncStateMachine"))
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}

		public static bool IsCompilerGenerated(this TypeDefinition source)
		{
			if (source == null)
			{
				return false;
			}
			return source.HasCompilerGeneratedAttribute();
		}

		public static bool IsDelegate(this TypeDefinition memberDefinition)
		{
			if (memberDefinition.get_BaseType() == null)
			{
				return false;
			}
			return String.op_Equality(memberDefinition.get_BaseType().get_FullName(), Type.GetTypeFromHandle(// 
			// Current member / type: System.Boolean Mono.Cecil.Extensions.TypeDefinitionExtensions::IsDelegate(Mono.Cecil.TypeDefinition)
			// Exception in: System.Boolean IsDelegate(Mono.Cecil.TypeDefinition)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal static bool IsNestedIn(this TypeDefinition self, TypeDefinition typeDef)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (typeDef == null)
			{
				throw new ArgumentNullException("typeDef");
			}
			V_0 = self;
			while (V_0.get_IsNested())
			{
				V_1 = V_0.get_DeclaringType().Resolve();
				if (V_1 == null)
				{
					return false;
				}
				if ((object)V_1 == (object)typeDef)
				{
					return true;
				}
				V_0 = V_1;
			}
			return false;
		}
	}
}