using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class ParameterTypeSearchResult : SearchResult
{
    public required ParameterDefinition ParameterDefinition { get; init; }
}
