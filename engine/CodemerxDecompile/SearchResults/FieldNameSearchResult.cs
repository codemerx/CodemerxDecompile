using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class FieldNameSearchResult : SearchResult
{
    public required FieldDefinition FieldDefinition { get; init; }
}
