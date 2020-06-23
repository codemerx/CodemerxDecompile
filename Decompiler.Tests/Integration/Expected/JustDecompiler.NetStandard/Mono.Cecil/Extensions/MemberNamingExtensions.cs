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
			Collection<TypeReference> genericArguments = self.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				TypeReference item = genericArguments[i];
				if (self.PostionToArgument.ContainsKey(i))
				{
					item = self.PostionToArgument[i];
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
			if (self.DeclaringType == null)
			{
				return self.GetGenericName(language, "<", ">");
			}
			return String.Concat(self.DeclaringType.GetGenericFullName(language), "::", self.GetGenericName(language, "<", ">"));
		}

		private static string GetFriendlyFullArrayTypeName(this ArrayType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), self.Suffix);
		}

		private static string GetFriendlyFullByReferenceTypeName(this ByReferenceType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), "&");
		}

		private static string GetFriendlyFullEventReferenceName(this EventReference self, ILanguage language)
		{
			return String.Concat(self.EventType.GetFriendlyFullName(language), " ", self.MemberFriendlyFullName(language));
		}

		private static string GetFriendlyFullFieldReferenceName(this FieldReference self, ILanguage language)
		{
			return String.Concat(self.FieldType.GetFriendlyFullName(language), " ", self.MemberFriendlyFullName(language));
		}

		private static string GetFriendlyFullFunctionPointerTypeName(this FunctionPointerType self, ILanguage language)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(self.Function.Name);
			stringBuilder.Append(" ");
			stringBuilder.Append(self.Function.FixedReturnType.GetFriendlyFullName(language));
			stringBuilder.Append(" *");
			self.MethodSignatureFriendlyFullName(language, stringBuilder, false);
			return stringBuilder.ToString();
		}

		private static string GetFriendlyFullGenericInstanceMethodName(this GenericInstanceMethod self, ILanguage language)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(self.FixedReturnType.FullName).Append(" ").Append(self.MemberFriendlyFullName(language));
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
			return self.Name;
		}

		private static string GetFriendlyFullMethodReferenceName(this MethodReference self, ILanguage language)
		{
			return self.GetFriendlyFullMethodReferenceName(language, self.MemberFriendlyFullName(language), false);
		}

		private static string GetFriendlyFullMethodReferenceName(this MethodReference self, ILanguage language, string memberFriendlyFullName, bool useGenericName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string friendlyFullName = self.FixedReturnType.GetFriendlyFullName(language);
			if (useGenericName)
			{
				TypeDefinition typeDefinition = self.FixedReturnType.Resolve();
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
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), self.Suffix);
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
			stringBuilder.Append(self.PropertyType.GetFriendlyFullName(language));
			stringBuilder.Append(' ');
			stringBuilder.Append(memberFriendlyFullName);
			stringBuilder.Append('(');
			if (self.HasParameters)
			{
				Collection<ParameterDefinition> parameters = self.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(parameters[i].ParameterType.GetFriendlyFullName(language));
				}
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		private static string GetFriendlyFullRequiredModifierTypeName(this RequiredModifierType self, ILanguage language)
		{
			return String.Concat(self.GetFriendlyFullTypeSpecificationName(language), self.Suffix);
		}

		private static string GetFriendlyFullTypeReferenceName(this TypeReference self, ILanguage language)
		{
			if (self.IsNested)
			{
				return String.Concat(self.DeclaringType.GetFriendlyFullName(language), "/", self.GetGenericName(language, "<", ">"));
			}
			if (String.IsNullOrEmpty(self.Namespace))
			{
				return self.GetGenericName(language, "<", ">");
			}
			return String.Concat(self.Namespace, ".", self.GetGenericName(language, "<", ">"));
		}

		private static string GetFriendlyFullTypeSpecificationName(this TypeSpecification self, ILanguage language)
		{
			return self.ElementType.GetFriendlyFullName(language);
		}

		private static string GetFriendlyGenericInstanceName(this GenericInstanceType self, ILanguage language, string leftBracket, string rightbracket)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string nonGenericName = GenericHelper.GetNonGenericName(self.Name);
			stringBuilder.Append(GenericHelper.ReplaceInvalidCharactersName(language, nonGenericName));
			if (language != null && language.Name == "IL")
			{
				int count = self.GenericArguments.Count;
				nonGenericName = String.Concat(nonGenericName, "`", count.ToString());
				return nonGenericName;
			}
			stringBuilder.Append(leftBracket);
			for (int i = 0; i < self.GenericArguments.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(self.GenericArguments[i].GetGenericName(language, leftBracket, rightbracket));
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
			if (!self.IsNested)
			{
				return self.GetGenericName(language, leftBracket, rightBracket);
			}
			if (!self.Name.StartsWith("<>"))
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
				return GenericHelper.ReplaceInvalidCharactersName(language, self.Name);
			}
			if (self is EventDefinition)
			{
				EventDefinition eventDefinition = self as EventDefinition;
				if (!eventDefinition.IsExplicitImplementation())
				{
					return GenericHelper.ReplaceInvalidCharactersName(language, self.Name);
				}
				string[] strArray = eventDefinition.Name.Split(new Char[] { '.' });
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
				return (self as MethodDefinition).GetFriendlyFullMethodReferenceName(language, self.Name, false);
			}
			if (!(self is PropertyDefinition))
			{
				throw new Exception("Invalid member definition type.");
			}
			PropertyDefinition propertyDefinition = self as PropertyDefinition;
			if (!propertyDefinition.IsExplicitImplementation())
			{
				return (self as PropertyDefinition).GetFriendlyFullPropertyDefinitionName(language, self.Name);
			}
			string[] strArray1 = propertyDefinition.Name.Split(new Char[] { '.' });
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
			string genericName = ((IGenericDefinition)self).GetGenericName(language, "<", ">");
			if (self.IsNested)
			{
				return String.Concat(self.DeclaringType.GetGenericFullName(language), "/", genericName);
			}
			if (String.IsNullOrEmpty(self.Namespace))
			{
				return genericName;
			}
			return String.Concat(self.Namespace, ".", genericName);
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
			if (self.DeclaringType == null)
			{
				return GenericHelper.ReplaceInvalidCharactersName(language, self.Name);
			}
			return String.Concat(self.DeclaringType.GetFriendlyFullName(language), "::", self.Name);
		}

		private static void MethodSignatureFriendlyFullName(this IMethodSignature self, ILanguage language, StringBuilder builder, bool useGenericName)
		{
			builder.Append("(");
			if (self.HasParameters)
			{
				Collection<ParameterDefinition> parameters = self.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterDefinition item = parameters[i];
					if (i > 0)
					{
						builder.Append(",");
					}
					if (item.ParameterType.IsSentinel)
					{
						builder.Append("...,");
					}
					if (useGenericName)
					{
						TypeDefinition typeDefinition = item.ParameterType.Resolve();
						if (typeDefinition == null)
						{
							goto Label1;
						}
						builder.Append(typeDefinition.GetGenericFullName(language));
						goto Label0;
					}
				Label1:
					builder.Append(item.ParameterType.GetFriendlyFullName(language));
				Label0:
				}
			}
			builder.Append(")");
		}
	}
}