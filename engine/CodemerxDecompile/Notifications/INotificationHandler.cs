namespace CodemerxDecompile.Notifications;

public interface INotificationHandler
{
    void ShowNotification(Notification notification);

    void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification);
}
