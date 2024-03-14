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

    public void ShowNotification(Notification notification)
    {
        Notifications.Add(notification);
    }

    public void ReplaceNotification(Notification notificationToBeReplaced, Notification replacementNotification)
    {
        CloseNotification(notificationToBeReplaced);
        ShowNotification(replacementNotification);
    }

    [RelayCommand]
    private void CloseNotification(Notification notification)
    {
        Notifications.Remove(notification);
    }

    [RelayCommand]
    private void ExecuteNotificationAction(NotificationAction notificationAction) => notificationAction.Action();
}
