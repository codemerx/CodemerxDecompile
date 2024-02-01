using System;

namespace Telerik.JustDecompiler.Pattern
{
	public interface ICodePattern
	{
		bool Match(MatchContext context, object @object);
	}
}