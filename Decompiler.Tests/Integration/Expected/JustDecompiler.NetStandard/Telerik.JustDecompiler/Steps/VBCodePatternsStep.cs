using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
	internal class VBCodePatternsStep : CodePatternsStep
	{
		public VBCodePatternsStep(bool isAggressive)
		{
			base(isAggressive);
			return;
		}

		protected override IEnumerable<ICodePattern> GetCodePatterns()
		{
			stackVariable2 = new List<ICodePattern>(this.GetCodePatterns());
			stackVariable2.Add(new RaiseEventPattern());
			return stackVariable2;
		}
	}
}