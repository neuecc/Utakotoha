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
using System.Runtime.Serialization;
using Utakotoha.Model.Bing;
#endif

namespace Utakotoha.Model
{


    public class GoogleRequest
    {
        public string Site { get; set; }
        public string Language { get; set; }
        public string Location { get; set; }
        public string[] ExceptKeywords { get; set; }

        public int Num { get; set; }

        private string BuildQuery(string[] words)
        {
            //var site = (Site == null) ? "" : "site:" + Site;
            //var except = (ExceptKeywords == null) ? "" : "-" + ExceptKeywords.Select(s => s. .Wrap("\"")).Join(" OR ");


            //var keyword = keywords.Select(s => s.Wrap("\"")).Join(" ");

            //var query =
            //    "q" + "=" + Uri.EscapeUriString(queryOption + keywords + except + site)
            //    + "&v=1.0"
            //    + "&rsz=8"
            //    + "&hl=ja";

            // return "https://ajax.googleapis.com/ajax/services/search/web?" + query;
            return null;
        }

        public GoogleRequest()
        {

        }

        private string CleanHref(string href)
        {
            return Regex.Match(href, "^.*?(http.+?)(&amp;sa=U|$)+").Groups[1].Value;
        }

        public IObservable<SearchResult> Search(params string[] keywords)
        {
            var req = (HttpWebRequest)BuildQuery(keywords).Pipe(WebRequest.Create);
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows Phone 7)";
            req.Headers[HttpRequestHeader.Referer] = "hope to make referrer!";

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