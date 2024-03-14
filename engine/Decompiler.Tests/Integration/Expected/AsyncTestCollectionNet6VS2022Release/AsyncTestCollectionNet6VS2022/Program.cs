using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AsyncTestCollectionNet6VS2022
{
	[Nullable(0)]
	[NullableContext(1)]
	internal class Program
	{
		public Program()
		{
		}

		private static async Task AwaitInCatch()
		{
			int num = 0;
			try
			{
			}
			catch
			{
				num = 1;
			}
			if (num == 1)
			{
				await Task.Delay(5);
			}
		}

		private static async Task AwaitInFinally()
		{
			object obj;
			object obj1 = null;
			try
			{
			}
			catch
			{
				obj = obj2;
				obj1 = obj;
			}
			await Task.Delay(5);
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
		}

		private static async Task AwaitInTry()
		{
			try
			{
				await Task.Delay(5);
			}
			catch
			{
				Console.WriteLine("in catch");
			}
		}

		private static async Task AwaitInTryInForeach(IEnumerable<int> durations)
		{
			foreach (int duration in durations)
			{
				try
				{
					await Task.Delay(duration);
				}
				catch
				{
					Console.WriteLine("in catch");
				}
			}
		}

		private static async Task FiniteWhileForForeach(IEnumerable<int> durations, IEnumerable<int> another)
		{
			while (another.Any<int>())
			{
				for (int i = 0; i < 5; i++)
				{
					foreach (int duration in durations)
					{
						await Task.Delay(duration + i);
					}
				}
			}
		}

		private static async Task For()
		{
			for (int i = 0; i < 5; i++)
			{
				await Task.Delay(i);
			}
		}

		private static async Task Foreach(IEnumerable<int> durations)
		{
			foreach (int duration in durations)
			{
				await Task.Delay(duration);
			}
		}

		private static async Task ForeachFor(IEnumerable<int> durations, IEnumerable<int> another)
		{
			foreach (int duration in durations)
			{
				for (int i = 0; i < 5; i++)
				{
					await Task.Delay(duration + i);
				}
			}
		}

		private static async Task ForeachForeach(IEnumerable<int> durations, IEnumerable<int> another)
		{
			foreach (int duration in durations)
			{
				foreach (int num in another)
				{
					await Task.Delay(duration + num);
				}
			}
		}

		private static async Task ForeachForeachForeach(IEnumerable<int> durations, IEnumerable<int> another)
		{
			foreach (int duration in durations)
			{
				foreach (int num in durations)
				{
					foreach (int duration1 in durations)
					{
						await Task.Delay(duration + num + duration1);
					}
				}
			}
		}

		private static async Task ForeachMultiAwait(IEnumerable<int> durations, IEnumerable<int> another)
		{
			foreach (int duration in durations)
			{
				await Task.Delay(duration);
				await Task.Delay(duration);
				await Task.Delay(duration);
			}
		}

		private static async Task ForeachOnResultOfAwait()
		{
			foreach (int durationsAsync in await Program.GetDurationsAsync())
			{
				await Task.Delay(durationsAsync);
			}
		}

		private static async Task ForeachOnResultOfMultiAwait()
		{
			IEnumerable<int> durationsAsync = await Program.GetDurationsAsync();
			IEnumerable<int> nums = await Program.GetDurationsAsync();
			IEnumerable<int> durationsAsync1 = await Program.GetDurationsAsync();
			foreach (int num in durationsAsync)
			{
				foreach (int num1 in nums)
				{
					foreach (int num2 in durationsAsync1)
					{
						await Task.Delay(num);
						await Task.Delay(num1);
						await Task.Delay(num2);
					}
				}
			}
			durationsAsync = null;
			nums = null;
			durationsAsync1 = null;
		}

		private static async Task ForeachTryCatchAwait(IEnumerable<int> durations, IEnumerable<int> another)
		{
			foreach (int duration in durations)
			{
				int num = 0;
				try
				{
					await Task.Delay(duration);
				}
				catch
				{
					num = 1;
				}
				if (num != 1)
				{
					continue;
				}
				await Task.Delay(duration);
			}
		}

		private static async Task ForForeach(IEnumerable<int> durations, IEnumerable<int> another)
		{
			for (int i = 0; i < 5; i++)
			{
				foreach (int duration in durations)
				{
					await Task.Delay(duration + i);
				}
			}
		}

		private static async Task ForMultiAwait()
		{
			for (int i = 0; i < 5; i++)
			{
				await Task.Delay(i);
				await Task.Delay(i + 1);
				await Task.Delay(i + 2);
			}
		}

		private static async Task<IEnumerable<int>> GetDurationsAsync()
		{
			return null;
		}

		private static async Task InfiniteWhileForForeach(IEnumerable<int> durations, IEnumerable<int> another)
		{
			Program.u003cInfiniteWhileForForeachu003ed__10 variable = new Program.u003cInfiniteWhileForForeachu003ed__10();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.durations = durations;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Program.u003cInfiniteWhileForForeachu003ed__10>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private static void Main(string[] args)
		{
		}

		private static async Task MultiAwaitInTry()
		{
			try
			{
				await Task.Delay(5);
				await Task.Delay(5);
				await Task.Delay(5);
			}
			catch
			{
				Console.WriteLine("in catch");
			}
		}

		private static async Task MultiAwaitInTryInForeach(IEnumerable<int> durations)
		{
			foreach (int duration in durations)
			{
				try
				{
					await Task.Delay(duration);
					await Task.Delay(duration);
					await Task.Delay(duration);
				}
				catch
				{
					Console.WriteLine("in catch");
				}
			}
		}

		private static async Task NestedForeachMultiAwait(IEnumerable<int> durations, IEnumerable<int> another)
		{
			foreach (int duration in durations)
			{
				await Task.Delay(duration);
				foreach (int num in durations)
				{
					await Task.Delay(num);
					foreach (int duration1 in durations)
					{
						await Task.Delay(duration1);
					}
				}
			}
		}

		private static async Task TryCatchFinallyAwait()
		{
			object obj;
			object obj1 = null;
			try
			{
				int num = 0;
				try
				{
					await Task.Delay(1);
				}
				catch
				{
					num = 1;
				}
				if (num == 1)
				{
					await Task.Delay(2);
				}
			}
			catch
			{
				obj = obj3;
				obj1 = obj;
			}
			await Task.Delay(3);
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
		}

		private static async Task TryCatchFinallyAwaitV2()
		{
			object obj;
			await Task.Delay(1);
			object obj1 = null;
			try
			{
				int num = 0;
				try
				{
					await Task.Delay(2);
				}
				catch
				{
					num = 1;
				}
				if (num == 1)
				{
					await Task.Delay(3);
				}
			}
			catch
			{
				obj = obj3;
				obj1 = obj;
			}
			await Task.Delay(4);
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
			await Task.Delay(5);
		}
	}
}