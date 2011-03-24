using System;
using System.Linq;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace Utakotoha
{
    public class Song
    {
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";

        public string Artist { get; private set; }
        public string Title { get; private set; }

        public Song(string artist, string title)
        {
            Artist = artist;
            Title = title;
        }

        public IObservable<SearchResult> SearchLyric()
        {
            return new GoogleRequest { Num = 10, Site = GooLyricUri }
                .Search(Artist, Title)
                .Where(sr => sr.Title.Contains(Artist)
                          && sr.Title.Contains(Title)
                          && sr.Url.EndsWith("index.html"));
        }

        public IObservable<SearchResult> SearchFromArtist()
        {
            return new GoogleRequest { Num = 10, Site = GooLyricUri }
                .Search(Artist)
                .Where(sr => sr.Title.Contains(Artist)
                          && sr.Url.EndsWith("index.html"));
        }

        public IObservable<SearchResult> SearchFromTitle()
        {
            return new GoogleRequest { Num = 10, Site = GooLyricUri }
                .Search(Title)
                .Where(sr => sr.Title.Contains(Title)
                          && sr.Url.EndsWith("index.html"));
        }
    }
}