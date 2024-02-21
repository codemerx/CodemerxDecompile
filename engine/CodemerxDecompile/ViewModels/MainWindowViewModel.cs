using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CodemerxDecompile.Extensions;
using CodemerxDecompile.Nodes;
using CodemerxDecompile.SearchResults;
using CodemerxDecompile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Mono.Cecil.Extensions;
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
    private readonly List<AssemblyDefinition> assemblies = new();
    
    private readonly Stack<(Node, Vector, int)> backStack = new();
    private readonly Stack<(Node, Vector, int)> forwardStack = new();

    private readonly SearchService searchService = new();
    private readonly Debouncer searchDebouncer = new(TimeSpan.FromMilliseconds(500));

    private TypeDefinition? currentTypeDefinition;
    private DecompiledTypeMetadata? currentDecompiledTypeMetadata;
    private bool isBackForwardNavigation = false;
    private Vector? scrollOffset = null;
    private int? caretOffset = null;
    private bool isSearchNavigation;

    private Task? currentSearchTask;
    private Task? lastSearchTask;
    
    [ObservableProperty]
    private Node? selectedNode;

    [ObservableProperty]
    private TextDocument? document;

    [ObservableProperty]
    private Language selectedLanguage;

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SearchPaneSelected))]
    private int selectedPaneIndex;

    [ObservableProperty]
    private SearchResult? selectedSearchResult;

    public MainWindowViewModel()
    {
        selectedLanguage = Languages[0];
    }

    public ObservableCollection<AssemblyNode> AssemblyNodes { get; } = new();

    public ObservableCollection<Language> Languages { get; } = new()
    {
        new Language("C#", LanguageFactory.GetLanguage(CSharpVersion.V7)),
        new Language("VB.NET", LanguageFactory.GetLanguage(VisualBasicVersion.V10)),
        new Language("IL", new IntermediateLanguage())
    };

    public ObservableCollection<SearchResult> SearchResults { get; } = new();

    public bool SearchPaneSelected => SelectedPaneIndex == 1;

    internal void SelectNodeByMemberReference(MemberReference memberReference)
    {
        var toBeResolved = memberReference.GetTopDeclaringTypeOrSelf();
        var typeDefinition = toBeResolved.Resolve();
        if (assemblies.All(assembly => assembly.MainModule.FilePath != typeDefinition.Module.FilePath))
        {
            LoadAssemblies(new [] { typeDefinition.Module.FilePath });
        }
        
        SelectNodeByMemberFullName(typeDefinition.Module.Assembly.FullName, memberReference.FullName);
        
        void SelectNodeByMemberFullName(string assemblyName, string fullName)
        {
            var nodeToBeSelected = memberFullNameToNodeMap[assemblyName][fullName];
            var nodesToBeExpanded = new Stack<Node>();
            var parentNode = nodeToBeSelected.Parent;
            while (parentNode != null)
            {
                nodesToBeExpanded.Push(parentNode);
                parentNode = parentNode.Parent;
            }
        
            var mainWindow = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!;
            var treeView = mainWindow.TreeView;

            foreach (var nodeToBeExpanded in nodesToBeExpanded)
            {
                var treeViewItem = (TreeViewItem)treeView.TreeContainerFromItem(nodeToBeExpanded)!;
                treeViewItem.IsExpanded = true;
                treeViewItem.UpdateLayout();  // Force framework to render children
            }

            SelectedNode = nodeToBeSelected;
        }
    }

    internal async void TryLoadUnresolvedReference(MemberReference memberReference)
    {
        var storageProvider = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow!.StorageProvider;
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = $"Load {(memberReference.GetTopDeclaringTypeOrSelf().Scope as AssemblyNameReference)?.FullName}",
            AllowMultiple = false
            // TODO: Add file type filter
        });

        // User closed the dialog without selecting an assembly
        if (files.Count == 0)
        {
            return;
        }
        
        // TODO: There has to be a better way to do this...
        var assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(files[0].Path.LocalPath);
        var special = assembly.MainModule.IsReferenceAssembly() ? SpecialTypeAssembly.Reference : SpecialTypeAssembly.None;
        var strongName = new AssemblyStrongNameExtended(assembly.Name.FullName, assembly.MainModule.GetModuleArchitecture(), special);
        GlobalAssemblyResolver.Instance.RemoveFromFailedAssemblies(strongName);
        
        if (memberReference.GetTopDeclaringTypeOrSelf().Resolve() != null)
        {
            SelectNodeByMemberReference(memberReference);
        }
    }

    [RelayCommand]
    private async Task OpenFile()
    {
        var storageProvider = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow!.StorageProvider;
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Load assemblies",
            AllowMultiple = true,
            FileTypeFilter = new []
            {
                new FilePickerFileType("Assemblies")
                {
                    Patterns = new [] { "*.exe", "*.dll" },
                    AppleUniformTypeIdentifiers = new [] { "com.microsoft.windows-executable", "com.microsoft.windows-dynamic-link-library" },
                    MimeTypes = new [] { "application/vnd.microsoft.portable-executable" }
                }
            }
        });

        LoadAssemblies(files.Select(file => file.Path.LocalPath));
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void Back()
    {
        isBackForwardNavigation = true;
        
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        var textEditor = mainWindow!.TextEditor;
        
        forwardStack.Push((SelectedNode!, textEditor.TextArea.TextView.ScrollOffset, textEditor.CaretOffset));
        var (node, offset, caretOffset) = backStack.Pop();
        this.scrollOffset = offset;
        this.caretOffset = caretOffset;
        SelectedNode = node;
        ForwardCommand.NotifyCanExecuteChanged();
        BackCommand.NotifyCanExecuteChanged();
    }

    private bool CanGoBack() => backStack.Any();

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    private void Forward()
    {
        isBackForwardNavigation = true;
        
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        var textEditor = mainWindow!.TextEditor;
        
        backStack.Push((SelectedNode!, textEditor.TextArea.TextView.ScrollOffset, textEditor.CaretOffset));
        var (node, scrollOffset, caretOffset) = forwardStack.Pop();
        this.scrollOffset = scrollOffset;
        this.caretOffset = caretOffset;
        SelectedNode = node;
        BackCommand.NotifyCanExecuteChanged();
        ForwardCommand.NotifyCanExecuteChanged();
    }

    private bool CanGoForward() => forwardStack.Any();

    internal void LoadAssemblies(IEnumerable<string> filePaths)
    {
        // TODO: Rebuild tree view upon language change
        // TODO: Rebuild all reference nodes upon loading of a new assembly
        AssemblyNode? firstLoadedAssemblyNode = null;
        
        foreach (var file in filePaths)
        {
            var assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(file);
            var assemblyNode = new AssemblyNode
            {
                Name = assembly.Name.Name,
                Parent = null,
                AssemblyDefinition = assembly
            };

            if (firstLoadedAssemblyNode == null)
            {
                firstLoadedAssemblyNode = assemblyNode;
            }

            foreach (var reference in assembly.MainModule.AssemblyReferences)
            {
                var moduleArchitecture = assembly.MainModule.GetModuleArchitecture();
                var special = assembly.MainModule.IsReferenceAssembly()
                    ? SpecialTypeAssembly.Reference
                    : SpecialTypeAssembly.None;
                var resolvedReference = GlobalAssemblyResolver.Instance.Resolve(reference, null, moduleArchitecture, special);

                Node referenceNode = resolvedReference switch
                {
                    not null => new ResolvedReferenceNode
                    {
                        Name = reference.Name,
                        Parent = assemblyNode
                    },
                    null => new UnresolvedReferenceNode
                    {
                        Name = reference.Name,
                        Parent = assemblyNode
                    }
                };
                
                assemblyNode.References.Items.Add(referenceNode);
            }
            
            var dict = new Dictionary<string, Node>();
            foreach (var namespaceGroup in assembly.MainModule.Types.GroupBy(t => t.Namespace))
            {
                var namespaceNode = new NamespaceNode
                {
                    Name = namespaceGroup.Key == string.Empty ? "<Default namespace>" : namespaceGroup.Key,
                    Parent = assemblyNode
                };

                foreach (var typeDefinition in namespaceGroup)
                {
                    var typeNode = BuildTypeSubtree(typeDefinition, namespaceNode, dict);
                    namespaceNode.Types.Add(typeNode);
                    dict.Add(typeDefinition.FullName, typeNode);
                }
                
                assemblyNode.AddNamespace(namespaceNode);
            }

            memberFullNameToNodeMap.Add(assembly.FullName, dict);
            AssemblyNodes.Add(assemblyNode);
            assemblies.Add(assembly);
            ClearAssemblyListCommand.NotifyCanExecuteChanged();
        }

        SelectedNode = firstLoadedAssemblyNode;

        TypeNode BuildTypeSubtree(TypeDefinition typeDefinition, Node parentNode, Dictionary<string, Node> dict)
        {
            TypeNode typeNode = typeDefinition switch
            {
                { IsEnum: true } => new EnumNode
                {
                    Name = typeDefinition.Name,
                    Parent = parentNode,
                    TypeDefinition = typeDefinition
                },
                { IsValueType: true } => new StructNode
                {
                    Name = typeDefinition.Name,
                    Parent = parentNode,
                    TypeDefinition = typeDefinition
                },
                { IsClass: true } => new ClassNode
                {
                    Name = typeDefinition.Name,
                    Parent = parentNode,
                    TypeDefinition = typeDefinition
                },
                { IsInterface: true } => new InterfaceNode
                {
                    Name = typeDefinition.Name,
                    Parent = parentNode,
                    TypeDefinition = typeDefinition
                },
                _ => throw new NotSupportedException()
            };
            
            var members = typeDefinition.GetMembersSorted(false, SelectedLanguage.Instance);
            foreach (var memberDefinition in members)
            {
                MemberNode node = memberDefinition switch
                {
                    FieldDefinition fieldDefinition => new FieldNode
                    {
                        Name = fieldDefinition.Name,
                        FieldDefinition = fieldDefinition,
                        Parent = typeNode
                    },
                    MethodDefinition { IsConstructor: true } methodDefinition => new ConstructorNode
                    {
                        Name = methodDefinition.Name,
                        MethodDefinition = methodDefinition,
                        Parent = typeNode
                    },
                    PropertyDefinition propertyDefinition => new PropertyNode
                    {
                        Name = propertyDefinition.Name,
                        PropertyDefinition = propertyDefinition,
                        Parent = typeNode
                    },
                    MethodDefinition methodDefinition => new MethodNode
                    {
                        Name = methodDefinition.Name,
                        MethodDefinition = methodDefinition,
                        Parent = typeNode
                    },
                    EventDefinition eventDefinition => new EventNode
                    {
                        Name = eventDefinition.Name,
                        EventDefinition = eventDefinition,
                        Parent = typeNode
                    },
                    TypeDefinition nestedTypeDefinition => BuildTypeSubtree(nestedTypeDefinition, typeNode, dict),
                    _ => throw new NotSupportedException()
                };
                
                typeNode.Members.Add(node);
                dict.Add(memberDefinition.FullName, node);
            }

            return typeNode;
        }
    }

    [RelayCommand(CanExecute = nameof(CanClearAssemblyList))]
    private void ClearAssemblyList()
    {
        SelectedNode = null;
        AssemblyNodes.Clear();
        memberFullNameToNodeMap.Clear();
        
        assemblies.Clear();
        ClearAssemblyListCommand.NotifyCanExecuteChanged();
        
        backStack.Clear();
        forwardStack.Clear();
        BackCommand.NotifyCanExecuteChanged();
        ForwardCommand.NotifyCanExecuteChanged();
        
        GlobalAssemblyResolver.Instance.ClearCache();
    }

    private bool CanClearAssemblyList() => assemblies.Any();

    partial void OnSelectedNodeChanged(Node? oldNode, Node? newNode)
    {
        if (!isBackForwardNavigation)
        {
            if (oldNode is MemberNode or AssemblyNode)
            {
                var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
                var textEditor = mainWindow!.TextEditor;
                
                backStack.Push((oldNode, textEditor.TextArea.TextView.ScrollOffset, textEditor.CaretOffset));
                forwardStack.Clear();
                
                BackCommand.NotifyCanExecuteChanged();
                ForwardCommand.NotifyCanExecuteChanged();
            }
        }
        
        Decompile(newNode, false);

        isBackForwardNavigation = false;
    }

    partial void OnSelectedLanguageChanged(Language value)
    {
        Decompile(SelectedNode, true);
    }

    private void Decompile(Node? node, bool forceRecompilation)
    {
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        if (mainWindow == null)
            return;
        
        var textEditor = mainWindow.TextEditor;

        TypeDefinition containingType;
        IMemberDefinition memberDefinition;
        if (node is MemberNode memberNode)
        {
            containingType = GetContainingTypeNode(memberNode).TypeDefinition;
            memberDefinition = memberNode.MemberDefinition;
        }
        else if (node is AssemblyNode assemblyNode)
        {
            var language = SelectedLanguage.Instance;
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var formatter = new MemberReferenceTrackingFormatter(stringWriter);
            var writerSettings = new WriterSettings();
            var writer = language.GetAssemblyAttributeWriter(formatter, new SimpleExceptionFormatter(), writerSettings);
            var writerContextService = new SimpleWriterContextService(new DefaultDecompilationCacheService(), true);
            
            writer.WriteAssemblyAttributes(assemblyNode.AssemblyDefinition, writerContextService);
            
            Document = new TextDocument(stringBuilder.ToString());
            
            currentTypeDefinition = null;
            currentDecompiledTypeMetadata = null;
            MainWindow.references.Clear();
            
            return;
        }
        else
        {
            Document = null;
            currentTypeDefinition = null;
            currentDecompiledTypeMetadata = null;
            return;
        }

        if (containingType != currentTypeDefinition || forceRecompilation)
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
                writingInfos = namespaceImperativeLanguageWriter.WriteTypeAndNamespaces(containingType, writerContextService);
            }
            else
            {
                writingInfos = writer.Write(containingType, writerContextService);
            }

            Document = new TextDocument(stringBuilder.ToString());
            currentTypeDefinition = containingType;
            currentDecompiledTypeMetadata = CreateTypeMetadata(formatter.CodeSpanToMemberReference, writingInfos[0]);

            MainWindow.references.Clear();
            foreach (var kvp in formatter.CodeSpanToMemberReference)
            {
                var typeDefinition = kvp.Value.GetTopDeclaringTypeOrSelf().Resolve();
                MainWindow.references.Add(new ReferenceTextSegment
                {
                    StartOffset = kvp.Key.StartOffset,
                    EndOffset = kvp.Key.EndOffset,
                    Length = kvp.Key.EndOffset - kvp.Key.StartOffset,
                    MemberReference = kvp.Value,
                    Resolved = typeDefinition != null
                });
            }
        }

        textEditor.UpdateLayout();  // Force editor to render to ensure ScrollToLine works as expected

        if (!isSearchNavigation)
        {
            if (scrollOffset == null)
            {
                // Normal navigation
                var codePosition = currentDecompiledTypeMetadata!.MemberDeclarationToCodePostionMap[memberDefinition];
                textEditor.Select(codePosition.StartOffset, codePosition.EndOffset - codePosition.StartOffset + 1); // TODO: Figure out why we need +1 here
                textEditor.ScrollToLine(Document!.GetLocation(codePosition.StartOffset).Line);
            }
            else
            {
                // History navigation
                textEditor.ScrollToHorizontalOffset(scrollOffset.Value.X);
                textEditor.ScrollToVerticalOffset(scrollOffset.Value.Y);
            
                scrollOffset = null;
            }

            if (caretOffset != null)
            {
                textEditor.CaretOffset = caretOffset.Value;

                caretOffset = null;
            }
        }

        TypeNode GetContainingTypeNode(MemberNode memberNode)
        {
            var result = memberNode switch
            {
                TypeNode typeNode => typeNode,
                _ => (TypeNode)memberNode.Parent!
            };
            
            var parentNode = result!.Parent as TypeNode;
            while (parentNode != null)
            {
                result = parentNode;
                parentNode = result.Parent as TypeNode;
            }

            return result;
        }

        DecompiledTypeMetadata CreateTypeMetadata(Dictionary<OffsetSpan, MemberReference> codeSpanToMemberReference, WritingInfo info)
        {
            DecompiledTypeMetadata decompiledTypeMetadata = new DecompiledTypeMetadata();

            decompiledTypeMetadata.CodeSpanToMemberReference.AddRange(codeSpanToMemberReference);

            decompiledTypeMetadata.MemberDeclarationToCodeSpan.AddRange(info.MemberDeclarationToCodeSpan);
            decompiledTypeMetadata.MemberDeclarationToCodePostionMap.AddRange(info.MemberDeclarationToCodePostionMap);

            decompiledTypeMetadata.CodeMappingInfo.NodeToCodeMap.AddRange(info.CodeMappingInfo.NodeToCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.InstructionToCodeMap.AddRange(info.CodeMappingInfo.InstructionToCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.FieldConstantValueToCodeMap.AddRange(info.CodeMappingInfo.FieldConstantValueToCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.VariableToCodeMap.AddRange(info.CodeMappingInfo.VariableToCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.ParameterToCodeMap.AddRange(info.CodeMappingInfo.ParameterToCodeMap);

            decompiledTypeMetadata.CodeMappingInfo.MethodDefinitionToMethodReturnTypeCodeMap.AddRange(info.CodeMappingInfo.MethodDefinitionToMethodReturnTypeCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.FieldDefinitionToFieldTypeCodeMap.AddRange(info.CodeMappingInfo.FieldDefinitionToFieldTypeCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.PropertyDefinitionToPropertyTypeCodeMap.AddRange(info.CodeMappingInfo.PropertyDefinitionToPropertyTypeCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.EventDefinitionToEventTypeCodeMap.AddRange(info.CodeMappingInfo.EventDefinitionToEventTypeCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap.AddRange(info.CodeMappingInfo.ParameterDefinitionToParameterTypeCodeMap);
            decompiledTypeMetadata.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap.AddRange(info.CodeMappingInfo.VariableDefinitionToVariableTypeCodeMap);

            return decompiledTypeMetadata;
        }
    }

    [RelayCommand]
    private void OpenSearchPane()
    {
        SelectedPaneIndex = 1;
        
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        mainWindow.SearchTextBox.SelectAll();
        mainWindow.SearchTextBox.Focus();
    }

    partial void OnSearchTextChanged(string value)
    {
        searchDebouncer.Debounce(() =>
        {
            lastSearchTask = currentSearchTask;
            currentSearchTask = Task.Run(async () =>
            {
                if (lastSearchTask is { IsCompleted: false })
                {
                    searchService.CancelSearch();
                    await lastSearchTask;
                }

                SearchResults.Clear();
            
                if (string.IsNullOrWhiteSpace(value))
                    return;
        
                foreach (var searchResult in searchService.Search(assemblies.Select(a => a.MainModule.FilePath), value))
                {
                    SearchResults.Add(searchResult);
                }
            });
        });
    }

    partial void OnSelectedSearchResultChanged(SearchResult? value)
    {
        if (value == null)
            return;
        
        isSearchNavigation = true;
        
        SelectNodeByMemberReference(value.DeclaringType);

        var codeSpan = searchService.GetSearchResultPosition(value, currentDecompiledTypeMetadata!);
        if (codeSpan == null)
            return;

        var startOffset = Document.GetOffset(codeSpan.Value.Start.Line + 1, codeSpan.Value.Start.Column);
        var endOffset = Document.GetOffset(codeSpan.Value.End.Line + 1, codeSpan.Value.End.Column);
        
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        var textEditor = mainWindow.TextEditor;
        textEditor.Select(startOffset + 1, endOffset - startOffset); // TODO: Figure out why we need +1 here
        textEditor.ScrollTo(codeSpan.Value.Start.Line, codeSpan.Value.Start.Column);
        
        isSearchNavigation = false;
    }

    public record Language(string Name, ILanguage Instance);
}
