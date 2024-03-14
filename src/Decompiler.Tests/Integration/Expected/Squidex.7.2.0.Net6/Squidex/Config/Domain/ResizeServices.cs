using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Assets;
using Squidex.Assets.Remote;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class ResizeServices
	{
		[NullableContext(1)]
		public static void AddSquidexImageResizing(IServiceCollection services, IConfiguration config)
		{
			CompositeThumbnailGenerator compositeThumbnailGenerator = new CompositeThumbnailGenerator(new IAssetThumbnailGenerator[] { new ImageSharpThumbnailGenerator(), new ImageMagickThumbnailGenerator() }, 0);
			string value = ConfigurationBinder.GetValue<string>(config, "assets:resizerUrl");
			if (string.IsNullOrWhiteSpace(value))
			{
				DependencyInjectionExtensions.AddSingletonAs<CompositeThumbnailGenerator>(services, (IServiceProvider c) => compositeThumbnailGenerator).As<IAssetThumbnailGenerator>();
				return;
			}
			HttpClientFactoryServiceCollectionExtensions.AddHttpClient(services, "Resize", (HttpClient options) => options.BaseAddress = new Uri(value));
			DependencyInjectionExtensions.AddSingletonAs<RemoteThumbnailGenerator>(services, (IServiceProvider c) => new RemoteThumbnailGenerator(ServiceProviderServiceExtensions.GetRequiredService<IHttpClientFactory>(c), compositeThumbnailGenerator)).As<IAssetThumbnailGenerator>();
		}
	}
}