using System;
using System.Collections.Generic;

namespace Mono.Cecil.Extensions
{
    public static class ParameterDefinitionExtensions
    {
        public static bool IsParamArray(this ParameterDefinition self)
        {
            return self.HasCustomAttribute(new string[] { "System.ParamArrayAttribute" });
        }

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
			if (self.ParameterType == null || !self.ParameterType.IsByReference)
			{
				return false;
			}
			return true;
		}
    }
}
