using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Inlining;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class VariableInliningPatternAggressive : VariableInliningPattern
	{
		public VariableInliningPatternAggressive(CodePatternsContext patternsContext, MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder)
		{
			base(patternsContext, methodContext, finder);
			return;
		}

		protected override bool ShouldInlineAggressively(VariableDefinition variable)
		{
			return true;
		}
	}
}