using System;
using Telerik.JustDecompiler.Decompiler.Inlining;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
    class VariableInliningPatternAggressive : VariableInliningPattern
    {
        public VariableInliningPatternAggressive(CodePatternsContext patternsContext, Telerik.JustDecompiler.Decompiler.MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder)
            :base(patternsContext, methodContext, finder)
        {
        }

        protected override bool ShouldInlineAggressively(Mono.Cecil.Cil.VariableDefinition variable)
        {
            return true;
        }
    }
}
