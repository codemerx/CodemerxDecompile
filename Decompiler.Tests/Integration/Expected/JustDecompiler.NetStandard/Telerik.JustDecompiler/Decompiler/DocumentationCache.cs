using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class DocumentationCache
	{
		private readonly Dictionary<string, string> documentationMap;

		public string DocumentationFilePath
		{
			get;
			private set;
		}

		public DocumentationCache(Dictionary<string, string> map, string filePath)
		{
			this.documentationMap = map;
			this.DocumentationFilePath = filePath;
		}

		private string AppendGenericArguments(GenericInstanceType generic, int startingIndex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('{');
			for (int i = startingIndex; i < generic.GenericArguments.Count; i++)
			{
				if (i > startingIndex)
				{
					stringBuilder.Append(',');
				}
				TypeReference item = generic.PostionToArgument[i];
				stringBuilder.Append(this.GetParameterTypeRepresentation(item));
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		public void ClearCache()
		{
			this.documentationMap.Clear();
		}

		private string GetArgumentsString(IMemberDefinition member)
		{
			if (member is MethodDefinition)
			{
				return this.GetArgumentsString((member as MethodDefinition).Parameters);
			}
			if (!(member is PropertyDefinition))
			{
				return String.Empty;
			}
			return this.GetArgumentsString((member as PropertyDefinition).Parameters);
		}

		private string GetArgumentsString(Collection<ParameterDefinition> parameters)
		{
			if (parameters.Count == 0)
			{
				return String.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('(');
			bool flag = true;
			foreach (ParameterDefinition parameter in parameters)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				string parameterTypeRepresentation = this.GetParameterTypeRepresentation(parameter.ParameterType);
				stringBuilder.Append(parameterTypeRepresentation);
				flag = false;
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public string GetDocumentationForMember(IMemberDefinition member)
		{
			string documentationMemberName = this.GetDocumentationMemberName(member);
			if (!this.documentationMap.ContainsKey(documentationMemberName))
			{
				return String.Empty;
			}
			return this.documentationMap[documentationMemberName];
		}

		private string GetDocumentationMemberName(IMemberDefinition member)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.GetMemberPrefix(member));
			stringBuilder.Append(this.GetMemberFullName(member));
			return stringBuilder.ToString();
		}

		private string GetMemberFullName(IMemberDefinition member)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (member is TypeDefinition)
			{
				return this.GetTypeFullName(member as TypeDefinition);
			}
			stringBuilder.Append(this.GetTypeFullName(member.DeclaringType));
			stringBuilder.Append('.');
			string str = member.Name.Replace('.', '#');
			stringBuilder.Append(str);
			if (member is MethodDefinition)
			{
				stringBuilder.Append(this.GetMethodGenericParametersMarker(member as MethodDefinition));
			}
			if (member is MethodDefinition || member is PropertyDefinition)
			{
				stringBuilder.Append(this.GetArgumentsString(member));
			}
			return stringBuilder.ToString();
		}

		private string GetMemberPrefix(IMemberDefinition member)
		{
			if (member is EventDefinition)
			{
				return "E:";
			}
			if (member is FieldDefinition)
			{
				return "F:";
			}
			if (member is MethodDefinition)
			{
				return "M:";
			}
			if (member is PropertyDefinition)
			{
				return "P:";
			}
			if (member is TypeDefinition)
			{
				return "T:";
			}
			return String.Empty;
		}

		private string GetMethodGenericParametersMarker(MethodDefinition method)
		{
			if (!method.HasGenericParameters)
			{
				return String.Empty;
			}
			return String.Format("``{0}", method.GenericParameters.Count);
		}

		private string GetParameterTypeRepresentation(TypeReference parameterType)
		{
			if (parameterType is GenericParameter)
			{
				return this.HandleGenericParameterType(parameterType as GenericParameter);
			}
			if (parameterType is ByReferenceType)
			{
				return this.HandleByReferenceType(parameterType as ByReferenceType);
			}
			if (parameterType is ArrayType)
			{
				return this.HandleArrayType(parameterType as ArrayType);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (parameterType.DeclaringType == null)
			{
				stringBuilder.Append(parameterType.Namespace);
			}
			else
			{
				if (parameterType.DeclaringType.HasGenericParameters && parameterType is GenericInstanceType)
				{
					return this.HandleNestedGenericTypes(parameterType);
				}
				stringBuilder.Append(this.GetParameterTypeRepresentation(parameterType.DeclaringType));
			}
			stringBuilder.Append('.');
			string nonGenericName = GenericHelper.GetNonGenericName(parameterType.Name);
			stringBuilder.Append(nonGenericName);
			if (parameterType is GenericInstanceType)
			{
				stringBuilder.Append(this.AppendGenericArguments(parameterType as GenericInstanceType, 0));
			}
			return stringBuilder.ToString();
		}

		private string GetTypeFullName(TypeDefinition theType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (theType.DeclaringType == null)
			{
				stringBuilder.Append(theType.Namespace);
			}
			else
			{
				stringBuilder.Append(this.GetTypeFullName(theType.DeclaringType));
			}
			string str = GenericHelper.GetNonGenericName(theType.Name).Replace('.', '#');
			stringBuilder.AppendFormat(".{0}", str);
			if (theType.HasGenericParameters)
			{
				int count = theType.GenericParameters.Count;
				if (theType.DeclaringType != null && theType.DeclaringType.HasGenericParameters)
				{
					count -= theType.DeclaringType.GenericParameters.Count;
				}
				if (count > 0)
				{
					stringBuilder.AppendFormat("`{0}", count);
				}
			}
			return stringBuilder.ToString();
		}

		private string HandleArrayType(ArrayType parameterType)
		{
			int? lowerBound;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.GetParameterTypeRepresentation(parameterType.ElementType));
			stringBuilder.Append('[');
			bool flag = true;
			foreach (ArrayDimension dimension in parameterType.Dimensions)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				if (dimension.LowerBound.HasValue)
				{
					lowerBound = dimension.LowerBound;
					stringBuilder.Append(lowerBound.Value);
				}
				if (dimension.IsSized)
				{
					stringBuilder.Append(':');
				}
				if (dimension.UpperBound.HasValue)
				{
					lowerBound = dimension.UpperBound;
					stringBuilder.Append(lowerBound.Value);
				}
				flag = false;
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		private string HandleByReferenceType(ByReferenceType parameterType)
		{
			TypeReference elementType = parameterType.ElementType;
			return String.Format("{0}@", this.GetParameterTypeRepresentation(elementType));
		}

		private string HandleGenericParameterType(GenericParameter parameterType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!(parameterType.Owner is TypeReference))
			{
				stringBuilder.Append("``");
			}
			else
			{
				stringBuilder.Append('\u0060');
			}
			stringBuilder.Append(parameterType.Position);
			return stringBuilder.ToString();
		}

		private string HandleNestedGenericTypes(TypeReference parameterType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TypeReference declaringType = parameterType.DeclaringType;
			GenericInstanceType genericInstanceType = parameterType as GenericInstanceType;
			GenericInstanceType genericInstanceType1 = new GenericInstanceType(declaringType);
			Collection<TypeReference> typeReferences = new Collection<TypeReference>(genericInstanceType.GenericArguments);
			Collection<TypeReference> typeReferences1 = new Collection<TypeReference>(genericInstanceType1.GenericArguments);
			int count = declaringType.GenericParameters.Count;
			for (int i = 0; i < count; i++)
			{
				genericInstanceType1.AddGenericArgument(genericInstanceType.GenericArguments[i]);
				genericInstanceType1.GenericArguments.Add(genericInstanceType.GenericArguments[i]);
			}
			stringBuilder.Append(this.GetParameterTypeRepresentation(genericInstanceType1));
			stringBuilder.Append('.');
			if (genericInstanceType.GenericArguments.Count - count <= 0)
			{
				string nonGenericName = GenericHelper.GetNonGenericName(parameterType.Name);
				stringBuilder.Append(nonGenericName);
			}
			else
			{
				stringBuilder.Append(this.AppendGenericArguments(genericInstanceType, count));
			}
			genericInstanceType.GenericArguments.Clear();
			genericInstanceType.GenericArguments.AddRange(typeReferences);
			genericInstanceType1.GenericArguments.Clear();
			genericInstanceType1.GenericArguments.AddRange(typeReferences1);
			return stringBuilder.ToString();
		}
	}
}