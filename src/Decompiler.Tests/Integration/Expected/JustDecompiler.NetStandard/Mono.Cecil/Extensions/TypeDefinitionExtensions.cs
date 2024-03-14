using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil.Extensions
{
	public static class TypeDefinitionExtensions
	{
		public static IEnumerable<MethodDefinition> GetAllMethodsUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers, IEnumerable<string> attributesToSkip = null)
		{
			foreach (MethodDefinition method in typeDefinition.get_Methods())
			{
				if (!showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute() || method.HasCustomAttribute(attributesToSkip))
				{
					continue;
				}
				yield return method;
			}
		}

		public static List<TypeDefinition> GetBaseTypes(this TypeDefinition targetType)
		{
			List<TypeDefinition> typeDefinitions = new List<TypeDefinition>();
			if (targetType == null)
			{
				return typeDefinitions;
			}
			typeDefinitions.Add(targetType);
			for (int i = 0; i < typeDefinitions.Count; i++)
			{
				TypeDefinition item = typeDefinitions[i];
				TypeReference baseType = item.get_BaseType();
				if (baseType != null)
				{
					TypeDefinition typeDefinition = baseType.Resolve();
					if (typeDefinition != null)
					{
						typeDefinitions.Add(typeDefinition);
					}
				}
				foreach (TypeReference @interface in item.get_Interfaces())
				{
					if (@interface == null)
					{
						continue;
					}
					TypeDefinition typeDefinition1 = @interface.Resolve();
					if (typeDefinition1 == null)
					{
						continue;
					}
					typeDefinitions.Add(typeDefinition1);
				}
			}
			return typeDefinitions;
		}

		public static Dictionary<FieldDefinition, EventDefinition> GetFieldToEventMap(this TypeDefinition typeDefinition, ILanguage language)
		{
			FieldDefinition fieldDefinition;
			Dictionary<FieldDefinition, EventDefinition> fieldDefinitions = new Dictionary<FieldDefinition, EventDefinition>();
			foreach (EventDefinition @event in typeDefinition.get_Events())
			{
				if (!(new AutoImplementedEventMatcher(@event, language)).IsAutoImplemented(out fieldDefinition))
				{
					continue;
				}
				fieldDefinitions[fieldDefinition] = @event;
			}
			return fieldDefinitions;
		}

		public static Dictionary<FieldDefinition, PropertyDefinition> GetFieldToPropertyMap(this TypeDefinition typeDefinition, ILanguage language)
		{
			FieldDefinition fieldDefinition;
			Dictionary<FieldDefinition, PropertyDefinition> fieldDefinitions = new Dictionary<FieldDefinition, PropertyDefinition>();
			foreach (PropertyDefinition property in typeDefinition.get_Properties())
			{
				if (!(new PropertyDecompiler(property, language, null)).IsAutoImplemented(out fieldDefinition))
				{
					continue;
				}
				fieldDefinitions[fieldDefinition] = property;
			}
			return fieldDefinitions;
		}

		public static IMemberDefinition GetMember(this TypeDefinition self, string fullName)
		{
			IMemberDefinition memberDefinition = (
				from m in self.get_Methods()
				where m.get_FullName() == fullName
				select m).FirstOrDefault<MethodDefinition>();
			if (memberDefinition == null && self.get_HasProperties())
			{
				memberDefinition = (
					from p in self.get_Properties()
					where p.get_FullName() == fullName
					select p).FirstOrDefault<PropertyDefinition>();
			}
			if (memberDefinition == null && self.get_HasEvents())
			{
				memberDefinition = (
					from e in self.get_Events()
					where e.get_FullName() == fullName
					select e).FirstOrDefault<EventDefinition>();
			}
			if (memberDefinition == null && self.get_HasFields())
			{
				memberDefinition = (
					from f in self.get_Fields()
					where f.get_FullName() == fullName
					select f).FirstOrDefault<FieldDefinition>();
			}
			return memberDefinition;
		}

		public static IEnumerable<IMemberDefinition> GetMembersSorted(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers, ILanguage language, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null, HashSet<FieldReference> eventFields = null, IEnumerable<MethodDefinition> generatedFilterMethods = null, IEnumerable<FieldReference> propertyFields = null)
		{
			HashSet<FieldReference> fieldReferences = new HashSet<FieldReference>();
			fieldReferences = (eventFields != null ? new HashSet<FieldReference>(eventFields) : new HashSet<FieldReference>(typeDefinition.GetFieldToEventMap(language).Keys));
			if (propertyFields != null)
			{
				fieldReferences.UnionWith(propertyFields);
			}
			else
			{
				fieldReferences.UnionWith(typeDefinition.GetFieldToPropertyMap(language).Keys);
			}
			if (typeDefinition.get_HasFields())
			{
				foreach (FieldDefinition field in typeDefinition.get_Fields())
				{
					if (!showCompilerGeneratedMembers && field.IsCompilerGenerated(true) || field.HasCustomAttribute(attributesToSkip) || fieldReferences.Contains(field) || fieldsToSkip != null && fieldsToSkip.Contains(field.get_Name()))
					{
						continue;
					}
					yield return field;
				}
			}
			if (typeDefinition.get_HasProperties())
			{
				Collection<PropertyDefinition> properties = typeDefinition.get_Properties();
				foreach (PropertyDefinition propertyDefinition in 
					from p in properties
					orderby p.get_Name()
					select p)
				{
					if (propertyDefinition.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return propertyDefinition;
				}
			}
			if (typeDefinition.get_HasMethods())
			{
				IEnumerable<MethodDefinition> methods = typeDefinition.get_Methods();
				if (generatedFilterMethods != null)
				{
					methods = methods.Concat<MethodDefinition>(generatedFilterMethods);
				}
				IEnumerable<MethodDefinition> methodDefinitions = methods;
				foreach (MethodDefinition methodDefinition in 
					from m in methodDefinitions
					orderby m.get_Name()
					select m)
				{
					if (methodDefinition.get_IsGetter() || methodDefinition.get_IsSetter() || methodDefinition.get_IsAddOn() || methodDefinition.get_IsRemoveOn() || !showCompilerGeneratedMembers && methodDefinition.HasCompilerGeneratedAttribute() || methodDefinition.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return methodDefinition;
				}
			}
			if (typeDefinition.get_HasEvents())
			{
				Collection<EventDefinition> events = typeDefinition.get_Events();
				foreach (EventDefinition eventDefinition in 
					from e in events
					orderby e.get_Name()
					select e)
				{
					if (eventDefinition.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return eventDefinition;
				}
			}
			if (typeDefinition.get_HasNestedTypes())
			{
				Collection<TypeDefinition> nestedTypes = typeDefinition.get_NestedTypes();
				foreach (TypeDefinition typeDefinition1 in 
					from t in nestedTypes
					orderby t.get_Name()
					select t)
				{
					if (!showCompilerGeneratedMembers && typeDefinition1.HasCompilerGeneratedAttribute() || typeDefinition1.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return typeDefinition1;
				}
			}
		}

		public static IEnumerable<IMemberDefinition> GetMembersToDecompile(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers = true)
		{
			foreach (PropertyDefinition property in typeDefinition.get_Properties())
			{
				yield return property;
			}
			if (typeDefinition.get_HasMethods())
			{
				foreach (MethodDefinition method in typeDefinition.get_Methods())
				{
					if (method.get_IsGetter() || method.get_IsSetter() || method.get_IsAddOn() || method.get_IsRemoveOn() || !showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return method;
				}
			}
			if (typeDefinition.get_HasEvents())
			{
				foreach (EventDefinition @event in typeDefinition.get_Events())
				{
					yield return @event;
				}
			}
			if (typeDefinition.get_HasNestedTypes())
			{
				Collection<TypeDefinition> nestedTypes = typeDefinition.get_NestedTypes();
				foreach (TypeDefinition typeDefinition1 in 
					from t in nestedTypes
					orderby t.get_Name()
					select t)
				{
					if (!showCompilerGeneratedMembers && typeDefinition1.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return typeDefinition1;
				}
			}
		}

		public static IEnumerable<IMemberDefinition> GetMembersUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			if (typeDefinition.get_HasFields())
			{
				foreach (FieldDefinition field in typeDefinition.get_Fields())
				{
					if (!showCompilerGeneratedMembers && field.IsCompilerGenerated(true))
					{
						continue;
					}
					yield return field;
				}
			}
			if (typeDefinition.get_HasProperties())
			{
				foreach (PropertyDefinition property in typeDefinition.get_Properties())
				{
					yield return property;
				}
			}
			if (typeDefinition.get_HasMethods())
			{
				foreach (MethodDefinition method in typeDefinition.get_Methods())
				{
					if (method.get_IsGetter() || method.get_IsSetter() || method.get_IsSpecialName() && (method.get_Name().StartsWith("remove_") || method.get_Name().StartsWith("add_")) || !showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return method;
				}
			}
			if (typeDefinition.get_HasEvents())
			{
				foreach (EventDefinition @event in typeDefinition.get_Events())
				{
					yield return @event;
				}
			}
		}

		public static IEnumerable<IMemberDefinition> GetMethodsEventsPropertiesUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			if (typeDefinition.get_HasProperties())
			{
				foreach (PropertyDefinition property in typeDefinition.get_Properties())
				{
					yield return property;
				}
			}
			if (typeDefinition.get_HasMethods())
			{
				foreach (MethodDefinition method in typeDefinition.get_Methods())
				{
					if (method.get_IsGetter() || method.get_IsSetter() || method.get_IsAddOn() || method.get_IsRemoveOn() || !showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return method;
				}
			}
			if (typeDefinition.get_HasEvents())
			{
				foreach (EventDefinition @event in typeDefinition.get_Events())
				{
					yield return @event;
				}
			}
		}

		public static Dictionary<MethodDefinition, PropertyDefinition> GetMethodToPropertyMap(this TypeDefinition typeDefinition)
		{
			Dictionary<MethodDefinition, PropertyDefinition> methodDefinitions = new Dictionary<MethodDefinition, PropertyDefinition>();
			foreach (PropertyDefinition property in typeDefinition.get_Properties())
			{
				if (property.get_GetMethod() != null)
				{
					methodDefinitions.Add(property.get_GetMethod(), property);
				}
				if (property.get_SetMethod() == null)
				{
					continue;
				}
				methodDefinitions.Add(property.get_SetMethod(), property);
			}
			return methodDefinitions;
		}

		internal static bool IsAnonymous(this TypeDefinition self)
		{
			bool flag;
			if (self == null || self.get_Namespace() != String.Empty || !self.get_IsSealed() || !self.get_IsNotPublic() || self.get_BaseType().get_FullName() != "System.Object" || !self.get_HasGenericParameters() || !self.HasCompilerGeneratedAttribute())
			{
				return false;
			}
			int num = 0;
			if (self.get_Interfaces().get_Count() > 1)
			{
				return false;
			}
			if (self.get_Interfaces().get_Count() == 1)
			{
				if (self.get_Interfaces().get_Item(0).get_Name() != "IEquatable`1")
				{
					return false;
				}
				num = 1;
			}
			int count = self.get_Properties().get_Count();
			if (count != self.get_GenericParameters().get_Count() || count != self.get_Fields().get_Count())
			{
				return false;
			}
			int num1 = 0;
			Collection<PropertyDefinition>.Enumerator enumerator = self.get_Properties().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PropertyDefinition current = enumerator.get_Current();
					if (current.get_GetMethod() != null)
					{
						num1 = num1 + (current.get_SetMethod() != null ? 2 : 1);
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				int count1 = self.get_Methods().get_Count();
				if (num1 + num >= count1)
				{
					return false;
				}
				return count1 <= num1 + num + 4;
			}
			finally
			{
				enumerator.Dispose();
			}
			return flag;
		}

		public static bool IsAsyncStateMachine(this TypeDefinition self)
		{
			bool flag;
			Collection<TypeReference>.Enumerator enumerator = self.get_Interfaces().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.get_Current().get_FullName() != "System.Runtime.CompilerServices.IAsyncStateMachine")
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				enumerator.Dispose();
			}
			return flag;
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
			return memberDefinition.get_BaseType().get_FullName() == typeof(MulticastDelegate).FullName;
		}

		internal static bool IsNestedIn(this TypeDefinition self, TypeDefinition typeDef)
		{
			TypeDefinition typeDefinition = null;
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (typeDef == null)
			{
				throw new ArgumentNullException("typeDef");
			}
			for (TypeDefinition i = self; i.get_IsNested(); i = typeDefinition)
			{
				typeDefinition = i.get_DeclaringType().Resolve();
				if (typeDefinition == null)
				{
					return false;
				}
				if ((object)typeDefinition == (object)typeDef)
				{
					return true;
				}
			}
			return false;
		}
	}
}