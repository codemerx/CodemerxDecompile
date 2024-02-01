using System;

namespace Mono.Cecil.Extensions
{
	public static class DecompilationLocker
	{
		public static object Lock;

		static DecompilationLocker()
		{
			DecompilationLocker.Lock = new Object();
		}
	}
}