using System.Collections.ObjectModel;
using System.Linq;
using Telerik.JustDecompiler.Decompiler;

namespace CodemerxDecompile.ViewModels;

public class MainWindowViewModel
{
    public MainWindowViewModel()
    {
        var assembly = Utilities.GetAssembly(
            "/Users/alexander/work/codemerx/CodemerxDecompile/engine/JustDecompiler.NetStandard/bin/Debug/netstandard2.0/JustDecompiler.NetStandard.dll");
        var types = assembly.MainModule.GetTypes();
        var groupedByNamespace = types.GroupBy(t => t.Namespace);
        
        Nodes = new ObservableCollection<Node>
        {
            new Node(assembly.Name.Name, new ObservableCollection<Node>(
                groupedByNamespace.Select(g => new Node(g.Key == string.Empty ? "<Default namespace>" : g.Key, new ObservableCollection<Node>(
                    g.Select(t => new Node(t.Name)))))))
        };
    }
    
    public ObservableCollection<Node> Nodes { get; }
    
    public class Node
    {
        public string Title { get; }
        public ObservableCollection<Node>? SubNodes { get; }
  
        public Node(string title)
        {
            Title = title;
        }

        public Node(string title, ObservableCollection<Node> subNodes)
            : this(title)
        {
            SubNodes = subNodes;
        }
    }
}
