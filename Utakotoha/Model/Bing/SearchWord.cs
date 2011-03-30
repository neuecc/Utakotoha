using System.Collections.Generic;
using System.Linq;

namespace Utakotoha.Model.Bing
{
    public class SearchWord
    {
        public string Query { get; set; }
        public SearchMode SearchMode { get; set; }

        public SearchWord(string query, SearchMode searchMode = SearchMode.All)
        {
            this.Query = query;
            this.SearchMode = searchMode;
        }
    }

    public static class SearchWordExtensions
    {
        public static string BuildQuery(this IEnumerable<SearchWord> searchWords)
        {
            return searchWords.GroupBy(s => s.SearchMode)
                .Select(xs => ((xs.Key == SearchMode.All) ? "" : xs.Key.ToString().ToLower() + ":")
                    + "(" + xs.Select(s => s.Query).Join(" AND ") + ")")
                .Join(" ");
        }
    }
}