using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class MethodDefinitionExtensions
	{
		public static IEnumerable<Instruction> GetMethodInstructions(this MethodDefinition method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("MethodDefinition");
			}
			if (!method.get_HasBody())
			{
				return Enumerable.Empty<Instruction>();
			}
			try
			{
				V_0 = method.get_Body().get_Instructions();
			}
			catch
			{
				dummyVar0 = exception_0;
				V_0 = Enumerable.Empty<Instruction>();
			}
			return V_0;
		}

		public static MethodDefinition GetMoreVisibleMethod(this MethodDefinition method, MethodDefinition other)
		{
			if (method == null)
			{
				return other;
			}
			if (other == null)
			{
				return method;
			}
			if (method.get_IsPrivate())
			{
				return other;
			}
			if (method.get_IsFamily() || method.get_IsAssembly())
			{
				if (!other.get_IsPublic() && !other.get_IsFamilyOrAssembly())
				{
					return method;
				}
				return other;
			}
			if (!method.get_IsFamilyOrAssembly())
			{
				return method;
			}
			if (other.get_IsPublic())
			{
				return other;
			}
			return method;
		}

		public static bool HasAsyncAttributes(this MethodDefinition self)
		{
			V_0 = false;
			V_1 = false;
			V_2 = self.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!V_0 & V_1)
					{
						if (!V_0 && MethodDefinitionExtensions.IsAsyncStateMachineAttribute(V_3))
						{
							V_0 = true;
						}
						if (V_1 || !MethodDefinitionExtensions.IsDebuggerStepThroughAttribute(V_3))
						{
							continue;
						}
						V_1 = true;
					}
					else
					{
						V_4 = true;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				V_2.Dispose();
			}
		Label1:
			return V_4;
		Label0:
			return V_0 & V_1;
		}

		public static bool HasAsyncStateMachineVariable(this MethodDefinition self)
		{
			V_0 = self.get_Body().get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_VariableType() == null)
					{
						stackVariable10 = null;
					}
					else
					{
						stackVariable10 = V_1.get_VariableType().Resolve();
					}
					V_2 = stackVariable10;
					if (V_2 == null || !V_2.IsAsyncStateMachine())
					{
						continue;
					}
					V_3 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_3;
		Label0:
			return false;
		}

		public static bool IsAsync(this MethodDefinition self)
		{
			if (self.IsAsync(out V_0))
			{
				return true;
			}
			if (!self.HasAsyncAttributes())
			{
				return false;
			}
			return self.HasAsyncStateMachineVariable();
		}

		public static bool IsAsync(this MethodDefinition self, out TypeDefinition asyncStateMachineType)
		{
			asyncStateMachineType = null;
			V_0 = self.get_CustomAttributes().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (!MethodDefinitionExtensions.IsAsyncAttribute(V_0.get_Current(), self, self.get_DeclaringType(), out asyncStateMachineType))
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}

		private static bool IsAsyncAttribute(CustomAttribute customAttribute, MethodDefinition method, TypeDefinition declaringType, out TypeDefinition stateMachineType)
		{
			stateMachineType = null;
			if (String.op_Inequality(customAttribute.get_AttributeType().get_FullName(), "System.Runtime.CompilerServices.AsyncStateMachineAttribute"))
			{
				return false;
			}
			customAttribute.Resolve();
			if (customAttribute.get_ConstructorArguments().get_Count() != 1 || String.op_Inequality(customAttribute.get_ConstructorArguments().get_Item(0).get_Type().get_FullName(), "System.Type"))
			{
				return false;
			}
			V_2 = customAttribute.get_ConstructorArguments().get_Item(0);
			V_0 = V_2.get_Value() as TypeReference;
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.Resolve();
			if (V_1 == null || (object)V_1.get_DeclaringType() != (object)declaringType || !V_1.IsAsyncStateMachine())
			{
				return false;
			}
			stateMachineType = V_1;
			return true;
		}

		private static bool IsAsyncStateMachineAttribute(CustomAttribute customAttribute)
		{
			return String.op_Equality(customAttribute.get_AttributeType().get_FullName(), "System.Runtime.CompilerServices.AsyncStateMachineAttribute");
		}

		private static bool IsDebuggerStepThroughAttribute(CustomAttribute customAttribute)
		{
			return String.op_Equality(customAttribute.get_AttributeType().get_FullName(), "System.Diagnostics.DebuggerStepThroughAttribute");
		}

		public static bool IsExtern(this MethodDefinition method)
		{
			if (method.get_IsInternalCall())
			{
				return true;
			}
			if (method.get_IsPInvokeImpl())
			{
				return true;
			}
			if (method.get_IsRuntime())
			{
				return true;
			}
			return false;
		}

		public static bool IsFunction(this MethodDefinition self)
		{
			if (self.get_FixedReturnType() == null)
			{
				return false;
			}
			return String.op_Inequality(self.get_FixedReturnType().get_FullName(), "System.Void");
		}

		public static bool IsQueryableMethod(this MethodDefinition self)
		{
			if (self == null || !self.get_IsStatic())
			{
				return false;
			}
			return String.op_Equality(self.get_DeclaringType().get_FullName(), "System.Linq.Queryable");
		}

		public static bool IsQueryMethod(this MethodDefinition self)
		{
			// 
			// Current member / type: System.Boolean Mono.Cecil.Extensions.MethodDefinitionExtensions::IsQueryMethod(Mono.Cecil.MethodDefinition)
			// Exception in: System.Boolean IsQueryMethod(Mono.Cecil.MethodDefinition)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}