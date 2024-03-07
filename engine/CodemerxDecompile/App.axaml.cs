using System;
using System.Threading.Tasks;
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

        var analyticsService = Services.GetRequiredService<IAnalyticsService>();
        _ = Task.Run(() => analyticsService.TrackEventAsync("startup"));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private static IServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .ConfigureOptions()
            .ConfigureLogging()
            .AddViewModels()
            .AddServices()
            .AddProviders()
            .AddHttpClients()
            .BuildServiceProvider();
}
