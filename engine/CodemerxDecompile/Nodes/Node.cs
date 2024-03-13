namespace CodemerxDecompile.Nodes;

public abstract class Node
{
    public required string Name { get; init; }
    public required Node? Parent { get; init; }
}
