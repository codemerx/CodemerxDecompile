using System;

namespace Telerik.JustDecompiler
{
	internal class DecompilationException : Exception
	{
		public DecompilationException()
		{
			base();
			return;
		}

		public DecompilationException(string message)
		{
			base(message);
			return;
		}
	}
}