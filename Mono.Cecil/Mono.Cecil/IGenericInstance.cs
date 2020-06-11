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
using System.Collections.Generic;
using System.Text;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public interface IGenericInstance : IMetadataTokenProvider {

		bool HasGenericArguments { get; }
		Collection<TypeReference> GenericArguments { get; }

		/*Telerik Authosrhip*/
		void AddGenericArgument(TypeReference theArgument);

		/*Telerik Authorship*/
		Dictionary<int, TypeReference> PostionToArgument { get; set; }
	}

	static partial class Mixin {

		public static bool ContainsGenericParameter (this IGenericInstance self)
		{
			var arguments = self.GenericArguments;

			for (int i = 0; i < arguments.Count; i++)
				if (arguments [i].ContainsGenericParameter)
					return true;

			return false;
		}

		public static void GenericInstanceFullName (this IGenericInstance self, StringBuilder builder)
		{
			builder.Append ("<");
			var arguments = self.GenericArguments;
			/*Telerik Authorship*/
			for (int i = 0; i < arguments.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(",");
				}
				if (self.PostionToArgument.ContainsKey(i))
				{
					builder.Append(self.PostionToArgument[i].FullName);
				}
				else
				{
					builder.Append(arguments[i].FullName);
				}
			}
			builder.Append (">");
		}

		/*Telerik Authorship*/
		public static void GenericInstanceFullName(this IGenericInstance self, StringBuilder builder, string leftBracket, string rightBracket, System.Func<TypeReference, string> getTypeGenericName)
		{
			builder.Append(leftBracket);
			var arguments = self.GenericArguments;
			for (int i = 0; i < arguments.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(",");
				}

				builder.Append(getTypeGenericName(arguments[i]));
			}
			builder.Append(rightBracket);
		}
	}
}
