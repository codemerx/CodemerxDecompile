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
				hasOverrides = ((MethodDefinition)genericDefinition).HasOverrides;
			}
			string nonGenericName = GenericHelper.GetNonGenericName(genericDefinition.Name);
			if (!hasOverrides)
			{
				if ((!(genericDefinition is MethodDefinition) ? true : !((MethodDefinition)genericDefinition).IsConstructor))
				{
					nonGenericName = GenericHelper.ReplaceInvalidCharactersName(language, nonGenericName);
				}
			}
			if (genericDefinition.HasGenericParameters)
			{
				int count = 0;
				if (genericDefinition is TypeDefinition)
				{
					TypeDefinition declaringType = (genericDefinition as TypeDefinition).DeclaringType;
					if (declaringType != null && declaringType.HasGenericParameters)
					{
						count = declaringType.GenericParameters.Count;
					}
				}
				if (count < genericDefinition.GenericParameters.Count)
				{
					nonGenericName = String.Concat(nonGenericName, leftBracket);
					while (count < genericDefinition.GenericParameters.Count)
					{
						GenericParameter item = genericDefinition.GenericParameters[count];
						if (item.IsCovariant)
						{
							nonGenericName = String.Concat(nonGenericName, "out ");
						}
						if (item.IsContravariant)
						{
							nonGenericName = String.Concat(nonGenericName, "in ");
						}
						nonGenericName = String.Concat(nonGenericName, GenericHelper.ReplaceInvalidCharactersName(language, item.Name));
						if (count != genericDefinition.GenericParameters.Count - 1)
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
			string name = genericDefinition.Name;
			if (genericDefinition.HasGenericParameters)
			{
				name = GenericHelper.GetNonGenericName(genericDefinition.Name);
				if (!genericDefinition.IsPointer && !genericDefinition.IsByReference && !genericDefinition.IsArray)
				{
					name = GenericHelper.ReplaceInvalidCharactersName(language, name);
				}
				if (language != null && language.Name == "IL")
				{
					int count = genericDefinition.GenericParameters.Count;
					name = String.Concat(name, "`", count.ToString());
					return name;
				}
				name = String.Concat(name, leftBracket);
				for (int i = 0; i < genericDefinition.GenericParameters.Count; i++)
				{
					GenericParameter item = genericDefinition.GenericParameters[i];
					if (item.IsCovariant)
					{
						name = String.Concat(name, "out ");
					}
					if (item.IsContravariant)
					{
						name = String.Concat(name, "in ");
					}
					name = String.Concat(name, GenericHelper.ReplaceInvalidCharactersName(language, item.Name));
					if (i != genericDefinition.GenericParameters.Count - 1)
					{
						name = String.Concat(name, ", ");
					}
				}
				name = String.Concat(name, rightBracket);
			}
			else if (!genericDefinition.IsPointer && !genericDefinition.IsByReference && !genericDefinition.IsArray)
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