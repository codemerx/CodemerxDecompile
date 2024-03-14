using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Squidex.Config.Domain;
using System;
using System.Runtime.CompilerServices;

namespace Squidex
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class Program
	{
		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return GenericHostBuilderExtensions.ConfigureWebHostDefaults(HostingHostBuilderExtensions.ConfigureLogging(Host.CreateDefaultBuilder(args), (HostBuilderContext context, ILoggingBuilder builder) => builder.ConfigureForSquidex(context.get_Configuration())).ConfigureAppConfiguration((HostBuilderContext hostContext, IConfigurationBuilder builder) => Squidex.Config.Domain.ConfigurationExtensions.ConfigureForSquidex(builder)).ConfigureServices((HostBuilderContext context, IServiceCollection services) => {
				ServiceCollectionHostedServiceExtensions.AddHostedService<LogConfigurationHost>(services);
				HostingServiceExtensions.AddInitializer(services);
				ServiceCollectionHostedServiceExtensions.AddHostedService<MigratorHost>(services);
				ServiceCollectionHostedServiceExtensions.AddHostedService<MigrationRebuilderHost>(services);
				HostingServiceExtensions.AddBackgroundProcesses(services);
			}), (IWebHostBuilder builder) => {
				WebHostBuilderKestrelExtensions.ConfigureKestrel(builder, (WebHostBuilderContext context, KestrelServerOptions serverOptions) => {
					if (HostEnvironmentEnvExtensions.IsDevelopment(context.get_HostingEnvironment()) || ConfigurationBinder.GetValue<bool>(context.get_Configuration(), "devMode:enable"))
					{
						serverOptions.ListenAnyIP(0x1389, (ListenOptions listenOptions) => ListenOptionsHttpsExtensions.UseHttps(listenOptions, "../../../dev/squidex-dev.pfx", "password"));
						serverOptions.ListenAnyIP(0x1388);
					}
				});
				WebHostBuilderExtensions.UseStartup<Startup>(builder);
			});
		}

		public static void Main(string[] args)
		{
			HostingAbstractionsHostExtensions.Run(Program.CreateHostBuilder(args).Build());
		}
	}
}