using System.Collections.ObjectModel;
using CodemerxDecompile.Notifications;
using CodemerxDecompile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CodemerxDecompile.ViewModels;

public partial class NotificationsViewModel : ObservableObject, INotificationsViewModel, INotificationHandler
{
    public NotificationsViewModel(INotificationService notificationService)
    {
        notificationService.RegisterHandler(this);
    }

    public ObservableCollection<Notification> Notifications { get; } = new();

    public void CreateNotification(string message, NotificationLevel level)
    {
        Notifications.Add(new Notification
        {
            Message = message,
            Level = level
        });
    }

    [RelayCommand]
    private void CloseNotification(Notification notification)
    {
        Notifications.Remove(notification);
    }
}
