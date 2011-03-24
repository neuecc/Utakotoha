using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.Moles;
using Utakotoha.Model.Moles;

namespace Utakotoha.Model.Test
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

        [TestMethod, HostType("Moles")]
        public void PlayingSongChangedTest()
        {
            var fire = new Subject<MediaPlayerWatcher.Status>();
            MMediaPlayerWatcher.ActiveSongChanged = () => fire;

            // make behavior subject
            var target = MediaPlayerWatcher.PlayingSongChanged(0).Publish(default(Song));
            target.Connect();

            // at first, stopped
            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Playing, ActiveSong = CreateSong("artist1", "song1") });
            Thread.Sleep(100);
            target.First().Is(s => s.Title == "song1" && s.Artist == "artist1");

            // next, playing
            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Playing, ActiveSong = CreateSong("artist2", "song2") });
            Thread.Sleep(100);
            var playing = target.First();
            playing.Is(s => s.Title == "song2" && s.Artist == "artist2");

            fire.OnNext(new MediaPlayerWatcher.Status { MediaState = MediaState.Paused, ActiveSong = CreateSong("artist3", "song3") });
            Thread.Sleep(100);
            target.Delay(TimeSpan.FromMilliseconds(10)).First().IsSameReferenceAs(playing); // not change
        }
    }
}