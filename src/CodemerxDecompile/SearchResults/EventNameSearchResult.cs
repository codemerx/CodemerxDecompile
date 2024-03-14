using Mono.Cecil;

namespace CodemerxDecompile.SearchResults;

public class EventNameSearchResult : SearchResult
{
    public required EventDefinition EventDefinition { get; init; }
}
