using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Migrations.Migrations;
using Squidex.Infrastructure.Migrations;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class MigrationServices
	{
		[NullableContext(1)]
		public static void AddSquidexMigration(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<RebuildOptions>(services, config, "rebuild");
			DependencyInjectionExtensions.AddSingletonAs<Migrator>(services).AsSelf();
			DependencyInjectionExtensions.AddTransientAs<RebuildRunner>(services).AsSelf();
			DependencyInjectionExtensions.AddTransientAs<MigrationPath>(services).As<IMigrationPath>();
			DependencyInjectionExtensions.AddTransientAs<ConvertEventStore>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<ConvertEventStoreAppId>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<ClearRules>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<ClearSchemas>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<CreateAssetSlugs>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildContents>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildSnapshots>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildApps>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildSchemas>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildRules>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildAssets>(services).As<IMigration>();
			DependencyInjectionExtensions.AddTransientAs<RebuildAssetFolders>(services).As<IMigration>();
		}
	}
}