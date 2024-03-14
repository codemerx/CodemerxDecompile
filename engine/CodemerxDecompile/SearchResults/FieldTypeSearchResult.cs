using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class FieldTypeSearchResult : SearchResult
{
    public required FieldDefinition FieldDefinition { get; init; }
}
