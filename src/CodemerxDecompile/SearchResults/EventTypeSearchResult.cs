using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class EventTypeSearchResult : SearchResult
{
    public required EventDefinition EventDefinition { get; init; }
}
