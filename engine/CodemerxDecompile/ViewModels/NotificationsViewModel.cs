using System.Collections.Generic;
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

    public Notification ShowNotification(string message, NotificationLevel level, IEnumerable<NotificationAction>? actions)
    {
        var notification = new Notification
        {
            Message = message,
            Level = level,
            Actions = actions
        };
        Notifications.Add(notification);

        return notification;
    }

    public Notification ReplaceNotification(Notification notification, string message, NotificationLevel level, IEnumerable<NotificationAction>? actions)
    {
        CloseNotification(notification);
        return ShowNotification(message, level, actions);
    }

    [RelayCommand]
    private void CloseNotification(Notification notification)
    {
        Notifications.Remove(notification);
    }

    [RelayCommand]
    private void ExecuteNotificationAction(NotificationAction notificationAction) => notificationAction.Action();
}
