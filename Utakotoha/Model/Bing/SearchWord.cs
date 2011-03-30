using System.Collections.Generic;
using System.Linq;

namespace Utakotoha.Model.Bing
{
    public class SearchWord
    {
        public string Query { get; set; }
        public SearchLogicalOp SearchLogicalOp { get; set; }
        public SearchTarget SearchTarget { get; set; }

        public SearchWord(string query, SearchLogicalOp logicalOp = SearchLogicalOp.And, SearchTarget target = SearchTarget.All)
        {
            this.Query = query;
            this.SearchLogicalOp = logicalOp;
            this.SearchTarget = target;
        }
    }

    public static class SearchWordExtensions
    {
        public static string BuildQuery(this IEnumerable<SearchWord> searchWords)
        {
            return searchWords
                .Select((x, i) =>
                      ((i == 0 && x.SearchLogicalOp != SearchLogicalOp.Not) ? "" : x.SearchLogicalOp.ToString().ToUpper() + " ")
                    + ((x.SearchTarget == SearchTarget.All) ? "" : x.SearchTarget.ToString().ToLower() + ":")
                    + x.Query.Quote())
                .Join(" ");
        }
    }
}