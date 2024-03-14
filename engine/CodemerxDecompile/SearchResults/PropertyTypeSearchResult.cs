using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class PropertyTypeSearchResult : SearchResult
{
    public required PropertyDefinition PropertyDefinition { get; init; }
}
