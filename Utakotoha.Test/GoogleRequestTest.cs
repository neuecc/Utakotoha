using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using Sgml;
using System.Xml.Linq;

namespace Utakotoha.Test
{
    [TestClass]
    public class GoogleRequestTest
    {
        [TestMethod]
        public void CleanHref()
        {
            (new GoogleRequest().AsDynamic()
                .CleanHref(@"/url?q=http://d.hatena.ne.jp/keyword/hoge&amp;sa=U&amp;ei=wsGHTe-qFIPEvQO_k9HUCA&amp;ved=0CAkQFjAA&amp;usg=AFQjCNGxRTtlNP9hQ6-FEGw4L7TFAQRzVQ")
                as string).Is("http://d.hatena.ne.jp/keyword/hoge");

            (new GoogleRequest().AsDynamic()
                .CleanHref(@"http://d.hatena.ne.jp/keyword/hoge")
                as string).Is("http://d.hatena.ne.jp/keyword/hoge");
        }

        [TestMethod]
        [Timeout(3000)]
        public void Search()
        {
            var r = new GoogleRequest() { Num = 10 }.Search("hoge").ToEnumerable().ToArray();

            r.Count().Is(10);
            r.Any(x => x.Title == "メタ構文変数 - Wikipedia").Is(true);
            r.All(x => x.Url.StartsWith("http")).Is(true);
        }
    }
}
