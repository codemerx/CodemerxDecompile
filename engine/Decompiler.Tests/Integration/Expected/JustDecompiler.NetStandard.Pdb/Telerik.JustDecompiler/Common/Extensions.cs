using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Common
{
	public static class Extensions
	{
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> source)
		{
			V_0 = source.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					self.Add(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public static bool IsValidIdentifier(this string self)
		{
			if (self == null || String.op_Equality(self, String.Empty))
			{
				return false;
			}
			try
			{
				V_0 = self.Normalize();
				goto Label0;
			}
			catch (ArgumentException exception_0)
			{
				dummyVar0 = exception_0;
				V_1 = false;
			}
			return V_1;
		Label0:
			if (!V_0.get_Chars(0).IsValidIdentifierFirstCharacter())
			{
				return false;
			}
			V_2 = 1;
			while (V_2 < V_0.get_Length())
			{
				if (!V_0.get_Chars(V_2).IsValidIdentifierCharacter())
				{
					return false;
				}
				V_2 = V_2 + 1;
			}
			return true;
		}

		public static bool IsValidIdentifierCharacter(this char currentChar)
		{
			if (currentChar.IsValidIdentifierFirstCharacter())
			{
				return true;
			}
			V_0 = Char.GetUnicodeCategory(currentChar);
			if (V_0 != 5 && V_0 != 6 && V_0 != 8 && V_0 != 18 && V_0 != 15)
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
			V_0 = Char.GetUnicodeCategory(firstCharacter);
			if (V_0 != 1 && V_0 != UnicodeCategory.UppercaseLetter && V_0 != 2 && V_0 != 3 && V_0 != 4 && V_0 != 9)
			{
				return false;
			}
			return true;
		}

		public static bool? ResolveToOverloadedEqualityOperator(TypeReference type, out TypeReference lastResolvedType)
		{
			if (type.get_IsValueType() || type.get_IsFunctionPointer() || type.get_IsPrimitive() || type.get_IsGenericParameter())
			{
				throw new NotSupportedException();
			}
			lastResolvedType = type;
			V_0 = type.Resolve();
			if (V_0 == null)
			{
				V_1 = null;
				return V_1;
			}
			if (V_0.get_IsInterface())
			{
				return new bool?(false);
			}
			while (String.op_Inequality(V_0.get_Name(), "Object"))
			{
				stackVariable21 = V_0.get_Methods();
				stackVariable22 = Telerik.JustDecompiler.Common.Extensions.u003cu003ec.u003cu003e9__4_0;
				if (stackVariable22 == null)
				{
					dummyVar0 = stackVariable22;
					stackVariable22 = new Func<MethodDefinition, bool>(Telerik.JustDecompiler.Common.Extensions.u003cu003ec.u003cu003e9.u003cResolveToOverloadedEqualityOperatoru003eb__4_0);
					Telerik.JustDecompiler.Common.Extensions.u003cu003ec.u003cu003e9__4_0 = stackVariable22;
				}
				if (stackVariable21.Any<MethodDefinition>(stackVariable22))
				{
					return new bool?(true);
				}
				lastResolvedType = V_0.get_BaseType();
				V_2 = V_0.get_BaseType().Resolve();
				if (V_2 == null)
				{
					V_1 = null;
					return V_1;
				}
				V_0 = V_2;
			}
			return new bool?(false);
		}

		public static string ToString(this FrameworkVersion self, bool includeVersionSign)
		{
			V_0 = String.Empty;
			switch (self - 1)
			{
				case 0:
				{
					V_0 = "1.0";
					break;
				}
				case 1:
				case 24:
				{
				Label0:
					return String.Empty;
				}
				case 2:
				{
					V_0 = "2.0";
					break;
				}
				case 3:
				{
					V_0 = "3.0";
					break;
				}
				case 4:
				{
					V_0 = "3.5";
					break;
				}
				case 5:
				{
					V_0 = "4.0";
					break;
				}
				case 6:
				{
					V_0 = "4.5";
					break;
				}
				case 7:
				{
					V_0 = "4.5.1";
					break;
				}
				case 8:
				{
					V_0 = "4.5.2";
					break;
				}
				case 9:
				{
					V_0 = "4.6";
					break;
				}
				case 10:
				{
					V_0 = "4.6.1";
					break;
				}
				case 11:
				{
					V_0 = "4.6.2";
					break;
				}
				case 12:
				{
					V_0 = "4.7";
					break;
				}
				case 13:
				{
					V_0 = "4.7.1";
					break;
				}
				case 14:
				case 15:
				case 16:
				{
					return self.ToString();
				}
				case 17:
				{
					V_0 = ".NETPortable v4.0";
					break;
				}
				case 18:
				{
					V_0 = ".NETPortable v4.6";
					break;
				}
				case 19:
				{
					V_0 = ".NETPortable v4.5";
					break;
				}
				case 20:
				{
					V_0 = ".NETPortable v5.0";
					break;
				}
				case 21:
				{
					V_0 = "WinRT - 4.5";
					break;
				}
				case 22:
				{
					V_0 = "WinRT - 4.5.1";
					break;
				}
				case 23:
				{
					V_0 = "UWP";
					break;
				}
				case 25:
				{
					V_0 = "netcoreapp2.1";
					break;
				}
				case 26:
				{
					V_0 = "netcoreapp2.0";
					break;
				}
				case 27:
				{
					V_0 = "netcoreapp1.1";
					break;
				}
				case 28:
				{
					V_0 = "netcoreapp1.0";
					break;
				}
				default:
				{
					goto Label0;
				}
			}
			if (!includeVersionSign)
			{
				return V_0;
			}
			return String.Concat("v", V_0);
		}
	}
}