using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CodemerxDecompile.Extensions;
using CodemerxDecompile.Services;
using CodemerxDecompile.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CodemerxDecompile;

public partial class App : Application
{
    public new static App Current => (App)Application.Current!;
    
    public IServiceProvider Services { get; } = ConfigureServices();
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();

            desktop.ShutdownRequested += (_, _) =>
            {
                var analyticsService = Services.GetRequiredService<IAnalyticsService>();
                try
                {
                    analyticsService.TrackEvent(AnalyticsEvents.Shutdown);
                }
                catch { }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private static IServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .ConfigureOptions()
            .ConfigureLogging()
            .AddViews()
            .AddViewModels()
            .AddServices()
            .AddProviders()
            .AddHttpClients()
            .BuildServiceProvider();
}
