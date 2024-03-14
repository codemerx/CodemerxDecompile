using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class MethodNameSearchResult : SearchResult
{
    public required MethodDefinition MethodDefinition { get; init; }
}
