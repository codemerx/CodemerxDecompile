using CodemerxDecompile.Service.Services.Search.Models;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemerxDecompile.Service.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<SearchResult> Search(string searchString, bool matchCase = false);
    }
}
