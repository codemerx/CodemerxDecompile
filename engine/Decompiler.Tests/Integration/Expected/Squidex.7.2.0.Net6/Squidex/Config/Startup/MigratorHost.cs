using Microsoft.Extensions.Hosting;
using Squidex.Infrastructure.Migrations;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Config.Startup
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class MigratorHost : IHostedService
	{
		private readonly Migrator migrator;

		public MigratorHost(Migrator migrator)
		{
			this.migrator = migrator;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return this.migrator.MigrateAsync(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}