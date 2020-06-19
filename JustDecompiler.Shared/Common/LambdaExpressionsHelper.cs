using System;
using Mono.Cecil;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Common
{
    internal static class LambdaExpressionsHelper
    {
        internal static bool HasAnonymousParameter(Mono.Collections.Generic.Collection<ParameterDefinition> parameters)
        {
            foreach (ParameterDefinition paramDef in parameters)
            {
                if (paramDef.ParameterType.ContainsAnonymousType())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
