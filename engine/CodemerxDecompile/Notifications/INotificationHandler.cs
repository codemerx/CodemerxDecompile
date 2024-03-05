namespace CodemerxDecompile.Notifications;

public interface INotificationHandler
{
    Notification ShowNotification(string message, NotificationLevel level);

    Notification ReplaceNotification(Notification notification, string message, NotificationLevel level);
}
