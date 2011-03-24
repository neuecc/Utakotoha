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
    public class MediaPlayerWatcherTest
    {
        private Microsoft.Xna.Framework.Media.Song CreateSong(string artist, string name)
        {
            return new Microsoft.Xna.Framework.Media.Moles.MSong
            {
                NameGet = () => name,
                ArtistGet = () => new MArtist
                {
                    NameGet = () => artist
                }
            };
        }

        [TestMethod, HostType("Moles")]
        public void PlayingSongActiveTest()
        {
            var fire = new Subject<MediaPlayerWatcher.Status>();
            MMediaPlayerWatcher.MediaStateChanged = () => fire;

            // make behavior subject
            var target = MediaPlayerWatcher.PlayingSongActive().Publish(default(Song));
            target.Connect();

            // at first, stopped
            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Stopped, ActiveSong = null });
            target.First().IsNull();

            // next, playing
            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Playing, ActiveSong = CreateSong("artist", "song") });
            var playing = target.First();
            playing.Is(s => s.Title == "song" && s.Artist == "artist");

            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Stopped, ActiveSong = null });
            target.First().IsSameReferenceAs(playing); // no called

            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Playing, ActiveSong = CreateSong("artist2", "song2") });
            target.First().Is(s => s.Title == "song2" && s.Artist == "artist2");
        }
    }
}