using System.Collections.ObjectModel;

namespace CodemerxDecompile.Nodes;

public class AssemblyNode : Node
{
    public ObservableCollection<NamespaceNode> Namespaces { get; } = new();
}
