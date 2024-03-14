using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Extensions
{
    public static class SecurityAttributeExtensions
    {
        public static int CompareToSecurityAttribute(this SecurityAttribute first, SecurityAttribute second, SecurityDeclaration firstDeclaration, SecurityDeclaration secondDeclaration)
        {
            if (first == second)
            {
                return 0;
            }

            string firstActionAsString = firstDeclaration.Action.ToString();
            string secondActionAsString = secondDeclaration.Action.ToString();

            if (firstActionAsString != secondActionAsString)
            {
                return firstActionAsString.CompareTo(secondActionAsString);
            }

            #region Compare Properties

            int maxProperties = Math.Max(first.Properties.Count, second.Properties.Count);

            for (int i = 0; i < maxProperties; i++)
            {

                if (i >= first.Properties.Count)
                {
                    return 1;
                }
                if (i >= second.Properties.Count)
                {
                    return -1;
                }

                CustomAttributeNamedArgument firstProperty = first.Properties[i];
                CustomAttributeNamedArgument secondProperty = second.Properties[i];

                if (firstProperty.Name == secondProperty.Name)
                {
                    string firstValue = firstProperty.Argument.Value.ToString();
                    string secondValue = secondProperty.Argument.Value.ToString();
                    if (firstValue != secondValue)
                    {
                        return firstValue.CompareTo(secondValue);
                    }
                }
                else
                {
                    return firstProperty.Name.CompareTo(secondProperty.Name);
                }
            }

            #endregion

            #region Compare Fiels

            int maxFields = Math.Max(first.Fields.Count, second.Fields.Count);

            for (int i = 0; i < maxFields; i++)
            {

                if (i >= first.Fields.Count)
                {
                    return 1;
                }
                if (i >= second.Fields.Count)
                {
                    return -1;
                }

                CustomAttributeNamedArgument firstField = first.Fields[i];
                CustomAttributeNamedArgument secondField = second.Fields[i];

                if (firstField.Name == secondField.Name)
                {
                    string firstValue = firstField.Argument.Value.ToString();
                    string secondValue = secondField.Argument.Value.ToString();
                    if (firstValue != secondValue)
                    {
                        return firstValue.CompareTo(secondValue);
                    }
                }
                else
                {
                    return firstField.Name.CompareTo(secondField.Name);
                }
            }

            #endregion

            return 0;

        }
    }
}
