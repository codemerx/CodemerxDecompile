using FluentFTP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Squidex.Assets;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Assets.Queries;
using Squidex.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Config.Domain
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class AssetServices
	{
		public static void AddSquidexAssetInfrastructure(this IServiceCollection services, IConfiguration config)
		{
			IConfiguration configuration = config;
			Alternatives alternative = new Alternatives();
			alternative["Default"] = () => DependencyInjectionExtensions.AddSingletonAs<NoopAssetStore>(services).AsOptional<IAssetStore>();
			alternative["Folder"] = () => {
				string requiredValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:folder:path");
				DependencyInjectionExtensions.AddSingletonAs<FolderAssetStore>(services, (IServiceProvider c) => new FolderAssetStore(requiredValue, ServiceProviderServiceExtensions.GetRequiredService<ILogger<FolderAssetStore>>(c))).As<IAssetStore>();
			};
			alternative["GoogleCloud"] = () => {
				GoogleCloudAssetOptions googleCloudAssetOption = new GoogleCloudAssetOptions();
				googleCloudAssetOption.set_BucketName(Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:googleCloud:bucket"));
				GoogleCloudAssetOptions googleCloudAssetOption1 = googleCloudAssetOption;
				DependencyInjectionExtensions.AddSingletonAs<GoogleCloudAssetStore>(services, (IServiceProvider c) => new GoogleCloudAssetStore(googleCloudAssetOption1)).As<IAssetStore>();
			};
			alternative["AzureBlob"] = () => {
				AzureBlobAssetOptions azureBlobAssetOption = new AzureBlobAssetOptions();
				azureBlobAssetOption.set_ConnectionString(Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:azureBlob:connectionString"));
				azureBlobAssetOption.set_ContainerName(Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:azureBlob:containerName"));
				AzureBlobAssetOptions azureBlobAssetOption1 = azureBlobAssetOption;
				DependencyInjectionExtensions.AddSingletonAs<AzureBlobAssetStore>(services, (IServiceProvider c) => new AzureBlobAssetStore(azureBlobAssetOption1)).As<IAssetStore>();
			};
			alternative["AmazonS3"] = () => {
				AmazonS3AssetOptions amazonS3AssetOption = ConfigurationBinder.Get<AmazonS3AssetOptions>(config.GetSection("assetStore:amazonS3")) ?? new AmazonS3AssetOptions();
				DependencyInjectionExtensions.AddSingletonAs<AmazonS3AssetStore>(services, (IServiceProvider c) => new AmazonS3AssetStore(amazonS3AssetOption)).As<IAssetStore>();
			};
			alternative["MongoDb"] = () => {
				string requiredValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:mongoDb:configuration");
				string str = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:mongoDb:database");
				string requiredValue1 = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:mongoDb:bucket");
				DependencyInjectionExtensions.AddSingletonAs<MongoGridFsAssetStore>(services, (IServiceProvider c) => {
					IMongoDatabase database = StoreServices.GetMongoClient(requiredValue).GetDatabase(str, null);
					GridFSBucketOptions gridFSBucketOption = new GridFSBucketOptions();
					gridFSBucketOption.set_BucketName(requiredValue1);
					return new MongoGridFsAssetStore(new GridFSBucket<string>(database, gridFSBucketOption));
				}).As<IAssetStore>();
			};
			alternative["Ftp"] = () => {
				Func<FtpClient> func1 = null;
				string requiredValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:ftp:serverHost");
				int optionalValue = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetOptionalValue<int>(config, "assetStore:ftp:serverPort", 21);
				string str = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:ftp:username");
				string requiredValue1 = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetRequiredValue(config, "assetStore:ftp:password");
				FTPAssetOptions fTPAssetOption = new FTPAssetOptions();
				fTPAssetOption.set_Path(Microsoft.Extensions.Configuration.ConfigurationExtensions.GetOptionalValue<string>(config, "assetStore:ftp:path", "/"));
				FTPAssetOptions fTPAssetOption1 = fTPAssetOption;
				DependencyInjectionExtensions.AddSingletonAs<FTPAssetStore>(services, (IServiceProvider c) => {
					Func<FtpClient> u003cu003e9_14 = func1;
					if (u003cu003e9_14 == null)
					{
						Func<FtpClient> ftpClient = () => new FtpClient(requiredValue, optionalValue, str, requiredValue1);
						Func<FtpClient> func = ftpClient;
						func1 = ftpClient;
						u003cu003e9_14 = func;
					}
					return new FTPAssetStore(u003cu003e9_14, fTPAssetOption1, ServiceProviderServiceExtensions.GetRequiredService<ILogger<FTPAssetStore>>(c));
				}).As<IAssetStore>();
			};
			Microsoft.Extensions.Configuration.ConfigurationExtensions.ConfigureByOption(configuration, "assetStore:type", alternative);
			DependencyInjectionExtensions.AddSingletonAs<IInitializable>(services, (IServiceProvider c) => {
				IAssetStore requiredService = ServiceProviderServiceExtensions.GetRequiredService<IAssetStore>(c);
				IAssetStore assetStore = requiredService;
				return new DelegateInitializer(requiredService.GetType().Name, new Func<CancellationToken, Task>(assetStore.InitializeAsync));
			});
		}

		public static void AddSquidexAssets(this IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<AssetOptions>(services, config, "assets");
			if (ConfigurationBinder.GetValue<bool>(config, "assets:deleteRecursive"))
			{
				DependencyInjectionExtensions.AddTransientAs<RecursiveDeleter>(services).As<IEventConsumer>();
			}
			if (ConfigurationBinder.GetValue<bool>(config, "assets:deletePermanent"))
			{
				DependencyInjectionExtensions.AddTransientAs<AssetPermanentDeleter>(services).As<IEventConsumer>();
			}
			DependencyInjectionExtensions.AddSingletonAs<AssetQueryParser>(services).AsSelf();
			DependencyInjectionExtensions.AddTransientAs<AssetTagsDeleter>(services).As<IDeleter>();
			DependencyInjectionExtensions.AddTransientAs<AssetCache>(services).As<IAssetCache>();
			DependencyInjectionExtensions.AddSingletonAs<AssetTusRunner>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<AssetTusStore>(services).As<ITusStore>().As<ITusExpirationStore>();
			DependencyInjectionExtensions.AddSingletonAs<RebuildFiles>(services).AsSelf();
			DependencyInjectionExtensions.AddTransientAs<AssetHistoryEventsCreator>(services).As<IHistoryEventsCreator>();
			DependencyInjectionExtensions.AddSingletonAs<AssetsSearchSource>(services).As<ISearchSource>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultAssetFileStore>(services).As<IAssetFileStore>().As<IDeleter>();
			DependencyInjectionExtensions.AddSingletonAs<AssetEnricher>(services).As<IAssetEnricher>();
			DependencyInjectionExtensions.AddSingletonAs<AssetQueryService>(services).As<IAssetQueryService>();
			DependencyInjectionExtensions.AddSingletonAs<AssetLoader>(services).As<IAssetLoader>();
			DependencyInjectionExtensions.AddSingletonAs<AssetUsageTracker>(services).As<IEventConsumer>().As<IDeleter>();
			DependencyInjectionExtensions.AddSingletonAs<FileTypeAssetMetadataSource>(services).As<IAssetMetadataSource>();
			DependencyInjectionExtensions.AddSingletonAs<FileTagAssetMetadataSource>(services).As<IAssetMetadataSource>();
			DependencyInjectionExtensions.AddSingletonAs<ImageAssetMetadataSource>(services).As<IAssetMetadataSource>();
			DependencyInjectionExtensions.AddSingletonAs<SvgAssetMetadataSource>(services).As<IAssetMetadataSource>();
		}
	}
}