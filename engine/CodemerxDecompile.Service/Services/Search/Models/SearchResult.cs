namespace CodemerxDecompile.Service.Services.Search.Models
{
    public class SearchResult
    {
        public SearchResult(SearchResultType type, string declaringTypeFilePath, string matchedString, object objectReference)
        {
            this.Type = type;
            this.DeclaringTypeFilePath = declaringTypeFilePath;
            this.MatchedString = matchedString;
            this.ObjectReference = objectReference;
        }

        public SearchResultType Type { get; }
        public string DeclaringTypeFilePath { get; }
        public string MatchedString { get; }
        public object ObjectReference { get; }
    }
}
