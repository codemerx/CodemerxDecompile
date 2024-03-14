using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
    public class TypeSpecificContext
    {
        private Dictionary<FieldDefinition, EventDefinition> fieldToEventMap;
        public Dictionary<FieldDefinition, EventDefinition> GetFieldToEventMap(ILanguage language)
        {
            if (fieldToEventMap == null)
            {
                fieldToEventMap = this.CurrentType.GetFieldToEventMap(language);
            }

            return fieldToEventMap;
        }

		private Dictionary<MethodDefinition, PropertyDefinition> methodToPropertyMap;
		public Dictionary<MethodDefinition, PropertyDefinition> MethodToPropertyMap
		{
			get
			{
				return methodToPropertyMap ?? (methodToPropertyMap = this.CurrentType.GetMethodToPropertyMap());
			}
		}

        private Dictionary<FieldDefinition, PropertyDefinition> fieldToPropertyMap;
        public Dictionary<FieldDefinition, PropertyDefinition> GetFieldToPropertyMap(ILanguage language)
        {
            if (fieldToPropertyMap == null)
            {
                fieldToPropertyMap = this.CurrentType.GetFieldToPropertyMap(language);
            }

            return fieldToPropertyMap;
        }

		public Dictionary<string, InitializationAssignment> AssignmentData { get; set; }

        public TypeDefinition CurrentType { get; private set; }
		public Dictionary<MethodDefinition, string> MethodDefinitionToNameMap { get; set; }
		public Dictionary<FieldDefinition, string> BackingFieldToNameMap { get; set; }
		public ICollection<string> UsedNamespaces { get; private set; }
		public ICollection<string> VisibleMembersNames { get; private set; }
		/// Contains all of the type's constructors, that invoke base constructors.
		public ICollection<MethodDefinition> BaseCtorInvocators { get; private set; }
		public bool FieldInitializationFailed { get; set; }
		public HashSet<PropertyDefinition> AutoImplementedProperties { get; set; }
		public HashSet<EventDefinition> AutoImplementedEvents { get; set; }
		public ExplicitlyImplementedMembersCollection ExplicitlyImplementedMembers { get; set; }
		public ICollection<MethodDefinition> ExceptionWhileDecompiling { get; private set; }
        public bool IsWinRTImplementation { get; set; }
        public IList<GeneratedMethod> GeneratedFilterMethods { get; set; }
        public IDictionary<MethodDefinition, string> GeneratedMethodDefinitionToNameMap { get; set; }

		public TypeSpecificContext(TypeDefinition currentType, Dictionary<MethodDefinition, string> methodDefinitionToNameMap, 
			Dictionary<FieldDefinition, string> backingFieldToNameMap, ICollection<string> usedNamespaces, ICollection<string> visibleMembersNames,
			Dictionary<string, InitializationAssignment> fieldToAssignedExpression, HashSet<PropertyDefinition> autoImplementedProperties,
			HashSet<EventDefinition> autoImplementedEvents, ExplicitlyImplementedMembersCollection explicitlyImplementedMembers,
            ICollection<MethodDefinition> exceptionsWhileDecompiling, IList<GeneratedMethod> generatedFilterMethods,
            IDictionary<MethodDefinition, string> generatedMethodDefinitionToNameMap)
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

        private TypeSpecificContext ()
	    {

	    }

        public TypeSpecificContext ShallowPartialClone()
        {
            TypeSpecificContext partialClone = new TypeSpecificContext();
            partialClone.CurrentType = this.CurrentType;
            partialClone.MethodDefinitionToNameMap = this.MethodDefinitionToNameMap;
            partialClone.BackingFieldToNameMap = this.BackingFieldToNameMap;
            partialClone.UsedNamespaces = this.UsedNamespaces;
            partialClone.VisibleMembersNames = this.VisibleMembersNames;
            partialClone.fieldToEventMap = this.fieldToEventMap;
            partialClone.methodToPropertyMap = this.MethodToPropertyMap;
            partialClone.IsWinRTImplementation = this.IsWinRTImplementation;
            partialClone.fieldToPropertyMap = this.fieldToPropertyMap;
            partialClone.GeneratedFilterMethods = this.GeneratedFilterMethods;
            partialClone.GeneratedMethodDefinitionToNameMap = this.GeneratedMethodDefinitionToNameMap;

			partialClone.AssignmentData = new Dictionary<string, InitializationAssignment>();
            partialClone.BaseCtorInvocators = new HashSet<MethodDefinition>();
            partialClone.FieldInitializationFailed = false;

            return partialClone;
        }
    }
}
