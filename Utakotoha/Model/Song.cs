#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
using System;
using System.Linq;
using System.Runtime.Serialization;
using Utakotoha.Model.Bing;
using System.Text.RegularExpressions;

namespace Utakotoha.Model
{
    [DataContract]
    public class Song
    {
        private static SearchWord LyricSite = new SearchWord("http://music.goo.ne.jp/lyric", SearchLogicalOp.And, SearchTarget.Site);
        private static SearchWord InAnchor = new SearchWord("index.html", SearchLogicalOp.And, SearchTarget.InAnchor);
        private static SearchWord Location = new SearchWord("jp", SearchLogicalOp.And, SearchTarget.Location);
        private static SearchWord Language = new SearchWord("ja", SearchLogicalOp.And, SearchTarget.Language);

        [DataMember]
        public string Artist { get; private set; }
        [DataMember]
        public string Title { get; private set; }

        public Song(string artist, string title)
        {
            Artist = artist;
            Title = title;
        }

        public override string ToString()
        {
            return Title + " - " + Artist;
        }

        private SearchWord MakeWord(string keyword, SearchTarget target = SearchTarget.All)
        {
            return new SearchWord(keyword, SearchLogicalOp.And, target);
        }

        private void Clean(SearchWebResult result)
        {
            result.Title = Regex.Replace(result.Title, "歌詞情報.+$", "").Trim();
            result.Description = Regex.Replace(result.Description, "^.+の歌詞:", "").Trim();
        }

        // not put inanchor for query. inanchor down to precision.
        public IObservable<SearchWebResult> SearchLyric()
        {
            return new BingRequest()
                .Search(MakeWord(Artist), MakeWord(Title), LyricSite, Location, Language)
                .Where(sr => sr.Url.EndsWith("index.html"))
                .Do(Clean);
        }

        public IObservable<SearchWebResult> SearchFromArtist()
        {
            return new BingRequest()
                .Search(MakeWord(Artist), LyricSite, Location, Language)
                .Where(sr => sr.Url.EndsWith("index.html"))
                .Do(Clean);
        }

        public IObservable<SearchWebResult> SearchFromTitle()
        {
            return new BingRequest()
                .Search(MakeWord(Title), LyricSite, Location, Language)
                .Where(sr => sr.Url.EndsWith("index.html"))
                .Do(Clean);
        }
    }
}