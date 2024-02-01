using System;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Pattern
{
	public static class Extensions
	{
		public static bool TryMatch(this ICodePattern self, MatchContext context, object @object)
		{
			if (self == null)
			{
				return true;
			}
			return self.Match(context, @object);
		}
	}
}