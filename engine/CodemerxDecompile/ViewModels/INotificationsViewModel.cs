using System.Collections.ObjectModel;
using CodemerxDecompile.Notifications;
using CommunityToolkit.Mvvm.Input;

namespace CodemerxDecompile.ViewModels;

public interface INotificationsViewModel
{
    ObservableCollection<Notification> Notifications { get; }
    
    IRelayCommand<Notification> CloseNotificationCommand { get; }
}
