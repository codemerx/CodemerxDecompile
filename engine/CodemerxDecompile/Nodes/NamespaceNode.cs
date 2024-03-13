using System.Collections.ObjectModel;

namespace CodemerxDecompile.Nodes;

public class NamespaceNode : Node
{
    public ObservableCollection<TypeNode> Types { get; } = new();
}
