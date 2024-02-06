using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CodemerxDecompile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.IL;
using Telerik.JustDecompiler.Languages.VisualBasic;

namespace CodemerxDecompile.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly Dictionary<string, Dictionary<string, Node>> memberFullNameToNodeMap = new();
    private TypeDefinition? currentTypeDefinition;
    private WritingInfo? currentWritingInfo;
    
    [ObservableProperty]
    private Node? selectedNode;

    [ObservableProperty]
    private Language selectedLanguage;

    public MainWindowViewModel()
    {
        selectedLanguage = Languages[0];
    }

    public ObservableCollection<Node> Nodes { get; } = new();

    public ObservableCollection<Language> Languages { get; } = new()
    {
        new Language("C#", LanguageFactory.GetLanguage(CSharpVersion.V7)),
        new Language("VB.NET", LanguageFactory.GetLanguage(VisualBasicVersion.V10)),
        new Language("IL", new IntermediateLanguage())
    };

    internal void SelectNodeByMemberReference(MemberReference memberReference)
    {
        var toBeResolved = memberReference as TypeReference;
        var declaringType = memberReference.DeclaringType;
        while (declaringType != null)
        {
            toBeResolved = declaringType;
            declaringType = declaringType.DeclaringType;
        }

        var typeDefinition = toBeResolved!.Resolve();
        if (typeDefinition == null)
        {
            // show dialog
        }
        else
        {
            SelectNodeByMemberFullName(typeDefinition.Module.Assembly.FullName, memberReference.FullName);
        }
    }
    
    internal void SelectNodeByMemberFullName(string assemblyName, string fullName)
    {
        var nodeToBeSelected = memberFullNameToNodeMap[assemblyName][fullName];
        var nodesToBeExpanded = new Stack<Node>();
        var parentNode = nodeToBeSelected.ParentNode;
        while (parentNode != null)
        {
            nodesToBeExpanded.Push(parentNode);
            parentNode = parentNode.ParentNode;
        }
        
        var mainWindow = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!;
        var treeView = mainWindow.TreeView;

        foreach (var nodeToBeExpanded in nodesToBeExpanded)
        {
            var treeViewItem = (TreeViewItem)treeView.TreeContainerFromItem(nodeToBeExpanded);
            treeViewItem.IsExpanded = true;
            treeViewItem.Presenter.UpdateLayout();  // Force framework to render children
        }

        SelectedNode = nodeToBeSelected;
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
            var assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(file.Path.AbsolutePath);
            var types = assembly.MainModule.Types;
            var groupedByNamespace = types.GroupBy(t => t.Namespace);

            var assemblyNode = new Node
            {
                Title = assembly.Name.Name,
                SubNodes = new ObservableCollection<Node>()
            };
            
            var dict = new Dictionary<string, Node>();
            foreach (var namespaceGroup in groupedByNamespace)
            {
                var namespaceNode = new Node
                {
                    Title = namespaceGroup.Key == string.Empty ? "<Default namespace>" : namespaceGroup.Key,
                    ParentNode = assemblyNode,
                    SubNodes = new ObservableCollection<Node>()
                };

                foreach (var type in namespaceGroup)
                {
                    var typeNode = new Node
                    {
                        Title = type.Name,
                        MemberDefinition = type,
                        ParentNode = namespaceNode,
                        SubNodes = new ObservableCollection<Node>()
                    };
                    
                    foreach (var member in Utilities.GetTypeMembers(type, LanguageFactory.GetLanguage(CSharpVersion.V7)))
                    {
                        var memberNode = new Node
                        {
                            Title = member.Name,
                            MemberDefinition = member,
                            ParentNode = typeNode
                        };
                        typeNode.SubNodes.Add(memberNode);
                        dict.Add(member.FullName, memberNode);
                    }
                    
                    namespaceNode.SubNodes.Add(typeNode);
                    dict.Add(type.FullName, typeNode);
                }
                
                assemblyNode.SubNodes.Add(namespaceNode);
            }

            memberFullNameToNodeMap.Add(assembly.FullName, dict);
            Nodes.Add(assemblyNode);
        }
    }

    [RelayCommand]
    private void ClearAssemblyList()
    {
        Nodes.Clear();
    }

    partial void OnSelectedNodeChanged(Node? value)
    {
        Decompile(value, false);
    }

    partial void OnSelectedLanguageChanged(Language value)
    {
        Decompile(SelectedNode, true);
    }

    private void Decompile(Node? value, bool forceRecompilation)
    {
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        if (mainWindow == null)
            return;
        
        var textEditor = mainWindow.TextEditor;
        
        if (value is null or { MemberDefinition: null })
        {
            textEditor.Document = null;
            currentTypeDefinition = null;
            currentWritingInfo = null;
            return;
        }

        var typeDefinition = value switch
        {
            { MemberDefinition: TypeDefinition typeDef } => typeDef,
            _ => value.MemberDefinition.DeclaringType
        };

        if (typeDefinition != currentTypeDefinition || forceRecompilation)
        {
            var language = SelectedLanguage.Instance;
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var formatter = new MemberReferenceTrackingFormatter(stringWriter);
            var writerSettings = new WriterSettings();
            var writer = language.GetWriter(formatter, new SimpleExceptionFormatter(), writerSettings);
            var writerContextService = new SimpleWriterContextService(new DefaultDecompilationCacheService(), true);
            List<WritingInfo> writingInfos;
            if (writer is NamespaceImperativeLanguageWriter namespaceImperativeLanguageWriter)
            {
                writingInfos = namespaceImperativeLanguageWriter.WriteTypeAndNamespaces(typeDefinition, writerContextService);
            }
            else
            {
                writingInfos = writer.Write(typeDefinition, writerContextService);
            }

            textEditor.Document = new TextDocument(stringBuilder.ToString());
            currentTypeDefinition = typeDefinition;
            currentWritingInfo = writingInfos[0];

            MainWindow.references.Clear();
            // foreach (var kvp in currentWritingInfo.MemberDeclarationToCodePostionMap)
            // {
            //     MainWindow.references.Add(new ReferenceTextSegment
            //     {
            //         StartOffset = kvp.Value.StartOffset,
            //         EndOffset = kvp.Value.EndOffset,
            //         Length = kvp.Value.EndOffset - kvp.Value.StartOffset + 1, // TODO: Figure out why we need +1 here
            //         MemberDefinition = kvp.Key
            //     });
            // }

            foreach (var kvp in formatter.CodeSpanToMemberReference)
            {
                MainWindow.references.Add(new ReferenceTextSegment()
                {
                    StartOffset = kvp.Key.StartOffset,
                    EndOffset = kvp.Key.EndOffset,
                    Length = kvp.Key.EndOffset - kvp.Key.StartOffset,
                    MemberReference = kvp.Value
                });
            }
        }

        textEditor.UpdateLayout();  // Force editor to render to ensure ScrollToLine works as expected
        
        var codePosition = currentWritingInfo!.MemberDeclarationToCodePostionMap[value.MemberDefinition];
        textEditor.Select(codePosition.StartOffset, codePosition.EndOffset - codePosition.StartOffset + 1); // TODO: Figure out why we need +1 here
        textEditor.ScrollToLine(textEditor.Document!.GetLocation(codePosition.StartOffset).Line);
    }

    public class Node
    {
        public required string Title { get; init; }
        public Node? ParentNode { get; init; }
        public ObservableCollection<Node>? SubNodes { get; init; }
        public IMemberDefinition? MemberDefinition { get; init; }
    }

    public record Language(string Name, ILanguage Instance);
}
