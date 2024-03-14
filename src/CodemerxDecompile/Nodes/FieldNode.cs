using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class FieldNode : MemberNode
{
    public required FieldDefinition FieldDefinition { get; init; }
    public override IMemberDefinition MemberDefinition => FieldDefinition;
}
