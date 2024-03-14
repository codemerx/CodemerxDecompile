using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
	internal class VBCodePatternsStep : CodePatternsStep
	{
		public VBCodePatternsStep(bool isAggressive) : base(isAggressive)
		{
		}

		protected override IEnumerable<ICodePattern> GetCodePatterns()
		{
			return new List<ICodePattern>(base.GetCodePatterns())
			{
				new RaiseEventPattern()
			};
		}
	}
}