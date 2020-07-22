using System;

namespace Telerik.JustDecompiler.Pattern
{
	public abstract class CodePattern : ICodePattern
	{
		protected CodePattern()
		{
			base();
			return;
		}

		public static MatchContext Match(ICodePattern pattern, object object)
		{
			V_0 = new MatchContext();
			V_0.set_Success(pattern.Match(V_0, object));
			return V_0;
		}

		public abstract bool Match(MatchContext context, object object);
	}
}