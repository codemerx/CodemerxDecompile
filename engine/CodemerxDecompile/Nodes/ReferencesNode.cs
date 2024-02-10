using System.Collections.ObjectModel;

namespace CodemerxDecompile.Nodes;

public class ReferencesNode : Node
{
    public ObservableCollection<Node> Items { get; } = new();
}
