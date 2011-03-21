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
    public class SongTest
    {
        [TestMethod]
        [Timeout(3000)]
        public void SearchLyric()
        {
            var song = new Song { Artist = "吉幾三", Title = "俺ら東京さ行ぐだ" };
            var array = song.SearchLyric().ToEnumerable().ToArray();

            array.Count().Is(1);
            array.First().Title.Is("俺ら東京さ行ぐだ 吉幾三 歌詞情報 - goo 音楽");
            array.First().Url.Is("http://music.goo.ne.jp/lyric/LYRUTND1127/index.html");
        }
    }
}
