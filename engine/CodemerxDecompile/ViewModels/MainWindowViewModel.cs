using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CodemerxDecompile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace CodemerxDecompile.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private TypeDefinition? currentTypeDefinition;
    private WritingInfo? currentWritingInfo;
    
    [ObservableProperty]
    private Node? selectedNode;

    public ObservableCollection<Node> Nodes { get; } = new();

    [RelayCommand]
    private async Task OpenFile()
    {
        var storageProvider = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow!.StorageProvider;
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Load assemblies",
            AllowMultiple = true
            // TODO: Add file type filter
        });

        foreach (var file in files)
        {
            var assembly = Utilities.GetAssembly(file.Path.AbsolutePath);
            var types = assembly.MainModule.GetTypes();
            var groupedByNamespace = types.GroupBy(t => t.Namespace);
        
            Nodes.Add(new()
            {
                Title = assembly.Name.Name,
                SubNodes = new ObservableCollection<Node>(
                    groupedByNamespace.Select(g => new Node
                    {
                        Title = g.Key == string.Empty ? "<Default namespace>" : g.Key,
                        SubNodes = new ObservableCollection<Node>(
                            g.Select(t => new Node
                            {
                                Title = t.Name,
                                MemberDefinition = t,
                                SubNodes = new ObservableCollection<Node>(
                                    Utilities.GetTypeMembers(t, LanguageFactory.GetLanguage(CSharpVersion.V7)).Select(m => new Node()
                                    {
                                        Title = m.Name,
                                        MemberDefinition = m
                                    })
                                )
                            })
                        )
                    })
                )
            });
        }
    }

    [RelayCommand]
    private void ClearAssemblyList()
    {
        Nodes.Clear();
    }

    partial void OnSelectedNodeChanged(Node? value)
    {
        var textEditor = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!.TextEditor;
        
        if (value is null or { MemberDefinition: null })
        {
            textEditor.Document.Text = string.Empty;
            currentTypeDefinition = null;
            currentWritingInfo = null;
            return;
        }

        var typeDefinition = value switch
        {
            { MemberDefinition: TypeDefinition typeDef } => typeDef,
            _ => value.MemberDefinition.DeclaringType
        };

        if (typeDefinition != currentTypeDefinition)
        {
            var language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var writerSettings = new WriterSettings();
            var writer = language.GetWriter(new PlainTextFormatter(stringWriter), new SimpleExceptionFormatter(), writerSettings);
            var writingInfos = (writer as NamespaceImperativeLanguageWriter)!.WriteTypeAndNamespaces(typeDefinition, new SimpleWriterContextService(new DefaultDecompilationCacheService(), true));

            textEditor.Document.Text = stringBuilder.ToString();
            currentTypeDefinition = typeDefinition;
            currentWritingInfo = writingInfos[0];
        }

        var codePosition = currentWritingInfo!.MemberDeclarationToCodePostionMap[value.MemberDefinition];
        textEditor.Select(codePosition.StartOffset, codePosition.EndOffset - codePosition.StartOffset + 1); // TODO: Figure out why we need +1 here
        textEditor.ScrollToLine(textEditor.Document!.GetLocation(codePosition.StartOffset).Line);
    }

    public class Node
    {
        public required string Title { get; init; }
        public ObservableCollection<Node>? SubNodes { get; init; }
        public IMemberDefinition? MemberDefinition { get; init; }
    }
}
