using System;
using CodemerxDecompile.Options;
using CodemerxDecompile.Providers;
using CodemerxDecompile.Services;
using CodemerxDecompile.ViewModels;
using CodemerxDecompile.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace CodemerxDecompile.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services)
    {
        var configuration = GetConfiguration();

        if (EnvironmentProvider.Environment == Environment.Production)
            services.Configure<MatomoAnalyticsOptions>(configuration.GetSection(MatomoAnalyticsOptions.Key));

        return services;
    }

    public static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddConsole());

        return services;
    }

    public static IServiceCollection AddViews(this IServiceCollection services) =>
        services
            .AddTransient<MainWindow>();

    public static IServiceCollection AddViewModels(this IServiceCollection services) =>
        services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<INotificationsViewModel, NotificationsViewModel>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddSingleton<INotificationService, NotificationService>()
            .AddTransient<IProjectGenerationService, ProjectGenerationService>()
            .AddTransient<IAutoUpdateService, AutoUpdateService>()
            .AddTransient<IAnalyticsService, LoggerAnalyticsService>(Environment.Development)
            .AddTransient<IAnalyticsService, MatomoAnalyticsService>(Environment.Production);

    public static IServiceCollection AddProviders(this IServiceCollection services) =>
        services
            .AddSingleton<ISystemInformationProvider, SystemInformationProvider>()
            .AddSingleton<IDeviceIdentifierProvider, DeviceIdentifierProvider>();

    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        var policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5));
        
        services
            .AddHttpClient<IAutoUpdateService, AutoUpdateService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://api.github.com");

                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                // As per https://docs.github.com/en/rest/using-the-rest-api/troubleshooting-the-rest-api?apiVersion=2022-11-28#user-agent-required
                httpClient.DefaultRequestHeaders.Add("User-Agent", "CodemerxDecompile");
            })
            .AddPolicyHandler(policy);

        if (EnvironmentProvider.Environment == Environment.Production)
            services
                .AddHttpClient<IAnalyticsService, MatomoAnalyticsService>()
                .ConfigureHttpClient((services, httpClient) =>
                {
                    var options = services.GetRequiredService<IOptions<MatomoAnalyticsOptions>>();

                    httpClient.BaseAddress = new Uri(options.Value.ServerUrl);
                })
                .AddPolicyHandler(policy);

        return services;
    }

    private static IConfigurationRoot GetConfiguration() =>
        new ConfigurationBuilder()
            .AddEmbeddedResource("appsettings.json")
            .AddEmbeddedResource($"appsettings.{EnvironmentProvider.Environment}.json")
            .Build();

    private static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, Environment environment)
        where TService : class
        where TImplementation : class, TService
    {
        if (environment == EnvironmentProvider.Environment)
            services.AddTransient<TService, TImplementation>();

        return services;
    }
}
