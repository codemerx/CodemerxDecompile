using System.Collections.ObjectModel;

namespace CodemerxDecompile.Nodes;

public class ModuleNode : Node
{
    public ObservableCollection<NamespaceNode> Namespaces { get; } = new();
}
