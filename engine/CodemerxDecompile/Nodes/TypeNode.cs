using System.Collections.ObjectModel;
using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class TypeNode : MemberNode
{
    public required TypeDefinition TypeDefinition { get; init; }
    public ObservableCollection<MemberNode> Members { get; } = new();
    public override IMemberDefinition MemberDefinition => TypeDefinition;
}
