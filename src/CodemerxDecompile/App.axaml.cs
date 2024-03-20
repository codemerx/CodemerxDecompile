/*
    Copyright 2024 CodeMerx
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

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

    private void About_OnClick(object? sender, EventArgs e)
    {
        _ = Services.GetRequiredService<IAnalyticsService>().TrackEventAsync(AnalyticsEvents.About);
        Services.GetRequiredService<IDialogService>().ShowDialog<AboutWindow>();
    }
}
