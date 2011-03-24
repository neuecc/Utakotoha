#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Utakotoha.Model
{
    [DataContract]
    public class Song
    {
        const string GooLyricUri = "http://music.goo.ne.jp/lyric";

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