using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public interface INotificationService
{
    void RegisterHandler(INotificationHandler handler);

    void ShowNotification(Notification notification);

    void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification);
}
