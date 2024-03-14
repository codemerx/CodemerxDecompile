using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public class TypeSpecificContext
	{
		private Dictionary<FieldDefinition, EventDefinition> fieldToEventMap;

		private Dictionary<MethodDefinition, PropertyDefinition> methodToPropertyMap;

		private Dictionary<FieldDefinition, PropertyDefinition> fieldToPropertyMap;

		public Dictionary<string, InitializationAssignment> AssignmentData
		{
			get;
			set;
		}

		public HashSet<EventDefinition> AutoImplementedEvents
		{
			get;
			set;
		}

		public HashSet<PropertyDefinition> AutoImplementedProperties
		{
			get;
			set;
		}

		public Dictionary<FieldDefinition, string> BackingFieldToNameMap
		{
			get;
			set;
		}

		public ICollection<MethodDefinition> BaseCtorInvocators
		{
			get;
			private set;
		}

		public TypeDefinition CurrentType
		{
			get;
			private set;
		}

		public ICollection<MethodDefinition> ExceptionWhileDecompiling
		{
			get;
			private set;
		}

		public ExplicitlyImplementedMembersCollection ExplicitlyImplementedMembers
		{
			get;
			set;
		}

		public bool FieldInitializationFailed
		{
			get;
			set;
		}

		public IList<GeneratedMethod> GeneratedFilterMethods
		{
			get;
			set;
		}

		public IDictionary<MethodDefinition, string> GeneratedMethodDefinitionToNameMap
		{
			get;
			set;
		}

		public bool IsWinRTImplementation
		{
			get;
			set;
		}

		public Dictionary<MethodDefinition, string> MethodDefinitionToNameMap
		{
			get;
			set;
		}

		public Dictionary<MethodDefinition, PropertyDefinition> MethodToPropertyMap
		{
			get
			{
				Dictionary<MethodDefinition, PropertyDefinition> methodDefinitions = this.methodToPropertyMap;
				if (methodDefinitions == null)
				{
					Dictionary<MethodDefinition, PropertyDefinition> methodToPropertyMap = this.CurrentType.GetMethodToPropertyMap();
					Dictionary<MethodDefinition, PropertyDefinition> methodDefinitions1 = methodToPropertyMap;
					this.methodToPropertyMap = methodToPropertyMap;
					methodDefinitions = methodDefinitions1;
				}
				return methodDefinitions;
			}
		}

		public ICollection<string> UsedNamespaces
		{
			get;
			private set;
		}

		public ICollection<string> VisibleMembersNames
		{
			get;
			private set;
		}

		public TypeSpecificContext(TypeDefinition currentType, Dictionary<MethodDefinition, string> methodDefinitionToNameMap, Dictionary<FieldDefinition, string> backingFieldToNameMap, ICollection<string> usedNamespaces, ICollection<string> visibleMembersNames, Dictionary<string, InitializationAssignment> fieldToAssignedExpression, HashSet<PropertyDefinition> autoImplementedProperties, HashSet<EventDefinition> autoImplementedEvents, ExplicitlyImplementedMembersCollection explicitlyImplementedMembers, ICollection<MethodDefinition> exceptionsWhileDecompiling, IList<GeneratedMethod> generatedFilterMethods, IDictionary<MethodDefinition, string> generatedMethodDefinitionToNameMap)
		{
			this.CurrentType = currentType;
			this.MethodDefinitionToNameMap = methodDefinitionToNameMap;
			this.BackingFieldToNameMap = backingFieldToNameMap;
			this.UsedNamespaces = usedNamespaces;
			this.VisibleMembersNames = visibleMembersNames;
			this.AssignmentData = fieldToAssignedExpression;
			this.BaseCtorInvocators = new HashSet<MethodDefinition>();
			this.FieldInitializationFailed = false;
			this.AutoImplementedProperties = autoImplementedProperties;
			this.AutoImplementedEvents = autoImplementedEvents;
			this.ExplicitlyImplementedMembers = explicitlyImplementedMembers;
			this.ExceptionWhileDecompiling = exceptionsWhileDecompiling;
			this.GeneratedFilterMethods = generatedFilterMethods;
			this.GeneratedMethodDefinitionToNameMap = generatedMethodDefinitionToNameMap;
		}

		public TypeSpecificContext(TypeDefinition currentType)
		{
			this.CurrentType = currentType;
			this.MethodDefinitionToNameMap = new Dictionary<MethodDefinition, string>();
			this.BackingFieldToNameMap = new Dictionary<FieldDefinition, string>();
			this.UsedNamespaces = new HashSet<string>();
			this.VisibleMembersNames = new HashSet<string>();
			this.AssignmentData = new Dictionary<string, InitializationAssignment>();
			this.BaseCtorInvocators = new HashSet<MethodDefinition>();
			this.FieldInitializationFailed = false;
			this.AutoImplementedProperties = new HashSet<PropertyDefinition>();
			this.AutoImplementedEvents = new HashSet<EventDefinition>();
			this.ExplicitlyImplementedMembers = new ExplicitlyImplementedMembersCollection();
			this.ExceptionWhileDecompiling = new List<MethodDefinition>();
			this.GeneratedFilterMethods = new List<GeneratedMethod>();
			this.GeneratedMethodDefinitionToNameMap = new Dictionary<MethodDefinition, string>();
		}

		private TypeSpecificContext()
		{
		}

		public Dictionary<FieldDefinition, EventDefinition> GetFieldToEventMap(ILanguage language)
		{
			if (this.fieldToEventMap == null)
			{
				this.fieldToEventMap = this.CurrentType.GetFieldToEventMap(language);
			}
			return this.fieldToEventMap;
		}

		public Dictionary<FieldDefinition, PropertyDefinition> GetFieldToPropertyMap(ILanguage language)
		{
			if (this.fieldToPropertyMap == null)
			{
				this.fieldToPropertyMap = this.CurrentType.GetFieldToPropertyMap(language);
			}
			return this.fieldToPropertyMap;
		}

		public TypeSpecificContext ShallowPartialClone()
		{
			return new TypeSpecificContext()
			{
				CurrentType = this.CurrentType,
				MethodDefinitionToNameMap = this.MethodDefinitionToNameMap,
				BackingFieldToNameMap = this.BackingFieldToNameMap,
				UsedNamespaces = this.UsedNamespaces,
				VisibleMembersNames = this.VisibleMembersNames,
				fieldToEventMap = this.fieldToEventMap,
				methodToPropertyMap = this.MethodToPropertyMap,
				IsWinRTImplementation = this.IsWinRTImplementation,
				fieldToPropertyMap = this.fieldToPropertyMap,
				GeneratedFilterMethods = this.GeneratedFilterMethods,
				GeneratedMethodDefinitionToNameMap = this.GeneratedMethodDefinitionToNameMap,
				AssignmentData = new Dictionary<string, InitializationAssignment>(),
				BaseCtorInvocators = new HashSet<MethodDefinition>(),
				FieldInitializationFailed = false
			};
		}
	}
}