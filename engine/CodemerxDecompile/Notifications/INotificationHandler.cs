using System.Collections.Generic;

namespace CodemerxDecompile.Notifications;

public interface INotificationHandler
{
    Notification ShowNotification(string message, NotificationLevel level, IEnumerable<NotificationAction>? actions);

    Notification ReplaceNotification(Notification notification, string message, NotificationLevel level, IEnumerable<NotificationAction>? actions);
}
