
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;

namespace Utakotoha.Model.Bing
{
    public class BingRequest
    {
        private const string AppId = "3A6EF110D9DA4EF1FD5074D9A7FC01FE19A380E7"; // secret, secret...

        public int Count { get; set; }
        public int Offset { get; set; }
        public string Market { get; set; }

        public BingRequest()
        {
            this.Count = 20;
            this.Offset = 0;
            this.Market = "en-us";
        }

        private string BuildUrl(IEnumerable<SearchWord> words)
        {
            var query =
                  "?Appid=" + AppId
                + "&query=" + Uri.EscapeUriString(words.BuildQuery())
                + "&sources=web"
                + "&version=2.0"
                + "&Market=" + Market
                + "&web.count=" + Count
                + "&web.offset=" + Offset;

            return "http://api.search.live.net/json.aspx" + query;
        }

        public IObservable<SearchWebResult> Search(params SearchWord[] keywords)
        {
            var req = WebRequest.Create(BuildUrl(keywords));

            return Observable.Defer(() => req.GetResponseAsObservable())
                .Select(res =>
                {
                    var serializer = new DataContractJsonSerializer(typeof(SearchWebStructure));
                    using (var stream = res.GetResponseStream())
                    {
                        return (SearchWebStructure)serializer.ReadObject(stream);
                    }
                })
                .SelectMany(x => (x.SearchResponse.Web.Results != null)
                    ? x.SearchResponse.Web.Results
                    : Enumerable.Empty<SearchWebResult>());
        }
    }
}