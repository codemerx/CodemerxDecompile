using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class MethodNode : MemberNode
{
    public required MethodDefinition MethodDefinition { get; init; }
    public override IMemberDefinition MemberDefinition => MethodDefinition;
}
