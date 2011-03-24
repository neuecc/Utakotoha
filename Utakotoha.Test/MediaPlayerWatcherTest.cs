using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.Moles;
using Utakotoha.Model.Moles;
using System.Concurrency;

namespace Utakotoha.Model.Test
{
    [TestClass]
    public class MediaPlayerWatcherTest
    {
        private MediaPlayerWatcher.Status CreateStatus(MediaState state, string artist, string name)
        {
            return new MediaPlayerWatcher.Status
            {
                MediaState = state,
                ActiveSong = new Microsoft.Xna.Framework.Media.Moles.MSong
                {
                    NameGet = () => name,
                    ArtistGet = () => new MArtist
                    {
                        NameGet = () => artist
                    }
                }
            };
        }

        [TestMethod, HostType("Moles")]
        public void PlayingSongActiveTest()
        {
            // event invoker
            var invoker = new Subject<MediaPlayerWatcher.Status>();
            MMediaPlayerWatcher.MediaStateChanged = () => invoker;

            // make target observable
            var target = MediaPlayerWatcher.PlayingSongActive().Publish();
            target.Connect();

            // at first, stopped
            using (target.Verify(Verifier.AtZero))
            {
                invoker.OnNext(CreateStatus(MediaState.Stopped, "", ""));
            }

            // next, playing
            using (target.Verify(Verifier.AtOnce, song => song.Is(s => s.Title == "song" && s.Artist == "artist")))
            {
                invoker.OnNext(CreateStatus(MediaState.Playing, "artist", "song"));
            }

            // pause
            using (target.Verify(Verifier.AtZero))
            {
                invoker.OnNext(CreateStatus(MediaState.Paused, "", ""));
            }

            // play again
            using (target.Verify(Verifier.AtOnce, song => song.Is(s => s.Title == "song2" && s.Artist == "artist2")))
            {
                invoker.OnNext(CreateStatus(MediaState.Playing, "artist2", "song2"));
            }
        }

        [TestMethod, HostType("Moles")]
        public void PlayingSongChangedTest()
        {
            var invoker = new Subject<MediaPlayerWatcher.Status>();
            MMediaPlayerWatcher.ActiveSongChanged = () => invoker;

            var target = MediaPlayerWatcher.PlayingSongChanged(0, Scheduler.Immediate);

            using (target.Verify(Verifier.AtOnce, song => song.Is(s => s.Title == "song1" && s.Artist == "artist1")))
            {
                invoker.OnNext(CreateStatus(MediaState.Playing, "artist1", "song1"));
            }

            using (target.Verify(Verifier.AtOnce, song => song.Is(s => s.Title == "song2" && s.Artist == "artist2")))
            {
                invoker.OnNext(CreateStatus(MediaState.Playing, "artist2", "song2"));
            }

            using (target.Verify(Verifier.AtZero))
            {
                invoker.OnNext(CreateStatus(MediaState.Paused, "", ""));
            }
        }
    }
}