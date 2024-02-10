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
    
    private readonly Stack<(Node, Vector)> backStack = new();
    private readonly Stack<(Node, Vector)> forwardStack = new();
    
    private TypeDefinition? currentTypeDefinition;
    private WritingInfo? currentWritingInfo;
    private bool isBackForwardNavigation = false;
    private Vector? offset = null;
    
    [ObservableProperty]
    private Node? selectedNode;

    [ObservableProperty]
    private TextDocument? document;

    [ObservableProperty]
    private Language selectedLanguage;

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
        
        forwardStack.Push((SelectedNode!, mainWindow!.TextEditor.TextArea.TextView.ScrollOffset));
        var (node, offset) = backStack.Pop();
        this.offset = offset;
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
        
        backStack.Push((SelectedNode!, mainWindow!.TextEditor.TextArea.TextView.ScrollOffset));
        var (node, offset) = forwardStack.Pop();
        this.offset = offset;
        SelectedNode = node;
        BackCommand.NotifyCanExecuteChanged();
        ForwardCommand.NotifyCanExecuteChanged();
    }

    private bool CanGoForward() => forwardStack.Any();

    private void LoadAssemblies(IEnumerable<string> filePaths)
    {
        // TODO: Rebuild tree view upon language change
        foreach (var file in filePaths)
        {
            var assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(file);
            var assemblyNode = new AssemblyNode
            {
                Name = assembly.Name.Name,
                Parent = null
            };

            var dict = new Dictionary<string, Node>();
            foreach (var module in assembly.Modules)
            {
                var moduleNode = new ModuleNode
                {
                    Name = module.Name,
                    Parent = assemblyNode
                };
                
                foreach (var namespaceGroup in module.Types.GroupBy(t => t.Namespace))
                {
                    var namespaceNode = new NamespaceNode
                    {
                        Name = namespaceGroup.Key == string.Empty ? "<Default namespace>" : namespaceGroup.Key,
                        Parent = moduleNode
                    };

                    foreach (var typeDefinition in namespaceGroup)
                    {
                        var typeNode = BuildTypeSubtree(typeDefinition, namespaceNode, dict);
                        namespaceNode.Types.Add(typeNode);
                        dict.Add(typeDefinition.FullName, typeNode);
                    }
                
                    moduleNode.Namespaces.Add(namespaceNode);
                }

                assemblyNode.Modules.Add(moduleNode);
            }

            memberFullNameToNodeMap.Add(assembly.FullName, dict);
            AssemblyNodes.Add(assemblyNode);
            assemblies.Add(assembly);
            ClearAssemblyListCommand.NotifyCanExecuteChanged();
        }

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
            if (oldNode is MemberNode)
            {
                var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
                
                backStack.Push((oldNode, mainWindow!.TextEditor.TextArea.TextView.ScrollOffset));
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

    private void Decompile(Node? value, bool forceRecompilation)
    {
        var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow;
        if (mainWindow == null)
            return;
        
        var textEditor = mainWindow.TextEditor;

        TypeDefinition containingType;
        IMemberDefinition memberDefinition;
        if (value is MemberNode memberNode)
        {
            containingType = GetContainingTypeNode(memberNode).TypeDefinition;
            memberDefinition = memberNode.MemberDefinition;
        }
        else
        {
            Document = null;
            currentTypeDefinition = null;
            currentWritingInfo = null;
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
            currentWritingInfo = writingInfos[0];

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

        if (offset == null)
        {
            var codePosition = currentWritingInfo!.MemberDeclarationToCodePostionMap[memberDefinition];
            textEditor.Select(codePosition.StartOffset, codePosition.EndOffset - codePosition.StartOffset + 1); // TODO: Figure out why we need +1 here
            textEditor.ScrollToLine(Document!.GetLocation(codePosition.StartOffset).Line);
        }
        else
        {
            textEditor.ScrollToHorizontalOffset(offset.Value.X);
            textEditor.ScrollToVerticalOffset(offset.Value.Y);
            offset = null;
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
    }

    public record Language(string Name, ILanguage Instance);
}
