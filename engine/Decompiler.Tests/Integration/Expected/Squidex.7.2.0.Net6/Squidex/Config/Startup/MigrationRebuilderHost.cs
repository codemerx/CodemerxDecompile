using Microsoft.Extensions.Hosting;
using Migrations;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Config.Startup
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class MigrationRebuilderHost : IHostedService
	{
		private readonly RebuildRunner rebuildRunner;

		public MigrationRebuilderHost(RebuildRunner rebuildRunner)
		{
			this.rebuildRunner = rebuildRunner;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return this.rebuildRunner.RunAsync(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}