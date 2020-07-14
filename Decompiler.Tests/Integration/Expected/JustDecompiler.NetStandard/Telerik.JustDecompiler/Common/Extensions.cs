using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Common
{
	public static class Extensions
	{
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> source)
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in source)
			{
				self.Add(keyValuePair);
			}
		}

		public static bool IsValidIdentifier(this string self)
		{
			bool flag;
			if (self == null || self == String.Empty)
			{
				return false;
			}
			try
			{
				string str = self.Normalize();
				if (!str[0].IsValidIdentifierFirstCharacter())
				{
					return false;
				}
				for (int i = 1; i < str.Length; i++)
				{
					if (!str[i].IsValidIdentifierCharacter())
					{
						return false;
					}
				}
				return true;
			}
			catch (ArgumentException argumentException)
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsValidIdentifierCharacter(this char currentChar)
		{
			if (currentChar.IsValidIdentifierFirstCharacter())
			{
				return true;
			}
			UnicodeCategory unicodeCategory = Char.GetUnicodeCategory(currentChar);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark && unicodeCategory != UnicodeCategory.SpacingCombiningMark && unicodeCategory != UnicodeCategory.DecimalDigitNumber && unicodeCategory != UnicodeCategory.ConnectorPunctuation && unicodeCategory != UnicodeCategory.Format)
			{
				return false;
			}
			return true;
		}

		public static bool IsValidIdentifierFirstCharacter(this char firstCharacter)
		{
			if (firstCharacter == '\u005F')
			{
				return true;
			}
			UnicodeCategory unicodeCategory = Char.GetUnicodeCategory(firstCharacter);
			if (unicodeCategory != UnicodeCategory.LowercaseLetter && unicodeCategory != UnicodeCategory.UppercaseLetter && unicodeCategory != UnicodeCategory.TitlecaseLetter && unicodeCategory != UnicodeCategory.ModifierLetter && unicodeCategory != UnicodeCategory.OtherLetter && unicodeCategory != UnicodeCategory.LetterNumber)
			{
				return false;
			}
			return true;
		}

		public static bool? ResolveToOverloadedEqualityOperator(TypeReference type, out TypeReference lastResolvedType)
		{
			bool? nullable;
			if (type.get_IsValueType() || type.get_IsFunctionPointer() || type.get_IsPrimitive() || type.get_IsGenericParameter())
			{
				throw new NotSupportedException();
			}
			lastResolvedType = type;
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == null)
			{
				nullable = null;
				return nullable;
			}
			if (typeDefinition.get_IsInterface())
			{
				return new bool?(false);
			}
			while (typeDefinition.get_Name() != "Object")
			{
				if (typeDefinition.get_Methods().Any<MethodDefinition>((MethodDefinition m) => {
					if (m.get_Name() == "op_Equality")
					{
						return true;
					}
					return m.get_Name() == "op_Inequality";
				}))
				{
					return new bool?(true);
				}
				lastResolvedType = typeDefinition.get_BaseType();
				TypeDefinition typeDefinition1 = typeDefinition.get_BaseType().Resolve();
				if (typeDefinition1 == null)
				{
					nullable = null;
					return nullable;
				}
				typeDefinition = typeDefinition1;
			}
			return new bool?(false);
		}

		public static string ToString(this FrameworkVersion self, bool includeVersionSign)
		{
			string empty = String.Empty;
			switch (self)
			{
				case 1:
				{
					empty = "1.0";
					break;
				}
				case 2:
				case 25:
				{
					return String.Empty;
				}
				case 3:
				{
					empty = "2.0";
					break;
				}
				case 4:
				{
					empty = "3.0";
					break;
				}
				case 5:
				{
					empty = "3.5";
					break;
				}
				case 6:
				{
					empty = "4.0";
					break;
				}
				case 7:
				{
					empty = "4.5";
					break;
				}
				case 8:
				{
					empty = "4.5.1";
					break;
				}
				case 9:
				{
					empty = "4.5.2";
					break;
				}
				case 10:
				{
					empty = "4.6";
					break;
				}
				case 11:
				{
					empty = "4.6.1";
					break;
				}
				case 12:
				{
					empty = "4.6.2";
					break;
				}
				case 13:
				{
					empty = "4.7";
					break;
				}
				case 14:
				{
					empty = "4.7.1";
					break;
				}
				case 15:
				case 16:
				case 17:
				{
					return self.ToString();
				}
				case 18:
				{
					empty = ".NETPortable v4.0";
					break;
				}
				case 19:
				{
					empty = ".NETPortable v4.6";
					break;
				}
				case 20:
				{
					empty = ".NETPortable v4.5";
					break;
				}
				case 21:
				{
					empty = ".NETPortable v5.0";
					break;
				}
				case 22:
				{
					empty = "WinRT - 4.5";
					break;
				}
				case 23:
				{
					empty = "WinRT - 4.5.1";
					break;
				}
				case 24:
				{
					empty = "UWP";
					break;
				}
				case 26:
				{
					empty = "netcoreapp2.1";
					break;
				}
				case 27:
				{
					empty = "netcoreapp2.0";
					break;
				}
				case 28:
				{
					empty = "netcoreapp1.1";
					break;
				}
				case 29:
				{
					empty = "netcoreapp1.0";
					break;
				}
				default:
				{
					return String.Empty;
				}
			}
			if (!includeVersionSign)
			{
				return empty;
			}
			return String.Concat("v", empty);
		}
	}
}