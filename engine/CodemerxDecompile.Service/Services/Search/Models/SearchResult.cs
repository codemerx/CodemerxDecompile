//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

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
