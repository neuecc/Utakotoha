using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Reactive;

namespace Utakotoha
{
    public static class SongChangedWatcher
    {
        /// <summary>raise when ActiveSongChanged and MediaState is Playing</summary>
        public static IObservable<Song> PlayingSongChanged()
        {
            return Observable.FromEvent<EventArgs>(h => MediaPlayer.ActiveSongChanged += h, h => MediaPlayer.ActiveSongChanged -= h)
                .Throttle(TimeSpan.FromSeconds(2)) // wait for seeking
                .Where(_ => MediaPlayer.State == MediaState.Playing)
                .Select(s =>
                {
                    var song = MediaPlayer.Queue.ActiveSong;
                    return new Song { Artist = song.Artist.Name, Title = song.Name };
                });
        }

        /// <summary>raise when MediaState Pause/Stopped -> Playing</summary>
        public static IObservable<Song> PlayingSongActive()
        {
            var stateChanged = Observable.FromEvent<EventArgs>(
                    h => MediaPlayer.MediaStateChanged += h, h => MediaPlayer.MediaStateChanged -= h)
                .Select(_ => MediaPlayer.State);

            return stateChanged.Zip(stateChanged.Skip(1), (prev, curr) => new { prev, curr })
                .Where(a => (a.prev == MediaState.Paused || a.prev == MediaState.Stopped) && a.curr == MediaState.Playing)
                .Select(s =>
                {
                    var song = MediaPlayer.Queue.ActiveSong;
                    return new Song { Artist = song.Artist.Name, Title = song.Name };
                });
        }
    }
}