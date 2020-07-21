using Mono.Cecil;
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
				stackVariable1 = this.methodToPropertyMap;
				if (stackVariable1 == null)
				{
					dummyVar0 = stackVariable1;
					stackVariable5 = this.get_CurrentType().GetMethodToPropertyMap();
					V_0 = stackVariable5;
					this.methodToPropertyMap = stackVariable5;
					stackVariable1 = V_0;
				}
				return stackVariable1;
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
			base();
			this.set_CurrentType(currentType);
			this.set_MethodDefinitionToNameMap(methodDefinitionToNameMap);
			this.set_BackingFieldToNameMap(backingFieldToNameMap);
			this.set_UsedNamespaces(usedNamespaces);
			this.set_VisibleMembersNames(visibleMembersNames);
			this.set_AssignmentData(fieldToAssignedExpression);
			this.set_BaseCtorInvocators(new HashSet<MethodDefinition>());
			this.set_FieldInitializationFailed(false);
			this.set_AutoImplementedProperties(autoImplementedProperties);
			this.set_AutoImplementedEvents(autoImplementedEvents);
			this.set_ExplicitlyImplementedMembers(explicitlyImplementedMembers);
			this.set_ExceptionWhileDecompiling(exceptionsWhileDecompiling);
			this.set_GeneratedFilterMethods(generatedFilterMethods);
			this.set_GeneratedMethodDefinitionToNameMap(generatedMethodDefinitionToNameMap);
			return;
		}

		public TypeSpecificContext(TypeDefinition currentType)
		{
			base();
			this.set_CurrentType(currentType);
			this.set_MethodDefinitionToNameMap(new Dictionary<MethodDefinition, string>());
			this.set_BackingFieldToNameMap(new Dictionary<FieldDefinition, string>());
			this.set_UsedNamespaces(new HashSet<string>());
			this.set_VisibleMembersNames(new HashSet<string>());
			this.set_AssignmentData(new Dictionary<string, InitializationAssignment>());
			this.set_BaseCtorInvocators(new HashSet<MethodDefinition>());
			this.set_FieldInitializationFailed(false);
			this.set_AutoImplementedProperties(new HashSet<PropertyDefinition>());
			this.set_AutoImplementedEvents(new HashSet<EventDefinition>());
			this.set_ExplicitlyImplementedMembers(new ExplicitlyImplementedMembersCollection());
			this.set_ExceptionWhileDecompiling(new List<MethodDefinition>());
			this.set_GeneratedFilterMethods(new List<GeneratedMethod>());
			this.set_GeneratedMethodDefinitionToNameMap(new Dictionary<MethodDefinition, string>());
			return;
		}

		private TypeSpecificContext()
		{
			base();
			return;
		}

		public Dictionary<FieldDefinition, EventDefinition> GetFieldToEventMap(ILanguage language)
		{
			if (this.fieldToEventMap == null)
			{
				this.fieldToEventMap = this.get_CurrentType().GetFieldToEventMap(language);
			}
			return this.fieldToEventMap;
		}

		public Dictionary<FieldDefinition, PropertyDefinition> GetFieldToPropertyMap(ILanguage language)
		{
			if (this.fieldToPropertyMap == null)
			{
				this.fieldToPropertyMap = this.get_CurrentType().GetFieldToPropertyMap(language);
			}
			return this.fieldToPropertyMap;
		}

		public TypeSpecificContext ShallowPartialClone()
		{
			stackVariable0 = new TypeSpecificContext();
			stackVariable0.set_CurrentType(this.get_CurrentType());
			stackVariable0.set_MethodDefinitionToNameMap(this.get_MethodDefinitionToNameMap());
			stackVariable0.set_BackingFieldToNameMap(this.get_BackingFieldToNameMap());
			stackVariable0.set_UsedNamespaces(this.get_UsedNamespaces());
			stackVariable0.set_VisibleMembersNames(this.get_VisibleMembersNames());
			stackVariable0.fieldToEventMap = this.fieldToEventMap;
			stackVariable0.methodToPropertyMap = this.get_MethodToPropertyMap();
			stackVariable0.set_IsWinRTImplementation(this.get_IsWinRTImplementation());
			stackVariable0.fieldToPropertyMap = this.fieldToPropertyMap;
			stackVariable0.set_GeneratedFilterMethods(this.get_GeneratedFilterMethods());
			stackVariable0.set_GeneratedMethodDefinitionToNameMap(this.get_GeneratedMethodDefinitionToNameMap());
			stackVariable0.set_AssignmentData(new Dictionary<string, InitializationAssignment>());
			stackVariable0.set_BaseCtorInvocators(new HashSet<MethodDefinition>());
			stackVariable0.set_FieldInitializationFailed(false);
			return stackVariable0;
		}
	}
}