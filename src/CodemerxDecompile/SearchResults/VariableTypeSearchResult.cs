using Mono.Cecil.Cil;

namespace CodemerxDecompile.SearchResults;

public class VariableTypeSearchResult : SearchResult
{
    public required VariableDefinition VariableDefinition { get; init; }
}
