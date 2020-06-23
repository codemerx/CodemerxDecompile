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
			foreach (MethodDefinition method in typeDefinition.Methods)
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
				TypeReference baseType = item.BaseType;
				if (baseType != null)
				{
					TypeDefinition typeDefinition = baseType.Resolve();
					if (typeDefinition != null)
					{
						typeDefinitions.Add(typeDefinition);
					}
				}
				foreach (TypeReference @interface in item.Interfaces)
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
			foreach (EventDefinition @event in typeDefinition.Events)
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
			foreach (PropertyDefinition property in typeDefinition.Properties)
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
				from m in self.Methods
				where m.FullName == fullName
				select m).FirstOrDefault<MethodDefinition>();
			if (memberDefinition == null && self.HasProperties)
			{
				memberDefinition = (
					from p in self.Properties
					where p.FullName == fullName
					select p).FirstOrDefault<PropertyDefinition>();
			}
			if (memberDefinition == null && self.HasEvents)
			{
				memberDefinition = (
					from e in self.Events
					where e.FullName == fullName
					select e).FirstOrDefault<EventDefinition>();
			}
			if (memberDefinition == null && self.HasFields)
			{
				memberDefinition = (
					from f in self.Fields
					where f.FullName == fullName
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
			if (typeDefinition.HasFields)
			{
				foreach (FieldDefinition field in typeDefinition.Fields)
				{
					if (!showCompilerGeneratedMembers && field.IsCompilerGenerated(true) || field.HasCustomAttribute(attributesToSkip) || fieldReferences.Contains(field) || fieldsToSkip != null && fieldsToSkip.Contains(field.Name))
					{
						continue;
					}
					yield return field;
				}
			}
			if (typeDefinition.HasProperties)
			{
				Collection<PropertyDefinition> properties = typeDefinition.Properties;
				foreach (PropertyDefinition propertyDefinition in 
					from p in properties
					orderby p.Name
					select p)
				{
					if (propertyDefinition.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return propertyDefinition;
				}
			}
			if (typeDefinition.HasMethods)
			{
				IEnumerable<MethodDefinition> methods = typeDefinition.Methods;
				if (generatedFilterMethods != null)
				{
					methods = methods.Concat<MethodDefinition>(generatedFilterMethods);
				}
				IEnumerable<MethodDefinition> methodDefinitions = methods;
				foreach (MethodDefinition methodDefinition in 
					from m in methodDefinitions
					orderby m.Name
					select m)
				{
					if (methodDefinition.IsGetter || methodDefinition.IsSetter || methodDefinition.IsAddOn || methodDefinition.IsRemoveOn || !showCompilerGeneratedMembers && methodDefinition.HasCompilerGeneratedAttribute() || methodDefinition.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return methodDefinition;
				}
			}
			if (typeDefinition.HasEvents)
			{
				Collection<EventDefinition> events = typeDefinition.Events;
				foreach (EventDefinition eventDefinition in 
					from e in events
					orderby e.Name
					select e)
				{
					if (eventDefinition.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return eventDefinition;
				}
			}
			if (typeDefinition.HasNestedTypes)
			{
				Collection<TypeDefinition> nestedTypes = typeDefinition.NestedTypes;
				foreach (TypeDefinition typeDefinition1 in 
					from t in nestedTypes
					orderby t.Name
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
			foreach (PropertyDefinition property in typeDefinition.Properties)
			{
				yield return property;
			}
			if (typeDefinition.HasMethods)
			{
				foreach (MethodDefinition method in typeDefinition.Methods)
				{
					if (method.IsGetter || method.IsSetter || method.IsAddOn || method.IsRemoveOn || !showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return method;
				}
			}
			if (typeDefinition.HasEvents)
			{
				foreach (EventDefinition @event in typeDefinition.Events)
				{
					yield return @event;
				}
			}
			if (typeDefinition.HasNestedTypes)
			{
				Collection<TypeDefinition> nestedTypes = typeDefinition.NestedTypes;
				foreach (TypeDefinition typeDefinition1 in 
					from t in nestedTypes
					orderby t.Name
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
			if (typeDefinition.HasFields)
			{
				foreach (FieldDefinition field in typeDefinition.Fields)
				{
					if (!showCompilerGeneratedMembers && field.IsCompilerGenerated(true))
					{
						continue;
					}
					yield return field;
				}
			}
			if (typeDefinition.HasProperties)
			{
				foreach (PropertyDefinition property in typeDefinition.Properties)
				{
					yield return property;
				}
			}
			if (typeDefinition.HasMethods)
			{
				foreach (MethodDefinition method in typeDefinition.Methods)
				{
					if (method.IsGetter || method.IsSetter || method.IsSpecialName && (method.Name.StartsWith("remove_") || method.Name.StartsWith("add_")) || !showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return method;
				}
			}
			if (typeDefinition.HasEvents)
			{
				foreach (EventDefinition @event in typeDefinition.Events)
				{
					yield return @event;
				}
			}
		}

		public static IEnumerable<IMemberDefinition> GetMethodsEventsPropertiesUnordered(TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			if (typeDefinition.HasProperties)
			{
				foreach (PropertyDefinition property in typeDefinition.Properties)
				{
					yield return property;
				}
			}
			if (typeDefinition.HasMethods)
			{
				foreach (MethodDefinition method in typeDefinition.Methods)
				{
					if (method.IsGetter || method.IsSetter || method.IsAddOn || method.IsRemoveOn || !showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}
					yield return method;
				}
			}
			if (typeDefinition.HasEvents)
			{
				foreach (EventDefinition @event in typeDefinition.Events)
				{
					yield return @event;
				}
			}
		}

		public static Dictionary<MethodDefinition, PropertyDefinition> GetMethodToPropertyMap(this TypeDefinition typeDefinition)
		{
			Dictionary<MethodDefinition, PropertyDefinition> methodDefinitions = new Dictionary<MethodDefinition, PropertyDefinition>();
			foreach (PropertyDefinition property in typeDefinition.Properties)
			{
				if (property.GetMethod != null)
				{
					methodDefinitions.Add(property.GetMethod, property);
				}
				if (property.SetMethod == null)
				{
					continue;
				}
				methodDefinitions.Add(property.SetMethod, property);
			}
			return methodDefinitions;
		}

		internal static bool IsAnonymous(this TypeDefinition self)
		{
			bool flag;
			if (self == null || self.Namespace != String.Empty || !self.IsSealed || !self.IsNotPublic || self.BaseType.FullName != "System.Object" || !self.HasGenericParameters || !self.HasCompilerGeneratedAttribute())
			{
				return false;
			}
			int num = 0;
			if (self.Interfaces.Count > 1)
			{
				return false;
			}
			if (self.Interfaces.Count == 1)
			{
				if (self.Interfaces[0].Name != "IEquatable`1")
				{
					return false;
				}
				num = 1;
			}
			int count = self.Properties.Count;
			if (count != self.GenericParameters.Count || count != self.Fields.Count)
			{
				return false;
			}
			int num1 = 0;
			Collection<PropertyDefinition>.Enumerator enumerator = self.Properties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PropertyDefinition current = enumerator.Current;
					if (current.GetMethod != null)
					{
						num1 = num1 + (current.SetMethod != null ? 2 : 1);
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				int count1 = self.Methods.Count;
				if (num1 + num >= count1)
				{
					return false;
				}
				return count1 <= num1 + num + 4;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static bool IsAsyncStateMachine(this TypeDefinition self)
		{
			bool flag;
			Collection<TypeReference>.Enumerator enumerator = self.Interfaces.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.FullName != "System.Runtime.CompilerServices.IAsyncStateMachine")
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
				((IDisposable)enumerator).Dispose();
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
			if (memberDefinition.BaseType == null)
			{
				return false;
			}
			return memberDefinition.BaseType.FullName == typeof(MulticastDelegate).FullName;
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
			for (TypeDefinition i = self; i.IsNested; i = typeDefinition)
			{
				typeDefinition = i.DeclaringType.Resolve();
				if (typeDefinition == null)
				{
					return false;
				}
				if (typeDefinition == typeDef)
				{
					return true;
				}
			}
			return false;
		}
	}
}