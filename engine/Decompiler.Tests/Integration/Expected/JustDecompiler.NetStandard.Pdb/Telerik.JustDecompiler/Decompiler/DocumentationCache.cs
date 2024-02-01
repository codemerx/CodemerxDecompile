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
			for (int i = startingIndex; i < generic.get_GenericArguments().get_Count(); i++)
			{
				if (i > startingIndex)
				{
					stringBuilder.Append(',');
				}
				TypeReference item = generic.get_PostionToArgument()[i];
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
				return this.GetArgumentsString((member as MethodDefinition).get_Parameters());
			}
			if (!(member is PropertyDefinition))
			{
				return String.Empty;
			}
			return this.GetArgumentsString((member as PropertyDefinition).get_Parameters());
		}

		private string GetArgumentsString(Collection<ParameterDefinition> parameters)
		{
			if (parameters.get_Count() == 0)
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
				string parameterTypeRepresentation = this.GetParameterTypeRepresentation(parameter.get_ParameterType());
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
			stringBuilder.Append(this.GetTypeFullName(member.get_DeclaringType()));
			stringBuilder.Append('.');
			string str = member.get_Name().Replace('.', '#');
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
			if (!method.get_HasGenericParameters())
			{
				return String.Empty;
			}
			return String.Format("``{0}", method.get_GenericParameters().get_Count());
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
			if (parameterType.get_DeclaringType() == null)
			{
				stringBuilder.Append(parameterType.get_Namespace());
			}
			else
			{
				if (parameterType.get_DeclaringType().get_HasGenericParameters() && parameterType is GenericInstanceType)
				{
					return this.HandleNestedGenericTypes(parameterType);
				}
				stringBuilder.Append(this.GetParameterTypeRepresentation(parameterType.get_DeclaringType()));
			}
			stringBuilder.Append('.');
			string nonGenericName = GenericHelper.GetNonGenericName(parameterType.get_Name());
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
			if (theType.get_DeclaringType() == null)
			{
				stringBuilder.Append(theType.get_Namespace());
			}
			else
			{
				stringBuilder.Append(this.GetTypeFullName(theType.get_DeclaringType()));
			}
			string str = GenericHelper.GetNonGenericName(theType.get_Name()).Replace('.', '#');
			stringBuilder.AppendFormat(".{0}", str);
			if (theType.get_HasGenericParameters())
			{
				int count = theType.get_GenericParameters().get_Count();
				if (theType.get_DeclaringType() != null && theType.get_DeclaringType().get_HasGenericParameters())
				{
					count -= theType.get_DeclaringType().get_GenericParameters().get_Count();
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
			stringBuilder.Append(this.GetParameterTypeRepresentation(parameterType.get_ElementType()));
			stringBuilder.Append('[');
			bool flag = true;
			foreach (ArrayDimension dimension in parameterType.get_Dimensions())
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				if (dimension.get_LowerBound().HasValue)
				{
					lowerBound = dimension.get_LowerBound();
					stringBuilder.Append(lowerBound.Value);
				}
				if (dimension.get_IsSized())
				{
					stringBuilder.Append(':');
				}
				if (dimension.get_UpperBound().HasValue)
				{
					lowerBound = dimension.get_UpperBound();
					stringBuilder.Append(lowerBound.Value);
				}
				flag = false;
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		private string HandleByReferenceType(ByReferenceType parameterType)
		{
			TypeReference elementType = parameterType.get_ElementType();
			return String.Format("{0}@", this.GetParameterTypeRepresentation(elementType));
		}

		private string HandleGenericParameterType(GenericParameter parameterType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!(parameterType.get_Owner() is TypeReference))
			{
				stringBuilder.Append("``");
			}
			else
			{
				stringBuilder.Append('\u0060');
			}
			stringBuilder.Append(parameterType.get_Position());
			return stringBuilder.ToString();
		}

		private string HandleNestedGenericTypes(TypeReference parameterType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TypeReference declaringType = parameterType.get_DeclaringType();
			GenericInstanceType genericInstanceType = parameterType as GenericInstanceType;
			GenericInstanceType genericInstanceType1 = new GenericInstanceType(declaringType);
			Collection<TypeReference> collection = new Collection<TypeReference>(genericInstanceType.get_GenericArguments());
			Collection<TypeReference> collection1 = new Collection<TypeReference>(genericInstanceType1.get_GenericArguments());
			int count = declaringType.get_GenericParameters().get_Count();
			for (int i = 0; i < count; i++)
			{
				genericInstanceType1.AddGenericArgument(genericInstanceType.get_GenericArguments().get_Item(i));
				genericInstanceType1.get_GenericArguments().Add(genericInstanceType.get_GenericArguments().get_Item(i));
			}
			stringBuilder.Append(this.GetParameterTypeRepresentation(genericInstanceType1));
			stringBuilder.Append('.');
			if (genericInstanceType.get_GenericArguments().get_Count() - count <= 0)
			{
				string nonGenericName = GenericHelper.GetNonGenericName(parameterType.get_Name());
				stringBuilder.Append(nonGenericName);
			}
			else
			{
				stringBuilder.Append(this.AppendGenericArguments(genericInstanceType, count));
			}
			genericInstanceType.get_GenericArguments().Clear();
			genericInstanceType.get_GenericArguments().AddRange(collection);
			genericInstanceType1.get_GenericArguments().Clear();
			genericInstanceType1.get_GenericArguments().AddRange(collection1);
			return stringBuilder.ToString();
		}
	}
}