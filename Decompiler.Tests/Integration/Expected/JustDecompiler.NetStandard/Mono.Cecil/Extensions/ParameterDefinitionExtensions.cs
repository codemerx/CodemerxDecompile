using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class ParameterDefinitionExtensions
	{
		public static bool IsOutParameter(this ParameterDefinition self)
		{
			if (!self.IsOut)
			{
				return false;
			}
			if (self.IsIn)
			{
				return false;
			}
			if (self.ParameterType != null && self.ParameterType.IsByReference)
			{
				return true;
			}
			return false;
		}

		public static bool IsParamArray(this ParameterDefinition self)
		{
			return self.HasCustomAttribute(new String[] { "System.ParamArrayAttribute" });
		}
	}
}