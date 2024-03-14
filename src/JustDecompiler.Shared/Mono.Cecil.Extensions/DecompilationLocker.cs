namespace Mono.Cecil.Extensions
{
	public static class DecompilationLocker
	{
		public static object Lock = new object();
	}
}
