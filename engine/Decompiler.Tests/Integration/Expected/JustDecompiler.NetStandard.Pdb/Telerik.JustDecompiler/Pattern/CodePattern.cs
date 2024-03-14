using System;

namespace Telerik.JustDecompiler.Pattern
{
	public abstract class CodePattern : ICodePattern
	{
		protected CodePattern()
		{
		}

		public static MatchContext Match(ICodePattern pattern, object @object)
		{
			MatchContext matchContext = new MatchContext()
			{
				Success = pattern.Match(matchContext, @object)
			};
			return matchContext;
		}

		public abstract bool Match(MatchContext context, object @object);
	}
}