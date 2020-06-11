using Mono.Collections.Generic;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Mono.Cecil.Extensions
{
	public static class MemberDefinitionExtensions
	{
		public static bool HasBody(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				return ((MethodDefinition) memberDefinition).HasBody;
			}
			if (memberDefinition is PropertyDefinition)
			{
				var propertyDefinition = (PropertyDefinition) memberDefinition;
				if ((propertyDefinition.GetMethod != null) && (propertyDefinition.GetMethod.HasBody))
					return true;

				return ((propertyDefinition.SetMethod != null) && (propertyDefinition.SetMethod.HasBody));
			}
			return false;
		}

		public static TypeReference GetReturnType(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				return ((MethodDefinition) memberDefinition).FixedReturnType;
			}
			if (memberDefinition is PropertyDefinition)
			{
				return ((PropertyDefinition) memberDefinition).PropertyType;
			}
			if (memberDefinition is FieldDefinition)
			{
				return ((FieldDefinition) memberDefinition).FieldType; 
			}
			return null;
		}

		private static Collection<Instruction> GetSafeBodyInstructions(MethodDefinition memberDefinition)
		{
			Collection<Instruction> instructions = null;
			try
			{
				instructions = memberDefinition.Body.Instructions;
			}
			catch
			{
			}
			return instructions;
		}

		public static IEnumerable<Instruction> GetInstructions(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				Collection<Instruction> instructions = GetSafeBodyInstructions((MethodDefinition) memberDefinition);
				if (instructions != null)
				{
					foreach (var instruction in instructions)
					{
						yield return instruction;
					}
				}
			}
			if (memberDefinition is PropertyDefinition)
			{
				var propertyDefinition = (PropertyDefinition) memberDefinition;
				if ((propertyDefinition.GetMethod != null) && (propertyDefinition.GetMethod.HasBody))
				{
					var getInstructions = GetSafeBodyInstructions(propertyDefinition.GetMethod);
					if (getInstructions != null)
					{
						foreach (var getInstruction in getInstructions)
						{
							yield return getInstruction;
						}
					}
				}
				if ((propertyDefinition.SetMethod != null) && (propertyDefinition.SetMethod.HasBody))
				{
					var setInstructions = GetSafeBodyInstructions(propertyDefinition.SetMethod);
					if (setInstructions != null)
					{
						foreach (var setInstruction in setInstructions)
						{
							yield return setInstruction;
						}
					}
				}
			}
			yield break;
		}

        public static bool TryGetDynamicAttribute(this ICustomAttributeProvider self, out CustomAttribute dynamicAttribute)
        {
            if (!self.HasCustomAttributes)
            {
                dynamicAttribute = null;
                return false;
            }

            foreach (CustomAttribute attribute in self.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == "System.Runtime.CompilerServices.DynamicAttribute")
                {
                    dynamicAttribute = attribute;
                    return true;
                }
            }

            dynamicAttribute = null;
            return false;
        }
	}
}