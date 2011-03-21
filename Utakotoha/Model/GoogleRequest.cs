using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Sgml;
using System.Xml.Linq;
using System.IO;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace Utakotoha
{
    public class GoogleRequest
    {
        public int Num { get; set; }
        public string Site { get; set; }

        public GoogleRequest()
        {
            Num = 10;
        }

        private string BuildQuery(string[] keywords)
        {
            var site = (Site != null) ? "site:" + Uri.EscapeUriString(Site) + " " : "";
            var num = "num=" + Num;
            return string.Format("http://google.co.jp/search?num={0}&q={1}",
                Num, site + keywords.Select(s => Uri.EscapeUriString(s.Wrap("\""))).Join(" "));
        }

        private string CleanHref(string href)
        {
            return Regex.Match(href, "^.*?(http.+?)&amp;").Groups[1].Value;
        }

        public IObservable<SearchResult> Search(params string[] keywords)
        {
            var req = (HttpWebRequest)BuildQuery(keywords).Pipe(WebRequest.Create);
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows Phone 7)"; // dummy ua(set mobile)

            return Observable.Defer(() => req.GetResponseAsObservable())
                .Select(res =>
                {
                    using (var stream = res.GetResponseStream())
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    using (var sgml = new SgmlReader() { CaseFolding = CaseFolding.ToLower, InputStream = sr })
                    {
                        return XElement.Load(sgml);
                    }
                })
                .SelectMany(x => x.Descendants("p"))
                .Where(x => x.Element("a") != null)
                .Select(x =>
                {
                    var a = x.Element("a");
                    return new SearchResult(a.Value, a.Attribute("href").Value.Pipe(CleanHref));
                });

        }
    }
}