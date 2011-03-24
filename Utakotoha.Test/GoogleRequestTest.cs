using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using Sgml;
using System.Xml.Linq;
using Utakotoha.Moles;
using Microsoft.Xna.Framework.Media.Moles;
using Microsoft.Xna.Framework.Media;

namespace Utakotoha.Test
{
    [TestClass]
    public class GoogleRequestTest
    {
        [TestMethod]
        public void CleanHref()
        {
            (new GoogleRequest().AsDynamic()
                .CleanHref(@"/url?q=http://d.hatena.ne.jp/keyword/hoge&amp;sa=U&amp;ei=wsGHTe-qFIPEvQO_k9HUCA&amp;ved=0CAkQFjAA&amp;usg=AFQjCNGxRTtlNP9hQ6-FEGw4L7TFAQRzVQ")
                as string).Is("http://d.hatena.ne.jp/keyword/hoge");

            (new GoogleRequest().AsDynamic()
                .CleanHref(@"http://d.hatena.ne.jp/keyword/hoge")
                as string).Is("http://d.hatena.ne.jp/keyword/hoge");
        }

        [TestMethod]
        [Timeout(3000)]
        public void Search()
        {
            var r = new GoogleRequest() { Num = 10 }.Search("hoge").ToEnumerable().ToArray();



            r.Count().Is(10);
            r.Any(x => x.Title == "メタ構文変数 - Wikipedia").Is(true);
            r.All(x => x.Url.StartsWith("http")).Is(true);
        }

        [TestMethod, HostType("Moles")]
        public void MolesTest()
        {
            // TODO:Test....

            EventHandler<EventArgs> mediaStateChanged = (s, e) => { };
            MMediaPlayer.MediaStateChangedAddEventHandlerOfEventArgs =
                 h => mediaStateChanged += h;
            
            MMediaPlayer.QueueGet = () => new MMediaQueue()
            {
                ActiveSongGet = () => new Microsoft.Xna.Framework.Media.Moles.MSong
                {
                    NameGet = () => "SongName",
                    ArtistGet = () => new MArtist
                    {
                        NameGet = () => "ArtistName"
                    }
                }
            };

            MMediaPlayer.StateGet = () => MediaState.Stopped;
            mediaStateChanged.Invoke(null, null);

            MMediaPlayer.StateGet = () => MediaState.Playing;
            mediaStateChanged.Invoke(null, null);



            // validation submit
            var target = MediaPlayerWatcher.PlayingSongActive()
                .Publish(default(Song));
            target.Connect();

            // at first, stopped
            MMediaPlayer.StateGet = () => MediaState.Stopped;
            mediaStateChanged.Invoke(null, null);

            target.FirstOrDefault().IsNull();

            // turn on playing
            MMediaPlayer.StateGet = () => MediaState.Playing;
            mediaStateChanged.Invoke(null, null);
            Console.WriteLine(target.First().Title);

        }
    }
}
