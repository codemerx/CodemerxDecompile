using Avalonia.Controls;
using CodemerxDecompile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CodemerxDecompile.Views;

public partial class NotificationsView : UserControl
{
    public NotificationsView()
    {
        InitializeComponent();
        
        if (!Design.IsDesignMode)
            DataContext = App.Current.Services.GetRequiredService<INotificationsViewModel>();
    }
}
