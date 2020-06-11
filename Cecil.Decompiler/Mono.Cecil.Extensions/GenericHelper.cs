using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil
{
    public static class GenericHelper
    {
        public static string GetGenericName(IGenericDefinition genericDefinition, string leftBracket, string rightBracket, ILanguage language)
        {
            bool isExplicitMethodOverrides = false;
            if (genericDefinition is MethodDefinition)
            {
                isExplicitMethodOverrides = ((MethodDefinition)genericDefinition).HasOverrides;
            }
            string name = GetNonGenericName(genericDefinition.Name);

            if (!isExplicitMethodOverrides)
            {
                bool isConstructor = genericDefinition is MethodDefinition && ((MethodDefinition)genericDefinition).IsConstructor;

                if (!isConstructor)
                {
                    name = ReplaceInvalidCharactersName(language, name);
                }
            }
            if (genericDefinition.HasGenericParameters)
            {
                int i = 0;
                if (genericDefinition is TypeDefinition)
                {
                    TypeDefinition nestedType = genericDefinition as TypeDefinition;
                    TypeDefinition parentType = nestedType.DeclaringType;
                    if (parentType != null && parentType.HasGenericParameters)
                    {
                        i = parentType.GenericParameters.Count;
                    }
                }
                if (i < genericDefinition.GenericParameters.Count)
                {
                    name += leftBracket;
                    for (; i < genericDefinition.GenericParameters.Count; i++)
                    {
                        GenericParameter currentGenericParameter = genericDefinition.GenericParameters[i];

                        if (currentGenericParameter.IsCovariant)
                        {
                            name += "out ";
                        }
                        if (currentGenericParameter.IsContravariant)
                        {
                            name += "in ";
                        }
                        name += ReplaceInvalidCharactersName(language, currentGenericParameter.Name);
                        if (i != genericDefinition.GenericParameters.Count - 1)
                        {
                            name += ", ";
                        }
                    }
                    name += rightBracket;
                }
            }
            return name;
        }

        internal static string GetGenericName(TypeReference genericDefinition, string leftBracket, string rightBracket, ILanguage language)
        {
            string name = genericDefinition.Name;

            if (genericDefinition.HasGenericParameters)
            {
                name = GetNonGenericName(genericDefinition.Name);

                if (!genericDefinition.IsPointer && !genericDefinition.IsByReference && !genericDefinition.IsArray)
                {
                    name = ReplaceInvalidCharactersName(language, name);
                }
                if (language != null && language.Name == "IL")
                {
                    name += "`" + genericDefinition.GenericParameters.Count.ToString();

                    return name;
                }
                name += leftBracket;
                for (int i = 0; i < genericDefinition.GenericParameters.Count; i++)
                {
                    GenericParameter currentGenericParameter = genericDefinition.GenericParameters[i];

                    if (currentGenericParameter.IsCovariant)
                    {
                        name += "out ";
                    }
                    if (currentGenericParameter.IsContravariant)
                    {
                        name += "in ";
                    }
                    name += ReplaceInvalidCharactersName(language, currentGenericParameter.Name);
                    if (i != genericDefinition.GenericParameters.Count - 1)
                    {
                        name += ", ";
                    }
                }
                name += rightBracket;
            }
            else
            {
                if (!genericDefinition.IsPointer && !genericDefinition.IsByReference && !genericDefinition.IsArray)
                {
                    name = ReplaceInvalidCharactersName(language, name);
                }
            }
            return name;
        }

        public static string ReplaceInvalidCharactersName(ILanguage language, string memberName)
        {
            if (language == null)
            {
                return memberName;
            }
            if (!language.IsValidIdentifier(memberName))
            {
                return language.ReplaceInvalidCharactersInIdentifier(memberName);
            }
            return memberName;
        }

        public static string GetNonGenericName(string name)
        {
            int index = name.IndexOf('`');
            if (index > 0)
            {
                name = name.Substring(0, index);
            }
            return name;
        }
    }
}