using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil.Extensions
{
	public static class MemberNamingExtensions
	{
		private static void GenericInstanceFriendlyFullName(this IGenericInstance self, ILanguage language, StringBuilder builder, bool useGenericName, string leftBracket, string rightBracket)
		{
			dummyVar0 = builder.Append(leftBracket);
			V_0 = self.get_GenericArguments();
			V_1 = 0;
			while (V_1 < V_0.get_Count())
			{
				V_2 = V_0.get_Item(V_1);
				if (self.get_PostionToArgument().ContainsKey(V_1))
				{
					V_2 = self.get_PostionToArgument().get_Item(V_1);
				}
				if (V_1 > 0)
				{
					dummyVar1 = builder.Append(",");
				}
				V_3 = V_2.GetFriendlyFullName(language);
				if (useGenericName)
				{
					V_4 = V_2.Resolve();
					if (V_4 != null)
					{
						V_3 = V_4.GetGenericFullName(language);
					}
				}
				dummyVar2 = builder.Append(V_3);
				V_1 = V_1 + 1;
			}
			dummyVar3 = builder.Append(rightBracket);
			return;
		}

		private static string GenericMemberFullName(this MethodDefinition self, ILanguage language)
		{
			if (self.get_DeclaringType() == null)
			{
				return self.GetGenericName(language, "<", ">");
			}
			return String.Concat(self.get_DeclaringType().GetGenericFullName(language), "::", self.GetGenericName(language, "<", ">"));
		}

		private static string GetFriendlyFullArrayTypeName(this ArrayType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), self.get_Suffix());
		}

		private static string GetFriendlyFullByReferenceTypeName(this ByReferenceType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), "&");
		}

		private static string GetFriendlyFullEventReferenceName(this EventReference self, ILanguage language)
		{
			return String.Concat(self.get_EventType().GetFriendlyFullName(language), " ", self.MemberFriendlyFullName(language));
		}

		private static string GetFriendlyFullFieldReferenceName(this FieldReference self, ILanguage language)
		{
			return String.Concat(self.get_FieldType().GetFriendlyFullName(language), " ", self.MemberFriendlyFullName(language));
		}

		private static string GetFriendlyFullFunctionPointerTypeName(this FunctionPointerType self, ILanguage language)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(self.get_Function().get_Name());
			dummyVar1 = V_0.Append(" ");
			dummyVar2 = V_0.Append(self.get_Function().get_FixedReturnType().GetFriendlyFullName(language));
			dummyVar3 = V_0.Append(" *");
			self.MethodSignatureFriendlyFullName(language, V_0, false);
			return V_0.ToString();
		}

		private static string GetFriendlyFullGenericInstanceMethodName(this GenericInstanceMethod self, ILanguage language)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(self.get_FixedReturnType().get_FullName()).Append(" ").Append(self.MemberFriendlyFullName(language));
			self.GenericInstanceFriendlyFullName(language, V_0, false, "<", ">");
			self.MethodSignatureFriendlyFullName(language, V_0, false);
			return V_0.ToString();
		}

		private static string GetFriendlyFullGenericInstanceTypeName(this GenericInstanceType self, ILanguage language)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(self.GetFriendlyFullTypeSpecificationName(language));
			V_1 = V_0.ToString().LastIndexOf('<');
			if (V_1 >= 0 && V_1 < V_0.get_Length())
			{
				dummyVar1 = V_0.Remove(V_1, V_0.get_Length() - V_1);
				self.GenericInstanceFriendlyFullName(language, V_0, false, "<", ">");
				V_2 = 0;
				V_3 = 0;
				while (V_3 < V_0.get_Length())
				{
					if (V_0.get_Chars(V_3) != '<')
					{
						if (V_0.get_Chars(V_3) == '>')
						{
							V_2 = V_2 - 1;
						}
					}
					else
					{
						V_2 = V_2 + 1;
					}
					V_3 = V_3 + 1;
				}
				if (V_2 > 0)
				{
					dummyVar2 = V_0.Append(new String('>', V_2));
				}
			}
			return V_0.ToString();
		}

		private static string GetFriendlyFullGenericParameterName(this GenericParameter self)
		{
			return self.get_Name();
		}

		private static string GetFriendlyFullMethodReferenceName(this MethodReference self, ILanguage language)
		{
			return self.GetFriendlyFullMethodReferenceName(language, self.MemberFriendlyFullName(language), false);
		}

		private static string GetFriendlyFullMethodReferenceName(this MethodReference self, ILanguage language, string memberFriendlyFullName, bool useGenericName)
		{
			V_0 = new StringBuilder();
			V_1 = self.get_FixedReturnType().GetFriendlyFullName(language);
			if (useGenericName)
			{
				V_2 = self.get_FixedReturnType().Resolve();
				if (V_2 != null)
				{
					V_1 = V_2.GetGenericFullName(language);
				}
			}
			dummyVar0 = V_0.Append(V_1).Append(" ").Append(memberFriendlyFullName);
			self.MethodSignatureFriendlyFullName(language, V_0, useGenericName);
			return V_0.ToString();
		}

		public static string GetFriendlyFullName(this MemberReference self, ILanguage language)
		{
			if (self as ArrayType != null)
			{
				return (self as ArrayType).GetFriendlyFullArrayTypeName(language);
			}
			if (self as FunctionPointerType != null)
			{
				return (self as FunctionPointerType).GetFriendlyFullFunctionPointerTypeName(language);
			}
			if (self as GenericInstanceType != null)
			{
				return (self as GenericInstanceType).GetFriendlyFullGenericInstanceTypeName(language);
			}
			if (self as OptionalModifierType != null)
			{
				return (self as OptionalModifierType).GetFriendlyFullOptionalModifierTypeName(language);
			}
			if (self as RequiredModifierType != null)
			{
				return (self as RequiredModifierType).GetFriendlyFullRequiredModifierTypeName(language);
			}
			if (self as PointerType != null)
			{
				return (self as PointerType).GetFriendlyFullPointerTypeName(language);
			}
			if (self as ByReferenceType != null)
			{
				return (self as ByReferenceType).GetFriendlyFullByReferenceTypeName(language);
			}
			if (self as TypeSpecification != null)
			{
				return (self as TypeSpecification).GetFriendlyFullTypeSpecificationName(language);
			}
			if (self as GenericParameter != null)
			{
				return (self as GenericParameter).GetFriendlyFullGenericParameterName();
			}
			if (self as TypeReference != null)
			{
				return (self as TypeReference).GetFriendlyFullTypeReferenceName(language);
			}
			if (self as PropertyDefinition != null)
			{
				return (self as PropertyDefinition).GetFriendlyFullPropertyDefinitionName(language);
			}
			if (self as GenericInstanceMethod != null)
			{
				return (self as GenericInstanceMethod).GetFriendlyFullGenericInstanceMethodName(language);
			}
			if (self as MethodReference != null)
			{
				return (self as MethodReference).GetFriendlyFullMethodReferenceName(language);
			}
			if (self as FieldReference != null)
			{
				return (self as FieldReference).GetFriendlyFullFieldReferenceName(language);
			}
			if (self as EventReference == null)
			{
				throw new Exception("Invalid member type.");
			}
			return (self as EventReference).GetFriendlyFullEventReferenceName(language);
		}

		public static string GetFriendlyFullName(this PropertyDefinition self, ILanguage language)
		{
			return self.GetFriendlyFullPropertyDefinitionName(language);
		}

		public static string GetFriendlyFullName(this EventDefinition self, ILanguage language)
		{
			return self.GetFriendlyFullEventReferenceName(language);
		}

		public static string GetFriendlyFullName(this FieldDefinition self, ILanguage language)
		{
			return self.GetFriendlyFullFieldReferenceName(language);
		}

		public static string GetFriendlyFullName(this MethodDefinition self, ILanguage language)
		{
			return self.GetFriendlyFullMethodReferenceName(language);
		}

		public static string GetFriendlyFullName(this TypeDefinition self, ILanguage language)
		{
			return self.GetFriendlyFullTypeReferenceName(language);
		}

		public static string GetFriendlyFullName(this IMemberDefinition self, ILanguage language)
		{
			return ((MemberReference)self).GetFriendlyFullName(language);
		}

		public static string GetFriendlyFullNameInVB(this MemberReference self, ILanguage language)
		{
			return self.GetFriendlyFullName(language);
		}

		private static string GetFriendlyFullOptionalModifierTypeName(this OptionalModifierType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), self.get_Suffix());
		}

		private static string GetFriendlyFullPointerTypeName(this PointerType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), "*");
		}

		private static string GetFriendlyFullPropertyDefinitionName(this PropertyDefinition self, ILanguage language)
		{
			return self.GetFriendlyFullPropertyDefinitionName(language, self.MemberFriendlyFullName(language));
		}

		private static string GetFriendlyFullPropertyDefinitionName(this PropertyDefinition self, ILanguage language, string memberFriendlyFullName)
		{
			V_0 = new StringBuilder();
			dummyVar0 = V_0.Append(self.get_PropertyType().GetFriendlyFullName(language));
			dummyVar1 = V_0.Append(' ');
			dummyVar2 = V_0.Append(memberFriendlyFullName);
			dummyVar3 = V_0.Append('(');
			if (self.get_HasParameters())
			{
				V_1 = self.get_Parameters();
				V_2 = 0;
				while (V_2 < V_1.get_Count())
				{
					if (V_2 > 0)
					{
						dummyVar4 = V_0.Append(',');
					}
					dummyVar5 = V_0.Append(V_1.get_Item(V_2).get_ParameterType().GetFriendlyFullName(language));
					V_2 = V_2 + 1;
				}
			}
			dummyVar6 = V_0.Append(')');
			return V_0.ToString();
		}

		private static string GetFriendlyFullRequiredModifierTypeName(this RequiredModifierType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), self.get_Suffix());
		}

		private static string GetFriendlyFullTypeReferenceName(this TypeReference self, ILanguage language)
		{
			if (self.get_IsNested())
			{
				return String.Concat(self.get_DeclaringType().GetFriendlyFullName(language), "/", self.GetGenericName(language, "<", ">"));
			}
			if (String.IsNullOrEmpty(self.get_Namespace()))
			{
				return self.GetGenericName(language, "<", ">");
			}
			return String.Concat(self.get_Namespace(), ".", self.GetGenericName(language, "<", ">"));
		}

		private static string GetFriendlyFullTypeSpecificationName(this TypeSpecification self, ILanguage language)
		{
			return self.get_ElementType().GetFriendlyFullName(language);
		}

		private static string GetFriendlyGenericInstanceName(this GenericInstanceType self, ILanguage language, string leftBracket, string rightbracket)
		{
			V_0 = new StringBuilder();
			V_1 = GenericHelper.GetNonGenericName(self.get_Name());
			dummyVar0 = V_0.Append(GenericHelper.ReplaceInvalidCharactersName(language, V_1));
			if (language != null && String.op_Equality(language.get_Name(), "IL"))
			{
				V_2 = self.get_GenericArguments().get_Count();
				V_1 = String.Concat(V_1, "`", V_2.ToString());
				return V_1;
			}
			dummyVar1 = V_0.Append(leftBracket);
			V_3 = 0;
			while (V_3 < self.get_GenericArguments().get_Count())
			{
				if (V_3 > 0)
				{
					dummyVar2 = V_0.Append(", ");
				}
				dummyVar3 = V_0.Append(self.get_GenericArguments().get_Item(V_3).GetGenericName(language, leftBracket, rightbracket));
				V_3 = V_3 + 1;
			}
			dummyVar4 = V_0.Append(rightbracket);
			return V_0.ToString();
		}

		public static string GetFriendlyMemberName(this IMemberDefinition self, ILanguage language)
		{
			return self.GetFriendlyMemberName(language, "<", ">");
		}

		public static string GetFriendlyMemberName(this IMemberDefinition self, ILanguage language, string leftBracket, string rightBracket)
		{
			if (self as IGenericDefinition == null)
			{
				return self.GetFullMemberName(language);
			}
			return ((IGenericDefinition)self).GetGenericName(language, leftBracket, rightBracket);
		}

		public static string GetFriendlyMemberNameInVB(this IMemberDefinition self, ILanguage language)
		{
			return self.GetFriendlyMemberName(language, "(Of ", ")");
		}

		public static string GetFriendlyTypeName(this TypeReference self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
		{
			if (!self.get_IsNested())
			{
				return self.GetGenericName(language, leftBracket, rightBracket);
			}
			if (!self.get_Name().StartsWith("<>"))
			{
				return self.GetGenericName(language, leftBracket, rightBracket);
			}
			V_0 = self.GetFriendlyFullName(language).IndexOf('/');
			if (V_0 <= 0)
			{
				return self.GetGenericName(language, leftBracket, rightBracket);
			}
			V_1 = self.GetFriendlyFullName(language).Substring(0, V_0);
			V_2 = V_1.LastIndexOf('.');
			if (V_2 <= 0)
			{
				return V_1;
			}
			return V_1.Substring(V_2 + 1);
		}

		public static string GetFullMemberName(this IMemberDefinition self, ILanguage language)
		{
			if (self as TypeDefinition != null || self as FieldDefinition != null)
			{
				return GenericHelper.ReplaceInvalidCharactersName(language, self.get_Name());
			}
			if (self as EventDefinition != null)
			{
				V_0 = self as EventDefinition;
				if (!V_0.IsExplicitImplementation())
				{
					return GenericHelper.ReplaceInvalidCharactersName(language, self.get_Name());
				}
				stackVariable85 = V_0.get_Name();
				stackVariable87 = new Char[1];
				stackVariable87[0] = '.';
				V_1 = stackVariable85.Split(stackVariable87);
				V_2 = new StringBuilder((int)V_1.Length * 2);
				V_3 = 0;
				while (V_3 < (int)V_1.Length)
				{
					V_4 = V_1[V_3];
					dummyVar0 = V_2.Append(GenericHelper.ReplaceInvalidCharactersName(language, V_4));
					if (V_3 < (int)V_1.Length - 1)
					{
						dummyVar1 = V_2.Append(".");
					}
					V_3 = V_3 + 1;
				}
				return V_2.ToString();
			}
			if (self as MethodDefinition != null)
			{
				return (self as MethodDefinition).GetFriendlyFullMethodReferenceName(language, self.get_Name(), false);
			}
			if (self as PropertyDefinition == null)
			{
				throw new Exception("Invalid member definition type.");
			}
			V_5 = self as PropertyDefinition;
			if (!V_5.IsExplicitImplementation())
			{
				return (self as PropertyDefinition).GetFriendlyFullPropertyDefinitionName(language, self.get_Name());
			}
			stackVariable27 = V_5.get_Name();
			stackVariable29 = new Char[1];
			stackVariable29[0] = '.';
			V_6 = stackVariable27.Split(stackVariable29);
			V_7 = new StringBuilder((int)V_6.Length * 2);
			V_8 = 0;
			while (V_8 < (int)V_6.Length)
			{
				V_9 = V_6[V_8];
				dummyVar2 = V_7.Append(GenericHelper.ReplaceInvalidCharactersName(language, V_9));
				if (V_8 < (int)V_6.Length - 1)
				{
					dummyVar3 = V_7.Append(".");
				}
				V_8 = V_8 + 1;
			}
			dummyVar4 = V_7.Append("()");
			return V_7.ToString();
		}

		public static string GetGenericFullName(this IGenericDefinition self, ILanguage language)
		{
			if (self as TypeDefinition != null)
			{
				return (self as TypeDefinition).GetGenericFullTypeDefinitionName(language);
			}
			if (self as MethodDefinition == null)
			{
				throw new Exception("Invalid generic member definition type.");
			}
			V_0 = self as MethodDefinition;
			return V_0.GetFriendlyFullMethodReferenceName(language, V_0.GenericMemberFullName(language), true);
		}

		private static string GetGenericFullTypeDefinitionName(this TypeDefinition self, ILanguage language)
		{
			V_0 = self.GetGenericName(language, "<", ">");
			if (self.get_IsNested())
			{
				return String.Concat(self.get_DeclaringType().GetGenericFullName(language), "/", V_0);
			}
			if (String.IsNullOrEmpty(self.get_Namespace()))
			{
				return V_0;
			}
			return String.Concat(self.get_Namespace(), ".", V_0);
		}

		public static string GetGenericName(this IGenericDefinition self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
		{
			if (self as TypeDefinition == null && self as MethodDefinition == null)
			{
				throw new Exception("Invalid generic member definition type.");
			}
			return GenericHelper.GetGenericName(self, leftBracket, rightBracket, language);
		}

		public static string GetGenericName(this TypeReference self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
		{
			if (self as GenericInstanceType == null)
			{
				return GenericHelper.GetGenericName(self, leftBracket, rightBracket, language);
			}
			return (self as GenericInstanceType).GetFriendlyGenericInstanceName(language, leftBracket, rightBracket);
		}

		public static string GetUIFriendlyTypeNameInVB(this TypeReference self, ILanguage language)
		{
			return self.GetFriendlyTypeName(language, "(Of ", ")").Replace("[]", "()");
		}

		private static string MemberFriendlyFullName(this MemberReference self, ILanguage language)
		{
			if (self.get_DeclaringType() == null)
			{
				return GenericHelper.ReplaceInvalidCharactersName(language, self.get_Name());
			}
			return String.Concat(self.get_DeclaringType().GetFriendlyFullName(language), "::", self.get_Name());
		}

		private static void MethodSignatureFriendlyFullName(this IMethodSignature self, ILanguage language, StringBuilder builder, bool useGenericName)
		{
			dummyVar0 = builder.Append("(");
			if (self.get_HasParameters())
			{
				V_0 = self.get_Parameters();
				V_1 = 0;
				while (V_1 < V_0.get_Count())
				{
					V_2 = V_0.get_Item(V_1);
					if (V_1 > 0)
					{
						dummyVar1 = builder.Append(",");
					}
					if (V_2.get_ParameterType().get_IsSentinel())
					{
						dummyVar2 = builder.Append("...,");
					}
					if (useGenericName)
					{
						V_3 = V_2.get_ParameterType().Resolve();
						if (V_3 == null)
						{
							goto Label1;
						}
						dummyVar3 = builder.Append(V_3.GetGenericFullName(language));
						goto Label0;
					}
				Label1:
					dummyVar4 = builder.Append(V_2.get_ParameterType().GetFriendlyFullName(language));
				Label0:
					V_1 = V_1 + 1;
				}
			}
			dummyVar5 = builder.Append(")");
			return;
		}
	}
}