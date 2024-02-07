using System;
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
using CodemerxDecompile.Nodes;
using CodemerxDecompile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mono.Cecil;
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

    public ObservableCollection<AssemblyNode> AssemblyNodes { get; } = new();

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

        // TODO: Rebuild tree view upon language change
        foreach (var file in files)
        {
            var assembly = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(file.Path.AbsolutePath);
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
        }

        TypeNode BuildTypeSubtree(TypeDefinition typeDefinition, Node parentNode, Dictionary<string, Node> dict)
        {
            var typeNode = new TypeNode
            {
                Name = typeDefinition.Name,
                Parent = parentNode,
                TypeDefinition = typeDefinition
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

    [RelayCommand]
    private void ClearAssemblyList()
    {
        AssemblyNodes.Clear();
        GlobalAssemblyResolver.Instance.ClearCache();
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

        TypeDefinition containingType;
        IMemberDefinition memberDefinition;
        if (value is MemberNode memberNode)
        {
            containingType = GetContainingTypeNode(memberNode).TypeDefinition;
            memberDefinition = memberNode.MemberDefinition;
        }
        else
        {
            textEditor.Document = null;
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

            textEditor.Document = new TextDocument(stringBuilder.ToString());
            currentTypeDefinition = containingType;
            currentWritingInfo = writingInfos[0];

            MainWindow.references.Clear();
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
        
        var codePosition = currentWritingInfo!.MemberDeclarationToCodePostionMap[memberDefinition];
        textEditor.Select(codePosition.StartOffset, codePosition.EndOffset - codePosition.StartOffset + 1); // TODO: Figure out why we need +1 here
        textEditor.ScrollToLine(textEditor.Document!.GetLocation(codePosition.StartOffset).Line);

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
