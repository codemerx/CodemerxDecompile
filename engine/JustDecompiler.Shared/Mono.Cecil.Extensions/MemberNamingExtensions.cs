using System;
using System.Linq;
using System.Text;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace Mono.Cecil.Extensions
{
    public static class MemberNamingExtensions
    {
        #region MemberReference FriendlyFullName

        public static string GetFriendlyFullNameInVB(this MemberReference self, ILanguage language)
        {
            return self.GetFriendlyFullName(language);
        }

        public static string GetFriendlyMemberNameInVB(this IMemberDefinition self, ILanguage language)
        {
            return self.GetFriendlyMemberName(language, "(Of ", ")");
        }

        public static string GetUIFriendlyTypeNameInVB(this TypeReference self, ILanguage language)
        {
            string genericFriendlyName = self.GetFriendlyTypeName(language, "(Of ", ")");

            return genericFriendlyName.Replace("[]", "()");
        }

        public static string GetFriendlyFullName(this MemberReference self, ILanguage language, bool includeNamespace = true)
        {
            if (self is ArrayType)
            {
                return GetFriendlyFullArrayTypeName(self as ArrayType, language, includeNamespace);
            }
            else if (self is FunctionPointerType)
            {
                return GetFriendlyFullFunctionPointerTypeName(self as FunctionPointerType, language, includeNamespace);
            }
            else if (self is GenericInstanceType)
            {
                return GetFriendlyFullGenericInstanceTypeName(self as GenericInstanceType, language, includeNamespace);
            }
            else if (self is OptionalModifierType)
            {
                return GetFriendlyFullOptionalModifierTypeName(self as OptionalModifierType, language, includeNamespace);
            }
            else if (self is RequiredModifierType)
            {
                return GetFriendlyFullRequiredModifierTypeName(self as RequiredModifierType, language, includeNamespace);
            }
            else if (self is PointerType)
            {
                return GetFriendlyFullPointerTypeName(self as PointerType, language, includeNamespace);
            }
            else if (self is ByReferenceType)
            {
                return GetFriendlyFullByReferenceTypeName(self as ByReferenceType, language, includeNamespace);
            }
            else if (self is TypeSpecification) // + Pinned + Sentinel
            {
                return GetFriendlyFullTypeSpecificationName(self as TypeSpecification, language, includeNamespace);
            }
            else if (self is GenericParameter)
            {
                return GetFriendlyFullGenericParameterName(self as GenericParameter);
            }
            else if (self is TypeReference) // + TypeDefinition
            {
                return GetFriendlyFullTypeReferenceName(self as TypeReference, language, includeNamespace);
            }
            else if (self is PropertyDefinition)
            {
                return GetFriendlyFullPropertyDefinitionName(self as PropertyDefinition, language, includeNamespace);
            }
            else if (self is GenericInstanceMethod)
            {
                return GetFriendlyFullGenericInstanceMethodName(self as GenericInstanceMethod, language, includeNamespace);
            }
            else if (self is MethodReference) // + MethodDefinition
            {
                return GetFriendlyFullMethodReferenceName(self as MethodReference, language, includeNamespace);
            }
            else if (self is FieldReference) // + FieldDefinition
            {
                return GetFriendlyFullFieldReferenceName(self as FieldReference, language, includeNamespace);
            }
            else if (self is EventReference) // + EventDefinition
            {
                return GetFriendlyFullEventReferenceName(self as EventReference, language, includeNamespace);
            }

            throw new Exception("Invalid member type.");
        }

        #region IMemberDefinition FriendlyFullName
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
        #endregion

        #region Private methods
        private static string GetFriendlyFullArrayTypeName(this ArrayType self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullTypeSpecificationName(language, includeNamespace) + self.Suffix;
        }

        private static string GetFriendlyFullFunctionPointerTypeName(this FunctionPointerType self, ILanguage language, bool includeNamespace = true)
        {
            StringBuilder signature = new StringBuilder();
            signature.Append(self.Function.Name);
            signature.Append(" ");
            signature.Append(self.Function.FixedReturnType.GetFriendlyFullName(language, includeNamespace));
            signature.Append(" *");
            self.MethodSignatureFriendlyFullName(language, signature, false, includeNamespace);
            return signature.ToString();
        }

        private static string GetFriendlyFullGenericInstanceTypeName(this GenericInstanceType self, ILanguage language, bool includeNamespace = true)
        {
            StringBuilder name = new StringBuilder();
            name.Append(self.GetFriendlyFullTypeSpecificationName(language, includeNamespace));
            int indexOfBracket = name.ToString().LastIndexOf('<');
            if (indexOfBracket >= 0 && indexOfBracket < name.Length)
            {
                name.Remove(indexOfBracket, name.Length - indexOfBracket);
                self.GenericInstanceFriendlyFullName(language, name, false, "<", ">", includeNamespace);

                int count = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    if (name[i] == '<')
                    {
                        count++;
                        continue;
                    }
                    if (name[i] == '>')
                    {
                        count--;
                        continue;
                    }
                }
                if (count > 0)
                {
                    name.Append(new string('>', count));
                }
            }
            return name.ToString();
        }

        private static string GetFriendlyFullOptionalModifierTypeName(this OptionalModifierType self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullTypeSpecificationName(language, includeNamespace) + self.Suffix;
        }

        private static string GetFriendlyFullRequiredModifierTypeName(this RequiredModifierType self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullTypeSpecificationName(language, includeNamespace) + self.Suffix;
        }

        private static string GetFriendlyFullPointerTypeName(this PointerType self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullTypeSpecificationName(language, includeNamespace) + "*";
        }

        private static string GetFriendlyFullByReferenceTypeName(this ByReferenceType self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullTypeSpecificationName(language, includeNamespace) + "&";
        }

        private static string GetFriendlyFullTypeSpecificationName(this TypeSpecification self, ILanguage language, bool includeNamespace = true)
        {
            return self.ElementType.GetFriendlyFullName(language, includeNamespace);
        }

        private static string GetFriendlyFullGenericParameterName(this GenericParameter self)
        {
            return self.Name;
        }

        private static string GetFriendlyFullTypeReferenceName(this TypeReference self, ILanguage language, bool includeNamespace = true)
        {
            if (self.IsNested)
            {
                return self.DeclaringType.GetFriendlyFullName(language, includeNamespace) + "/" + self.GetGenericName(language);
            }
            if (string.IsNullOrEmpty(self.Namespace))
            {
                return self.GetGenericName(language);
            }

            if (includeNamespace)
            {
                return self.Namespace + "." + self.GetGenericName(language);
            }

            return self.GetGenericName(language);
        }

        private static string GetFriendlyFullPropertyDefinitionName(this PropertyDefinition self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullPropertyDefinitionName(language, self.MemberFriendlyFullName(language, includeNamespace), includeNamespace);
        }

        private static string GetFriendlyFullGenericInstanceMethodName(this GenericInstanceMethod self, ILanguage language, bool includeNamespace = true)
        {
            StringBuilder signature = new StringBuilder();
            signature.Append(self.FixedReturnType.FullName)
            .Append(" ")
            .Append(self.MemberFriendlyFullName(language, includeNamespace));
            self.GenericInstanceFriendlyFullName(language, signature, false, "<", ">", includeNamespace);
            self.MethodSignatureFriendlyFullName(language, signature, false, includeNamespace);
            return signature.ToString();
        }

        private static string GetFriendlyFullMethodReferenceName(this MethodReference self, ILanguage language, bool includeNamespace = true)
        {
            return self.GetFriendlyFullMethodReferenceName(language, self.MemberFriendlyFullName(language, includeNamespace), false, includeNamespace);
        }

        private static string GetFriendlyFullFieldReferenceName(this FieldReference self, ILanguage language, bool includeNamespace = true)
        {
            return self.FieldType.GetFriendlyFullName(language, includeNamespace) + " " + self.MemberFriendlyFullName(language, includeNamespace);
        }

        private static string GetFriendlyFullEventReferenceName(this EventReference self, ILanguage language, bool includeNamespace = true)
        {
            return self.EventType.GetFriendlyFullName(language, includeNamespace) + " " + self.MemberFriendlyFullName(language, includeNamespace);
        }
        #endregion
        #endregion

        #region IMemberDefinition FullMemberName
        public static string GetFriendlyFullName(this IMemberDefinition self, ILanguage language)
        {
            return ((MemberReference)self).GetFriendlyFullName(language);
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
            int index = self.GetFriendlyFullName(language).IndexOf('/');
            if (index > 0)
            {
                var fullName = self.GetFriendlyFullName(language).Substring(0, index);
                var typeIndex = fullName.LastIndexOf('.');
                if (typeIndex > 0)
                {
                    return fullName.Substring(typeIndex + 1);
                }
                return fullName;
            }
            return self.GetGenericName(language, leftBracket, rightBracket);
        }

        public static string GetFriendlyMemberName(this IMemberDefinition self, ILanguage language)
        {
            return self.GetFriendlyMemberName(language, "<", ">");
        }

        public static string GetFriendlyMemberName(this IMemberDefinition self, ILanguage language, string leftBracket, string rightBracket)
        {
            if (self is IGenericDefinition)
            {
                return ((IGenericDefinition)self).GetGenericName(language, leftBracket, rightBracket);
            }
            return self.GetFullMemberName(language);
        }

        public static string GetFullMemberName(this IMemberDefinition self, ILanguage language)
        {
            if (self is TypeDefinition || self is FieldDefinition)
            {
                return GenericHelper.ReplaceInvalidCharactersName(language, self.Name);
            }
            if (self is EventDefinition)
            {
                EventDefinition @event = self as EventDefinition;

                if (@event.IsExplicitImplementation())
                {
                    string[] nameParts = @event.Name.Split(new char[] { '.' });

                    var sb = new StringBuilder(nameParts.Length * 2);

                    for (int i = 0; i < nameParts.Length; i++)
                    {
                        string namePart = nameParts[i];

                        sb.Append(GenericHelper.ReplaceInvalidCharactersName(language, namePart));

                        if (i < nameParts.Length - 1)
                        {
                            sb.Append(".");
                        }
                    }
                    return sb.ToString();
                }
                return GenericHelper.ReplaceInvalidCharactersName(language, self.Name);
            }
            else if (self is MethodDefinition)
            {
                return (self as MethodDefinition).GetFriendlyFullMethodReferenceName(language, self.Name, false);
            }
            else if (self is PropertyDefinition)
            {
                PropertyDefinition propertyDefinition = self as PropertyDefinition;

                if (propertyDefinition.IsExplicitImplementation())
                {
                    string[] nameParts = propertyDefinition.Name.Split(new char[] { '.' });

                    var sb = new StringBuilder(nameParts.Length * 2);

                    for (int i = 0; i < nameParts.Length; i++)
                    {
                        string namePart = nameParts[i];

                        sb.Append(GenericHelper.ReplaceInvalidCharactersName(language, namePart));

                        if (i < nameParts.Length - 1)
                        {
                            sb.Append(".");
                        }
                    }
                    sb.Append("()");
                    return sb.ToString();
                }
                else
                {
                    return (self as PropertyDefinition).GetFriendlyFullPropertyDefinitionName(language, self.Name);
                }
            }
            throw new Exception("Invalid member definition type.");
        }
        #endregion

        #region IGenericDefinition GenericName and GenericFullName

        public static string GetGenericName(this IGenericDefinition self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
        {
            if (self is TypeDefinition || self is MethodDefinition)
            {
                return GenericHelper.GetGenericName(self, leftBracket, rightBracket, language);
            }
            throw new Exception("Invalid generic member definition type.");
        }

        public static string GetGenericFullName(this IGenericDefinition self, ILanguage language, bool includeNamespace = true)
        {
            if (self is TypeDefinition)
            {
                return (self as TypeDefinition).GetGenericFullTypeDefinitionName(language, includeNamespace);
            }
            else if (self is MethodDefinition)
            {
                MethodDefinition methodDef = self as MethodDefinition;
                return methodDef.GetFriendlyFullMethodReferenceName(language, methodDef.GenericMemberFullName(language, includeNamespace), true, includeNamespace);
            }
            throw new Exception("Invalid generic member definition type.");
        }

        private static string GetGenericFullTypeDefinitionName(this TypeDefinition self, ILanguage language, bool includeNamespace = true)
        {
            string name = (self as IGenericDefinition).GetGenericName(language);
            if (self.IsNested)
            {
                return self.DeclaringType.GetGenericFullName(language, includeNamespace) + "/" + name;
            }
            if (string.IsNullOrEmpty(self.Namespace))
            {
                return name;
            }
            return self.Namespace + "." + name;
        }

        private static string GenericMemberFullName(this MethodDefinition self, ILanguage language, bool includeNamespace = true)
        {
            if (self.DeclaringType == null)
            {
                return self.GetGenericName(language);
            }
            return self.DeclaringType.GetGenericFullName(language, includeNamespace) + "::" + self.GetGenericName(language);
        }
        #endregion

        #region TypeReference GenericName

        public static string GetGenericName(this TypeReference self, ILanguage language, string leftBracket = "<", string rightBracket = ">")
        {
            if (self is GenericInstanceType)
            {
                return (self as GenericInstanceType).GetFriendlyGenericInstanceName(language, leftBracket, rightBracket);
            }
            return GenericHelper.GetGenericName(self, leftBracket, rightBracket, language);
        }

        private static string GetFriendlyGenericInstanceName(this GenericInstanceType self, ILanguage language, string leftBracket, string rightbracket)
        {
            StringBuilder sb = new StringBuilder();

            string name = GenericHelper.GetNonGenericName(self.Name);
            sb.Append(GenericHelper.ReplaceInvalidCharactersName(language, name));

            if (language != null && language.Name == "IL")
            {
                name += "`" + self.GenericArguments.Count.ToString();

                return name;
            }
            sb.Append(leftBracket);

            for (int i = 0; i < self.GenericArguments.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(self.GenericArguments[i].GetGenericName(language, leftBracket, rightbracket));
            }
            sb.Append(rightbracket);

            return sb.ToString();
        }

        #endregion

        #region Common
        private static void MethodSignatureFriendlyFullName(this IMethodSignature self, ILanguage language, StringBuilder builder, bool useGenericName, bool includeNamespace = true)
        {
            builder.Append("(");

            if (self.HasParameters)
            {
                var parameters = self.Parameters;
                for (int i = 0; i < parameters.Count; i++)
                {
                    var parameter = parameters[i];
                    if (i > 0)
                        builder.Append(",");

                    if (parameter.ParameterType.IsSentinel)
                        builder.Append("...,");

                    if (useGenericName)
                    {
                        var typeDefinition = parameter.ParameterType.Resolve();
                        if (typeDefinition != null)
                        {
                            builder.Append(typeDefinition.GetGenericFullName(language, includeNamespace));
                            continue;
                        }
                    }
                    builder.Append(parameter.ParameterType.GetFriendlyFullName(language, includeNamespace));
                }
            }

            builder.Append(")");
        }

        private static void GenericInstanceFriendlyFullName(this IGenericInstance self, ILanguage language, StringBuilder builder, bool useGenericName, string leftBracket, string rightBracket, bool includeNamespace = true)
        {
            builder.Append(leftBracket);
            var arguments = self.GenericArguments;
            for (int i = 0; i < arguments.Count; i++)
            {
                TypeReference currentArgument = arguments[i];
                if (self.PostionToArgument.ContainsKey(i))
                {
                    currentArgument = self.PostionToArgument[i];
                }
                if (i > 0)
                {
                    builder.Append(",");
                }
                var fullName = currentArgument.GetFriendlyFullName(language, includeNamespace);

                if (useGenericName)
                {
                    var typeDefinition = currentArgument.Resolve();
                    if (typeDefinition != null)
                    {
                        fullName = typeDefinition.GetGenericFullName(language);
                    }
                }
                builder.Append(fullName);
            }
            builder.Append(rightBracket);
        }

        private static string MemberFriendlyFullName(this MemberReference self, ILanguage language, bool includeNamespace = true)
        {
            if (self.DeclaringType == null)
            {
                return GenericHelper.ReplaceInvalidCharactersName(language, self.Name);
            }
            return self.DeclaringType.GetFriendlyFullName(language, includeNamespace) + "::" + self.Name;
        }

        private static string GetFriendlyFullPropertyDefinitionName(this PropertyDefinition self, ILanguage language, string memberFriendlyFullName, bool includeNamespace = true)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(self.PropertyType.GetFriendlyFullName(language, includeNamespace));
            builder.Append(' ');
            builder.Append(memberFriendlyFullName);
            builder.Append('(');
            if (self.HasParameters)
            {
                Collection<ParameterDefinition> parameters = self.Parameters;
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i > 0)
                        builder.Append(',');
                    builder.Append(parameters[i].ParameterType.GetFriendlyFullName(language, includeNamespace));
                }
            }
            builder.Append(')');
            return builder.ToString();
        }

        private static string GetFriendlyFullMethodReferenceName(this MethodReference self, ILanguage language, string memberFriendlyFullName, bool useGenericName, bool includeNamespace = true)
        {
            StringBuilder builder = new StringBuilder();
            string typeFullName = self.FixedReturnType.GetFriendlyFullName(language, includeNamespace);
            if (useGenericName)
            {
                TypeDefinition typeDefinition = self.FixedReturnType.Resolve();
                if (typeDefinition != null)
                {
                    typeFullName = typeDefinition.GetGenericFullName(language, includeNamespace);
                }
            }
            builder.Append(typeFullName)
                .Append(" ")
                .Append(memberFriendlyFullName);
            self.MethodSignatureFriendlyFullName(language, builder, useGenericName, includeNamespace);
            return builder.ToString();
        }
        #endregion
    }
}
