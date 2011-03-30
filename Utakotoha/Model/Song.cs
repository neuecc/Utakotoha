#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
using System;
using System.Linq;
using System.Runtime.Serialization;
using Utakotoha.Model.Bing;

namespace Utakotoha.Model
{
    [DataContract]
    public class Song
    {
        private static SearchWord LyricSite = new SearchWord("http://music.goo.ne.jp/lyric", SearchLogicalOp.And, SearchTarget.Site);
        private static SearchWord InAnchor = new SearchWord("index.html", SearchLogicalOp.And, SearchTarget.InAnchor);

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

        // not put inanchor for query. inanchor down to precision.
        public IObservable<SearchWebResult> SearchLyric()
        {
            return new BingRequest()
                .Search(MakeWord(Artist), MakeWord(Title), LyricSite)
                .Where(sr => sr.Title.Contains(Artist)
                          && sr.Title.Contains(Title)
                          && sr.Url.EndsWith("index.html"));
        }

        public IObservable<SearchWebResult> SearchFromArtist()
        {
            return new BingRequest()
                .Search(MakeWord(Artist), InAnchor, LyricSite)
                .Where(sr => sr.Title.Contains(Artist)
                          && sr.Url.EndsWith("index.html"));
        }

        public IObservable<SearchWebResult> SearchFromTitle()
        {
            return new BingRequest()
                .Search(MakeWord(Title), InAnchor, LyricSite)
                .Where(sr => sr.Title.Contains(Title)
                          && sr.Url.EndsWith("index.html"));
        }
    }
}