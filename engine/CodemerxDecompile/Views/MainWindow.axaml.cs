using Avalonia.Controls;
using CodemerxDecompile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CodemerxDecompile.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<MainWindowViewModel>();
    }
}
