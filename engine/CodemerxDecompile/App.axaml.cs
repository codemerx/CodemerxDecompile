using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CodemerxDecompile.Services;
using CodemerxDecompile.ViewModels;
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
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<INotificationsViewModel, NotificationsViewModel>();

        services.AddSingleton<INotificationService, NotificationService>();

        return services.BuildServiceProvider();
    }
}
