using System;

namespace Mono.Cecil.Extensions
{
	public static class CustomAttributeExtensions
	{
		public static int CompareToCustomAttribute(this CustomAttribute first, CustomAttribute second, bool fullNameCheck = false)
		{
            if (first.AttributeType.Name != second.AttributeType.Name)
            {
                if (fullNameCheck)
                {
                    return first.AttributeType.FullName.CompareTo(second.AttributeType.FullName);
                }
                return first.AttributeType.Name.CompareTo(second.AttributeType.Name);
            }
            if (first == second)
            {
                return 0;
            }
			int returnValue = CompareConstructorArguments(first, second);
			if(returnValue != 0)
			{
				return returnValue;
			}
			returnValue = CompareConstructorFields(first, second);
			if (returnValue != 0)
			{
				return returnValue;
			}
			returnValue = CompareConstructorProperties(first, second);
			if (returnValue != 0)
			{
				return returnValue;
			}
			return 0;
		}
  
		private static int CompareConstructorProperties(CustomAttribute first, CustomAttribute second)
		{
			if (first.HasProperties && !second.HasProperties)
			{
				return 1;
			}
			if (!first.HasProperties && second.HasProperties)
			{
				return -1;
			}

			int maxArguments = Math.Max(first.Properties.Count, second.Properties.Count);

			for (int i = 0; i < maxArguments; i++)
			{
				if (first.Properties.Count <= i)
				{
					return 1;
				}
				if (second.Properties.Count <= i)
				{
					return -1;
				}
				CustomAttributeNamedArgument firstProperty = first.Properties[i];
				CustomAttributeNamedArgument secondProperty = second.Properties[i];

				int namesCompared = firstProperty.Name.CompareTo(secondProperty.Name);
				if (namesCompared != 0)
				{
					return namesCompared;
				}

				int compareResult = CompareCustomAttributeArguments(firstProperty.Argument, secondProperty.Argument);
				if (compareResult != 0)
				{
					return compareResult;
				}
			}
			return 0;
		}

		private static int CompareConstructorArguments(CustomAttribute first, CustomAttribute second)
		{
			if (first.HasConstructorArguments && !second.HasConstructorArguments)
			{
				return 1;
			}
			if (!first.HasConstructorArguments && second.HasConstructorArguments)
			{
				return -1;
			}

			int maxArguments = Math.Max(first.ConstructorArguments.Count, second.ConstructorArguments.Count);

			for (int i = 0; i < maxArguments; i++)
			{
				if (first.ConstructorArguments.Count <= i)
				{
					return 1;
				}
				if (second.ConstructorArguments.Count <= i)
				{
					return -1;
				}
				CustomAttributeArgument firstArgument = first.ConstructorArguments[i];
				CustomAttributeArgument secondArgument = second.ConstructorArguments[i];

				int compareResult = CompareCustomAttributeArguments(firstArgument, secondArgument);
				if (compareResult != 0)
				{
					return compareResult;
				}
			}
			return 0;
		}

		private static int CompareConstructorFields(CustomAttribute first, CustomAttribute second)
		{
			if (first.HasFields && !second.HasFields)
			{
				return 1; // <second> should be printed first
			}
			if (!first.HasFields && second.HasFields)
			{
				return -1;
			}

			int maxFields = Math.Max(first.Fields.Count, second.Fields.Count);

			for (int i = 0; i < maxFields; i++)
			{
				if (first.Fields.Count <= i)
				{
					return 1;
				}
				if (second.Fields.Count <= i)
				{
					return -1; // the arguments of <second> have ended, and they all equaled the arguments of <first>
				}
				CustomAttributeNamedArgument firstField = first.Fields[i];
				CustomAttributeNamedArgument secondField = second.Fields[i];

				int namesCompared = firstField.Name.CompareTo(secondField.Name);
				if (namesCompared != 0)
				{
					return namesCompared;
				}

				int compareResult = CompareCustomAttributeArguments(firstField.Argument, secondField.Argument);
				if (compareResult != 0)
				{
					return compareResult;
				}
			}
			return 0;
		}

		private static int CompareCustomAttributeArguments(CustomAttributeArgument firstArgument, CustomAttributeArgument secondArgument)
		{
			int compareResult = firstArgument.Value.ToString().CompareTo(secondArgument.Value.ToString());
			return compareResult;
		}
	}
}
