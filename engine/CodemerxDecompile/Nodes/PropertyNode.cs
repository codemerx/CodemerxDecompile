using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class PropertyNode : MemberNode
{
    public required PropertyDefinition PropertyDefinition { get; init; }
    public override IMemberDefinition MemberDefinition => PropertyDefinition;
}
