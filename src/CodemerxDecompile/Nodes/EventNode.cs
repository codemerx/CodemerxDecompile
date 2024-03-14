using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class EventNode : MemberNode
{
    public required EventDefinition EventDefinition { get; init; }
    public override IMemberDefinition MemberDefinition => EventDefinition;
}
