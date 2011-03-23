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
        }

        IDisposable activeChanged = Disposable.Empty;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            activeChanged = Observable.Merge(
                    MediaPlayerWatcher.PlayingSongChanged(),
                    MediaPlayerWatcher.PlayingSongActive())
                .Where(_ => Settings.Load().IsAutoSearchWhenMusicChanged)
                .SelectMany(s => s.SearchLyric(), (song, searchresult) => new { song, searchresult })
                .ObserveOnDispatcher()
                .Subscribe(a =>
                {
                    IsolatedStorageSettings.ApplicationSettings[Key.PlayingSong] = a.song;
                    IsolatedStorageSettings.ApplicationSettings[Key.SongSearchResult] = a.searchresult;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                    NavigationService.Navigate(new Uri("/View/LyricBrowse.xaml", UriKind.Relative));
                });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            activeChanged.Dispose();
        }

        // TODO:test
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var song = new Song { Artist = "吉幾三", Title = "俺ら東京さ行ぐだ" };
            var search = new SearchResult("俺ら東京さ行ぐだ", "http://music.goo.ne.jp/lyric/LYRUTND1127/index.html");

            IsolatedStorageSettings.ApplicationSettings[Key.PlayingSong] = song;
            IsolatedStorageSettings.ApplicationSettings[Key.SongSearchResult] = search;
            IsolatedStorageSettings.ApplicationSettings.Save();
            NavigationService.Navigate(new Uri("/View/LyricBrowse.xaml", UriKind.Relative));
        }
    }
}