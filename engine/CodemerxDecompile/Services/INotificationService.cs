using System.Collections.Generic;
using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public interface INotificationService
{
    void RegisterHandler(INotificationHandler handler);

    Notification ShowNotification(string message, NotificationLevel level, IEnumerable<NotificationAction>? actions = null);

    Notification ReplaceNotification(Notification notification, string message, NotificationLevel level, IEnumerable<NotificationAction>? actions = null);
}
