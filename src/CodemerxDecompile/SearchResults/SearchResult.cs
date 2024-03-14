using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public abstract class SearchResult
{
    public required TypeDefinition DeclaringType { get; init; }
    public required string MatchedString { get; init; }
}
