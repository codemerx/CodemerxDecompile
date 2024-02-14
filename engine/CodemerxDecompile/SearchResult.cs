using Mono.Cecil;

namespace CodemerxDecompile;

public readonly record struct SearchResult
{
    public required SearchResultType Type { get; init; }
    public required TypeDefinition DeclaringType { get; init; }
    public required string MatchedString { get; init; }
    public required object ObjectReference { get; init; }
}
