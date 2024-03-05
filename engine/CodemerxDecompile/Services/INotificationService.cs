using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public interface INotificationService
{
    void RegisterHandler(INotificationHandler handler);

    Notification ShowNotification(string message, NotificationLevel level);

    Notification ReplaceNotification(Notification notification, string message, NotificationLevel level);
}
