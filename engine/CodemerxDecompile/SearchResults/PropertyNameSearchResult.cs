using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class PropertyNameSearchResult : SearchResult
{
    public required PropertyDefinition PropertyDefinition { get; init; }
}
