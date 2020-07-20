using System;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil
{
	public static class GenericHelper
	{
		public static string GetGenericName(IGenericDefinition genericDefinition, string leftBracket, string rightBracket, ILanguage language)
		{
			V_0 = false;
			if (genericDefinition as MethodDefinition != null)
			{
				V_0 = ((MethodDefinition)genericDefinition).get_HasOverrides();
			}
			V_1 = GenericHelper.GetNonGenericName(genericDefinition.get_Name());
			if (!V_0)
			{
				if (genericDefinition as MethodDefinition == null)
				{
					stackVariable70 = false;
				}
				else
				{
					stackVariable70 = ((MethodDefinition)genericDefinition).get_IsConstructor();
				}
				if (!stackVariable70)
				{
					V_1 = GenericHelper.ReplaceInvalidCharactersName(language, V_1);
				}
			}
			if (genericDefinition.get_HasGenericParameters())
			{
				V_2 = 0;
				if (genericDefinition as TypeDefinition != null)
				{
					V_3 = (genericDefinition as TypeDefinition).get_DeclaringType();
					if (V_3 != null && V_3.get_HasGenericParameters())
					{
						V_2 = V_3.get_GenericParameters().get_Count();
					}
				}
				if (V_2 < genericDefinition.get_GenericParameters().get_Count())
				{
					V_1 = String.Concat(V_1, leftBracket);
					while (V_2 < genericDefinition.get_GenericParameters().get_Count())
					{
						V_4 = genericDefinition.get_GenericParameters().get_Item(V_2);
						if (V_4.get_IsCovariant())
						{
							V_1 = String.Concat(V_1, "out ");
						}
						if (V_4.get_IsContravariant())
						{
							V_1 = String.Concat(V_1, "in ");
						}
						V_1 = String.Concat(V_1, GenericHelper.ReplaceInvalidCharactersName(language, V_4.get_Name()));
						if (V_2 != genericDefinition.get_GenericParameters().get_Count() - 1)
						{
							V_1 = String.Concat(V_1, ", ");
						}
						V_2 = V_2 + 1;
					}
					V_1 = String.Concat(V_1, rightBracket);
				}
			}
			return V_1;
		}

		internal static string GetGenericName(TypeReference genericDefinition, string leftBracket, string rightBracket, ILanguage language)
		{
			V_0 = genericDefinition.get_Name();
			if (!genericDefinition.get_HasGenericParameters())
			{
				if (!genericDefinition.get_IsPointer() && !genericDefinition.get_IsByReference() && !genericDefinition.get_IsArray())
				{
					V_0 = GenericHelper.ReplaceInvalidCharactersName(language, V_0);
				}
			}
			else
			{
				V_0 = GenericHelper.GetNonGenericName(genericDefinition.get_Name());
				if (!genericDefinition.get_IsPointer() && !genericDefinition.get_IsByReference() && !genericDefinition.get_IsArray())
				{
					V_0 = GenericHelper.ReplaceInvalidCharactersName(language, V_0);
				}
				if (language != null && String.op_Equality(language.get_Name(), "IL"))
				{
					V_1 = genericDefinition.get_GenericParameters().get_Count();
					V_0 = String.Concat(V_0, "`", V_1.ToString());
					return V_0;
				}
				V_0 = String.Concat(V_0, leftBracket);
				V_2 = 0;
				while (V_2 < genericDefinition.get_GenericParameters().get_Count())
				{
					V_3 = genericDefinition.get_GenericParameters().get_Item(V_2);
					if (V_3.get_IsCovariant())
					{
						V_0 = String.Concat(V_0, "out ");
					}
					if (V_3.get_IsContravariant())
					{
						V_0 = String.Concat(V_0, "in ");
					}
					V_0 = String.Concat(V_0, GenericHelper.ReplaceInvalidCharactersName(language, V_3.get_Name()));
					if (V_2 != genericDefinition.get_GenericParameters().get_Count() - 1)
					{
						V_0 = String.Concat(V_0, ", ");
					}
					V_2 = V_2 + 1;
				}
				V_0 = String.Concat(V_0, rightBracket);
			}
			return V_0;
		}

		public static string GetNonGenericName(string name)
		{
			V_0 = name.IndexOf('\u0060');
			if (V_0 > 0)
			{
				name = name.Substring(0, V_0);
			}
			return name;
		}

		public static string ReplaceInvalidCharactersName(ILanguage language, string memberName)
		{
			if (language == null)
			{
				return memberName;
			}
			if (language.IsValidIdentifier(memberName))
			{
				return memberName;
			}
			return language.ReplaceInvalidCharactersInIdentifier(memberName);
		}
	}
}