using Mono.Collections.Generic;
using System;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil
{
	public static class GenericHelper
	{
		public static string GetGenericName(IGenericDefinition genericDefinition, string leftBracket, string rightBracket, ILanguage language)
		{
			bool hasOverrides = false;
			if (genericDefinition is MethodDefinition)
			{
				hasOverrides = ((MethodDefinition)genericDefinition).get_HasOverrides();
			}
			string nonGenericName = GenericHelper.GetNonGenericName(genericDefinition.get_Name());
			if (!hasOverrides)
			{
				if ((!(genericDefinition is MethodDefinition) ? true : !((MethodDefinition)genericDefinition).get_IsConstructor()))
				{
					nonGenericName = GenericHelper.ReplaceInvalidCharactersName(language, nonGenericName);
				}
			}
			if (genericDefinition.get_HasGenericParameters())
			{
				int count = 0;
				if (genericDefinition is TypeDefinition)
				{
					TypeDefinition declaringType = (genericDefinition as TypeDefinition).get_DeclaringType();
					if (declaringType != null && declaringType.get_HasGenericParameters())
					{
						count = declaringType.get_GenericParameters().get_Count();
					}
				}
				if (count < genericDefinition.get_GenericParameters().get_Count())
				{
					nonGenericName = String.Concat(nonGenericName, leftBracket);
					while (count < genericDefinition.get_GenericParameters().get_Count())
					{
						GenericParameter item = genericDefinition.get_GenericParameters().get_Item(count);
						if (item.get_IsCovariant())
						{
							nonGenericName = String.Concat(nonGenericName, "out ");
						}
						if (item.get_IsContravariant())
						{
							nonGenericName = String.Concat(nonGenericName, "in ");
						}
						nonGenericName = String.Concat(nonGenericName, GenericHelper.ReplaceInvalidCharactersName(language, item.get_Name()));
						if (count != genericDefinition.get_GenericParameters().get_Count() - 1)
						{
							nonGenericName = String.Concat(nonGenericName, ", ");
						}
						count++;
					}
					nonGenericName = String.Concat(nonGenericName, rightBracket);
				}
			}
			return nonGenericName;
		}

		internal static string GetGenericName(TypeReference genericDefinition, string leftBracket, string rightBracket, ILanguage language)
		{
			string name = genericDefinition.get_Name();
			if (genericDefinition.get_HasGenericParameters())
			{
				name = GenericHelper.GetNonGenericName(genericDefinition.get_Name());
				if (!genericDefinition.get_IsPointer() && !genericDefinition.get_IsByReference() && !genericDefinition.get_IsArray())
				{
					name = GenericHelper.ReplaceInvalidCharactersName(language, name);
				}
				if (language != null && language.Name == "IL")
				{
					int count = genericDefinition.get_GenericParameters().get_Count();
					name = String.Concat(name, "`", count.ToString());
					return name;
				}
				name = String.Concat(name, leftBracket);
				for (int i = 0; i < genericDefinition.get_GenericParameters().get_Count(); i++)
				{
					GenericParameter item = genericDefinition.get_GenericParameters().get_Item(i);
					if (item.get_IsCovariant())
					{
						name = String.Concat(name, "out ");
					}
					if (item.get_IsContravariant())
					{
						name = String.Concat(name, "in ");
					}
					name = String.Concat(name, GenericHelper.ReplaceInvalidCharactersName(language, item.get_Name()));
					if (i != genericDefinition.get_GenericParameters().get_Count() - 1)
					{
						name = String.Concat(name, ", ");
					}
				}
				name = String.Concat(name, rightBracket);
			}
			else if (!genericDefinition.get_IsPointer() && !genericDefinition.get_IsByReference() && !genericDefinition.get_IsArray())
			{
				name = GenericHelper.ReplaceInvalidCharactersName(language, name);
			}
			return name;
		}

		public static string GetNonGenericName(string name)
		{
			int num = name.IndexOf('\u0060');
			if (num > 0)
			{
				name = name.Substring(0, num);
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