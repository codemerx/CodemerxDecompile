using Mono.Cecil;

namespace CodemerxDecompile.Nodes;

public class ConstructorNode : MemberNode
{
    public required MethodDefinition MethodDefinition { get; init; }
    public override IMemberDefinition MemberDefinition => MethodDefinition;
}
