using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using CodemerxDecompile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using TextMateSharp.Grammars;

namespace CodemerxDecompile.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<MainWindowViewModel>();

        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var installation = TextEditor.InstallTextMate(registryOptions);
        installation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".cs").Id));
    }
}
