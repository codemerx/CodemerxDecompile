using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public class NotificationService : INotificationService
{
    private INotificationHandler? handler;
    
    public void RegisterHandler(INotificationHandler handler)
    {
        this.handler = handler;
    }

    public void Show(string message, NotificationLevel level = NotificationLevel.Information)
    {
        handler?.CreateNotification(message, level);
    }
}
