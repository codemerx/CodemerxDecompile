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
			base();
			this.documentationMap = map;
			this.set_DocumentationFilePath(filePath);
			return;
		}

		private string AppendGenericArguments(GenericInstanceType generic, int startingIndex)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append('{');
			V_1 = startingIndex;
			while (V_1 < generic.get_GenericArguments().get_Count())
			{
				if (V_1 > startingIndex)
				{
					dummyVar1 = V_0.Append(',');
				}
				V_2 = generic.get_PostionToArgument().get_Item(V_1);
				dummyVar2 = V_0.Append(this.GetParameterTypeRepresentation(V_2));
				V_1 = V_1 + 1;
			}
			dummyVar3 = V_0.Append('}');
			return V_0.ToString();
		}

		public void ClearCache()
		{
			this.documentationMap.Clear();
			return;
		}

		private string GetArgumentsString(IMemberDefinition member)
		{
			if (member as MethodDefinition != null)
			{
				return this.GetArgumentsString((member as MethodDefinition).get_Parameters());
			}
			if (member as PropertyDefinition == null)
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
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append('(');
			V_1 = true;
			V_2 = parameters.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!V_1)
					{
						dummyVar1 = V_0.Append(',');
					}
					V_4 = this.GetParameterTypeRepresentation(V_3.get_ParameterType());
					dummyVar2 = V_0.Append(V_4);
					V_1 = false;
				}
			}
			finally
			{
				V_2.Dispose();
			}
			dummyVar3 = V_0.Append(')');
			return V_0.ToString();
		}

		public string GetDocumentationForMember(IMemberDefinition member)
		{
			V_0 = this.GetDocumentationMemberName(member);
			if (!this.documentationMap.ContainsKey(V_0))
			{
				return String.Empty;
			}
			return this.documentationMap.get_Item(V_0);
		}

		private string GetDocumentationMemberName(IMemberDefinition member)
		{
			stackVariable0 = new StringBuilder();
			dummyVar0 = stackVariable0.Append(this.GetMemberPrefix(member));
			dummyVar1 = stackVariable0.Append(this.GetMemberFullName(member));
			return stackVariable0.ToString();
		}

		private string GetMemberFullName(IMemberDefinition member)
		{
			V_0 = new StringBuilder();
			if (member as TypeDefinition != null)
			{
				return this.GetTypeFullName(member as TypeDefinition);
			}
			dummyVar0 = V_0.Append(this.GetTypeFullName(member.get_DeclaringType()));
			dummyVar1 = V_0.Append('.');
			V_1 = member.get_Name().Replace('.', '#');
			dummyVar2 = V_0.Append(V_1);
			if (member as MethodDefinition != null)
			{
				dummyVar3 = V_0.Append(this.GetMethodGenericParametersMarker(member as MethodDefinition));
			}
			if (member as MethodDefinition != null || member as PropertyDefinition != null)
			{
				dummyVar4 = V_0.Append(this.GetArgumentsString(member));
			}
			return V_0.ToString();
		}

		private string GetMemberPrefix(IMemberDefinition member)
		{
			if (member as EventDefinition != null)
			{
				return "E:";
			}
			if (member as FieldDefinition != null)
			{
				return "F:";
			}
			if (member as MethodDefinition != null)
			{
				return "M:";
			}
			if (member as PropertyDefinition != null)
			{
				return "P:";
			}
			if (member as TypeDefinition != null)
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
			if (parameterType as GenericParameter != null)
			{
				return this.HandleGenericParameterType(parameterType as GenericParameter);
			}
			if (parameterType as ByReferenceType != null)
			{
				return this.HandleByReferenceType(parameterType as ByReferenceType);
			}
			if (parameterType as ArrayType != null)
			{
				return this.HandleArrayType(parameterType as ArrayType);
			}
			V_0 = new StringBuilder();
			if (parameterType.get_DeclaringType() == null)
			{
				dummyVar1 = V_0.Append(parameterType.get_Namespace());
			}
			else
			{
				if (parameterType.get_DeclaringType().get_HasGenericParameters() && parameterType as GenericInstanceType != null)
				{
					return this.HandleNestedGenericTypes(parameterType);
				}
				dummyVar0 = V_0.Append(this.GetParameterTypeRepresentation(parameterType.get_DeclaringType()));
			}
			dummyVar2 = V_0.Append('.');
			V_1 = GenericHelper.GetNonGenericName(parameterType.get_Name());
			dummyVar3 = V_0.Append(V_1);
			if (parameterType as GenericInstanceType != null)
			{
				dummyVar4 = V_0.Append(this.AppendGenericArguments(parameterType as GenericInstanceType, 0));
			}
			return V_0.ToString();
		}

		private string GetTypeFullName(TypeDefinition theType)
		{
			V_0 = new StringBuilder();
			if (theType.get_DeclaringType() == null)
			{
				dummyVar1 = V_0.Append(theType.get_Namespace());
			}
			else
			{
				dummyVar0 = V_0.Append(this.GetTypeFullName(theType.get_DeclaringType()));
			}
			V_1 = GenericHelper.GetNonGenericName(theType.get_Name()).Replace('.', '#');
			dummyVar2 = V_0.AppendFormat(".{0}", V_1);
			if (theType.get_HasGenericParameters())
			{
				V_2 = theType.get_GenericParameters().get_Count();
				if (theType.get_DeclaringType() != null && theType.get_DeclaringType().get_HasGenericParameters())
				{
					V_2 = V_2 - theType.get_DeclaringType().get_GenericParameters().get_Count();
				}
				if (V_2 > 0)
				{
					dummyVar3 = V_0.AppendFormat("`{0}", V_2);
				}
			}
			return V_0.ToString();
		}

		private string HandleArrayType(ArrayType parameterType)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(this.GetParameterTypeRepresentation(parameterType.get_ElementType()));
			dummyVar1 = V_0.Append('[');
			V_1 = true;
			V_2 = parameterType.get_Dimensions().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!V_1)
					{
						dummyVar2 = V_0.Append(',');
					}
					if (V_3.get_LowerBound().get_HasValue())
					{
						V_4 = V_3.get_LowerBound();
						dummyVar3 = V_0.Append(V_4.get_Value());
					}
					if (V_3.get_IsSized())
					{
						dummyVar4 = V_0.Append(':');
					}
					if (V_3.get_UpperBound().get_HasValue())
					{
						V_4 = V_3.get_UpperBound();
						dummyVar5 = V_0.Append(V_4.get_Value());
					}
					V_1 = false;
				}
			}
			finally
			{
				V_2.Dispose();
			}
			dummyVar6 = V_0.Append(']');
			return V_0.ToString();
		}

		private string HandleByReferenceType(ByReferenceType parameterType)
		{
			V_0 = parameterType.get_ElementType();
			return String.Format("{0}@", this.GetParameterTypeRepresentation(V_0));
		}

		private string HandleGenericParameterType(GenericParameter parameterType)
		{
			V_0 = new StringBuilder();
			if (parameterType.get_Owner() as TypeReference == null)
			{
				dummyVar1 = V_0.Append("``");
			}
			else
			{
				dummyVar0 = V_0.Append('\u0060');
			}
			dummyVar2 = V_0.Append(parameterType.get_Position());
			return V_0.ToString();
		}

		private string HandleNestedGenericTypes(TypeReference parameterType)
		{
			V_0 = new StringBuilder();
			stackVariable2 = parameterType.get_DeclaringType();
			V_1 = parameterType as GenericInstanceType;
			V_2 = new GenericInstanceType(stackVariable2);
			V_3 = new Collection<TypeReference>(V_1.get_GenericArguments());
			V_4 = new Collection<TypeReference>(V_2.get_GenericArguments());
			V_5 = stackVariable2.get_GenericParameters().get_Count();
			V_6 = 0;
			while (V_6 < V_5)
			{
				V_2.AddGenericArgument(V_1.get_GenericArguments().get_Item(V_6));
				V_2.get_GenericArguments().Add(V_1.get_GenericArguments().get_Item(V_6));
				V_6 = V_6 + 1;
			}
			dummyVar0 = V_0.Append(this.GetParameterTypeRepresentation(V_2));
			dummyVar1 = V_0.Append('.');
			if (V_1.get_GenericArguments().get_Count() - V_5 <= 0)
			{
				V_7 = GenericHelper.GetNonGenericName(parameterType.get_Name());
				dummyVar3 = V_0.Append(V_7);
			}
			else
			{
				dummyVar2 = V_0.Append(this.AppendGenericArguments(V_1, V_5));
			}
			V_1.get_GenericArguments().Clear();
			V_1.get_GenericArguments().AddRange(V_3);
			V_2.get_GenericArguments().Clear();
			V_2.get_GenericArguments().AddRange(V_4);
			return V_0.ToString();
		}
	}
}