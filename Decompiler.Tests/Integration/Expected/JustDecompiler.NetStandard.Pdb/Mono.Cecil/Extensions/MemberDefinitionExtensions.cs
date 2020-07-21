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
			stackVariable1 = new MemberDefinitionExtensions.u003cGetInstructionsu003ed__3(-2);
			stackVariable1.u003cu003e3__memberDefinition = memberDefinition;
			return stackVariable1;
		}

		public static TypeReference GetReturnType(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition as MethodDefinition != null)
			{
				return ((MethodDefinition)memberDefinition).get_FixedReturnType();
			}
			if (memberDefinition as PropertyDefinition != null)
			{
				return ((PropertyDefinition)memberDefinition).get_PropertyType();
			}
			if (memberDefinition as FieldDefinition == null)
			{
				return null;
			}
			return ((FieldDefinition)memberDefinition).get_FieldType();
		}

		private static Collection<Instruction> GetSafeBodyInstructions(MethodDefinition memberDefinition)
		{
			V_0 = null;
			try
			{
				V_0 = memberDefinition.get_Body().get_Instructions();
			}
			catch
			{
				dummyVar0 = exception_0;
			}
			return V_0;
		}

		public static bool HasBody(this IMemberDefinition memberDefinition)
		{
			if (memberDefinition as MethodDefinition != null)
			{
				return ((MethodDefinition)memberDefinition).get_HasBody();
			}
			if (memberDefinition as PropertyDefinition == null)
			{
				return false;
			}
			V_0 = (PropertyDefinition)memberDefinition;
			if (V_0.get_GetMethod() != null && V_0.get_GetMethod().get_HasBody())
			{
				return true;
			}
			if (V_0.get_SetMethod() == null)
			{
				return false;
			}
			return V_0.get_SetMethod().get_HasBody();
		}

		public static bool TryGetDynamicAttribute(this ICustomAttributeProvider self, out CustomAttribute dynamicAttribute)
		{
			if (!self.get_HasCustomAttributes())
			{
				dynamicAttribute = null;
				return false;
			}
			V_0 = self.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!String.op_Equality(V_1.get_AttributeType().get_FullName(), "System.Runtime.CompilerServices.DynamicAttribute"))
					{
						continue;
					}
					dynamicAttribute = V_1;
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			dynamicAttribute = null;
			return false;
		}
	}
}