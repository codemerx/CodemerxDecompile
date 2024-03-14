using System.Collections.ObjectModel;
using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class AssemblyNode : Node
{
    public AssemblyNode()
    {
        References = new ReferencesNode
        {
            Name = "References",
            Parent = this
        };
        Children.Add(References);
    }
    
    public required AssemblyDefinition AssemblyDefinition { get; init; }
    public ReferencesNode References { get; }
    public ObservableCollection<Node> Children { get; } = new();

    public void AddNamespace(NamespaceNode @namespace) => Children.Add(@namespace);
}
