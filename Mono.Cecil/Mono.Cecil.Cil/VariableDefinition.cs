//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

namespace Mono.Cecil.Cil {

	public sealed class VariableDefinition : VariableReference, /*Telerik Authorship*/IDynamicTypeContainer {

		public bool IsPinned {
			get { return variable_type.IsPinned; }
		}

		/*Telerik Authorship*/
		public bool IsDynamic
		{
			get
			{
				return DynamicPositioningFlags != null;
			}
		}
		public bool[] DynamicPositioningFlags { get; set; }
		TypeReference IDynamicTypeContainer.DynamicContainingType
		{
			get
			{
				return this.VariableType;
			}
		}

		/*Telerik Authorship*/
		public MethodDefinition ContainingMethod { get; internal set; }

		/*Telerik Authorship*/internal VariableDefinition (TypeReference variableType)
			: base (variableType)
		{
		}

		/*Telerik Authorship*/
		public VariableDefinition(TypeReference variableType, MethodDefinition containingMethod)
			: this (variableType)
		{
			this.ContainingMethod = containingMethod;
		}

		public VariableDefinition (string name, TypeReference variableType, /*Telerik Authorship*/MethodDefinition containingMethod)
			: base (name, variableType)
		{
			/*Telerik Authorship*/
			this.ContainingMethod = containingMethod;
		}

		public override VariableDefinition Resolve ()
		{
			return this;
		}
	}
}
