using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OrchardCore.BackgroundTasks;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	internal class ModularBackgroundService : BackgroundService
	{
		private readonly static TimeSpan PollingTime;

		private readonly static TimeSpan MinIdleTime;

		private readonly ConcurrentDictionary<string, BackgroundTaskScheduler> _schedulers = new ConcurrentDictionary<string, BackgroundTaskScheduler>();

		private readonly ConcurrentDictionary<string, IChangeToken> _changeTokens = new ConcurrentDictionary<string, IChangeToken>();

		private readonly IShellHost _shellHost;

		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly ILogger _logger;

		static ModularBackgroundService()
		{
			ModularBackgroundService.PollingTime = TimeSpan.FromMinutes(1);
			ModularBackgroundService.MinIdleTime = TimeSpan.FromSeconds(10);
		}

		public ModularBackgroundService(IShellHost shellHost, IHttpContextAccessor httpContextAccessor, ILogger<ModularBackgroundService> logger)
		{
			this._shellHost = shellHost;
			this._httpContextAccessor = httpContextAccessor;
			this._logger = logger;
		}

		private void CleanSchedulers(string tenant, IEnumerable<IBackgroundTask> tasks)
		{
			BackgroundTaskScheduler backgroundTaskScheduler;
			string[] array = (
				from task in tasks
				select string.Concat(tenant, BackgroundTaskExtensions.GetTaskName(task))).ToArray<string>();
			string[] strArrays = (
				from kv in this._schedulers
				where kv.Value.Tenant == tenant
				select kv.Key).ToArray<string>();
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!array.Contains<string>(str))
				{
					this._schedulers.TryRemove(str, out backgroundTaskScheduler);
				}
			}
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Exception exception = null;
			bool flag;
			stoppingToken.Register(() => LoggerExtensions.LogInformation(this._logger, "'{ServiceName}' is stopping.", new object[] { "ModularBackgroundService" }));
			while (this.GetRunningShells().Count<ShellContext>() < 1)
			{
				try
				{
					await Task.Delay(ModularBackgroundService.MinIdleTime, stoppingToken);
				}
				catch (TaskCanceledException taskCanceledException)
				{
					break;
				}
			}
			IEnumerable<ShellContext> shellContexts = Enumerable.Empty<ShellContext>();
			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					IEnumerable<ShellContext> runningShells = this.GetRunningShells();
					await this.UpdateAsync(shellContexts, runningShells, stoppingToken);
					shellContexts = runningShells;
					Task task = Task.Delay(ModularBackgroundService.PollingTime, stoppingToken);
					await this.RunAsync(runningShells, stoppingToken);
					await this.WaitAsync(task, stoppingToken);
					runningShells = null;
					task = null;
				}
			}
			catch// 
			// Current member / type: System.Threading.Tasks.Task OrchardCore.Modules.ModularBackgroundService::ExecuteAsync(System.Threading.CancellationToken)
			// Exception in: System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private IEnumerable<ShellContext> GetRunningShells()
		{
			return this._shellHost.ListShellContexts().Where<ShellContext>((ShellContext s) => {
				if (s.get_Settings().get_State() != 2)
				{
					return false;
				}
				return s.get_Pipeline() != null;
			}).ToArray<ShellContext>();
		}

		private IEnumerable<BackgroundTaskScheduler> GetSchedulersToRun(string tenant)
		{
			return this._schedulers.Where<KeyValuePair<string, BackgroundTaskScheduler>>((KeyValuePair<string, BackgroundTaskScheduler> s) => {
				if (s.Value.Tenant != tenant)
				{
					return false;
				}
				return s.Value.CanRun();
			}).Select<KeyValuePair<string, BackgroundTaskScheduler>, BackgroundTaskScheduler>((KeyValuePair<string, BackgroundTaskScheduler> s) => s.Value).ToArray<BackgroundTaskScheduler>();
		}

		private IEnumerable<ShellContext> GetShellsToRun(IEnumerable<ShellContext> shells)
		{
			string[] array = (
				from s in this._schedulers
				where s.Value.CanRun()
				select s.Value.Tenant).Distinct<string>().ToArray<string>();
			return (
				from s in shells
				where array.Contains<string>(s.get_Settings().get_Name())
				select s).ToArray<ShellContext>();
		}

		private IEnumerable<ShellContext> GetShellsToUpdate(IEnumerable<ShellContext> previousShells, IEnumerable<ShellContext> runningShells)
		{
			string[] array = (
				from s in previousShells
				where s.get_Released()
				select s.get_Settings().get_Name()).ToArray<string>();
			if (array.Any<string>())
			{
				this.UpdateSchedulers(array, (BackgroundTaskScheduler s) => s.Released = true);
			}
			string[] strArrays = (
				from t in this._changeTokens
				where t.Value.get_HasChanged()
				select t.Key).ToArray<string>();
			if (strArrays.Any<string>())
			{
				this.UpdateSchedulers(strArrays, (BackgroundTaskScheduler s) => s.Updated = false);
			}
			IEnumerable<string> strs = (
				from s in previousShells
				select s.get_Settings().get_Name()).Except<string>(array).Except<string>(strArrays);
			string[] array1 = (
				from s in runningShells
				select s.get_Settings().get_Name()).Except<string>(strs).ToArray<string>();
			return (
				from s in runningShells
				where array1.Contains<string>(s.get_Settings().get_Name())
				select s).ToArray<ShellContext>();
		}

		private async Task RunAsync(IEnumerable<ShellContext> runningShells, CancellationToken stoppingToken)
		{
			await EnumerableExtensions.ForEachAsync<ShellContext>(this.GetShellsToRun(runningShells), async (ShellContext shell) => {
				string str = shell.get_Settings().get_Name();
				IEnumerable<BackgroundTaskScheduler> schedulersToRun = this.GetSchedulersToRun(str);
				this._httpContextAccessor.set_HttpContext(shell.CreateHttpContext());
				foreach (BackgroundTaskScheduler backgroundTaskScheduler in schedulersToRun)
				{
					if (stoppingToken.IsCancellationRequested)
					{
						break;
					}
					ShellScope scopeAsync = await this._shellHost.GetScopeAsync(shell.get_Settings());
					if (scopeAsync.get_ShellContext().get_Pipeline() == null)
					{
						break;
					}
					await scopeAsync.UsingAsync(async (ShellScope scope) => {
						Exception exception = null;
						bool flag;
						string name = backgroundTaskScheduler.Name;
						IBackgroundTask taskByName = BackgroundTaskExtensions.GetTaskByName(ServiceProviderServiceExtensions.GetServices<IBackgroundTask>(scope.get_ServiceProvider()), name);
						if (taskByName != null)
						{
							try
							{
								LoggerExtensions.LogInformation(this._logger, "Start processing background task '{TaskName}' on tenant '{TenantName}'.", new object[] { name, str });
								backgroundTaskScheduler.Run();
								await taskByName.DoWorkAsync(scope.get_ServiceProvider(), stoppingToken);
								LoggerExtensions.LogInformation(this._logger, "Finished processing background task '{TaskName}' on tenant '{TenantName}'.", new object[] { name, str });
							}
							catch// 
							// Current member / type: System.Threading.Tasks.Task OrchardCore.Modules.ModularBackgroundService::RunAsync(System.Collections.Generic.IEnumerable`1<OrchardCore.Environment.Shell.Builders.ShellContext>,System.Threading.CancellationToken)
							// Exception in: System.Threading.Tasks.Task RunAsync(System.Collections.Generic.IEnumerable<OrchardCore.Environment.Shell.Builders.ShellContext>,System.Threading.CancellationToken)
							// Object reference not set to an instance of an object.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		private async Task UpdateAsync(IEnumerable<ShellContext> previousShells, IEnumerable<ShellContext> runningShells, CancellationToken stoppingToken)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task OrchardCore.Modules.ModularBackgroundService::UpdateAsync(System.Collections.Generic.IEnumerable`1<OrchardCore.Environment.Shell.Builders.ShellContext>,System.Collections.Generic.IEnumerable`1<OrchardCore.Environment.Shell.Builders.ShellContext>,System.Threading.CancellationToken)
			// Exception in: System.Threading.Tasks.Task UpdateAsync(System.Collections.Generic.IEnumerable<OrchardCore.Environment.Shell.Builders.ShellContext>,System.Collections.Generic.IEnumerable<OrchardCore.Environment.Shell.Builders.ShellContext>,System.Threading.CancellationToken)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void UpdateSchedulers(IEnumerable<string> tenants, Action<BackgroundTaskScheduler> action)
		{
			BackgroundTaskScheduler backgroundTaskScheduler;
			string[] array = (
				from kv in this._schedulers
				where tenants.Contains<string>(kv.Value.Tenant)
				select kv.Key).ToArray<string>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				string str = array[i];
				if (this._schedulers.TryGetValue(str, out backgroundTaskScheduler))
				{
					action(backgroundTaskScheduler);
				}
			}
		}

		private async Task WaitAsync(Task pollingDelay, CancellationToken stoppingToken)
		{
			try
			{
				await Task.Delay(ModularBackgroundService.MinIdleTime, stoppingToken);
				await pollingDelay;
			}
			catch (OperationCanceledException operationCanceledException)
			{
			}
		}
	}
}