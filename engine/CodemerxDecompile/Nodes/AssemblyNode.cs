using System.Collections.ObjectModel;

namespace CodemerxDecompile.Nodes;

public class AssemblyNode : Node
{
    public ObservableCollection<ModuleNode> Modules { get; } = new();
}
