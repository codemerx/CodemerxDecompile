using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sample3
{
	[Nullable(0)]
	[NullableContext(1)]
	internal class Program
	{
		public Program()
		{
		}

		[DebuggerStepThrough]
		// <Main>
		private static void u003cMainu003e(string[] args)
		{
			Program.Main(args).GetAwaiter().GetResult();
		}

		private static async Task Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
			await Program.sleep();
		}

		private static async Task sleep()
		{
			await Task.Delay(0x3e8);
		}
	}
}