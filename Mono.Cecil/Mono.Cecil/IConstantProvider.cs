//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

/*Telerik Authorship*/
using Mono.Cecil.Mono.Cecil;
namespace Mono.Cecil {

	public interface IConstantProvider : IMetadataTokenProvider {

		bool HasConstant { get; set; }
		/*Telerik Authorship*/
		ConstantValue Constant { get; set; }
	}

	static partial class Mixin {

		/*Telerik Authorship*/
		internal static ConstantValue NoValue = new ConstantValue ();
		/*Telerik Authorship*/
		internal static ConstantValue NotResolved = new ConstantValue ();

		public static void ResolveConstant (
			this IConstantProvider self,
			/*Telerik Authorship*/ ref ConstantValue constant,
			ModuleDefinition module)
		{
			if (module == null) {
				constant = Mixin.NoValue;
				return;
			}

			lock (module.SyncRoot) {
				if (constant != Mixin.NotResolved)
					return;
				if (module.HasImage ())
					constant = module.Read (self, (provider, reader) => reader.ReadConstant (provider));
				else
					constant = Mixin.NoValue;
			}
		}
	}
}
