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
/*Telerik Authorship*/
using System.Collections.Generic;
using System.Text;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public sealed class GenericInstanceMethod : MethodSpecification, IGenericInstance, IGenericContext {

		Collection<TypeReference> arguments;

		/*Telerik Authorship*/
		public Dictionary<int, TypeReference> PostionToArgument { get; set; }

		public bool HasGenericArguments {
			get { return !arguments.IsNullOrEmpty (); }
		}

		public Collection<TypeReference> GenericArguments {
			get { return arguments ?? (arguments = new Collection<TypeReference> ()); }
		}

		/*Telerik Authorship*/
		private object locker = new object();

		/*Telerik Authorship*/
		public void AddGenericArgument(TypeReference theArgument)
		{
			/// don't change the outer type.
			/// Instead better create a map and use it for creating the full name of the method.
			//this.GenericArguments.Add(theArgument);
			lock (locker)
			{
				this.PostionToArgument.Add(this.PostionToArgument.Count, theArgument);
				int i = this.GenericArguments.Count - 1;
				
				foreach (ParameterDefinition parameter in this.Parameters)
				{
					TypeReference pType = parameter.ParameterType;
					if (pType is ByReferenceType)
					{
						pType = (pType as ByReferenceType).ElementType;
					}
					if (pType is GenericInstanceType)
					{
						GenericInstanceType paramType = pType as GenericInstanceType;

						UpdateSingleDependingType(paramType, theArgument, i);
					}
				}

				if (ReturnType is GenericInstanceType)
				{
					GenericInstanceType returnType = ReturnType as GenericInstanceType;
					UpdateSingleDependingType(returnType, theArgument, i);
				}
			}
		}

		/*Telerik Authorship*/
		private void UpdateSingleDependingType(GenericInstanceType dependingType, TypeReference theArgument, int argumentIndex)
		{
			int index = 0;
			for (index = 0; index < dependingType.GenericArguments.Count; index++)
			{
				GenericParameter t = dependingType.GenericArguments[index] as GenericParameter;
				if (t == null)
				{
					if (dependingType.GenericArguments[index] is GenericInstanceType)
					{
						UpdateSingleDependingType(dependingType.GenericArguments[index] as GenericInstanceType, theArgument, argumentIndex);
					}
					continue;
				}
				if (t.Owner == this.ElementMethod && t.Position == argumentIndex)
				{
					dependingType.ReplaceGenericArgumentAt(index, theArgument);
				}
				//if (t.FullName == string.Format("!!{0}", i))
				//{
				//    break;
				//}
			}
			//if (index < paramType.GenericArguments.Count)
			//{
			//    paramType.ReplaceGenericArgumentAt(index, theArgument);
			//}
		}
		
		public override bool IsGenericInstance {
			get { return true; }
		}

		IGenericParameterProvider IGenericContext.Method {
			get { return ElementMethod; }
		}

		IGenericParameterProvider IGenericContext.Type {
			get { return ElementMethod.DeclaringType; }
		}

		public override bool ContainsGenericParameter {
			get { return this.ContainsGenericParameter () || base.ContainsGenericParameter; }
		}

		public override string FullName {
			get {
				var signature = new StringBuilder ();
				var method = this.ElementMethod;
				signature.Append (/*Telerik Authorship*/FixedReturnType.FullName)
					.Append (" ")
					.Append (/*Telerik Authorship*/DeclaringType.FullName)
					.Append ("::")
					.Append (method.Name);
				this.GenericInstanceFullName (signature);
				this.MethodSignatureFullName (signature);
				return signature.ToString ();

			}
		}

		/*Telerik Authorship*/
		public string GetGenericName(string leftBracket, string rightBracket, System.Func<TypeReference, string> getTypeGenericName)
		{
			var signature = new StringBuilder();
			var method = this.ElementMethod;
			signature.Append(method.Name);
			this.GenericInstanceFullName(signature, leftBracket, rightBracket, getTypeGenericName);
			return signature.ToString();
		}

		public GenericInstanceMethod (MethodReference method)
			: base (method)
		{

			/*Telerik Autrhorship*/
			this.PostionToArgument = new Dictionary<int, TypeReference>();

			/// Deep copy on generic methods is needed because the PositionToArgument cache is preserved for generic instance parameters.
			/// That is, if a method has a parameter of a generic type (i.e. List<T>) that is once ivoked with concrete signature (i.e. List<int>)
			/// the mapping for the concrete signature will remain, possibly corrupting further decompilation results.
			DeepCopyParameters(method);

			//this.MethodReturnType = new MethodReturnType(method);
			this.MethodReturnType = DeepCloneMethodReturnType(method);

			this.ReturnType = DeepCloneType(method.ReturnType);
		}
  
		/*Telerik Authorship*/
		private void DeepCopyParameters(MethodReference method)
		{
			this.parameters = new ParameterDefinitionCollection(this);
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				TypeReference typeClone = DeepCloneType(parameter.ParameterType);
				ParameterDefinition copy = new ParameterDefinition(parameter.Name,parameter.Attributes,typeClone);
				this.parameters.Add(copy);
			}
		}

		/*Telerik Authorship*/
		/// <summary>
		/// Performs shallow clone of the MethodReturnType of <paramref name="method"/>
		/// </summary>
		private MethodReturnType DeepCloneMethodReturnType(MethodReference method)
		{
			MethodReturnType result = new MethodReturnType(this); // not sure how will this behave
			result.parameter = method.MethodReturnType.parameter;
			result.ReturnType = DeepCloneType(method.ReturnType);

			return result;
		}

		/*Telerik Authorship*/
		private TypeReference DeepCloneType(TypeReference toClone)
		{
			if (toClone is ByReferenceType)
			{
				TypeReference clonedElementType = DeepCloneType((toClone as ByReferenceType).ElementType);
				return new ByReferenceType(clonedElementType);
			}
			if (toClone is GenericInstanceType)
			{
				GenericInstanceType original = toClone as GenericInstanceType;
				// for proof of concept - should clone all types for consistency
				GenericInstanceType result = new GenericInstanceType(toClone.GetElementType());
				foreach (TypeReference argument in (toClone as GenericInstanceType).GenericArguments)
				{
					TypeReference clonedArgument = DeepCloneType(argument); 
					// clone the arguments as well, as they might be another GenericInstanceTypes
					result.GenericArguments.Add(clonedArgument);
				}
				result.PostionToArgument.Clear();
				return result;
			}
			return toClone;
		}
	}
}
