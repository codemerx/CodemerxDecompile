using System;
using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public class NotificationService : INotificationService
{
    private INotificationHandler? handler;
    
    public void RegisterHandler(INotificationHandler handler)
    {
        this.handler = handler;
    }

    public void ShowNotification(Notification notification)
    {
        if (handler == null)
            throw new InvalidOperationException($"{nameof(NotificationService)} should be first initialized using {nameof(RegisterHandler)}.");
        
        handler.ShowNotification(notification);
    }

    public void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification)
    {
        if (handler == null)
            throw new InvalidOperationException($"{nameof(NotificationService)} should be first initialized using {nameof(RegisterHandler)}.");
        
        handler.ReplaceNotification(notificationToBeReplaced, replacementNotification);
    }
}
