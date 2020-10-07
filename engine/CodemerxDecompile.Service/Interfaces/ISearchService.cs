using CodemerxDecompile.Service.Services.Search.Models;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace CodemerxDecompile.Service.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<SearchResult> Search(string query, bool matchCase = false, bool matchWholeWord = false);

        void CancelSearch();

        CodeSpan? GetSearchResultPosition(int searchResultIndex);
    }
}
