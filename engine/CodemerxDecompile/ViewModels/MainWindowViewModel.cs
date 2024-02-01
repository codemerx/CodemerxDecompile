using System.Collections.Generic;
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
    private readonly Dictionary<IMemberDefinition, Node> MemberDefinitionToNodeMap = new();
    private TypeDefinition? currentTypeDefinition;
    private WritingInfo? currentWritingInfo;
    
    [ObservableProperty]
    private Node? selectedNode;

    public ObservableCollection<Node> Nodes { get; } = new();

    internal void SelectNodeByMemberDefinition(IMemberDefinition memberDefinition)
    {
        SelectedNode = MemberDefinitionToNodeMap[memberDefinition];
    }

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

            var namespaceNodes = new ObservableCollection<Node>();
            foreach (var namespaceGroup in groupedByNamespace)
            {
                var typeNodes = new ObservableCollection<Node>();
                foreach (var type in namespaceGroup)
                {
                    var memberNodes = new ObservableCollection<Node>();
                    foreach (var member in Utilities.GetTypeMembers(type, LanguageFactory.GetLanguage(CSharpVersion.V7)))
                    {
                        var memberNode = new Node
                        {
                            Title = member.Name,
                            MemberDefinition = member
                        };
                        memberNodes.Add(memberNode);
                        MemberDefinitionToNodeMap.Add(member, memberNode);
                    }
                    
                    var typeNode = new Node
                    {
                        Title = type.Name,
                        MemberDefinition = type,
                        SubNodes = memberNodes
                    };
                    typeNodes.Add(typeNode);
                    MemberDefinitionToNodeMap.Add(type, typeNode);
                }
                
                namespaceNodes.Add(new Node
                {
                    Title = namespaceGroup.Key == string.Empty ? "<Default namespace>" : namespaceGroup.Key,
                    SubNodes = typeNodes
                });
            }

            Nodes.Add(new Node
            {
                Title = assembly.Name.Name,
                SubNodes = namespaceNodes
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
        var mainWindow = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!;
        var textEditor = mainWindow.TextEditor;
        
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

            MainWindow.references.Clear();
            foreach (var kvp in currentWritingInfo.MemberDeclarationToCodePostionMap)
            {
                MainWindow.references.Add(new ReferenceTextSegment
                {
                    StartOffset = kvp.Value.StartOffset,
                    EndOffset = kvp.Value.EndOffset,
                    Length = kvp.Value.EndOffset - kvp.Value.StartOffset + 1, // TODO: Figure out why we need +1 here
                    MemberDefinition = kvp.Key
                });
            }
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
