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

using MD = Mono.Cecil.Metadata;

namespace Mono.Cecil {

	public sealed class GenericInstanceType : TypeSpecification, IGenericInstance, IGenericContext {

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
		public void AddGenericArgument(TypeReference argument)
		{
			lock (locker)
			{
				int argNumber = PostionToArgument.Count;
				this.PostionToArgument.Add(argNumber, argument);
				//this.GenericArguments.Add(argument);
			}
		}

		/*Telerik Authorship*/
		public void ReplaceGenericArgumentAt(int index, TypeReference argument)
		{
			lock (locker)
			{
				//this.GenericArguments[index] = argument;
				this.PostionToArgument[index] = argument;
				//var wtf = this.PostionToArgument;
				//wtf[index] = argument;
			}
		}
		
		public override TypeReference DeclaringType {
			get { return ElementType.DeclaringType; }
			set { throw new NotSupportedException (); }
		}

		public override string FullName {
			get {
				var name = new StringBuilder ();
				name.Append (base.FullName);
				this.GenericInstanceFullName (name);
				return name.ToString ();
			}
		}

		public override bool IsGenericInstance {
			get { return true; }
		}

		public override bool ContainsGenericParameter {
			get { return this.ContainsGenericParameter () || base.ContainsGenericParameter; }
		}

		IGenericParameterProvider IGenericContext.Type {
			get { return ElementType; }
		}

		public GenericInstanceType (TypeReference type)
			: base (type)
		{
			base.IsValueType = type.IsValueType;
			this.etype = MD.ElementType.GenericInst;
			/*Telerik Authorship*/
			this.PostionToArgument = new Dictionary<int, TypeReference>();
		}
	}
}
