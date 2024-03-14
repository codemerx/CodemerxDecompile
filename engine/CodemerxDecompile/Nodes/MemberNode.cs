using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public abstract class MemberNode : Node
{
    public abstract IMemberDefinition MemberDefinition { get; }
}
