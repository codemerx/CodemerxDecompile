namespace CodemerxDecompile.Notifications;

public interface INotificationHandler
{
    void CreateNotification(string message, NotificationLevel level);
}
