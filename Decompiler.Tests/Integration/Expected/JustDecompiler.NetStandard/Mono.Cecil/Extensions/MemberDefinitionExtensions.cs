using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class MemberDefinitionExtensions
	{
		public static IEnumerable<Instruction> GetInstructions(IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				Collection<Instruction> safeBodyInstructions = MemberDefinitionExtensions.GetSafeBodyInstructions((MethodDefinition)memberDefinition);
				if (safeBodyInstructions != null)
				{
					foreach (Instruction safeBodyInstruction in safeBodyInstructions)
					{
						yield return safeBodyInstruction;
					}
				}
			}
			if (memberDefinition is PropertyDefinition)
			{
				PropertyDefinition propertyDefinition = (PropertyDefinition)memberDefinition;
				if (propertyDefinition.get_GetMethod() != null && propertyDefinition.get_GetMethod().get_HasBody())
				{
					Collection<Instruction> collection = MemberDefinitionExtensions.GetSafeBodyInstructions(propertyDefinition.get_GetMethod());
					if (collection != null)
					{
						foreach (Instruction instruction in collection)
						{
							yield return instruction;
						}
					}
				}
				if (propertyDefinition.get_SetMethod() != null && propertyDefinition.get_SetMethod().get_HasBody())
				{
					Collection<Instruction> safeBodyInstructions1 = MemberDefinitionExtensions.GetSafeBodyInstructions(propertyDefinition.get_SetMethod());
					if (safeBodyInstructions1 != null)
					{
						foreach (Instruction safeBodyInstruction1 in safeBodyInstructions1)
						{
							yield return safeBodyInstruction1;
						}
					}
				}
				propertyDefinition = null;
			}
		}

		public static TypeReference GetReturnType(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				return ((MethodDefinition)memberDefinition).get_FixedReturnType();
			}
			if (memberDefinition is PropertyDefinition)
			{
				return ((PropertyDefinition)memberDefinition).get_PropertyType();
			}
			if (!(memberDefinition is FieldDefinition))
			{
				return null;
			}
			return ((FieldDefinition)memberDefinition).get_FieldType();
		}

		private static Collection<Instruction> GetSafeBodyInstructions(MethodDefinition memberDefinition)
		{
			Collection<Instruction> instructions = null;
			try
			{
				instructions = memberDefinition.get_Body().get_Instructions();
			}
			catch
			{
			}
			return instructions;
		}

		public static bool HasBody(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				return ((MethodDefinition)memberDefinition).get_HasBody();
			}
			if (!(memberDefinition is PropertyDefinition))
			{
				return false;
			}
			PropertyDefinition propertyDefinition = (PropertyDefinition)memberDefinition;
			if (propertyDefinition.get_GetMethod() != null && propertyDefinition.get_GetMethod().get_HasBody())
			{
				return true;
			}
			if (propertyDefinition.get_SetMethod() == null)
			{
				return false;
			}
			return propertyDefinition.get_SetMethod().get_HasBody();
		}

		public static bool TryGetDynamicAttribute(this ICustomAttributeProvider self, out CustomAttribute dynamicAttribute)
		{
			bool flag;
			if (!self.get_HasCustomAttributes())
			{
				dynamicAttribute = null;
				return false;
			}
			Collection<CustomAttribute>.Enumerator enumerator = self.get_CustomAttributes().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.get_Current();
					if (current.get_AttributeType().get_FullName() != "System.Runtime.CompilerServices.DynamicAttribute")
					{
						continue;
					}
					dynamicAttribute = current;
					flag = true;
					return flag;
				}
				dynamicAttribute = null;
				return false;
			}
			finally
			{
				enumerator.Dispose();
			}
			return flag;
		}
	}
}