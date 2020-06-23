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
			if (type.IsValueType || type.IsFunctionPointer || type.IsPrimitive || type.IsGenericParameter)
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
			if (typeDefinition.IsInterface)
			{
				return new bool?(false);
			}
			while (typeDefinition.Name != "Object")
			{
				if (typeDefinition.Methods.Any<MethodDefinition>((MethodDefinition m) => {
					if (m.Name == "op_Equality")
					{
						return true;
					}
					return m.Name == "op_Inequality";
				}))
				{
					return new bool?(true);
				}
				lastResolvedType = typeDefinition.BaseType;
				TypeDefinition typeDefinition1 = typeDefinition.BaseType.Resolve();
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
				case FrameworkVersion.v1_0:
				{
					empty = "1.0";
					break;
				}
				case FrameworkVersion.v1_1:
				case FrameworkVersion.WinRT_System:
				{
					return String.Empty;
				}
				case FrameworkVersion.v2_0:
				{
					empty = "2.0";
					break;
				}
				case FrameworkVersion.v3_0:
				{
					empty = "3.0";
					break;
				}
				case FrameworkVersion.v3_5:
				{
					empty = "3.5";
					break;
				}
				case FrameworkVersion.v4_0:
				{
					empty = "4.0";
					break;
				}
				case FrameworkVersion.v4_5:
				{
					empty = "4.5";
					break;
				}
				case FrameworkVersion.v4_5_1:
				{
					empty = "4.5.1";
					break;
				}
				case FrameworkVersion.v4_5_2:
				{
					empty = "4.5.2";
					break;
				}
				case FrameworkVersion.v4_6:
				{
					empty = "4.6";
					break;
				}
				case FrameworkVersion.v4_6_1:
				{
					empty = "4.6.1";
					break;
				}
				case FrameworkVersion.v4_6_2:
				{
					empty = "4.6.2";
					break;
				}
				case FrameworkVersion.v4_7:
				{
					empty = "4.7";
					break;
				}
				case FrameworkVersion.v4_7_1:
				{
					empty = "4.7.1";
					break;
				}
				case FrameworkVersion.Silverlight:
				case FrameworkVersion.WindowsPhone:
				case FrameworkVersion.WindowsCE:
				{
					return self.ToString();
				}
				case FrameworkVersion.NetPortableV4_0:
				{
					empty = ".NETPortable v4.0";
					break;
				}
				case FrameworkVersion.NetPortableV4_6:
				{
					empty = ".NETPortable v4.6";
					break;
				}
				case FrameworkVersion.NetPortableV4_5:
				{
					empty = ".NETPortable v4.5";
					break;
				}
				case FrameworkVersion.NetPortableV5_0:
				{
					empty = ".NETPortable v5.0";
					break;
				}
				case FrameworkVersion.WinRT_4_5:
				{
					empty = "WinRT - 4.5";
					break;
				}
				case FrameworkVersion.WinRT_4_5_1:
				{
					empty = "WinRT - 4.5.1";
					break;
				}
				case FrameworkVersion.UWP:
				{
					empty = "UWP";
					break;
				}
				case FrameworkVersion.NetCoreV2_1:
				{
					empty = "netcoreapp2.1";
					break;
				}
				case FrameworkVersion.NetCoreV2_0:
				{
					empty = "netcoreapp2.0";
					break;
				}
				case FrameworkVersion.NetCoreV1_1:
				{
					empty = "netcoreapp1.1";
					break;
				}
				case FrameworkVersion.NetCoreV1_0:
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