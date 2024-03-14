using System;

namespace Telerik.JustDecompiler
{
	internal class DecompilationException : Exception
	{
		public DecompilationException()
		{
		}

		public DecompilationException(string message) : base(message)
		{
		}
	}
}