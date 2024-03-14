using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;
using Squidex.Hosting;
using Squidex.Infrastructure.Timers;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Config
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TokenStoreInitializer : IInitializable, ISystem, IBackgroundProcess
	{
		private readonly OpenIddictMongoDbOptions options;

		private readonly IServiceProvider serviceProvider;

		private CompletionTimer timer;

		public TokenStoreInitializer(IOptions<OpenIddictMongoDbOptions> options, IServiceProvider serviceProvider)
		{
			this.options = options.get_Value();
			this.serviceProvider = serviceProvider;
		}

		public async Task InitializeAsync(CancellationToken ct)
		{
			await this.SetupIndexAsync(ct);
		}

		private async Task PruneAsync(CancellationToken ct)
		{
			TokenStoreInitializer.u003cPruneAsyncu003ed__7 variable = new TokenStoreInitializer.u003cPruneAsyncu003ed__7();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e4__this = this;
			variable.ct = ct;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<TokenStoreInitializer.u003cPruneAsyncu003ed__7>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task SetupIndexAsync(CancellationToken ct)
		{
			TokenStoreInitializer.u003cSetupIndexAsyncu003ed__8 variable = new TokenStoreInitializer.u003cSetupIndexAsyncu003ed__8();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e4__this = this;
			variable.ct = ct;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<TokenStoreInitializer.u003cSetupIndexAsyncu003ed__8>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public async Task StartAsync(CancellationToken ct)
		{
			TimeSpan timeSpan = TimeSpan.FromHours(6);
			this.timer = new CompletionTimer((int)timeSpan.TotalMilliseconds, new Func<CancellationToken, Task>(this.PruneAsync), 0);
			await this.PruneAsync(ct);
		}

		public Task StopAsync(CancellationToken ct)
		{
			object completedTask;
			CompletionTimer completionTimer = this.timer;
			if (completionTimer != null)
			{
				completedTask = completionTimer.StopAsync();
			}
			else
			{
				completedTask = null;
			}
			if (completedTask == null)
			{
				completedTask = Task.CompletedTask;
			}
			return completedTask;
		}
	}
}