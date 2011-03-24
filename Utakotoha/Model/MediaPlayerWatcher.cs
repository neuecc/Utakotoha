using System;
using System.Linq;
using Microsoft.Xna.Framework.Media;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace Utakotoha
{
    using ActiveSong = Microsoft.Xna.Framework.Media.Song;

    public static class MediaPlayerWatcher
    {
        public class Status
        {
            public MediaState MediaState { get; set; }
            public ActiveSong ActiveSong { get; set; }

            public static Status FromCurrent()
            {
                return new Status
                {
                    MediaState = MediaPlayer.State,
                    ActiveSong = MediaPlayer.Queue.ActiveSong
                };
            }
        }

        private static IObservable<Status> ActiveSongChanged()
        {
            return Observable.FromEvent<EventArgs>(
                    h => MediaPlayer.ActiveSongChanged += h, h => MediaPlayer.ActiveSongChanged -= h)
                .Select(_ => Status.FromCurrent());
        }

        private static IObservable<Status> MediaStateChanged()
        {
            return Observable.FromEvent<EventArgs>(
                    h => MediaPlayer.MediaStateChanged += h, h => MediaPlayer.MediaStateChanged -= h)
                .Select(_ => Status.FromCurrent());
        }

        /// <summary>raise when ActiveSongChanged and MediaState is Playing</summary>
        public static IObservable<Song> PlayingSongChanged()
        {
            return ActiveSongChanged()
                .Where(s => s.MediaState == MediaState.Playing)
                .Throttle(TimeSpan.FromSeconds(2)) // wait for seeking
                .Select(s => new Song(s.ActiveSong.Artist.Name, s.ActiveSong.Name));
        }

        /// <summary>raise when MediaState Pause/Stopped -> Playing</summary>
        public static IObservable<Song> PlayingSongActive()
        {
            return MediaStateChanged()
                .Zip(MediaStateChanged().Skip(1), (prev, curr) => new { prev, curr })
                .Where(a => (a.prev.MediaState == MediaState.Paused || a.prev.MediaState == MediaState.Stopped)
                    && a.curr.MediaState == MediaState.Playing)
                .Select(s => new Song(s.curr.ActiveSong.Artist.Name, s.curr.ActiveSong.Name));
        }
    }
}