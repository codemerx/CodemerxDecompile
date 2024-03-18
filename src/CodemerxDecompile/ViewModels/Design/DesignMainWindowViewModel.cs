/*
    Copyright CodeMerx 2024
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

using System.Threading.Tasks;
using Avalonia.Controls;
using CodemerxDecompile.Notifications;
using CodemerxDecompile.Services;
using JustDecompile.Tools.MSBuildProjectBuilder;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.ViewModels.Design;

public class DesignMainWindowViewModel : MainWindowViewModel
{
    public DesignMainWindowViewModel()
        : base(new DesignProjectGenerationService(), new DesignNotificationService(), new DesignAnalyticsService(), new DesignDialogService())
    {
    }
}

file class DesignProjectGenerationService : IProjectGenerationService
{
    public string GenerateProject(AssemblyDefinition assembly, VisualStudioVersion visualStudioVersion, ILanguage language, string outputPath)
    {
        return string.Empty;
    }
}

file class DesignNotificationService : INotificationService
{
    public void RegisterHandler(INotificationHandler handler)
    {
    }

    public void ShowNotification(Notification notification)
    {
    }

    public void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification)
    {
    }
}

file class DesignAnalyticsService : IAnalyticsService
{
    public void TrackEvent(AnalyticsEvent @event)
    {
    }

    public Task TrackEventAsync(AnalyticsEvent @event)
    {
        return Task.CompletedTask;
    }
}

file class DesignDialogService : IDialogService
{
    public void ShowDialog<TWindow>()
        where TWindow : Window
    {
    }
}
