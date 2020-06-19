using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler
{
	/// <summary>
	/// Contains the documentation for a single assembly.
	/// </summary>
	class DocumentationCache
	{
		/// <summary>
		/// The cached documentation.
		/// </summary>
		private readonly Dictionary<string, string> documentationMap;

		/// <summary>
		/// The full file path to the cached xml file.
		/// </summary>
		public string DocumentationFilePath { get; private set; }

		public DocumentationCache(Dictionary<string, string> map, string filePath)
		{
			this.documentationMap = map;
			this.DocumentationFilePath = filePath;
		}

		/// <summary>
		/// Gets the documentation related to <paramref name="member"/>. If no documentation is found for this particular member,
		/// an empty string is returned.
		/// </summary>
		/// <param name="member"></param>
		/// <returns>String containing the XML-formated documentation for <paramref name="member">.</returns>
		public string GetDocumentationForMember(IMemberDefinition member)
		{
			string memberName = GetDocumentationMemberName(member);
			if (documentationMap.ContainsKey(memberName))
			{
				return documentationMap[memberName];
			}
			return string.Empty;
		}

		/// <summary>
		/// Generates the name, used to specify <paramref name="member"/> according to ECMA-334 standart.
		/// </summary>
		/// <param name="member">The member who's name is about to be generated.</param>
		/// <returns>Returns the string used as ID in the documentation to refer to <paramref name="member">.</returns>
		private string GetDocumentationMemberName(IMemberDefinition member)
		{
			StringBuilder sb = new StringBuilder();

			string prefix = GetMemberPrefix(member);
			sb.Append(prefix);

			string memberFullname = GetMemberFullName(member);
			sb.Append(memberFullname);
			return sb.ToString();
		}

		/// <summary>
		/// Creates the part of the member's ID string representation, following the prefix showing the kind of the member, according to ECMA-334 standart. 
		/// See chapter E: Documentation Comments.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		private string GetMemberFullName(IMemberDefinition member)
		{
			StringBuilder result = new StringBuilder();
			if (member is TypeDefinition)
			{
				return GetTypeFullName(member as TypeDefinition);
			}
			else
			{
				// Every member, that is not a type, is supposed to have declaring type.
				// As the specification is for the C# language, which does not allow methods to be declared outside of classes,
				// global methods can't be documented using this format.
				result.Append(GetTypeFullName(member.DeclaringType));
				result.Append('.');
			}
			string memberName = member.Name.Replace('.', '#'); // according to the standart, '.' should be replaced with '#'
			result.Append(memberName);

			// Methods are the only members that can be generic (types are already handled).
			if (member is MethodDefinition)
			{
				string genericMarker = GetMethodGenericParametersMarker(member as MethodDefinition);
				result.Append(genericMarker);
			}

			if (member is MethodDefinition || member is PropertyDefinition)
			{
				string arguments = GetArgumentsString(member);
				result.Append(arguments);
			}
			return result.ToString();
		}

		/// <summary>
		/// Gets the string representing the count of the generic parameters of <paramref name="method"/>.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>Retruns the generated marker following the ECMA-334 standart. For methods that don't have generic parameters,
		/// the result is empty string.</returns>
		private string GetMethodGenericParametersMarker(MethodDefinition method)
		{
			if (!method.HasGenericParameters)
			{
				return string.Empty;
			}
			return string.Format("``{0}", method.GenericParameters.Count);
		}

		private string GetArgumentsString(IMemberDefinition member)
		{
			if (member is MethodDefinition)
			{
				return GetArgumentsString((member as MethodDefinition).Parameters);
			}
			if (member is PropertyDefinition)
			{
				return GetArgumentsString((member as PropertyDefinition).Parameters);
			}
			return string.Empty;
		}

		private string GetArgumentsString(Collection<ParameterDefinition> parameters)
		{
			if (parameters.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder result = new StringBuilder();
			result.Append('(');
			bool first = true;
			foreach (ParameterDefinition param in parameters)
			{
				if (!first)
				{
					result.Append(',');
				}
				string parameterTypeName = GetParameterTypeRepresentation(param.ParameterType);
				result.Append(parameterTypeName);
				first = false;
			}
			result.Append(')');
			return result.ToString();
		}

		/// <summary>
		/// Returns the string representation of <paramref name="parameterType"/> as expected when declaring a method/property parameter
		/// in documentation comment. Follows the rules specified in ECMA-334, partition E.
		/// </summary>
		/// <param name="parameterType">The type to be serialized.</param>
		/// <returns>Returns the string representation of the type.</returns>
		private string GetParameterTypeRepresentation(TypeReference parameterType)
		{
			if (parameterType is GenericParameter)
			{
				return HandleGenericParameterType(parameterType as GenericParameter);
			}
			if (parameterType is ByReferenceType)
			{
				return HandleByReferenceType(parameterType as ByReferenceType);
			}
			if (parameterType is ArrayType)
			{
				return HandleArrayType(parameterType as ArrayType);
			}

			StringBuilder result = new StringBuilder();
			//Nested Types
			if (parameterType.DeclaringType != null)
			{
				if (parameterType.DeclaringType.HasGenericParameters && parameterType is GenericInstanceType)
				{
					return HandleNestedGenericTypes(parameterType);
				}

				// The parent type is not generic
				result.Append(GetParameterTypeRepresentation(parameterType.DeclaringType));
			}
			else
			{
				// There was no parent type.
				result.Append(parameterType.Namespace);
			}
			// if the namespace is empty, do we need '.' ?????
			result.Append('.');

			string typeName = GenericHelper.GetNonGenericName(parameterType.Name);
			result.Append(typeName);

			if (parameterType is GenericInstanceType)
			{
				result.Append(AppendGenericArguments(parameterType as GenericInstanceType, 0));
			}

			return result.ToString();
		}

		private string HandleNestedGenericTypes(TypeReference parameterType)
		{
			/// Transfer the parameters from reference up to the declaring type.
			/// Bare in mind, that the declaring type might have less generic parameters.
			/// Transfer just the first X that match.
			/// This is needed, because VB and C# don't allow other language syntax
			StringBuilder result = new StringBuilder();
			TypeReference declaringType = parameterType.DeclaringType;
			GenericInstanceType referenceGeneric = parameterType as GenericInstanceType;

			GenericInstanceType declaringTypeInstance = new GenericInstanceType(declaringType);

			Collection<TypeReference> nestedTypeBackup = new Collection<TypeReference>(referenceGeneric.GenericArguments);
			Collection<TypeReference> declaringBackup = new Collection<TypeReference>(declaringTypeInstance.GenericArguments);
			int parametersToMoveUp = declaringType.GenericParameters.Count;
			for (int i = 0; i < parametersToMoveUp; i++)
			{
				declaringTypeInstance.AddGenericArgument(referenceGeneric.GenericArguments[i]);
				declaringTypeInstance.GenericArguments.Add(referenceGeneric.GenericArguments[i]);
			}
			result.Append(GetParameterTypeRepresentation(declaringTypeInstance));
			result.Append('.');
			if (referenceGeneric.GenericArguments.Count - parametersToMoveUp > 0)
			{
				result.Append(AppendGenericArguments(referenceGeneric, parametersToMoveUp));
			}
			else
			{
				string ptName = GenericHelper.GetNonGenericName(parameterType.Name);
				result.Append(ptName);
			}
			referenceGeneric.GenericArguments.Clear();
			referenceGeneric.GenericArguments.AddRange(nestedTypeBackup);

			declaringTypeInstance.GenericArguments.Clear();
			declaringTypeInstance.GenericArguments.AddRange(declaringBackup);
			return result.ToString();
		}

		private string AppendGenericArguments(GenericInstanceType generic, int startingIndex)
		{
			StringBuilder result = new StringBuilder();
			result.Append('{');

			for (int i = startingIndex; i < generic.GenericArguments.Count; i++)
			{
				if (i > startingIndex)
				{
					result.Append(',');
				}
				// nested generic types
				// for instance List<List<int>>
				TypeReference genericParameterType = generic.PostionToArgument[i];
				string paramType = GetParameterTypeRepresentation(genericParameterType);
				result.Append(paramType);
			}

			result.Append('}');
			return result.ToString();
		}

		private string HandleArrayType(ArrayType parameterType)
		{
			StringBuilder result = new StringBuilder();
			result.Append(GetParameterTypeRepresentation(parameterType.ElementType));
			result.Append('[');
			bool first = true;
			foreach (ArrayDimension dimention in parameterType.Dimensions)
			{
				if (!first)
				{
					result.Append(',');
				}
				if (dimention.LowerBound.HasValue)
				{
					result.Append(dimention.LowerBound.Value);
				}
				if (dimention.IsSized)
				{
					result.Append(':');
				}
				if (dimention.UpperBound.HasValue)
				{
					result.Append(dimention.UpperBound.Value);
				}
				first = false;
			}
			result.Append(']');
			return result.ToString();
		}

		private string HandleByReferenceType(ByReferenceType parameterType)
		{
			TypeReference elementType = parameterType.ElementType;
			string typeResult = string.Format("{0}@", GetParameterTypeRepresentation(elementType));
			return typeResult;
		}

		private string HandleGenericParameterType(GenericParameter parameterType)
		{
			StringBuilder result = new StringBuilder();
			if (parameterType.Owner is TypeReference)
			{
				result.Append('`');
			}
			else
			{
				result.Append("``");
			}
			result.Append(parameterType.Position);
			return result.ToString();
		}

		private string GetTypeFullName(TypeDefinition theType)
		{
			StringBuilder result = new StringBuilder();
			if (theType.DeclaringType != null)
			{
				result.Append(GetTypeFullName(theType.DeclaringType));
			}
			else
			{
				result.Append(theType.Namespace);
			}
			string typeName = GenericHelper.GetNonGenericName(theType.Name).Replace('.', '#');
			result.AppendFormat(".{0}", typeName); // generic parameters count?
			if (theType.HasGenericParameters)
			{
				int count = theType.GenericParameters.Count;
				if (theType.DeclaringType != null && theType.DeclaringType.HasGenericParameters)
				{
					count -= theType.DeclaringType.GenericParameters.Count;
				}
				if (count > 0)
				{
					result.AppendFormat("`{0}", count);
				}
			}
			return result.ToString();
		}

		private string GetMemberPrefix(IMemberDefinition member)
		{
			if (member is EventDefinition)
			{
				return "E:";
			}
			else if (member is FieldDefinition)
			{
				return "F:";
			}
			else if (member is MethodDefinition)
			{
				return "M:";
			}
			else if (member is PropertyDefinition)
			{
				return "P:";
			}
			else if (member is TypeDefinition)
			{
				return "T:";
			}
			return string.Empty;
		}

		public void ClearCache()
		{
			this.documentationMap.Clear();
		}
	}
}
