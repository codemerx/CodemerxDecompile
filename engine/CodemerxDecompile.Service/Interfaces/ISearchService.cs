using CodemerxDecompile.Service.Services.Search.Models;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Service.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<SearchResult> Search(string searchString, bool matchCase = false);

        CodeSpan? GetSearchResultPosition(int searchResultIndex);
    }
}
