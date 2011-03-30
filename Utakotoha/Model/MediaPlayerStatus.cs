using System;
using System.Linq;
using Microsoft.Xna.Framework.Media;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Concurrency;
#endif

namespace Utakotoha.Model
{
    using ActiveSong = Microsoft.Xna.Framework.Media.Song;

    public class MediaPlayerStatus
    {
        public MediaState MediaState { get; set; }
        public ActiveSong ActiveSong { get; set; }

        public static MediaPlayerStatus FromCurrent()
        {
            return new MediaPlayerStatus
            {
                MediaState = MediaPlayer.State,
                ActiveSong = MediaPlayer.Queue.ActiveSong
            };
        }

        public static IObservable<MediaPlayerStatus> ActiveSongChanged()
        {
            return Observable.FromEvent<EventArgs>(
                    h => MediaPlayer.ActiveSongChanged += h, h => MediaPlayer.ActiveSongChanged -= h)
                .Select(_ => MediaPlayerStatus.FromCurrent());
        }

        public static IObservable<MediaPlayerStatus> MediaStateChanged()
        {
            return Observable.FromEvent<EventArgs>(
                    h => MediaPlayer.MediaStateChanged += h, h => MediaPlayer.MediaStateChanged -= h)
                .Select(_ => MediaPlayerStatus.FromCurrent());
        }

        /// <summary>raise when ActiveSongChanged and MediaState is Playing</summary>
        public static IObservable<Song> PlayingSongChanged(int waitSeconds = 2, IScheduler scheduler = null)
        {
            return ActiveSongChanged()
                .Where(s => s.MediaState == MediaState.Playing)
                .Throttle(TimeSpan.FromSeconds(waitSeconds), scheduler ?? Scheduler.ThreadPool) // wait for seeking
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