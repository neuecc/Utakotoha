using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utakotoha.Model.Bing;

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
                new SearchWord("homu", SearchMode.All),
                new SearchWord("mogu", SearchMode.All),
                new SearchWord("mami", SearchMode.InAnchor),
                new SearchWord("yellow", SearchMode.InAnchor),
                new SearchWord("madomado", SearchMode.InBody),
                new SearchWord("gikagika", SearchMode.InBody),
                new SearchWord("anko", SearchMode.InTitle),
                new SearchWord("taiyaki", SearchMode.InTitle),
            };

            words.BuildQuery().Is("(homu OR mogu) inanchor:(mami OR yellow) inbody:(madomado OR gikagika) intitle:(anko OR taiyaki)");
        }
    }
}