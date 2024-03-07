using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace AsyncEnumTest
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

		private static async Task loop()
		{
			object obj;
			IAsyncEnumerable<int> asyncEnumerable = Program.RangeAsync(0, 5);
			IAsyncEnumerator<int> asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(new CancellationToken());
			object obj1 = null;
			int num = 0;
			try
			{
				while (true)
				{
					if (!await asyncEnumerator.MoveNextAsync())
					{
						break;
					}
					Console.WriteLine(asyncEnumerator.Current);
				}
			}
			catch
			{
				obj = obj2;
				obj1 = obj;
			}
			if (asyncEnumerator != null)
			{
				await asyncEnumerator.DisposeAsync();
			}
			obj = obj1;
			if (obj != null)
			{
				Exception exception = obj as Exception;
				if (exception == null)
				{
					throw obj;
				}
				ExceptionDispatchInfo.Capture(exception).Throw();
			}
			obj1 = null;
			asyncEnumerator = null;
		}

		private static async Task Main(string[] args)
		{
			await Program.loop();
		}

		[AsyncIteratorStateMachine(typeof(Program.u003cRangeAsyncu003ed__1))]
		private static IAsyncEnumerable<int> RangeAsync(int start, int count)
		{
			return new Program.u003cRangeAsyncu003ed__1(-2)
			{
				u003cu003e3__start = start,
				u003cu003e3__count = count
			};
		}
	}
}