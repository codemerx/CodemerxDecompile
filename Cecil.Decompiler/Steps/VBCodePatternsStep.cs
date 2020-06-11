using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
    class VBCodePatternsStep : CodePatternsStep
    {
        public VBCodePatternsStep(bool isAggressive)
            : base(isAggressive)
        {
        }

        protected override IEnumerable<ICodePattern> GetCodePatterns()
        {
            List<ICodePattern> result = new List<ICodePattern>(base.GetCodePatterns());

            result.Add(new RaiseEventPattern());

            return result;
        }
    }
}
