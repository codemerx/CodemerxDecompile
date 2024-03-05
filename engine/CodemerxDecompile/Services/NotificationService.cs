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

    public Notification ShowNotification(string message, NotificationLevel level)
    {
        if (handler == null)
            throw new InvalidOperationException($"{nameof(NotificationService)} should be first initialized using {nameof(RegisterHandler)}.");
        
        return handler.ShowNotification(message, level);
    }

    public Notification ReplaceNotification(Notification notification, string message, NotificationLevel level)
    {
        if (handler == null)
            throw new InvalidOperationException($"{nameof(NotificationService)} should be first initialized using {nameof(RegisterHandler)}.");
        
        return handler.ReplaceNotification(notification, message, level);
    }
}
