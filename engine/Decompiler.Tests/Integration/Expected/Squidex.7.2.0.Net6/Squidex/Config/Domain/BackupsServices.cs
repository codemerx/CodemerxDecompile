using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Schemas;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class BackupsServices
	{
		[NullableContext(1)]
		public static void AddSquidexBackups(IServiceCollection services)
		{
			DependencyInjectionExtensions.AddSingletonAs<TempFolderBackupArchiveLocation>(services).As<IBackupArchiveLocation>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultBackupHandlerFactory>(services).As<IBackupHandlerFactory>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultBackupArchiveStore>(services).As<IBackupArchiveStore>();
			DependencyInjectionExtensions.AddTransientAs<BackupService>(services).As<IBackupService>().As<IDeleter>();
			DependencyInjectionExtensions.AddTransientAs<BackupApps>(services).As<IBackupHandler>();
			DependencyInjectionExtensions.AddTransientAs<BackupAssets>(services).As<IBackupHandler>();
			DependencyInjectionExtensions.AddTransientAs<BackupContents>(services).As<IBackupHandler>();
			DependencyInjectionExtensions.AddTransientAs<BackupRules>(services).As<IBackupHandler>();
			DependencyInjectionExtensions.AddTransientAs<BackupSchemas>(services).As<IBackupHandler>();
			DependencyInjectionExtensions.AddTransientAs<DefaultBackupHandlerFactory>(services).As<IBackupHandlerFactory>();
			DependencyInjectionExtensions.AddTransientAs<RestoreProcessor>(services).AsSelf();
		}
	}
}