using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OrchardCore.BackgroundTasks;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	internal class ModularBackgroundService : BackgroundService
	{
		private readonly static TimeSpan PollingTime;

		private readonly static TimeSpan MinIdleTime;

		private readonly ConcurrentDictionary<string, BackgroundTaskScheduler> _schedulers;

		private readonly ConcurrentDictionary<string, IChangeToken> _changeTokens;

		private readonly IShellHost _shellHost;

		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly ILogger _logger;

		static ModularBackgroundService()
		{
			ModularBackgroundService.PollingTime = TimeSpan.FromMinutes(1);
			ModularBackgroundService.MinIdleTime = TimeSpan.FromSeconds(10);
			return;
		}

		public ModularBackgroundService(IShellHost shellHost, IHttpContextAccessor httpContextAccessor, ILogger<ModularBackgroundService> logger)
		{
			this._schedulers = new ConcurrentDictionary<string, BackgroundTaskScheduler>();
			this._changeTokens = new ConcurrentDictionary<string, IChangeToken>();
			base();
			this._shellHost = shellHost;
			this._httpContextAccessor = httpContextAccessor;
			this._logger = logger;
			return;
		}

		private void CleanSchedulers(string tenant, IEnumerable<IBackgroundTask> tasks)
		{
			V_0 = new ModularBackgroundService.u003cu003ec__DisplayClass17_0();
			V_0.tenant = tenant;
			V_1 = Enumerable.ToArray<string>(Enumerable.Select<IBackgroundTask, string>(tasks, new Func<IBackgroundTask, string>(V_0, ModularBackgroundService.u003cu003ec__DisplayClass17_0.u003cCleanSchedulersu003eb__0)));
			stackVariable14 = Enumerable.Where<KeyValuePair<string, BackgroundTaskScheduler>>(this._schedulers, new Func<KeyValuePair<string, BackgroundTaskScheduler>, bool>(V_0, ModularBackgroundService.u003cu003ec__DisplayClass17_0.u003cCleanSchedulersu003eb__1));
			stackVariable15 = ModularBackgroundService.u003cu003ec.u003cu003e9__17_2;
			if (stackVariable15 == null)
			{
				dummyVar0 = stackVariable15;
				stackVariable15 = new Func<KeyValuePair<string, BackgroundTaskScheduler>, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cCleanSchedulersu003eb__17_2);
				ModularBackgroundService.u003cu003ec.u003cu003e9__17_2 = stackVariable15;
			}
			V_2 = Enumerable.ToArray<string>(Enumerable.Select<KeyValuePair<string, BackgroundTaskScheduler>, string>(stackVariable14, stackVariable15));
			V_3 = 0;
			while (V_3 < (int)V_2.Length)
			{
				V_4 = V_2[V_3];
				if (!Enumerable.Contains<string>(V_1, V_4))
				{
					dummyVar1 = this._schedulers.TryRemove(V_4, ref V_5);
				}
				V_3 = V_3 + 1;
			}
			return;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			V_0.u003cu003e4__this = this;
			V_0.stoppingToken = stoppingToken;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularBackgroundService.u003cExecuteAsyncu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private IEnumerable<ShellContext> GetRunningShells()
		{
			stackVariable2 = this._shellHost.ListShellContexts();
			stackVariable3 = ModularBackgroundService.u003cu003ec.u003cu003e9__12_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<ShellContext, bool>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetRunningShellsu003eb__12_0);
				ModularBackgroundService.u003cu003ec.u003cu003e9__12_0 = stackVariable3;
			}
			return Enumerable.ToArray<ShellContext>(Enumerable.Where<ShellContext>(stackVariable2, stackVariable3));
		}

		private IEnumerable<BackgroundTaskScheduler> GetSchedulersToRun(string tenant)
		{
			V_0 = new ModularBackgroundService.u003cu003ec__DisplayClass15_0();
			V_0.tenant = tenant;
			stackVariable8 = Enumerable.Where<KeyValuePair<string, BackgroundTaskScheduler>>(this._schedulers, new Func<KeyValuePair<string, BackgroundTaskScheduler>, bool>(V_0, ModularBackgroundService.u003cu003ec__DisplayClass15_0.u003cGetSchedulersToRunu003eb__0));
			stackVariable9 = ModularBackgroundService.u003cu003ec.u003cu003e9__15_1;
			if (stackVariable9 == null)
			{
				dummyVar0 = stackVariable9;
				stackVariable9 = new Func<KeyValuePair<string, BackgroundTaskScheduler>, BackgroundTaskScheduler>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetSchedulersToRunu003eb__15_1);
				ModularBackgroundService.u003cu003ec.u003cu003e9__15_1 = stackVariable9;
			}
			return Enumerable.ToArray<BackgroundTaskScheduler>(Enumerable.Select<KeyValuePair<string, BackgroundTaskScheduler>, BackgroundTaskScheduler>(stackVariable8, stackVariable9));
		}

		private IEnumerable<ShellContext> GetShellsToRun(IEnumerable<ShellContext> shells)
		{
			V_0 = new ModularBackgroundService.u003cu003ec__DisplayClass13_0();
			stackVariable1 = V_0;
			stackVariable3 = this._schedulers;
			stackVariable4 = ModularBackgroundService.u003cu003ec.u003cu003e9__13_0;
			if (stackVariable4 == null)
			{
				dummyVar0 = stackVariable4;
				stackVariable4 = new Func<KeyValuePair<string, BackgroundTaskScheduler>, bool>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToRunu003eb__13_0);
				ModularBackgroundService.u003cu003ec.u003cu003e9__13_0 = stackVariable4;
			}
			stackVariable5 = Enumerable.Where<KeyValuePair<string, BackgroundTaskScheduler>>(stackVariable3, stackVariable4);
			stackVariable6 = ModularBackgroundService.u003cu003ec.u003cu003e9__13_1;
			if (stackVariable6 == null)
			{
				dummyVar1 = stackVariable6;
				stackVariable6 = new Func<KeyValuePair<string, BackgroundTaskScheduler>, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToRunu003eb__13_1);
				ModularBackgroundService.u003cu003ec.u003cu003e9__13_1 = stackVariable6;
			}
			stackVariable1.tenantsToRun = Enumerable.ToArray<string>(Enumerable.Distinct<string>(Enumerable.Select<KeyValuePair<string, BackgroundTaskScheduler>, string>(stackVariable5, stackVariable6)));
			return Enumerable.ToArray<ShellContext>(Enumerable.Where<ShellContext>(shells, new Func<ShellContext, bool>(V_0, ModularBackgroundService.u003cu003ec__DisplayClass13_0.u003cGetShellsToRunu003eb__2)));
		}

		private IEnumerable<ShellContext> GetShellsToUpdate(IEnumerable<ShellContext> previousShells, IEnumerable<ShellContext> runningShells)
		{
			V_0 = new ModularBackgroundService.u003cu003ec__DisplayClass14_0();
			stackVariable1 = previousShells;
			stackVariable2 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ShellContext, bool>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_0);
				ModularBackgroundService.u003cu003ec.u003cu003e9__14_0 = stackVariable2;
			}
			stackVariable3 = Enumerable.Where<ShellContext>(stackVariable1, stackVariable2);
			stackVariable4 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Func<ShellContext, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_1);
				ModularBackgroundService.u003cu003ec.u003cu003e9__14_1 = stackVariable4;
			}
			V_1 = Enumerable.ToArray<string>(Enumerable.Select<ShellContext, string>(stackVariable3, stackVariable4));
			if (Enumerable.Any<string>(V_1))
			{
				stackVariable52 = V_1;
				stackVariable53 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_2;
				if (stackVariable53 == null)
				{
					dummyVar2 = stackVariable53;
					stackVariable53 = new Action<BackgroundTaskScheduler>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_2);
					ModularBackgroundService.u003cu003ec.u003cu003e9__14_2 = stackVariable53;
				}
				this.UpdateSchedulers(stackVariable52, stackVariable53);
			}
			stackVariable10 = this._changeTokens;
			stackVariable11 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_3;
			if (stackVariable11 == null)
			{
				dummyVar3 = stackVariable11;
				stackVariable11 = new Func<KeyValuePair<string, IChangeToken>, bool>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_3);
				ModularBackgroundService.u003cu003ec.u003cu003e9__14_3 = stackVariable11;
			}
			stackVariable12 = Enumerable.Where<KeyValuePair<string, IChangeToken>>(stackVariable10, stackVariable11);
			stackVariable13 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_4;
			if (stackVariable13 == null)
			{
				dummyVar4 = stackVariable13;
				stackVariable13 = new Func<KeyValuePair<string, IChangeToken>, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_4);
				ModularBackgroundService.u003cu003ec.u003cu003e9__14_4 = stackVariable13;
			}
			V_2 = Enumerable.ToArray<string>(Enumerable.Select<KeyValuePair<string, IChangeToken>, string>(stackVariable12, stackVariable13));
			if (Enumerable.Any<string>(V_2))
			{
				stackVariable43 = V_2;
				stackVariable44 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_5;
				if (stackVariable44 == null)
				{
					dummyVar5 = stackVariable44;
					stackVariable44 = new Action<BackgroundTaskScheduler>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_5);
					ModularBackgroundService.u003cu003ec.u003cu003e9__14_5 = stackVariable44;
				}
				this.UpdateSchedulers(stackVariable43, stackVariable44);
			}
			stackVariable18 = previousShells;
			stackVariable19 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_6;
			if (stackVariable19 == null)
			{
				dummyVar6 = stackVariable19;
				stackVariable19 = new Func<ShellContext, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_6);
				ModularBackgroundService.u003cu003ec.u003cu003e9__14_6 = stackVariable19;
			}
			V_3 = Enumerable.Except<string>(Enumerable.Except<string>(Enumerable.Select<ShellContext, string>(stackVariable18, stackVariable19), V_1), V_2);
			stackVariable25 = V_0;
			stackVariable26 = runningShells;
			stackVariable27 = ModularBackgroundService.u003cu003ec.u003cu003e9__14_7;
			if (stackVariable27 == null)
			{
				dummyVar7 = stackVariable27;
				stackVariable27 = new Func<ShellContext, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cGetShellsToUpdateu003eb__14_7);
				ModularBackgroundService.u003cu003ec.u003cu003e9__14_7 = stackVariable27;
			}
			stackVariable25.tenantsToUpdate = Enumerable.ToArray<string>(Enumerable.Except<string>(Enumerable.Select<ShellContext, string>(stackVariable26, stackVariable27), V_3));
			return Enumerable.ToArray<ShellContext>(Enumerable.Where<ShellContext>(runningShells, new Func<ShellContext, bool>(V_0, ModularBackgroundService.u003cu003ec__DisplayClass14_0.u003cGetShellsToUpdateu003eb__8)));
		}

		private async Task RunAsync(IEnumerable<ShellContext> runningShells, CancellationToken stoppingToken)
		{
			V_0.u003cu003e4__this = this;
			V_0.runningShells = runningShells;
			V_0.stoppingToken = stoppingToken;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularBackgroundService.u003cRunAsyncu003ed__9>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task UpdateAsync(IEnumerable<ShellContext> previousShells, IEnumerable<ShellContext> runningShells, CancellationToken stoppingToken)
		{
			V_0.u003cu003e4__this = this;
			V_0.previousShells = previousShells;
			V_0.runningShells = runningShells;
			V_0.stoppingToken = stoppingToken;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularBackgroundService.u003cUpdateAsyncu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void UpdateSchedulers(IEnumerable<string> tenants, Action<BackgroundTaskScheduler> action)
		{
			V_0 = new ModularBackgroundService.u003cu003ec__DisplayClass16_0();
			V_0.tenants = tenants;
			stackVariable8 = Enumerable.Where<KeyValuePair<string, BackgroundTaskScheduler>>(this._schedulers, new Func<KeyValuePair<string, BackgroundTaskScheduler>, bool>(V_0, ModularBackgroundService.u003cu003ec__DisplayClass16_0.u003cUpdateSchedulersu003eb__0));
			stackVariable9 = ModularBackgroundService.u003cu003ec.u003cu003e9__16_1;
			if (stackVariable9 == null)
			{
				dummyVar0 = stackVariable9;
				stackVariable9 = new Func<KeyValuePair<string, BackgroundTaskScheduler>, string>(ModularBackgroundService.u003cu003ec.u003cu003e9, ModularBackgroundService.u003cu003ec.u003cUpdateSchedulersu003eb__16_1);
				ModularBackgroundService.u003cu003ec.u003cu003e9__16_1 = stackVariable9;
			}
			V_1 = Enumerable.ToArray<string>(Enumerable.Select<KeyValuePair<string, BackgroundTaskScheduler>, string>(stackVariable8, stackVariable9));
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				if (this._schedulers.TryGetValue(V_3, ref V_4))
				{
					action.Invoke(V_4);
				}
				V_2 = V_2 + 1;
			}
			return;
		}

		private async Task WaitAsync(Task pollingDelay, CancellationToken stoppingToken)
		{
			V_0.pollingDelay = pollingDelay;
			V_0.stoppingToken = stoppingToken;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularBackgroundService.u003cWaitAsyncu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}