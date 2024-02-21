using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class TypeNameSearchResult : SearchResult
{
    public required TypeDefinition TypeDefinition { get; init; }
}
