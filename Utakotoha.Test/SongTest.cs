using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Xml.Linq;
using Utakotoha.Model.Bing;

namespace Utakotoha.Model.Test
{
    [TestClass]
    public class SongTest
    {
        [TestMethod]
        [Timeout(3000)]
        public void SearchLyric()
        {
            var song = new Song("吉幾三", "俺ら東京さ行ぐだ");
            var array = song.SearchLyric().ToEnumerable().ToArray();

            array.Count().Is(1);
            array.First().Title.Is("俺ら東京さ行ぐだ 吉幾三");
            array.First().Url.Is("http://music.goo.ne.jp/lyric/LYRUTND1127/index.html");
        }

        [TestMethod]
        [Timeout(3000)]
        public void SearchFromArtist()
        {
            var array = new Song("吉幾三", "").SearchFromArtist().ToEnumerable().ToArray();

            array.Count().Is(i => i > 1);
            array.All(sr => sr.Title.Contains("吉幾三")).Is(true);
            array.All(sr => sr.Url.Contains("http://music.goo.ne.jp/lyric/") && sr.Url.EndsWith("index.html")).Is(true);
        }

        [TestMethod]
        [Timeout(3000)]
        public void SearchFromTitle()
        {
            var array = new Song("", "花").SearchFromTitle().ToEnumerable().ToArray();

            array.Count().Is(i => i > 1);
            array.Select(sr => sr.Title).All(s => s.Contains("花")).Is(true);
            array.Select(sr => sr.Url).All(s => s.Contains("http://music.goo.ne.jp/lyric/") && s.EndsWith("index.html")).Is(true);
        }
    }
}
