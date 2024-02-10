using System.Collections.ObjectModel;

namespace CodemerxDecompile.Nodes;

public class ReferencesNode : Node
{
    public ObservableCollection<ReferenceNode> Items { get; } = new();
}
