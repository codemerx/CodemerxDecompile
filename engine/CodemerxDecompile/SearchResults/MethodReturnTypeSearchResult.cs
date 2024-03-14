using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class MethodReturnTypeSearchResult : SearchResult
{
    public required MethodDefinition MethodDefinition { get; init; }
}
