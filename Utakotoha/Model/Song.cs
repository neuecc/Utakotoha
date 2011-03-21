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

        public string Artist { get; set; }
        public string Title { get; set; }

        public IObservable<SearchResult> SearchLyric()
        {
            return new GoogleRequest { Num = 10, Site = GooLyricUri }
                .Search(Artist, Title)
                .Where(sr => sr.Title.Contains(Artist)
                          && sr.Title.Contains(Title)
                          && sr.Url.EndsWith("index.html"));
        }
    }
}