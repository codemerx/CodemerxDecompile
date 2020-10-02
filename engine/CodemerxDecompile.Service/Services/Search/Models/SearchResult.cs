namespace CodemerxDecompile.Service.Services.Search.Models
{
    public class SearchResult
    {
        public SearchResult(int id, SearchResultType type, string declaringTypeFilePath, string matchedString, object objectReference)
        {
            this.Id = id;
            this.Type = type;
            this.DeclaringTypeFilePath = declaringTypeFilePath;
            this.MatchedString = matchedString;
            this.ObjectReference = objectReference;
        }

        public int Id { get; }
        public SearchResultType Type { get; }
        public string DeclaringTypeFilePath { get; }
        public string MatchedString { get; }
        public object ObjectReference { get; }
    }
}
