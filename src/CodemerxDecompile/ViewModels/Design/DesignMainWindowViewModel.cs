using System.Threading.Tasks;
using CodemerxDecompile.Notifications;
using CodemerxDecompile.Services;
using JustDecompile.Tools.MSBuildProjectBuilder;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.ViewModels.Design;

public class DesignMainWindowViewModel : MainWindowViewModel
{
    public DesignMainWindowViewModel()
        : base(new DesignProjectGenerationService(), new DesignNotificationService(), new DesignAnalyticsService())
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
