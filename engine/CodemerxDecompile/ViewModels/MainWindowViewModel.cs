using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace CodemerxDecompile.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private Node? selectedNode;

    [ObservableProperty]
    private TextDocument? document;
    
    public MainWindowViewModel()
    {
        var assembly = Utilities.GetAssembly(
            "/Users/alexander/work/codemerx/CodemerxDecompile/engine/JustDecompiler.NetStandard/bin/Debug/netstandard2.0/JustDecompiler.NetStandard.dll");
        var types = assembly.MainModule.GetTypes();
        var groupedByNamespace = types.GroupBy(t => t.Namespace);
        
        Nodes = new ObservableCollection<Node>
        {
            new()
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
                                TypeDefinition = t
                            })
                        )
                    })
                )
            }
        };
    }
    
    public ObservableCollection<Node> Nodes { get; }

    partial void OnSelectedNodeChanged(Node? value)
    {
        if (value is { TypeDefinition: not null })
        {
            var language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var writerSettings = new WriterSettings();
            var writer = language.GetWriter(new PlainTextFormatter(stringWriter), new SimpleExceptionFormatter(), writerSettings);
            writer.Write(value.TypeDefinition, new SimpleWriterContextService(new DefaultDecompilationCacheService(), true));
            Document = new TextDocument(stringBuilder.ToString());
        }
        else
        {
            Document = null;
        }
    }

    public class Node
    {
        public required string Title { get; init; }
        public ObservableCollection<Node>? SubNodes { get; init; }
        public TypeDefinition? TypeDefinition { get; init; }
    }
}
