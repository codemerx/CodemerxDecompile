using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil.Extensions
{
	public static class TypeDefinitionExtensions
	{
		public static IEnumerable<IMemberDefinition> GetMembersUnordered(this TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			if (typeDefinition.HasFields)
			{
				foreach (var field in typeDefinition.Fields)
				{
					if (!showCompilerGeneratedMembers && field.IsCompilerGenerated())
						continue;

					yield return field;
				}
			}
			if (typeDefinition.HasProperties)
			{
				foreach (var property in typeDefinition.Properties)
				{
					yield return property;
				}
			}
			if (typeDefinition.HasMethods)
			{
				foreach (var method in typeDefinition.Methods)
				{
					if (method.IsGetter || method.IsSetter)
						continue;

					if (method.IsSpecialName && (method.Name.StartsWith("remove_") || method.Name.StartsWith("add_")))
						continue;

					if (!showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
						continue;

					yield return method;
				}
			}
			if (typeDefinition.HasEvents)
			{
				foreach (var @event in typeDefinition.Events)
				{
					yield return @event;
				}
			}
		}

		public static IEnumerable<IMemberDefinition> GetMethodsEventsPropertiesUnordered(this TypeDefinition typeDefinition, bool showCompilerGeneratedMembers)
		{
			if (typeDefinition.HasProperties)
			{
				foreach (var property in typeDefinition.Properties)
				{
					yield return property;
				}
			}
			if (typeDefinition.HasMethods)
			{
				foreach (var method in typeDefinition.Methods)
				{
					if (method.IsGetter || method.IsSetter)
						continue;

					if (method.IsAddOn || method.IsRemoveOn)
						continue;

					if (!showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
						continue;

					yield return method;
				}
			}
			if (typeDefinition.HasEvents)
			{
				foreach (var @event in typeDefinition.Events)
				{
					yield return @event;
				}
			}
		}

		public static IEnumerable<MethodDefinition> GetAllMethodsUnordered(this TypeDefinition typeDefinition, bool showCompilerGeneratedMembers, IEnumerable<string> attributesToSkip = null)
        {
            foreach (MethodDefinition method in typeDefinition.Methods)
            {

                if (!showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
                    continue;

				if (method.HasCustomAttribute(attributesToSkip))
					continue;

                yield return method;
            }
        }

		public static IEnumerable<IMemberDefinition> GetMembersToDecompile(this TypeDefinition typeDefinition, bool showCompilerGeneratedMembers = true)
		{
            foreach (var property in typeDefinition.Properties)
            {
                yield return property;
            }

            if (typeDefinition.HasMethods)
			{
				foreach (var method in typeDefinition.Methods)
				{
					if (method.IsGetter || method.IsSetter)
					{
						continue;
					}

					if (method.IsAddOn || method.IsRemoveOn)
					{
						continue;
					}

					if (!showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute())
					{
						continue;
					}

					yield return method;
				}
			}

			if (typeDefinition.HasEvents)
			{
				foreach (var @event in typeDefinition.Events)
				{
					yield return @event;
				}
			}

			if (typeDefinition.HasNestedTypes)
			{
				foreach (TypeDefinition nestedType in typeDefinition.NestedTypes.OrderBy(t => t.Name))
				{
					if (!showCompilerGeneratedMembers && nestedType.HasCompilerGeneratedAttribute())
					{
						continue;
					}

					yield return nestedType;
				}
			}
		}

		public static Dictionary<MethodDefinition, PropertyDefinition> GetMethodToPropertyMap(this TypeDefinition typeDefinition)
		{
			Dictionary<MethodDefinition, PropertyDefinition> result = new Dictionary<MethodDefinition, PropertyDefinition>();
			foreach (PropertyDefinition property in typeDefinition.Properties)
			{
				if (property.GetMethod != null)
				{
					result.Add(property.GetMethod, property);
				}

				if (property.SetMethod != null)
				{
					result.Add(property.SetMethod, property);
				}
			}

			return result;
		}

        public static Dictionary<FieldDefinition, EventDefinition> GetFieldToEventMap(this TypeDefinition typeDefinition, ILanguage language)
        {
            Dictionary<FieldDefinition, EventDefinition> result = new Dictionary<FieldDefinition, EventDefinition>();
            foreach (EventDefinition @event in typeDefinition.Events)
            {
                FieldDefinition eventField;
                AutoImplementedEventMatcher matcher = new AutoImplementedEventMatcher(@event, language);
                if (matcher.IsAutoImplemented(out eventField))
                {
                    result[eventField] = @event;
                }
            }

            return result;
        }

        public static Dictionary<FieldDefinition, PropertyDefinition> GetFieldToPropertyMap(this TypeDefinition typeDefinition, ILanguage language)
        {
            Dictionary<FieldDefinition, PropertyDefinition> result = new Dictionary<FieldDefinition, PropertyDefinition>();
            foreach (PropertyDefinition property in typeDefinition.Properties)
            {
                FieldDefinition propertyField;
                PropertyDecompiler matcher = new PropertyDecompiler(property, language);
                if (matcher.IsAutoImplemented(out propertyField))
                {
                    result[propertyField] = property;
                }
            }

            return result;
        }

		public static IEnumerable<IMemberDefinition> GetMembersSorted(this TypeDefinition typeDefinition, bool showCompilerGeneratedMembers,
            ILanguage language, IEnumerable<string> attributesToSkip = null, ICollection<string> fieldsToSkip = null,
            HashSet<FieldReference> eventFields = null, IEnumerable<MethodDefinition> generatedFilterMethods = null,
            IEnumerable<FieldReference> propertyFields = null)
		{
            HashSet<FieldReference> backingFields = new HashSet<FieldReference>();
            if (eventFields == null)
            {
#if !NET35
                backingFields = new HashSet<FieldReference>(typeDefinition.GetFieldToEventMap(language).Keys);
#else
                IEnumerable<FieldDefinition> fields = typeDefinition.GetFieldToEventMap().Keys;
                backingFields = new HashSet<FieldReference>();

                foreach(FieldDefinition fdef in fields)
                {
                    backingFields.Add(fdef);
                }
#endif
            }
            else
            {
                backingFields = new HashSet<FieldReference>(eventFields);
            }
#if !NET35
            if (propertyFields == null)
            {
                backingFields.UnionWith(typeDefinition.GetFieldToPropertyMap(language).Keys);
            }
            else
            {
                backingFields.UnionWith(propertyFields);
            }
#else
            IEnumerable<FieldDefinition> properties;
            if (propertyFields == null)
            {
                properties = typeDefinition.GetFieldToPropertyMap(language).Keys;
	        }
            else
            {
                properties = propertyFields;
            }

            foreach (FieldDefinition prop in properties)
            {
                backingFields.Add(prop);
            }
#endif

            if (typeDefinition.HasFields)
			{
				foreach (FieldDefinition field in typeDefinition.Fields)//.OrderBy(f => f.Name))
				{
					if (!showCompilerGeneratedMembers && field.IsCompilerGenerated() || field.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}

                    if (backingFields.Contains(field))
                    {
                        continue;
                    }

					if (fieldsToSkip != null)
					{
						if (fieldsToSkip.Contains(field.Name))
						{
							continue;
						}
					}

					yield return field;
				}
			}
			if (typeDefinition.HasProperties)
			{
				foreach (PropertyDefinition property in typeDefinition.Properties.OrderBy(p => p.Name))
				{
					if (property.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return property;
				}
			}
			if (typeDefinition.HasMethods)
			{
                IEnumerable<MethodDefinition> methods = typeDefinition.Methods;
                if (generatedFilterMethods != null)
                {
                    methods = methods.Concat(generatedFilterMethods);
                }

				foreach (MethodDefinition method in methods.OrderBy(m => m.Name))
				{
					if (method.IsGetter || method.IsSetter)
					{
						continue;
					}

                    if (method.IsAddOn || method.IsRemoveOn)
                    {
                        continue;
                    }

					if (!showCompilerGeneratedMembers && method.HasCompilerGeneratedAttribute() || method.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}

					yield return method;
				}
			}
			if (typeDefinition.HasEvents)
			{
				foreach (EventDefinition @event in typeDefinition.Events.OrderBy(e => e.Name))
				{
					if (@event.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}
					yield return @event;
				}
			}
			if (typeDefinition.HasNestedTypes)
			{
				foreach (TypeDefinition nestedType in typeDefinition.NestedTypes.OrderBy(t => t.Name))
				{
					if (!showCompilerGeneratedMembers && nestedType.HasCompilerGeneratedAttribute() || nestedType.HasCustomAttribute(attributesToSkip))
					{
						continue;
					}

					yield return nestedType;
				}
			}
		}

        public static bool IsDelegate(this TypeDefinition memberDefinition)
        {
            return memberDefinition.BaseType != null && memberDefinition.BaseType.FullName == typeof(MulticastDelegate).FullName;
        }

        public static bool IsCompilerGenerated(this TypeDefinition source)
        {
            if (source == null)
            {
                return false;
            }
            return source.HasCompilerGeneratedAttribute();
        }


        /// <summary>
        /// Generates a collection of all base classes and interfaces of <paramref name="targetType"/>, including <paramref name="targetType"/> itself.
        /// </summary>
        /// <param name="targetType">The type that is the bottom of the inheritance chain.</param>
        /// <returns>Returns the generated collection.</returns>
        public static List<TypeDefinition> GetBaseTypes(this TypeDefinition targetType)
        {
            List<TypeDefinition> allParents = new List<TypeDefinition>();

            if (targetType == null)
            {
                return allParents;
            }

            /// Add the lowest type.
            /// It has no inheritors in the chain.
            allParents.Add(targetType);

            for (int i = 0; i < allParents.Count; i++)
            {
                TypeDefinition currentType = allParents[i];
                TypeReference baseType = currentType.BaseType;
                if (baseType != null)
                {
                    TypeDefinition baseDef = baseType.Resolve();
                    /// Add baseDef to the inheritance chains
                    /// mark class i as the decendant it came from
                    if (baseDef != null)
                    {
                        allParents.Add(baseDef);
                    }
                }
                foreach (TypeReference @interface in currentType.Interfaces)
                {
                    if (@interface != null)
                    {
                        TypeDefinition interfaceDef = @interface.Resolve();
                        if (interfaceDef != null)
                        {
                            allParents.Add(interfaceDef);
                        }
                    }
                }
            }
            return allParents;
        }

        internal static bool IsAnonymous(this TypeDefinition self)
        {
            if (self == null || self.Namespace != string.Empty || !self.IsSealed || !self.IsNotPublic ||
                self.BaseType.FullName != "System.Object" || !self.HasGenericParameters ||
                !self.HasCompilerGeneratedAttribute())
            {
                return false;
            }

            int implementsEquatable = 0;
            if (self.Interfaces.Count > 1)
            {
                return false;
            }
            else if (self.Interfaces.Count == 1)
            {
                if (self.Interfaces[0].Name == "IEquatable`1")
                {
                    implementsEquatable = 1;
                }
                else
                {
                    return false;
                }
            }

            int propertyCount = self.Properties.Count;
            if (propertyCount != self.GenericParameters.Count || propertyCount != self.Fields.Count)
            {
                return false;
            }

            int propertyMethodsCount = 0;
            foreach (PropertyDefinition property in self.Properties)
            {
                if (property.GetMethod == null)
                {
                    return false;
                }

                propertyMethodsCount += property.SetMethod != null ? 2 : 1;
            }

            int methodsCount = self.Methods.Count;

            return propertyMethodsCount + implementsEquatable < methodsCount &&     //ctor atleast
                    methodsCount <= propertyMethodsCount + implementsEquatable + 4; //ctor + Equals + GetHashCode + ToString
        }

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

            TypeDefinition current = self;
            while (current.IsNested)
            {
                TypeDefinition enclosingTypeDef = current.DeclaringType.Resolve();
                if (enclosingTypeDef == null)
                {
                    return false;
                }

                if (enclosingTypeDef == typeDef)
                {
                    return true;
                }

                current = enclosingTypeDef;
            }

            return false;
        }

        public static IMemberDefinition GetMember(this TypeDefinition self, string fullName)
        {
            //type = assembly.MainModule.Types.Where(t => t.FullName == TypeName).FirstOrDefault();
            IMemberDefinition member = self.Methods.Where(m => m.FullName == fullName).FirstOrDefault();

            //TryShowInstructionsForMember(method);
            if (member == null && self.HasProperties)
            {
                member = self.Properties.Where(p => p.FullName == fullName).FirstOrDefault();
            }

            if (member == null && self.HasEvents)
            {
                member = self.Events.Where(e => e.FullName == fullName).FirstOrDefault();
            }

            if (member == null && self.HasFields)
            {
                member = self.Fields.Where(f => f.FullName == fullName).FirstOrDefault();
            }
            return member;
        }

        /// <summary>
        /// Determines if the type is async state machine.
        /// </summary>
        /// <remarks>
        /// Since C# 6.0 the async state machine type is not nessesary to be struct (value type). In some cases the C# 6.0 compiler
        /// generates struct, in others - class.
        /// </remarks>
        /// <param name="self"></param>
        /// <returns></returns>
		public static bool IsAsyncStateMachine(this TypeDefinition self)
		{
			foreach (TypeReference @interface in self.Interfaces)
			{
				if (@interface.FullName == "System.Runtime.CompilerServices.IAsyncStateMachine")
				{
					return true;
				}
			}

			return false;
		}
	}
}