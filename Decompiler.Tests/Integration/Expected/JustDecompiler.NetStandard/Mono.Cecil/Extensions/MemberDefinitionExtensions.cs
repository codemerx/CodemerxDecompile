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
				if (propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.HasBody)
				{
					Collection<Instruction> instructions = MemberDefinitionExtensions.GetSafeBodyInstructions(propertyDefinition.GetMethod);
					if (instructions != null)
					{
						foreach (Instruction instruction in instructions)
						{
							yield return instruction;
						}
					}
				}
				if (propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.HasBody)
				{
					Collection<Instruction> safeBodyInstructions1 = MemberDefinitionExtensions.GetSafeBodyInstructions(propertyDefinition.SetMethod);
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
				return ((MethodDefinition)memberDefinition).FixedReturnType;
			}
			if (memberDefinition is PropertyDefinition)
			{
				return ((PropertyDefinition)memberDefinition).PropertyType;
			}
			if (!(memberDefinition is FieldDefinition))
			{
				return null;
			}
			return ((FieldDefinition)memberDefinition).FieldType;
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

		public static bool HasBody(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition is MethodDefinition)
			{
				return ((MethodDefinition)memberDefinition).HasBody;
			}
			if (!(memberDefinition is PropertyDefinition))
			{
				return false;
			}
			PropertyDefinition propertyDefinition = (PropertyDefinition)memberDefinition;
			if (propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.HasBody)
			{
				return true;
			}
			if (propertyDefinition.SetMethod == null)
			{
				return false;
			}
			return propertyDefinition.SetMethod.HasBody;
		}

		public static bool TryGetDynamicAttribute(this ICustomAttributeProvider self, out CustomAttribute dynamicAttribute)
		{
			bool flag;
			if (!self.HasCustomAttributes)
			{
				dynamicAttribute = null;
				return false;
			}
			Collection<CustomAttribute>.Enumerator enumerator = self.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.Current;
					if (current.AttributeType.FullName != "System.Runtime.CompilerServices.DynamicAttribute")
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
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}
	}
}