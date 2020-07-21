using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class ParameterDefinitionExtensions
	{
		public static bool IsOutParameter(this ParameterDefinition self)
		{
			if (!self.get_IsOut())
			{
				return false;
			}
			if (self.get_IsIn())
			{
				return false;
			}
			if (self.get_ParameterType() != null && self.get_ParameterType().get_IsByReference())
			{
				return true;
			}
			return false;
		}

		public static bool IsParamArray(this ParameterDefinition self)
		{
			stackVariable2 = new String[1];
			stackVariable2[0] = "System.ParamArrayAttribute";
			return self.HasCustomAttribute(stackVariable2);
		}
	}
}