//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;
using System.Text;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public class MethodReference : MemberReference, IMethodSignature, IGenericParameterProvider, IGenericContext {

		internal ParameterDefinitionCollection parameters;
		MethodReturnType return_type;

		bool has_this;
		bool explicit_this;
		MethodCallingConvention calling_convention;
		internal Collection<GenericParameter> generic_parameters;

		public virtual bool HasThis {
			get { return has_this; }
			set { has_this = value; }
		}

		public virtual bool ExplicitThis {
			get { return explicit_this; }
			set { explicit_this = value; }
		}

		public virtual MethodCallingConvention CallingConvention {
			get { return calling_convention; }
			set { calling_convention = value; }
		}

		public virtual bool HasParameters {
			get { return !parameters.IsNullOrEmpty (); }
		}

		public virtual Collection<ParameterDefinition> Parameters {
			get {
				if (parameters == null)
					parameters = new ParameterDefinitionCollection (this);

				return parameters;
			}
		}

		IGenericParameterProvider IGenericContext.Type {
			get {
				var declaring_type = this.DeclaringType;
				var instance = declaring_type as GenericInstanceType;
				if (instance != null)
					return instance.ElementType;

				return declaring_type;
			}
		}

		IGenericParameterProvider IGenericContext.Method {
			get { return this; }
		}

		GenericParameterType IGenericParameterProvider.GenericParameterType {
			get { return GenericParameterType.Method; }
		}

		public virtual bool HasGenericParameters {
			get { return !generic_parameters.IsNullOrEmpty (); }
		}

		public virtual Collection<GenericParameter> GenericParameters {
			get {
				if (generic_parameters != null)
					return generic_parameters;

				return generic_parameters = new GenericParameterCollection (this);
			}
		}

		public TypeReference ReturnType {
			get {
				var return_type = MethodReturnType;
				return return_type != null ? return_type.ReturnType : null;
			}
			set {
				var return_type = MethodReturnType;
				if (return_type != null)
					return_type.ReturnType = value;
			}
		}

		/*Telerik Authorship*/
		private object locker = new object();

		/*Telerik Authorship*/
		private TypeReference fixedReturnType;
		private bool resolved = false;
		public TypeReference FixedReturnType
		{
			get
			{
				if (resolved)
					return fixedReturnType;

				lock (locker)
				{
					if (resolved)
						return fixedReturnType;
					MethodReturnType returnType = MethodReturnType;
					TypeReference typeReference = returnType != null ? returnType.ReturnType : null;


					this.fixedReturnType = GetFixedReturnType(typeReference);
					this.resolved = true;
				}

				return fixedReturnType;
			}
			set
			{
				lock (locker)
				{
					var return_type = MethodReturnType;
					if (return_type != null)
						return_type.ReturnType = value;
				}
			}
		}

		/*Telerik Authorship*/
		private TypeReference GetFixedReturnType(TypeReference type)
		{
			if (type == null)
			{
				return null;
			}

			if (type.IsOptionalModifier)
			{
				OptionalModifierType omt = (OptionalModifierType)type;
				TypeReference fixedElement = GetFixedReturnType(omt.ElementType);
				return new OptionalModifierType(omt.ModifierType, fixedElement);
			}

			if (type.IsRequiredModifier)
			{
				RequiredModifierType rmt = (RequiredModifierType)type;
				TypeReference fixedElement = GetFixedReturnType(rmt.ElementType);
				return new RequiredModifierType(rmt.ModifierType, fixedElement);
			}			
			
			if (type.IsGenericParameter)
			{
				return GetActualType(type);
			}
			if (type.IsArray)
			{
				ArrayType at = (ArrayType)type;
				int rank = at.Rank;
				TypeReference arrayElementType = at.ElementType;
				arrayElementType = GetFixedReturnType(arrayElementType);
				return new ArrayType(arrayElementType, rank);
			}

			if (type.IsPointer)
			{
				TypeReference fixedElement = GetFixedReturnType(((PointerType)type).ElementType);
				return new PointerType(fixedElement);
			}

			if (type.IsByReference)
			{
				TypeReference fixedElement = GetFixedReturnType(((ByReferenceType)type).ElementType);
				return new ByReferenceType(fixedElement);
			}


			if (type.IsGenericInstance && DeclaringType.IsGenericInstance)
			{
				GenericInstanceType result = type as GenericInstanceType;
				GenericInstanceType declaringTypeGenericInstance = DeclaringType as GenericInstanceType;
				TypeReference declaringElementType = DeclaringType.GetElementType();
				for (int i = 0; i < result.GenericArguments.Count; i++)
				{
					GenericParameter currentParam = result.GenericArguments[i] as GenericParameter;
					if (currentParam != null && currentParam.Owner == declaringElementType)
					{
						if (declaringTypeGenericInstance.PostionToArgument.ContainsKey(currentParam.Position))
						{
							result.ReplaceGenericArgumentAt(i, declaringTypeGenericInstance.PostionToArgument[currentParam.position]);
						}
					}
				}
				return result;
			}

			return type;
		}

		/*Telerik Authorship*/
		private TypeReference GetActualType(TypeReference type)
		{
			GenericParameter gp = type as GenericParameter;
			if (gp == null)
			{
				return type;
			}
			int index = gp.Position;
			if (gp.Owner is MethodReference && this.IsGenericInstance)
			{
				return (this as GenericInstanceMethod).PostionToArgument[index];
				//GenericInstanceMethod generic = this as GenericInstanceMethod;
				//if (index >= 0 && index < generic.GenericArguments.Count)
				//{
				//    type = generic.GenericArguments[index];
				//}
			}
			else if (gp.Owner is TypeReference && this.DeclaringType.IsGenericInstance)
			{
				GenericInstanceType generic = this.DeclaringType as GenericInstanceType;
				if (index >= 0 && index < generic.GenericArguments.Count)
				{
					type = generic.GenericArguments[index];
				}
			}
			return type;
		}

		/*Telerik Authorship*/
		private GenericParameter genericParameterReturnType;
		public GenericParameter GenericParameterReturnType
		{
			get
			{
				lock (locker)
				{
					if (genericParameterReturnType != null)
					{
						return genericParameterReturnType;
					}

					if (return_type.ReturnType.IsGenericParameter)
					{
						genericParameterReturnType = return_type.ReturnType as GenericParameter;
					}
				}

				return genericParameterReturnType;
			}
		}

		public virtual MethodReturnType MethodReturnType {
			get { return return_type; }
			set { return_type = value; }
		}

		public override string FullName {
			get {
				var builder = new StringBuilder ();
				builder.Append (/*Telerik Authorship*/FixedReturnType.FullName)
					.Append (" ")
					.Append (MemberFullName ());
				this.MethodSignatureFullName (builder);
				return builder.ToString ();
			}
		}

		public virtual bool IsGenericInstance {
			get { return false; }
		}

		public override bool ContainsGenericParameter {
			get {
				if (this.ReturnType.ContainsGenericParameter || base.ContainsGenericParameter)
					return true;

				var parameters = this.Parameters;

				for (int i = 0; i < parameters.Count; i++)
					if (parameters [i].ParameterType.ContainsGenericParameter)
						return true;

				return false;
			}
		}

		internal MethodReference ()
		{
			this.return_type = new MethodReturnType (this);
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public MethodReference (string name, TypeReference returnType)
			: base (name)
		{
			if (returnType == null)
				throw new ArgumentNullException ("returnType");

			this.return_type = new MethodReturnType (this);
			this.return_type.ReturnType = returnType;
			this.token = new MetadataToken (TokenType.MemberRef);
		}

		public MethodReference (string name, TypeReference returnType, TypeReference declaringType)
			: this (name, returnType)
		{
			if (declaringType == null)
				throw new ArgumentNullException ("declaringType");

			this.DeclaringType = declaringType;
		}

		public virtual MethodReference GetElementMethod ()
		{
			return this;
		}

		public virtual MethodDefinition Resolve ()
		{
			var module = this.Module;
			if (module == null)
				throw new NotSupportedException ();

			return module.Resolve (this);
		}

		/*Telerik Authorship*/
		public virtual bool IsConstructor
		{
			get
			{
				return this.Name == ".cctor" || this.Name == ".ctor";
			}
		}
	}

	static partial class Mixin {

		public static bool IsVarArg (this IMethodSignature self)
		{
			return (self.CallingConvention & MethodCallingConvention.VarArg) != 0;
		}

		public static int GetSentinelPosition (this IMethodSignature self)
		{
			if (!self.HasParameters)
				return -1;

			var parameters = self.Parameters;
			for (int i = 0; i < parameters.Count; i++)
				if (parameters [i].ParameterType.IsSentinel)
					return i;

			return -1;
		}
	}
}
