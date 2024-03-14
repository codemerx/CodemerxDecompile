using Mono.Cecil.Cil;

namespace CodemerxDecompile.SearchResults;

public class VariableNameSearchResult : SearchResult
{
    public required VariableDefinition VariableDefinition { get; init; }
}
