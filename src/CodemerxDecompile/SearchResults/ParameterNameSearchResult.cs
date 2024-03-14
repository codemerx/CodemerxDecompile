using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class ParameterNameSearchResult : SearchResult
{
    public required MethodDefinition MethodDefinition { get; init; }
    public required ParameterDefinition ParameterDefinition { get; init; }
}
