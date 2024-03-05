using CodemerxDecompile.Notifications;
using CodemerxDecompile.Services;
using JustDecompile.Tools.MSBuildProjectBuilder;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.ViewModels.Design;

public class DesignMainWindowViewModel : MainWindowViewModel
{
    public DesignMainWindowViewModel()
        : base(new DesignProjectGenerationService(), new DesignNotificationService())
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

    public Notification ShowNotification(string message, NotificationLevel level)
    {
        return null!;
    }

    public Notification ReplaceNotification(Notification notification, string message, NotificationLevel level)
    {
        return null!;
    }
}
