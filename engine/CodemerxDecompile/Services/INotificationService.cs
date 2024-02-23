using CodemerxDecompile.Notifications;

namespace CodemerxDecompile.Services;

public interface INotificationService
{
    void RegisterHandler(INotificationHandler handler);

    void Show(string message, NotificationLevel level = NotificationLevel.Information);
}
