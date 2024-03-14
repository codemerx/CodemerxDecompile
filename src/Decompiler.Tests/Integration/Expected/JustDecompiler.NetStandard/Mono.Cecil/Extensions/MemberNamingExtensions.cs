using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil.Extensions
{
	public static class MemberNamingExtensions
	{
		private static void GenericInstanceFriendlyFullName(this IGenericInstance self, ILanguage language, StringBuilder builder, bool useGenericName, string leftBracket, string rightBracket)
		{
			builder.Append(leftBracket);
			Collection<TypeReference> genericArguments = self.get_GenericArguments();
			for (int i = 0; i < genericArguments.get_Count(); i++)
			{
				TypeReference item = genericArguments.get_Item(i);
				if (self.get_PostionToArgument().ContainsKey(i))
				{
					item = self.get_PostionToArgument()[i];
				}
				if (i > 0)
				{
					builder.Append(",");
				}
				string friendlyFullName = item.GetFriendlyFullName(language);
				if (useGenericName)
				{
					TypeDefinition typeDefinition = item.Resolve();
					if (typeDefinition != null)
					{
						friendlyFullName = typeDefinition.GetGenericFullName(language);
					}
				}
				builder.Append(friendlyFullName);
			}
			builder.Append(rightBracket);
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
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(self.get_Function().get_Name());
			stringBuilder.Append(" ");
			stringBuilder.Append(self.get_Function().get_FixedReturnType().GetFriendlyFullName(language));
			stringBuilder.Append(" *");
			self.MethodSignatureFriendlyFullName(language, stringBuilder, false);
			return stringBuilder.ToString();
		}

		private static string GetFriendlyFullGenericInstanceMethodName(this GenericInstanceMethod self, ILanguage language)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(self.get_FixedReturnType().get_FullName()).Append(" ").Append(self.MemberFriendlyFullName(language));
			self.GenericInstanceFriendlyFullName(language, stringBuilder, false, "<", ">");
			self.MethodSignatureFriendlyFullName(language, stringBuilder, false);
			return stringBuilder.ToString();
		}

		private static string GetFriendlyFullGenericInstanceTypeName(this GenericInstanceType self, ILanguage language)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(self.GetFriendlyFullTypeSpecificationName(language));
			int num = stringBuilder.ToString().LastIndexOf('<');
			if (num >= 0 && num < stringBuilder.Length)
			{
				stringBuilder.Remove(num, stringBuilder.Length - num);
				self.GenericInstanceFriendlyFullName(language, stringBuilder, false, "<", ">");
				int num1 = 0;
				for (int i = 0; i < stringBuilder.Length; i++)
				{
					if (stringBuilder[i] == '<')
					{
						num1++;
					}
					else if (stringBuilder[i] == '>')
					{
						num1--;
					}
				}
				if (num1 > 0)
				{
					stringBuilder.Append(new String('>', num1));
				}
			}
			return stringBuilder.ToString();
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
			StringBuilder stringBuilder = new StringBuilder();
			string friendlyFullName = self.get_FixedReturnType().GetFriendlyFullName(language);
			if (useGenericName)
			{
				TypeDefinition typeDefinition = self.get_FixedReturnType().Resolve();
				if (typeDefinition != null)
				{
					friendlyFullName = typeDefinition.GetGenericFullName(language);
				}
			}
			stringBuilder.Append(friendlyFullName).Append(" ").Append(memberFriendlyFullName);
			self.MethodSignatureFriendlyFullName(language, stringBuilder, useGenericName);
			return stringBuilder.ToString();
		}

		public static string GetFriendlyFullName(this MemberReference self, ILanguage language)
		{
			if (self is ArrayType)
			{
				return (self as ArrayType).GetFriendlyFullArrayTypeName(language);
			}
			if (self is FunctionPointerType)
			{
				return (self as FunctionPointerType).GetFriendlyFullFunctionPointerTypeName(language);
			}
			if (self is GenericInstanceType)
			{
				return (self as GenericInstanceType).GetFriendlyFullGenericInstanceTypeName(language);
			}
			if (self is OptionalModifierType)
			{
				return (self as OptionalModifierType).GetFriendlyFullOptionalModifierTypeName(language);
			}
			if (self is RequiredModifierType)
			{
				return (self as RequiredModifierType).GetFriendlyFullRequiredModifierTypeName(language);
			}
			if (self is PointerType)
			{
				return (self as PointerType).GetFriendlyFullPointerTypeName(language);
			}
			if (self is ByReferenceType)
			{
				return (self as ByReferenceType).GetFriendlyFullByReferenceTypeName(language);
			}
			if (self is TypeSpecification)
			{
				return (self as TypeSpecification).GetFriendlyFullTypeSpecificationName(language);
			}
			if (self is GenericParameter)
			{
				return (self as GenericParameter).GetFriendlyFullGenericParameterName();
			}
			if (self is TypeReference)
			{
				return (self as TypeReference).GetFriendlyFullTypeReferenceName(language);
			}
			if (self is PropertyDefinition)
			{
				return (self as PropertyDefinition).GetFriendlyFullPropertyDefinitionName(language);
			}
			if (self is GenericInstanceMethod)
			{
				return (self as GenericInstanceMethod).GetFriendlyFullGenericInstanceMethodName(language);
			}
			if (self is MethodReference)
			{
				return (self as MethodReference).GetFriendlyFullMethodReferenceName(language);
			}
			if (self is FieldReference)
			{
				return (self as FieldReference).GetFriendlyFullFieldReferenceName(language);
			}
			if (!(self is EventReference))
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
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(self.get_PropertyType().GetFriendlyFullName(language));
			stringBuilder.Append(' ');
			stringBuilder.Append(memberFriendlyFullName);
			stringBuilder.Append('(');
			if (self.get_HasParameters())
			{
				Collection<ParameterDefinition> parameters = self.get_Parameters();
				for (int i = 0; i < parameters.get_Count(); i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(parameters.get_Item(i).get_ParameterType().GetFriendlyFullName(language));
				}
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
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
			StringBuilder stringBuilder = new StringBuilder();
			string nonGenericName = GenericHelper.GetNonGenericName(self.get_Name());
			stringBuilder.Append(GenericHelper.ReplaceInvalidCharactersName(language, nonGenericName));
			if (language != null && language.Name == "IL")
			{
				int count = self.get_GenericArguments().get_Count();
				nonGenericName = String.Concat(nonGenericName, "`", count.ToString());
				return nonGenericName;
			}
			stringBuilder.Append(leftBracket);
			for (int i = 0; i < self.get_GenericArguments().get_Count(); i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(self.get_GenericArguments().get_Item(i).GetGenericName(language, leftBracket, rightbracket));
			}
			stringBuilder.Append(rightbracket);
			return stringBuilder.ToString();
		}

		public static string GetFriendlyMemberName(this IMemberDefinition self, ILanguage language)
		{
			return self.GetFriendlyMemberName(language, "<", ">");
		}

		public static string GetFriendlyMemberName(this IMemberDefinition self, ILanguage language, string leftBracket, string rightBracket)
		{
			if (!(self is IGenericDefinition))
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
			int num = self.GetFriendlyFullName(language).IndexOf('/');
			if (num <= 0)
			{
				return self.GetGenericName(language, leftBracket, rightBracket);
			}
			string str = self.GetFriendlyFullName(language).Substring(0, num);
			int num1 = str.LastIndexOf('.');
			if (num1 <= 0)
			{
				return str;
			}
			return str.Substring(num1 + 1);
		}

		public static string GetFullMemberName(this IMemberDefinition self, ILanguage language)
		{
			if (self is TypeDefinition || self is FieldDefinition)
			{
				return GenericHelper.ReplaceInvalidCharactersName(language, self.get_Name());
			}
			if (self is EventDefinition)
			{
				EventDefinition eventDefinition = self as EventDefinition;
				if (!eventDefinition.IsExplicitImplementation())
				{
					return GenericHelper.ReplaceInvalidCharactersName(language, self.get_Name());
				}
				string[] strArray = eventDefinition.get_Name().Split(new Char[] { '.' });
				StringBuilder stringBuilder = new StringBuilder((int)strArray.Length * 2);
				for (int i = 0; i < (int)strArray.Length; i++)
				{
					string str = strArray[i];
					stringBuilder.Append(GenericHelper.ReplaceInvalidCharactersName(language, str));
					if (i < (int)strArray.Length - 1)
					{
						stringBuilder.Append(".");
					}
				}
				return stringBuilder.ToString();
			}
			if (self is MethodDefinition)
			{
				return (self as MethodDefinition).GetFriendlyFullMethodReferenceName(language, self.get_Name(), false);
			}
			if (!(self is PropertyDefinition))
			{
				throw new Exception("Invalid member definition type.");
			}
			PropertyDefinition propertyDefinition = self as PropertyDefinition;
			if (!propertyDefinition.IsExplicitImplementation())
			{
				return (self as PropertyDefinition).GetFriendlyFullPropertyDefinitionName(language, self.get_Name());
			}
			string[] strArray1 = propertyDefinition.get_Name().Split(new Char[] { '.' });
			StringBuilder stringBuilder1 = new StringBuilder((int)strArray1.Length * 2);
			for (int j = 0; j < (int)strArray1.Length; j++)
			{
				string str1 = strArray1[j];
				stringBuilder1.Append(GenericHelper.ReplaceInvalidCharactersName(language, str1));
				if (j < (int)strArray1.Length - 1)
				{
					stringBuilder1.Append(".");
				}
			}
			stringBuilder1.Append("()");
			return stringBuilder1.ToString();
		}

		public static string GetGenericFullName(this IGenericDefinition self, ILanguage language)
		{
			if (self is TypeDefinition)
			{
				return (self as TypeDefinition).GetGenericFullTypeDefinitionName(language);
			}
			if (!(self is MethodDefinition))
			{
				throw new Exception("Invalid generic member definition type.");
			}
			MethodDefinition methodDefinition = self as MethodDefinition;
			return methodDefinition.GetFriendlyFullMethodReferenceName(language, methodDefinition.GenericMemberFullName(language), true);
		}

		private static string GetGenericFullTypeDefinitionName(this TypeDefinition self, ILanguage language)
		{
			string genericName = self.GetGenericName(language, "<", ">");
			if (self.get_IsNested())
			{
				return String.Concat(self.get_DeclaringType().GetGenericFullName(language), "/", genericName);
			}
			if (String.IsNullOrEmpty(self.get_Namespace()))
			{
				return genericName;
			}
			return String.Concat(self.get_Namespace(), ".", genericName);
		}

		public static string GetGenericName(this IGenericDefinition self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
		{
			if (!(self is TypeDefinition) && !(self is MethodDefinition))
			{
				throw new Exception("Invalid generic member definition type.");
			}
			return GenericHelper.GetGenericName(self, leftBracket, rightBracket, language);
		}

		public static string GetGenericName(this TypeReference self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
		{
			if (!(self is GenericInstanceType))
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
			builder.Append("(");
			if (self.get_HasParameters())
			{
				Collection<ParameterDefinition> parameters = self.get_Parameters();
				for (int i = 0; i < parameters.get_Count(); i++)
				{
					ParameterDefinition item = parameters.get_Item(i);
					if (i > 0)
					{
						builder.Append(",");
					}
					if (item.get_ParameterType().get_IsSentinel())
					{
						builder.Append("...,");
					}
					if (useGenericName)
					{
						TypeDefinition typeDefinition = item.get_ParameterType().Resolve();
						if (typeDefinition == null)
						{
							goto Label1;
						}
						builder.Append(typeDefinition.GetGenericFullName(language));
						goto Label0;
					}
				Label1:
					builder.Append(item.get_ParameterType().GetFriendlyFullName(language));
				Label0:
				}
			}
			builder.Append(")");
		}
	}
}