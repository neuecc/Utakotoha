using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utakotoha.Model.Bing;
using System.Linq;

namespace Utakotoha.Model.Test
{
    [TestClass]
    public class BingTest
    {
        [TestMethod]
        public void BuildSearchQuery()
        {
            var words = new[]
            {
                new SearchWord("homu", SearchLogicalOp.And,  SearchTarget.All),
                new SearchWord("mogu",SearchLogicalOp.Or, SearchTarget.All),
                new SearchWord("mami",SearchLogicalOp.And, SearchTarget.InAnchor),
                new SearchWord("yellow",SearchLogicalOp.Not, SearchTarget.InAnchor),
                new SearchWord("madomado",SearchLogicalOp.Or, SearchTarget.InBody),
                new SearchWord("gikagika",SearchLogicalOp.And, SearchTarget.InBody),
                new SearchWord("anko", SearchLogicalOp.And,SearchTarget.InTitle),
                new SearchWord("taiyaki",SearchLogicalOp.Not, SearchTarget.InTitle),
                new SearchWord("http://hogehoge.com",SearchLogicalOp.And , SearchTarget.Site)
            };

            words.BuildQuery().Is("\"homu\" OR \"mogu\" AND inanchor:\"mami\" NOT inanchor:\"yellow\" OR inbody:\"madomado\" AND inbody:\"gikagika\" AND intitle:\"anko\" NOT intitle:\"taiyaki\" AND site:\"http://hogehoge.com\"");
        }

        [TestMethod]
        public void SearchBasicTest()
        {
            var words = new[]
            {
                new SearchWord("hoge"),
                new SearchWord("jp", SearchLogicalOp.And, SearchTarget.Location),
                new SearchWord("ja",SearchLogicalOp.And, SearchTarget.Language)
            };

            var r = new BingRequest() { Market = "ja-jp" }.Search(words).ToEnumerable().ToArray();

            r.Count().Is(20);
            r.Any(x => x.Title == "Hogeとは - はてなキーワード").Is(true);
            r.All(x => x.Url.StartsWith("http")).Is(true);
        }

        [TestMethod]
        public void SearchLyricTest()
        {
            var words = new[]
            {
                new SearchWord("宇多田ヒカル", SearchLogicalOp.And),
                new SearchWord("SAKURAドロップス", SearchLogicalOp.And),
                new SearchWord("music.goo.ne.jp/lyric", SearchLogicalOp.And, SearchTarget.Site)
            };

            var r = new BingRequest() { Market = "en-us" }.Search(words).ToEnumerable().ToArray();

            r.Any(x => x.Title.Contains("宇多田ヒカル") && x.Title.Contains("SAKURAドロップス")).Is(true);
        }

        [TestMethod]
        public void NotFoundTest()
        {
            var words = new[]
            {
                new SearchWord("hogehogehogehogehoge", SearchLogicalOp.And),
                new SearchWord("mogumogumogumogumogu", SearchLogicalOp.And),
                new SearchWord("music.goo.ne.jp/lyric", SearchLogicalOp.And, SearchTarget.Site)
            };

            var r = new BingRequest() { Market = "en-us" }.Search(words).ToEnumerable().ToArray();

            r.Any().Is(false);
        }
    }
}