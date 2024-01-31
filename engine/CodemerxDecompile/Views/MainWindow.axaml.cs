using Avalonia.Controls;
using AvaloniaEdit.Document;
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

        TextEditor.Document = new TextDocument();
        
        // TODO: Switch editor theme according with app theme
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var installation = TextEditor.InstallTextMate(registryOptions);
        installation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".cs").Id));
    }
}
