using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Microsoft.Phone.Reactive;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Media;
using System.Reflection;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Utakotoha
{
    public partial class MainPage : PhoneApplicationPage
    {


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ApplicationBar = CommonApplicationBar.Create();





            // Global Timer Event ←のやり方がよくわからないのであとで調べる
            Observable.FromEvent<EventArgs>(h => MediaPlayer.ActiveSongChanged += h, h => MediaPlayer.ActiveSongChanged -= h)
               .Throttle(TimeSpan.FromSeconds(2)) // wait
               .StartWith(default(IEvent<EventArgs>))
               .Select(_ => (MediaPlayer.State == MediaState.Playing && MediaPlayer.Queue.ActiveSong != null)
                       ? new Song { Artist = MediaPlayer.Queue.ActiveSong.Artist.Name, Title = MediaPlayer.Queue.ActiveSong.Name }
                       : null)
               .Where(s => s != null)
               .Where(s =>
               {
                   //TODO
                   // Settings.Load()
                   return true;
               })
               .SelectMany(s => s.SearchLyric(), (song, searchresult) => new { song, searchresult })
               .ObserveOnDispatcher()
               .Subscribe(a =>
               {
                   IsolatedStorageSettings.ApplicationSettings["__lastsong"] = a.song;
                   IsolatedStorageSettings.ApplicationSettings["__lastsearch"] = a.searchresult;

                   NavigationService.Navigate(new Uri("/View/LyricBrowse.xaml?song=__lastsong&url=__lastsearch", UriKind.Relative));



               }, e => { }, () => { });








        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var song = new Song { Artist = "吉幾三", Title = "俺ら東京さ行ぐだ" };
            var search = new SearchResult("俺ら東京さ行ぐだ", "http://music.goo.ne.jp/lyric/LYRUTND1127/index.html");

            IsolatedStorageSettings.ApplicationSettings["__lastsong"] = song;
            IsolatedStorageSettings.ApplicationSettings["__lastsearch"] = search;

            NavigationService.Navigate(new Uri("/View/LyricBrowse.xaml?song=__lastsong&url=__lastsearch", UriKind.Relative));
        }


    }
}