using System;

namespace Telerik.JustDecompiler.Languages
{
	public struct OffsetSpan
	{
		public int StartOffset;

		public int EndOffset;

		public OffsetSpan(int start, int end)
		{
			this.StartOffset = start;
			this.EndOffset = end;
			return;
		}
	}
}